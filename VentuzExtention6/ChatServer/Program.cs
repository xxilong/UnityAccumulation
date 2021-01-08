using ShareLib.Log;
using ShareLib.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        private CmdTcpServer _server = new CmdTcpServer();
        private Dictionary<string,UserStatus> userDic=new Dictionary<string, UserStatus>();
        private string userList;

        static void Main(string[] args)
        {
            new Program().RealMain();            
        }

        private void RealMain()
        {
            _server.SetReciverListener(OnReciverAction);

            _server.Start(1012);
            _server.SetClientCloseListener(OnClientClose);

            while (Console.ReadLine() != "exit")
            {

            }
        }

        private void OnClientClose(string str)
        {
            Logger.Info(str);
            foreach (var item in userDic)
            {
                if (item.Value.ip==str)
                {
                    LoginOut(item.Key);
                }
            }
        }

       

        private void OnReciverAction(string cmd,string peername)
        {
            Logger.Info($"收到客户端 {peername} 命令: {cmd}");
            if (cmd== "CheckUser")
            {
                _server.SendCommandTo("UserList:"+userList, peername);
                return;
            }
            string[] str = cmd.Split('_');


            if (str[1]=="Login")//登入
            {
                if (userDic.ContainsKey(str[0]))//用户名重复
                {
                    _server.SendCommandTo("用户已存在",peername);
                    return;
                }
                else//登入
                {
                    Login(str[0], peername);
                    return;
                }
            }
            else if (str[1] == "LoginOut")//登出
            {
                LoginOut(str[0]);
                return;
            }
            else if(str[1]=="Call" && userDic.ContainsKey(str[2]))//通话呼叫
            {
                _server.SendCommandTo(str[0]+"_CallIn", userDic[str[2]].ip);
                return;
            }
            else if (str[1] == "EndCall" && userDic.ContainsKey(str[2]))//通话结束
            {
                _server.SendCommandTo(str[0] + "_EndCall", userDic[str[2]].ip);
            }            
           
            //_server.SendCommandToOther(cmd, peername);
        }


        private void Login(string user,string ip)
        {
            Logger.Info("用户" + user + "登入");
            userDic.Add(user, new UserStatus(user, ip, true));
            UpdateUserList();
            _server.SendCommandTo(user+"_Login", ip);
            _server.SendCommandToOther(user + "_Login", ip);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="user"></param>
        private void LoginOut(string user)
        {
            _server.SendCommandToOther(user+"_LoginOut", userDic[user].ip);
            userDic.Remove(user);
            UpdateUserList();
        }

        /// <summary>
        /// 更新用户列表
        /// </summary>
        /// <param name="str"></param>
        private void UpdateUserList()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in userDic)
            {
                builder.Append(item.Key + ",");
            }
            userList = builder.ToString();
            Logger.Info(userList);
        }
    }
}
