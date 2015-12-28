using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Models
{
    public class AccountConfigModel
    {
        #region �����
        /// <summary>
        /// ���� �� ���������
        /// </summary>
        public string ThemeDefault { get; set; }


        #endregion

        /// <summary>
        /// �������� ����������
        /// </summary>
        public void Load()
        {
            var prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
            if (prop != null) ThemeDefault = prop.ValueString;
            
        }

        /// <summary>
        /// ���������� ����������
        /// </summary>
        public void Save()
        {
            var prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
            if (prop != null) { prop.ValueString = ThemeDefault; prop.Save(); }
        }
    }
}