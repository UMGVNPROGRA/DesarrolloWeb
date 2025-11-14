using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HospitalQueueSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TurnosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TurnosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnos()
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Clinica)
                .Include(t => t.Medico)
                .ToListAsync();
        }

        [HttpGet("clinica/{clinicaId}")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnosPorClinica(int clinicaId)
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Clinica)
                .Where(t => t.ClinicaId == clinicaId &&
                           (t.Estado == "Pendiente" || t.Estado == "Llamado" || t.Estado == "EnAtencion"))
                .OrderBy(t => t.NumeroTurno)
                .ToListAsync();
        }

        [HttpGet("display")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetTurnosDisplay()
        {
            var turnos = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Clinica)
                .Where(t => t.Estado == "Llamado" || t.Estado == "EnAtencion")
                .Select(t => new
                {
                    ClinicaNombre = t.Clinica.Nombre,
                    t.NumeroTurno,
                    PacienteNombre = t.Paciente.Nombre,
                    t.Estado
                })
                .ToListAsync();

            return Ok(turnos);
        }

        [HttpPost]
        public async Task<ActionResult<Turno>> PostTurno(Turno turno)
        {
            // Obtener el último número de turno para esta clínica
            var ultimoTurno = await _context.Turnos
                .Where(t => t.ClinicaId == turno.ClinicaId)
                .OrderByDescending(t => t.NumeroTurno)
                .FirstOrDefaultAsync();

            turno.NumeroTurno = ultimoTurno?.NumeroTurno + 1 ?? 1;
            turno.Estado = "Pendiente";
            turno.FechaCreacion = DateTime.UtcNow;

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTurno", new { id = turno.Id }, turno);
        }

        [HttpPost("{id}/llamar")]
        [Authorize(Roles = "Medico,Enfermero")]
        public async Task<IActionResult> LlamarTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            turno.Estado = "Llamado";
            turno.FechaLlamado = DateTime.UtcNow;

            var medicoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(medicoId, out int idMedico))
                turno.MedicoId = idMedico;

            await _context.SaveChangesAsync();
            return Ok(turno);
        }

        [HttpPost("{id}/atender")]
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> AtenderTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            turno.Estado = "EnAtencion";
            turno.FechaAtencion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(turno);
        }

        [HttpPost("{id}/completar")]
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> CompletarTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            turno.Estado = "Atendido";
            await _context.SaveChangesAsync();

            return Ok(turno);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Turno>> GetTurno(int id)
        {
            var turno = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Clinica)
                .Include(t => t.Medico)
                .FirstOrDefaultAsync(t => t.Id == id);

            return turno == null ? NotFound() : turno;
        }
    }
}