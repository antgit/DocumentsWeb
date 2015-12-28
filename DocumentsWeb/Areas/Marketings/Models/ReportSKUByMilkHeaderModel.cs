using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Marketings.Models
{
    public class ReportSKUByMilkHeaderModel
    {
        /// <summary>Дата отчета</summary>
        public DateTime Date { get; set; }
        /// <summary>Филиал</summary>
        public string Depatment { get; set; }
    }
}