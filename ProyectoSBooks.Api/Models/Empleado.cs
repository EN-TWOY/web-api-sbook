using System.ComponentModel.DataAnnotations;

namespace ProyectoSBooks.Api.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string ApeEmpleado { get; set; }
        public string NomEmpleado { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FecNac { get; set; }
        public string DirEmpleado { get; set; }
        public int idDistrito { get; set; }
        public string fonoEmpleado { get; set; }
        public int idCargo { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]

        public DateTime FecContrata { get; set; }
    }
}
