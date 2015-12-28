using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Ourp.Models;
using DevExpress.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Models;
using System.Data.SqlClient;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
	[MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
	public class AutoController : CoreController
    {
		public AutoController()
        {
			//Name = "VLE";
			// TODO: Исправить на то, что нужно
			Name = WebModuleNames.WEB_BANKS;;
        }

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult IndexPartial()
		{
			return PartialView();
		}

		public ActionResult Edit(int id)
		{
			AutoModel model = id == 0 ? new AutoModel { Id = 0 } : AutoModel.GetObject(id);
			return View("Edit", model);
		}

		//public ActionResult Edit(int id, int kindId)
		//{
		//    return View();
		//}

		[HttpPost]
		public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] AutoModel model)
		{
			if (ModelState.IsValid)
			{
				Product eq = model.ToObject();

				if (model.Id == 0)
				{
					model.Id = model.SaveTransaction(eq);
				}
				else
					model.SaveTransaction(eq);
				//Product prod = new Product();

				//if (model.Id == 0)
				//{
				//    prod.Id = eq.Id;
				//    prod.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
				//    prod.KindId = Product.KINDID_AUTO;

				//    model.Id = eq.Id;
				//}
				//else
				//{
				//    //TODO: Сделать изменение в product.mycompanyid
				//    prod = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(eq.Id);
				//}

				////prod.Save();
				////eq.Save();

				//using (SqlConnection con = WADataProvider.WA.GetDatabaseConnection())
				//{
				//    SqlTransaction tran = con.BeginTransaction();
				//    try
				//    {
				//        prod.Save(tran);
				//        eq.Save(tran);
				//        tran.Commit();
				//    }
				//    catch (Exception)
				//    {
				//        tran.Rollback();
				//    }
				//    con.Close();
				//}

				return View("PopupWindowClose", model);
			}
			return View(model);
		}

		public void Delete(int id)
		{
			if (id > 0)
			{
				id = 1;
			}
		}
    }
}
