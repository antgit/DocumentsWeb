using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;

namespace DocumentsWeb.Models
{
    public static class HomeData
    {
        private static Dictionary<string, string> values = new Dictionary<string, string>();
        static HomeData()
        {
            values.Add(WebModuleNames.WEB_DOCMKTG, "Маркетинг");
            values.Add(WebModuleNames.WEB_DOCPRICE, "Ценовая политика");
            values.Add(WebModuleNames.WEB_DOCSALE, "Управление продажами");
            values.Add(WebModuleNames.WEB_DOCALL, "Все документы");
            values.Add(WebModuleNames.WEB_DOCTAX, "Налоговые документы");
            values.Add(WebModuleNames.WEB_DOCFINANCE, "Управление финансами");
            values.Add(WebModuleNames.WEB_DOCDOGOVOR, "Договора");
            values.Add(WebModuleNames.WEB_DOCSERVICE, "Услуги");
            values.Add(WebModuleNames.WEB_USERHOME, "Личная страница");
            values.Add(WebModuleNames.WEB_TASKS, "Задачи");
        }
        public static List<string> GetWidgets()
        {
            List<string> coll = new List<string>() { 
                WebModuleNames.WEB_DOCSALE, 
                WebModuleNames.WEB_DOCFINANCE, 
                WebModuleNames.WEB_DOCSERVICE, 
                WebModuleNames.WEB_DOCTAX, 
                WebModuleNames.WEB_DOCPRICE, 
                WebModuleNames.WEB_DOCDOGOVOR,
                WebModuleNames.WEB_DOCALL, 
                WebModuleNames.WEB_DOCMKTG,
                WebModuleNames.WEB_TASKS
                 };

            return coll.Where(f => WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, f)).ToList();
        }

        public static string GetWidgetsHeader(string key)
        {
            if (values.ContainsKey(key))
                return values[key];
            return "Виджет";
        }
    }
}