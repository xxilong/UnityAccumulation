using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.Input;
using Ventuz.Extention.Conf;

namespace Ventuz.Extention.Marker
{
    class TouchToFile : TouchRecorder
    {
        public static TouchToFile toFile = new TouchToFile();

        public void FrameBegin(double timeCode, List<EventAtom> events)
        {
            if(sw != null && events.Count != 0)
            {
                sw.WriteLine("-{0}", timeCode);
            }
        }

        public void FrameEnd(double timeCode, List<EventAtom> events)
        {
            if(sw != null && events.Count != 0)
            {
                sw.WriteLine("--");
                sw.Flush();
            }
        }

        public void FrameTouchEvent(EventAtomTouch touchEvent)
        {
            if(sw != null)
            {
                sw.WriteLine(touchEvent.ToString());
            }
        }

        public void Start()
        {
            if(sw != null)
            {
                return;
            }

            Console.WriteLine("\n!!开始录制!!\n");
            fs = new FileStream(FilePaths.TouchRecordePath, FileMode.Create);
            sw = new StreamWriter(fs);
        }

        public void Stop()
        {
            if(sw == null)
            {
                return;
            }

            Console.WriteLine("\n!!停止录制!!\n");
            sw.Close();
            fs.Close();

            sw = null;
            fs = null;
        }

        private FileStream fs = null;
        private StreamWriter sw = null;
    }
}
