using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Web.Core;
using BusinessObjects;
using DocumentsWeb.Controllers;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Ourp.Models;
using DevExpress.Web.Mvc;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
	[MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
	public class EquipmentWorkShopsController : CoreController<Depatment>
    {
		public EquipmentWorkShopsController()
		{
            // TODO: Исправить на то, что нужно
            Name = WebModuleNames.WEB_DEPATMENTS;
        }

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Edit(int id, int kindId, int parentId = 0)
		{
			int kid = 0;

			if (id == 0)
			{
				switch (kindId)
				{
					case Depatment.KINDID_SHOP:
						kid = Depatment.KINDID_SHOP;
						break;
				}
			}

			EquipmentWorkShopModel model = id == 0 ? new EquipmentWorkShopModel { Id = 0, KindId = kid, ParentId = parentId } : EquipmentWorkShopModel.GetObject(id);
			return View("Edit", model);
		}

		//public ActionResult Edit(int id, int kindId)
		//{
		//    return View();
		//}

		[HttpPost]
		public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] EquipmentDetailModel model)
		{
			if (ModelState.IsValid)
			{
				EquipmentDetail eq = model.ToObject();
				eq.Save();

				return View("PopupWindowClose", model);
			}
			return View(model);
		}
    }
}
