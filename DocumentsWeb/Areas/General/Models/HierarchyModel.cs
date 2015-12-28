using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using System.Web.UI;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Models
{
    public class HierarchyModel
    {
        /// <summary>
        /// Идентификатор связии иерархий по котором фильтруется грид
        /// </summary>
        public static int FILTER_HIERARCHY_CHAIN = 91;
        /// <summary>
        /// Идентификатор связи иерархий из окон редактирования
        /// </summary>
        public static int EDIT_HIERARCHY_CHAIN = 122;

        public int Id { get; set; }
        public int ParentId { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public String Name { get; set; }

        [Display(Name = "Код")]
        public String Code { get; set; }

        [Display(Name = "Примечание")]
        public String Memo { get; set; }

        public bool HasChild { get; set; }

        private int ViewListId { get; set; }
        private CustomViewList _viewList { get; set; }
        public CustomViewList ViewList 
        { 
            get
            {
                if(ViewListId !=0 && _viewList == null)
                {
                    _viewList = WADataProvider.WA.GetObject<CustomViewList>(ViewListId);
                }
                return _viewList;
            }
        }

        public static HierarchyModel GetObject(int id)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
            return ToModel(h);
        }

        public static HierarchyModel GetObject(string code)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(code);
            return ToModel(h);
        }

        public static HierarchyModel ToModel(Hierarchy hie)
        {
            return new HierarchyModel
            {
                Id = hie.Id,
                ParentId = hie.ParentId,
                Name = hie.Name,
                Code = hie.Code,
                Memo = hie.Memo,
                HasChild = hie.HasChildren, //hie.Children.Count > 0
                ViewListId = hie.ViewListId
            };
        }

        /// <summary>
        /// Возвращает ветку
        /// </summary>
        /// <param name="id">Идентификатор корня ветки</param>
        /// <returns>Коллекция вложенных в ветку элементов</returns>
        public static List<HierarchyModel> GetBranch(int id)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
            // TODO: Добавить обработку области видимости для компании и разрешения пользователя
            //List<IChainAdvanced<Hierarchy, Agent>> coll = h.Children[0].GetLinkedHierarchy();
            //WADataProvider.IsCompanyIdAllowIdToCurrentUser(coll[0].RightId)
            return h.Children.Where(f => f.GetLinkedHierarchy().Exists(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.RightId))
                && WADataProvider.HiearchyElementRightView.IsAllow(Right.VIEW, f.Id)).Select(s => HierarchyModel.ToModel(s)).ToList();

            //return h.Children.Select(s => HierarchyModel.ToModel(s)).ToList();
        }

        /// <summary>
        /// Возвращает коллекцию иерархий по связям
        /// </summary>
        /// <param name="root">Корневая иерархия</param>
        /// <param name="with_root">Добавлять в список корневую иерархию</param>
        /// <returns>Коллекция иерархий по связям</returns>
        public static List<HierarchyModel> GetLinkedHierarchies(string root, int chainKindId, bool with_root = true)
        {
            List<HierarchyModel> list = new List<HierarchyModel>();

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(root);
            if (with_root)
            {
                list.Add(HierarchyModel.ToModel(h));
            }
            // TODO: Как правельно получить идентификатор типа связи?
            List<IChain<Hierarchy>> chains = h.GetLinks(chainKindId);

            foreach (IChain<Hierarchy> c in chains)
            {
                list.Add(HierarchyModel.ToModel(c.Right));
            }

            return list;
        }

        /// <summary>
        /// Возвращает коллекцию иерархий в которые входит указанный объект
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Объект</param>
        /// <returns>Коллекция иерархий содержащих указанный объект</returns>
        public static List<HierarchyModel> GetHierarchiesWith<T>(T obj) where T : class, IBase, new()
        {
            Hierarchy h = WADataProvider.WA.Empty<Hierarchy>(); // new Hierarchy { Workarea = WADataProvider.WA };
            return h.Hierarchies<T>(obj).Select(ToModel).ToList<HierarchyModel>();
        }

        /// <summary>
        /// Разсовывыет элемент по иерархиям
        /// </summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="obj">Элемент</param>
        /// <param name="ContainsIn">Строка с идентификаторами иерархий в которые должен входить элемент (через запятую)</param>
        /// <param name="RootHierarchy">Корневая иерархия по умолчанию</param>
        public static void UpdateHierarchiesContent<T>(T obj, string ContainsIn, string RootHierarchy) where T : class, IBase, new()
        {
            // Список иерархий в которые входит элемент
            string[] currHies = HierarchyModel.GetHierarchiesWith<T>(obj).Select(s => s.Id.ToString()).ToArray<string>();
            // Список иерархий в которые должен входить элемент
            string[] setHies = ContainsIn.Split(',');

            // Иерархии из которых нужно удалить элемент
            string[] removeFrom = currHies.Where(w => !setHies.Contains(w)).ToArray<string>();
            // Иерархии в которые необходимо добавить элемент
            string[] addTo = setHies.Where(w => !currHies.Contains(w)).ToArray<string>();

            // Удаляем элемент из иерархий
            foreach (string z in removeFrom)
            {
                string val = z.Trim();

                if (val.Length > 0)
                {
                    int id = int.Parse(val);
                    Hierarchy h = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
                    h.ContentRemove(obj);
                }
            }

            // Добавляем элемент в иерархии
            foreach (string z in addTo)
            {
                string val = z.Trim();

                if (val.Length > 0)
                {
                    int id = int.Parse(val);
                    Hierarchy h = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(id);
                    h.ContentAdd(obj, true);
                }
            }

            // Если элемент небыл добавлен ни в одну иерархию, добавляем его в корневую по умолчанию
            if (currHies.Length == 0)
            {
                Hierarchy h = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierarchy);
                h.ContentAdd(obj, true);
            }
        }

        /// <summary>
        /// Получение данных по иерархии поиска
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomView()
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(Id);
            DataTable tbl=h.GetCustomView();
            return tbl;
        }
    }
}