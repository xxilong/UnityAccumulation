using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Protocl;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ShareLib.Ports
{
    public class SerialPort
    {
        public SerialPort(IPackProtocl reader)
        {
            _reader = reader;
        }

        public void Open(string cfgdomain)
        {
            if(_port != null && _port.IsOpen)
            {
                return;
            }
            
            _port = new System.IO.Ports.SerialPort(GlobalConf.getconf(cfgdomain, "name"),
                GlobalConf.getconf<int>(cfgdomain, "baudrate", 9600),
                GlobalConf.getenum<System.IO.Ports.Parity>(cfgdomain, "parity", System.IO.Ports.Parity.None),
                GlobalConf.getconf<int>(cfgdomain, "databits", 8),
                GlobalConf.getenum<System.IO.Ports.StopBits>(cfgdomain, "stopbits", System.IO.Ports.StopBits.None));
            
            _port.DtrEnable = GlobalConf.getconf<bool>(cfgdomain, "enable_dtr", false);
            _port.RtsEnable = GlobalConf.getconf<bool>(cfgdomain, "enable_rts", false);

            _port.DataReceived += DataReceivedHandler;

            _sendThread = new Thread(WriteThread);
            _sendThread.Start();

            try
            {
                _port.Open();
            }
            catch(Exception e)
            {
                Logger.Error($"打开串口时出现异常: {e}");
            }
        }

        public void Close()
        {
            _port.Close();
            _port.Dispose();
            _port = null;

            // 关闭线程稍后加
            _sendThread = null;
        }

        public void Write(byte[] data)
        {
            lock(_queuelock)
            {
                _sendquess.Enqueue(data);
            }

            _queueEvent.Set();
        }

        private void RealWrite(byte[] data)
        {
            Logger.Debug($"发送数据: {BitConverter.ToString(data)}");

            try
            {
                _port.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Logger.Error($"发送串口数据是出现异常: {e}");

                try
                {
                    _port.Open();
                    _port.Write(data, 0, data.Length);
                }
                catch (Exception eloop)
                {
                    Logger.Error($"发送数据异常后重试打开串口发送异常: {eloop}");
                }
            }
        }

        private void WriteThread()
        {
            Logger.Info("串口数据发送线程启动");
            while(true)
            {
                _queueEvent.WaitOne();
                while(true)
                {
                    byte[] data = null;

                    lock (_queuelock)
                    {
                        if (_sendquess.Count > 0)
                        {
                            data = _sendquess.Dequeue();
                        }
                    }

                    if (data != null)
                    {
                        RealWrite(data);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void DataReceivedHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] buff = new byte[_port.BytesToRead];
            _port.Read(buff, 0, buff.Length);
            Logger.Debug($"收到数据: {BitConverter.ToString(buff)}");
            _reader.OnRead(buff);
        }

        public IPackProtocl Protocol { get { return _reader; } }

        private System.IO.Ports.SerialPort _port = null;
        private IPackProtocl _reader;
        private AutoResetEvent _queueEvent = new AutoResetEvent(true);
        private Thread _sendThread = null; 
        private object _queuelock = new object();
        private Queue<byte[]> _sendquess = new Queue<byte[]>();
    }
}
