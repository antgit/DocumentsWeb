using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using System.Linq;

namespace DocumentsWeb.Areas.General.Models
{
    /// <summary>
    /// Модель "Валюты"
    /// </summary>
    public class WebCurrencyModel : CurrencyModel, IModelData, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //[StringLength(255, ErrorMessage = "Не более 255 символов")]
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование обязательно", new[] { GlobalPropertyNames.Name });
            }

            //[StringLength(255, ErrorMessage = "Не более 255 символов")]
            if (string.IsNullOrEmpty(Code))
            {
                yield return new ValidationResult("Код обязателен", new[] { GlobalPropertyNames.Code });
            }
        
            if (IntCode==0)
            {
                yield return new ValidationResult("Цифровой код обязателен", new[] { GlobalPropertyNames.IntCode });
            }

            Type targetType = this.GetType();
            string fulltypename = targetType.FullName;
            IEnumerable<Ruleset> collRules = WADataProvider.WA.GetCollection<Ruleset>().Where(s => s.ActivityName == fulltypename && s.StateId == 1 && s.KindValue == Ruleset.KINDVALUE_WEBRULESET);
            //if (collRules == null) return true;

            foreach (Ruleset rule in collRules)
            {
                RuleSet ruleSetToValidate = rule.DeserializeRuleSet();
                if (ruleSetToValidate != null)
                {
                    RuleEngine engine = new RuleEngine(ruleSetToValidate, targetType);
                    engine.Execute(this);
                    if (this.Errors.Count > 0)
                    {
                        foreach (string k in this.Errors.Keys)
                        {
                            yield return new ValidationResult(Errors[k], new[] { k });
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public WebCurrencyModel()
        {
            StateId = State.STATEACTIVE;
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }
        
        public string InHierarchies { get; set; }

        /// <summary>
        /// Преобразование валюты
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebCurrencyModel ConvertToModel(Currency value)
        {
            WebCurrencyModel obj = new WebCurrencyModel();
            obj.GetData(value);
            
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;
            return obj;
        }

        /// <summary>
        /// Преобразование модели в объект
        /// </summary>
        /// <returns></returns>
        public Currency ToObject()
        {
            Currency obj = new Currency { Workarea = WADataProvider.WA };
            if (Id != 0)
                obj.Load(Id);
            if (obj.Id == 0)
                obj.IsNew = true;
            obj.Id = Id;
            obj.Name = Name;
            obj.Code = Code;
            obj.Memo = Memo;
            obj.IntCode = IntCode;
            obj.StateId = StateId == 0 ? State.STATEACTIVE : StateId;
            obj.KindId = Currency.KINDID_CURRENCY;
            return obj;
        }

        public static void ToTrash(int id)
        {
            Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
            obj.Remove();
            //Hierarchy hRoot = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //DocumentsWeb.Models.WADataProvider.CacheUnitModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id)
        {
            if (id != 0)
            {
                Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
                Currency newObj = Currency.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }
                //Hierarchy hRoot = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                //DocumentsWeb.Models.WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id)
        {
            Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id)
        {
            Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
            obj.StateId = State.STATEACTIVE;
            try
            {
                obj.Save();
            }
            catch (DatabaseException dbex)
            {
                if (dbex.Id != 0)
                {
                    ErrorLog err = WADataProvider.WA.GetErrorLog(dbex.Id);
                    if (err != null && err.Message.Contains("конфликт версий"))
                    {
                        obj = WADataProvider.WA.GetObject<Currency>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id)
        {
            Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        /// <summary>
        /// Коллекция объектов
        /// </summary>
        /// <returns></returns>
        public static List<WebCurrencyModel> GetCollection()
        {
            List<Currency> coll = WADataProvider.WA.GetCollection<Currency>();
            List<WebCurrencyModel> collRet = new List<WebCurrencyModel>();
            foreach (Currency obj in coll)
            {
                collRet.Add(ConvertToModel(obj));
            }
            return collRet;
        }

        /// <summary>
        /// Модель по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WebCurrencyModel GetObject(int id)
        {
            Currency obj = WADataProvider.WA.Cashe.GetCasheData<Currency>().Item(id);
            return ConvertToModel(obj);
        }

        public static List<WebCurrencyModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Currency> coll = new List<Currency>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Currency>(true, refresh));
            }
            return coll.Select(ConvertToModel).Distinct(new CurrencyComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<WebCurrencyModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Currency>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            List<Currency> coll = new List<Currency>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Currency>());
            }
            return coll.Select(ConvertToModel).Distinct(new CurrencyComparer()).OrderBy(o => o.Name).ToList();
        }
    }

    internal class CurrencyComparer : IEqualityComparer<WebCurrencyModel>
    {
        public bool Equals(WebCurrencyModel x, WebCurrencyModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WebCurrencyModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
    
}