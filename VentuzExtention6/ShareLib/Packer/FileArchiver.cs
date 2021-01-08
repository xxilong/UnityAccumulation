using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShareLib.Packer
{
    public class FileArchiver
    {
        public FileArchiver(Stream stream)
        {
            _stream = stream;
        }

        public void PushFile(string name, byte[] data)
        {
            int datalen = data.Length;
            Write(name);
            Write(datalen);
            Write(data);
        }

        public bool PushDiskFile(string name, string filepath)
        {
            FileInfo info = new FileInfo(filepath);

            if(!info.Exists)
            {
                return false;
            }

            if(info.Length > 1024 * 1024 * 1024)
            {
                return false;
            }

            name = "$" + name;            
            int datalen = (int)info.Length + 8;
            Write(name);
            Write(datalen);
            Write(info.LastWriteTime.ToBinary());

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fs, _stream, (int)info.Length);
            }

            return true;
        }

        public int ExtractDiskFiles(string rootpath, bool forceoverride = false)
        {
            int extracted = 0;
            _stream.Seek(0, SeekOrigin.Begin);

            string s = ReadString();
            while(s != null)
            {
                int? datalen = ReadInt();
                if(s[0] != '$')
                {
                    _stream.Seek(datalen.Value, SeekOrigin.Current);
                }
                else
                {
                    int filelen = datalen.Value - 8;
                    DateTime modiyTime = DateTime.FromBinary(ReadLong().Value);

                    bool ret = true;
                    if(forceoverride)
                    {
                        CreateFileOverride(Path.Combine(rootpath, s.Substring(1)), modiyTime, _stream, filelen);
                    }
                    else
                    {
                        ret = CreateFile(Path.Combine(rootpath, s.Substring(1)), modiyTime, _stream, filelen);
                    }

                    if(ret)
                    {
                        ++extracted;
                    }
                }

                s = ReadString();
            }

            return extracted;
        }

        public byte[] GetFile(string name)
        {
            try
            {
                _stream.Seek(0, SeekOrigin.Begin);
                string s = ReadString();
                while(s != name)
                {
                    if(s == null)
                    {
                        return null;
                    }

                    int? datalen = ReadInt();
                    _stream.Seek(datalen.Value, SeekOrigin.Current);
                    s = ReadString();
                }

                int? len = ReadInt();
                return ReadBytes(len.Value);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public void Close()
        {
            _stream.Close();
        }

        private bool CreateFile(string path, DateTime modifyTime, Stream from, int fsize)
        {
            FileInfo info = new FileInfo(path);
            if(!info.Exists)
            {
                CreateFileOverride(path, modifyTime, from, fsize);
                return true;
            }

            if((int)info.Length == fsize && info.LastWriteTime == modifyTime)
            {
                from.Seek(fsize, SeekOrigin.Current);
                return false;
            }

            string empty = "";
            string latest = " (较新)";

            if(MessageBox.Show(
                $"文件 {path} 已经存在, 是否使用\n" + 
                $"   存档文件 [大小: {fsize} 字节, 修改日期: {modifyTime}]{ (modifyTime > info.LastWriteTime ? latest : empty) }\n" +
                $"来替换\n" +
                $"   现有文件 [大小: {info.Length} 字节, 修改日期: {info.LastWriteTime}{ (info.LastWriteTime > modifyTime ? latest : empty) }\n" +
                $"???", 
                "文件覆盖确认", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CreateFileOverride(path, modifyTime, from, fsize);
                return true;
            }
            else
            {
                from.Seek(fsize, SeekOrigin.Current);
                return false;
            }
        }

        private void CreateFileOverride(string path, DateTime modifyTime, Stream from, int fsize)
        {
            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                CopyStream(from, fStream, fsize);
            }

            File.SetLastWriteTime(path, modifyTime);
        }

        private void Write(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }
                
        private void Write(int data)
        {
            Write(BitConverter.GetBytes(data));
        }

        private void Write(long data)
        {
            Write(BitConverter.GetBytes(data));
        }

        private void CopyStream(Stream from, Stream to, int size)
        {
            const int bufsize = 1024 * 40;
            int copyed = 0;
            byte[] buf = new byte[bufsize];

            while(copyed < size)
            {
                int rd = from.Read(buf, 0, Math.Min(bufsize, size - copyed));
                to.Write(buf, 0, rd);
                copyed += rd;
            }
        }

        private void Write(string str)
        {
            byte[] namebyte = Encoding.UTF8.GetBytes(str);
            int namelen = namebyte.Length;
            Write(namelen);
            Write(namebyte);
        }

        private int? ReadInt()
        {
            byte[] data = ReadBytes(4);

            if(data == null)
            {
                return null;
            }

            return BitConverter.ToInt32(data, 0);
        }

        private long? ReadLong()
        {
            byte[] data = ReadBytes(8);
            if(data == null)
            {
                return null;
            }

            return BitConverter.ToInt64(data, 0);
        }

        private byte[] ReadBytes(int len)
        {
            byte[] data = new byte[len];
            int readed = _stream.Read(data, 0, len);
            return readed == len ? data : null;
        }

        private string ReadString()
        {
            int? len = ReadInt();

            if(len == null)
            {
                return null;
            }

            byte[] sbytes = ReadBytes(len.Value);
            if(sbytes == null)
            {
                return null;
            }

            return Encoding.UTF8.GetString(sbytes);
        }

        private Stream _stream = null;
    }
}
