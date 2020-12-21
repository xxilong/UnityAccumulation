using ShareLib.Ayz;
using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class NetSyncManager : MonoBehaviour {

    public static NetSyncManager instance;

    public NetType netType;
    public List<GameObject> netGameObjectList;

    private CmdTcpServer _server = new CmdTcpServer();
    private CmdControlClient client = new CmdControlClient();

    private bool _IsEngineError = true;
    private string serverIP = "192.168.1.160";
    private int port = 3131;
    private Timer timer;
    public bool getSignal = false;


    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        serverIP = Tcp_Util.GetAddressIP();
        if (netType==NetType.Server)
        {
            _server.SetReciverListener(delegate (string line, string peername)
            {
                Debug.Log("收到客户端 " + peername + " 命令: " + line);
                CmdLine cmdLine = new CmdLine(line);
            });
            _server.Start(3131);
        }
        else
        {
            client.SetConnectStatusListener(ConnectResult);
            client.SetReciver(OnReseiveCmd);
            client.Connect(serverIP, port);
        }      
        
    }

    public void SendCommand(string cmd)
    {
        _server.SendCommand(cmd);
    }


    private void ConnectResult(bool success)
    {
        if (success)
        {
            Debug.Log("Server Connected.");
        }
        else
        {
            Debug.Log("Server Connect Failed.");
            timer = Delay.Run(3000, new Action(client.ReConnect));
        }
    }

    private void OnReseiveCmd(string obj)
    {
        if (true)
        {

        }
    }


	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        ThreadPool.CheckInUpdate(Time.time);
#endif
    }

    private void OnDisable()
    {
        Close();
    }

    public void Close()
    {
        _server.Stop();
        client.Close();
        if (timer != null)
        {
            timer.Close();
        }
    }
}
