using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Controllers;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Ourp.Models;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;
using BusinessObjects.Web.Core;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
	[MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
	public class EquipmentDetailsController : CoreController<EquipmentDetail>
    {
		public EquipmentDetailsController()
		{
            // TODO: Исправить на то, что нужно
            //Name = "WEB_EQUIPMENTDETAILS";
			Name = WebModuleNames.WEB_BANKS;
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
					case EquipmentDetail.KINDID_DETAIL:
						kid = EquipmentDetail.KINDID_DETAIL;
						break;
					case EquipmentDetail.KINDID_STDDETAIL:
						kid = EquipmentDetail.KINDID_STDDETAIL;
						break;

				}
			}

			EquipmentDetailModel model = id == 0 ? new EquipmentDetailModel { Id = 0, KindId = kid, ParentId = parentId } : EquipmentDetailModel.GetObject(id);
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
				if (model.Id == 0)
				{
					model.Id = eq.Id;

					int KindId = 0;

					switch (model.KindId)
					{
						case EquipmentDetail.KINDID_DETAIL:
							KindId = 113;
							ChainAdvanced<EquipmentNode,EquipmentDetail> ce = new ChainAdvanced<EquipmentNode,EquipmentDetail>()
							{
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce.Save();
							break;
						case EquipmentDetail.KINDID_STDDETAIL:
							KindId = 105;
							ChainAdvanced<Equipment,EquipmentDetail> ce1 = new ChainAdvanced<Equipment,EquipmentDetail>()
							{
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce1.Save();
							break;
					}

					//ChainAdvanced<EquipmentNode,EquipmentDetail> ce = new ChainAdvanced<EquipmentNode,EquipmentDetail>()
					//{
					//    Workarea = WADataProvider.WA,
					//    LeftId = model.ParentId,
					//    RightId = model.Id,
					//    KindId = KindId,
					//    StateId = State.STATEACTIVE
					//};
					//ce.Save();
				}
				return View("PopupWindowClose", model);
			}
			return View(model);
		}
    }
}
