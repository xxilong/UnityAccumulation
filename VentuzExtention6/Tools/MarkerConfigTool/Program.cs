using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Conf;
using Ventuz.Extention.Detector;

namespace MarkerConfigTool
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MarkerConfig> markers = new List<MarkerConfig>();
            BinaryFormatter binFormat = new BinaryFormatter();
            try
            {
                using (Stream fStream = new FileStream("markers.data", FileMode.Open, FileAccess.Read))
                {
                    markers = (List<MarkerConfig>)binFormat.Deserialize(fStream);
                    Console.WriteLine("加载了 {0} 个 Marker 配置项", markers.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                return;
            }

            DoModify(markers);

            BinaryFormatter binFormat2 = new BinaryFormatter();
            using (Stream fStream = new FileStream("markers_new.data", FileMode.Create, FileAccess.Write))
            {
                binFormat2.Serialize(fStream, markers);
            }
            Console.WriteLine("Marker 配置已保存");
        }

        static void DoModify(List<MarkerConfig> markers)
        {
            while(true)
            {
                for (int i = 0; i < markers.Count; ++i)
                {
                    Console.WriteLine($"{i}: {markers[i].name}");
                }

                Console.WriteLine(" *  -1: 交换Marker位置               0-n: 修改对应编号的Marker");
                Console.WriteLine(" * ayz: 分析Marker之间的混淆情况     exit/quit: 保存并退出");
                Console.Write(" > ");

                string cmd = Console.ReadLine();
                if(cmd == "exit" || cmd == "quit")
                {
                    break;
                }
                else if(cmd == "ayz")
                {
                    ShowAyz(markers);
                    continue;
                }

                if(!int.TryParse(cmd, out int sel))
                {
                    continue;
                }

                if(sel == -1)
                {
                    Console.WriteLine("Swap Item: ");
                    string input = Console.ReadLine();
                    string[] inputs = input.Split(new char[] { ' ', '\t' });
                    int index1 = 0, index2 = 0;
                    int.TryParse(inputs[0], out index1);
                    int.TryParse(inputs[1], out index2);

                    MarkerConfig cfg1 = markers[index1];
                    markers[index1] = markers[index2];
                    markers[index2] = cfg1;
                    continue;
                }

                DoModifyMarker(markers[sel]);
            }
        }

        static void DoModifyMarker(MarkerConfig cfg)
        {
            Console.WriteLine($"DIS12: {cfg.dis12} err: {cfg.erdis12}");
            Console.WriteLine($"DIS13: {cfg.dis13} err: {cfg.erdis13}");
            Console.WriteLine($"DIS23: {cfg.dis23} err: {cfg.erdis23}");

            Console.Write("Modify Dis12: ");
            string val = Console.ReadLine();
            if(val != "")
            {
                double.TryParse(val, out cfg.erdis12); 
            }

            Console.Write("Modify Dis13: ");
            val = Console.ReadLine();
            if (val != "")
            {
                double.TryParse(val, out cfg.erdis13);
            }

            Console.Write("Modify Dis23: ");
            val = Console.ReadLine();
            if (val != "")
            {
                double.TryParse(val, out cfg.erdis23);
            }

            Console.WriteLine($"DIS12: {cfg.dis12} err: {cfg.erdis12}");
            Console.WriteLine($"DIS13: {cfg.dis13} err: {cfg.erdis13}");
            Console.WriteLine($"DIS23: {cfg.dis23} err: {cfg.erdis23}");
        }

        static void ShowAyz(List<MarkerConfig> markers)
        {
            Console.WriteLine($"序号\t名称\t长边\t次长边\t短边");

            for (int i = 0; i < markers.Count; ++i)
            {
                MarkerConfig m = markers[i];
                Console.WriteLine($"{i}\t{m.name}\t{m.dis12-m.erdis12}-{m.dis12+m.erdis12}\t{m.dis13-m.erdis13}-{m.dis13+m.erdis13}\t{m.dis23-m.erdis23}-{m.dis23+m.erdis23}");
            }
        }
    }
}
