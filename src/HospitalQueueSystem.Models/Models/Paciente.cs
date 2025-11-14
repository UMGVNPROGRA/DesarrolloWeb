using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalQueueSystem.Models
{
    public class Paciente
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; } = string.Empty;
        
        public DateTime FechaNacimiento { get; set; }
        
        [StringLength(10)]
        public string Genero { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Sintomas { get; set; } = string.Empty;
        
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}