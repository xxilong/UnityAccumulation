using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class EditorHelper : ScriptableObject
{ 

    #region NameEditor
    

    [MenuItem("Tools/NameEditor/去掉空格和括号", false)]
    public static void RemoveBrackets()
    {
        Transform[] arr = Selection.transforms;
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i].name = arr[i].name.Split('(')[0].Trim();
        }        
    }
    [MenuItem("Tools/NameEditor/随机排列子物体")]
    public static void SortChildByRandom()
    {        
        Transform[] parent = Selection.transforms;
        List<Transform> childList = parent[0].GetAllFirstChild();
        List<int> list = new List<int>();
        for (int i = 0; i < childList.Count; i++)
        {
            list.Add(i);
        }
        list=list.OrderBy(p => UnityEngine.Random.Range(0,childList.Count)).ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(list[i]);
        }


    }
    

    [MenuItem("Tools/NameEditor/按名称升序排列子物体")]
    static void OrderChildByName()
    {        
        List<Transform> childList= Selection.transforms[0].GetAllFirstChild().OrderBy(p => p.name).ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(i);           
        }
    }

    [MenuItem("Tools/NameEditor/按名称降序排列子物体")]
    static void OrderChildByNameDescending()
    {        
        List<Transform> childList = Selection.transforms[0].GetAllFirstChild().OrderByDescending(p => p.name).ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].SetSiblingIndex(i);
        }
    }

    #endregion

    #region GameObjectHelper
    /// <summary>
    /// 激活当前层级下的所有
    /// </summary>
    [MenuItem("Tools/GameObject/激活当前层所有")]
    static void ActiveAll()
    {
        GameObject[] arr = Selection.gameObjects;
        Transform parent = arr[0].transform.parent;
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            parent.GetChild(i).gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏当前层级下的所有
    /// </summary>
    [MenuItem("Tools/GameObject/隐藏当前层所有")]    
    static void DisActiveAll()
    {
        GameObject[] arr = Selection.gameObjects;
        Transform parent = arr[0].transform.parent;
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            parent.GetChild(i).gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 只激活当前层级下所选Gameobject
    /// </summary>   
    [MenuItem("Tools/GameObject/只激活所选")]
    static void ActiveSelectedOnly()
    {
        List<GameObject> list = new List<GameObject>(Selection.gameObjects);
        Transform parent = list[0].transform.parent;
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            parent.GetChild(i).gameObject.SetActive(list.Contains(parent.GetChild(i).gameObject));
        }
    }

    /// <summary>
    /// 只激活当前层级下所选Gameobject
    /// </summary>   
    [MenuItem("Tools/GameObject/只隐藏所选")]
    static void DisActiveSelectedOnly()
    {
        List<GameObject> list = new List<GameObject>(Selection.gameObjects);
        Transform parent = list[0].transform.parent;
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            parent.GetChild(i).gameObject.SetActive(!list.Contains(parent.GetChild(i).gameObject));
        }
    }
    #endregion


    #region TransformHelper
    //移至同层第一
    //移至同层最后
    #endregion
}