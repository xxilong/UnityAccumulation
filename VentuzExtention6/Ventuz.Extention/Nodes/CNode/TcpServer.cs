using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.DeviceIO.Network;
using Ventuz.Kernel;
using Ventuz.Kernel.CModel;

namespace Ventuz.Extention.Nodes.CNodes
{
    [MetaData]
    public class TcpServer : NetworkBase
    {
        public TcpServer(VSite site, string name) :
            base(site, name)
        {
        }

        protected override BGroup InputGroups => BGroup.A | BGroup.B | BGroup.C;
    }
}
