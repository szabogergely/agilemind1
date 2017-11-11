using System;
using System.Threading.Tasks;
using PicBook.Repository.AzureStorage;

namespace PicBook.ApplicationService
{
    public class ImageService : IImageService
    {
        private IImageRepository imageRepo;

        public ImageService(IImageRepository imageRepo)
        {
            this.imageRepo = imageRepo;
        }

        public async Task<Uri> UploadImage(byte[] imageBytes)
        {
            ImageUploadResult result = await imageRepo.UploadImage(imageBytes);
            await imageRepo.EnqueueWorkItem(result.ImageId);
            return result.ImageUri;
        }
    }
}
