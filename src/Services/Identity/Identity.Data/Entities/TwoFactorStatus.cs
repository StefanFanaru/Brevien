using System;

namespace Identity.Data.Entities
{
    public class TwoFactorStatus
    {
        public TwoFactorStatus(string userId)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            DateSent = DateTime.UtcNow;
        }

        private TwoFactorStatus()
        {
        }

        public string UserId { get; private set; }
        public DateTime DateSent { get; private set; }
        public string Id { get; private set; }
    }
}
