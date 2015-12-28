using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Models;
using System.Globalization;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class UserConfigController : Controller
    {
        /// <summary>
        /// Установка темы пользователя
        /// </summary>
        /// <param name="ThemeName">Имя темы</param>
        public void SetUserTheme(string ThemeName)
        {
            Utils.SetUserTheme(ThemeName);
        }

        /// <summary>
        /// Сохранение пейджера таблицы для текущего пользователя
        /// </summary>
        /// <param name="Controller">Имя контроллера</param>
        /// <param name="Action">Имя представления</param>
        /// <param name="GridName">Имя таблицы</param>
        /// <param name="PagesCount">Размер страницы</param>
        public void SetGridPager(string Controller, string Action, string GridName, int PagesCount)
        {
            Utils.SetPageSize(Controller, Action, GridName, PagesCount);
        }

        public ActionResult ChangePeriod()
        {
            PeriodModel current = WADataProvider.Period;
            switch (Request.Params["PeriodAction"].ToLower())
            {
                case "yesterday":
                    current.setYesterday();
                    break;
                case "today":
                    current.SetToday();
                    break;
                case "tomorrow":
                    current.SetTomorrow();
                    break;
                case "cur_month":
                    current.SetCurrMonth();
                    break;
                case "cur_year":
                    current.SetCurYear();
                    break;
                case "set_start":
                    current.periodStart = DateTime.ParseExact(Request.Params["Date"], "yyyy.MM.dd", CultureInfo.InvariantCulture);
                    break;
                case "set_end":
                    current.periodEnd = DateTime.ParseExact(Request.Params["Date"], "yyyy.MM.dd", CultureInfo.InvariantCulture);
                    break;
            }
            return PartialView("../Period/PeriodPartial", current);
        }
    }
}
