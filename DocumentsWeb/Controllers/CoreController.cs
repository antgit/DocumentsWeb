using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    /// <summary>
    /// Основной контролер используемый в проекте
    /// </summary>
    public class CoreController : Controller
    {
        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string Name { get; protected set; }

        private Library _selfLib;
        /// <summary>
        /// Библиотека зарегестрированная в системе соответствующая данному модулю
        /// </summary>
        public Library SelfLibrary
        {
            get { return _selfLib ?? (_selfLib = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>(Name)); }
        }

        public bool HelpHasPopup { get; set; }
        public string HelpDefaultLink
        {
            get
            {
                if (SelfLibrary != null)
                {
                    return SelfLibrary.HelpUrl;
                }
                return string.Empty;
            }
        }
        public ActionResult DenamdHelp()
        {
            return PartialView("HelpPartial");
        }

        // TODO: добавить получение разрешений...
        
        protected override void OnException(ExceptionContext filterContext)
        {

            if (filterContext.Exception is SecurityException)
            {
                filterContext.Result = new RedirectResult("~/SecureExeption.html");
                filterContext.ExceptionHandled = true;
            }
            base.OnException(filterContext);
        }

        public void SaveLayout(int id, bool? chkGroupPanel, bool? chkFilterPanel, bool? chkLinkName, bool? chkPreview)
        {
            LayoutHelper.SaveLayoutToDatabase(id, WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(), ControllerContext.HttpContext, chkGroupPanel, chkFilterPanel, chkLinkName, chkPreview);
        }

        public void LoadLayout(int id)
        {
            LayoutHelper.LoadLayoutFromDatabase(id, WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(), ControllerContext.HttpContext);
        }
    }

    public class CoreController<T> : CoreController where T : class, IBase, new()
    {
        /// <summary>
        /// Корневая иерархия
        /// </summary>
        public string RootHierachy { get; protected set; }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            OnCoreAuthorization(filterContext);
            //
            //this.Name
            base.OnAuthorization(filterContext);
        }
        
        protected virtual void OnCoreAuthorization(AuthorizationContext filterContext)
        {
            if (Request.ServerVariables.AllKeys.Contains("REMOTE_ADDR"))
                WADataProvider.OnUserActivity(Request.ServerVariables["REMOTE_ADDR"]);
            else
                WADataProvider.OnUserActivity();
            if (MvcApplication.Sessions.ContainsKey(Session.SessionID))
                MvcApplication.Sessions[Session.SessionID] = WADataProvider.CurrentUserName;
            else
                MvcApplication.Sessions.Add(Session.SessionID, WADataProvider.CurrentUserName);

            OnAuthorizationDeleteAction<T>(filterContext);
            OnAuthorizationViewAction<T>(filterContext);
            OnAuthorizationEditAction<T>(filterContext);

        }

        protected virtual void OnAuthorizationDeleteAction<T>(AuthorizationContext filterContext) where T : class, IBase, new()
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "DELETE")
            {
                if (!WADataProvider.LibrariesElementRightView.IsAllow(Right.UITRASH, Name))
                {
                    throw new SecurityException("Удаление запрещено!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                int objId = 0;
                Int32.TryParse(valueParam, out objId);
                if (objId != 0)
                {
                    T obj = WADataProvider.WA.Cashe.GetCasheData<T>().Item(objId);
                    if (obj.IsSystem || obj.IsReadOnly)
                    {
                        throw new SecurityException("Запись является системной или предназначена только для чтения!");
                        //filterContext.Result = new HttpUnauthorizedResult();     
                    }
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("Удаление данных вне собственной компании запрещено!");
                        }
                    }

                }
            }
        }

        protected virtual void OnAuthorizationEditAction<T>(AuthorizationContext filterContext) where T : class, IBase, new()
        {
            string currentActionName = filterContext.ActionDescriptor.ActionName.ToUpper();
            if (currentActionName == "EDIT"
                || currentActionName == "CHANGESTATE"
                || currentActionName == "CREATECOPY"
                || currentActionName == "CREATE"
                || currentActionName == "CONTROLVIEW"
                || currentActionName == "OPEN")
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
                if (objId != 0 && currentActionName != "OPEN" && !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на изменение данных!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                if (objId == 0 && !WADataProvider.LibrariesElementRightView.IsAllow(Right.UICREATE, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на создание данных!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                if (objId != 0 && currentActionName == "OPEN" && !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIVIEW, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на просмотр данных!");
                }

                if (objId != 0)
                {
                    T obj = WADataProvider.WA.Cashe.GetCasheData<T>().Item(objId);
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("Изменение данных вне собственной компании запрещено!");
                        }
                    }
                }
            }
        }

        protected virtual void OnAuthorizationViewAction<T>(AuthorizationContext filterContext) where T : class, IBase, new()
        {
            string currentActionName = filterContext.ActionDescriptor.ActionName.ToUpper();
            if (currentActionName == "PREVIEW"
                || currentActionName == "OPEN")
            {
                int objId = 0;
                if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("id"))
                {
                    string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                    Int32.TryParse(valueParam, out objId);
                }
                if (filterContext.RouteData.Values.ContainsKey("id"))
                {
                    Int32.TryParse(filterContext.RouteData.Values["id"].ToString(), out objId);
                }
                if (!(WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name) |
                      WADataProvider.LibrariesElementRightView.IsAllow(Right.UIVIEW, Name)))
                {
                    throw new SecurityException("Отсутствуют разрешения на просмотр данных!");
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                if (objId != 0)
                {
                    T obj = WADataProvider.WA.Cashe.GetCasheData<T>().Item(objId);
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("Изменение данных вне собственной компании запрещено!");
                        }
                    }
                }
            }
        }
    }
}
