using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public static UILogin instance;
    private const string _UserNamePlayerPref = "UserID";

    public InputField _idInput;
    public Button _btnLogin;
    public UIUserPanel userPanel;
    public Text _textInfo;

    private bool isError,getMsg;
    private string msg;

    private bool _loginIn=false;
    private string _loginCmdString;
    private bool _loginDone;
    private string _inputUserId;
    private GameObject child;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0).gameObject;
        //MyClientManager.instance.onReseiveMsgAction += OnReseiveMsg;            

        _btnLogin.onClick.AddListener(LoginIn);

        Open();
    }

    internal void Open()
    {
        child.gameObject.SetActive(true);
        string prefsName = PlayerPrefs.GetString(_UserNamePlayerPref);
        if (!string.IsNullOrEmpty(prefsName))
        {
            _idInput.text = prefsName;
        }
    }

    void Close()
    {
        child.SetActive(false);
        userPanel.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_loginIn&&!_loginDone)
        {
            //VideoChat.instance.Login();
            Close();
            _loginDone = true;

            print(_inputUserId + " Login in");
            //StartCoroutine(Pulse());
        }

        if (isError)
        {
            LogInfo(msg, isError);
            isError = false;
        }
        else if (getMsg)
        {
            LogInfo(msg);
            getMsg = false;
        }
    }
    IEnumerator Pulse()
    {
        while (true)
        {
            yield return new WaitForSeconds(4);
            if (!_loginDone)
            {
                break;
            }
            MyClientManager.instance.Send(VideoChat.instance.UserName+"_Online");
        }
    }
    /// <summary>
    /// 登录
    /// </summary>
    public void LoginIn()
    {        
        _inputUserId = _idInput.text.Trim();
        if (string.IsNullOrEmpty(_inputUserId))
        {
            return;
        }
        PlayerPrefs.SetString(_UserNamePlayerPref, _inputUserId);
        PlayerPrefs.Save();
        _loginCmdString = _inputUserId + "_Login";
        Debug.Log(_loginCmdString);
        VideoChat.instance.UserName = _inputUserId;
        //MyClientManager.instance.Send(_loginCmdString);
        LoginManager.instance.Login(_inputUserId);
    }

    private void OnReseiveMsg(string obj)
    {
        if (obj == _loginCmdString)
        {
            _loginIn = true;
        }
        else if(obj=="用户已存在")
        {
            Debug.LogError(obj);
            GetError(obj);
        }        
    }

    /// <summary>
    /// 登出
    /// </summary>
    public void LoginOut()
    {
        if (_loginDone)
        {
            MyClientManager.instance.Send(VideoChat.instance.UserName + "_LoginOut");
            _loginIn = false;
            _loginDone = false;
        }        
    }       

    public void GetError(string msg)
    {
        isError = true;
        this.msg = msg;
    }

    public void GetMsg(string msg)
    {
        getMsg = true;
        this.msg = msg;
    }

    public void LogInfo(string msg,bool isError = false)
    {
        if (isError)
        {
            _textInfo.color = Color.red;            
        }
        else
        {
            _textInfo.color = Color.white;
        }
        _textInfo.text = msg;
    }
}
