using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Web.Core;

namespace DocumentsWeb.Models
{
    /// <summary>
    /// Универсальная модель для привязки данных к комбобоксу
    /// </summary>
    public class ComboboxModel
    {
        public ComboboxModel(IBase value)
        {
            Id = value.Id;
            Name = value.Name;
            if(value is ICompanyOwner)
            {
                ICompanyOwner obj = value as ICompanyOwner;
                if (obj.MyCompanyId != 0)
                    MyCompanyName = obj.MyCompany.Name;
            }
        }

        public ComboboxModel(AgentWebView value)
        {
            Id = value.Id;
            Name = value.Name;
            MyCompanyName = value.MyCompanyName;
        }

        public ComboboxModel()
        {

        }
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Наименование компании владельца объекта
        /// </summary>
        public string MyCompanyName { get; set; }
    }
}