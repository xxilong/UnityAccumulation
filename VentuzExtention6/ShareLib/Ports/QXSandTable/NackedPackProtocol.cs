using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.QXSandTable
{
    public class NackedPackProtocol : QXSTSerialProtocol
    {
        public override byte[] MakeRawData(string hexstr)
        {
            return StrToToHexByte(hexstr);
        }
    }
}
