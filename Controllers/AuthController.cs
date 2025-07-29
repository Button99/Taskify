using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskify.Data;
using Taskify.Models;
using Taskify.Services;
using Taskify.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly AuthService _authService;
    private readonly TaskifyDbContext _db;


    public AuthController(AuthService authService, TaskifyDbContext db)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        if (await _db.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Username already exists.");

        _authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
            return Unauthorized("Invalid username or password.");
        if (!_authService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid username or password.");
        var token = _authService.CreateToken(user);
        return Ok(new { Token = token });
    }

}
