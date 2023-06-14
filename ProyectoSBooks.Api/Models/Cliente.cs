namespace ProyectoSBooks.Api.Models
{
    public class Cliente
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
