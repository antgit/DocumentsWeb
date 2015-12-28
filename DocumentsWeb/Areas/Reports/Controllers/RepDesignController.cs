using System.Data;
using System.Web.Mvc;
using System.Web.Routing;
using Stimulsoft.Report;
using Stimulsoft.Report.MvcDesign;

namespace DocumentsWeb.Areas.Reports.Controllers
{
    public class RepDesignController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View("Index");
        }
        

        public ActionResult NewReport()
        {
            return View("Design");
        }

        public ActionResult EditReport()
        {
            return View("Design");
        }

        public ActionResult GetReportTemplate()
        {
            // Create a report object
            StiReport report = new StiReport();

            // Restore the route values collection and load the report template, if necessary
            RouteValueDictionary routeValues = StiMvcDesignerHelper.GetRouteValues(this.Request);
            if ((string)routeValues["action"] == "EditReport") report.Load(Server.MapPath("~/Content/Reports/SimpleList.mrt"));

            return StiMvcDesignerHelper.GetReportTemplateResult(report);
        }

        public ActionResult SaveReportTemplate()
        {
            // Get the report template
            StiReport report = StiMvcDesignerHelper.GetReportObject(this.Request);

            // Save the report template to the packed string (for example)
            string packedReport = report.SavePackedReportToString();
            // ...
            // The save report code here
            // ...

            // Completion of the report saving without dialog box
            return StiMvcDesignerHelper.SaveReportResult(false);

            // Completion of the report saving with dialog box
            //return StiMvcDesignerHelper.SaveReportResult(true);

            // Completion of the report saving with message dialog box
            //return StiMvcDesignerHelper.SaveReportResult("Some message after saving");

            // Completion of the report saving with error dialog box
            //return StiMvcDesignerHelper.SaveReportResult(10);
        }

        public ActionResult GetReportSnapshot()
        {
            // Get the report template
            StiReport report = StiMvcDesignerHelper.GetReportObject(this.Request);

            // Register data, if necessary
            DataSet data = new DataSet("Demo");
            data.ReadXml(Server.MapPath("~/Content/Demo.xml"));
            report.Dictionary.Databases.Clear();
            report.RegData(data);

            // Return the report snapshot result to the client
            return StiMvcDesignerHelper.GetReportSnapshotResult(report, this.Request);
        }

        public FileResult ExportReport()
        {
            // Return the exported report file
            return StiMvcDesignerHelper.ExportReportResult(this.Request);
        }

        public ActionResult GetLocalization()
        {
            // Load the localization file requested by the designer
            string name = StiMvcDesignerHelper.GetLocalizationName(this.Request);
            string path = Server.MapPath("~/Content/Localization/");

            return StiMvcDesignerHelper.GetLocalizationResult(path + name);
        }

        public ActionResult ExitDesigner()
        {
            // Return the Index page
            //return Index();
            return View("Index");
        }

        public ActionResult DataProcessing()
        {
            // Necessary for work the designers data dictionary and some components
            return StiMvcDesignerHelper.DataProcessingResult(this.Request);
        }

        public ActionResult GoogleDocs()
        {
            // Necessary for work with Google Docs
            return StiMvcDesignerHelper.GoogleDocsResult(this.Request);
        }

    }
}
