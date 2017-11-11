using System;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public interface IImageService
    {
        Task<Uri> UploadImage(byte[] imageBytes);
    }
}
