using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Analitics.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Analitics.Controllers
{
    /// <summary>
    /// �������� ��������
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class AnaliticPaymentTypeController : AnaliticBaseController
    {
        /// <summary>
        /// �������� ��������
        /// </summary>
        private const string RootHie = Hierarchy.SYSTEM_ANALITIC_PAYMENTTYPE;

        /// <summary>
        /// ��� ���������
        /// </summary>
        private const int AnaliticKindId = Analitic.KINDID_MONEY;

        public AnaliticPaymentTypeController()
        {
            Name = WebModuleNames.WEB_MODULE_DICTIONARY_ANALITICPAYMENTTYPE;
            RootHierachy = RootHie;
        }

        public ActionResult Index()
        {
            LayoutHelper.LoadLayoutFromDatabase(
                LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString() + "Partial";

            List<AnaliticModel> list = new List<AnaliticModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = AnaliticModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = AnaliticModel.GetCollectionWONested(roots);

            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            List<AnaliticModel> list = new List<AnaliticModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = AnaliticModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = AnaliticModel.GetCollectionWONested(roots);

            return PartialView(list);
        }
        /// <summary>
        /// ������������� ����� ��������� ��� �������������� ������
        /// </summary>
        /// <remarks>� ����������� �� ������� ���������� ������������ � ����������� ��������� 
        /// ������������ �������� ���������� ��� �������� ��������� ������</remarks>
        /// <param name="id">������������� �������</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Open(int id)
        {
            AnaliticModel model = AnaliticModel.GetObject(id);
            // ���������� ������� ����������
            if (model.IsReadOnly ||
                !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name)
                || WADataProvider.CommonRightView.ReadOnly
                || !WADataProvider.IsCompanyIdAllowEditToCurrentUser(model.MyCompanyId))
                return Preview(id);

            return ControlView(id);
        }
        /// <summary>
        /// �������� ������
        /// </summary>
        /// <param name="id">������������� ��������������</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            AnaliticModel model = id == 0 ? new AnaliticModel { Id = 0, KindId = AnaliticKindId } : AnaliticModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);

            return result;
        }
        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="model">������</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] AnaliticModel model)
        {
            AnaliticModel modelCashe = (AnaliticModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //����������� ����� ���������, �� ������������� �� �������
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
                model.MyCompanyId = modelCashe.MyCompanyId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !AnaliticModel.CanSave(model.Id))
                {
                    ViewResult res = View("ControlView", model);
                    return res;
                }

                Analitic obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = AnaliticModel.GetObject(obj.Id);

                    WADataProvider.CacheAnaliticsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = AnaliticModel.GetObject(obj.Id);
                    WADataProvider.CacheAnaliticsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }
        /// <summary>
        /// �������� ����� ���������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            AnaliticModel model = new AnaliticModel { Id = 0, KindId = AnaliticKindId };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
                return Create();
            AnaliticModel model = AnaliticModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] AnaliticModel model)
        {
            AnaliticModel modelCashe = (AnaliticModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //����������� ����� ���������, �� ������������� �� �������
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
                model.MyCompanyId = modelCashe.MyCompanyId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !AnaliticModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Analitic obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHie);
                    h.ContentAdd(obj, true);
                }
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View("Edit", model);
        }

        /// <summary>
        /// ������� ���������
        /// </summary>
        public void Delete(int id)
        {
            AnaliticModel.ToTrash(id, RootHie);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    AnaliticModel.SetStatetDone(id, RootHie);
                    break;
                case State.STATENOTDONE:
                    AnaliticModel.SetStateNotDone(id, RootHie);
                    break;
                case State.STATEDENY:
                    AnaliticModel.SetStateDeny(id, RootHie);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            AnaliticModel.CreateCopy(id, RootHie);
        }

        /// <summary>
        /// ������� ������� � ����
        /// </summary>
        /// <param name="type">��� ����� (xls, pdf)</param>
        /// <param name="subtype">������ ������</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type, string subtype)
        {
            GridViewSettings settings = new GridViewSettings { Name = "���������" };
            settings.Columns.Add("Name", "���");
            settings.Columns.Add("NameFull", "�������� ������������");
            settings.Columns.Add("Code", "���");

            List<AnaliticModel> coll = AnaliticModel.GetCollection(RootHie);

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, coll);
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, coll);
                default:
                    throw new ArgumentException("����������� ��� ������ ��� ��������");
            }
        }
    }
}