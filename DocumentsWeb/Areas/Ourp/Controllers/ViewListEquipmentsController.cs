using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects.Web.Core;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
	[MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
	public class ViewListEquipmentsController : CoreController
    {
		public ViewListEquipmentsController()
        {
			//Name = "VLE";
			// TODO: Исправить на то, что нужно
			Name = WebModuleNames.WEB_BANKS;;
        }

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult IndexPartial()
		{
			return PartialView();
		}
    }
}
