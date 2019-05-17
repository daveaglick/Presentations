﻿namespace Nancy.Owin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Security.Cryptography.X509Certificates;

    using Nancy.IO;
    using Helpers;

    using AppFunc = System.Func<
       System.Collections.Generic.IDictionary<string, object>,
       System.Threading.Tasks.Task>;

    using MidFunc = System.Func<
        System.Func<
            System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>,
        System.Func<
            System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    /// <summary>
    /// Nancy middleware for OWIN.
    /// </summary>
    public static class NancyMiddleware
    {
        /// <summary>
        /// The request environment key
        /// </summary>
        public const string RequestEnvironmentKey = "OWIN_REQUEST_ENVIRONMENT";

        /// <summary>
        /// Use Nancy in an OWIN pipeline
        /// </summary>
        /// <param name="configuration">A delegate to configure the <see cref="NancyOptions"/>.</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc UseNancy(Action<NancyOptions> configuration)
        {
            var options = new NancyOptions();
            configuration(options);
            return UseNancy(options);
        }

        /// <summary>
        /// Use Nancy in an OWIN pipeline
        /// </summary>
        /// <param name="options">An <see cref="NancyOptions"/> to configure the Nancy middleware</param>
        /// <returns>An OWIN middleware delegate.</returns>
        public static MidFunc UseNancy(NancyOptions options = null)
        {
            options = options ?? new NancyOptions();
            options.Bootstrapper.Initialise();
            var engine = options.Bootstrapper.GetEngine();

            return
                next =>
                    environment =>
                    {
                        var owinRequestMethod = Get<string>(environment, "owin.RequestMethod");
                        var owinRequestScheme = Get<string>(environment, "owin.RequestScheme");
                        var owinRequestHeaders = Get<IDictionary<string, string[]>>(environment, "owin.RequestHeaders");
                        var owinRequestPathBase = Get<string>(environment, "owin.RequestPathBase");
                        var owinRequestPath = Get<string>(environment, "owin.RequestPath");
                        var owinRequestQueryString = Get<string>(environment, "owin.RequestQueryString");
                        var owinRequestBody = Get<Stream>(environment, "owin.RequestBody");
                        var owinRequestProtocol = Get<string>(environment, "owin.RequestProtocol");
                        var owinCallCancelled = Get<CancellationToken>(environment, "owin.CallCancelled");
                        var owinRequestHost = GetHeader(owinRequestHeaders, "Host") ?? Dns.GetHostName();

                        byte[] certificate = null;
                        if (options.EnableClientCertificates)
                        {
                            var clientCertificate = Get<X509Certificate>(environment, "ssl.ClientCertificate");
                            certificate = (clientCertificate == null) ? null : clientCertificate.GetRawCertData();
                        }

                        var serverClientIp = Get<string>(environment, "server.RemoteIpAddress");

                        var url = CreateUrl(owinRequestHost, owinRequestScheme, owinRequestPathBase, owinRequestPath, owinRequestQueryString);

                        var nancyRequestStream = new RequestStream(owinRequestBody, ExpectedLength(owinRequestHeaders), StaticConfiguration.DisableRequestStreamSwitching ?? false);

                        var nancyRequest = new Request(
                                owinRequestMethod,
                                url,
                                nancyRequestStream,
                                owinRequestHeaders.ToDictionary(kv => kv.Key, kv => (IEnumerable<string>)kv.Value, StringComparer.OrdinalIgnoreCase),
                                serverClientIp,
                                certificate,
                                owinRequestProtocol);

                        var tcs = new TaskCompletionSource<int>();

                        engine.HandleRequest(
                            nancyRequest,
                            StoreEnvironment(environment),
                            RequestComplete(environment, options.PerformPassThrough, next, tcs),
                            RequestErrored(tcs),
                            owinCallCancelled);

                        return tcs.Task;
                    };
        }

        /// <summary>
        /// Gets a delegate to handle converting a nancy response
        /// to the format required by OWIN and signals that the we are
        /// now complete.
        /// </summary>
        /// <param name="environment">OWIN environment.</param>
        /// <param name="next">The next stage in the OWIN pipeline.</param>
        /// <param name="tcs">The task completion source to signal.</param>
        /// <param name="performPassThrough">A predicate that will allow the caller to determine if the request passes through to the 
        /// next stage in the owin pipeline.</param>
        /// <returns>Delegate</returns>
        private static Action<NancyContext> RequestComplete(
            IDictionary<string, object> environment,
            Func<NancyContext, bool> performPassThrough,
            AppFunc next,
            TaskCompletionSource<int> tcs)
        {
            return context =>
                {
                    var owinResponseHeaders = Get<IDictionary<string, string[]>>(environment, "owin.ResponseHeaders");
                    var owinResponseBody = Get<Stream>(environment, "owin.ResponseBody");

                    var nancyResponse = context.Response;
                    if (!performPassThrough(context))
                    {
                        environment["owin.ResponseStatusCode"] = (int)nancyResponse.StatusCode;

                        if (nancyResponse.ReasonPhrase != null)
                        {
                            environment["owin.ResponseReasonPhrase"] = nancyResponse.ReasonPhrase;
                        }

                        foreach (var responseHeader in nancyResponse.Headers)
                        {
                            owinResponseHeaders[responseHeader.Key] = new[] {responseHeader.Value};
                        }

                        if (!string.IsNullOrWhiteSpace(nancyResponse.ContentType))
                        {
                            owinResponseHeaders["Content-Type"] = new[] {nancyResponse.ContentType};
                        }

                        if (nancyResponse.Cookies != null && nancyResponse.Cookies.Count != 0)
                        {
                            const string setCookieHeaderKey = "Set-Cookie";
                            string[] setCookieHeader = owinResponseHeaders.ContainsKey(setCookieHeaderKey)
                                                           ? owinResponseHeaders[setCookieHeaderKey]
                                                           : new string[0];
                            owinResponseHeaders[setCookieHeaderKey] = setCookieHeader
                                .Concat(nancyResponse.Cookies.Select(cookie => cookie.ToString()))
                                .ToArray();
                        }

                        nancyResponse.Contents(owinResponseBody);
                        tcs.SetResult(0);
                    }
                    else
                    {
                        next(environment).WhenCompleted(t => tcs.SetResult(0), t => tcs.SetException(t.Exception));
                    }

                    context.Dispose();
                };
        }

        /// <summary>
        /// Gets a delegate to handle request errors
        /// </summary>
        /// <param name="tcs">Completion source to signal</param>
        /// <returns>Delegate</returns>
        private static Action<Exception> RequestErrored(TaskCompletionSource<int> tcs)
        {
            return tcs.SetException;
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : default(T);
        }

        private static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            string[] value;
            return headers.TryGetValue(key, out value) && value != null ? string.Join(",", value.ToArray()) : null;
        }

        private static long ExpectedLength(IDictionary<string, string[]> headers)
        {
            var header = GetHeader(headers, "Content-Length");
            if (string.IsNullOrWhiteSpace(header))
                return 0;

            int contentLength;
            return int.TryParse(header, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ? contentLength : 0;
        }

        /// <summary>
        /// Creates the Nancy URL
        /// </summary>
        /// <param name="owinRequestHost">OWIN Hostname</param>
        /// <param name="owinRequestScheme">OWIN Scheme</param>
        /// <param name="owinRequestPathBase">OWIN Base path</param>
        /// <param name="owinRequestPath">OWIN Path</param>
        /// <param name="owinRequestQueryString">OWIN Querystring</param>
        /// <returns></returns>
        private static Url CreateUrl(
            string owinRequestHost,
            string owinRequestScheme,
            string owinRequestPathBase,
            string owinRequestPath,
            string owinRequestQueryString)
        {
            int? port = null;

            var hostnameParts = owinRequestHost.Split(':');
            if (hostnameParts.Length == 2)
            {
                owinRequestHost = hostnameParts[0];

                int tempPort;
                if (int.TryParse(hostnameParts[1], out tempPort))
                {
                    port = tempPort;
                }
            }

            var url = new Url
            {
                Scheme = owinRequestScheme,
                HostName = owinRequestHost,
                Port = port,
                BasePath = owinRequestPathBase,
                Path = owinRequestPath,
                Query = owinRequestQueryString,
            };
            return url;
        }

        /// <summary>
        /// Gets a delegate to store the OWIN environment into the NancyContext
        /// </summary>
        /// <param name="environment">OWIN Environment</param>
        /// <returns>Delegate</returns>
        private static Func<NancyContext, NancyContext> StoreEnvironment(IDictionary<string, object> environment)
        {
            return context =>
            {
                environment["nancy.NancyContext"] = context;
                context.Items[RequestEnvironmentKey] = environment;
                return context;
            };
        }
    }
}
