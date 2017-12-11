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

        public bool IsRemote()
        {
            return true;
        }
        public async Task DeletePic(string id, string userId)
        {
            var image = await dbimageRepo.FindByIdentifier(id);
            if (image.UserIdentifier.Equals(userId))
            {
                await dbimageRepo.DeletePic(image);
            }
        }
        public async Task PublicPic(string id, string userId)
        {
            var image = await dbimageRepo.FindByIdentifier(id);
            if (image.UserIdentifier.Equals(userId))
            {
                await dbimageRepo.PublicPic(image);
            }
        }
        public async Task<Image> UploadImage(byte[] imageBytes, String userIdentifier, String filename)
        {
            ImageUploadResult result = await imageRepo.UploadImage(imageBytes);
            await imageRepo.EnqueueWorkItem(result.ImageId);

            var u = new Image()
            {
                UserIdentifier = userIdentifier,
                Name = filename,
                ImageIdentifier = result.ImageId.ToString(),
                ImageURL = result.ImageUri.ToString(),
                Remote = true
            };
            await dbimageRepo.Create(u);

            return u;
        }

    }
}
