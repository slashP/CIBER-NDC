using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
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
        private readonly FaceRestApi _api = new FaceRestApi("50361475f52d1637dfdcf01802536f31", "e5190fd0d963c89b272c1b79f8c1fb17",
            string.Empty, false, "", null, null);

        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.Employees = _db.Employees.Where(x => x.Photos.Count > 0 && x.IsActive).Include(x => x.Photos).OrderByDescending(x => x.Id);
            return View();
        }

        public PartialViewResult PhotoGrid(int? count)
        {
            var employees = _db.Employees.Where(x => x.Photos.Count > 0 && x.IsActive).Include(x => x.Photos);
           
            return PartialView(IEnumerableRandomization.Randomize(employees));
        }

        public PartialViewResult PhotoGridTest()
        {
            var photos = _db.Photos.Where(p => p.Employee.IsActive);

            return PartialView((List<Photo>)IEnumerableRandomization.Randomize(photos).Take(Math.Min(photos.Count(), 72)).ToList());
        }


        public PartialViewResult GetMessage(int? id)
        {
            if (!_db.Messages.Any())
                return null;
            if (id != null && _db.Messages.Find(id) != null)
                return PartialView(_db.Messages.Find(id));
            return PartialView(_db.Messages.Find(new Random().Next(2, _db.Messages.Count() + 1)));
        }

        public ActionResult GetImage(int id, string size)
        {
            var bilde = _db.Photos.Find(id);

            if (bilde == null)
                return null;

            if(size != null)
            {
                var s = size.Split('x');
                if (s.Length == 2)
                {
                    var scalesize = new Size(Convert.ToInt32(s[0]), Convert.ToInt32(s[1])); 
                    var scaled = ImageUtil.ResizeImage(ImageUtil.ByteArrayToImage(bilde.ImageStream), scalesize, ImageUtil.GetImageFormat(bilde.Format), true);
                    return File(scaled, ImageUtil.GetImageContentType(bilde.Format));
                }
            }
                
            return File(bilde.ImageStream, ImageUtil.GetImageContentType(bilde.Format));
        }
        
        public ActionResult TakePhoto()
        {
            ViewBag.Employees = _db.Employees;
            return View();
        }

        
        public ActionResult UploadPhoto(string warning, string success)
        {
            ViewBag.EmployeeId = new SelectList(_db.Employees.Where(x => x.IsActive), "Id", "Name");
            ViewBag.Warning = warning;
            ViewBag.Success = success;
            return View();
        }

        [HttpPost]
        public ActionResult UploadPhotoPost(string codeword, string uploadedby)
         {
            if (String.IsNullOrEmpty(codeword))
                return RedirectToAction("UploadPhoto", new { warning = "Fyll inn kodeord!" });

            if (String.IsNullOrEmpty(uploadedby))
                return RedirectToAction("UploadPhoto", new { warning = "Fyll inn navn og telefonnr!" });

            var recognize = codeword != "ignore";
            var image = Request.Files.Count > 0 ? Request.Files[0] : null;

            if(image == null)
                return RedirectToAction("UploadPhoto", new { warning = "Du må velge et foto!" });

            if(!ImageUtil.AllowedImageTypes.Contains(image.ContentType))
                return RedirectToAction("UploadPhoto", new { warning = "Not allowed imagetype! (" + image.ContentType + ")" });

            var im = Image.FromStream(image.InputStream);
            var imageFormat = ImageUtil.GetImageFormat(image.ContentType);
            var newImage = im.ResizeImage(new Size(480, 480), imageFormat);
            var title = "MisterX-" + DateTime.Now.ToString("MM.dd HH:mm");

            var photo = new Photo
                            {
                                Filename = image.FileName,
                                Format = imageFormat.ToString(),
                                Name = title,
                                ImageStream = ReadFully(newImage),
                                UploadedBy = uploadedby
                            };
                _db.Photos.Add(photo);
                _db.SaveChanges();

                if (recognize)
                {
                    var e = Recognize(photo.Id);

                    if (e == null)
                    {
                        _db.Photos.Remove(photo);
                        _db.SaveChanges();
                        return RedirectToAction("UploadPhoto", new { warning = "Ansatt ikke gjenkjent!" });
                    }

                    if (e.Codeword.ToUpper() != codeword.ToUpper())
                    {
                        _db.Photos.Remove(photo);
                        _db.SaveChanges();
                        return RedirectToAction("UploadPhoto", new { warning = "Feil kodeord for " + e.Name + "!" });
                    }

                    var p = _db.Photos.Find(photo.Id);
                    p.Employee = e;
                    p.Name = p.Name.Replace("MisterX", e.Name + "(recognized)");
                    _db.SaveChanges();
                    return RedirectToAction("UploadPhoto", new { success = "Riktig kodeord for " + e.Name + "! Bilde opplastet!" });

                }

                return RedirectToAction("UploadPhoto", new { success = "Bilde opplastet, ikke tilordnet noen ansatte" });

         }

        [HttpPost]
        public ActionResult UploadAndRecognize(string image, string codeword, string uploadedby)
        {
            if (String.IsNullOrEmpty(uploadedby))
                return new JsonResult { Data = new { warning = true, message = "Fyll inn navn og telefonnr!" } };
            var filnanvn = "test-" + DateTime.Now.ToFileTime();
            var title = "MisterX-" + DateTime.Now.ToString("MM.dd HH:mm");
            var splitted = image.Split(',')[1];
            var imageEncoded = Base64ToImage(splitted);
            var recognize = codeword != "ignore";
            
            var photo = new Photo
            {
                Filename = filnanvn + ".jpg",
                Format = "Image/jpg",
                Name = title,
                ImageStream = ReadFully(imageEncoded.ResizeImage(new Size(480, 480), ImageFormat.Jpeg)),
                Employee = null,
                UploadedBy = uploadedby
            };

            _db.Photos.Add(photo);
            _db.SaveChanges(); //Må lagre det her for å hente det ut att på nytt.
            imageEncoded.Dispose();

            if (recognize)
            {
                var e = Recognize(photo.Id);

                if (e == null)
                {
                    _db.Photos.Remove(photo);
                    _db.SaveChanges();
                    return new JsonResult { Data = new { warning = true, message = "Ansatt ikke gjenkjent!" } };
                }

                if (e.Codeword.ToUpper() != codeword.ToUpper())
                {
                    _db.Photos.Remove(photo);
                    _db.SaveChanges();
                    return new JsonResult { Data = new { warning = true, message = "Feil kodeord for " + e.Name + "!" } };
                }
                
                var p = _db.Photos.Find(photo.Id);
                p.Employee = e;
                p.Name = p.Name.Replace("MisterX", e.Name + "(recognized)");
                _db.SaveChanges();
                return new JsonResult { Data = new { success = true, message = "Riktig kodeord for " + e.Name + "! Bilde opplastet!" } };
                

            }
            Thread.Sleep(2000);
            return new JsonResult { Data = new { success = true, message = "Bilde opplastet, ikke tilordnet noen ansatte" } };
        }

        private Employee Recognize(int photoId)
        {
            var photo = _db.Photos.Find(photoId);
            if (photo == null)
                return null;

            var uids = _db.Employees.Select(emp => emp.Name + "@ndcwebapp.apphb.com").ToList();
            var urls = new List<string> { "http://ciber.apphb.com/Home/GetImage/" + photo.Id };
            var asdf = _api.FacesRecognize(urls, uids, "ndcwebapp.apphb.com", "", "", null, null, null);
            var image = asdf.Photos.FirstOrDefault();

            if (image != null)
            {
                if (image.Tags != null) {
                    var tag = image.Tags.FirstOrDefault();
                    if (tag != null)
                    {
                        var uid  = tag.uids.FirstOrDefault();
                        if (uid != null)
                        {
                            return Enumerable.FirstOrDefault(_db.Employees, employee => uid.uid.Contains(employee.Name));
                        }
                    }
                }
            }
            return null;
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

        public ActionResult Employees()
        {
            ViewBag.Employees = _db.Employees.Where(x => x.Photos.Count > 0 && x.IsActive).Include(x => x.Photos).OrderByDescending(x => x.Id);
            return View();
        }
    }
}
