using System;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint.Update
{
    public class UpdateClientSecretRequest
    {
        public string Description { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}