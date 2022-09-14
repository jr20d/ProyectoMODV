using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;

namespace ProyectoModV.Uploads
{
    public class CargaArchivo : ICargaArchivo
    {
        public bool EliminarArchivo(string publicId)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var deletionParams = new DeletionParams(publicId);

            var resultado = cloudinary.Destroy(deletionParams);
            return resultado.Result == "ok";
        }

        public string[] Subir(IFormFile archivo, string nombre)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var extension = archivo.ContentType.ToLower() == "image/png" ? ".png" : ".jpg";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(nombre == "" ? Guid.NewGuid().ToString() + extension : nombre, archivo.OpenReadStream()),
                Folder = "mod-v",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };
            var resultado = cloudinary.Upload(uploadParams);
            if (!resultado.JsonObj.HasValues)
            {
                return new string[] { };
            }
            return new string[] { resultado.SecureUrl.ToString(), resultado.PublicId };
        }
    }
}
