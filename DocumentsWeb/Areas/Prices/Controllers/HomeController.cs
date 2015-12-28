using System.Data;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Controllers;
using BusinessObjects;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Prices.Models;

namespace DocumentsWeb.Areas.Prices.Controllers
{
    /// <summary>
    /// Основной контроллер раздела "Управление ценами"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPPRICES })]
    public class HomeController : CoreController
    {
        //if(DocumentsWeb.Models.WADataProvider.LibrariesElementRightView.IsAllow(BusinessObjects.Security.Right.UICREATE, WebModuleNames.WEB_DOCPRICE))
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCPRICE;
        }
       
        public ActionResult Index()
        {
            ViewResult result = View();
            if (!string.IsNullOrEmpty(HelpDefaultLink))
                result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView();
        }

        public ActionResult ViewBoardPriceListPartial(bool refresh=false)
        {
            DataTable tbl = PriceListHelper.GetDocuments(Folder.CODE_FIND_PRICES_DEAULT, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewBoardPriceListCommandPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocuments(Folder.CODE_FIND_PRICES_COMMAND, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewBoardPriceListIndPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocumentsInd(Folder.CODE_FIND_PRICES_IND, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewBoardPriceListSupplierPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocumentsInd(Folder.CODE_FIND_PRICES_SYPPLYER, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardPriceListCompetitorPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocumentsInd(Folder.CODE_FIND_PRICES_COMPETITOR, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardPriceListCompetitorIndPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocumentsInd(Folder.CODE_FIND_PRICES_COMPETITORIND, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardPriceListCommandIndPartial(bool refresh = false)
        {
            DataTable tbl = PriceListHelper.GetDocumentsInd(Folder.CODE_FIND_PRICES_COMMANDIND, refresh, 5, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId<>5";
            return PartialView(tbl.DefaultView.ToTable());
        }
        
        #region PriceName
        [HttpPost, ValidateInput(false)]
        public ActionResult ViewBoardPriceKindPartial()
        {
            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ViewBoardPriceKindAddPartial([ModelBinder(typeof(DevExpressEditorsBinder))] PriceNameModel model)
        {
            model.Save();
            return PartialView("ViewBoardPriceKindPartial");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ViewBoardPriceKindUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] PriceNameModel model)
        {
            model.Save();
            return PartialView("ViewBoardPriceKindPartial");
        }

        [HttpPost, ValidateInput(true)]
        public ActionResult ViewBoardPriceKindDeletePartial(int Id)
        {
            //http://stackoverflow.com/questions/5150476/how-to-display-mvc-3-client-side-validation-results-in-validation-summary
            //http://deanhume.com/Home/BlogPost/mvc-3-and-remote-validation/51
            PriceNameModel model = PriceNameModel.ToModel(Id);
            if(model.IsReadOnly)
            {
                ModelState.AddModelError("PriceNameErrorDelete", "Данный элемент нельзя редактировать...");
            }
            if (model.IsSystem)
            {
                ModelState.AddModelError("PriceNameErrorDelete", "Данный элемент нельзя удалять...");
            }
            if(ModelState.IsValid)
            {
                model.StateId = State.STATEDELETED;
                model.Save();
            }
            
            return PartialView("ViewBoardPriceKindPartial");
        }

        public ActionResult PriceKindListPartial()
        {
            return PartialView();
        }
        #endregion
    }
}
