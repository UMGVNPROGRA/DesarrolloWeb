using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using Microsoft.AspNetCore.Authorization;

namespace HospitalQueueSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Clinicas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinica>>> GetClinicas()
        {
            return await _context.Clinicas.Where(c => c.Activa).ToListAsync();
        }

        // GET: api/Clinicas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clinica>> GetClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);

            if (clinica == null)
            {
                return NotFound();
            }

            return clinica;
        }

        // GET: api/Clinicas/activas
        [HttpGet("activas")]
        [AllowAnonymous] // Para que el display pueda acceder sin auth
        public async Task<ActionResult<IEnumerable<Clinica>>> GetClinicasActivas()
        {
            return await _context.Clinicas.Where(c => c.Activa).ToListAsync();
        }

        // POST: api/Clinicas
        [HttpPost]
        [Authorize(Roles = "Recepcion,Enfermero")] // Solo personal autorizado puede crear clínicas
        public async Task<ActionResult<Clinica>> PostClinica(Clinica clinica)
        {
            // Asegurar que se cree como activa
            clinica.Activa = true;
            
            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinica", new { id = clinica.Id }, clinica);
        }

        // PUT: api/Clinicas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Recepcion,Enfermero")]
        public async Task<IActionResult> PutClinica(int id, Clinica clinica)
        {
            if (id != clinica.Id)
            {
                return BadRequest();
            }

            _context.Entry(clinica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Clinicas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Recepcion")] // Solo recepción puede eliminar clínicas
        public async Task<IActionResult> DeleteClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null)
            {
                return NotFound();
            }

            // En lugar de eliminar, la marcamos como inactiva
            clinica.Activa = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Clinicas/5/activar
        [HttpPatch("{id}/activar")]
        [Authorize(Roles = "Recepcion")]
        public async Task<IActionResult> ActivarClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null)
            {
                return NotFound();
            }

            clinica.Activa = true;
            await _context.SaveChangesAsync();

            return Ok(clinica);
        }

        // PATCH: api/Clinicas/5/desactivar
        [HttpPatch("{id}/desactivar")]
        [Authorize(Roles = "Recepcion")]
        public async Task<IActionResult> DesactivarClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            if (clinica == null)
            {
                return NotFound();
            }

            clinica.Activa = false;
            await _context.SaveChangesAsync();

            return Ok(clinica);
        }

        // GET: api/Clinicas/5/turnos
        [HttpGet("{id}/turnos")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnosPorClinica(int id)
        {
            var turnos = await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Where(t => t.ClinicaId == id && t.Estado != "Atendido" && t.Estado != "Cancelado")
                .OrderBy(t => t.NumeroTurno)
                .ToListAsync();

            return turnos;
        }

        // GET: api/Clinicas/5/estadisticas
        [HttpGet("{id}/estadisticas")]
        [Authorize(Roles = "Recepcion,Enfermero,Medico")]
        public async Task<ActionResult<object>> GetEstadisticasClinica(int id)
        {
            var hoy = DateTime.UtcNow.Date;
            
            var estadisticas = new
            {
                TotalTurnosHoy = await _context.Turnos
                    .Where(t => t.ClinicaId == id && t.FechaCreacion.Date == hoy)
                    .CountAsync(),
                
                Pendientes = await _context.Turnos
                    .Where(t => t.ClinicaId == id && t.Estado == "Pendiente" && t.FechaCreacion.Date == hoy)
                    .CountAsync(),
                
                EnAtencion = await _context.Turnos
                    .Where(t => t.ClinicaId == id && t.Estado == "EnAtencion" && t.FechaCreacion.Date == hoy)
                    .CountAsync(),
                
                Atendidos = await _context.Turnos
                    .Where(t => t.ClinicaId == id && t.Estado == "Atendido" && t.FechaCreacion.Date == hoy)
                    .CountAsync(),
                
                TiempoPromedioEspera = await _context.Turnos
                    .Where(t => t.ClinicaId == id && t.Estado == "Atendido" && t.FechaLlamado != null && t.FechaAtencion != null)
                    .AverageAsync(t => (double)(t.FechaAtencion - t.FechaLlamado).Value.TotalMinutes)
            };

            return estadisticas;
        }

        private bool ClinicaExists(int id)
        {
            return _context.Clinicas.Any(e => e.Id == id);
        }
    }
}