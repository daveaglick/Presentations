namespace Nancy.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Deserializes request bodies in XML format
    /// </summary>
    public class XmlBodyDeserializer : IBodyDeserializer
    {
        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>True if supported, false otherwise</returns>
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/xml", StringComparison.OrdinalIgnoreCase) ||
                   contentMimeType.Equals("text/xml", StringComparison.OrdinalIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.OrdinalIgnoreCase) &&
                   contentMimeType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param>
        /// <param name="bodyStream">Request body stream</param>
        /// <param name="context">Current <see cref="BindingContext"/>.</param>
        /// <returns>Model instance</returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            bodyStream.Position = 0;
            var ser = new XmlSerializer(context.DestinationType);
            return ser.Deserialize(bodyStream);
        }
    }
}