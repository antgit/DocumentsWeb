using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using DevExpress.Web.ASPxEditors;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Analitics.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.Products.Models
{
    public class ProductModel :BusinessObjects.Models.ProductModel, 
        IModelData,
        IValidatableObject
    {
        public ProductModel()
        {
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование объекта учета обязательно", new[] { GlobalPropertyNames.Name });
            }

            if (!string.IsNullOrEmpty(Name) && Name.Length>255)
            {
                yield return new ValidationResult("Не более 255 символов", new[] { GlobalPropertyNames.Name });
            }

            Type targetType = GetType();
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
                    if (Errors.Count > 0)
                    {
                        foreach (string k in Errors.Keys)
                        {
                            yield return new ValidationResult(Errors[k], new[] { k });
                        }
                    }
                }
            }

        }
        
       
        public string InHierarchies { get; set; }

        /// <summary>Создание копии</summary>
        /// <param name="id"></param>
        public static void CreateProductCopy(int id)
        {
            if(id!=0)
            {
                Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
                Product newObj = Product.CreateCopy(obj);
                Hierarchy hCurrent = obj.FirstHierarchy();
                if(hCurrent!=null)
                {
                    hCurrent.ContentAdd(newObj);
                }
                if (obj.KindId == Product.KINDID_PRODUCT)
                {
                    Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
                    WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
                }
                else if(obj.KindId == Product.KINDID_SERVICE)
                {
                    Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
                    WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
                }
            }
        }

        /// <summary>Удаление в корзину</summary>
        /// <param name="id"></param>
        public static void ToTrash(int id)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            try
            {
                obj.Remove();
            }
            catch (DatabaseException dbex)
            {
                if (dbex.Id != 0)
                {
                    ErrorLog err = WADataProvider.WA.GetErrorLog(dbex.Id);
                    if (err != null && err.Message.Contains("конфликт версий"))
                    {
                        obj = WADataProvider.WA.GetObject<Product>(id);
                        obj.Remove();
                    }
                }
            }
            

            if (obj.KindId == Product.KINDID_PRODUCT)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
            else if (obj.KindId == Product.KINDID_SERVICE)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
            
        }
        public static void SetStateNotDone(int id)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            obj.StateId = State.STATENOTDONE;
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
                        obj = WADataProvider.WA.GetObject<Product>(id);

                        obj.StateId = State.STATENOTDONE;
                        obj.Save();
                    }
                }
            }

            if (obj.KindId == Product.KINDID_PRODUCT)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
            else if (obj.KindId == Product.KINDID_SERVICE)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
        }
        public static void SetStatetDone(int id)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Product>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }

            if (obj.KindId == Product.KINDID_PRODUCT)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
            else if (obj.KindId == Product.KINDID_SERVICE)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
        }
        public static void SetStateDeny(int id)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            obj.StateId = State.STATEDENY;
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
                        obj = WADataProvider.WA.GetObject<Product>(id);

                        obj.StateId = State.STATEDENY;
                        obj.Save();
                    }
                }
            }

            if (obj.KindId == Product.KINDID_PRODUCT)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
            else if (obj.KindId == Product.KINDID_SERVICE)
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
            }
        }

        public Product ToObject(Workarea workarea)
        {
            Product product = new Product { Workarea = workarea };
            if(Id!=0)
                product = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(Id);
            if (product.Id == 0)
            {
                product.IsNew = true;
                product.StateId = State.STATEACTIVE;
                product.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
                //product.KindId = Product.KINDID_PRODUCT;
            }
            product.Id = Id;
            product.Name = Name;
            product.NameFull = NameFull;
            product.Code = Code;
            product.Nomenclature = Nomenclature;
            product.Memo = Memo;
            product.UnitId = UnitId ?? 0;
            product.Barcode = Barcode;

            product.ProductTypeId = ProductTypeId ?? 0;
            product.PakcTypeId = PakcTypeId ?? 0;
            product.BrandId = BrandId ?? 0;
            product.TradeMarkId = TradeMarkId ?? 0;
            product.Weight = Weight;
            product.Width = Width;
            product.Height = Height;
            product.Depth = Depth;
            product.Size = Size;
            
            return product;
        }
        /// <summary>
        /// Преобразование товара
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ProductModel ConvertToModel(Product value)
        {
            ProductModel obj = new ProductModel();
            obj.GetData(value);

            obj.ReloadOnEdit = false;
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith(value).Select(s => s.Id.ToString(CultureInfo.InvariantCulture)));
            return obj;
            
        }
        /// <summary>
        /// Список товаров
        /// </summary>
        public static IEnumerable GetProducts(bool refresh = false, bool onlyActiveState = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCTS);
            if (!refresh)
            {
                if (WADataProvider.CacheProductsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheProductsModelData.GetDataForUser(hRoot.Id);
            }

            List<Product> coll = hRoot.GetTypeContents<Product>(true, refresh).Where(s => (!onlyActiveState || s.IsStateActive) && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();
            
            WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheProductsModelData.GetDataForUser(hRoot.Id);
        }
        /// <summary>
        /// Список товаров
        /// </summary>
        public static IEnumerable GetServices(bool refresh = false, bool onlyActiveState = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_PRODUCT_SERVICE);
            if (!refresh)
            {
                if (WADataProvider.CacheProductsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheProductsModelData.GetDataForUser(hRoot.Id);
            }

            List<Product> coll = hRoot.GetTypeContents<Product>(true, refresh).Where(s => (!onlyActiveState || s.IsStateActive) && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();

            WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheProductsModelData.GetDataForUser(hRoot.Id);
        }
        /// <summary>
        /// Список товаров
        /// </summary>
        /// <param name="rootHierarchyCode">Код иерархии в котрой необходимо искать товары</param>
        /// <param name="refresh">Выполнять запрос к базе данных или использовать кеш данных</param>
        /// <param name="onlyActiveState">Только в состоянии "Активные"</param>
        public static IEnumerable GetCollection(string rootHierarchyCode=null, bool refresh=false, bool onlyActiveState=true)
        {
            if(string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Product>().Where(s => (!onlyActiveState || s.IsStateActive) && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).OrderBy(s=>s.Name).Select(ConvertToModel).ToList();
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Product> coll = h.GetTypeContents<Product>(true, refresh).Where(s => (!onlyActiveState || s.IsStateActive) && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();
            return coll.Select(ConvertToModel).OrderBy(s=>s.Name).ToList();
        }

        public static List<ProductModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Product> coll = new List<Product>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Product>(true, refresh).Where(
                    f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).ToList());
            }
            return coll.Select(ConvertToModel).Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<ProductModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Product>().Where(s => !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Product> coll = h.GetTypeContents<Product>().Where(
                    f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).ToList();
            return coll.Select(ConvertToModel).Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<ProductModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Product>().Where(s => !s.IsHiden 
                    && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<Product> coll = new List<Product>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Product>().Where(s=>WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)));
            }
            return coll.Select(ConvertToModel).Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Товар по идентификатору
        /// </summary>
        public static ProductModel GetObject(int id)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            return ConvertToModel(obj);
        }

        public static List<AnaliticGroupModel> GetProductTypes()
        {
            return AnaliticGroupModel.GetGroups().Where(w => w.KindId == AnaliticGroupKind.MktgProductType).ToList();
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
                Product newObj = Product.CreateCopy(obj);
                Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj);
                }
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Product>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static object GetProductsByValue(object value)
        {
            return value != null ?  new List<ProductModel>{ GetObject((int)value)}.Take(1).ToList() : null;
        }

        public static object GetProductRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            List<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
                       name: name, useAndFilter: true, kindId: Product.KINDID_PRODUCT, stateId: State.STATEACTIVE,
                       count: int.MaxValue,
                       filter:s =>!s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetProductNomenclatureRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            List<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
            useAndFilter: true, kindId: Product.KINDID_PRODUCT, stateId: State.STATEACTIVE, nomenclature:name,
                       count: int.MaxValue,
                       filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetServiceRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            // 

            IEnumerable<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
                       name: name, useAndFilter: true, kindId: Product.KINDID_SERVICE, stateId: State.STATEACTIVE,
                       count: int.MaxValue,
                       filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetServiceNomenclatureRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            // 

            IEnumerable<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
                        useAndFilter: true, kindId: Product.KINDID_SERVICE, stateId: State.STATEACTIVE, nomenclature:name,
                        count: int.MaxValue,
                        filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetProductAndServiceRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            List<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
                       name: name, useAndFilter: true, stateId: State.STATEACTIVE,
                       count: int.MaxValue,
                       filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetProductAndServiceNomenclatureRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            List<Product> coll = WADataProvider.WA.Empty<Product>().FindBy( /*myCompanyId: agentTo,*/
            useAndFilter: true, stateId: State.STATEACTIVE, nomenclature: name,
                       count: int.MaxValue,
                       filter: s => !s.IsTemplate && s.IsStateActive && !s.IsHiden && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));

            List<ProductModel> ret = coll.Skip(skip).Take(take).Select(ConvertToModel).OrderBy(o => o.Name).ToList();

            return ret;
        }

        public static object GetProductByID(ListEditItemRequestedByValueEventArgs args)
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
            Product p = new Product { Workarea = WADataProvider.WA };
            p.Load(id);
            return !(p.IsSystem | p.IsReadOnly);
        }

        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject(WADataProvider.WA).FlagsValueString();
        }

        internal class AgentComparer : IEqualityComparer<ProductModel>
        {
            public bool Equals(ProductModel x, ProductModel y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(ProductModel obj)
            {
                return 0;//obj.GetHashCode();
            }
        }
    }
}