using System.ComponentModel.DataAnnotations;

namespace health_app.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required, MinLength(3)] public string UserName { get; set; } = default!;
        [Required, MinLength(6)] public string Password { get; set; } = default!;
        [Required, Compare(nameof(Password))] public string ConfirmPassword { get; set; } = default!;
        [Required, MinLength(3)] public string FullName { get; set; } = default!;
    }

    public class LoginDto
    {
        [Required] public string UserNameOrEmail { get; set; } = default!;
        [Required] public string Password { get; set; } = default!;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public long ExpiresInSeconds { get; set; }
        public object User { get; set; } = default!;
    }
}
