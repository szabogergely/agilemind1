using PicBook.Domain;
using System;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public interface IImageService
    {
        Task<Image> UploadImage(byte[] imageBytes, String userIdentifier, String filename);
        bool IsRemote();
        Task DeletePic(string id, string userId);
        Task PublicPic(string id, string userId);
    }
}
