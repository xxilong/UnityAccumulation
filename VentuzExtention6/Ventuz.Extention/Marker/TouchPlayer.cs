using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ventuz.Kernel.Input;
using Ventuz.Extention.UI;
using Ventuz.Extention.Conf;

namespace Ventuz.Extention.Marker
{
    public class TouchPlayer
    {
        public static TouchPlayer player = new TouchPlayer();

        public void PlayFile()
        {
            begtime = 0;
            eventGroups = new LinkedList<TouchEventGroup>();
            LoadTouchEvents();
            Console.WriteLine("开始播放: 时长 {0} 秒", eventGroups.Last.Value.happentime - eventGroups.First.Value.happentime);

            pause = false;
            singStep = false;
            ExtMenuItems.Instance.pauseContinue.Text = "暂停";
            ExtMenuItems.Instance.pauseContinue.Enabled = true;
            ExtMenuItems.Instance.singleStep.Enabled = false;
        }

        public void PausePlay()
        {
            pause = !pause;
            if(pause)
            {
                pause_time = System.Environment.TickCount / 1000.0;
                ExtMenuItems.Instance.pauseContinue.Text = "继续";
                ExtMenuItems.Instance.singleStep.Enabled = true;
                Console.WriteLine("已暂停");
            }
            else
            {
                begtime += System.Environment.TickCount / 1000.0 - pause_time + 1;
                ExtMenuItems.Instance.pauseContinue.Text = "暂停";
                ExtMenuItems.Instance.singleStep.Enabled = false;
                Console.WriteLine("已继续");
            }
        }

        public void PlayStep()
        {
            if(!pause)
            {
                return;
            }

            Console.WriteLine("准备单步播放");
            singStep = true;
        }

        public void AddPlayEvents(double time, List<EventAtom> events)
        {
            if(eventGroups == null)
            {
                return;
            }

            if(begtime == 0)
            {
                begtime = time;
            }

            if(pause && !singStep)
            {
                return;
            }

            bool steped = false;
            while(eventGroups.Count > 0)
            {
                TouchEventGroup evg = eventGroups.First();
                if(evg.happentime > time - begtime)
                {
                    break;
                }

                steped = true;
                events.AddRange(evg.events);
                eventGroups.RemoveFirst();

                if(singStep)
                {
                    break;
                }
            }

            if(singStep && steped)
            {
                Console.WriteLine("单步播放完成: {0}", eventGroups.Count);
                singStep = false;
            }

            if (eventGroups.Count == 0)
            {
                Console.WriteLine("播放完成");
                eventGroups = null;
                ExtMenuItems.Instance.pauseContinue.Text = "暂停/继续";
                ExtMenuItems.Instance.pauseContinue.Enabled = false;
            }
        }

        private void LoadTouchEvents()
        {
            var fs = new FileStream(FilePaths.TouchRecordePath, FileMode.Open);
            var sr = new StreamReader(fs);

            double firsttime = -1;
            TouchEventGroup cur = new TouchEventGroup();
            string line = sr.ReadLine();
            while(line != null)
            {
                if(line.Length < 2)
                {
                    line = sr.ReadLine();
                    continue;
                }

                if(line[0] == '-' && line[1] == '-')
                {
                    eventGroups.AddLast(cur);
                    cur = new TouchEventGroup();
                }
                else if(line[0] == '-')
                {
                    line = line.Substring(1);
                    double happentime = double.Parse(line);

                    if(firsttime < 0)
                    {
                        firsttime = happentime;
                        Console.WriteLine("FirstEvent Happen At: {0}", firsttime);
                    }

                    cur.happentime = happentime - firsttime;
                }
                else
                {
                    EventAtom ev = ParseTouchEvent(line);
                    if(ev != null)
                    {
                        cur.events.Add(ev);
                    }
                }
                
                line = sr.ReadLine();
            }

            Console.WriteLine("Loaded {0} event groups.", eventGroups.Count);
        }

        private EventAtom ParseTouchEvent(string line)
        {
            string[] items = line.Split(' ');
            if (items.Length != 7)
            {
                Console.WriteLine("Error on parse event line: {0}", line);
                return null;
            }

            DeviceTypes deviceType = (DeviceTypes)Enum.Parse(typeof(DeviceTypes), items[0]);
            int deviceID = int.Parse(items[1]);
            string ID = items[2].Substring(0, items[2].Length - 1);

            Touch.TouchType touchType = (Touch.TouchType)Enum.Parse(typeof(Touch.TouchType), items[3]);
            string[] items2 = items[4].Split('@');
            if(items2.Length != 2)
            {
                Console.WriteLine("Error on parse event line: {0}", line);
                return null;
            }

            string touchID = items2[0];
            string[] coords = items2[1].Substring(1, items2[1].Length - 3).Split(',');
            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);
            float z = float.Parse(coords[2]);

            float pressure = float.Parse(items[5].Substring(1));
            float angle = float.Parse(items[6].Substring(1));

            return new EventAtomTouch(deviceType, deviceID, ID, 1, touchType, touchID, x, y, z, pressure, angle);
        }

        private double begtime;
        private bool pause;
        private bool singStep;
        private double pause_time;
        private LinkedList<TouchEventGroup> eventGroups;
    }
}
