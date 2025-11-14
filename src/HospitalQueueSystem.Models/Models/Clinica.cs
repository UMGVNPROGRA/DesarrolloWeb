using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalQueueSystem.Models
{
    public class Clinica
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;
    }
}
