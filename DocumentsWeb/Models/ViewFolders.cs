using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;

namespace DocumentsWeb.Models
{
    public class ViewFolders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HierarchyName { get; set; }
        public string Memo { get; set; }
        public Folder Folder { get; set; }
        public int FolderId { get { return Folder.Id; } }
        public Hierarchy Hierarchy { get; set; }
        public int Kind { get; set; }
        /// <summary>
        /// Код формы документа, используется только для папок документов для поиска View отображения документа
        /// </summary>
        [Obsolete("При создании документа используется только поле FolderId. Данное поле можно удалить")]
        public string FormCode { get; set; }
        /// <summary>
        /// Идентификатор шаблона документа, используется только для папок документов для поиска шаблона отображения документа
        /// </summary>
        public int DocumentTemplateId { get; set; }
    }
}