namespace ProyectoModV.DTO.Usuarios
{
    public class LoggedUsuarioDto : BaseUsuarioDto
    {
        public string Token { get; set; }
        public string Imagen { get; set; }
    }
}
