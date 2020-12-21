using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Byn.Media;
using Byn.Unity.Examples;
using Byn.Net;
using System;

public class VideoChat : MonoBehaviour
{
    public static VideoChat instance;
    public UIVideoChat ui;
    public UIUserPanel userPanel;

    /// <summary>
    /// This is a test server. Don't use in production! The server code is in a zip file in WebRtcNetwork
    /// </summary>
    public string uSignalingUrl = "ws://signaling.because-why-not.com/callapp";

    /// <summary>
    /// By default the secure version is currently only used in WebGL builds as
    /// some browsers require. Unity old mono version comes with a SSL implementation
    /// that can be quite slow and hangs sometimes
    /// </summary>
    public string uSecureSignalingUrl = "wss://signaling.because-why-not.com/callapp";

    /// <summary>
    /// If set to true only the secure signaling url will be used.
    /// </summary>
    public bool uForceSecureSignaling = false;

    /// <summary>
    /// Ice server is either a stun or a turn server used to get trough
    /// the firewall.
    /// Warning: make sure the url is in a valid format and
    /// starts with stun: or turn:
    /// 
    /// WebRTC will try many different ways to connect the peers so if
    /// this server is not available it might still be able
    /// to establish a direct connection or use the second ice server.
    /// 
    /// If you need more than two servers change the CreateNetworkConfig
    /// method.
    /// </summary>
    public string uIceServer = "stun:stun.because-why-not.com:443";

    //
    public string uIceServerUser = "";
    public string uIceServerPassword = "";
    /// <summary>
    /// Second ice server. As I can't guarantee the test server is always online.
    /// If you need more than two servers or username / password then
    /// change the CreateNetworkConfig method.
    /// </summary>
    public string uIceServer2 = "stun:stun.l.google.com:19302";

    public bool useVirtualCamera = false;
    private VirtualCamera virtualCamera;
    /// <summary>
    /// Do not change. This length is enforced on the server side to avoid abuse.
    /// </summary>
    public const int MAX_CODE_LENGTH = 256;
    /// <summary>
    /// Call class handling all the functionality
    /// </summary>
    protected ICall mCall;

    /// <summary>
    /// Contains the configuration used for the next call
    /// </summary>
    protected MediaConfig mMediaConfig;

    protected MediaConfig mMediaConfigInUse;
    //Network configuration shared by both
    NetworkConfig netConf;

    public string UserName { get; set; }
    internal string remoteUserName;
    //Address used to connect the right sender & receiver
    private string mUseAddress;
    internal ConnectionId mRemoteUserId = ConnectionId.INVALID;

    private bool mAutoRejoin = false;
    private float mRejoinTime = 2;
    private bool mLocalFrameEvents = true;
    /// <summary>
    /// Used to backup the original sleep timeout value.
    /// Will be restored after a call has ended
    /// </summary>
    private int mSleepTimeoutBackup;
    /// <summary>
    /// For customization. Set to false to allow devices to sleep
    /// even if a call is active. 
    /// </summary>
    private bool mBlockSleep = true;

    /// <summary>
    /// Set to true after Join is called.
    /// Set to false after either Join failed or the call
    /// ended / network failed / user exit
    /// 
    /// </summary>
    private bool mCallActive = false;
    private bool getCall,endCall,changeDevice = false;

    private string[] devices;
    private int devicesId=0;
    private bool isAnswerer = false;
    private string callTarget;
    
    private void Awake()
    {
        instance = this;
        mMediaConfig = CreateMediaConfig();
        mMediaConfigInUse = mMediaConfig;
        virtualCamera = FindObjectOfType<VirtualCamera>();
    }
    void Start()
    {
        ui = FindObjectOfType<UIVideoChat>();        
        
        //MyClientManager.instance.onReseiveMsgAction += OnReseiveMsg;        
    }     
    /// <summary>
    /// Called by Unitys update loop. Will refresh the state of the call, sync the events with
    /// unity and trigger the event callbacks below.
    ///
    /// </summary>
    private void Update()
    {
        if (mCall != null)
        {
            //update the call object. This will trigger all buffered events to be fired
            //to ensure it is done in the unity thread at a convenient time.
            mCall.Update();
        }
        if (getCall)
        {
            Join(remoteUserName);
            getCall = false;
        }
        if (endCall)
        {
            EndMyCall();
            endCall = false;
        }
        //if (changeDevice)
        //{
        //    ChangeDevice();
        //    changeDevice = false;
        //}
    }

    private void OnDestroy()
    {
        EndCall();
    }

    private void Append(string txt)
    {
        ui.Append(txt);
    }

    private void OnReseiveMsg(string str)
    {
        string[] msg = str.Split('_');
        if (msg.Length > 1)
        {
            if (msg[1] == "CallIn")//收到通话消息
            {
                remoteUserName = msg[0];
                isAnswerer = true;
                getCall = true;
            }
            else if (msg[1] == "EndCall")//收到结束通话消息
            {
                endCall = true;
            }
            else if (msg[1] == "LoginOut" && msg[0] == remoteUserName)
            {
                endCall = true;
            }
            else if (msg[1] == "ChangeDevice")
            {
                changeDevice = true;
            }
        }
    }

    #region 通话逻辑
    /// <summary>
    /// 拨通远程通话
    /// </summary>
    /// <param name="remoteUser">远程用户</param>
    public void Call(string remoteUser)
    {
        //userPanel.gameObject.SetActive(false);
        //MyClientManager.instance.Send(UserName + "_Call_" + remoteUser);
        remoteUserName = remoteUser;
        string channel = UserName + "_Call_" + remoteUser;
        Join(channel);
        LoginManager.instance.SendMsg(remoteUser, "call " + channel);
    }
    

    /// <summary>
    /// Called by UI when the join buttin is pressed.
    /// </summary>
    /// <param name="address"></param>
    public virtual void Join(string address)
    {               
        if (address.Length > MAX_CODE_LENGTH)
            throw new ArgumentException("Address can't be longer than " + MAX_CODE_LENGTH);
        SetupCall();
        mUseAddress= address;
        ui.Open();
        InternalJoin();        
    }

    /// <summary>
    /// 设置
    /// </summary>
    public void SetupCall()
    {        
        //STEP1: set up our member variables

        //first access to this will boot up the library and create a UnityCallFactory
        //game object which will manage everything in the background.
        if (UnityCallFactory.Instance == null)
        {
            //if it is null something went terribly wrong
            Debug.LogError("UnityCallFactory missing. Platform not supported / dll's missing?");
            return;
        }

        //Use this address to connect. Watch out: you use a generic name 
        //and someone else starts the app using the same name you might end up connecting to
        //someone else!
        //mUseAddress = UserName;

        //Set signaling server url. This server is used to reserve the address, to find the 
        //other call object, to exchange connection information (ip, port + webrtc specific info)
        //which are then used later to create a direct connection.
        //"callapp" corresponds to a specific configuration on the server side
        //and also acts as a pool of possible users with addresses we can connect to. 
        netConf = CreateNetworkConfig();

        //setup the server
        mCall = UnityCallFactory.Instance.Create(netConf);
        if (mCall == null)
        {
            Append("Failed to create the call");
            return;
        }
        Append("Call created!");
        mCall.LocalFrameEvents = mLocalFrameEvents;


        string[] devices = UnityCallFactory.Instance.GetVideoDevices();
        if (devices == null || devices.Length == 0)
        {
            Debug.Log("no device found or no device information available");
        }
        else
        {
            foreach (string s in devices)
                Debug.Log("device found: " + s);
        }
        Append("Call created!");
        mCall.CallEvent += Call_CallEvent;

        //this happens in awake now to allow an ui or other external app
        //to change media config before calling SetupCall
        //mMediaConfig = CreateMediaConfig();

        //make a deep clone to avoid confusion if settings are changed
        //at runtime. 
        if (mMediaConfig.VideoDeviceName==virtualCamera?._DeviceName)
        {
            virtualCamera?.gameObject.SetActive(true);
        }
        else
        {
            virtualCamera?.gameObject.SetActive(false);
        }
        mMediaConfigInUse = mMediaConfig.DeepClone();
        Debug.Log("Configure call using MediaConfig: " + mMediaConfigInUse);
        mCall.Configure(mMediaConfigInUse);

        if (mBlockSleep)
        {
            //backup sleep timeout and set it to never sleep
            mSleepTimeoutBackup = Screen.sleepTimeout;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        changeDevice = false;
    }

    private void InternalJoin()
    {
        if (mCallActive)
        {
            Debug.Log("Join call failed. Call is already/still active");
            return;
        }
        Debug.Log("Try listening on address: " + mUseAddress);
        mCallActive = true;
        this.mCall.Listen(mUseAddress);
    }  
   

    /// <summary>
    /// Handler of call events.
    /// 
    /// Can be customized in via subclasses.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Call_CallEvent(object sender, CallEventArgs e)
    {
        switch (e.Type)
        {
            case CallEventType.CallAccepted:
                //Outgoing call was successful or an incoming call arrived
                Append("Connection established");
                mRemoteUserId = ((CallAcceptedEventArgs)e).ConnectionId;
                Debug.Log("New connection with id: " + mRemoteUserId
                    + " audio:" + mCall.HasAudioTrack(mRemoteUserId)
                    + " video:" + mCall.HasVideoTrack(mRemoteUserId));
                break;
            case CallEventType.CallEnded:
                //Call was ended / one of the users hung up -> reset the app                
                if (!changeDevice)
                {
                    Append("Call ended");
                    EndMyCall();
                }
                else
                {
                    ReConnect();
                }
                break;
            case CallEventType.ListeningFailed:
                //listening for incoming connections failed
                //this usually means a user is using the string / room name already to wait for incoming calls
                //try to connect to this user
                //(note might also mean the server is down or the name is invalid in which case call will fail as well)
                mCall.Call(mUseAddress);
                break;

            case CallEventType.ConnectionFailed:
                {
                    Byn.Media.ErrorEventArgs args = e as Byn.Media.ErrorEventArgs;
                    Append("Connection failed error: " + args.ErrorMessage);
                    EndMyCall();
                }
                break;
            case CallEventType.ConfigurationFailed:
                {
                    Byn.Media.ErrorEventArgs args = e as Byn.Media.ErrorEventArgs;
                    Append("Configuration failed error: " + args.ErrorMessage);
                    EndMyCall();
                }
                break;

            case CallEventType.FrameUpdate:
                {

                    //new frame received from webrtc (either from local camera or network)
                    if (e is FrameUpdateEventArgs)
                    {
                        UpdateFrame((FrameUpdateEventArgs)e);
                    }
                    break;
                }

            case CallEventType.Message:
                {
                    //text message received
                    MessageEventArgs args = e as MessageEventArgs;
                    Append(args.Content);
                    break;
                }
            case CallEventType.WaitForIncomingCall:
                {
                    //the chat app will wait for another app to connect via the same string
                    WaitForIncomingCallEventArgs args = e as WaitForIncomingCallEventArgs;
                    Append("Waiting for incoming call address: " + args.Address);
                    break;
                }
        }

    }

    

    protected virtual void UpdateFrame(FrameUpdateEventArgs frameUpdateEventArgs)
    {
        //the avoid wasting CPU time the library uses the format returned by the browser -> ABGR little endian thus
        //the bytes are in order R G B A
        //Unity seem to use this byte order but also flips the image horizontally (reading the last row first?)
        //this is reversed using UI to avoid wasting CPU time

        //Debug.Log("frame update remote: " + frameUpdateEventArgs.IsRemote);

        if (frameUpdateEventArgs.IsRemote == false)
        {
            ui.UpdateLocalTexture(frameUpdateEventArgs.Frame, frameUpdateEventArgs.Format);
        }
        else
        {
            ui.UpdateRemoteTexture(frameUpdateEventArgs.Frame, frameUpdateEventArgs.Format);
        }
    }



    /// <summary>
    /// 结束通话
    /// </summary>
    public void EndCall()
    {
        if (mCallActive)
        {
            EndMyCall();
            LoginManager.instance.SendMsg(remoteUserName, "callend");
        }
    }

    /// <summary>
    /// 结束本地通话
    /// </summary>
    public void EndMyCall()
    {
        isAnswerer = false;
        ResetCall();
        ui.EndCall();
        userPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Destroys the call object and shows the setup screen again.
    /// Called after a call ends or an error occurred.
    /// </summary>
    public virtual void ResetCall()
    {
        //outside quits. don't rejoin automatically
        mAutoRejoin = false;
        InternalResetCall();
    }

    private void InternalResetCall()
    {
        CleanupCall();

        if (mAutoRejoin)
            StartCoroutine(CoroutineRejoin());
    }

    /// <summary>
    /// Destroys the call. Used if unity destroys the object or if a call
    /// ended / failed due to an error.
    /// 
    /// </summary>
    protected virtual void CleanupCall()
    {
        if (mCall != null)
        {
            mCallActive = false;
            mRemoteUserId = ConnectionId.INVALID;
            Debug.Log("Destroying call!");
            mCall.CallEvent -= Call_CallEvent;
            mCall.Dispose();
            mCall = null;
            //call the garbage collector. This isn't needed but helps discovering
            //memory bugs early on.
            Debug.Log("Triggering garbage collection");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Debug.Log("Call destroyed");

            if (mBlockSleep)
            {
                //revert to the original value
                Screen.sleepTimeout = mSleepTimeoutBackup;
            }
        }
    }

    private IEnumerator CoroutineRejoin()
    {
        yield return new WaitForSecondsRealtime(mRejoinTime);
        SetupCall();
        InternalJoin();
    }

    internal void ReConnect()
    {
        mAutoRejoin = true;
        InternalResetCall();
    }

    /// <summary>
    /// Called by ui to send a message.
    /// </summary>
    /// <param name="msg"></param>
    public virtual void Send(string msg)
    {
        this.mCall.Send(msg);
    }

    #endregion

    #region 通话配置
    /// <summary>
    /// Create the default configuration for this CallApp instance.
    /// This can be overwritten in a subclass allowing the creation custom apps that
    /// use a slightly different configuration.
    /// </summary>
    /// <returns></returns>
    public virtual MediaConfig CreateMediaConfig()
    {
        MediaConfig mediaConfig = new MediaConfig();
        //testing echo cancellation (native only)
        bool useEchoCancellation = false;
        if (useEchoCancellation)
        {
#if !UNITY_WEBGL
            var nativeConfig = new Byn.Media.Native.NativeMediaConfig();
            nativeConfig.AudioOptions.echo_cancellation = true;
            nativeConfig.AudioOptions.extended_filter_aec = true;
            nativeConfig.AudioOptions.delay_agnostic_aec = true;

            mediaConfig = nativeConfig;
#endif 
        }

        //use video and audio by default (the UI is toggled on by default as well it will change on click )
        mediaConfig.Audio = true;
        mediaConfig.Video = true;

        if (useVirtualCamera&& FindObjectOfType<VirtualCamera>()!=null)
        {
            mediaConfig.VideoDeviceName = FindObjectOfType<VirtualCamera>()._DeviceName;
        }
        else
        {
            mediaConfig.VideoDeviceName = null;
        }        

        //This format is the only reliable format that works on all
        //platforms currently.
        mediaConfig.Format = FramePixelFormat.ABGR;
        mediaConfig.MinWidth = 480;
        mediaConfig.MinHeight = 360;
        //Larger resolutions are possible in theory but
        //allowing users to set this too high is risky.
        //A lot of devices do have great cameras but not
        //so great CPU's which might be unable to
        //encode fast enough.
        mediaConfig.MaxWidth = 3840;
        mediaConfig.MaxHeight = 2160;

        //will be overwritten by UI in normal use
        mediaConfig.IdealHeight = 3840;
        mediaConfig.IdealWidth = 2160;
        mediaConfig.IdealFrameRate = 30;
        return mediaConfig;
    }
    /// <summary>
    /// Turns on sending audio for the next call.
    /// </summary>
    /// <param name="value"></param>
    public void SetAudio(bool value)
    {
        mMediaConfig.Audio = value;
    }
    /// <summary>
    /// Turns on sending video for the next call.
    /// </summary>
    /// <param name="value"></param>
    public void SetVideo(bool value)
    {
        mMediaConfig.Video = value;
    }    

    /// <summary>
    /// Changes the target resolution that will be used for
    /// sending video streams.
    /// The closest one the camera can handle will be used.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetIdealResolution(int width, int height)
    {
        mMediaConfig.IdealWidth = width;
        mMediaConfig.IdealHeight = height;
    }

    /// <summary>
    /// Sets the ideal FPS.
    /// This has a lower priority than the ideal resolution.
    /// Note that the FPS aren't enforced. It pick
    /// the closest FPS the video device supports.
    /// </summary>
    /// <param name="fps"></param>
    public void SetIdealFps(int fps)
    {
        mMediaConfig.IdealFrameRate = fps;
    }

    /// <summary>
    /// True will show the local video.
    /// False will not return the video and thus
    /// save some CPU work.
    /// </summary>
    /// <param name="showLocalVideo"></param>
    public void SetShowLocalVideo(bool showLocalVideo)
    {
        mLocalFrameEvents = showLocalVideo;
    }

    /// <summary>
    /// Can be used to make the app automatically reconnect
    /// if a sudden disconnect occurred or the other side ends
    /// the connection.
    /// </summary>
    /// <param name="rejoin"></param>
    /// <param name="rejoinTime"></param>
    public void SetAutoRejoin(bool rejoin, float rejoinTime = 2)
    {
        mAutoRejoin = rejoin;
        mRejoinTime = rejoinTime;
    }

    /// <summary>
    /// Forwarded to the call factory.
    /// Returns the loudspeaker status on mobile devices.
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetLoudspeakerStatus()
    {
        //check if call is created to ensure this isn't called before initialization
        if (mCall != null)
        {
            return UnityCallFactory.Instance.GetLoudspeakerStatus();
        }
        return false;
    }

    /// <summary>
    /// Sets the loudspeaker mode via the call factory.
    /// </summary>
    /// <param name="state"></param>
    public void SetLoudspeakerStatus(bool state)
    {
        //check if call is created to ensure this isn't called before initialization
        if (mCall != null)
        {
            UnityCallFactory.Instance.SetLoudspeakerStatus(state);
        }
    }

    /// <summary>
    /// Set to true to mute the microphone.
    /// </summary>
    /// <param name="state"></param>
    public void SetMute(bool state)
    {
        //check if call is created to ensure this isn't called before initialization
        if (mCall != null)
        {
            mCall.SetMute(state);
        }
    }

    /// <summary>
    /// True if the microphone is muted (or sending audio isn't active).
    /// </summary>
    /// <returns></returns>
    public bool IsMute()
    {
        //check if call is created to ensure this isn't called before initialization
        if (mCall != null)
        {
            return mCall.IsMute();
        }
        return true;
    }

    /// <summary>
    /// Allows to control the replay volume of the
    /// remote connection.
    /// </summary>
    /// <param name="volume">
    /// Usually between 0 and 1
    /// </param>
    public virtual void SetRemoteVolume(float volume)
    {
        if (mCall == null)
            return;
        if (mRemoteUserId == ConnectionId.INVALID)
        {
            return;
        }
        mCall.SetVolume(volume, mRemoteUserId);
    }

    /// <summary>
    /// 创建网络配置
    /// </summary>
    /// <returns></returns>
    protected virtual NetworkConfig CreateNetworkConfig()
    {
        NetworkConfig netConfig = new NetworkConfig();
        if (string.IsNullOrEmpty(uIceServer) == false)
            netConfig.IceServers.Add(new IceServer(uIceServer, uIceServerUser, uIceServerPassword));
        if (string.IsNullOrEmpty(uIceServer2) == false)
            netConfig.IceServers.Add(new IceServer(uIceServer2));

        if (Application.platform == RuntimePlatform.WebGLPlayer || uForceSecureSignaling)
        {
            netConfig.SignalingUrl = uSecureSignalingUrl;
        }
        else
        {
            netConfig.SignalingUrl = uSignalingUrl;
        }

        if (string.IsNullOrEmpty(netConfig.SignalingUrl))
        {
            throw new InvalidOperationException("set signaling url is null or empty");
        }
        return netConfig;
    }

    #endregion

    #region 镜头设置

    /// <summary>
    /// 切换本地镜头
    /// </summary>
    internal void ChangeDevice()
    {
        if (devices.Length < 2)
        {
            return;
        }
        print("ChangeDevice");
        devicesId++;
        devicesId = devicesId % devices.Length;
        SetVideoDevice(devices[devicesId]);
        if (mCallActive)
        {
            MyClientManager.instance.Send(UserName + "_ChangeDevice_" + remoteUserName);
            ReConnect();
        }
    }

    /// <summary>
    /// Allows to set a specific video device.
    /// This isn't supported on WebGL yet.
    /// </summary>
    /// <param name="deviceName"></param>
    public void SetVideoDevice(string deviceName)
    {
        mMediaConfig.VideoDeviceName = deviceName;
    }



    /// <summary>
    /// Returns a list of video devices for the UI to show.
    /// This is used to avoid having the UI directly access the UnityCallFactory.
    /// </summary>
    /// <returns></returns>
    public string[] GetVideoDevices()
    {
        if (CanSelectVideoDevice())
        {
            devices = UnityCallFactory.Instance.GetVideoDevices();
            if (devices == null || devices.Length == 0)
            {
                Debug.Log("no device found or no device information available");
            }
            else
            {
                foreach (string s in devices)
                    Debug.Log("device found: " + s);
            }
            return devices;
        }
        else
        {
            return new string[] { "Default" };
        };
    }

    /// <summary>
    /// Used by the UI
    /// </summary>
    /// <returns></returns>
    public bool CanSelectVideoDevice()
    {        
        return UnityCallFactory.Instance.CanSelectVideoDevice();
    }

    #endregion
}
