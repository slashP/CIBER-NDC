﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var employees = _db.Employees.Where(x => x.Photos.Count > 0 && x.IsActive).Include(x => x.Photos);
            return View(employees.ToList());
        }

        public ActionResult GetImage(int id)
        {
            var bilde = _db.Photos.Find(id);
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
        public ActionResult UploadPhoto(int? employeeId, string codeword)
         {
            if(employeeId == null)
                return RedirectToAction("UploadPhoto", new { warning = "You have to choose an employee!" });
            if (String.IsNullOrEmpty(codeword))
                return RedirectToAction("UploadPhoto", new { warning = "You have to fill in a codeword!" });

            var employee = _db.Employees.Find(employeeId);

            if (employee.Codeword.ToUpper() != codeword.ToUpper())
                return RedirectToAction("UploadPhoto", new { warning = "Wrong codeword!" });

            var image = Request.Files.Count > 0 ? Request.Files[0] : null;

            if (image != null && ImageUtil.AllowedImageTypes.Contains(image.ContentType))
            {
                var im = Image.FromStream(image.InputStream);
                var imageFormat = ImageUtil.GetImageFormat(image.ContentType);
                var newImage = im.ResizeImage(new Size(640, 480), imageFormat);
                var title = employee.Name + "-" + DateTime.Now.ToString("MM.dd HH:mm");

                var photo = new Photo()
                                {
                                    Filename = image.FileName,
                                    Format = imageFormat.ToString(),
                                    Name = title,
                                    ImageStream = ReadFully(newImage),
                                    Employee = employee
                                 };
                 _db.Photos.Add(photo);
                 _db.SaveChanges();
                 return RedirectToAction("UploadPhoto", new {success = "Correct codeword! Image uploaded!"});
             }

             return RedirectToAction("UploadPhoto", new {failure = "Something went wrong!"});
         }

        [HttpPost]
        public ActionResult Test(string image, string codeword, string employeeId)
        {
            var employee = _db.Employees.Find(Convert.ToInt32(employeeId));
            
            if (String.IsNullOrEmpty(codeword))
                return new JsonResult { Data = new { warning = true, message = "Fill in a codeword!" } };
            
            if (String.IsNullOrEmpty(employeeId))
                return new JsonResult { Data = new { warning = true, message = "Choose an employee!" } };

            if (employee == null)
                return new JsonResult { Data = new { warning = true, message = "Employee not found!" } };

            if (employee.Codeword.ToUpper() != codeword.ToUpper())
                return new JsonResult { Data = new { warning = true, message = "Wrong codeword!" } };

            var filnanvn = "test-" + DateTime.Now.ToFileTime();
            var title = employee.Name + "-" + DateTime.Now.ToString("MM.dd HH:mm");
            var splitted = image.Split(',')[1];
            var imageEncoded = Base64ToImage(splitted);

            var photo = new Photo
                            {
                                Filename = filnanvn + ".jpg",
                                Format = "Image/jpg",
                                Name = title,
                                ImageStream = ReadFully(imageEncoded.ResizeImage(new Size(640, 480), ImageFormat.Jpeg)),
                                Employee = employee
                            };
            employee.Photos.Add(photo); //funkar dette då?
            _db.Photos.Add(photo);
            _db.SaveChanges();

            imageEncoded.Dispose();

            return new JsonResult { Data = new { success = true, message = "Correct codeword, image uploaded." } };  
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
