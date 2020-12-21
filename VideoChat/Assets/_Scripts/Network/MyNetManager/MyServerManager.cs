using UnityEngine;
using System.Collections;
using ShareLib.Ayz;
using ShareLib.Log;
using ShareLib.Net;
using System;

public class MyServerManager : MonoBehaviour
{
    public static MyServerManager instance;
    private CmdTcpServer _server = new CmdTcpServer();

    private bool _IsEngineError = true;
    public Action<string> reseiveMsgAction;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        _server.SetReciverListener(delegate (string line, string peername)
        {
            ReciverListener(line, peername);
        });
        _server.Start(3132);
    }

    private void ReciverListener(string line, string peername)
    {
        Debug.Log("收到客户端 " + peername + " 命令: " + line);
        if (reseiveMsgAction != null)
        {
            reseiveMsgAction(line);
        }
    }

    public void Send(string msg)
    {
        _server.SendCommand(msg);
    }
}
