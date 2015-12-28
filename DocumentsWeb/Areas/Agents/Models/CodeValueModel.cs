using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Models
{
    public class CodeValueModel
    {
        public int Id { get; set; }
        public int CodeNameId { get; set; }
        public string CodeName { get; set; }
        public string CodeCode { get; set; }
        public int ElementId { get; set; }
        public string Value { get; set; }
        public int OrderNo { get; set; }
        public string Memo { get; set; }

        public static CodeValueModel ConverToModel<T>(CodeValue<T> value) where T : class, IBase, new()
        {
            return new CodeValueModel
                       {
                           Id = value.Id,
                           CodeNameId = value.CodeNameId,
                           CodeName = value.CodeName.Name,
                           CodeCode = value.CodeName.Code,
                           ElementId = value.ElementId,
                           Value = value.Value,
                           OrderNo = value.OrderNo,
                           Memo = value.Memo
                       };
        }

        public CodeValue<T> ToObject<T>() where T : class, IBase, new()
        {
            CodeValue<T> codeValue=new CodeValue<T>{Workarea = WADataProvider.WA, ElementId =  ElementId};
            codeValue.Load(Id);

            if(Id==0)
            {
                codeValue.IsNew = true;
            }

            codeValue.UserName = HttpContext.Current.User.Identity.Name;
            codeValue.CodeNameId = CodeNameId;
            codeValue.Value = Value;
            codeValue.OrderNo = OrderNo;
            codeValue.Memo = Memo;

            return codeValue;
        }

        public static List<ComboboxModel> GetCodeNames(int toEntityId)
        {
            return WADataProvider.WA.GetCollection<CodeName>().Where(s=>s.ToEntityId==toEntityId).Select(s => new ComboboxModel(s)).ToList();
        }
    }
}