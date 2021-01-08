using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;

namespace ShareLib.Page
{
    public struct PageChangeArg
    {
        public int fromPage;
        public int toPage;
    }

    public struct PropertyChangeArg
    {
        public string fromval;
        public string toval;
    }

    public class PropertyChangeEvent
    {
        public event EventHandler<PropertyChangeArg> Change;
        public void fireEvent(string oldval, string newval)
        {
            Change?.Invoke(this, new PropertyChangeArg { fromval = oldval, toval = newval });
        }
    }

    public class PropertyMonitor
    {
        public PropertyMonitor(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public void OnChange(object sender, PropertyChangeArg arg)
        {
            if(arg.toval == value)
            {
                Enter?.Invoke();
            }
            else
            {
                Leave?.Invoke();
            }
        }

        public Action Enter = null;
        public Action Leave = null;

        private string name;
        private string value;
    }

    public class SinglePageMonitor
    {
        public SinglePageMonitor(int page)
        {
            this.page = page;
        }

        public void OnChanged(object sender, int arg)
        {
            if(arg == page)
            {
                Enter?.Invoke();
            }
            else
            {
                Leave?.Invoke();
            }
        }

        public Action Enter = null;
        public Action Leave = null;

        private int page;
    }

    public class Page
    {
        public static Page Control = new Page();

        #region 初始化
        // 由脚本传入页面的配置进行初始化
        public void InitPages(string pagelist, string pagetree)
        {
            pages.ParsePageList(pagelist);
            pages.ParsePageTree(pagetree);
            curproperty = new Dictionary<string, string>(PageTreeNode.allPropertyDefault);
        }

        public void InitPages(Type pageEnumType, string pagetree)
        {
            pages.LoadPageList(pageEnumType);
            pages.ParsePageTree(pagetree);
            curproperty = new Dictionary<string, string>(PageTreeNode.allPropertyDefault);
        }

        public void InitPages(string pagetree)
        {
            pages.AutoGenPage = true;
            pages.ParsePageTree(pagetree);            
            pages.AutoGenPage = false;
            curproperty = new Dictionary<string, string>(PageTreeNode.allPropertyDefault);
            Logger.Info($"Generated {pages.PageCount} pages");
        }
        #endregion

        #region 事件通知
        public event EventHandler<PageChangeArg> OnChangePage;  // 换页前
        public event EventHandler<int> OnPageChanged;           // 换页后
        public event EventHandler<int> OnPageReload;            // 在当前页时重新加载当前页面

        public PropertyChangeEvent OnProperty(string name)      // 属性变化
        {
            if(!propertyChangeNotifys.ContainsKey(name))
            {
                propertyChangeNotifys[name] = new PropertyChangeEvent();
            }

            return propertyChangeNotifys[name];
        }

        public PropertyMonitor MProperty(string name, string value)
        {
            PropertyMonitor m = new PropertyMonitor(name, value);
            OnProperty(name).Change += m.OnChange;
            return m;
        }

        public void MProperty(string name, string value, Action enter, Action leave = null)
        {
            PropertyMonitor m = MProperty(name, value);
            m.Enter = enter;
            m.Leave = leave;
        }

        public void MPage(string name, Action enter, Action leave = null)
        {
            int page = pages.QueryPageByName(name);
            if(page == -1)
            {
                Logger.Error($"MPage: page {name} is not exist.");
                return;
            }

            SinglePageMonitor p = new SinglePageMonitor(page);
            p.Enter = enter;
            p.Leave = leave;
            OnPageChanged += p.OnChanged;
        }

        #endregion

        #region 换页函数

        public void SetProperty(string name, string val)
        {
            if(!extproperty.ContainsKey(name))
            {
                OnProperty(name).fireEvent("", val);
                extproperty[name] = val;
                return;
            }

            string oldval = extproperty[name];
            if(oldval != val)
            {
                OnProperty(name).fireEvent(oldval, val);
                extproperty[name] = val;
            }
        }

        public void SetTrigger(string name)
        {
            trigproperty[name] = true;
        }

        public bool IsTriggered(string name)
        {
            if(trigproperty.ContainsKey(name))
            {
                bool ret = trigproperty[name];
                trigproperty[name] = false;
                return ret;
            }

            return false;
        }

        public string GetProperty(string name)
        {
            if(curproperty.ContainsKey(name))
            {
                return curproperty[name];
            }

            if(extproperty.ContainsKey(name))
            {
                return extproperty[name];
            }

            return "";
        }

        // 跳转到指定名字的页面
        public void GotoPage(string pageName)
        {
            int page = pages.QueryPageByName(pageName);
            if(page == -1)
            {
                Console.WriteLine($"ERROR: 跳转的页面不存在 { pageName }");
                return;
            }

            GotoPage(page);
        }

        // 如果当前在 ifPage 则跳转到 toPage
        // 否则跳转到 ifPage
        public void GotoPage(string ifPage, string toPage)
        {
            int p1 = pages.QueryPageByName(ifPage);
            int p2 = pages.QueryPageByName(toPage);

            if(p1 == -1 || p2 == -1)
            {
                Console.WriteLine($"ERROR: 跳转的页面不存在 { ifPage } 或 { toPage }");
                return;
            }

            if(curpage == p1)
            {
                GotoPage(p2);
            }
            else
            {
                GotoPage(p1);
            }
        }

        // 跳转到下一页, 会跨越层次, 循环(最后一页的下一页会回到第一页)
        public void GoNextPage() => SwitchPageByFunc(pages.QueryNextPage);

        // 跳转到下一页, 不能越过层次 level, 不循环
        public void GoNextPage(int level) =>
            SwitchPageByFunc((page)=> pages.QueryNextPageInSubLevelNoLoop(page, level));

        // 跳转的下一页,根据在树种定义的顺序,而不是页面编号的顺序
        public void GoNextPageByTree() => SwitchPageByFunc(pages.FindNextNodeInTree);

        public void GoNextInLevelByTree(bool loop) => 
            SwitchPageByFunc((page)=> pages.FindNextBrotherInTree(page, loop));

        public void GoPrevInLevelByTree(bool loop) =>
            SwitchPageByFunc((page) => pages.FindPrevBrotherInTree(page, loop));

        // 回到上一页, 会跨越层次, 循环
        public void GoPrevPage() => SwitchPageByFunc(pages.QueryPrevPage);

        // 上一页, 不能越过层次 level, 不循环
        public void GoPrevPage(int level) =>
            SwitchPageByFunc((page) => pages.QueryPrevPageInSubLevelNoLoop(page, level));

        public void GoPrevPageByTree() => SwitchPageByFunc(pages.FindPrevNodeInTree);

        // 进入当前页面的第一个子页面
        public void EnterFirstChild() => SwitchPageByFunc(pages.QueryFirstChild);
        
        // 回到当前页面的第一层父页面
        public void BackToParent() => SwitchPageByFunc(pages.QueryParent);

        // 一直往上回退到父页面, 直到达到指定的层次
        public void BackToLevel(int level) =>
            SwitchPageByFunc((page) => pages.QueryParentOnLevel(level, page));        

        // 先回到第 level 层的父节点, 然后在进入它的第 order 子个节点页面, order 从 0 开始编号的
        public void GotoLevelEntry(int level, int order) =>
            SwitchPageByFunc((page) => pages.QueryLevelEntry(level, order, page));

        #endregion

        #region 页面属性和调试
        // 获取页面同级菜单页面数量
        public int GetBrotherCount(int page)
        {
            return pages.QueryBrotherCount(page);
        }

        public int GetCurBrotherCount()
        {
            return pages.QueryBrotherCount(curpage);
        }

        // 获取页面在同级菜单中的位置, 从 0 开始编号
        public int GetBrotherRank(int page)
        {
            return pages.QueryBrotherRank(page);
        }

        public int GetCurBrotherRank()
        {
            return pages.QueryBrotherRank(curpage);
        }

        // 通过内部的页码编号获取页面名称
        public string GetPageName(int page)
        {
            return pages.QueryNameByPage(page);
        }

        public string GetCurPageName()
        {
            return GetPageName(curpage);
        }

        public int CurPage { get { return this.curpage; } }

        #endregion

        public void GotoPage(int page)
        {
            if(page == curpage)
            {
                OnPageReload?.Invoke(this, curpage);
                return;
            }

            Logger.Info($"Begin GotoPage {curpage}->{page}");
            OnChangePage?.Invoke(this, new PageChangeArg { fromPage = curpage, toPage = page });

            PageTreeNode node = pages.FindPageNode(page);
            if (node == null)
            {
                Console.WriteLine($"ERROR: 跳转的页面未在节点树中进行定义 { page }");
                return;
            }

            var changed = new Dictionary<string, string>();
            foreach (var property in curproperty)
            {
                var key = property.Key;
                var val = property.Value;
                var newval = node.GetProperty(key);
                if(newval == "***")
                {
                    continue;
                }

                if (newval != val)
                {
                    //Console.WriteLine($"property <{key}> change: {val} -> {newval}");
                    OnProperty(key).fireEvent(val, newval);
                    changed[key] = newval;
                }
            }

            foreach (var property in changed)
            {
                curproperty[property.Key] = property.Value;
            }

            curpage = page;
            OnPageChanged?.Invoke(this, curpage);
        }
        private void SwitchPageByFunc(Func<int, int> compute)
        {
            if (curpage == -1)
            {
                return;
            }

            int page = compute(curpage);
            if (page == -1)
            {
                return;
            }

            GotoPage(page);
        }

        public PageInfo Info { get { return pages; } }

        private PageInfo pages = new PageInfo();
        private Dictionary<string, string> curproperty = new Dictionary<string, string>();
        private Dictionary<string, string> extproperty = new Dictionary<string, string>();
        private Dictionary<string, bool> trigproperty = new Dictionary<string, bool>();
        private Dictionary<string, PropertyChangeEvent> propertyChangeNotifys = new Dictionary<string, PropertyChangeEvent>();
        private int curpage = -1;
    }
}
