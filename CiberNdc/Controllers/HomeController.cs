using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CiberNdc.Context;
using CiberNdc.Models;
using CiberNdc.Util;

namespace CiberNdc.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _db = new DataContext();

        // GET: /Home/

        public ActionResult Index()
        {
            var photos = _db.Photos.Where(x => !String.IsNullOrEmpty(x.Name)).OrderBy(x => x.Id).Skip(Math.Max(0, _db.Photos.Count() - 8)).Take(8);
            return View(photos);
        }

        public ActionResult GetImage(int id)
        {
            var bilde = _db.Photos.Find(id);
            return File(bilde.ImageStream, ImageUtil.GetImageContentType(bilde.Format));
        }

        public ActionResult TakePhoto()
        {
            return View();
        }

        public ActionResult UploadPhoto(string warning, string success)
        {
            ViewBag.Warning = warning;
            ViewBag.Success = success;
            return View();
        }

         [HttpPost]
        public ActionResult UploadPhoto(string codeword)
         {
             var image = Request.Files.Count > 0 ? Request.Files[0] : null;

             if (String.IsNullOrEmpty(codeword))
                 codeword = DateTime.Now.ToShortDateString();

             if (image != null && ImageUtil.AllowedImageTypes.Contains(image.ContentType))
             {
                 var im = Image.FromStream(image.InputStream);
                 var imageFormat = ImageUtil.GetImageFormat(image.ContentType);
                 var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);

                 var photo = new Photo()
                                 {
                                     Filename = image.FileName,
                                     Format = imageFormat.ToString(),
                                     Name = codeword,
                                     ImageStream = ReadFully(newImage)
                                 };
                 _db.Photos.Add(photo);
                 _db.SaveChanges();
                 return RedirectToAction("UploadPhoto", new {success = "Image uploaded!"});
             }

             return RedirectToAction("UploadPhoto", new {warning = "Somethin went wrong!"});
         }

        [HttpPost]
        public ActionResult Test(string image, string codeword)
        {
            var splitted = image.Split(',')[1];
            var imageEncoded = Base64ToImage(splitted);
            var title = DateTime.Now.ToShortDateString();
            if (!String.IsNullOrEmpty(codeword))
                title = codeword;
            var filnanvn = "test-" + DateTime.Now.ToFileTime();
            //var fileLocation = "c:\\test\\" + filnanvn + ".png";
            //imageEncoded.Save(fileLocation, ImageFormat.Png);
            var photo = new Photo
                            {
                                Filename = filnanvn + ".png",
                                Format = "Image/Png",
                                Name = title,
                                ImageStream = ReadFully(imageEncoded.ResizeImage(new Size(400, 600), ImageFormat.Png)),
                            };
            _db.Photos.Add(photo);
            _db.SaveChanges();

            imageEncoded.Dispose();

            return new JsonResult { Data = new { success = true, name = "" } };  
        }
        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static Stream ToStream(Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            var imageBytes = Convert.FromBase64String(base64String);
            return Image.FromStream(new MemoryStream(imageBytes));
        }

    }
}
