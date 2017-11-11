using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.Repository.AzureStorage
{
    public class LocalImageRepository : IImageRepository
    {

        string savepath;

        public LocalImageRepository(string savepath)
        {
            this.savepath = savepath;
        }
        public Task EnqueueWorkItem(Guid imageId)
        {
            return null;
        }

        public Task<ImageUploadResult> UploadImage(byte[] imageBytes)
        {
            return null;
        }
    }
}
