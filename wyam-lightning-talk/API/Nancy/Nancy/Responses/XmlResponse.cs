﻿namespace Nancy.Responses
{
    using System;
    using System.IO;

    using Nancy.Xml;

    public class XmlResponse<TModel> : Response
    {
        [Obsolete("This constructor is obsolete and will be removed in a future version.")]
        public XmlResponse(TModel model, string contentType, ISerializer serializer) : this(model, serializer)
        {
            this.ContentType = contentType;
        }

        public XmlResponse(TModel model, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("XML Serializer not set");
            }

            this.Contents = GetXmlContents(model, serializer);
            this.ContentType = DefaultContentType;
            this.StatusCode = HttpStatusCode.OK;
        }

        private static string DefaultContentType
        {
            get { return string.Concat("application/xml", Encoding); }
        }

        private static string Encoding
        {
            get
            {
                return XmlSettings.EncodingEnabled
                    ? string.Concat("; charset=", XmlSettings.DefaultEncoding.WebName)
                    : string.Empty;
            }
        }

        private static Action<Stream> GetXmlContents(TModel model, ISerializer serializer)
        {
            return stream => serializer.Serialize(DefaultContentType, model, stream);
        }
    }
}