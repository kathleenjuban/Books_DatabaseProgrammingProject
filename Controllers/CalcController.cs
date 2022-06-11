using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DBProg_A3.Controllers
{
    public class CalcController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        // ADD
        /// <summary>
        /// Allows user to calculate by adding items in the table
        /// </summary>
        /// <param name="s1">String s1</param>
        /// <param name="s2">String s2</param>
        /// <param name="s3">String s3</param>
        /// <returns></returns>
        public ActionResult Add(string s1, string s2, string s3)
        {
            decimal a1 = decimal.Parse(s1);
            decimal a2 = decimal.Parse(s2);
            decimal a3 = decimal.Parse(s3);
            var result = a1 + a2 + a3;

            var data = new
            {
                query = $"{s1} + {s2} + {s3}",
                result,
                Message = "Sum was calculated.",
                isSuccess = true
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}