using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Code
{
    /// <summary>
    /// Настроки домашней страницы раздела
    /// </summary>
    [Serializable]
    public class HomePageLayoutSettings
    {
        public HomePageLayoutSettings()
        {
            IsNew = true;
            SectionCheckBoxes = new List<DictionaryItem>();
        }
        /// <summary>
        /// Список строковых идентификаторов, содержащих имена выбранных чекбоксов
        /// </summary>
        public List<DictionaryItem> SectionCheckBoxes { get; set; }

        /// <summary>
        /// Являются ли настройки только что созданными и не сохраненными в БД
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Сохранение настройки в БД
        /// </summary>
        /// <param name="userId">Ид пользователя</param>
        /// <param name="controllerName">Ключ</param>
        public void SaveLayoutToDatabase(int userId, string controllerName)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = LayoutHelper.FindStorage(userId, code) ?? new XmlStorage
            {
                Workarea = WADataProvider.WA,
                Name = "Layout for " + controllerName,
                Code = code,
                KindId = XmlStorage.KINDID_LAYOUTCONTROLDATA,
                UserId = userId
            };

            storage.XmlData = Save();
            storage.Save();
        }

        /// <summary>
        /// Загрузка настройки из БД
        /// </summary>
        /// <param name="userId">Ид пользователя</param>
        /// <param name="controllerName">Ключ</param>
        public static HomePageLayoutSettings LoadLayoutFromDatabase(int userId, string controllerName)
        {
            string code = controllerName.ToUpper() + "_LAYOUT";
            XmlStorage storage = LayoutHelper.FindStorage(userId, code);
            if (storage == null)
                return new HomePageLayoutSettings();
            return Load(storage.XmlData);
        }

        /// <summary>
        /// Создание настроек из запроса
        /// </summary>
        /// <param name="p">Параметры запроса</param>
        /// <returns>Настройки</returns>
        public static HomePageLayoutSettings CreateFromRequest(NameValueCollection p)
        {
            HomePageLayoutSettings settings = new HomePageLayoutSettings();
            foreach (var key in p.AllKeys)
            {
                if (!key.StartsWith("chk"))
                    continue;
                settings.SectionCheckBoxes.Add(new DictionaryItem {Key = key, Value = p[key] == "true"});
            }
            return settings;
        }

        /// <summary>
        /// Определение того, были отмеченные чекбоксы до заданного
        /// </summary>
        /// <param name="code">Код заданного чекбокса</param>
        /// <returns></returns>
        public bool FindBefore(string code)
        {
            if (IsNew)
                return true;

            bool found = false;
            for(int i=SectionCheckBoxes.FindIndex(s=>s.Key==code)-1; i>=0; i--)
            {
                if(SectionCheckBoxes[i].Value)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Определение того, есть ли отмеченные чекбоксы после заданного
        /// </summary>
        /// <param name="code">Код заданного чекбокса</param>
        /// <returns></returns>
        public bool FindAfter(string code)
        {
            if (IsNew)
                return true;

            bool found = false;
            for (int i = SectionCheckBoxes.FindIndex(s => s.Key == code) + 1; i <SectionCheckBoxes.Count; i++)
            {
                if (SectionCheckBoxes[i].Value)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// Определение отметки чекбокса с заданным кодом
        /// </summary>
        /// <param name="code">Код чекбокса</param>
        /// <returns></returns>
        public bool Checked(string code)
        {
            DictionaryItem item = SectionCheckBoxes.FirstOrDefault(s => s.Key == code);
            return item != null && item.Value;
        }

        #region Сериализация
        public string Save()
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HomePageLayoutSettings));
                serializer.Serialize(writer, this);
                writer.Close();
                return writer.ToString();
            }
        }

        static public HomePageLayoutSettings Load(string xmlData)
        {
            HomePageLayoutSettings ret;
            using (StringReader reader = new StringReader(xmlData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HomePageLayoutSettings));
                ret = serializer.Deserialize(reader) as HomePageLayoutSettings;
                reader.Close();
            }
            if (ret != null)
                ret.IsNew = false;
            return ret;
        }
        #endregion
    }

    [Serializable]
    public class DictionaryItem
    {
        public string Key { get; set; }
        public bool Value { get; set; }
    }
}