#define VUFORIA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    RectTransform rectTF;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Camera referCam;
    private Vector2 viewPoint;
    [SerializeField]
    private bool debug = false;
    private int width, height;
    // Start is called before the first frame update
    void Start()
    {
        rectTF = GetComponent<RectTransform>();
        if (!referCam)
        {
            referCam = Camera.main;
        }
        width = Screen.width;
        height = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            print(referCam.WorldToViewportPoint(target.position));
            print(referCam.WorldToScreenPoint(target.position));
        }
        
        if (referCam == Camera.main)
        {
            viewPoint = referCam.WorldToScreenPoint(target.position);
#if VUFORIA
            rectTF.anchoredPosition =new Vector2(viewPoint.x*2048/width,viewPoint.y*1536/height);  
#else
            float x = (viewPoint.x - 1366) * 0.75f * 0.75f;
            float y = (viewPoint.y - 1024) * 0.75f * 0.75f;
            rectTF.anchoredPosition = new Vector2(x, y);
#endif
        }
        else
        {
            viewPoint = referCam.WorldToViewportPoint(target.position);
            rectTF.anchoredPosition = new Vector2(viewPoint.x * 2048, viewPoint.y * 1536);
        }
        
    }
}
