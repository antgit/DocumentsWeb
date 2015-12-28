using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb
{
    public interface IModelData: BusinessObjects.Models.IModelData
    {
        int Id{get;set;}
        int MyCompanyId { get; set; }
        string UserName { get; set; }
        bool ReloadOnEdit { get; set; }
        string InHierarchies { get; set; }
    }
    public class HierarchyUserDataCashe<T> where T : class, IModelData
    {
        static HierarchyUserDataCashe()
        {
            if (StoredData==null)
                StoredData = new Dictionary<int, List<T>>();
        }
        internal static Dictionary<int, List<T>> StoredData;

        public List<T> GetFromCashe(int key)
        {
            if (StoredData.ContainsKey(key))
                return StoredData[key];
            else
                return new List<T>();
        }

        public void AddToCashe(int key, List<T> values)
        {
            if (StoredData.ContainsKey(key))
                StoredData[key] = values;
            else
                StoredData.Add(key, values);
        }
        public void AddToCashe(int key, T value)
        {
            if (!ContainsKey(key))
            {
                AddToCashe(key, new List<T>());
            }
            if (ExistValueInCashe(key, value))
            {
                T existObj = StoredData[key].Find(f => f.Id == value.Id);
                int idx = StoredData[key].IndexOf(existObj);
                StoredData[key][idx] = value;
            }
            else
            {
                StoredData[key].Add(value);
            }
        }
        public bool ExistInCashe(int key)
        {
            return StoredData.ContainsKey(key);
        }
        public bool ContainsKey(int key)
        {
            return ExistInCashe(key);
        }
        public bool ExistValueInCashe(int key, T value)
        {
            if (!StoredData.ContainsKey(key))
                return false;
            return StoredData[key].Exists(f => f.Id == value.Id);
        }
        public bool ExistValueInCashe(int key, int id)
        {
            if (!StoredData.ContainsKey(key))
                return false;
            return StoredData[key].Exists(f => f.Id == id);
        }

        public T GetFromCashe(int key, int id)
        {
            if (ExistValueInCashe(key, id))
                return StoredData[key].First(f => f.Id == id);
            return null;
        }
        /// <summary>
        /// ƒанные дл€ пользовател€ в области видимости
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetDataForUser(int key)
        {
            if (!ExistInCashe(key))
                return new List<T>();

            return StoredData[key].Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).ToList();
        }
        /// <summary>
        /// ƒанные дл€ пользовател€ в области создани€
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetDataForUserInCreateScope(int key)
        {
            if (!ExistInCashe(key))
                return new List<T>();

            return StoredData[key].Where(f => WADataProvider.IsCompanyIdAllowCreateToCurrentUser(f.MyCompanyId)).ToList();
        }
    }
}