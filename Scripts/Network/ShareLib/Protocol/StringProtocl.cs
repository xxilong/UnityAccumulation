using System;
using System.Text;

namespace ShareLib.Protocl
{
    public class StringProtocl : LenHeadPacker
    {
        public byte[] PackString(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return PackData(bytes);
        }

        public override void OnPackage(MyMemoryStream app)
        {
            string str = Encoding.UTF8.GetString(app.ToArray());
            if (OnRecvString!=null)
                OnRecvString.Invoke(str);
        }

        public void SetReciver(Action<string> act)
        {
            OnRecvString = act;
        }

        private Action<string> OnRecvString;
    }
}
