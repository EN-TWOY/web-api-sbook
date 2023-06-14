namespace ProyectoSBooks.Api.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string NombreCia { get; set; }
        public int IdProveedor { get; set; }
        public string NombreCategoria { get; set; }
        public int IdCategoria { get; set; }
        public string umedida { get; set; }
        public decimal PrecioUnidad { get; set; }
        public Int16 UnidadesEnExistencia { get; set; }
    }
}
