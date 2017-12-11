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
using PicBook.Domain;

namespace PicBook.Web.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private IImageService imageService;
        private ITagService tagService;
        private IHostingEnvironment _env;
        private ITagConnection tagConnection;
        private bool remote;
        public ImageController(IImageService imageService, ITagService tagService, ITagConnection tagConnection, IHostingEnvironment env)
        {
            this.imageService = imageService;
            this.tagService = tagService;
            this.tagConnection = tagConnection;
            this._env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DeletePic(string id)
        {
            await imageService.DeletePic(id);
            return Ok();
        }

        public async Task<IActionResult> PublicPic(string id)
        {
            await imageService.PublicPic(id);
            return Ok();
        }
        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            Image uploadedImage;
            String _path = null;
            List<String> tags;
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                tags = new List<string>();
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
                            uploadedImage = await imageService.UploadImage(ms.ToArray(), userId, formFile.FileName);
                            _path = uploadedImage.ImageURL;
                        }
                    } else
                    {
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        _path = _env.WebRootPath + Path.Combine("\\images", unixTimestamp+"_"+formFile.FileName);
                        
                        using (var stream = new FileStream(_path, FileMode.Create))
                        {
                            uploadedImage = await imageService.UploadImage(null, userId, unixTimestamp + "_" + formFile.FileName);
                            await formFile.CopyToAsync(stream);
                        }
                    }
                    tags = await MakeAnalysisRequest(_path, tagConnection.ConnectionString);
                    if (tags.Any())
                    {
                        await tagService.SaveTags(tags, uploadedImage.ImageIdentifier);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok(new { count = files.Count, size});
        }


        static async Task<List<string>> MakeAnalysisRequest(string imageFilePath, string tagconnection)
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