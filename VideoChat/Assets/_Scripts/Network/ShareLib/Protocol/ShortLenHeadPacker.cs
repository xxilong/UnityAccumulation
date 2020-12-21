﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Protocl
{
    public class ShortLenHeadPacker : PackageProtocl
    {
        public override byte[] PackData(byte[] appData)
        {
            var _stream = new MyMemoryStream();
            _stream.WriteHostOrderShort((short)(appData.Length + 2));
            _stream.WriteByteArray(appData);
            return _stream.ToArray();
        }

        protected override void CheckPackage()
        {
            while (true)
            {
                if (_stream.UnReadLength < 2)
                {
                    return;
                }

                int len = _stream.ReadHostOrderShort() - 2;
                if (_stream.UnReadLength < len)
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
