using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// 跟踪管理器
/// </summary>
public class TrackManager : DefaultTrackableEventHandler
{

    public static TrackManager instance { get; set; }

    public bool defaultOpen = true;
    public TrackObject trackObject;
    public TrackStatus trackStatus = new TrackStatus() { isFirstFound = false, isFound = false };
    public TrackPosture trackPosture;
    public VuforiaBehaviour vuforia;
    //要跟踪的物体类
    [System.Serializable]
    public class TrackObject
    {
        public Transform ImageTarget;
        public Transform modle;
    }
    public Action onTrackingFound;
    private bool open = false;
    private bool isFirst = true;

    [System.Serializable]
    public class TrackPosture
    {
        public bool oneTime = false;

        public void UpdateTrackPosture()
        {

            TrackManager.instance.trackObject.modle.position = TrackManager.instance.trackObject.ImageTarget.position;
            TrackManager.instance.trackObject.modle.rotation = TrackManager.instance.trackObject.ImageTarget.rotation;
        }
    }

    //跟踪状态类
    public class TrackStatus
    {
        protected internal bool isFirstFound { get; set; }
        protected internal bool isFound { get; set; }
    }

    void Awake()
    {
        if (trackObject == null)
        {
            Debug.LogError("No Track Objects");
            Destroy(this);
            return;
        }
        instance = this;
        if (instance.trackObject.ImageTarget == null)
        {
            Destroy(this);
            return;
        }

        if (defaultOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    protected override void Start()
    {
        mTrackableBehaviour = TrackManager.instance.trackObject.ImageTarget.GetComponent<TrackableBehaviour>();

        //mTrackableBehaviour?.RegisterTrackableEventHandler(this);
    }

    /// <summary>
    /// 开启追踪
    /// </summary>
    public void Open()
    {
        print("TrackOpen");
        open = true;
        trackStatus.isFirstFound = false;
        //trackObject.ImageTarget.gameObject.SetActive(true);  
        mTrackableBehaviour?.RegisterTrackableEventHandler(this);
        trackObject.modle.gameObject.SetActive(false);
        vuforia.enabled=true;
    }

    /// <summary>
    /// 关闭追踪
    /// </summary>
    public void Close()
    {
        print("TrackClose");
        open = false;
        trackStatus.isFound = false;
        trackStatus.isFirstFound = false;

        mTrackableBehaviour?.UnregisterTrackableEventHandler(this);
        //trackObject.ImageTarget.gameObject.SetActive(false);
        trackObject.modle.gameObject.SetActive(false);
        vuforia.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!trackStatus.isFirstFound && trackStatus.isFound)
        //{
        //    trackStatus.isFirstFound = true;
        //    //NetManager.isStopSync = false;
        //    FirstFoundActionOnContentGameObject();
        //}
        //if (trackStatus.isFound)
        //{
        //    if (TrackManager.Instant.trackPosture.oneTime == false)
        //    {
        //        TrackManager.Instant.trackPosture.oneTime = true;
        //        //NetManager.isStopSync = true;

        //        //trackPosture.UpdateTrackPosture();

        //        Debug.Log("wangcq327 --- UpdatePosture");
        //    }
        //}
        //else if (!trackStatus.isFound)
        //{
        //    TrackManager.Instant.trackPosture.oneTime = false;
        //    //NetManager.isStopSync = false;
        //}

    }

    void FirstFoundActionOnContentGameObject()
    {
        Debug.Log("TrackManager EnableContentGameObjects");
        if (trackObject.modle.gameObject.activeSelf == false)
        {
            trackObject.modle.gameObject.SetActive(true);
            trackPosture.UpdateTrackPosture();
        }

        if (onTrackingFound != null)
        {
            onTrackingFound();
            onTrackingFound = null;
        }
    }

    public new void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log(name + "Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
#if UNITY_2018_3_OR_NEWER
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
#else
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
             newStatus == TrackableBehaviour.Status.NOT_FOUND)
#endif
        {
            Debug.Log(name + "Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }
    protected override void OnTrackingFound()
    {
        print("OnTrackingFound");
        trackStatus.isFound = true;

        if (!open)
        {
            return;
        }

        if (!trackStatus.isFirstFound)
        {
            trackStatus.isFirstFound = true;
            FirstFoundActionOnContentGameObject();
        }
        else
        {
            trackPosture.UpdateTrackPosture();
        }
    }
    protected override void OnTrackingLost()
    {
        print("OnTrackingLost");
        trackStatus.isFound = false;
    }
}
