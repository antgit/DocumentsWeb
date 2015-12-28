using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Finances.Models;
using DocumentsWeb.Areas.Prices.Models;
using DocumentsWeb.Areas.Reports.Models;
using DocumentsWeb.Areas.Sales.Models;
using DocumentsWeb.Areas.Services.Models;
using DocumentsWeb.Areas.Taxes.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    /// <summary>
    /// ���������� ��������� 
    /// </summary>
    public abstract class CoreDocumentControler: CoreController
    {
        public virtual void OnCreate()
        {
            //Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            //int formId = Folder.FormId;
            //string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(Folder.FormId).Code;
            //Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(Folder.DocumentId);


            //DocumentContractModel documentModel = new DocumentContractModel { FormCode = formCode };
            //documentModel.LoadFromTemplate(template);
            //WADataProvider.ModelsCache.Add(documentModel.ModelId, documentModel);
            //return EditModel(documentModel.ModelId);
        }
        public virtual void OnEndingEditModel(ViewResult result, string modelId=null)
        {
            if (!string.IsNullOrEmpty(modelId))
            {
                DocumentModel documentModel = (DocumentModel)WADataProvider.ModelsCache.Get(modelId);
                if (documentModel.Id!=0)
                    result.ViewData.Add("PrintForms", PrintForms());

            }
            result.ViewData.Add("HelpHasPopup", HelpHasPopup);
            if (!string.IsNullOrEmpty(HelpDefaultLink))
                result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
        }
        /// <summary>
        /// ������������� ���� ���������, ������������� ������ ������������.
        /// </summary>
        /// <remarks>������������ ��� ������� �������� ����������� ��������� � ���������������� ���� ��������� 
        /// � ����� ��������� �������� ��������� ������������������ ���� � ������ �����������.</remarks>
        public int DocumentKindId { get; protected set; } 
        /// <summary>
        /// ��� ������ ����� ���������� �� ��������� 
        /// </summary>
        public string FolderCodeFind { get; protected set; }
        /// <summary>
        /// ����� ����������
        /// </summary>
        public Folder Folder
        {
            get
            {
                if (string.IsNullOrEmpty(FolderCodeFind)) return null;
                return WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);
            }
        }
        /// <summary>
        /// ��� ������ ��� ������
        /// </summary>
        public string PrintModelCode { get; protected set; }

        /// <summary>
        /// ��������� �������� ���� ���������
        /// </summary>
        /// <returns></returns>
        public List<Library> PrintForms()
        {
            // �� ������� ��� ����������� ������� ����������, �� �������� Name �����������
            if (SelfLibrary == null)
                return new List<Library>();
            List<Library> coll = Chain<Library>.GetChainSourceList(SelfLibrary, WADataProvider.WA.PrintFormChainId(), State.STATEACTIVE).Where(s => s.StateId == State.STATEACTIVE).ToList();

            return
                coll.Where(s =>WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)
                && WADataProvider.LibrariesElementRightView.IsAllow(Right.UIPRINT, s.Id)).ToList();
        }
        /// <summary>
        /// ������� �� �������� �����
        /// </summary>
        public bool HasPrintForms
        {
            get { return PrintForms().Count > 0; }
        }
        /// <summary>
        /// ������ ��������� � ������ �������� �������������
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public ActionResult PrintDefault(int documentId)
        {
            if(HasPrintForms)
            {
                string navUrl = ReportHelper.GetPrintFormNavigateUrlInternal(PrintForms()[0], PrintModelCode, documentId);
                //string navUrl = ReportHelper.GetPrintFormNavigateUrl(PrintForms()[0], PrintModelCode, documentId); //"SALES"
                return Redirect(navUrl);
            }
            return null;
        }

        /// <summary>
        /// ������ ��������� � ��������� �������� �������������
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="printFormId"></param>
        /// <returns></returns>
        public ActionResult Print(int documentId, int printFormId)
        {
            if (HasPrintForms)
            {
                Library printDoc = PrintForms().FirstOrDefault(s => s.Id == printFormId);
                if(printDoc==null)
                    printDoc = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(printFormId);

                ElementRightView coll = WADataProvider.WA.Access.ElementRightView((int)WhellKnownDbEntity.Library, WADataProvider.CurrentUserName);
                if (!coll.IsAllow(Right.UIPRINT, printFormId))
                {
                    return new RedirectResult("~/SecureExeptionPrintForm.html");
                }
                string navUrl = ReportHelper.GetPrintFormNavigateUrlInternal(printDoc, PrintModelCode, documentId); //"SALES"
                //string navUrl = ReportHelper.GetPrintFormNavigateUrl(PrintForms().FirstOrDefault(s => s.Id == printFormId), PrintModelCode, documentId); //"SALES"
                return Redirect(navUrl);
            }
            return null;
        }
        
        /// <summary>
        /// ������ ������� ��������� � ����������
        /// </summary>
        /// <returns></returns>
        public List<Library> LinkedReports()
        {
            return ReportData.LinkedReports(SelfLibrary);
        }


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
            if (Request.ServerVariables.AllKeys.Contains("REMOTE_ADDR"))
                WADataProvider.OnUserActivity(Request.ServerVariables["REMOTE_ADDR"]);
            else
                WADataProvider.OnUserActivity();
            if (MvcApplication.Sessions.ContainsKey(Session.SessionID))
                MvcApplication.Sessions[Session.SessionID] = WADataProvider.CurrentUserName;
            else
                MvcApplication.Sessions.Add(Session.SessionID, WADataProvider.CurrentUserName);
            OnAuthorizationPrintAction(filterContext);
            OnAuthorizationViewAction(filterContext);
            OnAuthorizationEditAction(filterContext);

        }
        protected virtual void OnAuthorizationPrintAction(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "PRINT" ||
                filterContext.ActionDescriptor.ActionName.ToUpper() == "PRINTDEFAULT")
            {

                if (!WADataProvider.FolderElementRightView.IsAllow(Right.DOCPREVIEW, Folder.Id))
                {
                    throw new SecurityException("������ ��������� ���������!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }

        protected virtual void OnAuthorizationEditAction(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "EDIT")
            {
                // ��� ���������� �� ��������������
                if (!WADataProvider.FolderElementRightView.IsAllow(Right.DOCEDIT, Folder.Id))
                {
                    // �� ���� ���������� �� ��������
                    if (WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, Folder.Id))
                    {
                        // TODO: ��������� ������������� �� ����� OPEN � ������� ������ ������
                    }
                    else
                    {
                        throw new SecurityException("����������� ���������� �� ��������� ������!");
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }
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
                if (objId != 0)
                {
                    Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(objId);
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("��������� ������ ��� ����������� �������� ���������!");
                        }
                    }
                }
            }
        }

        protected virtual void OnAuthorizationViewAction(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "OPEN" ||
                filterContext.ActionDescriptor.ActionName.ToUpper() == "VIEW")
            {
                if (!WADataProvider.FolderElementRightView.IsAllow(Right.DOCVIEW, Folder.Id))
                {
                    throw new SecurityException("����������� ���������� �� �������� ������!");
                    filterContext.Result = new HttpUnauthorizedResult();
                }
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
                if (objId != 0)
                {
                    Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(objId);
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowOpenToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("��������� ������ ��� ����������� �������� ���������!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �������� ���������� ��������� � �������� ��� � ��������������� �����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="chainId"></param>
        /// <param name="codeValue"></param>
        /// <returns></returns>
        protected ActionResult ChildDocumentCreate<T>(int id, int chainId, string codeValue) where T : DocumentBase, new()
        {
            T doc = WADataProvider.WA.GetObject<T>(id);
            DocChain v = DocChain.Get(WADataProvider.WA, doc.Document.TemplateId).FirstOrDefault(s => s.Id == chainId);

            if (string.IsNullOrWhiteSpace(codeValue))
            {
                throw new ApplicationException("�� ������ ������� ������� �������� ���������!");
            }

            string code = codeValue.ToUpper();
            Activity value = BusinessObjects.Workflows.WfCore.FindByCodeInternal(code) ??
                             BusinessObjects.Workflows.WfCore.FindByCode(WADataProvider.WA, code);


            IDictionary<string, object> outputs = WorkflowInvoker.Invoke(value, new Dictionary<string, object>
                    {
                        { "document", doc },
                        { "templateId", v.Id }
                    });
            if (outputs.ContainsKey("returnValue"))
            {
                object val = outputs["returnValue"];
                if (val != null)
                {
                    DocumentModel model;

                    //�������������� ��������� � ���������������� ����
                    if (val is DocumentSales)
                        model = DocumentSaleModel.ConvertToModel((DocumentSales)val);
                    else if (val is DocumentFinance)
                        model = DocumentFinanceModel.ConvertToModel((DocumentFinance)val);
                    else if (val is DocumentTaxes)
                        model = DocumentTaxModel.ConvertToModel((DocumentTaxes)val);
                    else if (val is DocumentService)
                        model = DocumentServiceModel.ConvertToModel((DocumentService)val);
                    else if (val is DocumentPrices)
                        model = DocumentPriceListModel.ConvertToModel((DocumentPrices)val);
                    else throw new Exception("����������� ��� ���������");

                    //���������� � ���
                    model.DocumentOwnerId = doc.Id;
                    WADataProvider.ModelsCache.Add(model.ModelId, model);

                    //����� ����������� � ����������� �� ����� ���������
                    DocumentBase newDoc = (DocumentBase)val;
                    Folder folder = WADataProvider.WA.Cashe.GetCasheData<Folder>().Item(newDoc.Document.FolderId);
                    bool nds = folder.CodeFind.EndsWith("NDS");
                    int formId = folder.FormId;
                    string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
                    object route = DocumentController.GetRouteValues(formCode, nds, modelId: model.ModelId);

                    //��������
                    return RedirectToAction("EditModel", route);
                }
            }
            return View();
        }

        public void WriteLog(int Id, string Name, string Memo)
        {
            LogUserAction ua = new LogUserAction
            {
                Workarea = WADataProvider.WA,
                DatabaseId = 1001,
                UserName = WADataProvider.CurrentUserName,
                DateModified = DateTime.Now,
                OwnId = Id,
                Name = Name,
                Memo = Memo
            };
            ua.Save();
        }
    }
}