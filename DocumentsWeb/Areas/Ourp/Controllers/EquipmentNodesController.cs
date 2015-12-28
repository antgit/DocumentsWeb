using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Controllers;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Ourp.Models;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
	[MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
	public class EquipmentNodesController : CoreController<EquipmentNode>
    {
		public EquipmentNodesController()
		{
            // TODO: Исправить на то, что нужно
            //Name = "WEB_EQUIPMENTNODES";
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
					case EquipmentNode.KINDID_EQUIPMENTNODE:
						kid = EquipmentNode.KINDID_EQUIPMENTNODE;
						break;
					case EquipmentNode.KINDID_EQUIPMENTSUBNODE:
						kid = EquipmentNode.KINDID_EQUIPMENTSUBNODE;
						break;
				}
			}

			EquipmentNodeModel model = id == 0 ? new EquipmentNodeModel { Id = 0, KindId = kid, ParentId = parentId } : EquipmentNodeModel.GetObject(id);
			return View("Edit", model);
		}

		//public ActionResult Edit(int id, int kindId)
		//{
		//    return View();
		//}

		[HttpPost]
		public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] EquipmentNodeModel model)
		{
			if (ModelState.IsValid)
			{
				EquipmentNode eq = model.ToObject();
				eq.Save();
				if (model.Id == 0)
				{
					model.Id = eq.Id;

					int KindId = 0;

					switch (model.KindId)
					{
						case EquipmentNode.KINDID_EQUIPMENTNODE:
							KindId = 111;
							//Chain<Equipment> ce = new Chain<Equipment>()
							ChainAdvanced<Equipment,EquipmentNode> ce = new ChainAdvanced<Equipment,EquipmentNode>()
							{
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce.Save();
							break;
						case EquipmentNode.KINDID_EQUIPMENTSUBNODE:
							KindId = 112;
							Chain<EquipmentNode> ce1 = new Chain<EquipmentNode>()
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

					//Chain<Equipment> ce = new Chain<Equipment>()
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
