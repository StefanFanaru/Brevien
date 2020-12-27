using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Data.Constants;
using Identity.Data.Entities;
using Identity.Data.Entities.ValueObjects;
using Identity.Data.Seeders.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Identity.Data.Seeders
{
    public class UsersSeeder : ISeeder
    {
        private const string Path = "Seeders/JSON/applicationUsers.json";
        private static (string email, string password) _adminAccount;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersSeeder(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _adminAccount.email = _configuration["AdminAccount:Email"];
            _adminAccount.password = _configuration["AdminAccount:Password"];
        }

        /// <summary>
        ///     Seeds different types of users
        /// </summary>
        public async Task SeedAsync()
        {
            if (!_userManager.Users.Any())
            {
                var userToSeed = await GetUsersForInsert();
                try
                {
                    await SeedUsersAsync(_userManager, userToSeed);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Users seeding error.");
                }
            }
            else
            {
                Log.Information($"Found {_userManager.Users.Count()} users in the database. Skipping seeding...");
            }
        }

        private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager,
            IEnumerable<UserToSeed> usersToSeed)
        {
            var environment = _configuration.GetSection("Settings:Environment").Value;

            foreach (var userToSeed in usersToSeed)
            {
                var (user, role, claims) = userToSeed.Deconstruct();
                if (environment != "local" && role != Roles.Administrator) continue;
                Log.Debug(_adminAccount.password);
                var result = await userManager.CreateAsync(user, _adminAccount.password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    result = userManager.AddClaimsAsync(user, claims).Result;

                    if (result.Succeeded)
                        Log.Information($"{role} account created.");
                    else
                        throw new Exception(result.Errors.First().Description);
                }
                else
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        private static async Task<List<UserToSeed>> GetUsersForInsert()
        {
            var usersForInsert = new List<UserToSeed>();
            var userData = await File.ReadAllTextAsync(Path);
            var userModels = JsonConvert.DeserializeObject<List<ApplicationUserModel>>(userData);
            var randomUsers = BuildUserEntities(userModels);

            var admin = ApplicationUser.Create(
                _adminAccount.email, "Brevien", "Admin", true);
            admin.EmailConfirmed = true;

            usersForInsert.Add(BuildUserForInsert(admin, Roles.Administrator));
            randomUsers.ForEach(x => usersForInsert.Add(BuildUserForInsert(x, Roles.BasicUser)));
            return usersForInsert;
        }

        private static UserToSeed BuildUserForInsert(ApplicationUser user, string role)
        {
            return new UserToSeed
            {
                User = user,
                Role = role,
                Claims = new List<Claim>
                {
                    new Claim(Claims.FirstName, user.FirstName),
                    new Claim(Claims.LastName, user.LastName),
                    new Claim(Claims.Email, user.Email),
                    new Claim(Claims.Role, role)
                }
            };
        }

        private static List<ApplicationUser> BuildUserEntities(IEnumerable<ApplicationUserModel> models)
        {
            var entities = new List<ApplicationUser>();
            foreach (var model in models)
            {
                var entity = ApplicationUser.Create(model.Email, model.LastName, model.FirstName,
                    model.AcceptsInformativeEmails);

                entity.InsertDate = model.InsertDate;
                entity.DisableDate = model.DisableDate;
                entity.EmailConfirmed = model.EmailConfirmed;
                entity.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                entity.TwoFactorEnabled = model.TwoFactorEnabled;

                var address = new Address(
                    model.Address.Street,
                    model.Address.City,
                    model.Address.County,
                    model.Address.Country,
                    null);
                entity.UpdateAddress(address);

                if (model.HasPicture)
                {
                    entity.UpdatePicture("https://imgur.com/vuYlgUY");
                }

                entities.Add(entity);
            }

            return entities;
        }

        private class UserToSeed
        {
            public ApplicationUser User { get; set; }
            public string Role { get; set; }
            public List<Claim> Claims { get; set; }

            public (ApplicationUser user, string role, List<Claim> claims) Deconstruct()
            {
                return (User, Role, Claims);
            }
        }
    }
}