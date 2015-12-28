using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Prices.Models
{
    public class PriceNameModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Тип вида цены
        /// </summary>
        public int KindId { get; set; }
        public string KindName { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Состояние
        /// </summary>
        public int StateId { get; set; }
        /// <summary>
        /// Набор флагов
        /// </summary>
        public int FlagsValue { get; set; }
        public bool IsSystem { get { return (FlagValue.FLAGSYSTEM & FlagsValue) == FlagValue.FLAGSYSTEM; } }
        public bool IsReadOnly { get { return (FlagValue.FLAGREADONLY & FlagsValue) == FlagValue.FLAGREADONLY; } }

        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        /// <summary>
        /// Приводит объект к модели
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        public static PriceNameModel ToModel(PriceName obj)
        {
            PriceNameModel model = new PriceNameModel
            {
                Id = obj.Id,
                KindId = obj.KindId,
                KindName = obj.KindName,
                Name = obj.Name,
                CurrencyId = obj.CurrencyId,
                CurrencyName = obj.Currency.Name,
                StateId = obj.StateId,
                Memo = obj.Memo,
                FlagsValue = obj.FlagsValue,
                MyCompanyId = obj.MyCompanyId,
                MyCompanyName = obj.MyCompanyId == 0 ? string.Empty : obj.MyCompany.Name
            };

            return model;
        }

        public static PriceNameModel ToModel(int Id)
        {
            PriceName obj = (PriceName)WADataProvider.WA.Cashe.GetCasheItem(9, Id);
            return ToModel(obj);
        }
        public static PriceNameModel GetObject(int id)
        {
            PriceName obj = WADataProvider.WA.Cashe.GetCasheData<PriceName>().Item(id);
            return ToModel(obj);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public PriceNameModel()
        {
            StateId = State.STATEACTIVE;
            KindId = PriceName.KINDID_PRICENAME;
        }

        /// <summary>
        /// Привести модель к объекту
        /// </summary>
        /// <returns></returns>
        public PriceName ToObject()
        {
            PriceName obj = new PriceName { Workarea = WADataProvider.WA };
            obj.Load(Id);

            obj.Name = Name;
            obj.KindId = KindId;
            obj.StateId = StateId;
            obj.CurrencyId = CurrencyId;
            obj.Memo = Memo;

            return obj;
        }

        public void Save()
        {
            PriceName obj = this.ToObject();
            obj.Save();

            if (Id == 0)
            {
                Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>((
                    KindId == PriceName.KINDID_COMPETITOR ? Hierarchy.SYSTEM_PRICENAME_COMPETITORPRICES : (
                    KindId == PriceName.KINDID_PROVIDER ? Hierarchy.SYSTEM_PRICENAME_INPRICES : Hierarchy.SYSTEM_PRICENAME_OUTPRICES
                    )
                ));
                hierarchy.ContentAdd(obj, true);
            }
        }
        public static object GetPriceNameByValue(object value)
        {
            return value != null && (int)value != 0 ? new List<PriceNameModel> { ToModel((int)value) }.Take(1).ToList() : null;
        }
    }
}