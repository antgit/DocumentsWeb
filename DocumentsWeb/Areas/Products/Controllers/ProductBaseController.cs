using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Controllers;
using DocumentsWeb.Areas.Products.Models;

namespace DocumentsWeb.Areas.Products.Controllers
{
    /// <summary>
    /// Базовый контроллер для объектов учета
    /// </summary>
    [Authorize]
    public class ProductBaseController : CoreController<Product>
    {
        public ActionResult BrandPartial()
        {
            return PartialView(new ProductModel());
        }

        public ActionResult ProductTypePartial()
        {
            return PartialView(new ProductModel());
        }

        public ActionResult TradeMarkPartial()
        {
            return PartialView(new ProductModel());
        }
    }
}
