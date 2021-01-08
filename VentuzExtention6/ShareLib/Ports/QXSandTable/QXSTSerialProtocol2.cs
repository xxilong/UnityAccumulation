using ShareLib.Protocl;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.QXSandTable
{
    public class QXSTSerialProtocol2 : QXSTSerialProtocol
    {
        public override byte[] GotoArea(int area)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x10);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteHostOrderInt(1 << area);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }

        public override byte[] Go()
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0B);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteHostOrderInt(0);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }

        public override byte[] Stop()
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0C);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteHostOrderInt(0);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }
    }
}
