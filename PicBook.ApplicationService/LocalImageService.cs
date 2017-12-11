using System;
using System.Threading.Tasks;
using PicBook.Domain;
using PicBook.Repository.AzureStorage;

namespace PicBook.ApplicationService
{
    public class LocalImageService : IImageService
    {
        private IImageRepository imageRepo;
        private readonly PicBook.Repository.EntityFramework.IImageRepository dbimageRepo;

        public LocalImageService(IImageRepository imageRepo, PicBook.Repository.EntityFramework.IImageRepository dbimageRepo)
        {
            this.imageRepo = imageRepo;
            this.dbimageRepo = dbimageRepo;
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
        public bool IsRemote()
        {
            return false;
        }       
        public async Task<Image> UploadImage(byte[] imageBytes, String userIdentifier, String filename)
        {
            var u = new Image()
            {
                UserIdentifier = userIdentifier,
                Name = filename,
                ImageIdentifier = filename,
                ImageURL = filename,
                Remote = false
            };
            await dbimageRepo.Create(u);

            return u;
        }

    }
}
