using UnityEngine;
using System.Collections;
using ShareLib.Net;
using System.Timers;
using ShareLib.Unity;
using System;

public class MyClientManager : MonoBehaviour
{
    public static MyClientManager instance;

    public string serverIP = "192.168.0.160";

    private CmdControlClient client = new CmdControlClient();    
    [SerializeField]
    private int port = 3132;
    private Timer timer;
    private bool stop = true;
    public Action<string> onReseiveMsgAction;
    internal bool connected = false;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        Debug.LogFormat("Server Port(Default {0}): ",port.ToString());
        client.SetConnectStatusListener(ConnectResult);
        client.SetReciver(OnReseiveSingal);
        
        Connect();
    }

    public void Connect()
    {        
        stop = false;
        client.Connect(serverIP, port);
    }

    private void OnReseiveSingal(string str)
    {
        Debug.Log(str);
        if (onReseiveMsgAction!=null)
        {            
            onReseiveMsgAction(str);
        }        
    }

    private void ConnectResult(bool success)
    {
        if (success)
        {
            Debug.Log("Server Connected.");
            UILogin.instance.GetMsg("服务器连接成功");
            connected = true;
        }
        else
        {
            Debug.Log("Server Connect Failed.");
            UILogin.instance.GetError("服务器连接失败");
            connected = false;
            if (!stop)
            {
                timer = Delay.Run(3000, new Action(client.ReConnect));
            }
        }
    }

    public void Send(string str)
    {
        client.SendCommand(str);
    }


    private void OnDisable()
    {
        DisConnect();
    }

    public void DisConnect()
    {
        stop = true;
        client.Close();
        if (timer != null)
        {
            timer.Stop();
            timer.Enabled = false;
        }
    }

    public void ReConnect()
    {
        StartCoroutine(ReConnectCort());
    }

    IEnumerator ReConnectCort()
    {
        DisConnect();
        yield return new WaitForSeconds(1);
        Connect();
    }

    private void Update()
    {
        ThreadPool.CheckInUpdate();
    }
}
