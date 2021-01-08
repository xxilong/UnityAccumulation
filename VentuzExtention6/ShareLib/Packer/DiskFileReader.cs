using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Packer
{
    public class DiskFileReader : IFileReader
    {
        public DiskFileReader(string rootPath)
        {
            _rootPath = rootPath;

            _watcher.Changed += OnFileChanged;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Path = _rootPath;
        }

        public override Stream GetReadStream(string name)
        {
            return new FileStream(Path.Combine(_rootPath, name), FileMode.Open, FileAccess.Read);
        }

        public override byte[] ReadAllBytes(string name)
        {
            return File.ReadAllBytes(Path.Combine(_rootPath, name));
        }

        public override string ReadAllText(string name)
        {
            return File.ReadAllText(Path.Combine(_rootPath, name));
        }

        public override void WatchFile(string name)
        {
            _watcher.Filter = name;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            int now = Environment.TickCount;
            if (now - lastChangeTime < 500)
            {
                return;
            }

            lastChangeTime = now;

            Delay.Run(100, () =>
            {
                fireFileChanged();
            });
        }

        private string _rootPath;
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private int lastChangeTime = 0;
    }
}
