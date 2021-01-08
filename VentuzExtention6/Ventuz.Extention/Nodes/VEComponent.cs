using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.CModel;

namespace Ventuz.Extention.Nodes
{
    public abstract class VEComponent : VComponent
    {
        public VEComponent(VSite site, string name)
            : base(site, name)
        {
        }

        public VEComponent(VSite site)
            : base(site)
        {
        }
    }
}
