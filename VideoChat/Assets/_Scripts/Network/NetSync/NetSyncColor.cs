using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetSyncColor : MonoBehaviour
{
    /// <summary>
    /// Transform同步频率  10表示同步10次/s
    /// </summary>
    public int updateFPS = 10;
    float pastTime = 0;
    int identiy = -1;
    Color oldColor;
    string receiveColorString;
    Material material;
    Image image;
    bool isReceiveColor = false;
    // Use this for initialization
    void Start()
    {
        if (GetComponent<Renderer>() != null)
        {
            material = GetComponent<Renderer>().material;
        }
        else if (GetComponent<Graphic>() != null)
        {
            material = GetComponent<Graphic>().material;
        }
        oldColor = material.color;
        transform.hasChanged = false;
        TcpReceiveManager.receiveMsgAction += ReceiveColor;
        for (int i = 0; i < NetManager.Instant.netGameObjectList.Count; i++)
        {
            if (NetManager.Instant.netGameObjectList[i] == gameObject)
            {
                identiy = i;
                break;
            }
        }
    }
    void OnDestroy()
    {
        TcpReceiveManager.receiveMsgAction -= ReceiveColor;
    }

    // Update is called once per frame
    void Update()
    {
        pastTime += Time.deltaTime;
        if (pastTime < (1.0f / updateFPS))
        {
            return;
        }
        pastTime = 0;

        if (isReceiveColor == false)
        {
            if (oldColor != material.color)
            {
                oldColor = material.color;
                SendColor();
            }
        }
        else if (isReceiveColor == true)
        {
            UpdateReceiveTransform();
        }
    }

    void GetGameObjectParebtPath()
    {

    }

    void SendColor()
    {
        if (identiy == -1)
        {
            Debug.LogError("Error please Drag the GameObject to NetManager netGameobjectList slot");
            return;
        }
        TransferData tData = new TransferData()
        {
            id = identiy.ToString(),
            text = "color",
            color = ColorUtility.ToHtmlStringRGB(material.color),
        };
        NetManager.Instant.SendMessage(tData);
    }

    void ReceiveColor(byte[] _msg)
    {
        TransferData aMsg = (TransferData)Tcp_Util.BufferToMessage(_msg);
        if (Convert.ToInt32(aMsg.id) == identiy && aMsg.text == "color")
        {
            byte[] receiveMsg = TcpReceiveManager.getMsg();
            if (receiveMsg != null)
            {
                MessageData msg = Tcp_Util.BufferToMessage(receiveMsg);
                TransferData td = (TransferData)msg;
                Debug.Log("wangcq327 ---NetColor:Id" + td.id + ";颜色 #" + td.color + ";++" + identiy);
                if (Convert.ToInt32(td.id) == identiy && td.id != "-1")
                {
                    receiveColorString = td.color;
                    isReceiveColor = true;
                }
            }
        }
    }

    void UpdateReceiveTransform()
    {
        Color receiveColor;
        ColorUtility.TryParseHtmlString("#" + receiveColorString, out receiveColor);
        material.color = receiveColor;
        isReceiveColor = false;
    }
}
