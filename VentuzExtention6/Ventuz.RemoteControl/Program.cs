using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentuzRemoteControl.Clusters;

namespace VentuzRemoteControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Program progrm = new Program();

            string command = "";
            while(command != "exit" && command != "quit")
            {
                progrm.ProcessCommand(command);
                Console.Write(">>> ");       
                command = Console.ReadLine();
            }
        }

        private void ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;

            if(command == "data")
            {
                cluster.ShowDataMode();
            }
            else if(command == "info")
            {
                cluster.ShowVentuzInfo();
            }
            else if(command == "scene")
            {
                cluster.ListScene("0000");
            }
            else if(command[0] == 's' && command[1] == ' ')
            {
                command = command.Substring(2);
                cluster.SetScene(command);
            }
            else if(command[0] == 'l' && command[1] == ' ')
            {
                command = command.Substring(2);
                cluster.LoadScene(command);
            }
        }

        private Program()
        {
            cluster.Start();
        }

        ~Program()
        {
            cluster.Stop();
        }
        
        private ClusterManager cluster = new ClusterManager();
    }
}
