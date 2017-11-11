using System;
using System.Threading.Tasks;

namespace PicBook.Repository.AzureStorage
{
    // TOOD: refactor to abstract repo project
    public interface IImageRepository
    {
        Task<ImageUploadResult> UploadImage(byte[] imageBytes);
        Task EnqueueWorkItem(Guid imageId);
    }
}