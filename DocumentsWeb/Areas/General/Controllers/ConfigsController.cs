using System.Collections.Generic;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class ConfigsController : Controller
    {
        public ActionResult SaveTableColumnsConfig()
        {
            List<TableColumnModel> list = new List<TableColumnModel>();
            string LastUrl = Request.Params["hCurrUrl"].ToString();
            string GridController = Request.Params["hGridController"].ToString();
            string GridAction = Request.Params["hGridAction"].ToString();
            int Count = int.Parse(Request.Params["colsCount"].ToString());

            for (int i = 0; i < Count; i++)
            {
                string Caption = Request.Params["Caption" + i.ToString()].ToString();
                string CaptionDefault = Request.Params["colCaptionDefault" + i.ToString()].ToString();
                string ColName = Request.Params["colName" + i.ToString()].ToString();
                string ColValue = Request.Params["colValue" + i.ToString()].ToString();
                int Transition = int.Parse(Request.Params["Transition" + i.ToString() + "_VI"].ToString());

                TableColumnModel model = new TableColumnModel {
                    Caption = Caption,
                    CaptionDefault = CaptionDefault,
                    ColName = ColName,
                    ColValue = ColValue,
                    Transition = (ColumnTransitionType)Transition
                };
                list.Add(model);
            }

            TableColumnModel.SetCollection(GridController, GridAction, list.ToArray());
            return Redirect(LastUrl);
        }
    }
}
