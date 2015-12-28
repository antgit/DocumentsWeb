using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    /// <summary>
    /// Базовый контроллер настройки раздела
    /// </summary>
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class BaseDocumentConfigController : CoreDocumentControler
    {
        public BaseDocumentConfigController()
        {
            PrintModelCode = "DOCUMENTCONFIG";
        }

        public virtual ActionResult AgentFromPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);

            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = mainCompanyDepatmentId;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, mainCompanyDepatmentId);

            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

    }
}
