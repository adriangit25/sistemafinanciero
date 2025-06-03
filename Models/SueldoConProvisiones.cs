using System;

namespace SF.Models
{
    public class SueldoConProvisiones : Sueldo
    {
        public decimal AportePatronal { get; set; }
        public decimal FondoReserva { get; set; }
        public decimal DecimoTercero { get; set; }
        public decimal DecimoCuarto { get; set; }
        public decimal Vacaciones { get; set; }
        public decimal TotalProvisiones { get; set; }
        
    }
}
