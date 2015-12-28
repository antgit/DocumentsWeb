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
    /// <summary>������ "���������"</summary>
    public class RouteMemberModel : IModelData
    {
        public int ModelUserOwnerId { get; set; }
        public string ModelId { get; set; }
        /// <summary>�������������</summary>
        public int Id { get; set; }
        /// <summary>������������� ����</summary>
        public int KindId { get; set; }
        /// <summary>������������</summary>
        public string Name { get; set; }
        /// <summary>�������� ������������</summary>
        public string NameFull { get; set; }
        /// <summary>��� ������</summary>
        public string Code { get; set; }
        /// <summary>����������</summary>
        public string Memo { get; set; }
        /// <summary>�������� �����</summary>
        public string DefaultGroup { get; set; }
        /// <summary>��� ������������ ����������/����������� ���������</summary>
        public string UserName { get; set; }
        /// <summary>��� ���������</summary>
        public string StateName { get; set; }
        /// <summary>������������� ���������</summary>
        public int StateId { get; set; }
        /// <summary>������������� �������� ���������</summary>
        public int MyCompanyId { get; set; }
        /// <summary>��������</summary>
        public string MyCompanyName { get; set; }
        /// <summary>���� ���������</summary>
        public DateTime? DateModified { get; set; }
        /// <summary>������ ��� ������</summary>
        public bool IsReadOnly { get; set; }
        /// <summary>���� "���������"</summary>
        public bool IsSystem { get; set; } //{ get { return (FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM; } }
        /// <summary>����</summary>
        public int FlagsValue { get; set; }
        /// <summary>��������� ������������ �� ���� ������ ��� ��������������</summary>
        public bool ReloadOnEdit { get; set; }

        public string InHierarchies { get; set; }

        /// <summary>������������� ����������</summary>
        public int DeviceId { get; set; }

        /// <summary>������������� ��������� �������������</summary>
        public int? ManagerId { get; set; }
        /// <summary>������������� ������������</summary>
        public int? SupervisorId { get; set; }
        /// <summary>������������� �����������</summary>
        public int? ShippingId { get; set; }

        public RouteMemberModel()
        {
            ModelId = Guid.NewGuid().ToString();
            StateId = State.STATEACTIVE;
        }
        /// <summary>�������������� ������ � ������</summary>
        /// <returns></returns>
        public RouteMember ToObject()
        {
            RouteMember obj = new RouteMember();
            if (Id == 0)
                obj = new RouteMember { Workarea = WADataProvider.WA };
            else
                obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(Id);
            
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
            obj.DeviceId = DeviceId;
            obj.ManagerId = ManagerId ?? 0;
            obj.SupervisorId = SupervisorId ?? 0;
            obj.ShippingId = ShippingId ?? 0;
            return obj;
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            RouteMember obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheRouteMembersModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                RouteMember obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
                RouteMember newObj = RouteMember.CreateCopy(obj);
                newObj.Name += " (�����)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<RouteMember>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<RouteMember>(newObj, hies, hierarchyCode);
                /*Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }*/
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheRouteMembersModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            RouteMember obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheRouteMembersModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            RouteMember obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
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
                    if (err != null && err.Message.Contains("�������� ������"))
                    {
                        obj = WADataProvider.WA.GetObject<RouteMember>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheRouteMembersModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            RouteMember obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheRouteMembersModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static List<RouteMemberModel> GetCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if(string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<RouteMember>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }
            
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<RouteMember> coll = h.GetTypeContents<RouteMember>(true, refresh);

            if (refresh)
                WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);

            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new RouteMemberComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<RouteMemberModel> GetCollection(int id, bool refresh = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
            List<RouteMember> coll = h.GetTypeContents<RouteMember>(true, refresh);
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new RouteMemberComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<RouteMemberModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<RouteMember> coll = new List<RouteMember>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<RouteMember>(true, refresh));
            }
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new RouteMemberComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<RouteMemberModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<RouteMember>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Where(s => !s.IsHiden).Select(ConvertToModel).ToList();                
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<RouteMember> coll = h.GetTypeContents<RouteMember>(false);
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new RouteMemberComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<RouteMemberModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<RouteMember>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            List<RouteMember> coll = new List<RouteMember>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<RouteMember>(false));
            }
            return coll.Select(ConvertToModel).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Distinct(new RouteMemberComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<RouteMemberModel> GetSubCollection(string Code)
        {
            RouteMember a = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().ItemCode<RouteMember>(Code);
            List<RouteMemberModel> list = new List<RouteMemberModel>();

            if (a != null)
            {
                //TODO: ��������� �����
                ChainKind kindObj = WADataProvider.WA.CollectionChainKinds.FirstOrDefault(
                    s =>
                    s.FromEntityId == (int)WhellKnownDbEntity.RouteMember &&
                    s.ToEntityId == (int)WhellKnownDbEntity.RouteMember && s.Code == ChainKind.TREE);
                list = Chain<RouteMember>.GetChainSourceList(a, kindObj == null ? 5 : kindObj.Id, State.STATEACTIVE).Select(s=>RouteMemberModel.ConvertToModel(s)).ToList();
                //Chain<RouteMember>.GetChainSourceList<RouteMember>(a, 5, State.STATEACTIVE);
                //List<IChain<RouteMember>> data = a.GetLinks(5/*kindObj == null ? 5 : kindObj.Id*/);
                //foreach (IChain<RouteMember> c in data)
                //{
                //    if (c.StateId == State.STATEACTIVE)
                //    {
                //        list.Add(RouteMemberModel.ConvertToModel(c.Right));
                //    }
                //}
            }

            return list;
        }

        public static RouteMemberModel GetObject(int id)
        {
            RouteMember obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(id);
            }
            return ConvertToModel(obj);
        }

        public static RouteMemberModel ConvertToModel(RouteMember value)
        {
            RouteMemberModel obj =  new RouteMemberModel
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
                FlagsValue = value.FlagsValue,
                DeviceId = value.DeviceId,
                ManagerId = value.ManagerId,
                SupervisorId = value.SupervisorId,
                ShippingId = value.ShippingId
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
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith<RouteMember>(value).Select(s => s.Id.ToString()));
            return obj;
        }

        public static TrackBarItem[] GetVariants(int AnaliticId)
        {
            List<TrackBarItem> list = new List<TrackBarItem>();

            //RouteMember an = new RouteMember { Workarea = WADataProvider.WA };
            //an.Load(AnaliticId);
            RouteMember an = WADataProvider.WA.Cashe.GetCasheData<RouteMember>().Item(AnaliticId);
            
            string memo;
            try
            {
                memo = an.GetValues(true).First(w => w.CodeName.Code.ToUpper() == "ANSWERS").Memo;
            }
            catch
            {
                memo = "���=0;��=100;";
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
            List<RouteMemberModel> list = RouteMemberModel.GetCollection(Hierarchy.SYSTEM_ANALITIC_BRANDS);
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
            RouteMember a = new RouteMember { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
    }

    internal class RouteMemberComparer : IEqualityComparer<RouteMemberModel>
    {
        public bool Equals(RouteMemberModel x, RouteMemberModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(RouteMemberModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}