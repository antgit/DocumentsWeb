using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Models
{
    public class NewsModel : MessageModel, IModelData
    {
        public NewsModel()
        {
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }

        /// <summary>
        /// Идентификатор иерархии в которую необходимо поместить элемент
        /// </summary>
        public int HierarchyId { get; set; }

        public string InHierarchies { get; set; }

        public static NewsModel GetObject(int id)
        {
            Message obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            }
            return ConvertToModel(obj);
        }
        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        /// <param name="val">Объект базы знаний</param>
        /// <returns>Модель объекта базы знаний</returns>
        public static NewsModel ConvertToModel(Message val)
        {
            NewsModel obj = new NewsModel();
            obj.GetData(val);
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith<Message>(val).Select(s => s.Id.ToString()));
            //NewsModel obj = new NewsModel
            //                         {
            //                             Id = val.Id,
            //                             Name = val.Name,
            //                             NameFull = val.NameFull,
            //                             StateId = val.StateId,
            //                             StateName = val.State.Name,
            //                             MyCompanyId = val.MyCompanyId,
            //                             Code = val.Code,
            //                             CodeFind = val.CodeFind,
            //                             IsReadOnly = val.IsReadOnly,
            //                             IsSystem = val.IsSystem,
            //                             KindId = val.KindId,
            //                             Memo = val.Memo,
            //                             UserName = val.UserName,
            //                             DateModified = val.DateModified
            //                         };
            //if (val.MyCompanyId != 0)
            //    obj.MyCompanyName = val.MyCompany.Name;
            return obj;
        }

        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        /// <param name="Id">Идентификатор объекта базы знаний</param>
        /// <returns>Модель объекта базы знаний</returns>
        public static NewsModel ConvertToModel(int Id)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(Id);
            return ConvertToModel(obj);
        }

        /// <summary>
        /// Конвертирует модель в объект
        /// </summary>
        /// <returns>Объект базы знаний</returns>
        public Message ToObject()
        {
            Message obj = new Message { Workarea = WADataProvider.WA };
            if (Id == 0)
            {
                obj.KindId = Message.KINDID_USER;
                obj.StateId = State.STATEACTIVE;
                //obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
            }
            else
                obj.Load(Id);

            obj.Name = Name;
            obj.NameFull = NameFull;
            obj.StateId = StateId == 0 ? State.STATEACTIVE : StateId;
            obj.Code = Code;
            obj.CodeFind = CodeFind;
            obj.Memo = Memo;
            obj.UserOwnerId = UserOwnerId;
            obj.UserId = UserId;
            if (SendDate.HasValue)
            {
                obj.SendDate = SendDate;
                obj.SendTime = Period.DateTime2TimeSpan(SendTimeAsDate);
                obj.IsSend = true;
            }
            else
            {
                obj.SendDate = null;
                obj.SendTime = null;
                obj.IsSend = false;
            }

            obj.PriorityId = PriorityId;
            return obj;
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
                Message newObj = Message.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<Message>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<Message>(newObj, hies, hierarchyCode);
                /*Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }*/
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
               // WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Message>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        /// <summary>
        /// Возвращает коллекцию элементов новостей
        /// </summary>
        /// <param name="HierarchyId">Идентификатор ветки иерархии</param>
        /// <param name="Refresh">Обновить объекты в кэш</param>
        /// <returns>Коллекция объектов</returns>
        public static List<NewsModel> GetCollection(int HierarchyId, bool Refresh = false, bool nested = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(HierarchyId);
            List<Message> coll = h.GetTypeContents<Message>(nested, Refresh);
            if (Refresh) WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            return coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }
        

        public static bool CanSave(int id)
        {
            Message a = new Message { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject().FlagsValueString();
        }
    }
}