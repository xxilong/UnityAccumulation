using System;
using System.IO;

namespace ShareLib.Packer
{
    public abstract class IFileReader
    {
        public abstract Stream GetReadStream(string name);
        public virtual void WatchFile(string name)
        {
        }
        public virtual void UnWatch()
        {
        }

        public virtual string ReadAllText(string name)
        {
            Stream stm = GetReadStream(name);
            if(stm == null)
            {
                return string.Empty;
            }
            else
            {
                using (StreamReader reader = new StreamReader(stm))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public virtual byte[] ReadAllBytes(string name)
        {
            Stream stm = GetReadStream(name);
            if (stm == null)
            {
                return new byte[0];
            }
            else
            {
                byte[] buffer = new byte[stm.Length];

                long count = stm.Length;
                long offset = 0;

                while(count > 0)
                {
                    int r = stm.Read(buffer, (int)offset, (int)count);
                    if(r == 0)
                    {
                        throw new EndOfStreamException();
                    }

                    offset += r;
                    count -= r;
                }

                return buffer;
            }
        }

        public event EventHandler<int> OnWatchFileChanged;
        protected void fireFileChanged()
        {
            OnWatchFileChanged?.Invoke(this, 0);
        }
    }
}
