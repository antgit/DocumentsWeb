using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Routes.Models;

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class LoggerController : Controller
    {
        //
        // GET: /Routes/Logger/

        public ActionResult Index()
        {
            return View();
        }

        #region Термометры

        public JsonResult GetLastValues(int deviceId, int count)
        {
            return Json(LoggerModel.GetLastValues(deviceId, count), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetValuesByDate(int deviceId, string Date)
        {
            return Json(LoggerModel.GetValuesByDate(deviceId, Date), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
