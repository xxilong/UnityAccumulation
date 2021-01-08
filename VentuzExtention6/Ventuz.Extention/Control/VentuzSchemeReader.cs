using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShareLib.Packer;
using Ventuz.Extention.Compatible;
using Ventuz.Kernel.IO;

namespace Ventuz.Extention.Control
{
    public class VentuzSchemeReader : IFileReader
    {
        public VentuzSchemeReader(string prefex = "ventuz://")
        {
            _prefex = prefex;
            _urimgr = VentuzWare.Instance.GetUriManager();
        }

        public override Stream GetReadStream(string name)
        {
            if(_urimgr == null)
            {
                _urimgr = VentuzWare.Instance.GetUriManager();
            }

            if(_urimgr == null)
            {
                return null;
            }

            return _urimgr.GetReadStream(new Uri(_prefex + name));
        }

        private string _prefex;
        private UriManager _urimgr = null;
    }
}
