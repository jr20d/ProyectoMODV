using Microsoft.EntityFrameworkCore;
using ProyectoModV.Data;
using ProyectoModV.DTO.Productos;
using ProyectoModV.Models;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.Uploads;

namespace ProyectoModV.Repository
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly TiendaDbContext context;
        private readonly IServiceProvider serviceProvider;
        private readonly ICargaArchivo cargaArchivo;

        public ProductoRepository(TiendaDbContext context, IServiceProvider serviceProvider, ICargaArchivo cargaArchivo)
        {
            this.context = context;
            this.serviceProvider = serviceProvider;
            this.cargaArchivo = cargaArchivo;
        }

        public async Task<bool> CreateProducto(Producto producto)
        {
            await context.Productos.AddAsync(producto);
            return await Save();
        }

        public async Task<bool> DeleteProducto(int productoId)
        {
            var producto = await context.Productos.FindAsync(productoId);
            if (producto == null)
            {
                return false;
            }
            if (producto.PublicId != null)
            {
                if (!cargaArchivo.EliminarArchivo(producto.PublicId))
                {
                    return false;
                }
            }
            
            context.Productos.Remove(producto);
            return await Save();
        }

        public async Task<bool> ExistsProducto(int productoId, int vendedorId)
        {
            return await context.Productos.AnyAsync(p => p.ProductoId == productoId && p.VendedorId == vendedorId);
        }

        public async Task<bool> ExistsProducto(string nombre, int vendedorId)
        {
            return await context.Productos.AnyAsync(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && p.VendedorId == vendedorId);
        }

        public async Task<bool> ExistsProducto(int productoId, string nombre, int vendedorId)
        {
            return await context.Productos.AnyAsync(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && p.ProductoId != productoId
            && p.VendedorId == vendedorId);
        }

        public async Task<int> GetCantidadProducto(int id)
        {
            return await context.Productos.AsQueryable()
                .Where(p => p.ProductoId == id).Select(p => p.Cantidad).SingleOrDefaultAsync();
        }

        public async Task<ProductoIdNombre?> GetIdAndNombreProducto(int id)
        {
            return await context.Productos.AsQueryable()
                .Where(p => p.ProductoId == id)
                .Select(p => p != null ? new ProductoIdNombre { ProductoId = p.ProductoId, Nombre = p.Nombre, Cantidad = p.Cantidad,
                Precio = p.Precio, Descuento = p.Descuento } :
                null)
                .FirstOrDefaultAsync();
        }

        public async Task<Producto?> GetProducto(int id)
        {
            return await context.Productos
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Include(p => p.Vendedor).ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(p => p.ProductoId == id);
        }

        public async Task<List<Producto>> GetProductos(string busqueda, int pagina, string marca, string tipoProducto)
        {
            return await context.Productos
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Include(p => p.Vendedor).ThenInclude(v => v.Usuario)
                .Where(p => (!string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower() :
                (!string.IsNullOrEmpty(marca) && string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower() :
                (string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower() :
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda)))
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<Producto>> GetProductos(string busqueda, int pagina, string marca, string tipoProducto, int vendedorId)
        {
            return await context.Productos
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Include(p => p.Vendedor).ThenInclude(v => v.Usuario)
                .Where(p => (!string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (!string.IsNullOrEmpty(marca) && string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.VendedorId == vendedorId)
                .OrderBy(p => p.Nombre)
                .Skip((pagina - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductos(string busqueda, string marca, string tipoProducto)
        {
            var transientContext = serviceProvider.GetRequiredService<TiendaDbContext>();

            return await transientContext.Productos
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Include(p => p.Vendedor).ThenInclude(v => v.Usuario)
                .Where(p => (!string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower() :
                (!string.IsNullOrEmpty(marca) && string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower() :
                (string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower() :
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda)))
                .CountAsync();
        }

        public async Task<int> GetTotalProductos(string busqueda, string marca, string tipoProducto, int vendedorId)
        {
            var transientContext = serviceProvider.GetRequiredService<TiendaDbContext>();

            return await transientContext.Productos
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Include(p => p.Vendedor).ThenInclude(v => v.Usuario)
                .Where(p => (!string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (!string.IsNullOrEmpty(marca) && string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.Marca.Nombre.Trim().ToLower() == marca.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (string.IsNullOrEmpty(marca) && !string.IsNullOrEmpty(tipoProducto)) ?
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.TipoProducto.Tipo.Trim().ToLower() == tipoProducto.Trim().ToLower()
                && p.VendedorId == vendedorId :
                (p.Nombre.Contains(busqueda) || p.Modelo.Contains(busqueda))
                && p.VendedorId == vendedorId)
                .CountAsync();
        }

        public async Task<bool> UpdateProducto(Producto producto)
        {

            context.Productos.Update(producto);
            return await Save();
        }

        private async Task<bool> Save()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}