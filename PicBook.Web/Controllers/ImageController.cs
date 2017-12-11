using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using PicBook.ApplicationService;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
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
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            await imageService.DeletePic(id, userId);
            return Ok();
        }

        public async Task<IActionResult> PublicPic(string id)
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            await imageService.PublicPic(id, userId);
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
                    tags = await tagService.MakeAnalysisRequest(_path, tagConnection.ConnectionString);
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
        private bool IsImage(IFormFile file)
        {
            //Checks for image type... you could also do filename extension checks and other things
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+"));
        }
    }
}