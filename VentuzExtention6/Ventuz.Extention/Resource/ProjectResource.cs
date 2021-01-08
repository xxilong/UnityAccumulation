using ICSharpCode.SharpZipLib.Zip;
using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using Ventuz.Extention.Conf;
using Ventuz.Extention.DRZ;
using Ventuz.Kernel.IO;

namespace Ventuz.Extention.Resource
{
    class ProjectResource
    {
        internal static ProjectResource instance = new ProjectResource();

        public SchemeManager UriFilter(ref Uri uri)
        {
            if(uri.Scheme == "ventuz")
            {
                string overpath = Path.Combine(FilePaths.ventuzDir, "mvs", MapHostName(uri.Host), uri.LocalPath.Substring(1));
                if(File.Exists(overpath))
                {
                    Logger.Debug($"Redirect {uri} to {overpath}");
                    uri = new Uri("file://" + overpath);
                    return null;
                }
            }

            var patch = Patchs;
            if (patch.ContainsKey(uri))
            {
                Logger.Debug($"{uri} has patched with {patch[uri].Item1}");
                return patch[uri].Item2;
            }

            return null;
        }

        private Dictionary<Uri, Tuple<string, SchemeManager>> Patchs {
            get
            {
                if(_patchs == null)
                {
                    InitPatchs();
                }
                return _patchs;
            }
        }

        private string MapHostName(string host)
        {
            if(_host_map.ContainsKey(host.ToLower()))
            {
                return _host_map[host.ToLower()];
            }

            return host;
        }

        private Dictionary<string, string> _host_map = new Dictionary<string, string> {
            {"movies", "mv" },
            {"pages", "ui" },
            {"images", "pic" },
            {"scripts", "bin" },
            {"textures", "pic" }
        };

        private void InitPatchs()
        {
            _patchs = new Dictionary<Uri, Tuple<string, SchemeManager>>();
            var patchFiles = DirScan.ScanFiles(FilePaths.ventuzDir, "_patch_?????_*.drz");
            foreach(var item in patchFiles)
            {
                try
                {
                    Logger.Debug($"Apply PatchFile: {item.Value}");
                    DrzStream stream = new DrzStream(item.Value);
                    ZipFile zip = new ZipFile(stream);
                    Stream uriliststream = zip.GetInputStream(zip.GetEntry("UriList"));
                    VBinaryFormatter formater = new VBinaryFormatter("Presentation");
                    UriCollection uriColl = formater.Deserialize(uriliststream) as UriCollection;
                    VentuzZipSchemeManager schemeMgr = new VentuzZipSchemeManager(uriColl, zip);

                    for(int i = 0; i < uriColl.Count; ++i)
                    {
                        UriCollectionEntry urlentry = uriColl[i];
                        if(urlentry.Available)
                        {
                            Uri uri = urlentry.Uri;
                            Logger.Debug($"-- Replace File: {uri}");
                            _patchs[uri] = new Tuple<string, SchemeManager>(item.Key, schemeMgr);
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.Error($"-- {e}");
                }
            }
        }

        private Dictionary<Uri, Tuple<string, SchemeManager>> _patchs = null;
    }
}
