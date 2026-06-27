using GymSys.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedingIdentityDataAsync(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger logger,
            CancellationToken ct = default)
        {
            try
            {
                bool hasRoles = await roleManager.Roles.AnyAsync(ct);
                bool hasUsers = await userManager.Users.AnyAsync(ct);

                if (hasRoles && hasUsers) return;

                var roles = new List<IdentityRole>()
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
            };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed to create role {role.Name} : {string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Hazem",
                        LastName = "Eltahan",
                        Email = "hazemeltahan@gmail.com",
                        UserName = "HazemEltahan",
                        PhoneNumber = "01177777777"
                    };

                    var mainAdminResult = await userManager.CreateAsync(MainAdmin, "Main@123");
                    if (!mainAdminResult.Succeeded)
                    {
                        logger.LogError($"Failed to create user {MainAdmin.UserName} : {string.Join(" ; ", mainAdminResult.Errors.Select(e => e.Description))}");
                    }

                    var mainAdminRoleResult = await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                    if (!mainAdminRoleResult.Succeeded)
                    {
                        logger.LogError($"Failed to add role SuperAdmin to user {MainAdmin.UserName} : {string.Join(" ; ", mainAdminRoleResult.Errors.Select(e => e.Description))}");
                    }

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Leen",
                        LastName = "Abdelhaleem",
                        Email = "leen@gmail.com",
                        UserName = "LeenAbdelhaleem",
                        PhoneNumber = "01157777777"
                    };

                    var adminResult = await userManager.CreateAsync(Admin, "Admin@123");
                    if (!adminResult.Succeeded)
                    {
                        logger.LogError($"Failed to create user {Admin.UserName} : {string.Join(" ; ", adminResult.Errors.Select(e => e.Description))}");
                    }

                    var adminRoleResult = await userManager.AddToRoleAsync(Admin, "Admin");
                    if (!adminRoleResult.Succeeded)
                    {
                        logger.LogError($"Failed to add role Admin to user {Admin.UserName} : {string.Join(" ; ", adminRoleResult.Errors.Select(e => e.Description))}");
                    }

                    logger.LogInformation($"Identity data seeded!");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity seeding failed!");
                return;
            }


        }
    }
}
