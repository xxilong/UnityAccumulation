using UnityEngine;
using System.Collections;
using System;

public abstract class NetSyncBasic : MonoBehaviour
{
    internal int identiy = -1;

    private void Awake()
    {
        //TcpReceiveManager.receiveMsgAction += ReceiveMsgAction;
    }

    // Use this for initialization
    protected virtual void Start()
    {
        //for (int i = 0; i < NetManager.Instant.netGameObjectList.Count; i++)
        //{
        //    if (NetManager.Instant.netGameObjectList[i] == gameObject)
        //    {
        //        identiy = i;
        //        break;
        //    }
        //}

        for (int i = 0; i < MyNetManager.instance.netGameObjectList.Count; i++)
        {
            if (MyNetManager.instance.netGameObjectList[i] == this)
            {
                identiy = i;
                break;
            }
        }
    }

    protected abstract void ReceiveMsgAction(byte[] msg);

    public virtual void ReceiveSignal(string signal)
    {
        if (string.IsNullOrEmpty(signal))
        {
            return;
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    protected virtual void SendMsg(MessageData msgData)
    {
        if (identiy == -1)
        {
            Debug.LogError("Error please Drag the GameObject to NetManager netGameobjectList slot");
            return;
        }
        NetManager.Instant.SendMessage(msgData);
    }

    void OnDestroy()
    {
        TcpReceiveManager.receiveMsgAction -= ReceiveMsgAction;
    }
}
