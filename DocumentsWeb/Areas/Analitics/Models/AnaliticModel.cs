using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using System.Linq;
using DevExpress.Web.ASPxEditors;
using System;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.Analitics.Models
{
    /// <summary>Модель "Аналитика"</summary>
    public class AnaliticModel : BusinessObjects.Models.AnaliticModel, IModelData, IValidatableObject
    {
        public string InHierarchies { get; set; }

        public AnaliticModel()
        {
            ModelId = Guid.NewGuid().ToString();
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
            StateId = State.STATEACTIVE;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование аналитики обязательно", new[] { GlobalPropertyNames.Name });
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
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject().FlagsValueString();
        }
        /// <summary>Преобразование модели в объект</summary>
        /// <returns></returns>
        public Analitic ToObject()
        {
            Analitic obj = new Analitic();
            if (Id == 0)
                obj = new Analitic { Workarea = WADataProvider.WA };
            else
                obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(Id);
            
            if (obj.Id == 0)
            {
                obj.IsNew = true;
                obj.StateId = State.STATEACTIVE;
                obj.Id = Id;
                obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
                obj.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
            }
            obj.Name = Name;
            obj.KindId = KindId;
            obj.NameFull = NameFull;
            obj.Code = Code;
            obj.CodeFind = CodeFind;
            obj.Memo = Memo;
            return obj;
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Analitic obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Analitic obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
                Analitic newObj = Analitic.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<Analitic>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<Analitic>(newObj, hies, hierarchyCode);
                /*Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }*/
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Analitic obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Analitic obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Analitic>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Analitic obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static List<AnaliticModel> GetCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if(string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Analitic>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }
            
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Analitic> coll = h.GetTypeContents<Analitic>(true, refresh);

            if (refresh)
                WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);

            return coll.Select(ConvertToModel).Distinct(new AnaliticsComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<AnaliticModel> GetCollection(int id, bool refresh = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
            List<Analitic> coll = h.GetTypeContents<Analitic>(true, refresh);
            return coll.Select(ConvertToModel).Distinct(new AnaliticsComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<AnaliticModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Analitic> coll = new List<Analitic>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Analitic>(true, refresh));
            }
            return coll.Select(ConvertToModel).Distinct(new AnaliticsComparer()).Where(f=>WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).OrderBy(o => o.Name).ToList();
        }

        public static List<AnaliticModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {                
                return WADataProvider.WA.GetCollection<Analitic>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();                
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Analitic> coll = h.GetTypeContents<Analitic>(false);
            return coll.Select(ConvertToModel).Distinct(new AnaliticsComparer()).Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).OrderBy(o => o.Name).ToList();
        }

        public static List<AnaliticModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Analitic>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            List<Analitic> coll = new List<Analitic>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Analitic>(false));
            }
            return coll.Select(ConvertToModel).Distinct(new AnaliticsComparer()).Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).OrderBy(o => o.Name).ToList();
        }
        /// <summary>
        /// Подколлекция связанных аналитик по коду родителя
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static List<AnaliticModel> GetSubCollection(string Code, bool refresh=false)
        {
            Analitic a = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Code);
            List<AnaliticModel> list = new List<AnaliticModel>();

            if (a != null)
            {
                //TODO: Проверить связи
                ChainKind kindObj = WADataProvider.WA.CollectionChainKinds.FirstOrDefault(
                    s =>
                    s.FromEntityId == (int)WhellKnownDbEntity.Analitic &&
                    s.ToEntityId == (int)WhellKnownDbEntity.Analitic && s.Code == ChainKind.TREE);
                list = Chain<Analitic>.GetChainSourceList(a, kindObj == null ? 5 : kindObj.Id, State.STATEACTIVE, refresh).Select(s => AnaliticModel.ConvertToModel(s)).ToList();
                //Chain<Analitic>.GetChainSourceList<Analitic>(a, 5, State.STATEACTIVE);
                //List<IChain<Analitic>> data = a.GetLinks(5/*kindObj == null ? 5 : kindObj.Id*/);
                //foreach (IChain<Analitic> c in data)
                //{
                //    if (c.StateId == State.STATEACTIVE)
                //    {
                //        list.Add(AnaliticModel.ConvertToModel(c.Right));
                //    }
                //}
            }

            return list;
        }

        /// <summary>
        /// Коллекция аналитики по коду аналитики содержащая собственно саму аналитику и все связанные с ней по связям 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static List<AnaliticModel> GetElementAndSubCollection(string code, bool refresh = false)
        {
            Analitic a = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(code);
            List<AnaliticModel> list = new List<AnaliticModel>();

            if (a != null)
            {
                ChainKind kindObj = WADataProvider.WA.CollectionChainKinds.FirstOrDefault(
                    s =>
                    s.FromEntityId == (int)WhellKnownDbEntity.Analitic &&
                    s.ToEntityId == (int)WhellKnownDbEntity.Analitic && s.Code == ChainKind.TREE);
                list = Chain<Analitic>.GetChainSourceList(a, kindObj == null ? 5 : kindObj.Id, State.STATEACTIVE, refresh).Select(s => AnaliticModel.ConvertToModel(s)).ToList();

                list.Add(AnaliticModel.ConvertToModel(a));
            }

            return list;
        }

        public static AnaliticModel GetObject(int id)
        {
            Analitic obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(id);
            }
            return ConvertToModel(obj);
        }

        public static AnaliticModel ConvertToModel(Analitic value)
        {
            AnaliticModel obj = new AnaliticModel();
            obj.GetData(value);

            obj.ReloadOnEdit = false;
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith(value).Select(s => s.Id.ToString()));
            return obj;
        }

        public static TrackBarItem[] GetVariants(int AnaliticId)
        {
            List<TrackBarItem> list = new List<TrackBarItem>();

            //Analitic an = new Analitic { Workarea = WADataProvider.WA };
            //an.Load(AnaliticId);
            Analitic an = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(AnaliticId);
            
            string memo;
            try
            {
                memo = an.GetValues(true).First(w => w.CodeName.Code.ToUpper() == "ANSWERS").Memo;
            }
            catch
            {
                memo = "Нет=0;Да=100;";
            }

            string[] items = memo.Split(';');
            foreach (string item in items)
            {
                string itm_name = item.Trim();
                if (itm_name.Length > 0)
                {
                    list.Add(new TrackBarItem(itm_name.Split('=')[0], itm_name.Split('=')[1]));
                }
            }

            return list.ToArray();
        }

        public static object GetBrandsRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            string name = args.Filter.ToLower();
            List<AnaliticModel> list = AnaliticModel.GetCollection(Hierarchy.SYSTEM_ANALITIC_BRANDS);
            return list.Where(w => (w.Name.ToLower().Contains(name) || w.CodeFind.ToLower().Contains(name)) && w.StateId == State.STATEACTIVE);
        }

        public static object GetBrandsFindByRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = args.Filter;
            //string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            //List<Analitic> coll = WADataProvider.WA.Empty<Analitic>().FindBy( /*myCompanyId: agentTo,*/
            //hierarchyId:WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_BRANDS).Id,
            //           name: name, useAndFilter: true, kindId: Analitic.KINDID_BRAND, stateId: State.STATEACTIVE,
            //           count: int.MaxValue,
            //           filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_BRANDS);
            List<Analitic> coll = h.GetTypeContents<Analitic>().Where(s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && s.Name.Contains(name)).ToList();

            List<AnaliticModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetProductTypesByRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = args.Filter;
            //string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            //List<Analitic> coll = WADataProvider.WA.Empty<Analitic>().FindBy( /*myCompanyId: agentTo,*/
            //hierarchyId: WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_PRODUCTTYPE).Id,
            //           name: name, useAndFilter: true, kindId: Analitic.KINDID_PRODUCTTYPE, stateId: State.STATEACTIVE,
            //           count: int.MaxValue,
            //           filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_PRODUCTTYPE);
            List<Analitic> coll = h.GetTypeContents<Analitic>().Where(s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && s.Name.Contains(name)).ToList();

            List<AnaliticModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetTradeMarkByRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = args.Filter;
            //string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            //List<Analitic> coll = WADataProvider.WA.Empty<Analitic>().FindBy( /*myCompanyId: agentTo,*/
            //hierarchyId: WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_TRADEGROUP).Id,
            //           name: name, useAndFilter: true, kindId: Analitic.KINDID_TRADEGROUP, stateId: State.STATEACTIVE,
            //           count: int.MaxValue,
            //           filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_TRADEGROUP);
            List<Analitic> coll = h.GetTypeContents<Analitic>().Where(s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && s.Name.Contains(name)).ToList();

            List<AnaliticModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        [Obsolete("Use GetAnaliticById")]
        public static object GetBrandByID(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
            }
            return null;
        }

        public static object GetAnaliticById(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
            }
            return null;
        }

        public static bool CanSave(int id)
        {
            Analitic a = new Analitic { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
    }

    internal class AnaliticsComparer : IEqualityComparer<AnaliticModel>
    {
        public bool Equals(AnaliticModel x, AnaliticModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(AnaliticModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}