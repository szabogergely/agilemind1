using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PicBook.Repository.AzureStorage
{
    public class ImageRepository : IImageRepository
    {
        private CloudStorageAccount storageAccount;

        public ImageRepository(string storageConnString)
        {
            storageAccount = CloudStorageAccount.Parse(storageConnString);
        }

        public async Task<ImageUploadResult> UploadImage(byte[] imageBytes)
        {
            // TODO: error handling
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync();
            var fileId = Guid.NewGuid();
            var blob = container.GetBlockBlobReference(fileId.ToString());
            await blob.UploadFromByteArrayAsync(imageBytes, 0, imageBytes.Length);
            
            return new ImageUploadResult
            {
                ImageId = fileId,
                ImageUri = blob.Uri
            };
        }

        public async Task EnqueueWorkItem(Guid imageId)
        {
            // TODO: error handling + retry policy
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("imageprocess");
            await queue.CreateIfNotExistsAsync();
            var message = new CloudQueueMessage(imageId.ToString());
            await queue.AddMessageAsync(message);
        }

    }
}
