using UnityEngine;
using System.Collections.Generic;
using System;

public class SetTransparent : MonoBehaviour
{
    /// <summary>    /// 透明开关    /// </summary>
    public bool On,Off = false;
    public bool on { get; set; }
    /// <summary>    /// 透明材质    /// </summary>
    public Material transparent;
    /// <summary>    /// 子物体列表    /// </summary>
    private List<Transform> ChildrenList = new List<Transform>();
    /// <summary>    /// 原始材质    /// </summary>
    private List<Material[]> originalMaterialList = new List<Material[]>();

    // Use this for initialization
    void Start()
    {
        GetAllChildrenAndMaterial(transform);
    }    

    /// <summary>
    /// 获取所有渲染子物体和材质
    /// </summary>
    /// <param name="parentTF">父物体Transform</param>
    private void GetAllChildrenAndMaterial(Transform parentTF)
    {
        Transform[] arr = parentTF.GetComponentsInChildren<Transform>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].GetComponent<MeshRenderer>()!=null)
            {
                ChildrenList.Add(arr[i]);
                originalMaterialList.Add(arr[i].GetComponent<MeshRenderer>().materials);
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
        if (On)
        {
            SetTransparentOn();
            On = false;
        }
        if (Off)
        {
            SetTransparentOff();
            Off = false;
        }
    }

    /// <summary>
    /// 开启透明
    /// </summary>
    private void SetTransparentOn()
    {
        if (!on)
        {
            for (int i = 0; i < ChildrenList.Count; i++)
            {
                List<Material> list = new List<Material>();                
                for (int j = 0; j < originalMaterialList[i].Length; j++)
                {
                    list.Add(transparent);
                }
                ChildrenList[i].GetComponent<MeshRenderer>().materials = list.ToArray();               
            }
            on = true;
        }  
    }

    /// <summary>
    /// 关闭透明
    /// </summary>
    private void SetTransparentOff()
    {
        if (on)
        {
            for (int i = 0; i < ChildrenList.Count; i++)
            {
                ChildrenList[i].GetComponent<MeshRenderer>().materials = originalMaterialList[i];
            }
            on = false;
        }       
    }

    public void OnDisable()
    {
        On = false;
        SetTransparentOff();
    }
}
