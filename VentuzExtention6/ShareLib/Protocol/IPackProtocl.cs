using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Protocl
{
    public interface IPackProtocl
    {
        void OnRead(byte[] data, int len = -1);
        byte[] PackData(byte[] appdata);
        string PeerName { get; set; }
    }

    public abstract class PackageProtocl : IPackProtocl
    {
        public void OnRead(byte[] data, int len = -1)
        {
            _stream.SeekToEnd();

            if (len < 0)
            {
                _stream.WriteByteArray(data);
            }
            else
            {
                _stream.Write(data, 0, len);
            }

            _stream.SeekToBegin();
            CheckPackage();
        }

        public byte[] ToArray(MyMemoryStream appData)
        {
            return PackData(appData.ToArray());
        }

        public abstract byte[] PackData(byte[] appdata);

        public string PeerName { get; set; }
        
        protected abstract void CheckPackage();
        protected MyMemoryStream _stream = new MyMemoryStream();
    }
}
