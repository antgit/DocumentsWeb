using System;
using System.Linq;
using System.Web.Mvc;
using DocumentsWeb.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Areas.Reports.Models;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;

namespace DocumentsWeb.Areas.Reports.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class ReportController : CoreController<Library>
    {
        public ReportController()
        {
            Name = WebModuleNames.WEB_REPORTS;
        }

        protected override void OnCoreAuthorization(AuthorizationContext filterContext)
        {
            base.OnCoreAuthorization(filterContext);
            string actionName = filterContext.ActionDescriptor.ActionName;
            if (actionName.ToUpper() == "BUILD")
            {
                int objId = 0;
                //filterContext.RouteData.Values["id"]
                if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("id"))
                {
                    string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                    Int32.TryParse(valueParam, out objId);
                }
                if (filterContext.RouteData.Values.ContainsKey("id"))
                {
                    Int32.TryParse(filterContext.RouteData.Values["id"].ToString(), out objId);
                }
                if (!WADataProvider.LibrariesElementRightView.IsAllow(Right.UIREPORTBUILD, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на построение отчетов!");
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                if (objId != 0)
                {
                    Library obj = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(objId);
                    if (obj != null)
                    {
                        if (obj is ICompanyOwner)
                        {
                            ICompanyOwner companyObj = obj as ICompanyOwner;
                            if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                            {
                                throw new SecurityException("Построение отчетов вне собственной компании запрещено!");
                            }
                        }
                        if(!WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, objId))
                        {
                            throw new SecurityException("Отсутствуют разрешения на построение данного отчета!");
                        }
                        if (!WADataProvider.LibrariesElementRightView.IsAllow(Right.UIREPORTBUILD, objId))
                        {
                            throw new SecurityException("Отсутствуют разрешения на построение данного отчета!");
                        }
                    }
                    else
                    {
                        throw new SecurityException("Отчет не найден!");
                    }
                }
                else
                {
                    throw new SecurityException("Отчет не найден!");
                }

            }
        }

        public ActionResult Index()
        {
            ViewResult result = View();
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            int? folderId = (Request.Params["folderId"] == "null" || Request.Params["folderId"] == null) ? (int?)null : int.Parse(Request.Params["folderId"]);
            return PartialView(WebReportModel.GetReportsByFolder(folderId, refresh));
        }

        public ActionResult IndexTreePartial(bool refresh = false)
        {
            //int? folderId = (Request.Params["folderId"] == "null" || Request.Params["folderId"] == null) ? (int?)null : int.Parse(Request.Params["folderId"]);
            return PartialView(WebReportModel.GetFolders());
        }
        /// <summary>
        /// Построить отчет
        /// </summary>
        /// <param name="id">Идентификатор отчета</param>
        /// <returns></returns>
        public ActionResult Build(int id)
        {
            WebReportModel value = id == 0 ? new WebReportModel { Id = 0, Name = "" } : WebReportModel.GetObject(id);
            
            //string url = WADataProvider.SysConfig.ReportsLocation + "WebViewer" + (WADataProvider.SysConfig.UseFlashForReports ? "Fx" : "") + ".aspx?repId=";
            //Response.Write("<script>window.open('" + value.NavigateUrl + "');</script>");

            return Redirect(value.NavigateUrl);
        }

        public ActionResult GetHelp(int id)
        {
            if (id != 0)
            {
                WebReportModel value = WebReportModel.GetObject(id);
                if (!string.IsNullOrEmpty(value.HelpUrl))
                    return Redirect(value.HelpUrl);
                return HttpNotFound();
            }
            return HttpNotFound();
        }
        
    }
}
