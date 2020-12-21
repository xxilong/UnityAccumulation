using ShareLib.Log;
using System;

namespace ShareLib.Protocl
{
    public class DeviceProtocl : PackageProtocl
    {
        public override byte[] PackData(byte[] appData)
        {
            var _stream = new MyMemoryStream();
            _stream.WriteByte(0x51);
            _stream.WriteByte(0x58);
            byte _bcc = 0x51 ^ 0x58;

            byte len = (byte)appData.Length;
            _stream.WriteByte(len);
            _bcc ^= len;

            for (int i = 0; i < len; ++i)
            {
                _bcc ^= appData[i];
            }

            _stream.WriteByteArray(appData);
            _stream.WriteByte(_bcc);
            return _stream.ToArray();
        }

        protected override void CheckPackage()
        {
            while(true)
            {
                if(_stream.UnReadLength < 4)
                {
                    return;
                }
         
                int st1 = _stream.ReadByte();
                while(_stream.UnReadLength > 0)
                {
                    int st2 = _stream.ReadByte();
                    if(st1 == 0x51 && st2 == 0x58)
                    {
                        break;
                    }

                    st1 = st2;
                }

                if(_stream.UnReadLength < 1)
                {
                    return;
                }

                int len = _stream.ReadByte();
                if(_stream.UnReadLength < len + 1)
                {
                    return;
                }

                byte[] appData = new byte[len];
                _stream.ReadByteArray(appData);
                int bcc = _stream.ReadByte();
                _stream.PopReaded();

                byte _bcc = 0x51 ^ 0x58;
                byte blen = (byte)appData.Length;
                _bcc ^= blen;
                for (int i = 0; i < len; ++i)
                {
                    _bcc ^= appData[i];
                }

                if(bcc != _bcc)
                {
                    Logger.Warning("Package BCC Check Failed, Droped!");
                    return;
                }

                MyMemoryStream app = new MyMemoryStream(appData);
                OnRecvPackage(app);
            }    
        }

        protected virtual void OnRecvPackage(MyMemoryStream appData)
        {
            if(_recvCallback!=null)
                _recvCallback.Invoke(appData);
        }

        public void SetRecivePackCallBack(Action<MyMemoryStream> onpack)
        {
            _recvCallback = onpack;
        }

        private Action<MyMemoryStream> _recvCallback;
    }
}
