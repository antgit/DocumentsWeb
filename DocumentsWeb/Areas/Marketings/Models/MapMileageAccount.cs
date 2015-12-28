using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Marketings.Models
{
    /// <summary>
    /// Карта учета пробега шин
    /// </summary>
    public class MapMileageAccount
    {
        /// <summary>
        /// Наименование организации, местоположение
        /// </summary>
        public string FirmName { get; set; }

        /// <summary>
        /// Условное обозначение размера шины
        /// </summary>
        public string SignTire { get; set; }

        /// <summary>
        /// Модель протектора шины
        /// </summary>
        public string ProtectorTire { get; set; }

        /// <summary>
        /// Название изготовителя или предприятия востановившего шину
        /// </summary>
        public string ManufacturerTireName { get; set; }

        /// <summary>
        /// Нормативный документ, по которому изготовлена шина
        /// </summary>
        public string NormativeDocumentTire { get; set; }
    }
}