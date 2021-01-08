using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ventuz.Extention.DRZ
{
    public class DrzStream : FileStream
    {
        public DrzStream(string path)
            : base(path, FileMode.Open, FileAccess.Read, FileShare.Read)
        {
            base.Seek(-1, SeekOrigin.End);
            seed = (byte)base.ReadByte();
            for(int i = 0; i < coding.Length; ++i)
            {
                coding[i] ^= seed;
            }
            base.Seek(0, SeekOrigin.Begin);
        }

        public DrzStream(string path, bool r)
            : base(path, FileMode.Create, FileAccess.Write, FileShare.Write)
        {
            seed = (byte)new Random().Next();
            write = true;

            for (int i = 0; i < coding.Length; ++i)
            {
                coding[i] ^= seed;
            }
        }

        #region 未实现
        public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            throw new NotImplementedException();
        }

        [SecuritySafeCritical]
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
            //return base.EndRead(asyncResult);
        }

        [SecuritySafeCritical]
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        #endregion

        [SecuritySafeCritical]
        public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            long pos = Position;
            for(int i = 0; i < numBytes; ++i)
            {
                array[offset + i] ^= coding[(pos + i) % coding.Length];
            }

            return base.BeginWrite(array, offset, numBytes, userCallback, stateObject);
        }

        [SecuritySafeCritical]
        public override int Read(byte[] array, int offset, int count)
        {
            long pos = Position;
            int len = base.Read(array, offset, count);
            for(int i = 0; i < len; ++i)
            {
                array[offset + i] ^= coding[(pos + i) % coding.Length];
            }
            return len;
        }
                       
        [SecuritySafeCritical]
        public override int ReadByte()
        {
            long pos = Position;
            int res = base.ReadByte();
            if(res == -1)
            {
                return -1;
            }

            return res ^= coding[pos % coding.Length];
        }
              
        [SecuritySafeCritical]
        public override void Write(byte[] array, int offset, int count)
        {
            long pos = Position;
            for (int i = 0; i < count; ++i)
            {
                array[offset + i] ^= coding[(pos + i) % coding.Length];
            }

            base.Write(array, offset, count);
        }
        
        [SecuritySafeCritical]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            long pos = Position;
            for (int i = 0; i < count; ++i)
            {
                buffer[offset + i] ^= coding[(pos + i) % coding.Length];
            }

            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }

        [SecuritySafeCritical]
        public override void WriteByte(byte value)
        {
            long pos = Position;
            value ^= coding[pos % coding.Length];
            base.WriteByte(value);
        }

        public override void Close()
        {
            if(write)
            {
                Seek(0, SeekOrigin.End);
                base.WriteByte(seed);
                write = false;
            }

            base.Close();
        }



        private byte[] coding = { 0x0A, 0x0D, 0x0E,0x5F, 0xAD, 0x1E, 0x12, 0x32, 0x43, 0x55, 0x8C, 0xE2, 0xDE, 0x53, 0x01, 0x80,
                                  0x49, 0x19, 0x94,0x20, 0x19, 0x11, 0x05, 0x28, 0x94, 0x10, 0x29, 0x20, 0x12, 0x76, 0x85, 0xA3,
                                  0x3C, 0xDD, 0x27};
        private byte seed = 0;
        private bool write = false;
    }
}
