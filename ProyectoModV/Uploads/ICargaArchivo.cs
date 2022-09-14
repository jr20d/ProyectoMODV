namespace ProyectoModV.Uploads
{
    public interface ICargaArchivo
    {
        string[] Subir(IFormFile archivo, string nombre);

        bool EliminarArchivo(string publicId);
    }
}
