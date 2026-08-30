using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApplicationAPI.DBContext;
using RestaurantApplicationAPI.Models;

namespace RestaurantApplicationAPI.Services
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(ApplicationDBContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);

            await SeedPermissionsAsync(context);

            await SeedRolePermissionsAsync(context, roleManager);

            await SeedSuperUserAsync(userManager, roleManager, configuration);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
            {
                new ApplicationRole
                { Name = "Super Admin", Description = "Full access to the entire application." },
                new ApplicationRole
                { Name = "Admin", Description = "Administrative access to the application." },
                new ApplicationRole
                {  Name = "Manager", Description = "Management level access." },
                new ApplicationRole
                { Name = "Staff", Description = "Staff level access." }
            };

            foreach (var role in roles)
            {
                if (string.IsNullOrWhiteSpace(role.Name))
                    continue;

                var existingRole = await roleManager.FindByNameAsync(role.Name);

                if (existingRole == null)
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDBContext context)
        {
            var permissions = new[]
            {
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Restaurant.View",
                    Description = "View restaurants."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Restaurant.Create",
                    Description = "Create restaurants."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Restaurant.Update",
                    Description = "Update restaurants."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Restaurant.Delete",
                    Description = "Delete restaurants."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "User.View",
                    Description = "View users."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "User.Create",
                    Description = "Create users."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "User.Update",
                    Description = "Update users."
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "User.Delete",
                    Description = "Delete users."
                }
            };

            foreach (var permission in permissions)
            {
                var exists = await context.Permissions
                    .AnyAsync(x => x.Name == permission.Name);

                if (!exists)
                {
                    context.Permissions.Add(permission);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePermissionsAsync(ApplicationDBContext context, RoleManager<ApplicationRole> roleManager)
        {
            var superAdmin = await roleManager.FindByNameAsync("Super Admin");
            var admin = await roleManager.FindByNameAsync("Admin");
            var manager = await roleManager.FindByNameAsync("Manager");
            var staff = await roleManager.FindByNameAsync("Staff");

            if (superAdmin == null || admin == null || manager == null || staff == null)
            {
                throw new InvalidOperationException(
                    "Required roles were not found.");
            }

            var permissions = await context.Permissions
                .ToListAsync();

            await AssignAllPermissionsAsync(context, superAdmin, permissions);

            await AssignPermissionsAsync(context, admin,
                permissions.Where(p => p.Name.StartsWith("Restaurant.") || p.Name.StartsWith("User.")).ToList());

            await AssignPermissionsAsync(context, manager,
                permissions.Where(p => p.Name == "Restaurant.View" || p.Name == "Restaurant.Update").ToList());

            await AssignPermissionsAsync(context, staff,
                permissions.Where(p => p.Name == "Restaurant.View").ToList());
        }

        private static async Task AssignAllPermissionsAsync(ApplicationDBContext context, ApplicationRole role, List<Permission> permissions)
        {
            await AssignPermissionsAsync(context, role, permissions);
        }

        private static async Task AssignPermissionsAsync(ApplicationDBContext context, ApplicationRole role, List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                var exists = await context.RolePermissions
                    .AnyAsync(x =>
                        x.RoleId == role.Id &&
                        x.PermissionId == permission.Id);

                if (!exists)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedSuperUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            var username = configuration["SUPERADMIN_USERNAME"];
            var email = configuration["SUPERADMIN_EMAIL"];
            var password = configuration["SUPERADMIN_PASSWORD"];

            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException(
                    "SUPERADMIN_USERNAME environment variable is missing.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException(
                    "SUPERADMIN_EMAIL environment variable is missing.");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "SUPERADMIN_PASSWORD environment variable is missing.");

            var existingUser = await userManager.FindByNameAsync(username);

            if (existingUser != null)
            {
                return;
            }

            var superUser = new ApplicationUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(
                superUser,
                password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e =>
                        $"{e.Code}: {e.Description}"));

                throw new InvalidOperationException(
                    $"Failed to create Super User: {errors}");
            }

            var roleResult = await userManager.AddToRoleAsync(
                superUser,
                "Super Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e =>
                        $"{e.Code}: {e.Description}"));

                throw new InvalidOperationException(
                    $"Failed to assign Super Admin role: {errors}");
            }
        }
    }
}