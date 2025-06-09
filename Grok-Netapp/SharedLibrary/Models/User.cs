namespace SharedLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Vulnerability: Plaintext password storage
        public string Email { get; set; }
        public string Role { get; set; } // Vulnerability: No validation on role
    }
}