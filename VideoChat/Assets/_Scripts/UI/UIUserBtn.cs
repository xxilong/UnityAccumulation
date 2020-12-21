using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUserBtn : MonoBehaviour
{
    public string UserName { get; set; }
    public Button btnCall;
    public Text nameText;
    // Start is called before the first frame update
    void Start()
    {
        btnCall.onClick.AddListener(CallUser);
        nameText.text = UserName;
    }

    /// <summary>
    /// 呼叫此按钮对应的用户
    /// </summary>
    private void CallUser()
    {
        VideoChat.instance.Call(UserName);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
