namespace ProyectoSBooks.Web.Models
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string NombreCia { get; set; }
        public string Direccion { get; set; }
        public string NombrePais { get; set; }
        public int idPais { get; set; }
        public string Telefono { get; set; }
        public int? dni { get; set; }

    }
}
