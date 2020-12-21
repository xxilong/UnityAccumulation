using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISet : MonoBehaviour
{
    public InputField inputField;
    public Button btnSure;
    private const string ServerPrefKey = "ServerIP";
    // Start is called before the first frame update
    private void OnEnable()
    {
        inputField.text = PlayerPrefs.GetString(ServerPrefKey, "192.168.0.132");
    }

    private void Start()
    {
        btnSure.onClick.AddListener(OnBtnSureClick);
    }

    private void OnBtnSureClick()
    {
        string str = inputField.text;
        if (!String.IsNullOrEmpty(str)&&str!=MyClientManager.instance.serverIP)
        {
            PlayerPrefs.SetString("ServerPrefKey", str);
            PlayerPrefs.Save();
            MyClientManager.instance.serverIP = str;
            MyClientManager.instance.ReConnect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
