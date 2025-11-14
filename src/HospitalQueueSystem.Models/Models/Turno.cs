using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalQueueSystem.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [Required]
        public int ClinicaId { get; set; }

        [Required]
        public int NumeroTurno { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Llamado, EnAtencion, Atendido, Cancelado

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaLlamado { get; set; }

        public DateTime? FechaAtencion { get; set; }

        public int? MedicoId { get; set; }

        public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey("PacienteId")]
        public Paciente? Paciente { get; set; }

        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        [ForeignKey("MedicoId")]
        public Usuario? Medico { get; set; }
    }
}
