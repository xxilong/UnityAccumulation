using System;
using System.Net;
using System.Net.Sockets;

namespace ShareLib.Unity
{
    public class NetUnity
    {
        public static string GetLocalIPs()
        {
            try
            {
                string ipAddres = "";

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型

                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        if(ipAddres != "")
                        {
                            ipAddres += ", ";
                        }

                        ipAddres += IpEntry.AddressList[i].ToString();
                    }
                }

                return ipAddres;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
