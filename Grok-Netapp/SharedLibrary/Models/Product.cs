namespace SharedLibrary.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } // Vulnerability: No sanitization for XSS
        public decimal Price { get; set; }
    }
}