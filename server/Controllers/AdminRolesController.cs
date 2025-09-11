using health_app.Authorization;
using health_app.Common;
using health_app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace health_app.Controllers
{
    [Route("api/admin/roles")]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminRolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AdminRolesController(UserManager<AppUser> userManager,
                                    RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public record SetRolesRequest(Guid UserId, string[] Roles);
        public record ChangeRoleRequest(Guid UserId, string Role);

        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> GetUserRoles([FromRoute] Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return ErrorResults.NotFound(this, "Usuário não encontrado.");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.UserName, roles });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] ChangeRoleRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId.ToString());
            if (user == null) return ErrorResults.NotFound(this, "Usuário não encontrado.");

            if (!await _roleManager.RoleExistsAsync(req.Role))
                return ErrorResults.BadRequest(this, $"Role '{req.Role}' não existe.");

            var res = await _userManager.AddToRoleAsync(user, req.Role);
            if (!res.Succeeded) return ErrorResults.BadRequest(this, string.Join("; ", res.Errors.Select(e => e.Description)));

            return Ok(new { message = "Role atribuída.", req.UserId, req.Role });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromBody] ChangeRoleRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId.ToString());
            if (user == null) return ErrorResults.NotFound(this, "Usuário não encontrado.");

            if (req.Role.Equals(RoleNames.Admin, StringComparison.OrdinalIgnoreCase))
            {
                // Evitar tirar o último admin do sistema (regra opcional)
                // Poderia checar se é o último admin aqui
            }

            var res = await _userManager.RemoveFromRoleAsync(user, req.Role);
            if (!res.Succeeded) return ErrorResults.BadRequest(this, string.Join("; ", res.Errors.Select(e => e.Description)));

            return Ok(new { message = "Role removida.", req.UserId, req.Role });
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetRoles([FromBody] SetRolesRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId.ToString());
            if (user == null) return ErrorResults.NotFound(this, "Usuário não encontrado.");

            foreach (var role in req.Roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    return ErrorResults.BadRequest(this, $"Role '{role}' não existe.");

            var current = await _userManager.GetRolesAsync(user);
            var toRemove = current.Except(req.Roles, StringComparer.OrdinalIgnoreCase).ToArray();
            var toAdd = req.Roles.Except(current, StringComparer.OrdinalIgnoreCase).ToArray();

            if (toRemove.Length > 0)
            {
                var r = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!r.Succeeded) return ErrorResults.BadRequest(this, string.Join("; ", r.Errors.Select(e => e.Description)));
            }

            if (toAdd.Length > 0)
            {
                var r = await _userManager.AddToRolesAsync(user, toAdd);
                if (!r.Succeeded) return ErrorResults.BadRequest(this, string.Join("; ", r.Errors.Select(e => e.Description)));
            }

            var final = await _userManager.GetRolesAsync(user);
            return Ok(new { message = "Roles definidas.", user.Id, user.UserName, roles = final });
        }

    }
}
