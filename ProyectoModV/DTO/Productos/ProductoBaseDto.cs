namespace ProyectoModV.DTO.Productos
{
    public class ProductoBaseDto
    {
        public string Nombre { get; set; }
        public string Modelo { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public int Cantidad { get; set; }
    }
}
