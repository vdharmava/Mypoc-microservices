using Microsoft.EntityFrameworkCore;
using StreamService.Models;

var builder = WebApplication.CreateBuilder(args);

// Vulnerability: Insecure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Vulnerability: Outdated Newtonsoft.Json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Vulnerability: No HTTPS
app.UseCors("AllowAll"); // Insecure CORS
app.UseAuthorization();
app.MapControllers();

app.Run();