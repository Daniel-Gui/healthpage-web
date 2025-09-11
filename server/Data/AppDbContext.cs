using health_app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace health_app.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            
            // Tabelas (snake_case por convenção; nomes explícitos p/ clareza)
            b.Entity<AppUser>(e =>
            {
                e.ToTable("users");

                // Id gerado no código ⇒ sem default SQL
                e.Property(x => x.Id)
                    .ValueGeneratedNever(); // garante que o EF não espere geração no banco

                e.Property(x => x.CreatedAtUtc)
                    .HasDefaultValueSql("timezone('utc', now())");

                e.Property(x => x.FullName).HasMaxLength(120);

                e.HasIndex(u => u.UserName).IsUnique();
                e.HasIndex(u => u.Email).IsUnique(false);
            });

            b.Entity<IdentityRole<Guid>>().ToTable("roles");
            b.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            b.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            b.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            b.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
            b.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        }
    }
}
