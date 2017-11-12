using System;
using System.Threading.Tasks;
using PicBook.Domain;
using PicBook.Repository.AzureStorage;

namespace PicBook.ApplicationService
{
    public class ImageService : IImageService
    {
        private IImageRepository imageRepo;
        private readonly PicBook.Repository.EntityFramework.IImageRepository dbimageRepo;

        public ImageService(IImageRepository imageRepo, PicBook.Repository.EntityFramework.IImageRepository dbimageRepo)
        {
            this.imageRepo = imageRepo;
            this.dbimageRepo = dbimageRepo;
        }

        public async void SaveImage(String userIdentifier, String filename)
        {
            var u = new Image()
            {
                UserIdentifier = userIdentifier,
                Name = filename
            };

            await dbimageRepo.Create(u);
        }

        public async Task<Uri> UploadImage(byte[] imageBytes, String userIdentifier, String filename)
        {
            ImageUploadResult result = await imageRepo.UploadImage(imageBytes);
            await imageRepo.EnqueueWorkItem(result.ImageId);

            var u = new Image()
            {
                UserIdentifier = userIdentifier,
                Name = filename,
                ImageIdentifier = result.ImageId.ToString(),
                ImageURL = result.ImageUri.ToString()
            };
            await dbimageRepo.Create(u);

            return result.ImageUri;
        }

    }
}
