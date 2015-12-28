using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class ReportRouteSkippingTasksHeaderModel
    {
        /// <summary>Дата отчета</summary>
        public DateTime Date { get; set; }
        /// <summary>Время начала маршрута</summary>
        public TimeSpan StartTime { get; set; }
        /// <summary>Время конца маршрута</summary>
        public TimeSpan EndTime { get; set; }
        /// <summary>Длинна маршрута</summary>
        public string RouteLength { get; set; }
        /// <summary>Имя торгового представителя</summary>
        public string TPName { get; set; }
    }
}