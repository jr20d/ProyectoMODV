using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProyectoModV.Data;
using ProyectoModV.DTO.Marcas;
using ProyectoModV.DTO.Productos;
using ProyectoModV.DTO.TiposProductos;
using ProyectoModV.DTO.Usuarios;
using ProyectoModV.DTO.Ventas;
using ProyectoModV.Mappers;
using ProyectoModV.Models;
using ProyectoModV.Repository;
using ProyectoModV.Repository.IRepository;
using ProyectoModV.SecurityConfig;
using ProyectoModV.Uploads;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//COnfiguración de conexión y contexto de Entity Framework
builder.Services.AddDbContext<TiendaDbContext>(conf =>
{
    conf.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
}, ServiceLifetime.Transient);

//Configuración de Identity framework
var identityCoreService = builder.Services.AddIdentityCore<Usuario>();
new IdentityBuilder(identityCoreService.UserType, identityCoreService.Services)
    .AddRoles<IdentityRole>()
    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Usuario, IdentityRole>>()
    .AddEntityFrameworkStores<TiendaDbContext>()
    .AddSignInManager<SignInManager<Usuario>>();

builder.Services.TryAddSingleton<ISystemClock, SystemClock>();

//Seguridad basada en JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(conf =>
{
    conf.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AA783gui28*=823+82.GFTYbjwdj")),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

//Inyeccion de dependencias
builder.Services.AddScoped<IVendedorRepository, VendedorRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRespository>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<ITipoProductoRepository, TipoProductoRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<ICargaArchivo, CargaArchivo>();

//Mappers
builder.Services.AddAutoMapper(typeof(VendedorProfileMapper), typeof(UsuarioProfileMapper), typeof(MarcaProfileMapper),
    typeof(TipoProductoProfileMapper), typeof(ProductoProfileMapper), typeof(VentaProfileMapper));

//Inyección de las validaciones de Fluent
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
}).AddFluentValidation(config => {
    //Validación usuarios
    config.RegisterValidatorsFromAssemblyContaining<CrearUsuarioValidator>();
    config.RegisterValidatorsFromAssemblyContaining<ActualizarUsuarioValidator>();
    config.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
    config.RegisterValidatorsFromAssemblyContaining<ActualizarPasswordvalidator>();

    //Validación Marcas
    config.RegisterValidatorsFromAssemblyContaining<MarcaBaseValidator>();

    //Validación TiposProductos
    config.RegisterValidatorsFromAssemblyContaining<TipoProductoValidator>();

    //Validación Productos
    config.RegisterValidatorsFromAssemblyContaining<DataProductoValidator>();
    config.RegisterValidatorsFromAssemblyContaining<ActualizarStockValidator>();

    //Validación Ventas
    config.RegisterValidatorsFromAssemblyContaining<AgregarVentaValidator>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExamples();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "API Proyecto MOD-V", Description = "Backend Mercado Online", Version = "v1", Contact = new OpenApiContact {
        Email = "ra1576012019@unab.edu.vs", Name = "UNAB", Url = new Uri("https://www.unab.edu.sv")
    }, License = new OpenApiLicense { Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT") } });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese un Bearer token de tipo JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    opt.EnableAnnotations();
    /*opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });*/
    opt.OperationFilter<FiltroAutorizacionEndpointsSwagger>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();