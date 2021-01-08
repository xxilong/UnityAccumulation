using ShareLib.Protocl;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.QXSandTable
{
    public class QXSTSerialProtocol3 : QXSTSerialProtocol2
    {
        public override bool OnRecvSubCmd(int subcmd, MyMemoryStream stream)
        {
            return base.OnRecvSubCmd(subcmd, stream);
        }
    }
}
