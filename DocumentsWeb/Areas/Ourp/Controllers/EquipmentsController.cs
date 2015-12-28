using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects;
using DocumentsWeb.Areas.Ourp.Models;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;

namespace DocumentsWeb.Areas.Ourp.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class EquipmentsController : CoreController<Equipment>
    {
        public EquipmentsController() {
            // TODO: Исправить на то, что нужно
			Name = WebModuleNames.WEB_BANKS;
			//Name = "WEB_EQUIPMENTS";
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial() 
        {
            return PartialView();
        }

        public ActionResult Edit(int id, int kindId, int parentId = 0)
        {
			int kid = 0;

			if (id == 0)
			{
				switch (kindId)
				{
					case Equipment.KINDID_EQUIPMENTUNIT:
						kid = Equipment.KINDID_EQUIPMENTUNIT;
						break;
					case Equipment.KINDID_EQUIPMENTAUTO:
						kid = Equipment.KINDID_EQUIPMENTAUTO;
						break;
					case Equipment.KINDID_FOLDER:
						kid = Equipment.KINDID_FOLDER;
						break;
					//case Equipment.KINDID_EQUIPMENT:
					//    //TODO: поменять id отдела на константу
					//    kid = 7143425;
					//    break;
					////TODO: поменять id отдела на константу
					//case 7143425:
					//    kid = Equipment.KINDID_EQUIPMENTUNIT;
					//    break;
					//case Equipment.KINDID_EQUIPMENTUNIT:
					//    kid = EquipmentNode.KINDID_EQUIPMENTNODE;
					//    break;
					//case EquipmentNode.KINDID_EQUIPMENTNODE:
					//    kid = EquipmentNode.KINDID_EQUIPMENTSUBNODE;
					//    break;
					//case EquipmentNode.KINDID_EQUIPMENTSUBNODE:
					//    kid = EquipmentDetail.KINDID_EQUIPMENT;
					//    break;
					//case EquipmentDetail.KINDID_FOLDER:
					//    kid = EquipmentDetail.KINDID_FOLDER;
					//    break;
					//case EquipmentDetail.KINDID_STDDETAIL:
					//    kid = EquipmentDetail.KINDID_STDDETAIL;
					//    break;
				}
			}

			EquipmentModel model = id == 0 ? new EquipmentModel { Id = 0, KindId = kid, ParentId = parentId } : EquipmentModel.GetObject(id);
            return View("Edit", model);
        }

		//public ActionResult Edit(int id, int kindId)
		//{
		//    return View();
		//}

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] EquipmentModel model)
        {
            if (ModelState.IsValid)
            {
                Equipment eq = model.ToObject();
                eq.Save();
                if (model.Id == 0)
                {
                    model.Id = eq.Id;

                    int KindId = 0;

                    switch (model.KindId)
                    {
						case Equipment.KINDID_EQUIPMENTUNIT:
							KindId = 109;
							ChainAdvanced<Depatment,Equipment> ce = new ChainAdvanced<Depatment,Equipment>()
							{ 
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce.Save();
							break;
						case Equipment.KINDID_EQUIPMENTAUTO:
							KindId = 109;
							ChainAdvanced<Depatment,Equipment> ce1 = new ChainAdvanced<Depatment,Equipment>()
							{ 
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce1.Save();
							break;
						case Equipment.KINDID_FOLDER:
							KindId = 105;
							Chain<Equipment> ce2 = new Chain<Equipment>()
							{ 
								Workarea = WADataProvider.WA,
								LeftId = model.ParentId,
								RightId = model.Id,
								KindId = KindId,
								StateId = State.STATEACTIVE
							};
							ce2.Save();
							break;
                    }

					//ChainAdvanced<Depatment,Equipment> ce = new ChainAdvanced<Depatment,Equipment>()
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
