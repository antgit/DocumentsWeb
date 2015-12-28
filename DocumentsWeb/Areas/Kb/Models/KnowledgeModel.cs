using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.Kb.Models
{
    /// <summary>
    /// Модель элемента базы знаний
    /// </summary>
    public class KnowledgeModel : BusinessObjects.Models.KnowledgeModel, IModelData
    {
        public KnowledgeModel()
        {
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject().FlagsValueString();
        }

        /// <summary>
        /// Идентификатор иерархии в которую необходимо поместить элемент
        /// </summary>
        public int HierarchyId { get; set; }

        public string InHierarchies { get; set; }
        
        public static KnowledgeModel GetObject(int id)
        {
            Knowledge obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
            }
            return ConvertToModel(obj);
        }
        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        /// <param name="val">Объект базы знаний</param>
        /// <returns>Модель объекта базы знаний</returns>
        public static KnowledgeModel ConvertToModel(Knowledge val)
        {
            KnowledgeModel obj = new KnowledgeModel();
            obj.GetData(val);
            return obj;
        }

        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        /// <param name="id">Идентификатор объекта базы знаний</param>
        /// <returns>Модель объекта базы знаний</returns>
        public static KnowledgeModel ConvertToModel(int id)
        {
            Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
            return ConvertToModel(obj);
        }

        /// <summary>
        /// Конвертирует модель в объект
        /// </summary>
        /// <returns>Объект базы знаний</returns>
        public Knowledge ToObject()
        {
            Knowledge obj = new Knowledge();
            if (Id == 0)
                obj = new Knowledge { Workarea = WADataProvider.WA };
            else
                obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(Id);

            if (obj.Id == 0)
            {
                obj.IsNew = true;
                obj.StateId = State.STATEACTIVE;
                obj.Id = Id;
                obj.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
            }

            obj.Name = Name;
            obj.NameFull = NameFull;
            obj.StateId = StateId == 0 ? State.STATEACTIVE : StateId;
            obj.Code = Code;
            obj.CodeFind = CodeFind;
            if (FileId.HasValue && FileId != 0)
                obj.FileId = FileId.Value;
            else
                obj.FileId = 0;
            obj.KindId = KindId;
            obj.Memo = Memo;
            if (IsReadOnly)
                obj.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                obj.FlagsValue &= ~FlagValue.FLAGREADONLY;
            
            return obj;
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
                Knowledge newObj = Knowledge.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<Knowledge>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<Knowledge>(newObj, hies, hierarchyCode);
                /*Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }*/
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Knowledge>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Knowledge obj = WADataProvider.WA.Cashe.GetCasheData<Knowledge>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheKnowledgeModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        /// <summary>
        /// Возвращает коллекцию элементов базы знаний
        /// </summary>
        /// <param name="HierarchyId">Идентификатор ветки иерархии</param>
        /// <param name="Refresh">Обновить объекты в кэш</param>
        /// <returns>Коллекция объектов</returns>
        public static List<KnowledgeModel> GetCollection(int HierarchyId, bool Refresh = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(HierarchyId);
            List<Knowledge> coll = h.GetTypeContents<Knowledge>(true, Refresh);
            if (Refresh) WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            return coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }

        public static bool CanSave(int id)
        {
            Knowledge a = new Knowledge { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
    }
}