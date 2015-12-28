using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;
using Recaptcha;

namespace DocumentsWeb.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            //Name = "Account";
        }

        public ActionResult LogOn()
        {
            LogOnModel model = new LogOnModel();
            return View(model);
        }


        [HttpPost]
        public ActionResult LogOn([ModelBinder(typeof(DevExpressEditorsBinder))]LogOnModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //http://msdn.microsoft.com/en-us/library/ff647070.aspx
                    if (Membership.ValidateUser(model.UserName, model.Password))
                    {
                        Uid uid = WADataProvider.WA.Access.GetAllUsers().FirstOrDefault(f => f.Name == model.UserName);
                        if (uid != null)
                        {
                            uid.Refresh();
                        }
                        if (uid != null && uid.IsStateDeny)
                        {
                            ModelState.AddModelError("ACCOUNTLOCKDOWN", "Ваш аккаунт временно заблокирован!");
                            model.LoginError = true;
                            return View(model);
                        }
                        else if (uid != null && !WADataProvider.WA.Access.GetUserGroups(uid.Name).Contains(Uid.GROUP_GROUPWEBUSER))
                        {
                            ModelState.AddModelError("ACCOUNTLOCKDOWN", "Ваш аккаунт найден, но Вы не имеете доступа к Web интерсейсу!");
                            model.LoginError = true;
                            return View(model);
                        }
                        else if (uid != null && uid.TimePeriodId != 0)
                        {
                            DateTime nowDate = DateTime.Today;
                            if (uid.TimePeriod.IsAllowOnWeekDay(nowDate.DayOfWeek))
                            {

                                // попали в запрещенные часы работы
                                if (uid.TimePeriod.GetStartValue(nowDate.DayOfWeek) < nowDate || uid.TimePeriod.GetEndValue(nowDate.DayOfWeek) > nowDate)
                                {
                                    ModelState.AddModelError("ACCOUNTTIMEPERIOD", "Ваш аккаунт запрещено использовать в текущее время!");
                                    model.LoginError = true;
                                    return View(model);
                                }
                            }
                        }
                        WADataProvider.RefreshHiearchyElementRightView(model.UserName);
                        WADataProvider.RefreshLibrariesElementRightView(model.UserName);
                        WADataProvider.RefreshFoldersElementRightView(model.UserName);
                        WADataProvider.RefreshCommonRightView(model.UserName);
                        WADataProvider.WA.Access.RefreshCompanyScopeViewContext(model.UserName);
                        WADataProvider.WA.CurrentUserContext = model.UserName;
                        
                        // Remember me was checked - set cookie to remember user for 10 days (or until he logs off)
                        if (model.RememberMe)
                        {
                            var tenDaysFromNow = DateTime.Now.AddDays(10);
                            FormsAuthentication.Initialize();
                            HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.UserName, model.RememberMe);
                            cookie.Expires = tenDaysFromNow;
                            var cookieVal = FormsAuthentication.Decrypt(cookie.Value);
                            FormsAuthenticationTicket at = new FormsAuthenticationTicket(cookieVal.Version, cookieVal.Name, cookieVal.IssueDate, tenDaysFromNow, true, cookieVal.UserData);
                            cookie.Value = FormsAuthentication.Encrypt(at);

                            HttpContext context = System.Web.HttpContext.Current;
                            context.Response.Cookies.Add(cookie);
                        }           
                        else
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        }
                        //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        Session["UserName"] = model.UserName;
                        if (MvcApplication.Sessions.ContainsKey(Session.SessionID))
                            MvcApplication.Sessions[Session.SessionID] = model.UserName;
                        else
                            MvcApplication.Sessions.Add(Session.SessionID, model.UserName);

                        if (returnUrl!=null && Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        if (returnUrl != null && !returnUrl.EndsWith("Partial"))
                        {
                            FormsAuthentication.RedirectFromLoginPage(model.UserName, false);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        /*
                        if (returnUrl != null && !returnUrl.EndsWith("Partial"))
                        {
                            FormsAuthentication.RedirectFromLoginPage(model.UserName, false);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }*/
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильно указано имя пользователя или пароль.");
                        model.LoginError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", ex.Message);
                model.LoginError = true;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult RestorePassword()
        {
            return View();
        }
        [HttpPost]
        [RecaptchaControlMvc.CaptchaValidatorAttribute]
        public ActionResult RestorePassword(RestoreModel model, bool captchaValid)
        {
            if(captchaValid)
            {
                if (Regex.IsMatch(model.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    Uid uid = WADataProvider.WA.GetCollection<Uid>().FirstOrDefault(s => s.Email == model.Email);
                    if(uid==null)
                    {
                        ViewBag.Message = "Адрес почты не найден!";
                    }
                    else
                    {
                        if (BusinessObjects.Web.Core.MailClient.SendPassword(WADataProvider.WA, uid))
                            return RedirectToAction("PasswordSent");
                        ViewBag.Message = "Ошибка высылания пароля";
                    }
                }
            }
            else
            {
                ViewBag.Message = "Неверный адрес почты или неверно введены слова с изображения";
            }
            return View();
        }

        public ActionResult PasswordSent()
        {
            return View();
        }
        
        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

    }
}
