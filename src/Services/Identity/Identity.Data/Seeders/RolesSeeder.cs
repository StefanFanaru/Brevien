using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data.Constants;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Identity.Data.Seeders
{
    public class RolesSeeder : ISeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        ///     Seeds all the needed roles
        /// </summary>
        public async Task SeedAsync()
        {
            string[] rolesToSeed =
            {
                Roles.Administrator,
                Roles.BasicUser,
            };

            if (!_roleManager.Roles.Any())
                try
                {
                    await SeedRolesAsync(rolesToSeed);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Roles seeding error.");
                }
            else
                Log.Information("Found user roles in the database. Skipping seeding...");
        }

        private async Task SeedRolesAsync(string[] rolesToSeed)
        {
            foreach (var roleName in rolesToSeed)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                        foreach (var error in result.Errors)
                            Log.Information(error.Description);

                    Log.Information($"Role \"{roleName}\" created.");
                }
            }
        }
    }
}