using System;
using System.Text;

namespace ShareLib.Protocl
{
    public class StringProtoclShortLen : ShortLenHeadPacker
    {
        public byte[] PackString(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return PackData(bytes);
        }

        public override void OnPackage(MyMemoryStream app)
        {
            string str = Encoding.Default.GetString(app.ToArray());
            if (OnRecvString != null)
                OnRecvString.Invoke(str);
        }

        public void SetReciver(Action<string> act)
        {
            OnRecvString = act;
        }

        private Action<string> OnRecvString;
    }
}
