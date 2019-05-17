﻿namespace Nancy.Routing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Nancy.Helpers;

    /// <summary>
    /// Route that is returned when the path could be matched but it was for the wrong request method.
    /// </summary>
    /// <remarks>This is equal to sending back the 405 HTTP status code.</remarks>
    public class MethodNotAllowedRoute : Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodNotAllowedRoute"/> type, for the
        /// specified <paramref name="path"/>, <paramref name="method"/> and <paramref name="allowedMethods"/>.
        /// </summary>
        /// <param name="path">The path of the route.</param>
        /// <param name="method">The HTTP method of the route.</param>
        /// <param name="allowedMethods">The HTTP methods that can be used to invoke the route.</param>
        public MethodNotAllowedRoute(string path, string method, IEnumerable<string> allowedMethods)
            : base(method, path, null, (x,c) => CreateMethodNotAllowedResponse(allowedMethods))
        {
        }

        private static Task<dynamic> CreateMethodNotAllowedResponse(IEnumerable<string> allowedMethods)
        {
            var response = new Response();
            response.Headers["Allow"] = string.Join(", ", allowedMethods);
            response.StatusCode = HttpStatusCode.MethodNotAllowed;

            return TaskHelpers.GetCompletedTask<dynamic>(response);
        }
    }
}