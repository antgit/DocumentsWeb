using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Models
{
    public class SysConfigModel
    {
        #region Отчеты
        /// <summary>
        /// Сервер отчетов
        /// </summary>
        public string ReportsServer { get; set; }

        /// <summary>
        /// Папка на сервере отчетов
        /// </summary>
        public string ReportsServerFolder { get; set; }

        /// <summary>
        /// Серверное расположение отчетов
        /// </summary>
        public string ReportsLocation { get; set; }

        /// <summary>
        /// Использовать Flash для печатных форм в Web интерфейсе
        /// </summary>
        public bool UseFlashForWebForms { get; set; }

        /// <summary>
        /// Использовать Flash для отчетов
        /// </summary>
        public bool UseFlashForReports { get; set; }
        #endregion

        #region Общие

        /// <summary>
        /// Строка "О комнпании"
        /// </summary>
        public string FooterBanerText { get; set; }
        /// <summary>
        /// Url навигации для ссылки "О компании"
        /// </summary>
        public string FooterNavigateUrl { get; set; }

        /// <summary>
        /// Тема по умолчанию
        /// </summary>
        public string ThemeDefault { get; set; }

        /// <summary>
        /// Разрешить регистрировать комнпании
        /// </summary>
        public bool SolveRegistryCompany { get; set; }

        #endregion

        #region SMTP

        /// <summary>
        /// Сервер рассылки почты
        /// </summary>
        public string SMTPServer { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string SMTPUser { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string SMTPPassword { get; set; }

        /// <summary>
        /// Электронный адрес администратора
        /// </summary>
        public string SMTPAdminEmail { get; set; }

        #endregion

        /// <summary>
        /// Загрузка параметров
        /// </summary>
        public void Load()
        {
            var prop = WADataProvider.GetSysProperty("REPORTSERVER");
            if (prop != null) ReportsServer = prop.ValueString;
            prop = WADataProvider.GetSysProperty("REPORTSERVERFOLDER");
            if (prop != null) ReportsServerFolder = prop.ValueString;
            prop = WADataProvider.GetSysProperty("REPORTSERVER_WIZARD");
            prop = WADataProvider.GetSysProperty("REPORTSTIMULSOFTPATH");
            if (prop != null) ReportsLocation = prop.ValueString;
            prop = WADataProvider.GetSysProperty("WEB_PRINTFORMFLASH");
            if (prop != null) UseFlashForWebForms = prop.ValueInt == 1 ? true : false;
            prop = WADataProvider.GetSysProperty("REPORTSTIMULSOFTFLASH");
            if (prop != null) UseFlashForReports = prop.ValueInt == 1 ? true : false;

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
            if (prop != null) ThemeDefault = prop.ValueString;
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBBANERTEXT");
            if (prop != null) FooterBanerText = prop.ValueString;

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBBANERLINK");
            if (prop != null) FooterNavigateUrl = prop.ValueString;
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBALLOWCOMPANYREGISTER");
            if (prop != null) SolveRegistryCompany = prop.ValueInt == 1 ? true : false;

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPSERVER");
            if (prop != null) SMTPServer = prop.ValueString;
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPUSER");
            if (prop != null) SMTPUser = prop.ValueString;
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPPASS");
            if (prop != null) SMTPPassword = prop.ValueString;
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_ADMINEMAIL");
            if (prop != null) SMTPAdminEmail = prop.ValueString;
        }

        /// <summary>
        /// Сохранение параметров
        /// </summary>
        public void Save()
        {
            var prop = WADataProvider.GetSysProperty("REPORTSERVER");
            if (prop != null) { prop.ValueString = ReportsServer; prop.Save(); }
            prop = WADataProvider.GetSysProperty("REPORTSERVERFOLDER");
            if (prop != null) { prop.ValueString = ReportsServerFolder; prop.Save(); }
            prop = WADataProvider.GetSysProperty("REPORTSTIMULSOFTPATH");
            if (prop != null) { prop.ValueString = ReportsLocation; prop.Save(); }
            prop = WADataProvider.GetSysProperty("WEB_PRINTFORMFLASH");
            if (prop != null) { prop.ValueInt = UseFlashForWebForms ? 1 : 0; prop.Save(); }
            prop = WADataProvider.GetSysProperty("REPORTSTIMULSOFTFLASH");
            if (prop != null) { prop.ValueInt = UseFlashForReports ? 1 : 0; prop.Save(); }

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBUISTYLE");
            if (prop != null) { prop.ValueString = ThemeDefault; prop.Save(); }
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBBANERTEXT");
            if (prop != null) { prop.ValueString = FooterBanerText; prop.Save(); }

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_WEBALLOWCOMPANYREGISTER");
            if (prop != null) { prop.ValueInt = SolveRegistryCompany ? 1 : 0; prop.Save(); }

            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPSERVER");
            if (prop != null) { prop.ValueString = SMTPServer; prop.Save(); }
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPUSER");
            if (prop != null) { prop.ValueString = SMTPUser; prop.Save(); }
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_SMTPPASS");
            if (prop != null) { prop.ValueString = SMTPPassword; prop.Save(); }
            prop = WADataProvider.GetSysProperty("SYSTEMPARAMETER_ADMINEMAIL");
            if (prop != null) { prop.ValueString = SMTPAdminEmail; prop.Save(); }
        }
    }
}