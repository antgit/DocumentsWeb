using System;
using System.Activities;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Workflows.Web;
using DocumentsWeb.Models;
using BusinessObjects.Web.Core;
using System.Text.RegularExpressions;

namespace DocumentsWeb.Areas.Commons.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        //
        // GET: /Commons/CompanyRegistration/

        public ActionResult Index()
        {
            if (HttpContext.Request.Cookies["Registry"] != null)
            {
                return RedirectPermanent("~/Commons/CompanyRegistration/AlreadyRegistered");
            }
            else
            {
                if (WADataProvider.SysConfig.SolveRegistryCompany)
                {
                    return View();
                }
                else
                {
                    return RedirectPermanent("~/Account/LogOn");
                }
            }
        }

        [NonAction]
        private string RegisterNewCompany(string companyName, string userMail, string userName, string userLogin)
        {
            try
            {
                ActivityRegisterNewCompany activity = new ActivityRegisterNewCompany();
                Dictionary<string, object> prms = new Dictionary<string, object>
                                                  {
                                                      {"CompanyName", companyName},
                                                      {"CurrentWorkarea", WADataProvider.WA},
                                                      {"Email", userMail},
                                                      {"UserLogin", userLogin},
                                                      {"UserName", userName}
                                                  };


                IDictionary<string, object> outputs = WorkflowInvoker.Invoke(activity, prms);
                bool IsSend = MailClient.SendWelcomeForNewCompany(WADataProvider.WA, userMail, userName, outputs["Password"].ToString(), userLogin);
                WADataProvider.WA.Refresh();
                return outputs["Password"].ToString();

                //bool IsSend = MailClient.SendWelcomeForNewCompany(WADataProvider.WA, "levkin77@mail.ru", "Денис", "paswordhfhfh", "testCompanyCoser");
                //IsSend = MailClient.SendTemplateMessage(WADataProvider.WA, "levkin77@mail.ru", "Денис", "<p>Тестовое сообщение для вас {USER_NAME}</p>");
                //MailClient.SendWelcomeForNewCompany(WADataProvider.WA, "levkin77@mail.ru", "Денис", "paswordhfhfh", "testCompanyCoser");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public ActionResult AlreadyRegistered()
        {
            ViewResult res = View("AlreadyRegistered");
            return res;
        }

        public ActionResult Registry(string CompanyName, string WorkerName, string Pohone, string Email, string Login)
        {
            if (WADataProvider.SysConfig.SolveRegistryCompany)
            {
                string email_expr = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                    + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                    + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                    + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                    + @"[a-zA-Z]{2,}))$";
                Regex rx = new Regex(email_expr);

                if ((CompanyName != null && CompanyName.Length > 0) && (WorkerName != null && WorkerName.Length > 0) && (Login != null && Login.Length > 0) && rx.IsMatch(Email))
                {
                    string password = this.RegisterNewCompany(CompanyName, Email, WorkerName, Login);
                    if (password != null && password.Length > 0)
                    {
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_CompanyName", CompanyName));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_WorkerName", WorkerName));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_Pohone", Pohone));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_Email", Email));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_Login", Login));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry_Password", password));
                        HttpContext.Response.Cookies.Add(new HttpCookie("Registry", "1") { Expires = DateTime.Now.AddMinutes(3) });
                        return RedirectPermanent("~/Commons/CompanyRegistration/AlreadyRegistered");
                    }
                    else
                    {
                        return RedirectPermanent("~/Commons/CompanyRegistration");
                    }
                }
                else
                {
                    return RedirectPermanent("~/Commons/CompanyRegistration");
                }
            }
            else
            {
                return RedirectPermanent("~/Account/LogOn");
            }
        }
    }
}
