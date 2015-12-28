using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using DevExpress.Web.Mvc;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using System.Web.UI.WebControls;
using BusinessObjects;

namespace DocumentsWeb.Models
{
    /// <summary>
    /// Тип перехода из колонки на объект
    /// </summary>
    public enum ColumnTransitionType {
        /// <summary>
        /// Нет перехода
        /// </summary>
        NoTransition,

        /// <summary>
        /// Локальное редактирование
        /// </summary>
        Editing,

        /// <summary>
        /// Переход на объект
        /// </summary>
        TransitionTo,

        /// <summary>
        /// Выбор пользователя
        /// </summary>
        UserChoice
    }

    /// <summary>
    /// Модель колонки
    /// </summary>
    public class TableColumnModel
    {
        /// <summary>
        /// Имя колонки cо значениями
        /// </summary>
        public string ColName { get; set; }

        /// <summary>
        /// Имя колонки с идентификатором
        /// </summary>
        public string ColValue { get; set; }

        /// <summary>
        /// Заголовок по умолчанию
        /// </summary>
        public string CaptionDefault { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Переход на объект из колонки
        /// </summary>
        public ColumnTransitionType Transition { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TableColumnModel()
        {
            this.Transition = ColumnTransitionType.NoTransition;
        }

        /// <summary>
        /// Загружает параметры колонки
        /// <param name="controller">Имя контроллера обслуживающего грид</param>
        /// <param name="action">Имя метода обслуживающего грид</param>
        /// </summary>
        public void Load(string controller, string action)
        {
            string path = "GridColumnsSettings-" + controller + "-" + action + "-" + WADataProvider.CurrentUser.Name;
            //JavaScriptSerializer json = new JavaScriptSerializer();
            //HttpContext context = HttpContext.Current;
            //string current_val = context.Request.Cookies.AllKeys.Contains(path) ? HttpUtility.UrlDecode(context.Request.Cookies[path].Value) : "";
            string current_val = "";

            XmlStorage storage = WADataProvider.GetXmlStorage(path);
            if (storage != null)
            {
                current_val = storage.XmlData;
            }

            if (current_val.Length > 0)
            {
                System.IO.StringReader sr = new System.IO.StringReader(current_val);
                System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(TableColumnModel[]));
                TableColumnModel[] arr = (TableColumnModel[])xml.Deserialize(sr);

                //TableColumnModel[] arr = json.Deserialize<TableColumnModel[]>(current_val);

                if (arr != null)
                {
                    foreach (TableColumnModel model in arr)
                    {
                        if (model.ColName == ColName)
                        {
                            CaptionDefault = model.CaptionDefault;
                            Caption = model.Caption;
                            Transition = model.Transition;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Генерирует колонку с выбором
        /// </summary>
        /// <example>
        /// TableColumnModel.GenerateColumn(
        /// "AgentFromName",
        /// "Кто",
        /// "javascript:popupVariants({0}, 'Byer');",
        /// "javascript:moveToAgent({0}, 'Byer');",
        /// "javascript:editAgent({0}, 'Byer');",
        /// 250,
        /// settings
        /// );
        /// </example>
        /// <param name="ColName">Имя колонки</param>
        /// <param name="DefaultCaption">Заголовок колонки по умолчанию</param>
        /// <param name="VariantURL">Ссылка на выбор метода редактирования</param>
        /// <param name="MoveToURL">Ссылка на редактирование с переходом</param>
        /// <param name="EditURL">Ссылка на локальное редактирование</param>
        /// <param name="ColWidth">Ширина колонки</param>
        /// <param name="settings">Параметризатор грида</param>
        [Obsolete]
        public static void GenerateColumn(string ColName, string DefaultCaption, string VariantURL, string MoveToURL, string EditURL, System.Web.UI.WebControls.Unit ColWidth, GridViewSettings settings)
        {
            Dictionary<string, TableColumnModel> columns = TableColumnModel.GetCollection(settings);
            if (columns.Keys.Contains(ColName))
            {
                MVCxGridViewColumn col_whom = settings.Columns.Add(columns[ColName].ColName, columns[ColName].Caption.Length == 0 ? columns[ColName].CaptionDefault : columns[ColName].Caption);
                col_whom.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                col_whom.CellStyle.VerticalAlign = VerticalAlign.Top;
                col_whom.CellStyle.HorizontalAlign = HorizontalAlign.Left;
                col_whom.Width = ColWidth;
                col_whom.ColumnType = columns[ColName].Transition == ColumnTransitionType.NoTransition ? MVCxGridViewColumnType.TextBox : MVCxGridViewColumnType.HyperLink;
                if (columns[ColName].Transition != ColumnTransitionType.NoTransition)
                {
                    col_whom.FieldName = columns[ColName].ColValue;
                    HyperLinkProperties prop_whom = col_whom.PropertiesEdit as HyperLinkProperties;
                    switch (columns[ColName].Transition)
                    {
                        case ColumnTransitionType.UserChoice:
                            prop_whom.NavigateUrlFormatString = HttpUtility.UrlDecode(VariantURL);
                            break;
                        case ColumnTransitionType.TransitionTo:
                            prop_whom.NavigateUrlFormatString = HttpUtility.UrlDecode(MoveToURL);
                            break;
                        case ColumnTransitionType.Editing:
                            prop_whom.NavigateUrlFormatString = HttpUtility.UrlDecode(EditURL);
                            break;
                    }
                    prop_whom.TextField = columns[ColName].ColName;
                }
            }
            else
            {
                settings.Columns.Add(ColName, DefaultCaption).Width = ColWidth;
            }
        }

        /// <summary>
        /// Возвращает словарь с настройками колонок
        /// </summary>
        /// <param name="s">Параметры грида</param>
        /// <returns>Словарь с настройками колонок</returns>
        public static Dictionary<string, TableColumnModel> GetCollection(GridViewSettings s)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];
            return GetCollection(ct, ac);
        }

        /// <summary>
        /// Возвращает словарь с настройками колонок
        /// </summary>
        /// <param name="controller">Имя контроллера обслуживающего грид</param>
        /// <param name="action">Имя метода обслуживающего грид</param>
        /// <returns>Словарь с настройками колонок</returns>
        public static Dictionary<string, TableColumnModel> GetCollection(string controller, string action)
        {
            Dictionary<string, TableColumnModel> dic = new Dictionary<string, TableColumnModel>();
            string path = "GridColumnsSettings-" + controller + "-" + action + "-" + WADataProvider.CurrentUser.Name;
            //JavaScriptSerializer json = new JavaScriptSerializer();
            //HttpContext context = HttpContext.Current;
            //string current_val = context.Request.Cookies.AllKeys.Contains(path) ? HttpUtility.UrlDecode(context.Request.Cookies[path].Value) : "";

            string current_val = "";
            XmlStorage storage = WADataProvider.GetXmlStorage(path);
            if (storage != null)
            {
                current_val = storage.XmlData;
            }

            if (current_val.Length > 0)
            {
                System.IO.StringReader sr = new System.IO.StringReader(current_val);
                System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(TableColumnModel[]));
                TableColumnModel[] arr = (TableColumnModel[])xml.Deserialize(sr);

                //TableColumnModel[] arr = json.Deserialize<TableColumnModel[]>(current_val);

                if (arr != null)
                {
                    foreach (TableColumnModel model in arr)
                    {
                        dic.Add(model.ColName, model);
                    }
                }
            }

            return dic;
        }

        /// <summary>
        /// Сохраняет коллекцию настроек колонок
        /// </summary>
        /// <param name="controller">Имя контроллера обслуживающего грид</param>
        /// <param name="action">Имя метода обслуживающего грид</param>
        /// <param name="array">Коллекция настроек</param>
        public static void SetCollection(string controller, string action, TableColumnModel[] array)
        {
            string path = "GridColumnsSettings-" + controller + "-" + action + "-" + WADataProvider.CurrentUser.Name;
            //JavaScriptSerializer json = new JavaScriptSerializer();
            //HttpContext context = HttpContext.Current;
            //HttpCookie hieFilter = new HttpCookie(path);
            //hieFilter.Expires = DateTime.Now.AddYears(50);

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(TableColumnModel[]));
            xml.Serialize(sw, array);

            XmlStorage storage = WADataProvider.GetXmlStorage(path, true);
            if (storage != null)
            {
                storage.XmlData = sw.ToString();
                storage.Save();
            }
            //hieFilter.Value = HttpUtility.UrlEncode(sw.ToString());//json.Serialize(array);
            
            
            /*context.Response.Cookies.Add(hieFilter);
            if (context.Request.Cookies.AllKeys.Contains(path))
            {
                context.Request.Cookies[path].Value = hieFilter.Value;
            }
            else
            {
                context.Request.Cookies.Add(hieFilter);
            }*/
        }

        /// <summary>
        /// Создание колонок грида на основе объекта CustomViewList
        /// </summary>
        /// <param name="settings">Параметризатор грида</param>
        /// <param name="list">Список</param>
        public static void GenerateColumns(GridViewSettings settings, CustomViewList list)
        {
            foreach(CustomViewColumn column in list.Columns.OrderBy(s=>s.OrderNo))
            {
                if (column.DataProperty == "Image" || column.DataProperty == "StateImage" || column.DataProperty == "State")
                    continue;
                settings.Columns.Add(col=>
                                         {
                                             col.Caption = column.Name;
                                             col.FieldName = column.DataProperty;
                                             col.Visible = column.Visible;
                                             col.Width = column.With;
                                             col.CellStyle.VerticalAlign = VerticalAlign.Top;
                                         });
            }

            //settings.SettingsCookies.Enabled = false;
        }
    }
}