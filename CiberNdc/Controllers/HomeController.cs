﻿using System;
using System.Collections.Generic;
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

        public ActionResult Test()
        {
            var asdf = " ;";
            return null;
        }

    }
}
