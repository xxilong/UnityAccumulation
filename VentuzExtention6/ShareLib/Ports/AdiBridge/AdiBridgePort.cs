using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.AdiBridge
{
    public class AdiBridgePort : SerialPort
    {
        public AdiBridgePort() : base(new AdiBridgeProtocol())
        {
            _procol = (AdiBridgeProtocol)base.Protocol;
        }

        public AdiBridgeProtocol _procol;
    }
}
