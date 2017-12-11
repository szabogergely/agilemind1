using Newtonsoft.Json.Linq;
using PicBook.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PicBook.ApplicationService
{
    public class TagService : ITagService
    {
        private readonly PicBook.Repository.EntityFramework.ITagRepository dbtagRepo;

        public TagService(PicBook.Repository.EntityFramework.ITagRepository dbtagRepo)
        {
            this.dbtagRepo = dbtagRepo;
        }
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        public async Task<List<string>> MakeAnalysisRequest(string imageFilePath, string tagconnection)
        {
            const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";
            string subscriptionKey = tagconnection;
            List<string> Tags = new List<string>();

            HttpClient client = new HttpClient();


            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            string requestParameters = "visualFeatures=Tags&language=en";

            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();

                dynamic data = JValue.Parse(contentString);

                for (int i = 0; i < (int)data.tags.Count; i++)
                {
                    dynamic item = data.tags[i];
                    double confidence = (double)item.confidence;
                    if (confidence > 0.5)
                    {
                        Tags.Add((string)item.name);
                    }
                }

            }

            return Tags;
        }


        public async Task<bool> SaveTags(List<String> tags, String imageIdentifier)
        {
            foreach(String tag in tags) {
                var u = new Tag()
                {
                    TagName = tag,
                    ImageIdentifier = imageIdentifier
                };
                await dbtagRepo.Create(u);
            }
            return true;
        }

    }
}
