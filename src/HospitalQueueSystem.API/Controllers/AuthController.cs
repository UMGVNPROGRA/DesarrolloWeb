using Microsoft.AspNetCore.Mvc;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using HospitalQueueSystem.API.Services;
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
                Expira = DateTime.Now.AddMinutes(60)
            };
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // En un proyecto real, usa BCrypt o similar
            return password == passwordHash; // Solo para desarrollo
        }
    }
}