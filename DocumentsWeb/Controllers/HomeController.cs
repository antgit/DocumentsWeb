using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using BusinessObjects;
using DocumentsWeb.Models;
using DevExpress.Web.Mvc;
using DevExpress.Web;

namespace DocumentsWeb.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        public ActionResult Index()
        {
            //ViewData["IsDinamicScriptRegister"] = true;
            //ViewData["ExtensionSuite.NavigationAndLayout"] = true;
            //ViewData["ExtensionSuite.Editors"] = true;
            return View();
        }
        //public ActionResult Index(string selectedNodeName)
        //{
        //    Uid user = (Membership.Provider as BusinessObjectMembershipProvider).GetUid(System.Web.HttpContext.Current.User.Identity.Name);
        //    if (user != null)
        //    {

        //        UserModel model = UserModel.ConvertToModel(user);

        //        try
        //        {
        //            WADataProvider.ModelsCache.Add(model.ModelId, model);
        //        }
        //        catch (ArgumentException e)
        //        {

        //        }

        //        string filePath = Request.MapPath("~/Images/" + user.Id + ".png");
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            model.Avatar = user.Id.ToString();
        //        }
        //        else
        //        {
        //            model.Avatar = "noavatar.png";
        //        }
        //        //return RedirectToAction("PersonalPageController", "Navigator");

        //        ViewResult res = View(model);
        //        string selNodeName = !string.IsNullOrEmpty(selectedNodeName) ? selectedNodeName : "Messages";
        //        //res.ViewData.Add("SelectedNodeName", "Incoming");
        //        res.ViewData.Add("SelectedNodeName", selNodeName);
        //        return res;
        //    }
        //    else
        //    {
        //        return RedirectToAction("LogOn", "Account");
        //        //return View("Welcome");
        //    }
        //}


        //public ActionResult NavpanelPartial()
        //{
        //    return View();
        //}
        

        //public ActionResult IndexPartial2(UserModel model)
        //{
        //    ViewData["SelectedNodeName"] = "messages";
        //    return PartialView(model);
        //}


        //public ActionResult PersonalPagePartial()
        //{
        //    string nodeName = !string.IsNullOrEmpty(Request.Params["node_name"]) ? Request.Params["node_name"].ToString() : "messages";
        //    //UserModel model = (UserModel)Request.Params["usermodel"];
        //    string mId = Request.Params["mId"];
        //    //PartialViewResult result = PartialView(WADataProvider.modelsCache[mId]);
        //    /*PartialViewResult result = PartialView("PersonalPagePartial");
        //    result.ViewData.Add("node_name", node_name);            
        //    return result;*/
        //    ViewData["SelectedNodeName"] = "Incoming";
        //    //ViewData["usermodel"] = (UserModel)WADataProvider.modelsCache[mId];
        //    //WADataProvider.modelsCache.Keys
        //    return PartialView("PersonalPagePartial");
        //}

        //public ActionResult SelectPartial()
        //{
        //    string nodeName;
        //    UserModel Model;
        //    RouteValueDictionary rv = RouteData.Values;
        //    nodeName = rv["node_name"].ToString();
        //    Model = (UserModel)rv["usermodel"];

        //    switch (nodeName)
        //    {
        //        case "information":
        //            return PartialView("InformationPartial", Model);
        //            break;

        //        case "messages":
        //            return PartialView("MessagesPartial", Model);
        //            break;

        //        case "incoming_messages":
        //            List<Message> inList = MessageModel.GetIncomingMessages(Model.Id);
        //            ViewData["incomingList"] = inList;
        //            return PartialView("incomingMessagesPartial", Model);
        //            break;

        //        case "outcoming_messages":
        //            return PartialView("outcomingMessagesPartial", Model);
        //            break;

        //        case "rough_copies_messages":
        //            return PartialView("roughCopiesMessagesPartial", Model);
        //            break;

        //        case "contacts":
        //            return PartialView("ContactsPartial", Model);
        //            break;

        //        case "tasks":
        //            return PartialView("TasksPartial", Model);
        //            break;

        //        case "files":
        //            return PartialView("FilesPartial", Model);
        //            break;

        //        case "events":
        //            return PartialView("EventsPartial", Model);
        //            break;

        //        default:
        //            return PartialView("IndexPartial2", Model);
        //            break;
        //    }

        //}

        //public ActionResult GeneralInformationPartial()
        //{
        //    string mId = Request.Params["mId"];
        //    return PartialView("GeneralInformationPartial", (UserModel)WADataProvider.ModelsCache[mId]);
        //}

        //public ActionResult UserPage(string pageName)
        //{
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    ViewResult res = View("Index", model);
        //    res.ViewData.Add("SelectedNodeName", pageName);
        //    return res;
        //}

        //public ActionResult EditNewHePartial()
        //{
        //    //int m = mId;
        //    //MessageModel model = MessageModel.GetObject(mId);
        //    //return PartialView("EditNewHePartial", model);
        //    return PartialView("EditNewHePartial", new MessageModel());
        //}

        //#region methods, to apply some actions to messages(read,delete,refresh,create new, edit existing)
        //public ActionResult ToReading(int messageId, string nodeName)
        //{
        //    Message message = WADataProvider.WA.GetObject<Message>(messageId);
            
        //    message.ReadDone = true;
        //    message.ReadDate = DateTime.Now;
        //    message.ReadTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            
        //    message.Save();

        //    return RedirectToAction("Index", "Home", new { selectedNodeName = nodeName });
        //}
        ////----------------------------------------------------------------------------------------------

        //public ActionResult ToDelete(int messageId, string nodeName)
        //{
        //    Message message = WADataProvider.WA.GetObject<Message>(messageId);
        //    message.StateId = State.STATEDELETED;
        //    message.Save();

        //    return RedirectToAction("Index", "Home", new { selectedNodeName = nodeName });
        //}
        ////----------------------------------------------------------------------------------------------

        //public ActionResult ToRefresh(string nodeName)
        //{
        //    return RedirectToAction("Index", "Home", new { selectedNodeName = nodeName });
        //}
        ////----------------------------------------------------------------------------------------------


        //[HttpGet]
        //public ActionResult Edit(int id)
        //{
        //    MessageModel value = id == 0 ? new MessageModel { Id = 0, Name = "" } : MessageModel.GetObject(id);
        //    return View(value);
        //}
        ////----------------------------------------------------------------------------------------------

        //public ActionResult EditPartial(int id)
        //{
        //    MessageModel value = id == 0 ? new MessageModel { Id = 0, Name = "" } : MessageModel.GetObject(id);
        //    return PartialView(value);
        //}
        ////----------------------------------------------------------------------------------------------

        //[HttpGet]
        //public ActionResult EditNew()
        //{            
        //    MessageModel value = new MessageModel {Id = 0, Name = ""};
            
        //    /*ViewData["Title"] = "Новое сообщение";
        //    ViewData["AutoUpdateGrids"] = new string[] { "roughCopiesMessagesGrid", "outcomingMessagesGrid" };
        //    ViewData["OnSave"] = "RefreshHtmlEdit";*/
        //    return View(value);
        //}
        ////----------------------------------------------------------------------------------------------

        //[HttpPost]
        //public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] MessageModel model)
        //{
        //    string s = HtmlEditorExtension.GetHtml("NameFull");
        //    model.NameFull = s;
        //    if (ModelState.IsValid)
        //    {
        //        Message message;
        //        if (model.IsValid())
        //        {
        //            if (model.IsSend)
        //            {
        //                model.SendDate = DateTime.Now;
        //                model.SendTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        //            }
                                                                
        //            message = model.ToObject(WADataProvider.WA);
        //            message.Save();
        //            model.Id = message.Id;
                    
        //            return View("PopupWindowClose", model);
                                            
        //        }
        //        else                     
        //             return View(model);
        //    }
        //    else
        //        return View(model); 
        //}
        ////----------------------------------------------------------------------------------------------

        //[HttpPost]
        //public ActionResult EditNew([ModelBinder(typeof(DevExpressEditorsBinder))] MessageModel model)
        //{
        //    string s = HtmlEditorExtension.GetHtml("NameFull");
        //    model.NameFull = s;
        //    if (ModelState.IsValid)
        //    {
        //        Message message;
        //        if (model.IsValid())
        //        {
        //            if (model.IsSend)
        //            {
        //                model.SendDate = DateTime.Now;
        //                model.SendTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        //            }

        //            message = model.ToObject(WADataProvider.WA);
        //            message.Save();
        //            model.Id = message.Id;
                    
        //            return View("EditingComplete2", model);                    
                    
        //        }
        //        else
        //            return View(model);
        //    }
        //    else
        //        return View(model);    
        //}
        ////----------------------------------------------------------------------------------------------
        //#endregion

        //#region methods for a messages grids callback

        //public ActionResult MessagesPartial()
        //{
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    return PartialView("MessagesPartial", model);
        //}

        //public ActionResult IncomingGridPartial()
        //{            
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    return PartialView("IncomingGridPartial", model);
        //}

        //public ActionResult OutcomingGridPartial()
        //{
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    return PartialView("OutcomingGridPartial", model);
        //}

        //public ActionResult RecycledGridPartial()
        //{
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    return PartialView("RecycledGridPartial", model);
        //}

        //public ActionResult RoughCopiesGridPartial()
        //{
        //    UserModel model = UserModel.GetObject(WADataProvider.CurrentUser.Id);
        //    return PartialView("RoughCopiesGridPartial", model);
        //}
        //#endregion
    }
}
