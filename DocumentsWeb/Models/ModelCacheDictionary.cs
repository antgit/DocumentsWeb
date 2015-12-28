using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Models
{
    /// <summary>
    /// Кэш моделей 
    /// </summary>
    public class ModelCacheDictionary: IDictionary<string, object>
    {
        private Dictionary<string, object> cache = new Dictionary<string,object>();
        private Dictionary<string, DateTime> create_date = new Dictionary<string,DateTime>();

        /// <summary>
        /// Время жизни в часах
        /// </summary>
        public int HoursLifeTime { get; set; }

        public ModelCacheDictionary()
        {
            this.HoursLifeTime = 2;
        }

        // TODO: Проверить и исправить + подключить куда нужно, что бы производилась автоматическая очистка
        public void ClearOutdate() {
            foreach(string key in cache.Keys) {
                DateTime now = DateTime.Now;
                DateTime cur = (DateTime)create_date[key];

                TimeSpan time = now - cur;
                if (time.Hours >= HoursLifeTime) {
                    this.Remove(key);
                }
            }
        }

        public void Add(string key, object value)
        {
            if (!cache.ContainsKey(key)){
                cache.Add(key, value);
                create_date.Add(key, DateTime.Now);
            }
        }

        public bool ContainsKey(string key)
        {
            return cache.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                return cache.Keys;
            }
        }

        public bool Remove(string key)
        {
            return cache.Remove(key) && create_date.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return cache.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get 
            { 
                return cache.Values;    
            }
        }

        public object this[string key]
        {
            get
            {
                return cache[key];
            }
            set
            {
                cache[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            cache.Clear();
            create_date.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return cache.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get 
            {
                return cache.Count;
            }
        }

        public bool IsReadOnly
        {
            get {
                return false;
            }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return cache.GetEnumerator();
        }
    }
}