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

        // Vulnerability: SQL Injection in login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Unsafe raw SQL query
            var query = $"SELECT * FROM Users WHERE Username = '{model.Username}' AND Password = '{model.Password}'";
            var user = _context.Users.FromSqlRaw(query).FirstOrDefault();

            // Vulnerability: Logging sensitive data
            InsecureLogger.LogSensitiveData($"Login attempt: {model.Username}, {model.Password}");

            // Vulnerability: Hardcoded admin check
            if (model.Username == "admin" && model.Password == "P@ssw0rd123")
            {
                HttpContext.Session.SetString("SessionId", "fixed-session-123"); // Vulnerability: Session fixation
                return Ok("Admin login successful");
            }

            if (user != null)
            {
                return Ok("Login successful");
            }
            return Unauthorized();
        }

        // Vulnerability: No input validation, plaintext password storage
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Vulnerability: No password strength check
            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password cannot be empty");
            }

            _context.Users.Add(user); // Stores password in plaintext
            _context.SaveChanges();
            return Ok("User registered");
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
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}