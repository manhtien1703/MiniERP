using System;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.Responses;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var (token, fullName, expiresAt) = await _authService.LoginAsync(request.Username, request.Password);

            var response = new LoginResponse
            {
                Token = token,
                Username = request.Username,
                FullName = fullName,
                ExpiresAt = expiresAt
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse
            {
                Title = "Login Failed",
                Status = 401,
                Detail = ex.Message
            });
        }
    }
}

