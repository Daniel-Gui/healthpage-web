using health_app.Authorization;
using health_app.Models;
using Microsoft.AspNetCore.Identity;

namespace health_app.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // 1) Garantir roles
            foreach (var role in new[] { RoleNames.Admin, RoleNames.Doctor, RoleNames.User })
            {
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole<Guid>(role));
            }

            // 2) (Opcional) Criar admin inicial via config (use user-secrets em dev)
            var adminEmail = config["SeedAdmin:Email"];
            var adminPass = config["SeedAdmin:Password"];
            var adminUser = config["SeedAdmin:UserName"] ?? "admin";
            var adminName = config["SeedAdmin:FullName"] ?? "Site Administrator";

            if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPass))
            {
                var existing = await userMgr.FindByEmailAsync(adminEmail);
                if (existing == null)
                {
                    var admin = new AppUser
                    {
                        Id = Guid.NewGuid(),
                        UserName = adminUser,
                        Email = adminEmail,
                        FullName = adminName,
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var createRes = await userMgr.CreateAsync(admin, adminPass);
                    if (createRes.Succeeded)
                    {
                        await userMgr.AddToRoleAsync(admin, RoleNames.Admin);
                    }
                }
                else
                {
                    // Garante que tem role admin
                    if (!await userMgr.IsInRoleAsync(existing, RoleNames.Admin))
                        await userMgr.AddToRoleAsync(existing, RoleNames.Admin);
                }
            }
        }

    }
}
