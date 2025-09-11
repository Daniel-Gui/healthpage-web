using health_app.Authorization;
using health_app.Common;
using health_app.Dtos;
using health_app.Models;
using health_app.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace health_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return ErrorResults.BadRequest(this, "Um ou mais campos estão inválidos.",
                           errors: ModelState.Where(k => k.Value!.Errors.Any())
                                             .ToDictionary(k => char.ToLower(k.Key[0]) + k.Key[1..],
                                                           k => k.Value!.Errors.Select(e => e.ErrorMessage).ToArray()));

            // 1) checar e-mail já usado
            var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingByEmail != null)
                return ErrorResults.Conflict(this, "E-mail já cadastrado.",
                           code: "DuplicateEmail",
                           errors: new Dictionary<string, string[]> { { "email", new[] { "E-mail já cadastrado." } } });

            var user = new AppUser
            {
                Id = Guid.NewGuid(), // GUID no código
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                // Agrupa mensagens por campo
                var bag = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                foreach (var e in result.Errors)
                {
                    var field = e.Code switch
                    {
                        "DuplicateEmail" => "email",
                        "InvalidEmail" => "email",
                        "DuplicateUserName" => "userName",
                        "InvalidUserName" => "userName",
                        "PasswordTooShort" or
                        "PasswordRequiresNonAlphanumeric" or
                        "PasswordRequiresDigit" or
                        "PasswordRequiresUpper" or
                        "PasswordRequiresLower" => "password",
                        _ => "general"
                    };

                    // Se já tiver IdentityErrorDescriber em pt-BR, a descrição já vem traduzida.
                    // Caso contrário, e.Description pode estar em inglês.
                    if (!bag.TryGetValue(field, out var list))
                        bag[field] = list = new List<string>();

                    list.Add(e.Description);
                }

                var errors = bag.ToDictionary(k => k.Key, v => v.Value.ToArray());

                // 409 para conflitos (e-mail / username já usados)
                if (result.Errors.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName"))
                    return ErrorResults.Conflict(this, "Dados em conflito.", code: "Duplicate", errors: errors);

                // 400 para demais validações
                return ErrorResults.BadRequest(this, "Um ou mais campos estão inválidos.", errors: errors);
            }

            // Role padrão
            await _userManager.AddToRoleAsync(user, RoleNames.User);
            var roles = await _userManager.GetRolesAsync(user);
            var (token, exp) = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresInSeconds = (long)(exp - DateTime.UtcNow).TotalSeconds,
                User = new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive, Roles = roles }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) 
                return ErrorResults.BadRequest(this, "Um ou mais campos estão inválidos.", 
                    errors: ModelState.Where(k => k.Value!.Errors.Any())
                                      .ToDictionary(k => char.ToLower(k.Key[0]) + k.Key[1..],
                                                    k => k.Value!.Errors.Select(e => e.ErrorMessage).ToArray()));

            AppUser? user = await _userManager.FindByNameAsync(dto.UserNameOrEmail)
                               ?? await _userManager.FindByEmailAsync(dto.UserNameOrEmail);

            if (user == null || !user.IsActive)
                return ErrorResults.Unauthorized(this, "Usuário ou senha inválidos.");

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                return ErrorResults.Unauthorized(this, "Usuário ou senha inválidos.");

            var roles = await _userManager.GetRolesAsync(user);

            var (token, exp) = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresInSeconds = (long)(exp - DateTime.UtcNow).TotalSeconds,
                User = new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive, Roles = roles }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var idStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(idStr))
                return ErrorResults.Unauthorized(this, "Não autorizado.");

            var id = Guid.Parse(idStr);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return ErrorResults.Unauthorized(this, "Não autorizado.");

            var roles = await _userManager.GetRolesAsync(user);


            return Ok(new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, Roles = roles });
        }
    }
}
