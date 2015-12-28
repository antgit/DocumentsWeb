using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Controllers
{
    public class BrowseDialogController : CoreController
    {
        public BrowseDialogController()
        {
            Name = "BrowseDialog";
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrowseDialogTreePartial()
        {
            return PartialView();
        }

        public ActionResult BrowseDialogListPartial(int hierarchyId)
        {
            Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(hierarchyId);
            string modelName = Request.Params["ModelName"];

            switch ((WhellKnownDbEntity)hierarchy.ContentEntityId)
            {
                case WhellKnownDbEntity.Product:
                    {
                        PartialViewResult result = PartialView(hierarchy.GetTypeContents<Product>().Select(ProductModel.ConvertToModel));
                        result.ViewData.Add("ModelName", modelName);
                        return result;
                    }
                case WhellKnownDbEntity.Agent:
                    {

                        PartialViewResult result = PartialView(ClientModel.GetHierarchyContents(hierarchy.Id));
                        //PartialViewResult result = PartialView(hierarchy.GetTypeContents<Agent>().Select(ClientModel.ConvertToModel));
                        result.ViewData.Add("ModelName", modelName);
                        return result;
                    }
                case WhellKnownDbEntity.Unit:
                    {
                        PartialViewResult result = PartialView(hierarchy.GetTypeContents<Unit>().Select(WebUnitModel.ConvertToModel));
                        result.ViewData.Add("ModelName", modelName);
                        return result;
                    }
                default:
                    throw new Exception("Неизвестный тип данных для диалога выбора");
            }
        }
    }
}
