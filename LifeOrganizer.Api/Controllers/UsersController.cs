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
        private readonly IGenericService<User> _userService;
        private readonly IAuthService _authService;

        public UsersController(IGenericService<User> userService, IAuthService authService)
        {
            _userService = userService;
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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var users = await _userService.GetAllAsync(userId, cancellationToken);
            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var user = await _userService.GetByIdAsync(id, userId, cancellationToken);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            user.UserId = userId;
            await _userService.AddAsync(user, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, User user, CancellationToken cancellationToken)
        {
            if (id != user.Id)
                return BadRequest("ID in URL and body do not match.");
            var userId = User.GetUserId();
            var existing = await _userService.GetByIdAsync(id, userId, cancellationToken);
            if (existing == null)
                return NotFound();
            user.UserId = userId;
            await _userService.UpdateAsync(user, cancellationToken);
            return NoContent();
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var user = await _userService.GetByIdAsync(id, userId, cancellationToken);
            if (user == null)
                return NotFound();
            await _userService.RemoveAsync(user, cancellationToken);
            return NoContent();
        }
    }
}
