using System.ComponentModel.DataAnnotations;

namespace HospitalQueueSystem.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required]
        public string Rol { get; set; } = string.Empty; // Recepcion, Enfermero, Medico
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}