using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Ventuz.Extention.Conf;

namespace Ventuz.Extention.Detector
{
    public class MarkerConfigManager
    {
        public static MarkerConfigManager instance = new MarkerConfigManager();

        public MarkerConfigManager()
        {
            LoadConfigs();
        }

        public MarkerConfig NewConfig()
        {
            MarkerConfig nf = new MarkerConfig();
            markers.Add(nf);
            return nf;
        }
        
        public MarkerConfig GetConfigByName(string name)
        {
            foreach(var cf in markers)
            {
                if(cf.name == name)
                {
                    return cf;
                }
            }

            return null;
        }

        public List<MarkerConfig> GetValidConfigs()
        {
            var valieds = new List<MarkerConfig>();

            foreach(var cf in markers)
            {
                if(cf.valid())
                {
                    valieds.Add(cf);
                }
            }

            return valieds;
        }

        public List<MarkerConfig> GetAllConfigs()
        {
            return markers;
        }

        public double GetMaxDistance()
        {
            double maxdis = -1;

            foreach (var cf in markers)
            {
                double dis = cf.dis12 + cf.erdis12;
                if(dis > maxdis)
                {
                    maxdis = dis;
                }
            }

            return maxdis;
        }
        
        public bool IsModifyed()
        {
            return modifyed;
        }

        public void ModifyedConfig()
        {
            modifyed = true;
        }

        public void ModifyHandled()
        {
            modifyed = false;
        }

        public void SaveConfigs()
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = new FileStream(FilePaths.MarkerConfigPath, FileMode.Create, FileAccess.Write))
            {
                binFormat.Serialize(fStream, markers);
            }

            Console.WriteLine("Marker 配置已保存");
        }

        public void LoadConfigs()
        {
            markers.Clear();

            BinaryFormatter binFormat = new BinaryFormatter();
            try
            {
                using (Stream fStream = new FileStream(FilePaths.MarkerConfigPath, FileMode.Open, FileAccess.Read))
                {
                    markers = (List<MarkerConfig>)binFormat.Deserialize(fStream);
                    Console.WriteLine("加载了 {0} 个 Marker 配置项", markers.Count);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private List<MarkerConfig> markers = new List<MarkerConfig>();
        private bool modifyed = false;
    }
}
