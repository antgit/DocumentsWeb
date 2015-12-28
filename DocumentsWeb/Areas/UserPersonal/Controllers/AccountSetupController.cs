using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.UserPersonal.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class AccountSetupController : Controller
    {
        public ActionResult Index()
        {
            AccountConfigModel model = WADataProvider.AccountConfig;
            model.ThemeDefault = Utils.CurrentTheme;
            return View(model);
        }

        /// <summary>
        /// Установка темы пользователя
        /// </summary>
        /// <param name="themeName">Имя темы</param>
        public void SetUserTheme(string themeName)
        {
            Utils.SetUserTheme(themeName);
        }

    }
}
