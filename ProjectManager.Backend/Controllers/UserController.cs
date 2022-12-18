using Microsoft.AspNetCore.Mvc;
using ProjectManager.Backend.Entities;
using ProjectManager.Backend.Security;
using ProjectManager.Backend.Apis;

namespace ProjectManager.Backend.Controllers; 

[ApiController]
[Route("users")]
public sealed class UserController : ControllerBase {
    private readonly IUserApi _users;
    private readonly ITokenApi _tokens;
    private readonly ITokenContext _context;

    public UserController(IUserApi users, ITokenApi tokens, ITokenContext context) {
        _users = users;
        _tokens = tokens;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User login) {
        var user = _users.Login(login);
        if (user == null) return Conflict();
        return Ok(new {Token = _tokens.GetValidToken(user.UserId, HttpContext.Connection.RemoteIpAddress?.ToString())});
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User register) {
        var user = _users.Register(register);
        if (user is null) return Conflict();
        return Ok(new {Token = _tokens.GetValidToken(user.UserId, HttpContext.Connection.RemoteIpAddress?.ToString())});
    }

    [Authorized]
    [HttpGet("token")]
    public IActionResult CheckToken() {
        return Ok(new {Valid = true});
    }

    [Authorized]
    [HttpGet("me")]
    public IActionResult GetMe() {
        return GetUser(_context.UserId);
    }

    [Authorized]
    [HttpGet]
    public IActionResult GetUsers() {
        return Ok(_users.GetUsers().Select(user => new User {
            UserId = user.UserId,
            Email = user.Email,
            Username = user.Username
        }));
    }

    [Authorized]
    [HttpGet("{userId}")]
    public IActionResult GetUser(string userId) {
        var user = _users.GetUser(userId);

        if (user is null) return NotFound();
        
        user = new() {
            UserId = user.UserId,
            Email = user.Email,
            Username = user.Username
        };
        return Ok(user);
    }

    [Authorized]
    [HttpPut]
    public IActionResult UpdateUser([FromBody] User user) {
        if (_context.UserId != user.UserId) return Forbid();
        if (!_users.UpdateUser(user)) return BadRequest();
        return Ok();
    }

    [Authorized]
    [HttpDelete]
    public IActionResult DeleteUser() {
        _users.DeleteUser(_context.UserId);
        return Ok();
    }
    
}