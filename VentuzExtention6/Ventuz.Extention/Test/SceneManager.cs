using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ventuz.Kernel;
using Ventuz.Extention.Control;

namespace Ventuz.Extention.Test
{

    public class Script : ScriptBase, System.IDisposable
    {
        public bool Mine, Port;
        // This member is used by the Validate() method to indicate
        // whether the Generate() method should return true or false
        // during its next execution.
        private bool changed;

        // This Method is called if the component is loaded/created.
        public Script()
        {
            // Note: Accessing input or output properties from this method
            // will have no effect as they have not been allocated yet.
        }
        // This Method is called if the function/method Start is invoked by the user or a bound event.
        // Return true, if this component has to be revalidated!
        public bool OnStart(int arg)
        {
            ControlServer.Instance.Start(3131);
            ControlServer.Instance.OnReciveCommand = OnReciveCommand;
            return false;
        }

        private void OnReciveCommand(string obj)
        {
            if (obj=="mine open")
            {
                Mine = true;
                Port = false;
            }
            else if (obj == "mine close")
            {
                Mine = false;
            }

            if (obj == "port open")
            {
                Port = true;
                Mine = false;
            }
            else if (obj == "port close")
            {
                Port = false;
            }
            changed = true;
        }

        // This Method is called if the component is unloaded/disposed
        public virtual void Dispose()
        {
        }

        // This Method is called if an input property has changed its value
        public override void Validate()
        {
            // Remember: set changed to true if any of the output 
            // properties has been changed, see Generate()
        }

        // This Method is called every time before a frame is rendered.
        // Return value: if true, Ventuz will notify all nodes bound to this
        //               script node that one of the script's outputs has a
        //               new value and they therefore need to validate. For
        //               performance reasons, only return true if output
        //               values really have been changed.
        public override bool Generate()
        {
            if (changed)
            {
                changed = false;
                return true;
            }

            return false;
        }
    }
}
