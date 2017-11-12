using PicBook.Domain;
using System;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public interface IImageService
    {
        Task<Uri> UploadImage(byte[] imageBytes, String userIdentifier, String filename);
        void SaveImage(String userIdentifier, String filename);
    }
}
