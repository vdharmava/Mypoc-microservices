using Microsoft.EntityFrameworkCore;
using UserService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Vulnerability: Outdated Newtonsoft.Json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession();

var app = builder.Build();

// Vulnerability: No HTTPS
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();
