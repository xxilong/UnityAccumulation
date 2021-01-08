using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Ventuz.Extention.Conf
{
    public class XmlConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defns">仅被用于创建节点时</param>
        public XmlConfig(string path, string defns = "")
        {
            _cfgFile = path;
            _cfgDoc.Load(_cfgFile);
            _defns = defns;
            _ns = new XmlNamespaceManager(_cfgDoc.NameTable);
        }

        public void Save()
        {
            _cfgDoc.Save(_cfgFile);
        }

        public XmlNode CreateElement(XmlNode root, string name, string inner = "")
        {
            XmlNode node = Doc.CreateNode(XmlNodeType.Element, name, _defns);
            node.InnerText = inner;
            root.AppendChild(node);
            return node;
        }

        public XmlAttribute CreateAttribute(XmlNode root, string name, string value)
        {
            XmlAttribute attr = Doc.CreateAttribute(name);
            attr.Value = value;
            root.Attributes.Append(attr);
            return attr;
        }

        public void SetNodeText(string xpath, string value)
        {
            try
            {
                XmlNode node = _cfgDoc.SelectSingleNode(xpath);
                if(node.InnerText == value)
                {
                    return;
                }

                node.InnerText = value;
            }
            catch(XPathException e)
            {
                Logger.Error($"SetNodeText Exception: {e}");
            }

            Save();
        }

        public string GetNodeText(string xpath)
        {
            try
            {
                XmlNode node = _cfgDoc.SelectSingleNode(xpath);
                if(node != null)
                {
                    return node.InnerText;
                }
            }
            catch (XPathException e)
            {
                Logger.Error($"GetNodeText Exception: {e}");
            }

            return string.Empty;
        }

        public XmlDocument Doc { get => _cfgDoc; }
        public XmlNamespaceManager NS { get => _ns; }
        private string _cfgFile;
        private string _defns;
        private XmlDocument _cfgDoc = new XmlDocument();
        private XmlNamespaceManager _ns = null;
    }
}
