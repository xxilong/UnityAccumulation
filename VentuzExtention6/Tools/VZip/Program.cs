using ICSharpCode.SharpZipLib.Zip;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.IO;

namespace VZip
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\BaiduNetdiskDownload\八大园区final_L.vpr";

            //FastZip fz = new FastZip();
            //fz.ExtractZip(path, @"D:\Download\大屏\", null);

            ExtractVPR(path, @"E:\project\technique\VentuzResources\八大园区\");
        }

        static void ExtractVPR(string vprPath, string extractPath)
        {
            Stream fileStream = File.OpenRead(vprPath);
            ZipFile zip = new ZipFile(fileStream);
            Stream uriliststream = zip.GetInputStream(zip.GetEntry("UriList"));
            VBinaryFormatter formater = new VBinaryFormatter("Presentation");
            UriCollection uriColl = formater.Deserialize(uriliststream) as UriCollection;
            VentuzZipSchemeManager schemeMgr = new VentuzZipSchemeManager(uriColl, zip);

            for (int i = 0; i < uriColl.Count; ++i)
            {
                UriCollectionEntry urlentry = uriColl[i];
                if (urlentry.Available)
                {
                    Uri uri = urlentry.Uri;
                    string ePath = uri.ToString().Replace("ventuz://", extractPath);
                    ePath = ePath.Replace("/", "\\");
                    int sharp = ePath.IndexOf('#');
                    if(sharp != -1)
                    {
                        ePath = ePath.Substring(0, sharp);
                    }

                    Logger.Debug($"Extract File: {uri}");
                    Directory.CreateDirectory(Path.GetDirectoryName(ePath));
                    Stream r = schemeMgr.GetReadStream(uri);
                    FileStream f = File.OpenWrite(ePath);
                    r.CopyTo(f);
                    f.Close();
                    r.Close();
                }
            }
        }
    }
}
