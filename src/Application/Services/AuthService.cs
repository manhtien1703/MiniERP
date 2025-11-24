using System;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.Exceptions;

namespace Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtTokenService _jwtService;

    public AuthService(IUserRepository userRepo, IJwtTokenService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<(string Token, string FullName, DateTime ExpiresAt)> LoginAsync(string username, string password)
    {
        var user = await _userRepo.GetByUsernameAsync(username);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        // Verify password (using BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        // Update last login time
        await _userRepo.UpdateLastLoginAsync(user.Id);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        var expiresAt = _jwtService.GetTokenExpiration();

        return (token, user.FullName, expiresAt);
    }
}

