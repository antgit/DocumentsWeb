using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBADMIN })]
    public class ViewListDocumentConfigController : Controller
    {
        public ActionResult Index()
        {
            ViewResult result = View(DocumentData.CollectionDocumentConfigs());
            result.ViewData.Add("HelpDefaultLink", "");
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(DocumentData.CollectionDocumentConfigs());
        }
        public virtual ActionResult Create(int? tmlId = null)
        {
            if (tmlId.HasValue && tmlId.Value != 0)
            {

                Document dt = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.Value);
                string codeFind = dt.Folder.CodeFind;

                if(codeFind== Folder.CODE_FIND_SALES_CONFIG)
                {
                    return RedirectToAction("Create", new { Area = "Admins", Controller = "DocumentConfigSale"});
                }
                if (codeFind == Folder.CODE_FIND_PRICE_CONFIG)
                {
                    return RedirectToAction("Create", new { Area = "Admins", Controller = "DocumentConfigPrice" });
                }
                if (codeFind == Folder.CODE_FIND_SERVICE_CONFIG)
                {
                    return RedirectToAction("Create", new { Area = "Admins", Controller = "DocumentConfigService" });
                }
                if (codeFind == Folder.CODE_FIND_TAX_CONFIG)
                {
                    return RedirectToAction("Create", new { Area = "Admins", Controller = "DocumentConfigTax" });
                }
                if (codeFind == Folder.CODE_FIND_FINANCE_CONFIG)
                {
                    return RedirectToAction("Create", new { Area = "Admins", Controller = "DocumentConfigFinance" });
                }
            }
            return new HttpNotFoundResult();
        }
        public ActionResult SelectDocumentTemplate()
        {
            return View("SelectDocumentTemplatePartial");
        }
        public ActionResult DocumentTemplateSelectPartial()
        {
            return PartialView("SelectDocumentTemplateGridPartial");
        }
        
    }
}
