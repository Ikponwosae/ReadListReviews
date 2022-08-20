using Domain.Entities;
using Domain.Enums;
using Domain.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Data.DbContext
{
    public static class DbInitializer
    {
        public static async Task SeedRoleData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            var rolesEnumList = EnumExtension.GetEnumResults<EUserRole>();
            if (rolesEnumList.Any())
            {
                foreach (var item in rolesEnumList)
                {
                    var roleRecord = context.Roles.Where(x => x.Name.Equals(item.Name));
                    if (roleRecord.FirstOrDefault()?.Name == null)
                    {
                        Role role = new()
                        {
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            Name = item.Name,
                        };
                        await roleManager.CreateAsync(role);
                    }
                }

            }
        }

        public static async Task SeedUserData(this IHost host)
        {
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            await context.Database.EnsureCreatedAsync();

            var query = context.Set<User>().AsQueryable();
            var email = "admin@readlist.com";

            var getUser = await query.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserName.Equals(email));

            if (getUser == null)
            {

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "admin",
                    LastName = "admin",
                    Email = email,
                    Status = EUserStatus.Active.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    UserName = email,
                    PhoneNumber = "07036000000",
                    RefreshToken = Guid.NewGuid().ToString(),
                    Role = EUserRole.Admin.ToString(),
                };

                var role = EUserRole.Admin.ToString();
                user.Password = userManager.PasswordHasher.HashPassword(user, "Admin123@");
                await userManager.CreateAsync(user, "Admin123@");
                if (!(await userManager.IsInRoleAsync(user, role)))
                {
                    //var newUser = await query.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserName.Equals(email));
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

    }
}
