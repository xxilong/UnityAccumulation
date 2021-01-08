using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ShareLib.Conf;
using ShareLib.Log;

namespace ShareLib.Page
{
    public class PageInfo
    {
        #region 初始化
        /*
         * 为支持在属性中使用配置文件项提供支持, 使用配置的语法为 @(domain/key) 
         */

        // 从文本中读取页面列表
        public void ParsePageList(string text)
        {
            string[] lines = text.Split('\n');
            pageList.Clear();
            int index = 0;
            foreach(string rline in lines)
            {
                var line = rline.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if(line.StartsWith("#"))
                {
                    continue;
                }

                if(line.StartsWith("//"))
                {
                    continue;
                }

                pageList[line] = index++;
            }

            Console.WriteLine($"Loaded { index } Page Defines.");
        }

        // 从 enum 类型定义中读取页面列表
        public void LoadPageList(Type pageEnumType)
        {
            foreach(var item in Enum.GetValues(pageEnumType))
            {
                pageList[item.ToString()] = (int)item;
            }

            Console.WriteLine($"Loaded { pageList.Count } Page From Enum.");
        }

        // 自动生成页面列表, 建议仅在 ParsePageTree 过程中打开
        public bool AutoGenPage { set { _autoGenPageList = value; } }
        public int PageCount { get { return pageList.Count; } }

        public void ParsePageTree(string text)
        {
            treeRoot = new PageTreeNode(-1, -1);
            PageTreeNode.allPropertyDefault.Clear();

            Regex vardef = new Regex(@"\+(\S+)[=:]\((.+)\)");
            Regex nodedef = new Regex(@"(-*)(\w*)(?:\((.+)\))?");
            char[] valsep = new char[] { '=', ':' };

            var vartable = new Dictionary<string, string>();

            int curlevel = -1;
            PageTreeNode curnode = treeRoot;

            string[] lines = text.Split('\n');
            foreach (string rline in lines)
            {
                var line = rline.Trim();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                if (line.StartsWith("//"))
                {
                    continue;
                }

                if(line.StartsWith("+"))
                {
                    var matchs = vardef.Match(line);
                    if(!matchs.Success)
                    {
                        Logger.Error($"变量定义语法错误: {line}");
                        continue;
                    }

                    var name = matchs.Groups[1];
                    var value = matchs.Groups[2];
                    vartable[name.Value] = value.Value;
                }
                else
                {
                    foreach(var v in vartable)
                    {
                        line = line.Replace(v.Key, v.Value);
                    }

                    var match = nodedef.Match(line);
                    if(!match.Success)
                    {
                        Logger.Error($"节点定义语法错误: {line}");
                        continue;
                    }

                    var level = match.Groups[1].Value.Length / 2;
                    var name = match.Groups[2].Value;
                    var property = match.Groups[3].Value;

                    var page = -1;
                    if(!string.IsNullOrEmpty(name))
                    {
                        page = QueryPageByName(name);
                        if(page == -1)
                        {
                            Logger.Error($"节点定义错误, 未知的页码名称: {name}");
                            continue;
                        }
                    }

                    PageTreeNode newNode = new PageTreeNode(page, level);
                    if(level > curlevel + 1)
                    {
                        Logger.Error($"节点定义错误, 不能跨级定义孙节点: {line}");
                        continue;
                    }

                    while(level != curlevel + 1 && curnode != null)
                    {
                        curnode = curnode.parent;
                        --curlevel;     
                    }

                    if(curnode == null)
                    {
                        Logger.Error($"节点定义错误, 未能追踪到父节点: {line}");
                        continue;
                    }

                    if(!string.IsNullOrEmpty(property))
                    {
                        string[] pros = property.Split(',');
                        foreach (var pro in pros)
                        {
                            string[] pair = pro.Split(valsep);
                            if (pair.Length != 2)
                            {
                                Logger.Error($"节点定义错误, 属性值未定义: {pro} @{line}");
                                continue;
                            }

                            var pname = pair[0].Trim();
                            var pvale = pair[1].Trim();
                            pvale = GlobalConf.ReplaceAtGramma(pvale);

                            if (!string.IsNullOrEmpty(pname))
                            {
                                newNode.AddProperty(pname, pvale);
                            }
                        }
                    }

                    curnode.AddChild(newNode);
                    curnode = newNode;
                    curlevel = level;
                }  
            }
        }

        #endregion


        #region 页面名称查询
        public int QueryPageByName(string name)
        {
            if(pageList.ContainsKey(name))
            {
                return pageList[name];
            }

            if(!_autoGenPageList)
            {
                Console.WriteLine("****************  ERRROR **********************");
                Console.WriteLine($"*** PAGE { name } NOT DEFAINED **********");
                return -1;
            }

            int index = pageList.Count;
            while(pageList.ContainsValue(index))
            {
                ++index;
            }
            pageList[name] = index;
            return index;
        }

        public string QueryNameByPage(int page)
        {
            if(page < 0 || page >= pageList.Count)
            {
                return "<错误页面>";
            }

            foreach(var item in pageList)
            {
                if(item.Value == page)
                {
                    return item.Key;
                }
            }

            return "<页面数据错误>";
        }

        public PageTreeNode FindPageNode(int pageid)
        {
            if(pageid == -1)
            {
                return null;
            }

            return treeRoot.FindNode(pageid);
        }

        #endregion

        #region 查询页面关系

        public int QueryNextPage(int page)
        {
            int npage = page + 1;
            if(npage >= pageList.Count)
            {
                npage = 0;
            }

            return npage;
        }

        // 根据树节点的配置查找下一页, 而不是通过页面列表中的顺序
        public int FindNextNodeInTree(int pageid)
        {
            bool foundcurpage = false;
            foreach(PageTreeNode page in EnumTreeAsc())
            {
                if(foundcurpage)
                {
                    if(page.pageid != -1)
                    {
                        return page.pageid;
                    }
                }
                else
                {
                    if(page.pageid == pageid)
                    {
                        foundcurpage = true;
                    }
                }
            }

            if(!foundcurpage)
            {
                Logger.Error($"查找下一页错误, 未能找到当前页面 { pageid }");
                return -1;
            }

            foreach (PageTreeNode page in EnumTreeAsc())
            {
                if(page.pageid != -1)
                {
                    return page.pageid;
                }
            }

            return -1;
        }

        // 根据树节点的配置查找上一页, 而不是通过页面列表中的顺序
        public int FindPrevNodeInTree(int pageid)
        {
            bool foundcurpage = false;
            foreach (PageTreeNode page in EnumTreeDesc())
            {
                if (foundcurpage)
                {
                    if (page.pageid != -1)
                    {
                        return page.pageid;
                    }
                }
                else
                {
                    if (page.pageid == pageid)
                    {
                        foundcurpage = true;
                    }
                }
            }

            if (!foundcurpage)
            {
                Logger.Error($"查找上一页错误, 未能找到当前页面 { pageid }");
                return -1;
            }

            foreach (PageTreeNode page in EnumTreeDesc())
            {
                if (page.pageid != -1)
                {
                    return page.pageid;
                }
            }

            return -1;
        }

        // 根据节点树查找同一级的下一个节点
        public int FindNextBrotherInTree(int pageid, bool loop)
        {
            PageTreeNode node = FindPageNode(pageid);
            if(node.parent == null)
            {
                return -1;
            }

            for(int i = 0; i < node.parent.childs.Count; ++i)
            {
                if(node.parent.childs[i] == node)
                {
                    if(i + 1 < node.parent.childs.Count)
                    {
                        return node.parent.childs[i + 1].pageid;
                    }
                    else if(loop)
                    {
                        return node.parent.childs[0].pageid;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            return -1;
        }

        // 根据节点树查找同一级的上一个节点
        public int FindPrevBrotherInTree(int pageid, bool loop)
        {
            PageTreeNode node = FindPageNode(pageid);
            if (node.parent == null)
            {
                return -1;
            }

            for (int i = 0; i < node.parent.childs.Count; ++i)
            {
                if (node.parent.childs[i] == node)
                {
                    if (i > 0)
                    {
                        return node.parent.childs[i - 1].pageid;
                    }
                    else if (loop)
                    {
                        return node.parent.childs[node.parent.childs.Count - 1].pageid;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            return -1;
        }

        // 不跳出指定的层进入下一页
        public int QueryNextPageInSubLevelNoLoop(int page, int level)
        {
            int npage = page + 1;
            if (npage >= pageList.Count)
            {
                npage = -1;
            }

            var node = FindPageNode(npage);
            if(node == null)
            {
                return -1;
            }

            if(node.level < level)
            {
                return -1;
            }

            return npage;
        }

        public int QueryPrevPage(int page)
        {
            int npage = page - 1;
            if(npage < 0)
            {
                npage = pageList.Count - 1;
            }

            return npage;
        }

        public int QueryPrevPageInSubLevelNoLoop(int page, int level)
        {
            int npage = page - 1;
            if (npage < 0)
            {
                return -1;
            }

            var node = FindPageNode(npage);
            if (node == null)
            {
                return -1;
            }

            if (node.level < level)
            {
                return -1;
            }

            return npage;
        }

        public int QueryFirstChild(int page)
        {
            return QueryFirstChild(FindPageNode(page));            
        }

        private int QueryFirstChild(PageTreeNode node)
        {
            if (node == null)
            {
                return -1;
            }

            if (node.childs.Count == 0)
            {
                return -1;
            }

            node = node.childs[0];
            while (node.pageid == -1)
            {
                if (node.childs.Count == 0)
                {
                    return -1;
                }

                node = node.childs[0];
            }

            return node.pageid;
        }

        public int QueryParent(int page)
        {
            PageTreeNode node = FindPageNode(page);
            if(node == null)
            {
                return -1;
            }

            if(node.parent == null)
            {
                return -1;
            }

            node = node.parent;
            while(node.pageid == -1)
            {
                if(node.parent == null)
                {
                    return -1;
                }

                node = node.parent;
            }

            return node.pageid;
        }

        public int QueryBrotherCount(int page)
        {
            PageTreeNode node = FindPageNode(page);
            if(node == null)
            {
                return 0;
            }

            if(node.parent == null)
            {
                return 0;
            }

            return node.parent.childs.Count;
        }

        public int QueryBrotherRank(int page)
        {
            PageTreeNode node = FindPageNode(page);
            if (node == null)
            {
                return 0;
            }

            if (node.parent == null)
            {
                return 0;
            }

            node = node.parent;
            for(int i = 0; i < node.childs.Count; ++i)
            {
                if(node.childs[i].pageid == page)
                {
                    return i;
                }
            }

            return 0;
        }

        public int QueryParentOnLevel(int level, int page)
        {
            PageTreeNode node = FindPageNode(page);
            if (node == null)
            {
                return -1;
            }

            if (node.parent == null)
            {
                return -1;
            }

            node = node.parent;
            while (node.pageid == -1 && node.level > level)
            {
                if (node.parent == null)
                {
                    return -1;
                }

                node = node.parent;
            }

            return node.pageid;
        }

        public int QueryLevelEntry(int level, int child, int page)
        {
            PageTreeNode node = FindPageNode(page);
            if (node == null)
            {
                return -1;
            }

            while(node.level > level)
            {
                node = node.parent;
                if(node == null)
                {
                    return -1;
                }
            }

            while(node.level < level)
            {
                if(node.childs.Count == 0)
                {
                    return -1;
                }

                node = node.childs[0];
            }

            if(node.childs.Count <= child)
            {
                return -1;
            }

            node = node.childs[child];
            if(node.pageid == -1)
            {
                return QueryFirstChild(node);
            }

            return node.pageid;
        }

        #endregion

        #region 遍历

        public void Dump()
        {
            Console.WriteLine("Page Defines:");
            
            foreach(var item in pageList)
            {
                Console.WriteLine($"  {item.Key}: {item.Value}");
            }

            Console.WriteLine("Page Tree:");
            treeRoot.Dump("  ");
        }

        public IEnumerable<PageTreeNode> EnumTreeAsc()
        {
            return treeRoot.EnumTreeAsc();
        }

        public IEnumerable<PageTreeNode> EnumTreeDesc()
        {
            return treeRoot.EnumTreeDesc();
        }

        public IEnumerable<PageTreeNode> EnumChild(PageTreeNode parent)
        {
            if(parent == null)
            {
                return treeRoot.EnumChild();
            }

            return parent.EnumChild();
        }

        #endregion

        private Dictionary<string, int> pageList = new Dictionary<string, int>();
        private PageTreeNode treeRoot = new PageTreeNode(-1, -1);
        private bool _autoGenPageList = false;
    }
}
