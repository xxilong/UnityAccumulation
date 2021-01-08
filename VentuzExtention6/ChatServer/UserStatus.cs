using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    struct UserStatus
    {
        public String name;
        public bool online;
        public String ip;
        public UserStatus(String name,String ip,bool online=false)
        {
            this.name = name;
            this.ip = ip;
            this.online = online;
        }
    }
}
