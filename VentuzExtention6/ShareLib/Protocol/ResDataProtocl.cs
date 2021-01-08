using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareLib.Protocl
{
    public class ResDataProtocl : LenHeadPacker
    {
        public byte[] PackResData(string resname, byte[] data)
        {
            resname = "!" + resname;
            byte[] namebytes = Encoding.UTF8.GetBytes(resname);
            var _stream = new MyMemoryStream();
            _stream.WriteNetOrderInt(namebytes.Length + data.Length + 1);
            _stream.WriteByteArray(namebytes);
            _stream.WriteByte(0);
            _stream.WriteByteArray(data);
            return _stream.ToArray();            
        }

        public override void OnPackage(MyMemoryStream app)
        {
            byte[] data = app.ToArray();
            int len = 0;
            for(int i = 0; i < data.Length; ++i)
            {
                if(data[i] == 0)
                {
                    len = i;
                    break;
                }
            }

            string resname = Encoding.UTF8.GetString(data, 1, len - 1);
            byte[] resdata = data.Skip(len + 1).Take(data.Length - len - 1).ToArray();

            OnRecvString?.Invoke(resname, resdata);
        }

        public void SetReciver(Action<string, byte[]> act)
        {
            OnRecvString = act;
        }

        private Action<string, byte[]> OnRecvString;
    }
}
