using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalQueueSystem.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }
}
