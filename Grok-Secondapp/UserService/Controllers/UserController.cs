using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using SharedLibrary.Utilities;
using System.Linq;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vulnerability: SQL Injection in login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Unsafe raw SQL query
            var query = $"SELECT * FROM Users WHERE Username = '{model.Username}' AND Password = '{model.Password}'";
            var user = _context.Users.FromSqlRaw(query).FirstOrDefault();

            // Vulnerability: Logging sensitive data
            InsecureLogger.LogSensitiveData($"Login: {model.Username}, {model.Password}");

            // Vulnerability: Hardcoded admin bypass
            if (model.Username == "admin" && model.Password == "secret123")
            {
                HttpContext.Session.SetString("SessionId", "fixed-session-123"); // Vulnerability: Session fixation
                return Ok("Admin logged in");
            }

            if (user != null)
            {
                return Ok("Login successful");
            }
            return Unauthorized();
        }

        // Vulnerability: Plaintext password storage, no validation
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Vulnerability: Weak password policy
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password required");
            }

            _context.Users.Add(user); // Stores password in plaintext
            _context.SaveChanges();
            return Ok("Registered");
        }

        // Vulnerability: Insecure direct object reference
        [HttpGet("profile/{id}")]
        public IActionResult GetProfile(int id)
        {
            var user = _context.Users.Find(id); // No authorization check
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user); // Exposes sensitive data (e.g., password)
        }

        // Vulnerability: XSS in profile page
        [HttpGet("profile/view/{username}")]
        public IActionResult ViewProfile(string username)
        {
            // Unsafe HTML output
            return Content($"<html><body>User: {username}</body></html>", "text/html"); // XSS vulnerability
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}