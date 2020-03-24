using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIScrollView : MonoBehaviour,IEndDragHandler
{
    public Button btnUp, btnDown;

    private ScrollRect scrollRect;
    private RectTransform content;
    [SerializeField]private int totalCount;    
    private float cellSize;
    private Vector2 targetPos;
    private bool needUpdate;

    public Action<int> onValueChanged;

    [SerializeField] private int count = 0;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            if (onValueChanged != null)
            {
                onValueChanged(count);
            }
            SetContent();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        content = scrollRect.content;
        totalCount = content.childCount;       

        if (scrollRect.horizontal)
        {
            cellSize = content.GetChild(0).GetComponent<RectTransform>().rect.width;
            CalculateCount(Mathf.Abs(content.rect.x));
        }
        else if (scrollRect.vertical)
        {
            cellSize = content.GetChild(0).GetComponent<RectTransform>().rect.height;
            CalculateCount(Mathf.Abs(content.rect.y));
        }

        btnUp.onClick.AddListener(OnBtnUpClick);
        btnDown.onClick.AddListener(OnBtnDownClick);
    }

    private void CalculateCount(float value)
    {
        Count = Mathf.RoundToInt(value/ cellSize);
        if (Count>totalCount-1)
        {
            Count = totalCount - 1;
        }
        else if (count<0)
        {
            Count = 0;
        }
    }

    private void OnBtnUpClick()
    {
        if (Count < totalCount-1)
        {
            Count++;
            //SetContent();
        }        
    }

    private void OnBtnDownClick()
    {
        if (Count > 0)
        {
            Count--;
            //SetContent();
        }        
    }


    private void SetContent()
    {
        if (scrollRect.horizontal)
        {
            targetPos = new Vector2(-count * cellSize, 0);
        }
        else if (scrollRect.vertical)
        {
            targetPos = new Vector2(0, count * cellSize);
        }
        needUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (needUpdate)
        {
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, 0.1f);
            if (Vector2.Distance(content.anchoredPosition,targetPos)<1)
            {
                needUpdate = false;
            }
        }        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("OnEndDrag");
        if (scrollRect.horizontal)
        {
            CalculateCount(-content.anchoredPosition.x);
        }
        else if (scrollRect.vertical)
        {
            CalculateCount(content.anchoredPosition.y);
        }
    }
}
