namespace ProyectoModV.DTO.Respuestas
{
    public class RespuestaListado<T> : RespuestaBase
    {
        public int Cantidad { get; set; }
        public List<T> Registros { get; set; }
    }
}
