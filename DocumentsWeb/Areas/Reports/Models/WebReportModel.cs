using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Models;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Reports.Models
{
    public class WebReportModel : ReportModel, IModelData
    {
        public string InHierarchies { get; set; }

        public static WebReportModel ConvertToModel(Library value)
        {
            WebReportModel model = new WebReportModel();
            model.GetData(value);

            if(value.KindId == Library.KINDID_WEBREPORT)
            {
                model.NavigateUrl = string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}", WADataProvider.SysConfig.ReportsLocation,
                                             WADataProvider.SysConfig.UseFlashForReports
                                                 ? "WebViewerFx.aspx?repId="
                                                 : "WebViewer.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name, WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId);
                model.NavigateUrlFx = WADataProvider.SysConfig.ReportsServer + "WebViewerFx.aspx?repId=" + value.Id;
            }
            else if(value.KindId== Library.KINDID_REPSQL)
            {
                //HttpUtility.UrlEncode(item.TypeUrl)
                /*
                 if(srv.EndsWith("reportserver"))
                navstring = srv + "?" + navstring;
            else
                navstring = srv + "/Pages/Report.aspx?ItemPath=" + navstring;
                 */
                if (WADataProvider.SysConfig.ReportsServer.EndsWith("reportserver"))
                    model.NavigateUrl = WADataProvider.SysConfig.ReportsServer + "?" + HttpUtility.UrlEncode(value.TypeUrl);
            }
            

            return model;
        }
        /// <summary>
        /// Товар по идентификатору
        /// </summary>
        public static WebReportModel GetObject(int id)
        {
            Library obj = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(id);
            return ConvertToModel(obj);
        }
        public static IEnumerable GetFolders()
        {
            Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("REPORTSMODULEWEBREPORTS");
            return hierarchy.Children.Where(f=>!f.IsHiden).Select(s => new ComboboxModel(s));
        }

        public static IEnumerable GetReportsByFolder(int? folderId, bool refresh=false)
        {
            if (!folderId.HasValue)
                return new List<WebReportModel>();
            if (refresh)
                WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(folderId.Value);
            return hierarchy.GetTypeContents<Library>(false, refresh).Where(f => !f.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId) && WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, f.Id)).Select(ConvertToModel);
        }

        public static bool IsReportAllow(string reportCode)
        {
            Library report = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>(reportCode);
            if (report != null && report.Id != 0)
                return WADataProvider.IsCompanyIdAllowIdToCurrentUser(report.MyCompanyId) &&
                       WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, report.Id);
            return false;
        }
    }
}