using HospitalQueueSystem.API.Services;
using HospitalQueueSystem.Models.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// CORS - ACTUALIZADO para permitir Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS - DEBE IR ANTES de Authentication y Authorization
app.UseCors("AllowBlazorApp"); // Usar esta política para desarrollo

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

// Initialize database with sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.EnsureCreated();
        Console.WriteLine("✅ Database initialized successfully");

        // Add sample data if database is empty
        if (!db.Usuarios.Any())
        {
            db.Usuarios.AddRange(
                new HospitalQueueSystem.Models.Usuario
                {
                    Nombre = "Administrador",
                    Email = "admin@hospital.com",
                    PasswordHash = "admin123",
                    Rol = "Recepcion"
                },
                new HospitalQueueSystem.Models.Usuario
                {
                    Nombre = "Dr. Juan Perez",
                    Email = "juan.perez@hospital.com",
                    PasswordHash = "medico123",
                    Rol = "Medico"
                },
                new HospitalQueueSystem.Models.Usuario
                {
                    Nombre = "Enf. Maria Lopez",
                    Email = "maria.lopez@hospital.com",
                    PasswordHash = "enfermero123",
                    Rol = "Enfermero"
                }
            );

            db.Clinicas.AddRange(
                new HospitalQueueSystem.Models.Clinica
                {
                    Nombre = "Medicina General",
                    Descripcion = "Consulta general para pacientes adultos"
                },
                new HospitalQueueSystem.Models.Clinica
                {
                    Nombre = "Pediatría",
                    Descripcion = "Atención especializada para niños"
                },
                new HospitalQueueSystem.Models.Clinica
                {
                    Nombre = "Ginecología",
                    Descripcion = "Salud femenina y control prenatal"
                }
            );

            db.SaveChanges();
            Console.WriteLine("✅ Sample data added successfully");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
    }
}

// Log startup information
Console.WriteLine("🚀 Hospital Queue System API is starting...");
Console.WriteLine($"🔗 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🔗 CORS enabled for: https://localhost:7001, http://localhost:5001");

app.Run();