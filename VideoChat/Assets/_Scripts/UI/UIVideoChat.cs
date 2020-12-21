using Byn.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVideoChat : MonoBehaviour
{
    [Header("Video and Chat panel")]
    public RectTransform uInCallBase;
    public RectTransform uVideoPanel;
    public RectTransform uChatPanel;
    public RectTransform uVideoOverlay;
    public Button uBtnChangeDevice;

    [Header("Default positions/transformations")]
    public RectTransform uVideoBase;
    public RectTransform uChatBase;


    [Header("Fullscreen positions/transformations")]
    public RectTransform uFullscreenPanel;
    public RectTransform uVideoBaseFullscreen;
    public RectTransform uChatBaseFullscreen;
    [Header("Chat panel elements")]
    /// <summary>
    /// Input field to enter a new message.
    /// </summary>
    public InputField uMessageInputField;

    /// <summary>
    /// Output message list to show incoming and sent messages + output messages of the
    /// system itself.
    /// </summary>
    public MessageList uMessageOutput;

    /// <summary>
    /// Send button.
    /// </summary>
    public Button uSendMessageButton;

    /// <summary>
    /// Shutdown button. Disconnects all connections + shuts down the server if started.
    /// </summary>
    public Button uShutdownButton;

    /// <summary>
    /// Toggle to switch the microphone on / off. 
    /// </summary>
    public Toggle uMuteToggle;

    /// <summary>
    /// Toggle to switch the loudspeakers on / off. Only for mobile visible.
    /// </summary>
    public Toggle uLoudspeakerToggle;

    /// <summary>
    /// Slider to just the remote users volume.
    /// </summary>
    public Slider uVolumeSlider;

    /// <summary>
    /// Slider to just the remote users volume.
    /// </summary>
    public Text uOverlayInfo;

    public Text uRemoteUserText;
    [Header("Video panel elements")]
    /// <summary>
    /// Image of the local camera
    /// </summary>
    public RawImage uLocalVideoImage;

    /// <summary>
    /// Image of the remote camera
    /// </summary>
    public RawImage uRemoteVideoImage;

    [Header("Resources")]
    public Texture2D uNoCameraTexture;

    private float mVideoOverlayTimeout = 0;
    private static readonly float sDefaultOverlayTimeout = 8;
    private float mFpsTimer = 0;

    protected bool mFullscreen = false;

    private bool mHasLocalVideo = false;
    private int mLocalVideoWidth = -1;
    private int mLocalVideoHeight = -1;
    private int mLocalFps = 0;
    private int mLocalFrameCounter = 0;
    private FramePixelFormat mLocalVideoFormat = FramePixelFormat.Invalid;

    private bool mHasRemoteVideo = false;
    private int mRemoteVideoWidth = -1;
    private int mRemoteVideoHeight = -1;
    private int mRemoteFps = 0;
    private int mRemoteFrameCounter = 0;
    private FramePixelFormat mRemoteVideoFormat = FramePixelFormat.Invalid;

    /// <summary>
    /// Texture of the local video
    /// </summary>
    protected Texture2D mLocalVideoTexture = null;

    /// <summary>
    /// Texture of the remote video
    /// </summary>
    protected Texture2D mRemoteVideoTexture = null;


    protected VideoChat videoChat;

    private string mPrefix = "CallAppUI_";
    private static readonly string PREF_AUDIO = "audio";
    private static readonly string PREF_VIDEO = "video";
    private static readonly string PREF_VIDEODEVICE = "videodevice";
    private static readonly string PREF_ROOMNAME = "roomname";
    private static readonly string PREF_IDEALWIDTH = "idealwidth";
    private static readonly string PREF_IDEALHEIGHT = "idealheight";
    private static readonly string PREF_IDEALFPS = "idealfps";
    private static readonly string PREF_REJOIN = "rejoin";
    private static readonly string PREF_LOCALVIDEO = "localvideo";


    protected virtual void Awake()
    {
        videoChat = FindObjectOfType<VideoChat>();

        if (Application.isMobilePlatform == false)
            uLoudspeakerToggle.gameObject.SetActive(false);
        mPrefix += this.gameObject.name + "_";
    }
    // Start is called before the first frame update
    void Start()
    {
        uShutdownButton.onClick.AddListener(OnBtnShutDownClick);
        uSendMessageButton.onClick.AddListener(SendButtonPressed);
        uMessageInputField.onEndEdit.AddListener(InputOnEndEdit);
        uMuteToggle.onValueChanged.AddListener(OnMuteToggle);
        uLoudspeakerToggle.onValueChanged.AddListener(OnLoudspeakerToggle);
        uVolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        uBtnChangeDevice.onClick.AddListener(OnBtnChangeDeviceClick);
    }

    

    public void Open()
    {
        uInCallBase.gameObject.SetActive(true);
        uVideoPanel.gameObject.SetActive(true);
        SetFullscreen(false);
        uRemoteUserText.text = videoChat.remoteUserName;        
        uBtnChangeDevice.gameObject.SetActive(videoChat.CanSelectVideoDevice()&& videoChat.GetVideoDevices().Length>1);
    }

    private static bool PlayerPrefsGetBool(string name, bool defval)
    {
        int def = 0;
        if (defval)
            def = 1;
        return PlayerPrefs.GetInt(name, def) == 1 ? true : false;
    }

    private static void PlayerPrefsSetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    protected virtual void Update()
    {
        if (mVideoOverlayTimeout > 0)
        {
            string local = "Local:";
            if (mHasLocalVideo == false)
            {
                local += "no video";
            }
            else
            {
                local += mLocalVideoWidth + "x" + mLocalVideoHeight + Enum.GetName(typeof(FramePixelFormat), mLocalVideoFormat) + " FPS:" + mLocalFps;
            }
            string remote = "Remote:";
            if (mHasRemoteVideo == false)
            {
                remote += "no video";
            }
            else
            {
                remote += mRemoteVideoWidth + "x" + mRemoteVideoHeight + Enum.GetName(typeof(FramePixelFormat), mRemoteVideoFormat) + " FPS:" + mRemoteFps;
            }

            uOverlayInfo.text = local + "\n" + remote;
            mVideoOverlayTimeout -= Time.deltaTime;
            if (mVideoOverlayTimeout <= 0)
            {
                mVideoOverlayTimeout = 0;
                uVideoOverlay.gameObject.SetActive(false);
            }
        }

        float fpsTimeDif = Time.realtimeSinceStartup - mFpsTimer;
        if (fpsTimeDif > 1)
        {
            mLocalFps = Mathf.RoundToInt(mLocalFrameCounter / fpsTimeDif);
            mRemoteFps = Mathf.RoundToInt(mRemoteFrameCounter / fpsTimeDif);
            mFpsTimer = Time.realtimeSinceStartup;
            mLocalFrameCounter = 0;
            mRemoteFrameCounter = 0;
        }
    }

    /// <summary>
    /// Updates the local video. If the frame is null it will hide the video image
    /// </summary>
    /// <param name="frame"></param>
    public virtual void UpdateLocalTexture(IFrame frame, FramePixelFormat format)
    {
        if (uLocalVideoImage != null)
        {
            if (frame != null)
            {
                UnityMediaHelper.UpdateTexture(frame, ref mLocalVideoTexture);
                uLocalVideoImage.texture = mLocalVideoTexture;
                if (uLocalVideoImage.gameObject.activeSelf == false)
                {
                    uLocalVideoImage.gameObject.SetActive(true);
                }
                //apply rotation
                //watch out uLocalVideoImage should be scaled -1 X to make the local camera appear mirrored
                //it should also be scaled -1 Y because Unity reads the image from bottom to top
                uLocalVideoImage.transform.rotation = Quaternion.Euler(0, 0, frame.Rotation);

                mHasLocalVideo = true;
                mLocalFrameCounter++;
                mLocalVideoWidth = frame.Width;
                mLocalVideoHeight = frame.Height;
                mLocalVideoFormat = format;
            }
            else
            {
                //app shutdown. reset values
                mHasLocalVideo = false;
                uLocalVideoImage.texture = null;
                uLocalVideoImage.transform.rotation = Quaternion.Euler(0, 0, 0);
                uLocalVideoImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the remote video. If the frame is null it will hide the video image.
    /// </summary>
    /// <param name="frame"></param>
    public virtual void UpdateRemoteTexture(IFrame frame, FramePixelFormat format)
    {
        if (uRemoteVideoImage != null)
        {
            if (frame != null)
            {
                UnityMediaHelper.UpdateTexture(frame, ref mRemoteVideoTexture);
                uRemoteVideoImage.texture = mRemoteVideoTexture;
                //watch out: due to conversion from WebRTC to Unity format the image is flipped (top to bottom)
                //this also inverts the rotation
                uRemoteVideoImage.transform.rotation = Quaternion.Euler(0, 0, frame.Rotation * -1);
                mHasRemoteVideo = true;
                mRemoteVideoWidth = frame.Width;
                mRemoteVideoHeight = frame.Height;
                mRemoteVideoFormat = format;
                mRemoteFrameCounter++;
            }
            else
            {
                mHasRemoteVideo = false;
                uRemoteVideoImage.texture = uNoCameraTexture;
                uRemoteVideoImage.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void OnBtnShutDownClick()
    {
        videoChat.EndCall();        
    }

    public void OnVolumeChanged(float value)
    {
       videoChat.SetRemoteVolume(value);
    }

    public void OnLoudspeakerToggle(bool state)
    {
        //watch out the on state of the toggle means
        //the icon is crossed out thus
        //isOn == true means the speaker is off
        videoChat.SetLoudspeakerStatus(state);
        //read if the state actually changed
        RefreshLoudspeakerToggle();
    }
    private void RefreshLoudspeakerToggle()
    {
        bool state = videoChat.GetLoudspeakerStatus();
        uLoudspeakerToggle.isOn = !state;
    }
    public void OnMuteToggle(bool state)
    {
        videoChat.SetMute(state);
        //read if the state actually changed
        RefreshMuteToggle();
    }


    private void RefreshMuteToggle()
    {
        bool state = videoChat.IsMute();
        uMuteToggle.isOn = state;
    }

    public void OnBtnChangeDeviceClick()
    {
        print("onBtnChangeDeviceClick");
        videoChat.ChangeDevice();
    }

    public void EndCall()
    {
        if (mFullscreen)
        {
            SetFullscreen(false);
        }        
        uInCallBase.gameObject.SetActive(false);
    }

    /// <summary>
    /// This is called if the send button
    /// </summary>
    public void SendButtonPressed()
    {
        //get the message written into the text field
        string msg = uMessageInputField.text;
        SendMsg(msg);
    }

    /// <summary>
    /// User either pressed enter or left the text field
    /// -> if return key was pressed send the message
    /// </summary>
    public void InputOnEndEdit(string msg)
    {
        if (Input.GetKey(KeyCode.Return))
        {           
            SendMsg(msg);
        }
    }

    /// <summary>
    /// Sends a message to the other end
    /// </summary>
    /// <param name="msg"></param>
    private void SendMsg(string msg)
    {
        if (String.IsNullOrEmpty(msg))
        {
            //never send null or empty messages. webrtc can't deal with that
            return;
        }

        Append(msg);
        videoChat.Send(msg);

        //reset UI
        uMessageInputField.text = "";
        uMessageInputField.Select();
    }

    /// <summary>
    /// Adds a new message to the message view
    /// </summary>
    /// <param name="text"></param>
    public void Append(string text)
    {
        if (uMessageOutput != null)
        {
            uMessageOutput.AddTextEntry(text);
        }
        Debug.Log("Chat output: " + text);
    }

    public void SetFullscreen()
    {
        SetFullscreen(!mFullscreen);

        transform.SetAsLastSibling();
    }

    private void SetFullscreen(bool value)
    {
        mFullscreen = value;
        if (mFullscreen)
        {
            print("SetFullscreen");
            uVideoPanel.SetParent(uVideoBaseFullscreen, false);
            uChatPanel.SetParent(uChatBaseFullscreen, false);
            uInCallBase.gameObject.SetActive(false);
            uFullscreenPanel.gameObject.SetActive(true);
        }
        else
        {
            print("ResetSetFullscreen");
            uVideoPanel.GetComponent<RectTransform>().SetParent(uVideoBase, false);
            uChatPanel.GetComponent<RectTransform>().SetParent(uChatBase, false);
            uInCallBase.gameObject.SetActive(true);
            uFullscreenPanel.gameObject.SetActive(false);
        }
    }

    public void ShowOverlay()
    {
        if (this.uVideoOverlay == null)
        {
            Debug.LogError("VideoOverlay transform is missing.");
            return;
        }
        if (this.uVideoOverlay.gameObject.activeSelf)
        {
            this.uVideoOverlay.gameObject.SetActive(false);
            mVideoOverlayTimeout = 0;
        }
        else
        {
            this.uVideoOverlay.gameObject.SetActive(true);
            mVideoOverlayTimeout = sDefaultOverlayTimeout;
        }
    }
}
