using UnityEngine;
using System.Collections;

public class MonoTools:MonoBehaviour
{
    public static MonoTools I;
    private void Awake()
    {       
        I = this;
    }
    /// <summary>
    /// 设置物体延时显影
    /// </summary>
    /// <param name="go"></param>
    /// <param name="active"></param>
    /// <param name="waitTime"></param>
    public void SetActive(GameObject go,bool active, float waitTime = 0)
    {
       StartCoroutine( SetActiveCor(go,active, waitTime));
    }

    private IEnumerator SetActiveCor(GameObject go, bool active, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        go.SetActive(active);
    }
}
