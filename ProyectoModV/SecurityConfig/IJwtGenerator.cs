using ProyectoModV.Models;

namespace ProyectoModV.SecurityConfig
{
    public interface IJwtGenerator
    {
        string Generar(Usuario usuario, List<string> rol);

        string GetIdUsuarioActual();

        DataUsuarioToken GetUsuarioActual();
    }
}
