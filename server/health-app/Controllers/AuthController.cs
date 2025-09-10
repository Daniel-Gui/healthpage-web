using health_app.Authorization;
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
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var user = new AppUser
            {
                Id = Guid.NewGuid(), // GUID no código
                UserName = dto.UserName,
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
                foreach (var err in result.Errors)
                    ModelState.AddModelError(err.Code, err.Description);
                return ValidationProblem(ModelState);
            }

            // Role padrão
            await _userManager.AddToRoleAsync(user, RoleNames.User);

            var (token, exp) = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresInSeconds = (long)(exp - DateTime.UtcNow).TotalSeconds,
                User = new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            AppUser? user = await _userManager.FindByNameAsync(dto.UserNameOrEmail)
                               ?? await _userManager.FindByEmailAsync(dto.UserNameOrEmail);

            if (user == null || !user.IsActive)
                return Unauthorized(new { message = "Usuário ou senha inválidos." });

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                return Unauthorized(new { message = "Usuário ou senha inválidos." });

            var (token, exp) = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresInSeconds = (long)(exp - DateTime.UtcNow).TotalSeconds,
                User = new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var idStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(idStr)) return Unauthorized();

            var id = Guid.Parse(idStr);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return Unauthorized();

            return Ok(new { user.Id, user.UserName, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc });
        }
    }
}
