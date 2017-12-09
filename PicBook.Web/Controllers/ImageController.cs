using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PicBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using PicBook.ApplicationService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace PicBook.Web.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private IImageService imageService;
        private IHostingEnvironment _env;
        private bool remote;

        public ImageController(IImageService imageService, IHostingEnvironment env)
        {
            this.imageService = imageService;
            this._env = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            Uri uploadedImageUri = null;
            String _path = null;
            object Tags = null;
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0 && IsImage(formFile))
                {
                    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    var userId = claim.Value;

                    if (imageService.IsRemote())
                    {
                        using (var ms = new MemoryStream())
                        {
                            formFile.CopyTo(ms);
                            uploadedImageUri = await imageService.UploadImage(ms.ToArray(), userId, formFile.FileName);
                        }
                    } else
                    {
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        _path = _env.WebRootPath + Path.Combine("\\images", unixTimestamp+"_"+formFile.FileName);
                        
                        using (var stream = new FileStream(_path, FileMode.Create))
                        {
                            uploadedImageUri = new Uri(_path);
                            await imageService.UploadImage(null, userId, unixTimestamp + "_" + formFile.FileName);
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            Tags = await MakeAnalysisRequest(_path);

            return Ok(new { count = files.Count, size, uploadedImageUri, Tags});
        }


        static async Task<List<string>> MakeAnalysisRequest(string imageFilePath)
        {
            const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";
            const string subscriptionKey = "xxx";
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

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }


        private bool IsImage(IFormFile file)
        {
            //Checks for image type... you could also do filename extension checks and other things
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+"));
        }
    }
}