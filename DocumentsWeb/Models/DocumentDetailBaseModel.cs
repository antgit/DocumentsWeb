using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Models
{
    public class DocumentDetailBaseModel
    {
        /// <summary>Идентификатор</summary>
        public int Id { get; set; }
        /// <summary>Глобальный идентификатор</summary>
        public Guid Guid { get; set; }
        /// <summary>Состояние</summary>
        public int StateId { get; set; }
        /// <summary>Идентификатор родителя</summary>
        public int OwnerId { get; set; }
        /// <summary>Идентификатор строки</summary>
        public string RowId { get; set; }

        public DocumentDetailBaseModel()
        {
            RowId = Guid.NewGuid().ToString();
        }
    }
}