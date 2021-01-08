using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareLib.Page
{
    public class MenuControl
    {
        static public MenuControl Instance = new MenuControl();

        public void ParseMenuTree(string treedesc)
        {
            _fastLookTable.Clear();
            _root = new MenuNode("");
            _fastLookTable[_root.Id] = _root;

            Regex vardef = new Regex(@"\+(\S+)[=:]\((.+)\)");
            Regex nodedef = new Regex(@"(-*)(\w*)(?:\((.+)\))?");
            char[] valsep = new char[] { '=', ':' };

            var vartable = new Dictionary<string, string>();

            int curlevel = -1;
            MenuNode curParent = _root;

            string[] lines = treedesc.Split('\n');
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

                if (line.StartsWith("+"))
                {
                    var matchs = vardef.Match(line);
                    if (!matchs.Success)
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
                    foreach (var v in vartable)
                    {
                        line = line.Replace(v.Key, v.Value);
                    }

                    var match = nodedef.Match(line);
                    if (!match.Success)
                    {
                        Logger.Error($"节点定义语法错误: {line}");
                        continue;
                    }

                    var level = match.Groups[1].Value.Length / 2;
                    var name = match.Groups[2].Value;
                    var property = match.Groups[3].Value;

 
                    if (string.IsNullOrEmpty(name))
                    {
                        Logger.Error($"节点定义错误, 不运行为空的菜单项: {line}");
                        continue;   
                    }
                    
                    if (level > curlevel + 1)
                    {
                        Logger.Error($"节点定义错误, 不能跨级定义孙节点: {line}");
                        continue;
                    }

                    MenuNode newNode = new MenuNode(name);
                    while (level != curlevel + 1 && curParent != null)
                    {
                        curParent = curParent.Parent;
                        --curlevel;
                    }

                    if (curParent == null)
                    {
                        Logger.Error($"节点定义错误, 未能追踪到父节点: {line}");
                        continue;
                    }

                    if (!string.IsNullOrEmpty(property))
                    {
                        string[] pros = property.Split(',');
                        foreach (var pro in pros)
                        {
                            string[] pair = pro.Split(valsep);
                            if (pair.Length != 2)
                            {
                                Console.WriteLine($"节点定义错误, 属性值未定义: {pro} @{line}");
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

                    curParent.AddChild(newNode);
                    curParent = newNode;
                    curlevel = level;
                    _fastLookTable[newNode.Id] = newNode;
                }
            }

            OnMenuLoaded?.Invoke(this, _root);
        }

        public void SelectMenu(string menuid)
        {
            MenuNode nowSel = GetMenu(menuid);
            if(nowSel == null)
            {
                return;
            }

            if(nowSel == _curSel)
            {
                return;
            }

            List<MenuNode> toUnSel = new List<MenuNode>();
            List<MenuNode> toSel = new List<MenuNode>();
            
            MenuNode lookUp = _curSel;
            while(lookUp != null)
            {
                toUnSel.Add(lookUp);
                lookUp = lookUp.Parent;
            }

            lookUp = nowSel;
            while(lookUp != null)
            {
                toSel.Add(lookUp);
                lookUp = lookUp.Parent;
            }

            foreach(MenuNode unSel in toUnSel)
            {
                if(!toSel.Contains(unSel))
                {
                    unSel.IsSelected = false;
                }
            }

            for(int i = toSel.Count - 1; i >= 0; --i)
            {
                toSel[i].IsSelected = true;
            }

            _curSel = nowSel;
        }

        public void UnSelectAll()
        {
            foreach(var item in _fastLookTable)
            {
                item.Value.IsSelected = false;
            }
        }

        public MenuNode GetMenu(string menuid)
        {
            if(string.IsNullOrEmpty(menuid))
            {
                return null;
            }

            if(_fastLookTable.ContainsKey(menuid))
            {
                return _fastLookTable[menuid];
            }

            return null;
        }
        public MenuNode RootMenu { get => _root; }

        public EventHandler<MenuNode> OnMenuLoaded;

        private MenuNode _root = null;
        private MenuNode _curSel = null;
        private Dictionary<string, MenuNode> _fastLookTable = new Dictionary<string, MenuNode>();
    }
}
