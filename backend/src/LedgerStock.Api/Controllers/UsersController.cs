using LedgerStock.Application.DTOs.Users;
using LedgerStock.Domain.Constants;
using LedgerStock.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Master")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserListItemDto>>> GetAll()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserListItemDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserListItemDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        return Ok(result.OrderBy(u => u.FullName).ToList());
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateUserRequestDto request)
    {
        if (!SystemRoles.All.Contains(request.Role))
        {
            return BadRequest(new { message = "Perfil inválido." });
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new { message = "Já existe um usuário com este e-mail." });
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Não foi possível criar o usuário.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        return Ok(new { message = "Usuário criado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound(new { message = "Usuário não encontrado." });
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(SystemRoles.Master))
        {
            return BadRequest(new { message = "Não é permitido remover um usuário Master." });
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Não foi possível remover o usuário.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new { message = "Usuário removido com sucesso." });
    }
}