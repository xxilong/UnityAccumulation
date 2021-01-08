using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidCtrl;
using ShareLib.Ayz;
using ShareLib.Log;

namespace VolumeControlServ
{
    public class PlugEntry : MidPlug
    {
        public override void OnRecvCommand(string cmd)
        {
            Logger.Debug($"[Volume] {cmd}");

            CmdLine command = new CmdLine(cmd);
            switch(command.cmd)
            {
                case "vol":
                    if (!command.getarg<double>(0, out double val))
                    {
                        Logger.Warning($"[Volume] vol argument should be double with 0-100");
                        return;
                    }

                    VolumeControl.Instance.MasterVolume = val;
                    Logger.Info($"[Volume] volume has been set to {VolumeControl.Instance.MasterVolume}/{val}");
                    break;

                case "mute":
                    if(command.argcount == 0)
                    {
                        VolumeControl.Instance.IsMute = !VolumeControl.Instance.IsMute;
                        Logger.Info($"[Volume] mute is set to {VolumeControl.Instance.IsMute}");
                        return;
                    }

                    command.getarg<string>(0, out string vm);
                    if(vm != "on" && vm != "off")
                    {
                        Logger.Warning($"[Volume] mute argument should be on/off");
                        return;
                    }

                    if(vm == "on")
                    {
                        VolumeControl.Instance.IsMute = true;
                    }
                    else
                    {
                        VolumeControl.Instance.IsMute = false;
                    }
                    Logger.Info($"[Volume] mute is set to {VolumeControl.Instance.IsMute}");
                    break;

                case "echo":
                    string backcmd = command.argsline;
                    backcmd = backcmd.Replace("!(vol)", VolumeControl.Instance.MasterVolume.ToString());
                    int p1 = backcmd.IndexOf("!(mute?");
                    if(p1 >= 0)
                    {
                        int p2 = backcmd.IndexOf(":", p1);
                        int p3 = backcmd.IndexOf(")", p2);
                        if(p2 >= 0 && p3 >= 0)
                        {
                            string str1 = backcmd.Substring(p1 + 7, p2 - p1 - 7);
                            string str2 = backcmd.Substring(p2 + 1, p3 - p2 - 1);

                            if(VolumeControl.Instance.IsMute)
                            {
                                backcmd = backcmd.Remove(p1, p3 - p1 + 1).Insert(p1, str1);
                            }
                            else
                            {
                                backcmd = backcmd.Remove(p1, p3 - p1 + 1).Insert(p1, str2);
                            }
                        }
                    }

                    SendCommandToControl(backcmd);
                    break;

                default:
                    Logger.Warning($"unknown command {command.cmd}");
                    return;
            }


        }
    }
}
