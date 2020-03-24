using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour {

    Netdiscovery netDiscovery;
    Tcp_Server netServer;
    public static ServiceManager Instant;
    void Awake() {
        Instant = this;
    }

    // Use this for initialization
    void Start () {
        if (GetComponent<Tcp_Server>() == null) {
            gameObject.AddComponent<Tcp_Server>();
        }
        if (GetComponent<Netdiscovery>() == null) {
            gameObject.AddComponent<Netdiscovery>();
        }

        netServer = GetComponent<Tcp_Server>();
        netDiscovery = GetComponent<Netdiscovery>();
        //开始广播
        StartBroadCast();
        StartServer();
    }
	
	// Update is called once per frame
	void Update () {
       
    }

    void StartServer() {
        Debug.Log("wangcq327 --- local IP:" + Tcp_Util.GetAddressIP());
        netServer.StartUp();
    }
    void StartBroadCast() {
        netDiscovery.StartBroadCast();
    }

    public void SendMessage(MessageData msg) {
        byte[] bmsg = CommonAPI.SerializeMessage(msg);
        netServer.SendAll(bmsg);
    }

}
