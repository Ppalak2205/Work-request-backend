using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Work_request.DTOs;
using Work_request.Models;
using Work_request.Services;

namespace Work_request.Controllers;

[ApiController]
[Route("api/auth")]
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return InvalidCredentials();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return InvalidCredentials();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? user.UserName ?? string.Empty,
            Roles = roles
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "A user with this email already exists." });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            FullName = dto.FullName.Trim()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        await _userManager.AddToRoleAsync(user, "User");
        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user, roles);

        return StatusCode(StatusCodes.Status201Created, new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? user.UserName ?? string.Empty,
            Roles = roles
        });
    }

    [HttpGet("admin-status")]
    public async Task<IActionResult> GetAdminStatus()
    {
        var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
        return Ok(new
        {
            adminSeeded = adminUsers.Any(),
            adminCount = adminUsers.Count
        });
    }

    private ObjectResult InvalidCredentials() =>
        Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized",
            detail: "Invalid email or password.");
}
