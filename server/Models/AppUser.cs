using Microsoft.AspNetCore.Identity;

namespace health_app.Models
{
    // Herda IdentityUser<Guid>; Id é propriedade herdada
    public class AppUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
