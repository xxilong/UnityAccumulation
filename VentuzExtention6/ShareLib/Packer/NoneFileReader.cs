using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Packer
{
    public class NoneFileReader : IFileReader
    {
        public override Stream GetReadStream(string name)
        {
            return new MemoryStream();
        }
    }
}
