using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace DocumentsWeb.Code
{
    [Serializable]
    public class LayoutSettingItem
    {
        public LayoutSettingItem()
        {
            GridSetting = string.Empty;
            NavSetting = string.Empty;
            ShowGroupPanel = true;
            ShowFilterPanel = true;
            ShowLinkInName = true;
            //ShowCommandColumn = true;
        }

        public string GridSetting { get; set; }
        public string NavSetting { get; set; }
        public bool ShowGroupPanel { get; set; }
        public bool ShowFilterPanel { get; set; }
        public bool ShowLinkInName { get; set; }
        
        [OptionalField]
        private bool _showPreview;
        public bool ShowPreview
        {
            get { return _showPreview; }
            set { _showPreview = value; }
        }

        //public bool ShowCommandColumn { get; set; }
    }

    [Serializable]
    public class LayoutSettings
    {
        public const int Count = 3;

        public LayoutSettings()
        {
            Settings = new LayoutSettingItem[Count];
            for (int i = 0; i < Count; i++)
                Settings[i] = new LayoutSettingItem();
        }

        public LayoutSettingItem[] Settings { get; set; }

        public int SelectedSettingIndex { get; set; }

        public LayoutSettingItem Current
        {
            get { return Settings[SelectedSettingIndex]; }
        }

        #region Сериализация
        public string Save()
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LayoutSettings));
                serializer.Serialize(writer, this);
                writer.Close();
                return writer.ToString();
            }
        }

        static public LayoutSettings Load(string xmlData)
        {
            LayoutSettings ret;
            using (StringReader reader = new StringReader(xmlData))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LayoutSettings));
                ret = serializer.Deserialize(reader) as LayoutSettings;
                reader.Close();
            }
            return ret;
        }
        #endregion
    }
}