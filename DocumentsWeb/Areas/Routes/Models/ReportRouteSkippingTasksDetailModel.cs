using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class ReportRouteSkippingTasksDetailModel
    {
        /// <summary>Номер по порядку</summary>
        public int Npp { get; set; }
        /// <summary>Плановое время посещения</summary>
        public TimeSpan PlanTime { get; set; }
        /// <summary>Фактическое время посещения</summary>
        public TimeSpan FactTime { get; set; }
        /// <summary>Продолжительность посещения</summary>
        public TimeSpan VisitLength { get; set; }
        /// <summary>Наименование клиента</summary>
        public string Name { get; set; }
        /// <summary>Город</summary>
        public string Town { get; set; }
        /// <summary>Адрес</summary>
        public string Address { get; set; }
    }
}