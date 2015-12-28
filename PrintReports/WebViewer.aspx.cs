using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Print;
using BusinessObjects.Security;
using Stimulsoft.Report;
using Stimulsoft.Report.Web;

public partial class WebViewer : System.Web.UI.Page
{
    //Thread.CurrentThread.CurrentCulture 
    //http://localhost:5898/PrintReports/WebViewer.aspx?repId=1005&userName=demo&userpsw=78DB459747F424C6F08108D31ED647CFF4929F88&MyCompanyId=15562
    //http://localhost:5898/PrintReports/WebViewer.aspx?repId=650&userName=WebAdmin&userpsw=13812338E0A7CD1A8A16DCAF8C22E2415C082691
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //StiWebViewer1.ReportConnect += new Stimulsoft.Report.Web.StiReportDataEventHandler(StiWebViewer1_ReportConnect);
            //StiWebViewer1.ReportAfterDialogResult += new Stimulsoft.Report.Web.StiReportDialogResultEventHandler(StiWebViewer1_ReportAfterDialogResult);
            int repId = 0;
            int id = 0;
            int MyCompanyId = 0;
            string userName = string.Empty;
            string userpsw = string.Empty;
            bool RepIdParamValid = false;

            /*ƒополнительные QueryString дл€ авто експорта*/
            string paramRepAutoExport = Request.QueryString["repExport"];
            string paramValueExport = string.Empty;
            if (paramRepAutoExport != null)
            {
                paramValueExport = paramRepAutoExport;
            }

            string paramRepString = Request.QueryString["repId"];
            if (paramRepString != null)
            {
                RepIdParamValid = Int32.TryParse(paramRepString, out repId);
            }

            string paramRepStringId = Request.QueryString["Id"];
            if (paramRepString != null)
            {
                RepIdParamValid = Int32.TryParse(paramRepStringId, out id);
            }

            string paramMyCompanyStringId = Request.QueryString["MyCompanyId"];
            if (paramMyCompanyStringId != null)
            {
                RepIdParamValid = Int32.TryParse(paramMyCompanyStringId, out MyCompanyId);
            }
            string paramUserNameStringId = Request.QueryString["UserName"];
            if (paramUserNameStringId != null)
            {
                userName = paramUserNameStringId;
            }
            string paramUserPaswStringId = Request.QueryString["userpsw"];
            if (paramUserPaswStringId != null)
            {
                userpsw = paramUserPaswStringId;
            }
            string pringModel = string.Empty;
            bool IsPrintDocument = false;
            string paramDocPrintModelId = Request.QueryString["docprint"];
            if (paramDocPrintModelId != null)
            {
                pringModel = paramDocPrintModelId.ToUpper();
                IsPrintDocument = true;
            }
            //string appDirectory = HttpContext.Current.Server.MapPath(string.Empty);

            //// Prepare data
            //DataSet dataSet = new DataSet();
            //dataSet.ReadXml(appDirectory + "\\Data\\Demo.xml");
            //dataSet.ReadXmlSchema(appDirectory + "\\Data\\Demo.xsd");

            //// Load report
            //StiReport report = new StiReport();
            //report.Load(appDirectory + "\\Reports\\SimpleList.mrt");
            //report.RegData(dataSet);

            Uid user = null;

            Workarea wa = OpenDataBase(out user);
            // проверка пользовател€ и парол€
            if (!Uid.IsUserPasswordHashValid(wa, userName, userpsw))
            {
                Response.Redirect("secureNotAllow.aspx", true);
            }
            // проверка компании

            // проверка доступности отчета
            ElementRightView coll = wa.Access.ElementRightView((int) WhellKnownDbEntity.Library, userName);
            if (!coll.IsAllow("VIEW", repId))
            {
                Response.Redirect("secureNotAllow.aspx", true);
            }

            Uid currentUid = wa.Access.GetAllUsers().FirstOrDefault(f => f.Name.ToUpper() == userName.ToUpper());
            Library lib = wa.GetObject<Library>(repId);

            if (currentUid == null || lib == null || lib.Id == 0)
            {
                Response.Redirect("secureNotAllow.aspx", true);
            }
            // StiReport report = Stimulsoft.Report.StiReport.GetReportFromAssembly(lib.GetAssembly());

            StiReport report = new StiReport();
            report.Load(lib.GetSource());
            report.Dictionary.DataStore.Clear();
            int idx = report.Dictionary.Databases.IndexOf("MainCnn");
            if (idx > -1)
            {
                report.Dictionary.Databases.RemoveAt(idx);
                report.RegData("MainCnn", wa.MyBranche.GetDatabaseConnection());
            }


            //DateTime? v = (DateTime?)wa.Period.Start;
            //        report["UserName"] = wa.CurrentUser.Name;
            //        report["ds"] = wa.Period.Start;
            //        report["de"] = wa.Period.End;
            //        report["UserId"] = wa.CurrentUser.Id;
            //        report["ElementId"] = id;
            ////        if (toValue != null)
            ////            report["Id"] = toValue.Id;
            //        report.Dictionary.Synchronize();
            //report.Dictionary.Variables["xxx"].ValueObject
            if (IsPrintDocument)
            {
                if (pringModel == "SALES")
                {
                    DocumentSales sourceDoc = new DocumentSales {Workarea = wa};

                    sourceDoc.Load(id);

                    PreparePrintSales doc = new PreparePrintSales {SourceDocument = sourceDoc};
                    report.RegData("Document", doc.PrintHeader);
                    report.RegData("DocumentDetail", doc.PrintData);
                }
                if (pringModel == "FINANCES")
                {
                    DocumentFinance sourceDoc = new DocumentFinance {Workarea = wa};

                    sourceDoc.Load(id);

                    PreparePrintFinance doc = new PreparePrintFinance {SourceDocument = sourceDoc};
                    report.RegData("Document", doc.PrintHeader);
                    report.RegData("DocumentDetail", doc.PrintData);
                }
                if (pringModel == "SERVICERS")
                {
                    DocumentService sourceDoc = new DocumentService {Workarea = wa};

                    sourceDoc.Load(id);

                    PreparePrintServices doc = new PreparePrintServices {SourceDocument = sourceDoc};
                    report.RegData("Document", doc.PrintHeader);
                    report.RegData("DocumentDetail", doc.PrintData);
                }
                if (pringModel == "PRICES")
                {
                    DocumentPrices sourceDoc = new DocumentPrices {Workarea = wa};
                    sourceDoc.Load(id);

                    PreparePrintPrices doc = new PreparePrintPrices {SourceDocument = sourceDoc};
                    report.RegData("Document", doc.PrintHeader);
                    report.RegData("DocumentDetail", doc.PrintData);
                }
                if (pringModel == "PERSONAL")
                {
                    DocumentPerson sourceDoc = new DocumentPerson { Workarea = wa };
                    sourceDoc.Load(id);

                    PreparePrintPerson doc = new PreparePrintPerson { SourceDocument = sourceDoc };
                    report.RegData("Document", doc.PrintHeader);
                }
                if (pringModel == "CONTRACTORGTEX")
                {
                    DocumentContract sourceDoc = new DocumentContract { Workarea = wa };
                    sourceDoc.Load(id);

                    PreparePrintContractPrinter doc = new PreparePrintContractPrinter { SourceDocument = sourceDoc };
                    report.RegData("Document", doc.PrintHeader);
                    report.RegData("DocumentDetail", doc.PrintData);
                }
            }
            if (!IsPrintDocument)
            {
                report["UserName"] = userName;
                report["ds"] = wa.Period.Start;
                report["de"] = wa.Period.End;
                report["UserId"] = wa.CurrentUser.Id;
                report["Id"] = id;
                if (report.Dictionary.Variables.Contains("rootserver"))
                {
                    report["rootserver"] =
                        wa.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
                }
                if (report.Dictionary.Variables.Contains("CurrentUidName"))
                {
                    report["CurrentUidName"] = currentUid.AgentId == 0 ? userName : currentUid.Agent.Name;
                }
                //report.Dictionary.Synchronize();
                report.Compile();

                if (report.Dictionary.Variables.Contains("rootserver"))
                {
                    report["rootserver"] =
                        wa.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
                }

                //if (report.Dictionary.Variables.Contains("CurrentUidName"))
                //{
                //    report["CurrentUidName"] = currentUid.AgentId == 0 ? userName : currentUid.Agent.Name;
                //}
                

                bool IsDStartRequestUser = false;
                if(report.Dictionary.Variables.Contains("ds"))
                {
                    IsDStartRequestUser = report.Dictionary.Variables["ds"].RequestFromUser;
                    if (!IsDStartRequestUser)
                        report["ds"] = wa.Period.Start;
                }

                bool IsDEndRequestUser = false;
                if (report.Dictionary.Variables.Contains("de"))
                {
                    IsDEndRequestUser = report.Dictionary.Variables["de"].RequestFromUser;
                    if (!IsDEndRequestUser)
                        report["de"] = wa.Period.End;
                }
                    

                report["UserName"] = userName;
                report["UserId"] = wa.CurrentUser.Id;
                report["Id"] = id;
                if (report.CompiledReport.DataSources.Contains("DataSourceMain"))
                {
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@Id"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@Id"].ParameterValue = id;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@ds") && !IsDStartRequestUser)
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@ds"].ParameterValue =
                            wa.Period.Start;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@de") && !IsDEndRequestUser)
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@de"].ParameterValue =
                            wa.Period.End;

                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserId"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserId"].ParameterValue =
                            currentUid.Id;
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserName"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserName"].ParameterValue =
                            userName;
                    if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@MyCompanyId"))
                        report.CompiledReport.DataSources["DataSourceMain"].Parameters["@MyCompanyId"].ParameterValue =
                            MyCompanyId;
                }
                if (report.CompiledReport.DataSources.Contains("AllowedCompany"))
                {
                    if (report.CompiledReport.DataSources["AllowedCompany"].Parameters.Contains("@UserName"))
                        report.CompiledReport.DataSources["AllowedCompany"].Parameters["@UserName"].ParameterValue =
                            userName;
                }
                //report.Dictionary.Variables["Id"].ValueObject = id;
            }
            report.Render();
            /*
             StiReport report = StiReportWeb1.GetReport();        
            report.Dictionary.DataStore.Clear();

            System.Data.OleDb.OleDbConnection connection =
                new System.Data.OleDb.OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=\"Nwind.mdb\"");

            report.RegData("NorthWind", connection);
            report.Compile();
            report.CompiledReport.DataSources["Customers"].Parameters["@countryID"].ParameterValue = ListBox1.SelectedItem.Value;

            report.Render();
            StiWebViewer1.Report = report;
             */

            //report.ExportDocument(StiExportFormat.Pdf, "document.pdf");
            if (paramRepAutoExport != null)
            {
                if (paramValueExport == "EXCEL")
                {
                    StiWebViewer1.Report = report;
                    StiReportResponse.ResponseAsExcel2007(this, report);
                    //StiWebViewer1.Report.ExportDocument(StiExportFormat.Excel2007, string.Format("{0}.xlsx", report.ReportName));
                    //report.ExportDocument(StiExportFormat.Excel2007, string.Format("{0}.xlsx", report.ReportName));
                }
                else if (paramValueExport == "PDF")
                {
                    StiWebViewer1.Report = report;
                    StiWebViewer1.PrintToPdf();
                    //report.ExportDocument(StiExportFormat.Excel2007, string.Format("{0}.pdf", report.ReportName));
                }
            }
            else
            {
                // View report
                StiWebViewer1.Report = report;
            }
        }

        //StiWebViewer1.GetReportData += new Stimulsoft.Report.Web.StiReportDataEventHandler(StiWebViewer1_GetReportData);
    }

    protected void StiWebViewer1_GetReportData(object sender, Stimulsoft.Report.Web.StiReportDataEventArgs e)
    {
        int repId = 0;
        int id = 0;
        int MyCompanyId = 0;
        string userName = string.Empty;
        string userpsw = string.Empty;
        bool RepIdParamValid = false;
        string paramRepString = Request.QueryString["repId"];
        if (paramRepString != null)
        {
            RepIdParamValid = Int32.TryParse(paramRepString, out repId);
        }

        string paramRepStringId = Request.QueryString["Id"];
        if (paramRepString != null)
        {
            RepIdParamValid = Int32.TryParse(paramRepStringId, out id);
        }

        string paramMyCompanyStringId = Request.QueryString["MyCompanyId"];
        if (paramMyCompanyStringId != null)
        {
            RepIdParamValid = Int32.TryParse(paramMyCompanyStringId, out MyCompanyId);
        }
        string paramUserNameStringId = Request.QueryString["UserName"];
        if (paramUserNameStringId != null)
        {
            userName = paramUserNameStringId;
        }
        string paramUserPaswStringId = Request.QueryString["userpsw"];
        if (paramUserPaswStringId != null)
        {
            userpsw = paramUserPaswStringId;
        }
        string pringModel = string.Empty;
        bool IsPrintDocument = false;
        string paramDocPrintModelId = Request.QueryString["docprint"];
        if (paramDocPrintModelId != null)
        {
            pringModel = paramDocPrintModelId.ToUpper();
            IsPrintDocument = true;
        }
        
        Uid user = null;

        Workarea wa = OpenDataBase(out user);
        // проверка пользовател€ и парол€
        if (!Uid.IsUserPasswordHashValid(wa, userName, userpsw))
        {
            Response.Redirect("secureNotAllow.aspx", true);
        }
        // проверка компании

        // проверка доступности отчета
        ElementRightView coll = wa.Access.ElementRightView((int)WhellKnownDbEntity.Library, userName);
        if (!coll.IsAllow("VIEW", repId))
        {
            Response.Redirect("secureNotAllow.aspx", true);
        }

        Uid currentUid = wa.Access.GetAllUsers().FirstOrDefault(f => f.Name.ToUpper() == userName.ToUpper());
        Library lib = wa.GetObject<Library>(repId);


        StiReport report = e.Report;
        //if(report.data)
        report.RegData("MainCnn", wa.MyBranche.GetDatabaseConnection());
        report["UserName"] = userName;

        if (report.Dictionary.Variables.Contains("rootserver"))
        {
            report["rootserver"] =
                wa.Cashe.SystemParameters.ItemCode<SystemParameter>("WEBROOTSERVER").ValueString;
        }
        if (report.Dictionary.Variables.Contains("CurrentUidName"))
        {
            report["CurrentUidName"] = currentUid.AgentId==0?userName: currentUid.Agent.Name;
        }

        if (!IsPrintDocument)
        {


            report["UserName"] = userName;
            //report["ds"] = wa.Period.Start;
            //report["de"] = wa.Period.End;
            report["UserId"] = wa.CurrentUser.Id;
            report["Id"] = id;
            //report.Dictionary.Synchronize();

            //if (!StiWebViewer1.Report.IsCompiled)
            //if (!report.IsCompiled)
            //    report.Compile();
            
            bool IsDStartRequestUser = report.Dictionary.Variables["ds"].RequestFromUser;
            if (!IsDStartRequestUser)
                report["ds"] = wa.Period.Start;
            bool IsDEndRequestUser = report.Dictionary.Variables["de"].RequestFromUser;
            if (!IsDEndRequestUser)
                report["de"] = wa.Period.End;
            report["UserId"] = wa.CurrentUser.Id;
            report["Id"] = id;
            if (report.CompiledReport.DataSources.Contains("DataSourceMain"))
            {
                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@Id"))
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@Id"].ParameterValue = id;

                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@ds") && !IsDStartRequestUser)
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@ds"].ParameterValue =
                        wa.Period.Start;

                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@de") && !IsDEndRequestUser )
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@de"].ParameterValue =
                        wa.Period.End;

                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserId"))
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserId"].ParameterValue =
                        currentUid.Id;
                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@UserName"))
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@UserName"].ParameterValue =
                        userName;
                if (report.CompiledReport.DataSources["DataSourceMain"].Parameters.Contains("@MyCompanyId"))
                    report.CompiledReport.DataSources["DataSourceMain"].Parameters["@MyCompanyId"].ParameterValue =
                        MyCompanyId;
            }
            if (report.CompiledReport.DataSources.Contains("AllowedCompany"))
            {
                if (report.CompiledReport.DataSources["AllowedCompany"].Parameters.Contains("@UserName"))
                    report.CompiledReport.DataSources["AllowedCompany"].Parameters["@UserName"].ParameterValue =
                        userName;
            }
        }
        
        
        report.Render();
        
    }

    protected void StiWebViewer1_ReportConnect(object sender, Stimulsoft.Report.Web.StiReportDataEventArgs e)
    {
        Console.WriteLine(e.Report.DataStore.Count);
        //if(e.Report.DataStore.Count==0)
        //{
        //    Uid user = null;
        //    Workarea wa = OpenDataBase(out user);
        //    e.Report.RegData("MainCnn", wa.MyBranche.GetDatabaseConnection());
        //}
        //throw new NotImplementedException();
    }

    void StiWebViewer1_ReportAfterDialogResult(object sender, Stimulsoft.Report.Web.StiReportDialogResultEventArgs e)
    {
        //throw new NotImplementedException();
    }
    private static Workarea OpenDataBase(out Uid user)
    {
        string cnnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Document2011"].ConnectionString;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cnnStr);

        user = new Uid { Name = builder.UserID, Password = builder.Password, AuthenticateKind = (int)AuthenticateKind.SqlServer };


        Workarea WA = new Workarea();
        WA.ConnectionString = builder.ConnectionString;
        try
        {
            if (WA.LogOn(user.Name))
            {
                return WA;
            }
        }
        catch (SqlException sqlEx)
        {

        }
        catch (Exception ex)
        {

        }
        return null;
    }
}
