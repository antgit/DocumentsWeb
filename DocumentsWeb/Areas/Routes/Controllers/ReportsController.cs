using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Models;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Routes.Models;
using Stimulsoft.Report.Dictionary;
using System.Data.SqlClient;

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class ReportsController : Controller
    {
        #region Отчет о пропусках заданий (RouteSkippingTasks)

        public ActionResult RouteSkippingTasks()
        {
            return View();
        }

        public ActionResult RouteSkippingTasksSnapshot()
        {
            DateTime? DateValue = (DateTime?)this.Session["RouteSkippingTasks_Date"];
            int? AutoValue = (int?)this.Session["RouteSkippingTasks_Auto"];
            int? PlanValue = (int?)this.Session["RouteSkippingTasks_Plan"];

            Library lib = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("WEBREPORT_ROUTE_SKIPPING_TASKS");
            StiReport report = new StiReport();
            if (lib != null && lib.Id > 0)
            {
                report.Load(lib.GetSource());
            }

            if (DateValue.HasValue && AutoValue.HasValue && PlanValue.HasValue)
            {
                ReportRouteSkippingTasksHeaderModel head = new ReportRouteSkippingTasksHeaderModel();
                List<ReportRouteSkippingTasksDetailModel> list = new List<ReportRouteSkippingTasksDetailModel>();

                head.TPName = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(3).Name;
                SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "Route.XReportSkippingTasks";
                cmd.Parameters.Add("@Date", System.Data.SqlDbType.Date).Value = DateValue;
                cmd.Parameters.Add("@RouteTemplateId", System.Data.SqlDbType.Int).Value = PlanValue;
                cmd.Parameters.Add("@RouteMemberId", System.Data.SqlDbType.Int).Value = AutoValue;

                SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    head.StartTime = rd.IsDBNull(rd.GetOrdinal("StartTrack")) ? new TimeSpan() : (new DateTime(((TimeSpan)rd["StartTrack"]).Ticks)).ToLocalTime().TimeOfDay;
                    head.EndTime = rd.IsDBNull(rd.GetOrdinal("EndTrack")) ? new TimeSpan() : (new DateTime(((TimeSpan)rd["EndTrack"]).Ticks)).ToLocalTime().TimeOfDay;
                    head.Date = DateValue.Value;
                }

                if (rd.NextResult())
                {
                    if (rd.Read())
                    {
                        head.RouteLength = rd["PathLength"].ToString() + " км.";
                    }
                }

                int n = 1;
                if (rd.NextResult())
                {
                    while (rd.Read())
                    {
                        ReportRouteSkippingTasksDetailModel item = new ReportRouteSkippingTasksDetailModel
                        {
                            Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? "" : (string)rd["Address"],
                            FactTime = rd.IsDBNull(rd.GetOrdinal("FactTime")) ? new TimeSpan() : (new DateTime(((TimeSpan)rd["FactTime"]).Ticks)).ToLocalTime().TimeOfDay,
                            Name = rd.IsDBNull(rd.GetOrdinal("Name")) ? "" : (string)rd["Name"],
                            PlanTime = rd.IsDBNull(rd.GetOrdinal("PlanTime")) ? new TimeSpan() : (TimeSpan)rd["PlanTime"],
                            Town = rd.IsDBNull(rd.GetOrdinal("Town")) ? "" : (string)rd["Town"],
                            VisitLength = rd.IsDBNull(rd.GetOrdinal("VisitLength")) ? new TimeSpan() : (TimeSpan)rd["VisitLength"],
                            Npp = n
                        };
                        list.Add(item);
                        n++;
                    }
                }

                if (rd.NextResult())
                {
                    while (rd.Read())
                    {
                        if (!rd.IsDBNull(rd.GetOrdinal("VisitLength")) && ((TimeSpan)rd["VisitLength"]).Ticks != 0)
                        {
                            ReportRouteSkippingTasksDetailModel item = new ReportRouteSkippingTasksDetailModel
                            {
                                Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? "" : (string)rd["Address"],
                                FactTime = rd.IsDBNull(rd.GetOrdinal("FactTime")) ? new TimeSpan() : (new DateTime(((TimeSpan)rd["FactTime"]).Ticks)).ToLocalTime().TimeOfDay,
                                Name = rd.IsDBNull(rd.GetOrdinal("Name")) ? "" : (string)rd["Name"],
                                PlanTime = rd.IsDBNull(rd.GetOrdinal("PlanTime")) ? new TimeSpan() : (TimeSpan)rd["PlanTime"],
                                Town = rd.IsDBNull(rd.GetOrdinal("Town")) ? "" : (string)rd["Town"],
                                VisitLength = rd.IsDBNull(rd.GetOrdinal("VisitLength")) ? new TimeSpan() : (TimeSpan)rd["VisitLength"],
                                Npp = n++
                            };
                            list.Add(item);
                        }
                    }
                }

                rd.Close();
                con.Close();

                report.RegData("RouteSkippingTasksHeader", head);
                report.RegData("RouteSkippingTasks", list);

                if (report.Dictionary.Variables.Contains("rootserver"))
                {
                    report["rootserver"] = WADataProvider.WA.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
                }
            }
            return StiMvcViewerFxHelper.GetReportSnapshotResult(report, this.Request);
        }

        [HttpPost]
        public ActionResult RouteSkippingTasksSubmit(DateTime? DateValue, int? AutoValue_VI, int? PlanValue_VI)
        {
            this.Session["RouteSkippingTasks_Date"] = DateValue;
            this.Session["RouteSkippingTasks_Auto"] = AutoValue_VI;
            this.Session["RouteSkippingTasks_Plan"] = PlanValue_VI;

            return View("RouteSkippingTasks");
        }

        public ActionResult RouteSkippingTasksPlanPartial()
        {
            this.Session["RouteSkippingTasks_Plan"] = null;
            PartialViewResult res = PartialView();
            res.ViewData.Add("RouteMemberId", Request.Params["RouteMemberId"]);
            return res;
        }

        #endregion

        public ActionResult GetLocalization()
        {
            string path = Server.MapPath("~/Content/Localization/ru.xml");
            return StiMvcViewerFxHelper.GetLocalizationResult(path);
        }

        public FileResult ExportReport()
        {
            return StiMvcViewerFxHelper.ExportReportResult(this.Request);
        }
    }
}
