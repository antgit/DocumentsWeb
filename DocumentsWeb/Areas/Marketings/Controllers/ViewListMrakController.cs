using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using DevExpress.Web.Mvc;

namespace DocumentsWeb.Areas.Marketings.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPMKTG, Uid.GROUP_GROUPWEBADMIN })]
    public class ViewListMrakController : CoreDocumentListControler
    {
        public ViewListMrakController()
        {
            Name = "WEBМРАК";
            FolderCodeFind = Folder.CODE_FIND_MKTG_MRAK;
        }
        
        public ActionResult Index()
        {
            return View(MktgHelper.GetDocumentsMrak(true));
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(MktgHelper.GetDocumentsMrak(refresh));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ToTrash(int id)
        {
            if (id != 0)
            {
                try
                {
                    DocumentModel.Remove(id);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("IndexPartial", MktgHelper.GetDocumentsMrak(true));
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <param name="subtype">Подтип данных</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type, string subtype)
        {
            GridViewSettings settings = new GridViewSettings { Name = "gvDocuments" };
            settings.KeyFieldName = "Id";
            settings.CallbackRouteValues = new { Controller = "ViewListMrak", Action = "IndexPartial" };

            settings.Columns.Add(column =>
            {
                column.FieldName = "Date";
                column.Caption = "Дата";
                column.Width = 70;
                column.PropertiesEdit.DisplayFormatString = "d";
                column.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
                column.ColumnType = MVCxGridViewColumnType.DateEdit;
            });

            MVCxGridViewColumn col_number = settings.Columns.Add("DocNumber", "Номер");
            col_number.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_number.Width = 70;

            MVCxGridViewColumn col_who = settings.Columns.Add("CompanyName", "Компания");
            col_who.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_who.Width = 450;
            col_who.Visible = false;

            MVCxGridViewColumn col_whoCode = settings.Columns.Add("CompanyCode", "Предприятие");
            col_whoCode.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_whoCode.Width = 200;

            MVCxGridViewColumn col_whom = settings.Columns.Add("AgentDepartmentToName", "Клиент");
            col_whom.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_whom.Width = 450;

            MVCxGridViewColumn col_manager = settings.Columns.Add("ManagerName", "Менеджер");
            col_manager.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_manager.Width = 250;

            MVCxGridViewColumn col_TownName = settings.Columns.Add("TownName", "Город");
            col_TownName.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_TownName.Width = 150;

            MVCxGridViewColumn col_StreetName = settings.Columns.Add("StreetName", "Улица");
            col_StreetName.CellStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;
            col_StreetName.Width = 150;

            /*settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("NameFull", "Печатное наименование");
            settings.Columns.Add("Code", "Код");*/

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, MktgHelper.GetDocumentsMrak(true));
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, MktgHelper.GetDocumentsMrak(true));
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }
    }
}
