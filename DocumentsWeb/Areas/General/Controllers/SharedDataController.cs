using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DocumentsWeb.Areas.Kb.Models;

namespace DocumentsWeb.Areas.General.Controllers
{
    public class SharedDataController : Controller
    {
        /// <summary>
        /// Последние пять новостей
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLastFiveNews()
        {
            var data = NewsData.GetSharedLastFiveNews().Select(s => new { Name = s.Name, SendDate = s.SendDate.Value.ToString("dd MMMM yyyy"), Memo = s.Memo }).ToList();
            return new JsonpResult(data);
        }
        /// <summary>
        /// Последняя новость
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLastFirstNews()
        {
            var data = NewsData.GetSharedLastFiveNews().OrderByDescending(s=>s.SendDate).Take(1).Select(s => new { Name = s.Name, SendDate = s.SendDate.Value.ToString("dd MMMM yyyy"), Memo = s.Memo }).ToList();
            return new JsonpResult(data);
        }
        public ActionResult WaitViewPartial()
        {
            return PartialView("WaitViewPartial");
        }
    }

    public class JsonpResult : ActionResult
    {
        public string CallbackFunction { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonpResult(object data):this(data, null){}
        public JsonpResult(object data, string callbackFunction)
        {
            Data = data;
            CallbackFunction = callbackFunction;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/x-javascript" : ContentType;

            if (ContentEncoding != null) response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                HttpRequestBase request = context.HttpContext.Request;

                var callback = CallbackFunction ?? request.Params["callback"] ?? "callback";

#pragma warning disable 0618 // JavaScriptSerializer is no longer obsolete
                var serializer = new JavaScriptSerializer();
                response.Write(string.Format("{0}({1});", callback, serializer.Serialize(Data)));
#pragma warning restore 0618
            }
        }
    }
    
}
