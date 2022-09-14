using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.Uploads;

namespace ProyectoModV.Repository
{
    public class UsuarioRespository : IUsuarioRepository
    {
        private readonly TiendaDbContext context;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPasswordHasher<Usuario> passwordHasher;
        private readonly ICargaArchivo cargaArchivo;

        public UsuarioRespository(TiendaDbContext context, UserManager<Usuario> userManager, 
            SignInManager<Usuario> signInManager, IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager,
            IPasswordHasher<Usuario> passwordHasher, ICargaArchivo cargaArchivo)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            _serviceProvider = serviceProvider;
            this.roleManager = roleManager;
            this.passwordHasher = passwordHasher;
            this.cargaArchivo = cargaArchivo;
        }

        public async Task<bool> CreateUsuario(Usuario usuario, string password, string rol)
        {
            if (await roleManager.FindByNameAsync(rol) == null)
            {
                var respuestaRol = await roleManager.CreateAsync(new IdentityRole(rol));
                if (!respuestaRol.Succeeded)
                {
                    return false;
                }
            }

            await userManager.CreateAsync(usuario, password);
            var resultado = await userManager.AddToRoleAsync(usuario, rol);

            return resultado.Succeeded;
        }

        public async Task<bool> DeleteUsuario(string id, string rol)
        {            
            var usuario = await userManager.FindByIdAsync(id);
            if (!string.IsNullOrEmpty(usuario.PublicId))
            {
                var respuestaArchivo = cargaArchivo.EliminarArchivo(usuario.PublicId);
                if (respuestaArchivo == false)
                {
                    return false;
                }
            }
            await userManager.RemoveFromRoleAsync(usuario, rol);
            var resultado = await userManager.DeleteAsync(usuario);
            return resultado.Succeeded;
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<Usuario> Login(Usuario usuario, string password)
        {
            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, password, false);

            return resultado.Succeeded ? usuario : null;
        }

        public async Task<Usuario> GetUsuario(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<List<Usuario>> GetUsuarios(string rol)
        {
            return (List<Usuario>)await userManager.GetUsersInRoleAsync(rol);
        }

        public async Task<bool> UpdateUsuario(Usuario usuario)
        {
            Usuario usuarioBD = await userManager.FindByIdAsync(usuario.Id);
            usuarioBD.Nombre = usuario.Nombre;
            usuarioBD.PhoneNumber = usuario.PhoneNumber;
            usuarioBD.Email = usuario.Email;
            var resultado = await userManager.UpdateAsync(usuarioBD);
            return resultado.Succeeded;
        }

        public async Task<bool[]> ExistsUsuario(string email, string telefono)
        {
            var transientContext = _serviceProvider.GetRequiredService<TiendaDbContext>();
            bool[] resultados = await Task.WhenAll(
                transientContext.Users.AnyAsync(v => v.Email == email),
                context.Users.AnyAsync(v => v.PhoneNumber == telefono)
            );

            return resultados;
        }

        public async Task<bool[]> ExistsUsuario(string id, string email, string telefono)
        {
            var transientContext = _serviceProvider.GetRequiredService<TiendaDbContext>();
            bool[] resultados = await Task.WhenAll(
                transientContext.Users.AnyAsync(v => v.Email == email && v.Id != id),
                userManager.Users.AnyAsync(v => v.PhoneNumber == telefono && v.Id != id)
            );

            return resultados;
        }

        public async Task<bool> ExistsUsuario(string id)
        {
            return await userManager.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> UpdatePassword(Usuario usuario, string password)
        {
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, password);
            var resultado = await userManager.UpdateAsync(usuario);
            return resultado.Succeeded;
        }

        public async Task<List<string>> GetRoles(string id)
        {
            var usuario = await userManager.FindByIdAsync(id);
            return (List<string>)await userManager.GetRolesAsync(usuario);
        }

        public async Task<bool> ValidatePassword(Usuario usuario, string password)
        {
            return await userManager.CheckPasswordAsync(usuario, password);
        }

        public async Task<Usuario> GetImagen(string id)
        {
            return await userManager.FindByIdAsync(id);
        }
    }
}
