using UnityEngine;
using System.Collections.Generic;

public class TransparentManager : MonoSingleton<TransparentManager> {
    List<SetTransparent> transparentList = new List<SetTransparent>();
	// Use this for initialization
	void Start () {
        SetTransparent[] array = FindObjectsOfType<SetTransparent>();
        transparentList = new List<SetTransparent>(array);
	}
	
	// Update is called once per frame
	void Update () {
	
	}    
    /// <summary>
    /// 开透明
    /// </summary>
    /// <param name="objName"></param>
    public void SetTransparentOn(string objName)
    {
        transparentList.Find(p => p.name == objName).On = true;
    }
    /// <summary>
    /// 关透明
    /// </summary>
    /// <param name="objName"></param>
    public void SetTransparentOff(string objName)
    {
        transparentList.Find(p => p.name == objName).Off = true;
    }
    public void CloseAllTransparent()
    {
        for (int i = 0; i < transparentList.Count; i++)
        {
            transparentList[i].gameObject.SetActive(true);
            transparentList[i].Off = true;
        }
    }
    public void SetActive(string objName,bool active)
    {
        transparentList.Find(p => p.name == objName).gameObject.SetActive(active);
    }												 
}
