using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Analitics.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Analitics.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class AnaliticBaseController : CoreController<Analitic>
    {
        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View("Preview", AnaliticModel.GetObject(id));
        }

        public ActionResult GetNameById(int id)
        {
            Analitic analitic = WADataProvider.WA.GetObject<Analitic>(id);
            return Content(analitic == null ? string.Empty : analitic.Name);
        }
    }
}
