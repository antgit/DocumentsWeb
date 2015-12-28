using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using BusinessObjects.Security;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Code
{
    public class LayoutHelper
    {
        #region Настройки справочников
        /// <summary>
        /// Сохрание настрок внешнего вида в БД
        /// </summary>
        /// <param name="id">Ид настройки</param>
        /// <param name="userId">Ид пользователя</param>
        /// <param name="controllerName">Имя контроллера</param>
        /// <param name="context">HTTP контекст</param>
        /// <param name="chkGroupPanel">Видимость панели группировки</param>
        /// <param name="chkFilterPanel">Видимость панели фильтров</param>
        /// <param name="chkLinkName">Показывать имена сущностей как ссылки на из редактирование</param>
        /// <param name="chkPreview">Показывать строку предпросмотра</param>
        public static void SaveLayoutToDatabase(int id, int userId, string controllerName, HttpContextBase context, bool? chkGroupPanel = null, bool? chkFilterPanel = null, bool? chkLinkName = null, bool? chkPreview = null)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = FindStorage(userId, code);
            LayoutSettings settings;

            if (storage == null)
            {
                storage = new XmlStorage
                {
                    Workarea = WADataProvider.WA,
                    Name = "Layout for " + controllerName,
                    Code = code,
                    KindId = XmlStorage.KINDID_LAYOUTCONTROLDATA,
                    UserId = userId
                };
                settings = new LayoutSettings();
            }
            else
                settings = LayoutSettings.Load(storage.XmlData);

            string gridSettings = null;
            string cookieName = controllerName + "Grid";
            if (context.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = context.Request.Cookies[cookieName];
                gridSettings = cookie.Value;
                context.Response.Cookies.Add(cookie);
            }

            string navSettings = null;
            cookieName = controllerName + "Nav";
            if (context.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = context.Request.Cookies[cookieName];
                navSettings = cookie.Value;
                context.Response.Cookies.Add(cookie);
            }

            settings.Settings[id].GridSetting = gridSettings;
            settings.Settings[id].NavSetting = navSettings;
            if (chkGroupPanel.HasValue)
                settings.Settings[id].ShowGroupPanel = chkGroupPanel.Value;
            if(chkFilterPanel.HasValue)
                settings.Settings[id].ShowFilterPanel = chkFilterPanel.Value;
            if (chkLinkName.HasValue)
                settings.Settings[id].ShowLinkInName = chkLinkName.Value;
            if(chkPreview.HasValue)
                settings.Settings[id].ShowPreview = chkPreview.Value;
            
            string res=settings.Save();
            if (storage.XmlData != res)
            {
                storage.XmlData = res;
                storage.Save();
            }
        }

        /// <summary>
        /// Загрузка настрок внешнего вида из БД в cookies
        /// </summary>
        /// <param name="id">Ид настройки</param>
        /// <param name="userId">Ид пользователя</param>
        /// <param name="controllerName">Имя контроллера</param>
        /// <param name="context">HTTP контектст</param>
        public static void LoadLayoutFromDatabase(int id, int userId, string controllerName, HttpContextBase context)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = FindStorage(userId, code);
            LayoutSettings settings;
            if (storage == null)
            {
                storage = new XmlStorage
                {
                    Workarea = WADataProvider.WA,
                    Name = "Layout for " + controllerName,
                    Code = code,
                    KindId = XmlStorage.KINDID_LAYOUTCONTROLDATA,
                    UserId = userId
                };
                settings = new LayoutSettings();
            }
            else
                settings = LayoutSettings.Load(storage.XmlData);

            string gridSettings, navSettings;

            gridSettings = settings.Settings[id].GridSetting;
            navSettings = settings.Settings[id].NavSetting;

            string cookieName = controllerName + "Grid";
            //if (context.Request.Cookies.AllKeys.Contains(cookieName))
            //{
            //    HttpCookie cookie = context.Request.Cookies[cookieName];
            //    cookie.Value = gridSettings;
            //    context.Response.Cookies.Add(cookie);
            //}

            cookieName = controllerName + "Nav";
            if (context.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = context.Request.Cookies[cookieName];
                cookie.Value = navSettings;
                context.Response.Cookies.Add(cookie);
            }

            if (settings.SelectedSettingIndex != id || storage.Id == 0)
            {
                settings.SelectedSettingIndex = id;
                storage.XmlData = settings.Save();
                storage.Save();
            }
        }
        #endregion

        public static void DeleteCookies(string controllerName, HttpContextBase context)
        {
            string cookieName = controllerName + "Grid";
            if (context.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies.Add(cookie);
            }

            cookieName = controllerName + "Nav";
            if (context.Request.Cookies.AllKeys.Contains(cookieName))
            {
                HttpCookie cookie = new HttpCookie(cookieName);
                cookie.Expires = DateTime.Now.AddDays(-1d);
                context.Response.Cookies.Add(cookie);
            }
        }

        public static int GetCurrentSettingIndex(string controllerName)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = FindStorage(WADataProvider.CurrentUser.Id, code);
            if (storage == null)
                return 0;
            LayoutSettings layoutSettings = LayoutSettings.Load(storage.XmlData);
            return layoutSettings.SelectedSettingIndex;
        }

        public static LayoutSettings GetCurrentSetting(string controllerName)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = FindStorage(WADataProvider.CurrentUser.Id, code);
            if (storage == null)
                return new LayoutSettings();
            LayoutSettings layoutSettings = LayoutSettings.Load(storage.XmlData);
            return layoutSettings;
        }

        public static XmlStorage FindStorage(int userId, string code)
        {
            XmlStorage storage = WADataProvider.WA.Cashe.GetCasheData<XmlStorage>().Dictionary.Values.FirstOrDefault(
                   w => w.Code == code && w.UserId == userId);

            if (storage == null)
            {
                List<XmlStorage> collObjs = new List<XmlStorage>();
                collObjs = WADataProvider.WA.Empty<XmlStorage>().FindBy(code: code, userId: userId, useAndFilter: true);
                if (collObjs == null)
                {
                    storage = WADataProvider.WA.GetCollection<XmlStorage>().FirstOrDefault(w => w.Code == code && w.UserId == userId);
                }
                else if (collObjs.Count == 0)
                {
                    storage = WADataProvider.WA.GetCollection<XmlStorage>().FirstOrDefault(w => w.Code == code && w.UserId == userId);
                }
                else if (collObjs.Count == 1)
                {
                    storage = collObjs[0];
                }
            }
            return storage;
        }
    }
}