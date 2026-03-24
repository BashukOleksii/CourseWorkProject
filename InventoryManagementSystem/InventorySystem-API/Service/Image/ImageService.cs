
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
        
           
        


    }
}
