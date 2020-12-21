using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ShareLib.Unity;
using System;

public class MyNetManager : MonoBehaviour
{
    public static MyNetManager instance;
    public NetType netType;
    private MyServerManager serverManager;
    private MyClientManager clientManager;
    public List<NetSyncBasic> netGameObjectList = new List<NetSyncBasic>();
    

    private void Awake()
    {
        instance = this;

        serverManager = GetComponentInChildren<MyServerManager>();
        clientManager = GetComponentInChildren<MyClientManager>();
        if (netType == NetType.Server)
        {
            serverManager.gameObject.SetActive(true);
            clientManager.gameObject.SetActive(false);
        }
        else
        {
            serverManager.gameObject.SetActive(false);
            clientManager.gameObject.SetActive(true);
        }
    }

    // Use this for initialization
    void Start()
    {
        

        for (int i = 0; i < netGameObjectList.Count; i++)
        {
            netGameObjectList[i].identiy = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ThreadPool.CheckInUpdate(Time.time);
    }

    private void OnReseiveMsg(string str)
    {
        Debug.Log("ReseiveMsg:" + str);
        string[] ar = str.Split(' ');
        int i = -1;
        if (int.TryParse(ar[0],out i)&&i>0&&i<netGameObjectList.Count)
        {
            netGameObjectList[i].ReceiveSignal(ar[1]);
        }
    }

    public void Send(string msg)
    {
        if (netType== NetType.Server)
        {
            serverManager.Send(msg);
        }
        else
        {
            clientManager.Send(msg);
        }
        
    }

    private void OnDestroy()
    {
       
    }
}
