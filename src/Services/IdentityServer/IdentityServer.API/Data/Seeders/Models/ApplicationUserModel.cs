using System;

namespace IdentityServer.API.Data.Seeders.Models
{
    public class ApplicationUserModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DisableDate { get; set; }
        public bool AcceptsInformativeEmails { get; set; }
        public bool HasPicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string Email { get; set; }
        public AddressModel Address { get; set; }
    }

    public class AddressModel
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}