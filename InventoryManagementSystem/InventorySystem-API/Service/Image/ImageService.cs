
namespace InventorySystem_API.Service.Image
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        public ImageService(IWebHostEnvironment env) 
        {
            _env = env;
        }   

        public async Task<string> SaveImage(IFormFile file, string folder)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var path = Path.Combine(
                _env.WebRootPath,
                "Images",
                folder,
                fileName
            );

            using var stream = new FileStream(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/Images/{folder}/{fileName}";
        }

        public void DeleteImage(string path)
        {
            if (path.Contains("default"))
                return;

            if(File.Exists(path))
                File.Delete(path);
        }

        public string GetDefaultImage(string folder) =>
            $"/Images/{folder}/default.png";

        public async Task<string> CopyImage(string path)
        {
            var oldFile = Path.Combine(_env.WebRootPath, path.TrimStart('/'));

            if(!File.Exists(oldFile))
                throw new FileNotFoundException($"Не знайдно файлу за вказаним шляхом {oldFile}");

            var directory = Path.GetDirectoryName(oldFile);
            var extension = Path.GetExtension(oldFile);
            var newName = Guid.NewGuid().ToString() + extension;
            var newFile = Path.Combine(directory!, newName);

            using var sourceStream = new FileStream(oldFile, FileMode.Open);
            using var destinationStream = new FileStream(newFile, FileMode.Create);

            await sourceStream.CopyToAsync(destinationStream);

            var folder = Path.GetFileName(directory!);

            return $"/Images/{folder}/{newName}";
        }
    }
}
