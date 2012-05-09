using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CiberNdc.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Mobil()
        {
            return View();
        }

        public ActionResult Test(string image, string test)
        {
            var splitted = image.Split(',')[1];
            var imageEncoded = Base64ToImage(splitted);

            var fileLocation = "c:\\test\\test-" + DateTime.Now.ToFileTime() + ".png";


            imageEncoded.Save(fileLocation, ImageFormat.Png);
            imageEncoded.Dispose();

            return new JsonResult() { Data = new { success = true, name = "Woot!" } };  
        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

    }
}
