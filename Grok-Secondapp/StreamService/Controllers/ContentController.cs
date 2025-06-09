using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;
using SharedLibrary.Utilities;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace StreamService.Controllers
{
    [ApiController]
    [Route("stream/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vulnerability: Reflected XSS in search
        [HttpGet("search")]
        public IActionResult Search(string query)
        {
            // Unsafe HTML output
            var results = _context.Contents
                .Where(c => c.Title.Contains(query)) // Vulnerability: No sanitization
                .ToList();
            InsecureLogger.LogSensitiveData($"Search: {query}");
            return Content($"<html><body>Results for: {query}</body></html>", "text/html"); // XSS
        }

        // Vulnerability: Insecure deserialization
        [HttpPost("import")]
        public IActionResult ImportContent([FromBody] byte[] serializedData)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter(); // Vulnerable deserialization
                using (var stream = new MemoryStream(serializedData))
                {
                    var content = (Content)formatter.Deserialize(stream);
                    _context.Contents.Add(content);
                    _context.SaveChanges();
                    return Ok("Content imported");
                }
            }
            catch (Exception ex)
            {
                // Vulnerability: Exposing stack trace
                return BadRequest(ex.ToString());
            }
        }

        // Vulnerability: Insecure direct object reference
        [HttpGet("{id}")]
        public IActionResult GetContent(int id)
        {
            var content = _context.Contents.Find(id); // No authorization
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }

        // Vulnerability: SSRF in stream URL
        [HttpGet("stream/{id}")]
        public IActionResult StreamContent(int id)
        {
            var content = _context.Contents.Find(id);
            if (content == null)
            {
                return NotFound();
            }
            // Vulnerability: Unvalidated URL redirect
            return Redirect(content.StreamUrl);
        }
    }
}