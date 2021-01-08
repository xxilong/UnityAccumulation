using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;
using ShareLib.Log;

namespace WebCache
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RealMain(args);
        }

        void RealMain(string[] args)
        {
            foreach(string arg in args)
            {
                if(arg == "-noinc")
                {
                    _addcache = false;
                }
            }

            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8090, true);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;     
            proxyServer.Start();
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
            while(true)
            {
                Console.ReadLine();
            }
        }

        private ProxyServer proxyServer = new ProxyServer();
        private string _cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
        private bool _addcache = true;

        private async Task OnRequest(object sender, SessionEventArgs e)
        {     
            string url = e.HttpClient.Request.Url;
            string filename = UrlToFileName(url);

            if (url.Contains(".qq.com") || url.Contains(".microsoft.com") ||
                url.Contains(".google.com") || url.Contains(".googleapis.com"))
            {
                return;
            }

            string cachePath = Path.Combine(_cacheDir, filename);
            if(File.Exists(cachePath))
            {
                Logger.Debug($"USE CACHE: {filename}");
                e.Ok(File.ReadAllBytes(cachePath));
            }
            else
            {
                Logger.Warning($"MISS CACHE: {filename}");
            }
        }

        private async Task OnResponse(object sender, SessionEventArgs e)
        {
            if(!_addcache)
            {
                return;
            }

            if (e.HttpClient.Request.Method != "GET" && e.HttpClient.Request.Method != "POST")
            {
                return;
            }

            if (e.HttpClient.Response.StatusCode != 200)
            {
                return;
            }

            string url = e.HttpClient.Request.Url;
            string filename = UrlToFileName(url);

            if (url.Contains(".qq.com") || url.Contains(".microsoft.com") ||
                url.Contains(".google.com") || url.Contains(".googleapis.com"))
            {
                return;
            }


            string cachePath = Path.Combine(_cacheDir, filename);
            if (!File.Exists(cachePath))
            {
                Logger.Info($"ADD CACHE: {filename}");
                byte[] body = await e.GetResponseBody();
                File.WriteAllBytes(cachePath, body);
            }
        }

        private string UrlToFileName(string url)
        {
            url = url.Substring(url.IndexOf("//") + 2);
            url = url.Replace('/', '_');
            url = url.Replace('?', '_');
            if(url.Length > 255)
            {
                url = url.Substring(0, 255);
            }

            if (url.Contains("hm.baidu.com_hm.gif"))
            {
                return "hm.baidu.com_hm.gif";
            }

            return url;
        }

    }
}
