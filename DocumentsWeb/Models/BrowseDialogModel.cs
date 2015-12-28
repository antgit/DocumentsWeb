using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Models
{
    public class BrowseDialogModel
    {
        /// <summary>
        /// Имя контрола
        /// </summary>
        public string Name;
        /// <summary>
        /// Корневая иерархия для построения дерева
        /// </summary>
        public string RootHierarchy;
        /// <summary>
        /// Имя функции, в которую передаются идентификатор и имя выбранного объекта
        /// </summary>
        public string CallbackFunction;
        /// <summary>
        /// Заголовок окна
        /// </summary>
        public string Caption;
    }
}