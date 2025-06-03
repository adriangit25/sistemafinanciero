using System;

namespace SF.Models
{
    public class Sueldo
    {
        public int sueldo_id { get; set; }
        public int reg_id { get; set; }
        public decimal sueldo { get; set; }
        public decimal bono { get; set; }
        public decimal total_ingresos { get; set; }
        public decimal deduccion_ap_pers { get; set; }
        public decimal anticipo { get; set; }
        public decimal total_deducciones { get; set; }
        public decimal liquido_recibir { get; set; }
        public DateTime fecha { get; set; }
        // Solo para mostrar en la tabla:
        public string? empleado_nombre { get; set; }
        public string? empleado_usuario { get; set; }

        // Provisiones (calculadas)
        public decimal aporte_patronal { get; set; }
        public decimal fondo_reserva { get; set; }
        public decimal decimo_tercero { get; set; }
        public decimal decimo_cuarto { get; set; }
        public decimal vacaciones { get; set; }
        public decimal total_provisiones { get; set; }
        public bool mostrar_fondo_reserva { get; set; } // Para bloquear celda si es menor a 12 meses
        public int meses_antiguedad { get; set; }
    }
}
