using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PicBook.Repository.AzureStorage
{
    public class ImageUploadResult
    {
        public Guid ImageId { get; set; }

        public Uri ImageUri { get; set; }

    }
}