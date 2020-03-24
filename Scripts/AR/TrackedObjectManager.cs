using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedObjectManager : MonoBehaviour
{
    public static TrackedObjectManager instance;
    [SerializeField]
    ARTrackedObjectManager m_TrackedObjectManager;
    [SerializeField]
    GameObject target;

    public Action onTrackingFound;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Open();
    }

    public void Open()
    {
        print("Open");
        //m_TrackedObjectManager.enabled = true;
        m_TrackedObjectManager.trackedObjectsChanged += OnTrackObjectsChanged;
    }

    public void Close()
    {
        m_TrackedObjectManager.trackedObjectsChanged -= OnTrackObjectsChanged;
        
        //m_TrackedObjectManager.enabled = false;
        target.SetActive(false);
    }

    private void OnTrackObjectsChanged(ARTrackedObjectsChangedEventArgs eventArgs)
    {
        foreach (var trackedObject in eventArgs.added)
        {
            //trackedObject.transform.localScale = new Vector3(1f, 1f, 1f);          
            
            UpdateTrackObject(trackedObject);


            InvokeOnTrackFound();
        }


        foreach (var trackedObject in eventArgs.updated)
        {
            UpdateTrackObject(trackedObject);
            InvokeOnTrackFound();
        }


        foreach (var trackedObject in eventArgs.removed)
        {
            Destroy(trackedObject.gameObject);
        }
    }

    private void InvokeOnTrackFound()
    {
        if (onTrackingFound != null)
        {
            onTrackingFound();
            onTrackingFound = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateTrackObject(ARTrackedObject trackedObject)
    {
        if (trackedObject.trackingState==TrackingState.Tracking)
        {
            target.SetActive(true);
            target.transform.position = trackedObject.transform.position;
            target.transform.rotation = trackedObject.transform.rotation;


            float offsetX = PlayerPrefs.GetFloat("OffsetX");
            float offsetY = PlayerPrefs.GetFloat("OffsetY");
            float offsetZ = PlayerPrefs.GetFloat("OffsetZ");
            float scale = PlayerPrefs.GetFloat("Scale");

            if (offsetX != 0 || offsetY != 0 || offsetZ != 0)
            {
                target.transform.GetChild(0).localPosition = new Vector3(offsetX, offsetY, offsetZ);
            }

            if (scale!=0)
            {
                target.transform.GetChild(0).localScale = Vector3.one * scale;
            }           
                       
        }
    }

    private void OnDisable()
    {
        
    }

    public void SetObjectOffset(Vector3 offset,float scale=1)
    {
        if (target.activeInHierarchy)
        {

            target.transform.GetChild(0).localPosition = offset;
            target.transform.GetChild(0).localScale = Vector3.one * scale;
        }        
    }
}
