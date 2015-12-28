using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Marketings.Models
{
    public class ReportSKUByMilkDetailModel
    {
        /// <summary>Номер по порядку</summary>
        public int Npp { get; set; }
        /// <summary>Филиал</summary>
        public string Depatment { get; set; }
        /// <summary>Юридическое наименование ТРТ</summary>
        public string TRTUrName { get; set; }
        /// <summary>Фактическое наименование ТРТ</summary>
        public string TRTFactName { get; set; }
        /// <summary>Адрес ТРТ</summary>
        public string TRTAddress { get; set; }
        /// <summary>Расположение ТРТ</summary>
        public string TRTOutletLocation { get; set; }
        /// <summary>Торговая марка</summary>
        public string TM { get; set; }
        /// <summary>SKU по молоку</summary>
        public decimal MilkSKU { get; set; }
        /// <summary>SKU по кефиру</summary>
        public decimal KefirSKU { get; set; }
        /// <summary>SKU по сметане</summary>
        public decimal SmetanaSKU { get; set; }
        /// <summary>SKU по маслу</summary>
        public decimal MasloSKU { get; set; }
    }
}