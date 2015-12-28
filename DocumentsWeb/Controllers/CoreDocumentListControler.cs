using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    /// <summary>
    /// Базовый класс просмотра списков документов
    /// </summary>
    public abstract class CoreDocumentListControler: CoreController
    {
        /// <summary>
        /// Код поиска папки документов по умолчанию 
        /// </summary>
        public string FolderCodeFind { get; protected set; }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            OnCoreAuthorization(filterContext);
            //
            //this.Name
            base.OnAuthorization(filterContext);
        }
        protected virtual void OnCoreAuthorization(AuthorizationContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;

            OnAuthorizationDeleteAction(filterContext);
            OnAuthorizationViewAction(filterContext);
            OnAuthorizationEditAction(filterContext);

        }
        protected virtual void OnAuthorizationDeleteAction(AuthorizationContext filterContext) 
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "DELETE")
            {

                if (!WADataProvider.FolderElementRightView.IsAllow(Right.UITRASH, WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind).Id))
                {
                    throw new SecurityException("Удаление запрещено!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                int objId = 0;
                Int32.TryParse(valueParam, out objId);
                if (objId != 0)
                {
                    Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(objId);
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

        protected virtual void OnAuthorizationEditAction(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "EDIT"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CHANGESTATE"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CREATECOPY"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CREATE")
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
                if (objId != 0 && !WADataProvider.FolderElementRightView.IsAllow(Right.DOCEDIT, WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind).Id))
                {
                    throw new SecurityException("Отсутствуют разрешения на изменение документа!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }

                if (objId == 0 && !WADataProvider.FolderElementRightView.IsAllow(Right.DOCCREATE, WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind).Id))
                {
                    throw new SecurityException("Отсутствуют разрешения на создание документа!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }

                if (objId != 0)
                {
                    Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(objId);
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

        protected virtual void OnAuthorizationViewAction(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "OPEN")
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
                if (!(WADataProvider.FolderElementRightView.IsAllow(Right.DOCEDIT, WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind).Id) |
                      WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind).Id)))
                {
                    throw new SecurityException("Отсутствуют разрешения на изменение данных!");
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                if (objId != 0)
                {
                    Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(objId);
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
        public virtual ActionResult SelectDocumentTemplate()
        {
            return View("SelectDocumentTemplatePartial");
        }
        public virtual ActionResult DocumentTemplateSelectPartial()
        {
            return PartialView("SelectDocumentTemplateGridPartial");
        }

    }
}