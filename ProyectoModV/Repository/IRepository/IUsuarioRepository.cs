using ProyectoModV.Models;

namespace ProyectoModV.Repository.IRepository
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetUsuarios(string rol);
        Task<Usuario> GetUsuario(string id);
        Task<List<string>> GetRoles(string id);
        Task<Usuario> GetUsuarioByEmail(string email);
        Task<bool> ExistsUsuario(string id);
        Task<bool[]> ExistsUsuario(string email, string telefono);
        Task<bool[]> ExistsUsuario(string id, string email, string telefono);
        Task<Usuario> Login(Usuario usuario, string password);
        Task<bool> CreateUsuario(Usuario usuario, string password, string rol);
        Task<bool> UpdateUsuario(Usuario usuario);
        Task<bool> DeleteUsuario(string id, string rol);
        Task<bool> UpdatePassword(Usuario usuario, string password);
        Task<bool> ValidatePassword(Usuario usuario, string password);
        Task<Usuario> GetImagen(string id);
    }
}
