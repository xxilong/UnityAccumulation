using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Ventuz.Extention.Resource;

namespace Ventuz.Extention.Conf
{
    public class VentuzAVConfig : XmlConfig
    {
        public class OutStream
        {
            public OutStream(XmlConfig cfg, XmlNode root)
            {
                _cfg = cfg;
                _root = root;
            }

            public void SetScreen(int order)
            {
                string idval = $"DX_{order}";
                XmlNode idNode1 = _root.SelectSingleNode("@ID");
                if(idNode1.Value == idval)
                {
                    return;
                }

                XmlNode idNode2 = _cfg.Doc.SelectSingleNode("/av:AVConfig/av:VideoOutputs/av:Stream/@ID", _cfg.NS);
                idNode1.Value = idval;
                idNode2.Value = idval;
                _cfg.Save();
            }

            public void ShowCursor(bool show)
            {
                XmlNode sp = _root.SelectSingleNode("av:ShowPointer", _cfg.NS);
                sp.InnerText = show ? "true" : "false";
                _cfg.Save();
            }

            private XmlConfig _cfg;
            private XmlNode _root;
        }
        public class InStreams
        {
            private string ns = "http://www.ventuz.com/AVConfigFile/1.0/";
            public InStreams(XmlConfig cfg)
            {
                _cfg = cfg;
            }

            public void AddInputVideo(string searid)
            {
                string boardid = SeaialID2BoardID(searid);
                string streamid = BoardID2StreamID(boardid);

                Thread.Sleep(1);

                XmlNode rootNode = _cfg.Doc.SelectSingleNode("/av:AVConfig", _cfg.NS);
                XmlNode videoInput = _cfg.Doc.SelectSingleNode("/av:AVConfig/av:VideoInputs", _cfg.NS);

                XmlNode board = _cfg.Doc.CreateNode(XmlNodeType.Element, "Board", ns);
                _cfg.CreateAttribute(board, "ID", boardid);
                _cfg.CreateAttribute(board, "Name", "Camera#" + Environment.TickCount);
                _cfg.CreateElement(board, "VerifyFrameRate", "true");
                rootNode.InsertBefore(board, videoInput);

                XmlNode stm = _cfg.Doc.CreateNode(XmlNodeType.Element, "Stream", ns);
                _cfg.CreateAttribute(stm, "ID", streamid);
                _cfg.CreateElement(stm, "CustomFormat", "AutoDetect");
                _cfg.CreateElement(stm, "Format", "AutoDetect");
                _cfg.CreateElement(stm, "RGBFormat", "8");
                _cfg.CreateElement(stm, "Mipmaps", "false");
                _cfg.CreateElement(stm, "StartInError", "true");
                _cfg.CreateElement(stm, "Synchronized", "false");
                _cfg.CreateElement(stm, "LowLatency", "false");
                _cfg.CreateElement(stm, "ExtraBuffers", "0");
                _cfg.CreateElement(stm, "DisabledContent", "LastFrame");
                _cfg.CreateElement(stm, "AutoDisable", "false");
                _cfg.CreateElement(stm, "FlipVertically", "false");
                rootNode.InsertAfter(stm, board);

                _cfg.CreateAttribute(
                    _cfg.CreateElement(videoInput, "Stream"),
                    "ID", streamid);

                _cfg.Save();
            }

            private static string SeaialID2BoardID(string searialid)
            {
                if(searialid.StartsWith("WMF_"))
                {
                    if(searialid.EndsWith("_INP_in_Input_"))
                    {
                        return searialid.Substring(0, searialid.Length - 14);
                    }

                    return searialid;
                }

                if(!searialid.StartsWith(@"\\?"))
                {
                    Logger.Error($"错误的摄像头ID: {searialid}");
                    return string.Empty;
                }

                string repstr = searialid.Replace(@"\\?\", "WMF_");
                repstr = repstr.Replace("#", "");
                repstr = repstr.Replace("\\", "");
                repstr = repstr.Replace("&", "");
                repstr = repstr.Replace("{", "");
                repstr = repstr.Replace("}", "");
                repstr = repstr.Replace("-", "");
                return repstr;
            }

            private static string BoardID2StreamID(string boardid)
            {
                return boardid + "_INP_in_Input_";
            }

            private XmlConfig _cfg;
        }

        public VentuzAVConfig(string name)
            : base(InitAVConfig(name), "http://www.ventuz.com/AVConfigFile/1.0/")
        {
            NS.AddNamespace("av", "http://www.ventuz.com/AVConfigFile/1.0/");
        }

        public OutStream RenderOut
        {
            get
            {
                XmlNode curOutID = Doc.SelectSingleNode("/av:AVConfig/av:VideoOutputs/av:Stream/@ID", NS);
                return new OutStream(this, Doc.SelectSingleNode($"/av:AVConfig/av:Stream[@ID=\"{curOutID.Value}\"]", NS));
            }
        }

        public InStreams VideoIn
        {
            get
            {
                if(_instreamInstance == null)
                {
                    _instreamInstance = new InStreams(this);
                }

                return _instreamInstance;
            }
        }

        private InStreams _instreamInstance = null;

        private static string InitAVConfig(string name)
        {
            string avpath = Path.Combine(FilePaths.pakCfgDir, "AVConfig", name + ".avc");
            if(!File.Exists(avpath))
            {
                File.WriteAllText(avpath, ExtentionResource.GetTextContent("InitAVSetup"));
            }
            return avpath;
        }

        private static string InitTestConfig(string name)
        {
            string avpath = Path.Combine(PathHelp.appDir, name + ".avc");
            File.WriteAllText(avpath, ExtentionResource.GetTextContent("InitAVSetup"));
            return avpath;
        }
    }
}
