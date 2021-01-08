using System;
using System.Collections.Generic;

namespace ShareLib.Page
{
    /// <summary>
    /// 定义分页树的节点类型
    /// </summary>
    public class PageTreeNode
    {
        public PageTreeNode(int pageid, int level)
        {
            _pageid = pageid;
            _level = level;
        }

        public string GetProperty(string name)
        {
            if(propertys.ContainsKey(name))
            {
                return propertys[name];
            }

            if(parent != null)
            {
                return parent.GetProperty(name);
            }
            
            return allPropertyDefault[name];
        }

        public void AddChild(PageTreeNode child)
        {
            child.parent = this;
            childs.Add(child);
        }

        public void AddProperty(string name, string value)
        {
            propertys[name] = value;

            if(!allPropertyDefault.ContainsKey(name))
            {
                allPropertyDefault[name] = "";
            }
        }

        public PageTreeNode FindNode(int pageid)
        {
            if(pageid == this.pageid)
            {
                return this;
            }

            foreach(var child in childs)
            {
                if(pageid == child.pageid)
                {
                    return child;
                }

                var node = child.FindNode(pageid);
                if(node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public IEnumerable<PageTreeNode> EnumTreeAsc()
        {
            yield return this;
            foreach(PageTreeNode child in childs)
            {
                foreach(PageTreeNode page in child.EnumTreeAsc())
                {
                    yield return page;
                }
            }
        }

        public IEnumerable<PageTreeNode> EnumTreeDesc()
        {
            for(int i = childs.Count - 1; i >= 0; --i)
            {
                foreach(PageTreeNode page in childs[i].EnumTreeDesc())
                {
                    yield return page;
                }
            }

            yield return this;
        }

        public IEnumerable<PageTreeNode> EnumChild()
        {
            return childs;
        }

        public void Dump(string prefex)
        {
            string pros = "(";
            foreach(var item in propertys)
            {
                pros += $"{item.Key}:{item.Value},";
            }
            pros += ")";

            Console.WriteLine($"{prefex}{pageid}{pros}");
            foreach(var child in childs)
            {
                child.Dump(prefex + "    ");
            }
        }

        public int pageid { get { return _pageid; } }
        public int level { get { return _level; } }

        public static Dictionary<string, string> allPropertyDefault = new Dictionary<string, string>();
        public List<PageTreeNode> childs = new List<PageTreeNode>();
        public PageTreeNode parent = null;

        private Dictionary<string, string> propertys = new Dictionary<string, string>();
        private int _pageid = -1;
        private int _level = -1;
    }
}
