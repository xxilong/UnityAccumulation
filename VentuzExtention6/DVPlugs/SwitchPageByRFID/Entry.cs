using MidCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Conf;
using System.IO;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;
using System.Timers;

namespace SwitchPageByRFID
{
    public class Entry : DVPlayer
    {
        public override void OnInit()
        {
            Logger.Info("Loading Plug SwitchPageByRFID...");
            GlobalConf.SetGlobalFileGetter(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));

            try
            {
                QXSTSerialPort.Instance.Open();
                QXSTSerialPort.Instance._procol.SetMarkersStatusListenner(OnMarkerStatusChange);
            }
            catch (Exception e)
            {
                Logger.Error($"[SwitchPageByRFID] open serial port error: {e}");
            }
        }

        private void OnMarkerStatusChange(int[] ids, int[] status)
        {
            int firstSide = status[0];
            if(firstSide == _firstSide)
            {
                Logger.Debug($"First Slot RFID side is ${firstSide}, not changed.");
            }
            else
            {
                if(_delayrun != null)
                {
                    _delayrun.Stop();
                    _delayrun.Close();
                    _delayrun = null;
                }

                _firstSide = firstSide;
                if(firstSide == 0)
                {
                    Logger.Debug($"First Slot RFID side is change to 0 in 5 seconds...");
                    _delayrun = Delay.Run(5000, () =>
                    {
                        Logger.Debug($"First Slot RFID side is change to 0 now");
                        GotoPage($"{_firstSide}");
                    });
                }
                else
                {
                    Logger.Debug($"First Slot RFID side is change to {firstSide}");
                    GotoPage($"{_firstSide}");
                }
            }
        }

        private int _firstSide = 0;
        private Timer _delayrun = null;
    }
}
