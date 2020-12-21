using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncTransform : MonoBehaviour {
    /// <summary>
    /// Transform同步频率  10表示同步10次/s
    /// </summary>
    public int updateFPS = 10;
    float pastTime = 0;
    int identiy = -1;
    string parentName;
    GameObject parent;
    private Vector3 receiveposition = Vector3.zero;
    private Vector3 receiveeulerAngles = Vector3.zero;
    private Vector3 receiveLocalScale = Vector3.zero;
    private bool active;
    bool isReceiveTransform = false;
    // Use this for initialization
    void Start() {
        transform.hasChanged = false;
        TcpReceiveManager.receiveMsgAction += ReceiveTransform;
        for (int i = 0; i < NetManager.Instant.netGameObjectList.Count; i++) {
            if (NetManager.Instant.netGameObjectList[i] == gameObject) {
                identiy = i;
                break;
            }
        }
    }
    void OnDestroy() {
        TcpReceiveManager.receiveMsgAction -= ReceiveTransform;
    }

    // Update is called once per frame
    void Update() {
        pastTime += Time.deltaTime;
        if (pastTime < (1.0f / updateFPS)) {
            return;
        }
        pastTime = 0;

        if (isReceiveTransform == false) {
            if (transform.hasChanged) {
                transform.hasChanged = false;
                SendTransform();
            }
        } else if (isReceiveTransform == true) {
            UpdateReceiveTransform();
        }
    }

    void GetGameObjectParebtPath() {

    }

    void SendTransform() {
        
        if (identiy == -1) {
            Debug.LogError("Error please Drag the GameObject to NetManager netGameobjectList slot");
            return;
        }

        TransferData tData = new TransferData() {
            id = identiy.ToString(),
            text = "Transform",
            posX = transform.localPosition.x.ToString(),
            posY = transform.localPosition.y.ToString(),
            posZ = transform.localPosition.z.ToString(),
            rtX = transform.localEulerAngles.x.ToString(),
            rtY = transform.localEulerAngles.y.ToString(),
            rtZ = transform.localEulerAngles.z.ToString(),
            scX = transform.localScale.x.ToString(),
            scY = transform.localScale.y.ToString(),
            scZ = transform.localScale.z.ToString(),
            parent = transform.parent ? transform.parent.name : null,
            enable = gameObject.activeInHierarchy.ToString()
        };
        NetManager.Instant.SendMessage(tData);
    }

    void ReceiveTransform(byte[] _msg) {
        TransferData aMsg = (TransferData)Tcp_Util.BufferToMessage(_msg);
        if (Convert.ToInt32(aMsg.id) == identiy && aMsg.text == "Transform") {
            byte[] receiveMsg = TcpReceiveManager.getMsg();
            if (receiveMsg != null) {
                MessageData msg = Tcp_Util.BufferToMessage(receiveMsg);
                TransferData td = (TransferData)msg;
                Debug.Log("wangcq327 ---NetTransform:Id" + td.id + ";位置" + td.posX + ";" + td.posY + ";" + td.posZ + ";旋转" + td.rtX + ";" + td.rtY + ";" + td.rtZ + ";缩放" + td.scX + ";" + td.scY + ";" + td.scZ + ";++" + identiy);
                if (Convert.ToInt32(td.id) == identiy && td.id != "-1") {
                    receiveposition = new Vector3(Convert.ToSingle(td.posX), Convert.ToSingle(td.posY), Convert.ToSingle(td.posZ));
                    receiveeulerAngles = new Vector3(Convert.ToSingle(td.rtX), Convert.ToSingle(td.rtY), Convert.ToSingle(td.rtZ));
                    receiveLocalScale = new Vector3(Convert.ToSingle(td.scX), Convert.ToSingle(td.scY), Convert.ToSingle(td.scZ));
                    parentName = td.parent;
                    isReceiveTransform = true;
                }
            }
        }
        
    }

    void UpdateReceiveTransform() {
        transform.localPosition = receiveposition;
        transform.localEulerAngles = receiveeulerAngles;
        transform.localScale = receiveLocalScale;
        isReceiveTransform = false;
    }
}
