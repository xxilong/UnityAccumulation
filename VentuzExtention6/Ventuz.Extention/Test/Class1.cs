using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ventuz.Kernel;
using Ventuz.Extention.Control;
using ShareLib.Page;

public class Script1 : ScriptBase, System.IDisposable
{

    private int[] _marker = { 0, 0, 0, 0, 0, 0 };
    private int[] _args = { 0, 0, 0, 0, 0, 0 };
    private int[] _togs = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private bool changed;

    // This Method is called if the component is loaded/created.
    public Script1()
    {
        // Note: Accessing input or output properties from this method
        // will have no effect as they have not been allocated yet.
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


    public bool OnOnStartup(int arg)
    {
        Page.Control.InitPages(Pages);
        ControlServer.Instance.RegCmdHander<int, int, int, int, int, int>("set_mark_status", OnSetMarkerStatus);
        ControlServer.Instance.RegCmdHander<int, int, int, int, int, int>("set_args_status", OnSetArgStatus);
        ControlServer.Instance.RegCmdHander<string>("trygo", TryGo);
        ControlServer.Instance.RegCmdHander<int>("set_cube", SetCube);
        ControlServer.Instance.RegCmdHander<int, int, int, int, int, int, int, int>("set_togs_status", OnSetTogStatus);
        ControlServer.Instance.Start(3131);
        Page.Control.GotoPage("home");
        return false;
    }

    public void OnSetMarkerStatus(int side1, int side2, int side3, int side4, int side5, int side6)
    {
        _marker[0] = side1;
        _marker[1] = side2;
        _marker[2] = side3;
        _marker[3] = side4;
        _marker[4] = side5;
        _marker[5] = side6;
        MarkerStatus = _marker;
        changed = true;
    }

    public void OnSetArgStatus(int a1, int a2, int a3, int a4, int a5, int a6)
    {
        _args[0] = a1;
        _args[1] = a2;
        _args[2] = a3;
        _args[3] = a4;
        _args[4] = a5;
        _args[5] = a6;
        ArgStatus = _args;
        changed = true;
    }

    public void TryGo(string name)
    {
        if (Page.Control.GetCurPageName() == "aicde")
        {
            Page.Control.GotoPage(name);
        }
    }

    public void SetCube(int cube)
    {
        if (cube == 0)
        {
            Cube = false;
        }
        else if (cube == 1)
        {
            Cube = true;
        }
        changed = true;
    }

    public void OnSetTogStatus(int tog0, int tog1, int tog2, int tog3, int tog4, int tog5, int tog6, int tog7)
    {
        _togs[0] = tog0;
        _togs[1] = tog1;
        _togs[2] = tog2;
        _togs[3] = tog3;
        _togs[4] = tog4;
        _togs[5] = tog5;
        _togs[6] = tog6;
        _togs[7] = tog7;
        TogStatus = _togs;
        changed = true;
    }

}
