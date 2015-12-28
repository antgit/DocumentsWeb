using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Agents.Models;
using BusinessObjects;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Routes.Models;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

// Группировка зон по mycompany и по произвольным иерархиям

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class MapController : Controller
    {
        //
        // GET: /Routes/Map/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ZonesListPartial()
        {
            return PartialView();
        }

        public ActionResult RouteMemberListPartial()
        {
            return PartialView();
        }

        public ActionResult RoutesListPartial()
        {
            return PartialView();
        }

        #region Адрес клиента
        public ActionResult LightAddress(int id)
        {
            AgentAddressModel addr = AgentAddressModel.GetMktgAddressByAgentId(id);
            AgentAddressModel model = (addr == null ? new AgentAddressModel { OwnId = id, AddressKindId = AgentAddress.KINDID_ACTUALADDRESS } : addr);
            return View(model);
        }

        [HttpPost]
        public ActionResult LightAddress([ModelBinder(typeof(DevExpressEditorsBinder))] AgentAddressModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
                return View("PopupWindowClose", new ClientModel { Name = model.GetActualName(), Id = 0 });
            }
            return View(model);
        }

        public ActionResult cmbTerritoryPartial()
        {
            int countryId = int.Parse(Request.Params["CountryId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("CountryId", countryId);
            return result;
        }

        public ActionResult cmbRegionPartial()
        {
            if (Request.Params["TerritoryId"] == null || Request.Params["TerritoryId"] == "null")
                return PartialView();
            int territoryId = int.Parse(Request.Params["TerritoryId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("TerritoryId", territoryId);
            return result;
        }

        public ActionResult cmbTownPartial()
        {
            int territoryId = 0;
            int regionId = 0;
            try
            {
                territoryId = int.Parse(Request.Params["TerritoryId"]);
            }
            catch (Exception)
            {
                return PartialView();
            }
            try
            {
                regionId = int.Parse(Request.Params["RegionId"]);
            }
            catch (Exception)
            {
                return PartialView();
            }
            PartialViewResult result = PartialView();
            result.ViewData.Add("TerritoryId", territoryId);
            result.ViewData.Add("RegionId", regionId);
            return result;
        }

        public ActionResult cmbStreetPartial()
        {
            int townId = int.Parse(Request.Params["TownId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("TownId", townId);
            return result;
        }
        #endregion

        #region Маршрут
        public ActionResult RouteEditor(int id)
        {
            RouteTemplateModel model = null;
            if (id > 0)
            {
                DocumentRoute doc = new DocumentRoute { Workarea = WADataProvider.WA };
                doc.Load(id);
                model = RouteTemplateModel.ConvertToModel(doc);
            }
            else
            {
                model = new RouteTemplateModel();
            }
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View(model);
        }

        public ActionResult RouteEditorDetailsPartial(string mId)
        {
            RouteTemplateModel model = (RouteTemplateModel)WADataProvider.ModelsCache.Get(mId);
            return PartialView(model);
        }

        public ActionResult ZonePartial(string mId)
        {
            int id = int.Parse(Request.Params["Id"]);
            string Name = Request.Params["Name"];
            PartialViewResult result = PartialView((RouteTemplateModel)WADataProvider.ModelsCache.Get(mId));
            result.ViewData.Add("Name", Name);
            result.ViewData.Add("Id", id);
            return result;
        }

        [HttpPost]
        public ActionResult RouteEditorSave([ModelBinder(typeof(DevExpressEditorsBinder))] RouteTemplateModel model, string mId)
        {
            RouteTemplateModel details = (RouteTemplateModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                model.Details = details.Details;
                DocumentRoute doc = model.ToObject();
                doc.FlagsValue = 9;
                doc.Save();
                model.Id = doc.Id;
                return View("PopupWindowClose", model);
            }
            return View("RouteEditor", model);
        }

        [HttpPost]
        public ActionResult AddZonePartial([ModelBinder(typeof(DevExpressEditorsBinder))] RouteTemplateDetailModel model, string mId)
        {
            RouteTemplateModel route = (RouteTemplateModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                model.StateId = State.STATEACTIVE;
                ZoneModel z = ZoneModel.GetZone(model.AgentId);
                model.AddressId = z.AddressId;
                model.AgentName = z.Name;
                model.AddressName = z.Address;
                route.Details.Add(model);
            }
            return PartialView("RouteEditorDetailsPartial", route);
        }

        [HttpPost]
        public ActionResult UpdateZonePartial([ModelBinder(typeof(DevExpressEditorsBinder))] RouteTemplateDetailModel model, string mId)
        {
            RouteTemplateModel route = (RouteTemplateModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                model.StateId = State.STATEACTIVE;
                ZoneModel z = ZoneModel.GetZone(model.AgentId);
                model.AddressId = z.AddressId;
                model.AgentName = z.Name;
                model.AddressName = z.Address;
                for (int i = 0; i < route.Details.Count; i++)
                {
                    if (route.Details[i].RowId == model.RowId)
                    {
                        route.Details[i] = model;
                        break;
                    }
                }
            }
            return PartialView("RouteEditorDetailsPartial", route);
        }

        [HttpPost]
        public ActionResult DeleteZonePartial(string RowId, string mId)
        {
            RouteTemplateModel route = (RouteTemplateModel)WADataProvider.ModelsCache.Get(mId);
            List<RouteTemplateDetailModel> v_list = route.Details.Where(w => w.StateId != State.STATEDELETED).OrderBy(o => o.OrderNo).ToList();
            bool isDeleted = false;
            for (int i = 0; i < v_list.Count; i++)
            {
                if (!isDeleted)
                {
                    if (v_list[i].RowId == RowId)
                    {
                        if (v_list[i].Id == 0)
                        {
                            v_list.RemoveAt(i);
                        }
                        else
                        {
                            v_list[i].StateId = State.STATEDELETED;
                        }
                        isDeleted = true;
                    }
                }
                else
                {
                    v_list[i].OrderNo--;
                }
            }
            return PartialView("RouteEditorDetailsPartial", route);
        }

        public void DeleteRoute(int Id)
        {
            RouteTemplateModel.Delete(Id);
        }

        public int GetDefaultDeviceId(int RouteMemberId)
        {
            RouteMember rm = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(RouteMemberId);
            return rm.DeviceId;
        }

        #region Перемещение точек маршрута
        private bool OrderNoError(List<RouteTemplateDetailModel> list)
        {
            bool flag = false;

            List<RouteTemplateDetailModel> v_list = list.Where(w => w.StateId != State.STATEDELETED).OrderBy(o => o.OrderNo).ToList();
            int i = 0;
            foreach (RouteTemplateDetailModel m in v_list)
            {
                if (i == m.OrderNo)
                {
                    i++;
                }
                else
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        private void CorrectOrderNo(List<RouteTemplateDetailModel> list)
        {
            List<RouteTemplateDetailModel> v_list = list.Where(w => w.StateId != State.STATEDELETED).OrderBy(o => o.OrderNo).ToList();
            int i = 0;
            foreach (RouteTemplateDetailModel m in v_list)
            {
                m.OrderNo = i;
                i++;
            }
        }

        private void MoveUp(List<RouteTemplateDetailModel> list, int PointId)
        {
            List<RouteTemplateDetailModel> v_list = list.Where(w => w.StateId != State.STATEDELETED).OrderBy(o => o.OrderNo).ToList();
            for (int i = 0; i < v_list.Count; i++)
            {
                if (v_list[i].Id == PointId)
                {
                    if (v_list[i].OrderNo > 0)
                    {
                        v_list[i].OrderNo--;
                        v_list[i - 1].OrderNo++;
                    }
                    break;
                }
            }
        }

        private void MoveDown(List<RouteTemplateDetailModel> list, int PointId)
        {
            List<RouteTemplateDetailModel> v_list = list.Where(w => w.StateId != State.STATEDELETED).OrderBy(o => o.OrderNo).ToList();
            for (int i = 0; i < v_list.Count; i++)
            {
                if (v_list[i].Id == PointId)
                {
                    if (v_list[i].OrderNo < v_list.Count - 1)
                    {
                        v_list[i].OrderNo++;
                        v_list[i + 1].OrderNo--;
                    }
                    break;
                }
            }
        }

        public void MoveRoutePoint(string ModelId, int PointId, string Direction)
        {
            RouteTemplateModel route = (RouteTemplateModel)WADataProvider.ModelsCache.Get(ModelId);
            if (OrderNoError(route.Details))
            {
                CorrectOrderNo(route.Details);
            }
            if (Direction == "UP")
            {
                MoveUp(route.Details, PointId);
            }
            else if (Direction == "DOWN")
            {
                MoveDown(route.Details, PointId);
            }
        }
        #endregion

        #endregion

        #region Треки

        public ActionResult TracksListPartial()
        {
            return PartialView();
        }

        #endregion
    }
}
