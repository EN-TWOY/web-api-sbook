namespace ProyectoSBooks.Models {
    public class Producto {
        public int idproducto { get; set; }
        public string descripcion { get; set; }
        public string categoria { get; set; }
        public decimal precio { get; set; }
        public Int16 stock { get; set; } //equivalente al smallint del SQL
        public string umedida { get; set; }
    }
}
