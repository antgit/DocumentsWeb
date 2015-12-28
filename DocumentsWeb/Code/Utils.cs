using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxUploadControl;
using System.Linq;
using BusinessObjects;
using DocumentsWeb.Models;
using System.Xml;
using DevExpress.Web.Mvc;
using DevExpress.Web.ASPxGridView;
namespace DocumentsWeb
{
    public static class Utils
    {
        const string
            CurrentDemoKey = "DXCurrentDemo",
            CurrentThemeCookieKeyPrefix = "DXCurrentTheme",
            DefaultTheme = "DevEx",
            PageSizeCookie = "PageSize";

        public static Dictionary<string, string> CastAnonymous(object an)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string value = Convert.ToString(an);
            value = value.Replace("{", "").Replace("}", "").Trim();
            string[] values = value.Split(',');

            foreach (string str in values)
            {
                string[] key_val = str.Trim().Split('=');
                dic.Add(key_val[0].Trim(), key_val[1].Trim());
            }

            return dic;
        }

        static HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        static HttpRequest Request
        {
            get { return Context.Request; }
        }

        public static string CurrentThemeCookieKey
        {
            // TODO: CurrentThemeCookieKeyPrefix
            //get { return CurrentThemeCookieKeyPrefix + DemosModel.Current.Key; }
            get { return CurrentThemeCookieKeyPrefix; }
        }

        public static string CurrentTheme
        {
            get
            {
                /*if (Request.Cookies[CurrentThemeCookieKey] != null)
                {
                    return HttpUtility.UrlDecode(Request.Cookies[CurrentThemeCookieKey].Value);
                }
                else
                {*/
                    var prop = DocumentsWeb.Models.WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
                    if (prop != null)
                    {
                        SystemParameterUser user_prop = null;
                        try { user_prop = prop.GetUserParams().First(w => w.Name == HttpContext.Current.User.Identity.Name); } catch { }

                        if (user_prop != null)
                        {
                            return user_prop.ValueString;
                        }
                        else
                        {
                            return string.IsNullOrEmpty(prop.ValueString)? "Aqua": prop.ValueString;
                        }
                    }
                //}
                return DefaultTheme;
            }
        }

        /*public static string GetBannerText
        {
            get
            {
               
                var prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBBANERTEXT");
                if (prop != null)
                {
                    SystemParameterUser user_prop = null;
                    try { user_prop = prop.GetUserParams().First(w => w.Name == HttpContext.Current.User.Identity.Name); }
                    catch { }

                    if (user_prop != null)
                    {
                        return user_prop.ValueString;
                    }
                    else
                    {
                        return prop.ValueString;
                    }
                }
                //}
                return string.Empty;
            }
        }*/

        #region Pager utils
        public static int GetPageSize(string controller, string action, string grid_name)
        {
            XmlStorage storage = WADataProvider.GetXmlStorage("WEB_GRIDS_PAGERS_LISTS");
            if (storage != null)
            {
                string xml = storage.XmlData;
                string grid_id = controller.ToUpper() + "_" + action.ToUpper() + "_" + grid_name.ToUpper();
                if (xml.Length > 0)
                {
                    GridsPagersList pagers = new GridsPagersList();
                    pagers.LoadFromXML(xml);
                    return pagers.GetPages(grid_id);
                }
            }
            return 15;
        }

        public static int GetPageSize(GridViewSettings s)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            return GetPageSize(dic["Controller"], dic["Action"], s.Name);
        }

        public static void SetPageSize(string controller, string action, string grid_name, int pages)
        {
            XmlStorage storage = WADataProvider.GetXmlStorage("WEB_GRIDS_PAGERS_LISTS", true);
            string xml = storage.XmlData;
            string grid_id = controller.ToUpper() + "_" + action.ToUpper() + "_" + grid_name.ToUpper();
            
            GridsPagersList pagers = new GridsPagersList();
            if (xml.Length > 0)
            {
                pagers.LoadFromXML(xml);
            }
            pagers.SetPages(grid_id, pages);
            storage.XmlData = pagers.SaveToXML();
            storage.Save();
        }
        #endregion

        #region Hierachy utils
        public static void AddHieRoot(GridViewSettings s, int root)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];
            AddHieRoot(ct, ac, root);
        }

        public static void AddHieRoot(string controller, string action, int root)
        {
            string path = "HieFiltersSettings-" + controller + "-" + action;

            HttpContext context = HttpContext.Current;
            string current_val = context.Request.Cookies.AllKeys.Contains(path) ? context.Request.Cookies[path].Value : "";
            List<string> list = current_val.Split(',').Where(w => w.Length > 0).ToList();
            list.Add(root.ToString());
            HttpCookie hieFilter = new HttpCookie(path);
            hieFilter.Expires = DateTime.Now.AddYears(50);
            hieFilter.Value = String.Join(",", list);
            context.Response.Cookies.Add(hieFilter);
            if (context.Request.Cookies.AllKeys.Contains(path))
            {
                context.Request.Cookies[path].Value = hieFilter.Value;
            }
            else
            {
                context.Request.Cookies.Add(hieFilter);
            }
        }

        public static void DeleteHieRoot(GridViewSettings s, int root)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];
            DeleteHieRoot(ct, ac, root);
        }

        public static void DeleteHieRoot(string controller, string action, int root)
        {
            string path = "HieFiltersSettings-" + controller + "-" + action;

            HttpContext context = HttpContext.Current;
            string current_val = context.Request.Cookies.AllKeys.Contains(path) ? context.Request.Cookies[path].Value : "";
            List<string> list = current_val.Split(',').Where(w => w.Length > 0).ToList();
            list.Remove(root.ToString());
            HttpCookie hieFilter = new HttpCookie(path);
            hieFilter.Expires = DateTime.Now.AddYears(50);
            hieFilter.Value = String.Join(",", list);
            context.Response.Cookies.Add(hieFilter);
            if (context.Request.Cookies.AllKeys.Contains(path))
            {
                context.Request.Cookies[path].Value = hieFilter.Value;
            }
            else
            {
                context.Request.Cookies.Add(hieFilter);
            }
        }

        public static int[] GetHieRoots(GridViewSettings s)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];

            return GetHieRoots(ct, ac);
        }

        public static int[] GetHieRoots(string controller, string action)
        {
            string path = "HieFiltersSettings-" + controller + "-" + action;

            HttpContext context = HttpContext.Current;
            string current_val = context.Request.Cookies.AllKeys.Contains(path) ? context.Request.Cookies[path].Value : ",0";
            return current_val.Split(',').Where(w => w.Length > 0).Select(str => int.Parse(str)).ToArray<int>();
        }

        /*public static void SetGridHieRoot(GridViewSettings s, int root)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];
            SetGridHieRoot(ct, ac, root);            
        }

        public static void SetGridHieRoot(string controller, string action, int root)
        {
            string path = "HieGridRoot-" + controller + "-" + action;
            HttpContext context = HttpContext.Current;
            HttpCookie hieRoot = new HttpCookie(path);
            DateTime now = DateTime.Now;
            hieRoot.Value = root.ToString();
            context.Response.Cookies.Add(hieRoot);
            if (context.Request.Cookies.AllKeys.Contains(path))
            {
                context.Request.Cookies[path].Value = hieRoot.Value;
            }
            else
            {
                context.Request.Cookies.Add(hieRoot);
            }
        }*/

        /*public static int GetGridHieRoot(GridViewSettings s)
        {
            Dictionary<string, string> dic = Utils.CastAnonymous(s.CallbackRouteValues);
            string ct = dic["Controller"];
            string ac = dic["Action"];
            return GetGridHieRoot(ct, ac);
        }

        public static int GetGridHieRoot(string controller, string action)
        {
            string path = "HieGridRoot-" + controller + "-" + action;
            HttpContext context = HttpContext.Current;
            string current_val = context.Request.Cookies.AllKeys.Contains(path) ? context.Request.Cookies[path].Value : "0";
            return int.Parse(current_val);
        }*/
        #endregion

        public static void SetUserTheme(string ThemeName)
        {
            if(string.IsNullOrEmpty(ThemeName))
                ThemeName = "Office2010Silver";

            var prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
            if (prop != null)
            {
                SystemParameterUser user_prop = null;
                try { user_prop = prop.GetUserParams(true).First(w => w.Name == HttpContext.Current.User.Identity.Name); } catch { }

                if (user_prop == null || user_prop.Id==0)
                {
                    user_prop = new SystemParameterUser(prop, WADataProvider.CurrentUser.Id) 
                    { 
                        Workarea = WADataProvider.WA, 
                        StateId = State.STATEACTIVE,
                        IsNew = true,
                        DatabaseId = WADataProvider.WA.MyBranche.Id
                    };
                }

                try
                {
                    user_prop.ValueString = ThemeName;
                    user_prop.Save();
                }
                catch (Exception e)
                {
                    string s = e.Message;
                }
            }
        }

        public static void InjectIE7CompatModeMeta(Control parent)
        {
            InjectIECompatModeMeta(parent, 7);
        }

        public static void InjectIECompatModeMeta(Control parent, int compatibilityVersion)
        {
            if (!RenderUtils.Browser.IsIE || RenderUtils.Browser.Version < compatibilityVersion + 1)
                return;
            ASPxWebControl.SetIECompatibilityMode(compatibilityVersion, parent);
        }

        public static bool IsIE6()
        {
            return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 7;
        }

        public static bool IsIE9()
        {
            return RenderUtils.Browser.IsIE && RenderUtils.Browser.Version > 8;
        }
        public static bool IsMozzila()
        {
            return RenderUtils.Browser.IsFirefox && RenderUtils.Browser.Version > 8;
        }
        /// <summary>
        /// Является ли текущее приложение отладочной версией.
        /// </summary>
        /// <remarks>Используется для определения подключения файлов скриптов в полном виде или в сжатом виде, 
        /// также используется для вывода дополнительной отладочной информации.</remarks>
        public static bool IsDebugMode
        {
            get
            {
                #if(DEBUG)
                    return true;
                #endif
                return false;
            }
            
        }
        /// <summary>
        /// Имеет ли приложение доступ к интернет ресурсам
        /// </summary>
        /// <remarks>Используется для определения внешних ссылок на скрипты JQUERY. 
        /// Для отладочной версии всегда имеет значение <c>false</c></remarks>
        public static bool IsRealWebAccessMode
        {
            get
            {
#if(DEBUG)
                    return false;
#endif
#if(REALWEBAPP)
                    return true;
#endif
                return false;
            }

        }
        
    }

    #region Темы...
    public class ThemeModelBase
    {
        string _name;
        string _title;

        [XmlAttribute]
        public string Name
        {
            get
            {
                if (_name == null)
                    return "";
                return _name;
            }
            set { _name = value; }
        }
        [XmlAttribute]
        public string Title
        {
            get
            {
                if (_title == null)
                    return "";
                return _title;
            }
            set { _title = value; }
        }
    }
    public class ThemeGroupModel : ThemeModelBase
    {
        List<ThemeModel> _themes = new List<ThemeModel>();

        [XmlElement(ElementName = "Theme")]
        public List<ThemeModel> Themes
        {
            get { return _themes; }
        }
    }
    [XmlRoot("Themes")]
    public class ThemesModel
    {
        static ThemesModel _current;
        static readonly object _currentLock = new object();

        public static ThemesModel Current
        {
            get
            {
                lock (_currentLock)
                {
                    if (_current == null)
                    {
                        using (Stream stream = File.OpenRead(HttpContext.Current.Server.MapPath("~/App_Data/Themes.xml")))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ThemesModel));
                            _current = (ThemesModel)serializer.Deserialize(stream);
                        }
                    }
                    return _current;
                }
            }
        }

        List<ThemeGroupModel> _groups = new List<ThemeGroupModel>();

        [XmlElement("ThemeGroup")]
        public List<ThemeGroupModel> Groups
        {
            get { return _groups; }
        }
    }
    public class ThemeModel : ThemeModelBase
    {
        string _spriteCssClass;

        [XmlAttribute]
        public string SpriteCssClass
        {
            get
            {
                if (_spriteCssClass == null)
                    return "";
                return _spriteCssClass;
            }
            set { _spriteCssClass = value; }
        }
    }

    [XmlRoot("Pagers")]
    public class GridsPagersList
    {
        [XmlArray("PagersList")]
        public List<Pager> pagers = new List<Pager>();

        public int GetPages(string code)
        {
            Pager pager = null;
            try { pager = pagers.First(w => w.Code == code); }
            catch { }
            if (pager != null)
            {
                return pager.Pages;
            }
            return 15;
        }

        public void SetPages(string code, int pages)
        {
            Pager pager = null;
            try { pager = pagers.First(w => w.Code == code); }
            catch { }
            if (pager != null)
            {
                pager.Pages = pages;
            }
            else
            {
                pagers.Add(new Pager { Code = code, Pages = pages });
            }
        }

        public void LoadFromXML(string xml)
        {
            StringReader sr = new StringReader(xml);
            using (XmlReader reader = XmlReader.Create(sr))
            {
                XmlSerializer Scheme = new XmlSerializer(typeof(GridsPagersList));
                GridsPagersList list = (GridsPagersList)Scheme.Deserialize(reader);
                pagers = list.pagers;
            }
        }

        public string SaveToXML()
        {
            StringBuilder stringXml = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(stringXml, new XmlWriterSettings() { Indent = true }))
            {
                XmlSerializer Childrens = new XmlSerializer(typeof(GridsPagersList));
                Childrens.Serialize(writer, this);
            }
            return stringXml.ToString();
        }
    }
    public class Pager
    {
        public string Code;
        public int Pages;
    } 
    #endregion
}
