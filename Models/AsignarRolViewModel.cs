using System.Collections.Generic;

namespace SF.Models
{
    public class AsignarRolViewModel
    {
        public int reg_id { get; set; }
        public int rol_id { get; set; }
        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<Rol> Roles { get; set; } = new List<Rol>();
    }
}
