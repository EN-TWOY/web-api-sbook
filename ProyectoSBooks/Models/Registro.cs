namespace ProyectoSBooks.Models {
    public class Registro {
        public int idproducto { get; set; }
        public string descripcion { get; set; }
        public string categoria { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; }
        public decimal monto { get { return precio * cantidad; } }
    }
}
