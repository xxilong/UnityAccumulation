using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using System;
using System.Net;

namespace PhotoPrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            //printer.PrintImage("http://expo2019.10086.devotedigital.cn/media/photos/3a25b2d0636908f4_1107377297.jpg", (string res) => { Console.WriteLine("ok"); });
            lisSrv.SetReciverListener(OnRecvCommand);
            lisSrv.Start(GlobalConf.getconf<int>("server", "listen"));

            while (true)
            {
                Console.ReadLine();
            }
        }

        static void OnRecvCommand(string cmd, string client)
        {
            Logger.Info($"收到{client}命令: {cmd}");
            string ordercode = cmd.Substring(0, 16);
            string filename = cmd.Substring(16);
            if(ordercode != filename.Substring(0, 16))
            {
                lisSrv.SendCommandTo($"照片错误, 选择的照片不属于此预约单", client);
                return;
            }

            DoPrint(ordercode, filename, client);
        }

        static async void DoPrint(string ordercode, string filename, string client)
        {
            string chkres = await webClient.DownloadStringTaskAsync(webHost + "/prtadmin/check?code=" + ordercode);
            Logger.Info($"{ordercode} Check Result: {chkres}");

            if(chkres != "0")
            {
                Logger.Info($"预约单检查失败: {chkres}");
                lisSrv.SendCommandTo($"预约单检查失败: {chkres}", client);
                return;
            }

            try
            {
                string[] files = filename.Split(spechar);
                object lck = new object();
                int count = 0;
                int finished = 0;
                foreach(string file in files)
                {
                    if(string.IsNullOrEmpty(file))
                    {
                        continue;
                    }

                    ++count;
                }

                foreach (string file in files)
                {
                    if (string.IsNullOrEmpty(file))
                    {
                        continue;
                    }
                    await printer.PrintImage(webHost + "/media/photos/" + file + ".jpg", (string res) =>
                    {
                        DoPrintResult(res, ordercode, client, lck, count, ref finished);
                    });
                }

            }
            catch(Exception e)
            {
                Logger.Info($"打印异常: {e}");
                lisSrv.SendCommandTo($"打印异常: {e}", client);
                return;
            }
        }

        static void DoPrintResult(string res, string code, string client, object lck, int count, ref int finisehd)
        {
            lock(lck)
            {
                Logger.Info($"打印完成: {res}");
                ++finisehd;
                if (res == "ok")
                {
                    try
                    {
                        webClient.DownloadString(webHost + "/prtadmin/setprint?code=" + code);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (count == finisehd)
                {
                    lisSrv.SendCommandTo(res, client);
                }
            }
        }

        static PrintImgURL printer = new PrintImgURL();
        static CmdTcpServer lisSrv = new CmdTcpServer();
        static string webHost = GlobalConf.getconf("server", "host", "http://expo2019.10086.devotedigital.cn");
        static WebClient webClient = new WebClient();
        static char[] spechar = new char[] { ',' };
    }
}
