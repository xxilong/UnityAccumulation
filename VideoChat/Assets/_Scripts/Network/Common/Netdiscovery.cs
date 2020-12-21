using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
public class Netdiscovery : NetworkDiscovery {

    public bool isOnReceivedBroadcast = false;
    public event Action OnReceivedBroadcastAction;
    private string _ip;
    public string ip {
        get {
            if (_ip != "") {
                return _ip;
            }
            return "";
        }
    }
    private int _port;
    public int port {
        get {
            if (_port != -1) {
                return _port;
            }
            return -1;
        }
    }
    public override void OnReceivedBroadcast(string fromAddress, string data) {
        if (data.Contains("HELLO")) {
            Debug.Log("wangcq327 --- Received broadcast from: " + fromAddress + " with the data:" + data);

            var items = data.Split(':');
            var itemsaddr = fromAddress.Split(':');
            _ip = itemsaddr[3];
            //_port = Convert.ToInt32(items[2]);
            if (OnReceivedBroadcastAction != null) {
                OnReceivedBroadcastAction();
            }
            
            if (items.Length == 3 && items[0] == "NetworkManager" && isOnReceivedBroadcast == false) {
                if (NetworkManager.singleton != null && NetworkManager.singleton.client == null) {
                    NetworkManager.singleton.networkAddress = itemsaddr[3];
                    NetworkManager.singleton.networkPort = Convert.ToInt32(items[2]);
                    Debug.Log("wangcq327 --- StartClient");
                    NetworkManager.singleton.StartClient();
                    isOnReceivedBroadcast = true;
                    GameObject.Find("ControlPanel").transform.Find("Info").GetComponent<Text>().text += "\n Connected to Server IP:" + itemsaddr[3] + "; Port:" + Convert.ToInt32(items[2]);
                }
            }
        }
    }


    public void StartBroadCast() {
        Initialize();
        StartAsServer();
    }

    public void ListenBroadCast() {
        Initialize();
        StartAsClient();
    }
    
}