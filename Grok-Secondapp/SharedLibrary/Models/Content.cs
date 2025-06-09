namespace SharedLibrary.Models
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // Vulnerability: No XSS sanitization
        public string Genre { get; set; }
        public string StreamUrl { get; set; } // Vulnerability: Potential SSRF
    }
}