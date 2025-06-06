using LifeOrganizer.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Business.DTOs.Auth;
using LifeOrganizer.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeOrganizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        // POST: api/Users/change-password
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var response = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            if (response == null)
                return BadRequest("Current password is incorrect or user not found.");
            return Ok(response);
        }

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized("Invalid credentials");
            return Ok(result);
        }

        // POST: api/Users/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == null)
                return BadRequest("Username or email already exists");
            return Ok(result);
        }
    }
}
