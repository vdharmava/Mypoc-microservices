using Microsoft.EntityFrameworkCore;
using UserService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Vulnerability: Using outdated Newtonsoft.Json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession(); // Enable session for session fixation vulnerability

var app = builder.Build();

// Vulnerability: No HTTPS redirection
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();
