using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CiberNdc.Context;
using CiberNdc.Util;

namespace CiberNdc.Controllers
{
    [Authorize]
    public class RecognitionController : Controller
    {
        private readonly DataContext _db = new DataContext();
        private readonly FaceRestApi _api = new FaceRestApi("50361475f52d1637dfdcf01802536f31", "e5190fd0d963c89b272c1b79f8c1fb17",
                                      string.Empty, false, "", null, null);

        public ActionResult Train(int photoId, int employeeId)
        {
            var emp = _db.Employees.Find(employeeId);
            var photo = _db.Photos.Find(photoId);
            if (emp == null || photo == null)
                return null;

            var uids = new List<string> { emp.Name + "@ndcwebapp.apphb.com" };
            var urls = new List<string> { "http://ndcwebapp.apphb.com/Home/GetImage/" + photo.Id };

            var detection = _api.FacesDetect(urls, "", null, null, null);
            var traint = _api.FacesTrain(uids, "ndcwebapp.apphb.com", "");

            return null;
        }

        public ActionResult Recognize(int photoId)
        {
            var photo = _db.Photos.Find(photoId);
            if (photo == null)
                return null;

            var uids = _db.Employees.Select(emp => emp.Name + "@ndcwebapp.apphb.com").ToList();
            var urls = new List<string> { "http://ndcwebapp.apphb.com/Home/GetImage/" + photo.Id };
            var asdf = _api.FacesRecognize(urls, uids, "ndcwebapp.apphb.com", "", "", null, null, null);
            var firstOrDefault = asdf.Photos.FirstOrDefault();
            float conf = 0;

            var uid = new FaceRestApi.UID();

            if (firstOrDefault != null)
            {
                var orDefault = firstOrDefault.Tags.FirstOrDefault();
                if (orDefault != null)
                {
                    uid = orDefault.uids.FirstOrDefault();
                    if (uid != null)
                    {
                        conf = uid.confidence;
                    }
                }
            }
            ViewBag.recongizedEmployee = uid.uid + "(" + uid.confidence + ")";
            return View();
        }

        public ActionResult EmployeeBatchTrain(int employeeId)
        {
            var emp = _db.Employees.Find(employeeId);
            if (emp == null)
                return null;

            var uids = new List<string> { emp.Name + "@ndcwebapp.apphb.com" };
            var urls = emp.Photos.Select(photo => "http://ndcwebapp.apphb.com/Home/GetImage/" + photo.Id).ToList();
            var detection = _api.FacesDetect(urls, "", null, null, null);
            var traint = _api.FacesTrain(uids, "ndcwebapp.apphb.com", "");

            return null;
        }
    }
}
