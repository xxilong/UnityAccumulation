using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用户管理
/// </summary>
public class UIUserPanel : MonoBehaviour
{
    public GameObject userBtnPrefab;
    public Transform content;
    public Text nameText;
    public Button btnExit;

    private Dictionary<string, UIUserBtn> userList = new Dictionary<string, UIUserBtn>();
    private bool check, checkDone,addUser, removeUser = false;
    private string[] userArray;
    private string[] msgArray;

    private void OnEnable()
    {
        //MyClientManager.instance.onReseiveMsgAction += OnReseiveMsg;
        nameText.text = VideoChat.instance.UserName;
        CheckUser();
    }

    // Start is called before the first frame update
    void Start()
    {
        btnExit.onClick.AddListener(OnBtnExitClick);
    }

    private void OnBtnExitClick()
    {
        ClearUser();
        gameObject.SetActive(false);
        //UILogin.instance.LoginOut();
        //UILogin.instance.Open();
        LoginManager.instance.Disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (check&&checkDone&&userArray.Length>0)
        {
            for (int i = 0; i < userArray.Length; i++)
            {
                if (userArray[i]!=VideoChat.instance.UserName)
                {
                    AddUser(userArray[i]);
                }                
            }
            check = false;
        }
        if (addUser)
        {
            AddUser(msgArray[0]);
            addUser = false;
        }
        if (removeUser)
        {
            RemoveUser(msgArray[0]);
            removeUser = false;
        }
        //if (!MyClientManager.instance.connected)
        //{
        //    OnBtnExitClick();
        //}
    }

    /// <summary>
    /// 查询用户
    /// </summary>
    public void CheckUser()
    {
        check = true;
        checkDone = false;
        //MyClientManager.instance.Send("CheckUser");
    }

    /// <summary>
    /// 获取用户查询结果
    /// </summary>
    /// <param name="obj"></param>
    private void OnReseiveMsg(string obj)
    {
        if (check && !checkDone&&obj.StartsWith("UserList:"))
        {
            obj=obj.Remove(0,9);            
            userArray = obj.Split(',');
            checkDone = true;
        }
        else
        {
            msgArray = obj.Split('_');
            if (msgArray.Length>1)
            {
                if (msgArray[1] == "Login")
                {
                    addUser = true;
                }
                else if (msgArray[1] == "LoginOut")
                {
                    removeUser = true;
                }
            }
        }
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="userName"></param>
    public void AddUser(string userName)
    {
        if (userList.ContainsKey(userName)||string.IsNullOrEmpty(userName))
        {
            return;
        }
        UIUserBtn userBtn= Instantiate(userBtnPrefab, content).GetComponent<UIUserBtn>();
        userBtn.name = userName;
        userBtn.UserName = userName;
        userList.Add(userName, userBtn);
    }

    /// <summary>
    /// 移除用户
    /// </summary>
    /// <param name="userName"></param>
    public void RemoveUser(string userName)
    {
        if (userList.ContainsKey(userName))
        {
            Destroy(userList[userName].gameObject);
            userList.Remove(userName);
        }
    }

    public void ClearUser()
    {
        foreach (var item in userList)
        {
            Destroy(item.Value.gameObject);
        }
        userList.Clear();
    }

    private void OnDisable()
    {
        //MyClientManager.instance.onReseiveMsgAction -= OnReseiveMsg;
        //ClearUser();
    }
}
