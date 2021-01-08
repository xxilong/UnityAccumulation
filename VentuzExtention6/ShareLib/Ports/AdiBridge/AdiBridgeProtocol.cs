using ShareLib.Log;
using ShareLib.Protocl;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.AdiBridge
{
    [Serializable]
    public class AttitudeAngle  // 姿态角
    {
        public float yaw = 0;
        public float pitch = 0;
        public float roll = 0;
    }

    [Serializable]
    public class ShockFreq
    {
        public float freq_x = 0;           // 频率
        public float amplitude_x = 0;      // 振幅
        public float freq_y = 0;
        public float amplitude_y = 0;
        public float freq_z = 0;
        public float amplitude_z = 0;
    }

    [Serializable]
    public class BridgeShockData
    {
        public AttitudeAngle angle = new AttitudeAngle();
        public ShockFreq freqs = new ShockFreq();
        public short[] spectrogram = new short[128];  // 频谱图数据 128 个
    }


    public class AdiBridgeProtocol : DeviceProtocl
    {
        protected override void OnRecvPackage(MyMemoryStream stream)
        {
            int maincmd = stream.ReadByte();
            if (maincmd == 0xF8)
            {
                Logger.Info($"收到程序更新指令, 忽略");
                return;
            }

            if (maincmd != 0x10)
            {
                Logger.Error($"收到未知的指令 0x{maincmd.ToString("x")}");
                return;
            }

            int sensorid = 0;
            int subcmd = stream.ReadByte();

            switch (subcmd)
            {
                case 0x30:
                    sensorid = stream.ReadByte();

                    _bridgeData.angle.pitch = stream.ReadHostOrderFloat();
                    _bridgeData.angle.roll = stream.ReadHostOrderFloat();
                    _bridgeData.angle.yaw = stream.ReadHostOrderFloat();
                    OnRecvShockData?.Invoke(_bridgeData);
                    Logger.Info($"收到姿态角信息");                  
                    return;

                case 0x31:
                    sensorid = stream.ReadByte();

                    _bridgeData.freqs.freq_x = stream.ReadHostOrderFloat();
                    _bridgeData.freqs.amplitude_x = stream.ReadHostOrderFloat();
                    _bridgeData.freqs.freq_y = stream.ReadHostOrderFloat();
                    _bridgeData.freqs.amplitude_y = stream.ReadHostOrderFloat();
                    _bridgeData.freqs.freq_z = stream.ReadHostOrderFloat();
                    _bridgeData.freqs.amplitude_z = stream.ReadHostOrderFloat();
                    
                    OnRecvShockData?.Invoke(_bridgeData);
                    Logger.Info($"收到频率速度信息");
                    return;

                case 0x32:
                    sensorid = stream.ReadByte();
                    int axel = stream.ReadByte();

                    for(int i = 0; i < 128; ++i)
                    {
                        _bridgeData.spectrogram[i] = (short)stream.ReadByte();
                    }

                    OnRecvShockData?.Invoke(_bridgeData);
                    Logger.Info($"收到频谱数据信息");
                    return;

                default:
                    Logger.Info($"收到未知指令 0x{subcmd.ToString("x")}, 已忽略");
                    return;
            }
        }

        public override byte[] PackData(byte[] appdata)
        {
            throw new NotImplementedException();
        }        

        public void SetShockFrameListener(Action<BridgeShockData> act) => OnRecvShockData = act;
        private Action<BridgeShockData> OnRecvShockData = null;
        private BridgeShockData _bridgeData = new BridgeShockData();
    }
}
