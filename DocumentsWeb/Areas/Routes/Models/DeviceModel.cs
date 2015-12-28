using System.Collections.Generic;
using System.Web;
using BusinessObjects;
using System.Linq;
using DevExpress.Web.ASPxEditors;
using System;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    /// <summary>Модель "Аналитика"</summary>
    public class DeviceModel : IModelData
    {
        public int ModelUserOwnerId { get; set; }
        public string ModelId { get; set; }
        /// <summary>Идентификатор</summary>
        public int Id { get; set; }
        /// <summary>Идентификатор типа</summary>
        public int KindId { get; set; }
        /// <summary>Наименование</summary>
        public string Name { get; set; }
        /// <summary>Печатное наименование</summary>
        public string NameFull { get; set; }
        /// <summary>Код валюты</summary>
        public string Code { get; set; }
        /// <summary>Примечание</summary>
        public string Memo { get; set; }
        /// <summary>Основная група</summary>
        public string DefaultGroup { get; set; }
        /// <summary>Имя пользователя создавшего/изменившего аналитику</summary>
        public string UserName { get; set; }
        /// <summary>Имя состояния</summary>
        public string StateName { get; set; }
        /// <summary>Идентификатор состояния</summary>
        public int StateId { get; set; }
        /// <summary>Идентификатор компании владельца</summary>
        public int MyCompanyId { get; set; }
        /// <summary>Компания</summary>
        public string MyCompanyName { get; set; }
        /// <summary>Дата изменения</summary>
        public DateTime? DateModified { get; set; }
        /// <summary>Только для чтения</summary>
        public bool IsReadOnly { get; set; }
        /// <summary>Флаг "Системный"</summary>
        public bool IsSystem { get; set; } //{ get { return (FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM; } }
        /// <summary>Флаг</summary>
        public int FlagsValue { get; set; }
        /// <summary>Выполнять перезагрузку из базы данных при редактировании</summary>
        public bool ReloadOnEdit { get; set; }

        public string InHierarchies { get; set; }

        public DeviceModel()
        {
            ModelId = Guid.NewGuid().ToString();
            StateId = State.STATEACTIVE;
        }
        /// <summary>Преобразование модели в объект</summary>
        /// <returns></returns>
        public Device ToObject()
        {
            Device obj = new Device();
            if (Id == 0)
                obj = new Device { Workarea = WADataProvider.WA };
            else
                obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(Id);
            
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
            obj.Memo = Memo;
            return obj;
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Device obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDevicesModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Device obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
                Device newObj = Device.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<Device>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<Device>(newObj, hies, hierarchyCode);
                /*Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }*/
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheDevicesModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Device obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDevicesModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Device obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Device>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDevicesModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Device obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDevicesModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static List<DeviceModel> GetCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if(string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Device>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }
            
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Device> coll = h.GetTypeContents<Device>(true, refresh);

            if (refresh)
                WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);

            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new DeviceComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<DeviceModel> GetCollectionForCurrentUser()
        {
            List<DeviceModel> coll = GetCollection();
            List<DeviceModel> ret = coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();
            return ret;
        }

        public static List<DeviceModel> GetCollection(int id, bool refresh = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
            List<Device> coll = h.GetTypeContents<Device>(true, refresh);
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new DeviceComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<DeviceModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Device> coll = new List<Device>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Device>(true, refresh));
            }
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new DeviceComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<DeviceModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {                
                return WADataProvider.WA.GetCollection<Device>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();                
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Device> coll = h.GetTypeContents<Device>(false);
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new DeviceComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<DeviceModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Device>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            List<Device> coll = new List<Device>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Device>(false));
            }
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new DeviceComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<DeviceModel> GetSubCollection(string Code)
        {
            Device a = WADataProvider.WA.Cashe.GetCasheData<Device>().ItemCode<Device>(Code);
            List<DeviceModel> list = new List<DeviceModel>();

            if (a != null)
            {
                //TODO: Проверить связи
                ChainKind kindObj = WADataProvider.WA.CollectionChainKinds.FirstOrDefault(
                    s =>
                    s.FromEntityId == (int)WhellKnownDbEntity.Device &&
                    s.ToEntityId == (int)WhellKnownDbEntity.Device && s.Code == ChainKind.TREE);
                list = Chain<Device>.GetChainSourceList(a, kindObj == null ? 5 : kindObj.Id, State.STATEACTIVE).Select(s=>DeviceModel.ConvertToModel(s)).ToList();
                //Chain<Device>.GetChainSourceList<Device>(a, 5, State.STATEACTIVE);
                //List<IChain<Device>> data = a.GetLinks(5/*kindObj == null ? 5 : kindObj.Id*/);
                //foreach (IChain<Device> c in data)
                //{
                //    if (c.StateId == State.STATEACTIVE)
                //    {
                //        list.Add(DeviceModel.ConvertToModel(c.Right));
                //    }
                //}
            }

            return list;
        }

        public static DeviceModel GetObject(int id)
        {
            Device obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(id);
            }
            return ConvertToModel(obj);
        }

        public static DeviceModel ConvertToModel(Device value)
        {
            DeviceModel obj =  new DeviceModel
            {
                Id = value.Id,
                KindId = value.KindId,
                Name = value.Name,
                NameFull = value.NameFull,
                Code = value.Code,
                Memo = value.Memo,
                UserName = value.UserName,
                StateName = value.State.Name,
                StateId = value.StateId,
                DateModified = value.DateModified,
                MyCompanyId = value.MyCompanyId,
                FlagsValue = value.FlagsValue
            };
            obj.IsReadOnly = value.IsReadOnly;
            obj.IsSystem = value.IsSystem;
            if (value.MyCompanyId != 0)
            {
                obj.MyCompanyName = value.MyCompany.Name;
            }
            obj.ReloadOnEdit = false;
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith<Device>(value).Select(s => s.Id.ToString()));
            return obj;
        }

        public static TrackBarItem[] GetVariants(int AnaliticId)
        {
            List<TrackBarItem> list = new List<TrackBarItem>();

            //Device an = new Device { Workarea = WADataProvider.WA };
            //an.Load(AnaliticId);
            Device an = WADataProvider.WA.Cashe.GetCasheData<Device>().Item(AnaliticId);
            
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
            string name = args.Filter;
            List<DeviceModel> list = DeviceModel.GetCollection(Hierarchy.SYSTEM_ANALITIC_BRANDS);
            return list.Where(w => w.Name.ToLower().Contains(name.ToLower()) && w.StateId == State.STATEACTIVE);
        }

        public static object GetBrandByID(ListEditItemRequestedByValueEventArgs args)
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
            Device a = new Device { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
    }

    internal class DeviceComparer : IEqualityComparer<DeviceModel>
    {
        public bool Equals(DeviceModel x, DeviceModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(DeviceModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}