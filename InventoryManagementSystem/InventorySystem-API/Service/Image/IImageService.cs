namespace InventorySystem_API.Service.Image
{
    public interface IImageService
    {
        Task<string> SaveImage(IFormFile file, string folder);
        void DeleteImage(string path);
        string GetDefaultImage(string folder);
        Task<string> CopyImage(string path);
    }
}
