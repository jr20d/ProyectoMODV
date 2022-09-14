namespace ProyectoModV.DTO.Respuestas
{
    public class RespuestaListadoPagedDto<T> : RespuestaListado<T>
    {
        public int TotalRegistros { get; set; }
    }
}
