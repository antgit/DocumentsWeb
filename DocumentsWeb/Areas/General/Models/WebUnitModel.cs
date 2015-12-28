using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Models
{
    public class WebUnitModel : UnitModel, IModelData, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //[StringLength(255, ErrorMessage = "Не более 255 символов")]
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование обязательно", new[] { GlobalPropertyNames.Name });
            }

            //[StringLength(255, ErrorMessage = "Не более 100 символов")]
            if (string.IsNullOrEmpty(Code))
            {
                yield return new ValidationResult("Код обязателен", new[] { GlobalPropertyNames.Code });
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
        public WebUnitModel()
        {
            StateId = State.STATEACTIVE;
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject(WADataProvider.WA).FlagsValueString();
        }
        public string InHierarchies { get; set; }

        public Unit ToObject(Workarea workarea)
        {
            Unit unit = new Unit { Workarea = workarea };
            if(Id!=0)
                unit.Load(Id);
            if (unit.Id == 0)
            {
                unit.IsNew = true;
                unit.KindId = Unit.KINDID_UNIT;
            }
            unit.StateId = StateId == 0 ? State.STATEACTIVE : StateId;
            unit.Id = Id;
            unit.Name = Name;
            unit.Code = Code;
            unit.Memo = Memo;
            unit.CodeInternational = CodeInternational;
            return unit;
        }

        public static void ToTrash(int id)
        {
            Unit obj = WADataProvider.WA.Cashe.GetCasheData<Unit>().Item(id);
            obj.Remove();
            //Hierarchy hRoot = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //DocumentsWeb.Models.WADataProvider.CacheUnitModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id)
        {
            if (id != 0)
            {
                Unit obj = WADataProvider.WA.Cashe.GetCasheData<Unit>().Item(id);
                Unit newObj = Unit.CreateCopy(obj);
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
            Unit obj = WADataProvider.WA.Cashe.GetCasheData<Unit>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id)
        {
            Unit obj = WADataProvider.WA.Cashe.GetCasheData<Unit>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Unit>(id);

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
            Unit obj = WADataProvider.WA.Cashe.GetCasheData<Unit>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        /// <summary>
        /// Список единиц измерения
        /// </summary>
        public static IEnumerable GetCollection()
        {
            List<Unit> coll = WADataProvider.WA.GetCollection<Unit>();
            return coll.Select(ConvertToModel).ToList();
        }
        
        /// <summary>
        /// Единиц измерения по идентификатору
        /// </summary>
        public static WebUnitModel GetObject(int id)
        {
            Unit obj = WADataProvider.WA.GetObject<Unit>(id);
            return ConvertToModel(obj);
        }

        /// <summary>
        /// Преобразование единиц измерения
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebUnitModel ConvertToModel(Unit value)
        {
            WebUnitModel obj = new WebUnitModel();
            obj.GetData(value);
            
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;
            return obj;
        }

        public static bool CanSave(int id)
        {
            Unit u = new Unit { Workarea = WADataProvider.WA };
            u.Load(id);
            return !(u.IsSystem | u.IsReadOnly);
        }

        public static List<WebUnitModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Unit> coll = new List<Unit>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Unit>(true, refresh));
            }
            return coll.Select(ConvertToModel).Distinct(new UnitsComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<WebUnitModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Unit>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            List<Unit> coll = new List<Unit>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Unit>());
            }
            return coll.Select(ConvertToModel).Distinct(new UnitsComparer()).OrderBy(o => o.Name).ToList();
        }
    }
    public static class UnitData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (fieldName == GlobalPropertyNames.Name)
                return "Наименование единицы измерения является обязательным! Старайтесь задавать значение максимально кратким и понятным. Максимальная длина - 255 символов.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "Наименование единицы измерения используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание единицы измерения используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем. Максимальная длина не ограничена.";
            
            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код единицы измерения используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Дополнительный код поиска единицы измерения: при быстром вводе в соотвествующие поля - помимо наименования анализируется данное свойство. По умолчанию код поиска формируется путем транслитерации наименования и атрибута \"Код\".";

            if (fieldName == GlobalPropertyNames.DateModified)
                return "Дата последнего изменения данных";
            if (fieldName == GlobalPropertyNames.Id)
                return "Уникальный числовой идентификатор";
            if (fieldName == GlobalPropertyNames.UserName)
                return "Имя пользователя создавшего или изменившего данные в последний раз";
            if (fieldName == GlobalPropertyNames.MyCompanyId)
                return "Идентификатор компании-владельца данных";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "Наименование компании-владельца данных";
            return string.Empty;
        }

        public static string GetNullText(string controller, string fieldName)
        {
            switch (fieldName)
            {
                case GlobalPropertyNames.Name:
                    return "Введите наименование единицы измерения...";
                case GlobalPropertyNames.NameFull:
                    return "Введите печатное наименование единицы измерения...";
                case GlobalPropertyNames.Memo:
                    return "Введите примечание или описание единицы измерения...";
                case GlobalPropertyNames.Code:
                    return "Введите код единицы измерения...";
                case GlobalPropertyNames.CodeFind:
                    return "Введите код поиска единицы измерения...";
            }
            return string.Empty;
        }
    }
    internal class UnitsComparer : IEqualityComparer<WebUnitModel>
    {
        public bool Equals(WebUnitModel x, WebUnitModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WebUnitModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}