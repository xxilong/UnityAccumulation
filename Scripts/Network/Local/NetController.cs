using UnityEngine;
using System.Collections;
using ShareLib.Net;
using System;
using ShareLib.Unity;
using System.Timers;
using ShareLib.Ayz;

public class NetController : MonoBehaviour
{
    public static NetController instance;
    private CmdControlClient client = new CmdControlClient();
    public string ip = "192.168.0.132";
    int port = 3131;
    Timer timer;
    public bool right = false;
    private bool stop = true;
    private bool start=false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {     

        
        //Connect();
    }

    public void Connect()
    {
        Debug.Log("Sensor Server Port(Default 3131): ");
        client.SetConnectStatusListener(ConnectResult);
        client.SetReciver(OnReseiveSingal);
        stop = false;
        client.Connect(ip, port);
    }

    private void OnReseiveSingal(string str)
    {
        Debug.Log(str);
        if (str=="engine good")
        {
            right = true;
        }
        else if (str == "engine bad")
        {
            right = false;
        }
    }

    private  void Update()
    {

    }

    private void ConnectResult(bool success)
    {
        if (success)
        {
            Debug.Log("Server Connected.");
           
            if (!start)
            {
                SendCommand("go start");
                start = true;
            }
        }
        else
        {
            Debug.Log("Server Connect Failed.");
            if (!stop)
            {
                timer = Delay.Run(3000, new Action(client.ReConnect));
            }           
        }
    }

    public void CheckStatus()
    {
        client.SendCommand("engine status");        
    }

    public void SendCommand(string cmd)
    {
        client.SendCommand(cmd);
    }

    private void OnDisable()
    {
        DisConnect();
    }

    public void DisConnect()
    {
        stop = true;
        right = false;
        client.Close();
        if (timer != null)
        {
            timer.Close();
        }
    }

    public void Reconnect()
    {
        StartCoroutine(ReconnectCoroutine());
    }

    IEnumerator ReconnectCoroutine()
    {
        DisConnect();
        yield return new WaitForSeconds(1);
        Connect();
    }
}
