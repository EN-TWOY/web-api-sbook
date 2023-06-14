namespace ProyectoSBooks.Web.Models
{
	public class VentaDetalle
	{
        public int idpedido { get; set; }
        public int idproducto { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
    }
}
