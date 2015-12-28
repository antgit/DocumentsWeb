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
using Stimulsoft.Report.Dictionary;
using System.Data.SqlClient;
using DocumentsWeb.Areas.Marketings.Models;

namespace DocumentsWeb.Areas.Marketings.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class ReportsController : Controller
    {
        #region  SKU по молочке
        
        public ActionResult SKUByMilk()
        {
            return View();
        }

        public ActionResult SKUByMilkSnapshot()
        {
            DateTime? DateValue = (DateTime?)this.Session["SKUByMilk_Date"];
            int? DepatmentValue = (int?)this.Session["SKUByMilk_Depatment"];
            int i = 1;

            Library lib = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("WEBREPORT_MKTG_SKU_BY_MILK");
            StiReport report = new StiReport();
            if (lib != null && lib.Id > 0)
            {
                report.Load(lib.GetSource());
            }

            if (DateValue.HasValue && DepatmentValue.HasValue)
            {
                ReportSKUByMilkHeaderModel head = new ReportSKUByMilkHeaderModel();
                List<ReportSKUByMilkDetailModel> list = new List<ReportSKUByMilkDetailModel>();

                head.Date = DateValue.Value;//WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(3).Name;
                SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "Mktg.XReportSKUByMilk";
                cmd.Parameters.Add("@Date", System.Data.SqlDbType.Date).Value = DateValue;
                if (DepatmentValue.Value > 0)
                {
                    cmd.Parameters.Add("@DepatmentId", System.Data.SqlDbType.Int).Value = DepatmentValue.Value;
                }

                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    ReportSKUByMilkDetailModel row = new ReportSKUByMilkDetailModel
                    {
                        Depatment = rd.IsDBNull(rd.GetOrdinal("Depatment")) ? "" : (string)rd["Depatment"],
                        KefirSKU = rd.IsDBNull(rd.GetOrdinal("KefirSKU")) ? 0 : (decimal)rd["KefirSKU"],
                        MasloSKU = rd.IsDBNull(rd.GetOrdinal("MasloSKU")) ? 0 : (decimal)rd["MasloSKU"],
                        MilkSKU = rd.IsDBNull(rd.GetOrdinal("MilkSKU")) ? 0 : (decimal)rd["MilkSKU"],
                        SmetanaSKU = rd.IsDBNull(rd.GetOrdinal("SmetanaSKU")) ? 0 : (decimal)rd["SmetanaSKU"],
                        TM = rd.IsDBNull(rd.GetOrdinal("TM")) ? "" : (string)rd["TM"],
                        TRTAddress = rd.IsDBNull(rd.GetOrdinal("TRTAddress")) ? "" : (string)rd["TRTAddress"],
                        TRTFactName = rd.IsDBNull(rd.GetOrdinal("TRTFactName")) ? "" : (string)rd["TRTFactName"],
                        TRTOutletLocation = rd.IsDBNull(rd.GetOrdinal("TRTOutletLocation")) ? "" : (string)rd["TRTOutletLocation"],
                        TRTUrName = rd.IsDBNull(rd.GetOrdinal("TRTUrName")) ? "" : (string)rd["TRTUrName"],
                        Npp = i++
                    };
                    list.Add(row);
                }
                
                rd.Close();
                con.Close();

                report.RegData("SKUByMilkHeader", head);
                report.RegData("SKUByMilk", list);

                if (report.Dictionary.Variables.Contains("rootserver"))
                {
                    report["rootserver"] = WADataProvider.WA.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
                }
            }
            return StiMvcViewerFxHelper.GetReportSnapshotResult(report, this.Request);
        }

        [HttpPost]
        public ActionResult SKUByMilkSubmit(DateTime? DateValue, int? DepatmentValue_VI)
        {
            this.Session["SKUByMilk_Date"] = DateValue;
            this.Session["SKUByMilk_Depatment"] = DepatmentValue_VI;

            return View("SKUByMilk");
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
