using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;

/// <summary>
/// 登录管理，负责用户的登录、用户列表管理
/// </summary>
public class LoginManager : MonoBehaviour,IChatClientListener
{
    public static LoginManager instance;
    public GameObject panelLogin;
    public UIUserPanel panelUsers;

    public string channelsToJoinOnConnect;
    public ChatClient chatClient;
    public HashSet<string> friendsList=new HashSet<string>();
    public string UserName { get; set; }
    protected internal ChatSettings chatAppSettings;
    private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        chatAppSettings = ChatSettings.Instance;
        panelLogin.SetActive(true);
        panelUsers.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }
    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        Disconnect();
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        Disconnect();
    }

    public void Login(string userName)
    {
        this.UserName = userName;
        chatClient = new ChatClient(this);        
        chatClient.Connect(chatAppSettings.AppId, "1.0", new AuthenticationValues(UserName));
    }

    public void Disconnect()
    {
        if (chatClient != null)
        {
            chatClient.Disconnect();
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //TODO
    }

    public void OnChatStateChange(ChatState state)
    {
        //TODO
        Debug.LogFormat("{0}:{1}", UserName, state);
    }

    /// <summary>
    /// 连接
    /// </summary>
    public void OnConnected()
    {
        if (!string.IsNullOrEmpty(channelsToJoinOnConnect))
        {
            chatClient.Subscribe(channelsToJoinOnConnect, 0, -1, new ChannelCreationOptions{ PublishSubscribers = true});
        }
        //chatClient.AddFriends(new string[]{ "成都"});
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        Debug.LogFormat("Connected: channel=\"{0}\" userId=\"{1}\"", channelsToJoinOnConnect, UserName);
        //订阅频道
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void OnDisconnected()
    {
        panelUsers.ClearUser();
        panelUsers.gameObject.SetActive(false);
        panelLogin.SetActive(true);
        //TODO
    }

    public void SendMsg(string getter,string message)
    {
        chatClient.SendPrivateMessage(getter, message);
        //chatClient.PublishMessage(channelsToJoinOnConnect, message);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //TODO
        if (channelName!= channelsToJoinOnConnect)
        {
            return;
        }        
        Debug.LogFormat("Recieve message {0} from {1}", messages[0], senders[0]);
        //string msg = messages[0] as string;
        //if (msg == "callend")
        //{
        //    VideoChat.instance.EndMyCall();
        //}
        //string[] args = msg.Split(' ');
        //if (args[0] == "call" && args[1] != null)
        //{
        //    VideoChat.instance.remoteUserName = senders[0];
        //    VideoChat.instance.Join(args[1]);
        //}
    }

    /// <summary>
    /// 接收到私聊信息事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="channelName"></param>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //TODO  
        Debug.LogFormat("Recieve message {0} from {1}", message, sender);
        if (sender==UserName)
        {
            return;
        }
        string msg = message as string;
        if (msg == "callend")
        {
            VideoChat.instance.EndMyCall();
        }
        string[] args = msg.Split(' ');
        if (args[0] == "call" && args[1] != null)
        {
            VideoChat.instance.remoteUserName = sender;
            VideoChat.instance.Join(args[1]);
        }
    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //TODO
        Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));

        if (this.friendListItemLUT.ContainsKey(user))
        {
            FriendItem _friendItem = this.friendListItemLUT[user];
            if (_friendItem != null) _friendItem.OnFriendStatusUpdate(status, gotMessage, message);
        }
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        //登录成功
        panelLogin.SetActive(false);
        panelUsers.gameObject.SetActive(true);
        GetUsers();
        //刷新用户列表
        Debug.LogFormat("UserSubscribed: channel=\"{0}\" userId=\"{1}\" result=\"{2}\"", channels[0], UserName,results[0]);
    }

    private void GetUsers()
    {
        friendsList = chatClient.PublicChannels[channelsToJoinOnConnect].Subscribers;
        Debug.Log("Subscribers Count:" + friendsList.Count);
        foreach (var item in friendsList)
        {
            if (item!=UserName)
            {
                panelUsers.AddUser(item);
            }            
        }       
    }

    public void OnUnsubscribed(string[] channels)
    {
        //TODO
        //退出登录
    }

    /// <summary>
    /// 当其它用户登录
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="user"></param>
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("UserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
        panelUsers.AddUser(user);
    }

    /// <summary>
    /// 当其它用户退出登录
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="user"></param>
    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
        panelUsers.RemoveUser(user);
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }
    }
}
