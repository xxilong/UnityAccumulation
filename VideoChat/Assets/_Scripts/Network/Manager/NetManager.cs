using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum NetType
{
    Server,
    Client,
}

public class NetManager : MonoBehaviour {

    public NetType netType;
    public static bool isStopSync = false;
    public List<GameObject> netGameObjectList;
    public static NetManager Instant;

    Transform serviceManager;
    Transform clientManager;    

    // Use this for initialization
    void Awake() {
        Instant = this;
        serviceManager = transform.Find("ServiceManager");
        clientManager = transform.Find("ClientManager");
        if (serviceManager == null || clientManager == null) {
            Debug.LogError("Error:Must have serviceManager And clientManager");
        }
    }
	void Start () {
        serviceManager.gameObject.SetActive(false);
        clientManager.gameObject.SetActive(false);
        if (netType == NetType.Server) {
            serviceManager.gameObject.SetActive(true);
        } else if (netType == NetType.Client) {
            clientManager.gameObject.SetActive(true);
        }
    }

    public void SendMessage(MessageData msg) {
        if (isStopSync == true)
            return;
        if (netType == NetType.Server) {
            ServiceManager.Instant.SendMessage(msg);
        } else if (netType == NetType.Client) {
            //ClientManager.Instant.SendMessage(msg);
        }
    }
	
}
