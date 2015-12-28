using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Products.Controllers
{
    /// <summary>
    /// ���������� �������� "����������"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER})]
    public class ComputerController : CoreController<Product>
    {
        public ComputerController()
        {
            Name = WebModuleNames.WEB_COMPUTER;
            RootHierachy = Hierarchy.SYSTEM_PRODUCT_COMPUTER;
        }

        //������
        public ActionResult Index(bool refresh = false)
        {
            LayoutHelper.LoadLayoutFromDatabase(
                LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"] + "Partial";

            int[] roots = Utils.GetHieRoots(controller, action);
            List<ProductModel> list = roots.Contains(0) ? ProductModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray()) : ProductModel.GetCollectionWONested(roots);

            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            
            int[] roots = Utils.GetHieRoots(controller, action);
            List<ProductModel> list = roots.Contains(0) ? ProductModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray()) : ProductModel.GetCollectionWONested(roots);

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
            ProductModel model = ProductModel.GetObject(id);
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
        /// <param name="id">������������� ������� �����</param>
        /// <returns></returns>
        public ActionResult Preview(int id)
        {
            return View("Preview", ProductModel.GetObject(id));
        }
        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <param name="id">������������� ��������������</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            ProductModel model = id == 0 ? new ProductModel { Id = 0, KindId = Product.KINDID_PRODUCT } : ProductModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] ProductModel model)
        {
            ProductModel modelCashe = (ProductModel)WADataProvider.ModelsCache.Get(model.ModelId);
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
                if (model.Id != 0 && !ProductModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }

                Product obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();



                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = ProductModel.GetObject(obj.Id);

                    WADataProvider.CacheProductsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = ProductModel.GetObject(obj.Id);
                    WADataProvider.CacheProductsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }
        /// <summary>
        /// �������� ������ ������� �����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            ProductModel model = new ProductModel { Id = 0, Name = string.Empty, KindId = Product.KINDID_COMPUTER };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
                return Create();
            ProductModel value = ProductModel.GetObject(id);
            WADataProvider.ModelsCache.Add(value.ModelId, value);
            return View("Edit", value);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] ProductModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !ProductModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Product product = model.ToObject(WADataProvider.WA);
                product.KindId = Product.KINDID_COMPUTER;
                product.UserName = WADataProvider.CurrentMembershipUser.UserName;
                product.Save();
                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(product, true);

                    hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    model.Id = product.Id;
                    model = ProductModel.GetObject(product.Id);
                    WADataProvider.CacheProductsModelData.AddToCashe(hierarchy.Id, model);

                }
                else
                {
                    model.Id = product.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    model = ProductModel.GetObject(product.Id);
                    WADataProvider.CacheProductsModelData.AddToCashe(hierarchy.Id, model);
                }
                model.Id = product.Id;
                return View("PopupWindowClose", model);
            }
            return View(model);
        }

        public void Delete(int id)
        {
            ProductModel.ToTrash(id);
            //ClientModel.ToTrash(id, Hierarchy.SYSTEM_AGENT_BUYERS);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    ProductModel.SetStatetDone(id, Hierarchy.SYSTEM_PRODUCTS);
                    break;
                case State.STATENOTDONE:
                    ProductModel.SetStateNotDone(id, Hierarchy.SYSTEM_PRODUCTS);
                    break;
                case State.STATEDENY:
                    ProductModel.SetStateDeny(id, Hierarchy.SYSTEM_PRODUCTS);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            ProductModel.CreateCopy(id, Hierarchy.SYSTEM_PRODUCTS);
        }
        /// <summary>
        /// ������� ������� � ����
        /// </summary>
        /// <param name="type">��� ����� (xls, pdf)</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type)
        {
            GridViewSettings settings = new GridViewSettings
                {Name = "������", CallbackRouteValues = new {Action = "Index", Controller = "Product"}};
            //settings.Columns.Add("Id", "��");
            settings.Columns.Add("Name", "���");
            settings.Columns.Add("Code", "���");
            settings.Columns.Add("Nomenclature", "������������");
            settings.Columns.Add("UnitName", "��. ���.");

            IEnumerable coll = ProductModel.GetCollection(Hierarchy.SYSTEM_PRODUCTS);

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