namespace ShareLib.Protocl
{
    public class LenHeadPacker : PackageProtocl
    {
        public override byte[] PackData(byte[] appData)
        {
            var _stream = new MyMemoryStream();
            _stream.WriteNetOrderInt(appData.Length);
            _stream.WriteByteArray(appData);
            return _stream.ToArray();
        }

        protected override void CheckPackage()
        {
            while (true)
            {
                if (_stream.UnReadLength < 4)
                {
                    return;
                }

                int len = _stream.ReadNetOrderInt();
                if(_stream.UnReadLength < len)
                {
                    return;
                }

                byte[] appData = new byte[len];
                _stream.ReadByteArray(appData);
                _stream.PopReaded();
                
                MyMemoryStream app = new MyMemoryStream(appData);
                OnPackage(app);
            }
        }

        public virtual void OnPackage(MyMemoryStream app)
        {
        }
    }
}
