using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using System.Globalization;
using BusinessObjects;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Admins.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class SysConfigController : Controller
    {
        /// <summary>
        /// Главная страница
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(WADataProvider.SysConfig);
        }

        /// <summary>
        /// Очистка кэш на сервере
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCache()
        {
            WADataProvider.WA = null;
            WADataProvider.SysConfig = null;
            WADataProvider.WA.Access.RefreshCompanyScopeViewContext(WADataProvider.CurrentUser.Name);
            WADataProvider.WA.Refresh();
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_BRANDS);
            List<Analitic> coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_AGENTCATEGORY);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_AGENTMETRICAREA);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_AGENTTYPEOUTLET);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_AGENTOWNERSHIP);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_TAXDOCPAYMENTMETHOD);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_PACKTYPE);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_PRODUCTTYPE);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_RETURNREASON);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_TRADEGROUP);
            coll = h.GetTypeContents<Analitic>(true, true);

            h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_TRADEGROUP);
            coll = h.GetTypeContents<Analitic>(true, true);

            WADataProvider.WA.Cashe.RefreshChainCasheData();

            return RedirectPermanent(Url.Content("~/Admins/SysConfig"));
        }

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="model">Модель системной конфигурации</param>
        /// <returns></returns>
        public ActionResult Save([ModelBinder(typeof(DevExpressEditorsBinder))] SysConfigModel model)
        {
            model.Save();
            WADataProvider.SysConfig = model;
            return RedirectPermanent(Url.Content("~/Admins/SysConfig"));
        }
    }
}
