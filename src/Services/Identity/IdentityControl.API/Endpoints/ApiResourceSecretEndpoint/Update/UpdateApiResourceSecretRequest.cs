using System;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Update
{
    public class UpdateApiResourceSecretRequest
    {
        public string Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}