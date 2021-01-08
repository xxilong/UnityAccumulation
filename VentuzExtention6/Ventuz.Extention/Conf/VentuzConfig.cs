using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ventuz.Extention.Conf
{
    public class VentuzConfig : XmlConfig
    {
        public static VentuzConfig instance = new VentuzConfig();

        private VentuzConfig()
            : base(Path.Combine(FilePaths.pakCfgDir, "VentuzConfig.vcfg"))
        {
        }

        public string Name
        {
            get => GetNodeText("/VentuzConfig/Name");            
            set => SetNodeText("/VentuzConfig/Name", value);
        }

        public string MachineConfigName
        {
            get => GetNodeText("/VentuzConfig/MachineConfig");
            set => SetNodeText("/VentuzConfig/MachineConfig", value);
        }

        public string AVConfigName
        {
            get => GetNodeText("/VentuzConfig/AVConfig");
            set => SetNodeText("/VentuzConfig/AVConfig", value);
        }

        public string RenderSetupName
        {
            get => GetNodeText("/VentuzConfig/RenderSetup");
            set => SetNodeText("/VentuzConfig/RenderSetup", value);
        }

        public VentuzAVConfig AVConfig
        {
            get
            {
                string name = AVConfigName;
                if(string.IsNullOrEmpty(name))
                {
                    AVConfigName = name = "New Configuration";                    
                }

                return new VentuzAVConfig(name);
            }
        }

        public VentuzMachineConfig MachineConfig
        {
            get
            {
                string name = MachineConfigName;
                if (string.IsNullOrEmpty(name))
                {
                    MachineConfigName = name = "New Configuration";
                }

                return new VentuzMachineConfig(name);
            }
        }

        public VentuzRenderConfig RenderConfig
        {
            get
            {
                string name = RenderSetupName;
                if (string.IsNullOrEmpty(name))
                {
                    RenderSetupName = name = "New Configuration";
                }

                return new VentuzRenderConfig(name);
            }
        }
    }
}
