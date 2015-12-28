using System;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Models;
using BusinessObjects.Print;
using BusinessObjects.Security;
using DocumentsWeb.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace DocumentsWeb.Areas.Reports.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class PrintformController : Controller
    {
        //
        
        public ActionResult Index(int? docid, int? repid, string docprint)
        {
            ViewResult r = View();  
            //r.ViewData.Add("PrmValues", this.Request.QueryString);
            return r;
        }
        //http://localhost:4053/dgmz/Reports/Printform/index?id=1207152&repid=1072&docprint=CONTRACTORGTEX
        public ActionResult GetReportSnapshot()
        {

            RouteValueDictionary routeValues = StiMvcViewerFxHelper.GetRouteValues(this.Request);

            int documentId = 0;
            if (routeValues["docid"] != null) documentId = Convert.ToInt32(routeValues["docid"]);

            //// Идентификатор отчета
            int reportId = 0;
            if (routeValues["repId"] != null) reportId = Convert.ToInt32(routeValues["repId"]);

            string pringModel = string.Empty;
            if (routeValues["docprint"] != null) pringModel = routeValues["docprint"].ToString();

            Library lib = WADataProvider.WA.GetObject<Library>(reportId);
            StiReport report = new StiReport();
            ElementRightView coll = WADataProvider.WA.Access.ElementRightView((int)WhellKnownDbEntity.Library, WADataProvider.CurrentUserName);
            if (lib.KindId== Library.KINDID_WEBREPORT && !coll.IsAllow(Right.UIREPORTBUILD, reportId))
            {
                Library libErr = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("SYSREP_RINTFORMSECURE");
                report.Load(libErr.GetSource());
                return StiMvcViewerFxHelper.GetReportSnapshotResult(report, this.Request);
                
            }
            if (lib.KindId == Library.KINDID_WEBPRINTFORM && !coll.IsAllow(Right.UIPRINT, reportId))
            {
                Library libErr = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("SYSREP_RINTFORMSECURE");
                report.Load(libErr.GetSource());
                return StiMvcViewerFxHelper.GetReportSnapshotResult(report, this.Request);

            }
            

            
            if (lib != null && lib.Id != 0)
            {
                report.Load(lib.GetSource());
                //report.Dictionary.DataStore.Clear();
            }
            #region Определение модели документа
            if (pringModel == "SALES")
            {
                DocumentSales sourceDoc = new DocumentSales { Workarea = WADataProvider.WA };

                sourceDoc.Load(documentId);

                PreparePrintSales doc = new PreparePrintSales { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }
            if (pringModel == "FINANCES")
            {
                DocumentFinance sourceDoc = new DocumentFinance { Workarea = WADataProvider.WA };

                sourceDoc.Load(documentId);

                PreparePrintFinance doc = new PreparePrintFinance { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }
            if (pringModel == "SERVICES")
            {
                DocumentService sourceDoc = new DocumentService { Workarea = WADataProvider.WA };

                sourceDoc.Load(documentId);

                PreparePrintServices doc = new PreparePrintServices { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }
            if (pringModel == "PRICES")
            {
                DocumentPrices sourceDoc = new DocumentPrices { Workarea = WADataProvider.WA };
                sourceDoc.Load(documentId);

                PreparePrintPrices doc = new PreparePrintPrices { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }

            if (pringModel == "TAXES")
            {
                DocumentTaxes sourceDoc = new DocumentTaxes { Workarea = WADataProvider.WA };
                sourceDoc.Load(documentId);

                PreparePrintTaxes doc = new PreparePrintTaxes { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }

            if (pringModel == "PERSONAL")
            {
                DocumentPerson sourceDoc = new DocumentPerson { Workarea = WADataProvider.WA };
                sourceDoc.Load(documentId);

                PreparePrintPerson doc = new PreparePrintPerson { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
            }

            if (pringModel == "CONTRACTORGTEX")
            {
                DocumentContract sourceDoc = new DocumentContract { Workarea = WADataProvider.WA };
                sourceDoc.Load(documentId);

                PreparePrintContractPrinter doc = new PreparePrintContractPrinter { SourceDocument = sourceDoc };
                report.RegData("Document", doc.PrintHeader);
                report.RegData("DocumentDetail", doc.PrintData);
            }
            if (pringModel == "MESSAGE")
            {
                Message m = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(documentId);
                MessageModel model = new MessageModel();
                model.GetData(m);

                report.RegData("Document", model);
            }
            if (pringModel == "TASK")
            {
                Task task = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(documentId);
                TaskModel model = new TaskModel();
                model.GetData(task);
                report.RegData("Document", model);
            }
            #endregion

            if (report.Dictionary.Variables.Contains("rootserver"))
            {
                report["rootserver"] =
                   WADataProvider.WA.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
            }
            if (report.Dictionary.Variables.Contains("CurrentUidName"))
            {
                report["CurrentUidName"] = WADataProvider.CurrentUser.AgentId == 0 ? string.Empty : WADataProvider.CurrentUser.Agent.Name;
            }

            if (pringModel == "REPORT")
            {
                report["UserName"] = WADataProvider.CurrentUserName;
                report["ds"] = WADataProvider.WA.Period.Start;
                report["de"] = WADataProvider.WA.Period.End;
                report["UserId"] = WADataProvider.WA.CurrentUser.Id;
                report["Id"] = documentId;

                //report.Dictionary.DataStore.Clear();
                //int idx = report.Dictionary.Databases.IndexOf("MainCnn");
                //if (idx > -1)
                //{
                //    report.Dictionary.Databases.RemoveAt(idx);
                //    report.RegData("MainCnn", WADataProvider.WA.MyBranche.GetDatabaseConnection());
                //}
                report.RegData("MainCnn", WADataProvider.WA.MyBranche.GetDatabaseConnection());
                report.Compile();
                
                report["UserName"] = WADataProvider.CurrentUserName;
                report["UserId"] = WADataProvider.WA.CurrentUser.Id;
                report["Id"] = documentId;
                if (report.Dictionary.Variables.Contains("rootserver"))
                {
                    report["rootserver"] =
                       WADataProvider.WA.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
                }
                if (report.Dictionary.Variables.Contains("CurrentUidName"))
                {
                    report["CurrentUidName"] = WADataProvider.CurrentUser.AgentId == 0 ? string.Empty : WADataProvider.CurrentUser.Agent.Name;
                }
                bool IsDStartRequestUser = false;
                if (report.Dictionary.Variables.Contains("ds"))
                {
                    IsDStartRequestUser = report.Dictionary.Variables["ds"].RequestFromUser;
                    if (!IsDStartRequestUser)
                        report["ds"] = WADataProvider.WA.Period.Start;
                }

                bool IsDEndRequestUser = false;
                if (report.Dictionary.Variables.Contains("de"))
                {
                    IsDEndRequestUser = report.Dictionary.Variables["de"].RequestFromUser;
                    if (!IsDEndRequestUser)
                        report["de"] = WADataProvider.WA.Period.End;
                }
                if (report.CompiledReport.DataSources.Contains("DataSourceMain"))
                {
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@Id"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@Id"].ParameterValue =
                            documentId;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@ds") && !IsDStartRequestUser)
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@ds"].ParameterValue =
                            WADataProvider.WA.Period.Start;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@de") && !IsDEndRequestUser)
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@de"].ParameterValue =
                            WADataProvider.WA.Period.End;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserId"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserId"].ParameterValue =
                            WADataProvider.CurrentUser.Id;
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserName"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserName"].ParameterValue =
                            WADataProvider.CurrentUserName;
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@MyCompanyId"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@MyCompanyId"].ParameterValue =
                            WADataProvider.CurrentUser.MyCompanyId;
                }
                if (report.CompiledReport.DataSources.Contains("AllowedCompany"))
                {
                    if (report.CompiledReport.DataSources["AllowedCompany"].Parameters.Contains("@UserName"))
                        report.CompiledReport.DataSources["AllowedCompany"].Parameters["@UserName"].ParameterValue =
                            WADataProvider.CurrentUserName;
                }
            }
            //try
            //{
            //    report.Compile();
            //}
            //catch (Exception)
            //{
                
            //    throw;
            //}

            
            return StiMvcViewerFxHelper.GetReportSnapshotResult(report, this.Request);
        }
        public ActionResult GetLocalization()
        {
            // Load the viewer localization file
            string path = Server.MapPath("~/Content/Localization/ru.xml");
            return StiMvcViewerFxHelper.GetLocalizationResult(path);
        }
        public FileResult ExportReport()
        {
            // Return the exported report file
            return StiMvcViewerFxHelper.ExportReportResult(this.Request);
        }

        public FileResult ExportReport2()
        {
            // Use the following code to make some changes with report, if required
            StiReport report = StiMvcViewerFxHelper.GetReportObject(this.Request);

            // ...
            // Some changes with report object
            // ...

            return StiMvcViewerFxHelper.ExportReportResult(report, this.Request);
        }

    }
}