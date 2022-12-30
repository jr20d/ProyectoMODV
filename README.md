# API de tienda en línea desarrollada en ASP.NET Core 6
# Características:
* Permite crear cuentas para usuarios de tipo vendendor y cliente.
* Registro de productos por vendedor.
* Registro de tipo de productos, marcas y maestro detalle para las ventas realizadas.
* Documentación de la API con Swagger.
* Seguridad basada en tokens de tipo JWT (IdentityFrameworkCore).
* Enpoints dedicados a refrescar/renovar el token de tipo JWT, tanto para vendedores como clientes.
* Conexión a base de datos SQL Server utilizando EntityFrameworkCore.
* Uso de migraciones para generar la base de datos.
* Carga de imágenes de usuarios (clientes y vendedores) y productos utilizando Cloudinary.
* Consumo de API creada en Spring Boot para realizar el proceso de pago.

> URL repositorio de la API de pagos (Spring Boot) -> https://github.com/jr20d/API-Pagos.git
