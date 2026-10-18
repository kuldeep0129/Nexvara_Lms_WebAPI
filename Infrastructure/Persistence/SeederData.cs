using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public static class SeederData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider;
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = service.GetRequiredService<IConfiguration>();
            foreach (RoleData role in Enum.GetValues(typeof(RoleData)))
            {
                string roleName = role.ToString();
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var identityRole = new IdentityRole(roleName);
                    var roleResult = await roleManager.CreateAsync(identityRole);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                    }
                }
            }
            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];
            var fullName = configuration["SeedAdmin:FullName"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidOperationException("SeedAdmin configuration is missing.");
            }

            var superAdmin = await userManager.FindByEmailAsync(email);

            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(superAdmin, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create SuperAdmin: {errors}");
                }
            }

            var superAdminRole = RoleData.SuperAdmin.ToString();
            if (!await userManager.IsInRoleAsync(superAdmin, superAdminRole))
            {
                var roleResult = await userManager.AddToRoleAsync(superAdmin, superAdminRole);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to assign SuperAdmin role: {errors}");
                }
            }


        }
    }
}
