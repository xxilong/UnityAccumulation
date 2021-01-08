using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadControlCenter
{
    class PowerCtrlServer
    {
        private PageControlServer _server = new PageControlServer();
        

        public void Main()
        {
            int port = GlobalConf.getconf<int>("poweronly", "port", 3131);

            _server.SetSrvCtrlListener(HandleServerCtrlCommand);
            _server.Start(port);
            while (true)
            {
                string cmdline = Console.ReadLine();
                _server.SendCommand(cmdline);
            }
        }

        private void HandleServerCtrlCommand(string cmd, string clientname)
        {
            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#shutdown":
                    Machine.shutdown();
                    break;

                default:
                    break;
            }
        }
    }
}
