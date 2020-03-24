using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {

    public string serverIP;
    Netdiscovery netDiscovery;
    Tcp_Client netClient;
    public static ClientManager Instant;
    bool isListenBroadCast = true;
    // Use this for initialization
    void Awake() {
        Instant = this;
    }

    void Start () {
        if (GetComponent<Tcp_Client>() == null) {
            gameObject.AddComponent<Tcp_Client>();
        }
        if (GetComponent<Netdiscovery>() == null) {
            gameObject.AddComponent<Netdiscovery>();
        }
        netDiscovery = GetComponent<Netdiscovery>();
        netClient = GetComponent<Tcp_Client>();
        netDiscovery.OnReceivedBroadcastAction += StartClient;
        StartListenBroadCast();
    }
	
	// Update is called once per frame
	void Update () {

    }

    
    void StartClient() {
        StopListenBroadCast();
        Debug.Log("wangcq327 --- local IP:" + Tcp_Util.GetAddressIP());
        //if (!string.IsNullOrEmpty(serverIP))
        //{
        //    netClient.StartUp(serverIP, 50001);
        //}
        netClient.StartUp(netDiscovery.ip, 50001);
    }
    void StartListenBroadCast() {
        netDiscovery.ListenBroadCast();
    }

    void StopListenBroadCast() {
        netDiscovery.OnReceivedBroadcastAction -= StartClient;
    }
    void OnDestroy() {
        netDiscovery.OnReceivedBroadcastAction -= StartClient;
    }

    public void SendMessage(MessageData msg) {
        byte[] bmsg = CommonAPI.SerializeMessage(msg);
        if (netClient != null) {
            netClient.Send(bmsg);
        }
    }

}
