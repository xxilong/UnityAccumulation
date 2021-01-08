using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace ShareLib.Protocl
{
    public class MyMemoryStream : MemoryStream
    {
        public MyMemoryStream(byte[] data)
            : base(data)
        {            
        }

        public MyMemoryStream()
        {
        }

       

        public void WriteNetOrderShort(short value)
        {
            WriteByteArray(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)));
        }

        public void WriteHostOrderShort(short value)
        {
            WriteByteArray(BitConverter.GetBytes(value));
        }

        public void WriteNetOrderInt(int value)
        {
            WriteByteArray(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)));
        }

        public void WriteHostOrderInt(int value)
        {
            WriteByteArray(BitConverter.GetBytes(value));
        }

        public void WriteByteArray(byte[] bytes)
        {
            Write(bytes, 0, bytes.GetLength(0));
        }

        public void WriteString(string value)
        {
            WriteByteArray(System.Text.Encoding.Default.GetBytes(value));
        }      


        public void ReadByteArray(byte[] bytes)
        {
            Read(bytes, 0, bytes.GetLength(0));
        }

        //public byte[] ReadRest()
        //{
        //    this.Position
        //}

        public void Skip(int bytecount)
        {
            base.Seek(bytecount, SeekOrigin.Current);
        }

        public void SeekToBegin()
        {
            Seek(0, SeekOrigin.Begin);
        }

        public void SeekToEnd()
        {
            Seek(0, SeekOrigin.End);
        }

        public void PopReaded()
        {
            if(Position == Length)
            {
                Position = 0;
                SetLength(0);
                return;
            }

            byte[] buff = GetBuffer();
            MoveBytes(buff, Position, Length - Position);
            SetLength(Length - Position);
            Position = 0;
        }

        public long UnReadLength
        {
            get
            {
                return Length - Position;
            }
        }

        public short ReadNetOrderShort()
        {
            byte[] shortbyte = new byte[2];
            ReadByteArray(shortbyte);
            short origshort = BitConverter.ToInt16(shortbyte, 0);
            return IPAddress.NetworkToHostOrder(origshort);
        }

        public int ReadNetOrderInt()
        {
            byte[] intbyte = new byte[4];
            ReadByteArray(intbyte);
            int origint = BitConverter.ToInt32(intbyte, 0);
            return IPAddress.NetworkToHostOrder(origint);
        }

        public short ReadHostOrderShort()
        {
            byte[] shortbyte = new byte[2];
            ReadByteArray(shortbyte);
            return BitConverter.ToInt16(shortbyte, 0);
        }

        public int ReadHostOrderInt()
        {
            byte[] intbyte = new byte[4];
            ReadByteArray(intbyte);
            return BitConverter.ToInt32(intbyte, 0);
        }

        public float ReadHostOrderFloat()
        {
            byte[] fbyte = new byte[4];
            ReadByteArray(fbyte);
            return BitConverter.ToSingle(fbyte, 0);
        }

        public string ReadFixedString(int strlen)
        {
            byte[] resbyte = new byte[strlen];
            ReadByteArray(resbyte);
            return System.Text.Encoding.ASCII.GetString(resbyte);
        }

        public string ReadStringBySep(byte sep1, byte sep2)
        {
            int len = 0;
            byte[] buff = GetBuffer();
            
            for(long i = Position; i < buff.Length - 1; ++i)
            {
                if(buff[i] == sep1 && buff[i + 1] == sep2)
                {
                    len = (int)(i - Position);
                    break;
                }
            }

            if(len == 0)
            {
                return null;
            }

            string s = ReadFixedString(len);
            Skip(2);
            return s;
        }

        public string ReadByteLenString()
        {
            int strlen = ReadByte();
            return ReadFixedString(strlen);
        }

        public string ReadByteLenUtf16String()
        {
            int strlen = ReadByte();
            byte[] resbyte = new byte[strlen];
            ReadByteArray(resbyte);
            return System.Text.Encoding.GetEncoding(1201).GetString(resbyte);
        }

        public DateTime ReadDateTime()
        {
            string year = ReadFixedString(4);
            string month = ReadFixedString(2);
            string day = ReadFixedString(2);
            string hour = ReadFixedString(2);
            string minute = ReadFixedString(2);
            string second = ReadFixedString(2);

            return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month),
                Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second));
        }

        private void MoveBytes(byte[] array, long offset, long count)
        {
            for(int i = 0; i < count; ++i)
            {
                array[i] = array[i + offset];
            }
        }
    }
}
