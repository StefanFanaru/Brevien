using System.Collections.Generic;

namespace Posting.API.Asp.Http
{
    public class HttpEndpointConfiguration
    {
        public string BaseUri { get; set; }
        public bool UseBearerToken { get; set; }
        public IDictionary<string, HttpApiHeader> Headers { get; set; }
        public IDictionary<string, string> Apis { get; set; }
    }
}