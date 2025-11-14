using HospitalQueueSystem.API.Services;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using HospitalQueueSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalQueueSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null || !VerifyPassword(request.Password, usuario.PasswordHash))
            {
                return Unauthorized("Credenciales inválidas");
            }

            var token = _jwtService.GenerateToken(usuario);

            return new LoginResponse
            {
                Token = token,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol,
                Email = usuario.Email,
                Expira = DateTime.Now.AddMinutes(60)
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(Usuario usuario)
        {
            // Verificar si el email ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                return BadRequest("El email ya está registrado");
            }

            usuario.FechaCreacion = DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(usuario);

            return new LoginResponse
            {
                Token = token,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol,
                Email = usuario.Email,
                Expira = DateTime.Now.AddMinutes(60)
            };
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // En producción, usar BCrypt.Net
            return password == passwordHash;
        }
    }
}