namespace SF.Models
{
    public class Usuario
    {
        public int reg_id { get; set; }
        public string? reg_usuario { get; set; }
        public string? reg_contrasenia { get; set; }
        public string? reg_nombre { get; set; }
        public string? reg_apellido { get; set; }
        public string? reg_telefono { get; set; }
        public string? reg_estado { get; set; }
        public DateTime? reg_fecha_ingreso { get; set; }
        public DateTime? reg_fecha_actual { get; set; }
        public DateTime? reg_fecha_final { get; set; }
        public string? rol_nombre { get; set; }
    }
}
