using LedgerStock.Application.DTOs.Auth;
using LedgerStock.Application.Interfaces;
using LedgerStock.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LedgerStock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "As senhas não conferem.",
                Errors = new List<string> { "Password and ConfirmPassword must match." }
            });
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Já existe um usuário com este e-mail.",
                Errors = new List<string> { "Email already in use." }
            });
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            UserName = request.Email,
            Email = request.Email
        };

       var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new AuthResponseDto
            {
                Success = false,
                Message = "Não foi possível concluir o cadastro.",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        await _userManager.AddToRoleAsync(user, "Standard");

        var roles = await _userManager.GetRolesAsync(user);

        var (token, expiresAt) = _jwtTokenService.GenerateToken(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            roles
        );

        return Ok(new AuthResponseDto
        {
            Success = true,
            Message = "Usuário cadastrado com sucesso.",
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            }
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new AuthResponseDto
            {
                Success = false,
                Message = "E-mail ou senha inválidos."
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(new AuthResponseDto
            {
                Success = false,
                Message = "E-mail ou senha inválidos."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (token, expiresAt) = _jwtTokenService.GenerateToken(
            user.Id,
            user.Email ?? string.Empty,
            user.FullName,
            roles
        );

        return Ok(new AuthResponseDto
        {
            Success = true,
            Message = "Login realizado com sucesso.",
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserInfoDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        });
    }
}