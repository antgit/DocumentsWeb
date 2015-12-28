using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Models
{
    public class AgentBankAccountModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Наименование обязательно")]
        public string Name { get; set; }
        public string Code { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public int AgentId { get; set; }
        public int KindIdABA { get; set; }


        public static AgentBankAccountModel ConvertToModel(AgentBankAccount value)
        {
            return new AgentBankAccountModel
                       {
                           Id = value.Id,
                           Name = value.Name,
                           Code = value.Code,
                           BankId = value.BankId,
                           BankName = value.Bank.Name,
                           CurrencyId = value.CurrencyId,
                           CurrencyName = value.Currency.Name,
                           AgentId = value.AgentId,
                           KindIdABA = value.KindId
                       };
        }

        public AgentBankAccount ToObject()
        {
            AgentBankAccount account = new AgentBankAccount {Workarea = WADataProvider.WA};
            account.Load(Id);

            if(Id==0)
            {
                account.IsNew = true;
                account.AgentId = AgentId;
                account.StateId = State.STATEACTIVE;
            }

            account.Name = Name;
            account.Code = Code;
            account.BankId = BankId;
            account.CurrencyId = CurrencyId;
            account.KindId = KindIdABA;

            return account;
        }
    }
}