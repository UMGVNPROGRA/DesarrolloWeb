using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using Microsoft.AspNetCore.Authorization;

namespace HospitalQueueSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinica>>> GetClinicas()
        {
            return await _context.Clinicas.Where(c => c.Activa).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clinica>> GetClinica(int id)
        {
            var clinica = await _context.Clinicas.FindAsync(id);
            return clinica == null ? NotFound() : clinica;
        }

        [HttpPost]
        [Authorize(Roles = "Recepcion")]
        public async Task<ActionResult<Clinica>> PostClinica(Clinica clinica)
        {
            _context.Clinicas.Add(clinica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinica", new { id = clinica.Id }, clinica);
        }
    }
}