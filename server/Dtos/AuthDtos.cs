using System.ComponentModel.DataAnnotations;

namespace health_app.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "Senha deve ter ao menos 6 caracteres.")]
        public string Password { get; set; } = default!;

        [Compare(nameof(Password), ErrorMessage = "Confirmação de senha não confere.")]
        public string ConfirmPassword { get; set; } = default!;

        [Required(ErrorMessage = "Nome completo é obrigatório.")]
        [MinLength(3, ErrorMessage = "Nome completo deve ter pelo menos 3 caracteres.")]
        public string FullName { get; set; } = default!;
       
         
        //[Required, MinLength(3)] public string UserName { get; set; } = default!;
        
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "E-mail é obrigatório.")]
        public string UserNameOrEmail { get; set; } = default!;
        
        [Required(ErrorMessage = "Senha é obrigatória.")]
        public string Password { get; set; } = default!;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public long ExpiresInSeconds { get; set; }
        public object User { get; set; } = default!;
    }
}
