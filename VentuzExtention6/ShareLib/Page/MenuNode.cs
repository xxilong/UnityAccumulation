using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Page
{
    public class MenuNode
    {
        public MenuNode(string title)
        {
            _title = title;
        }

        public void AddChild(MenuNode node)
        {
            node._parent = this;
            node._id = $"{_id}-{_childs.Count}";
            _childs.Add(node);
        }

        public void AddProperty(string name, string value)
        {
            _propertys[name] = value;
        }

        public string Id { get { return _id; } }
        public string Title { get { return _title; } }
        public int MaxSubLevel {
            get
            {
                if(_childs.Count == 0)
                {
                    return 0;
                }

                int childLevel = 0;
                foreach(var c in _childs)
                {
                    int subLevel = c.MaxSubLevel;
                    if(subLevel > childLevel)
                    {
                        childLevel = subLevel;
                    }
                }

                return childLevel + 1;
            }
        }
        public MenuNode Parent { get { return _parent; } }
        public IEnumerable<MenuNode> Childs { get { return _childs; } }
        public int ChildCount { get { return _childs.Count; } }
        public EventHandler<bool> SelectChange = null;
        public bool IsSelected {
            get { return _selected; }
            set
            {
                if(_selected == value)
                {
                    return;
                }

                SelectChange?.Invoke(this, value);
                _selected = value;
            }
        }

        private string _id = "0";
        private string _title;
        private MenuNode _parent = null;
        private bool _selected = false;
        private List<MenuNode> _childs = new List<MenuNode>();
        private Dictionary<string, string> _propertys = new Dictionary<string, string>();  // 不继承父节点属性  
    }
}
