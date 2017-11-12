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

namespace PicBook.Web.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private IImageService imageService;
        private IHostingEnvironment _env;

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
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0 && IsImage(formFile))
                {
                    String _path = _env.WebRootPath + Path.Combine("\\images", formFile.FileName);

                    var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                    var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    var userId = claim.Value;
                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);
                        uploadedImageUri = await imageService.UploadImage(ms.ToArray(), userId, formFile.FileName);
                    }

                    //using (var stream = new FileStream(_path, FileMode.Create))
                    //{
                    //    uploadedImageUri = new Uri(_path);
                    //    imageService.SaveImage(userId, formFile.FileName);
                    //    await formFile.CopyToAsync(stream);
                    //}
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, uploadedImageUri });
        }
        private bool IsImage(IFormFile file)
        {
            //Checks for image type... you could also do filename extension checks and other things
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+"));
        }
    }
}