using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;
namespace DocumentsWeb
{
    public static class ReportHelper
    {
        public static string GetReportNavigateUrl(Library value, int id)
        {
            return string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}&Id={6}",
                                 WADataProvider.SysConfig.ReportsLocation,
                                 "WebViewer.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name,
                                 WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId, id);
        }
        public static string GetReportNavigateUrlFx(Library value, int id)
        {
            return string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}&Id={6}",
                                 WADataProvider.SysConfig.ReportsLocation,
                                 "WebViewerFx.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name,
                                 WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId, id);
        }
        public static string GetReportNavigateUrlFx(Library value)
        {
            return string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}",
                                 WADataProvider.SysConfig.ReportsLocation,
                                 "WebViewerFx.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name,
                                 WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId);
        }
        public static string GetReportNavigateUrl(Library value)
        {
            return string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}",
                                 WADataProvider.SysConfig.ReportsLocation,
                                 WADataProvider.SysConfig.UseFlashForReports
                                     ? "WebViewerFx.aspx?repId="
                                     : "WebViewer.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name,
                                 WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId);
        }
        public static string GetPrintFormNavigateUrl(Library value, string docprint, int documentId)
        {
            return string.Format("{0}{1}{2}&userName={3}&userpsw={4}&MyCompanyId={5}&docprint={6}&Id={7}",
                                 WADataProvider.SysConfig.ReportsLocation,
                                 WADataProvider.SysConfig.UseFlashForWebForms
                                     ? "WebViewerFx.aspx?repId="
                                     : "WebViewer.aspx?repId=", value.Id, HttpContext.Current.User.Identity.Name,
                                 WADataProvider.CurrentUser.PasswordHash, WADataProvider.CurrentUser.MyCompanyId, docprint, documentId);
        }

        public static string GetPrintFormNavigateUrlInternal(Library value, string docprint, int documentId)
        {
        //http://localhost/DocumentsWeb/Reports/Printform/1207152/1072/CONTRACTORGTEX
        //http://localhost/DocumentsWeb/reports/printform/index/1207152/1072/CONTRACTORGTEX
            return DevExpressHelper.GetUrl(new { Controller = "Printform", Action = "Index", Area = "Reports" }) + string.Format(@"/index/{0}/{1}/{2}", documentId, value.Id, docprint);
        }
        public static string GetPrintReportNavigateUrlInternal(Library value)
        {
            //http://localhost/DocumentsWeb/Reports/Printform/1207152/1072/CONTRACTORGTEX
            //http://localhost/DocumentsWeb/reports/printform/index/1207152/1072/CONTRACTORGTEX
            return DevExpressHelper.GetUrl(new { Controller = "Printform", Action = "Index", Area = "Reports" }) + string.Format(@"/index/{0}/{1}/{2}", 0, value.Id, "REPORT");
        }
    }
}