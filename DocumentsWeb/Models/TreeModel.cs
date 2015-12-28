using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Linq;
using BusinessObjects;

namespace DocumentsWeb.Models
{
    public class TreeModel : IHierarchicalEnumerable
    {
        private  List<TreeItemModel> _rootColl;

        public TreeModel()
        {
            _rootColl = new List<TreeItemModel>();
        }

        public TreeModel(IEnumerable<TreeItemModel> list)
        {
            _rootColl=new List<TreeItemModel>();
            _rootColl.AddRange(list);
        }

        public TreeModel(string rootHierarchyCode)
        {
            _rootColl = new List<TreeItemModel>();

            Hierarchy rootHierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            if (rootHierarchy == null) 
                throw new Exception("Не найдена корневая иерерхия для построения дерева");

            //Добавление корня
            TreeItemModel rootItem = new TreeItemModel { Id = rootHierarchy.Id, Name = rootHierarchy.Name };
            AddToRoot(rootItem);

            //Добавление элементов
            foreach (var hierarchy in rootHierarchy.Children.Where(s => !s.IsVirtual && s.Code != "FINDROOT" && !(s.Code.StartsWith("SYSTEM")&&s.Code.EndsWith("FINDROOT"))))
            {
                List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);
                List<IChainAdvanced<Hierarchy, Agent>> hierarchyScope = hierarchy.GetLinkedHierarchy().Where(s => s.KindId == WADataProvider.ScopeViewListId).ToList();
                bool allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
                if (allow || WADataProvider.HiearchyElementRightView.IsAllow("VIEW", hierarchy.Id))
                {
                    TreeItemModel item = new TreeItemModel {Id = hierarchy.Id, Name = hierarchy.Name};
                    rootItem.AddToChildrens(item);

                    //Добавление вложенных элементов
                    if (hierarchy.HasChildren)
                    {
                        AddChildsFromHierarchy(item, hierarchy);
                    }
                }
            }
        }

        private static void AddChildsFromHierarchy(TreeItemModel item, Hierarchy hierarchy)
        {
            foreach (var h in hierarchy.Children)
            {
                List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);
                List<IChainAdvanced<Hierarchy, Agent>> hierarchyScope = h.GetLinkedHierarchy().Where(s => s.KindId == WADataProvider.ScopeViewListId).ToList();
                bool allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));

                if (allow || WADataProvider.HiearchyElementRightView.IsAllow("VIEW", h.Id))
                {
                    TreeItemModel newItem = new TreeItemModel {Id = h.Id, Name = h.Name};
                    item.AddToChildrens(newItem);
                    if (h.HasChildren)
                        AddChildsFromHierarchy(newItem, h);
                }
            }
        }

        public void AddToRoot(TreeItemModel item)
        {
            _rootColl.Add(item);
        }

        #region IHierarchicalEnumerable members
        public IEnumerator GetEnumerator()
        {
            return _rootColl.GetEnumerator();
        }

        public IHierarchyData GetHierarchyData(object enumeratedItem)
        {
            return enumeratedItem as IHierarchyData;
        }
        #endregion
    }

    public class TreeItemModel : IHierarchyData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private List<TreeItemModel> _childrens;
        private TreeItemModel _parent;

        public TreeItemModel()
        {
            _parent = null;
            _childrens = new List<TreeItemModel>();
        }

        //public override string ToString()
        //{
        //    return Name;
        //}

        public void AddToChildrens(TreeItemModel item)
        {
            item._parent = this;
            _childrens.Add(item);
        }

        #region IHierarchicalEnumerable members
        public IHierarchicalEnumerable GetChildren()
        {
            return new TreeModel(_childrens);
        }

        public IHierarchyData GetParent()
        {
            return _parent;
        }

        public bool HasChildren
        {
            get { return _childrens.Count > 0; }
        }

        public string Path
        {
            get
            {
                TreeItemModel current = this;
                string path = "";
                while(current!=null)
                {
                    path = current.ToString() + '/' + path;
                    current = current._parent;
                }
                return path;
            }
        }

        public object Item
        {
            get { return this; }
        }

        public string Type
        {
            get { return typeof(TreeItemModel).ToString(); }
        }
        #endregion
    }
}