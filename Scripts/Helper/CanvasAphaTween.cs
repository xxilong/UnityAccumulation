using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasAphaTween : MonoBehaviour
{
    public bool autoStart=false;
    public float duration=1;
    [Range(0, 1)]
    public float from, to;
    CanvasGroup group;
    private bool forwad,backward, play;
    private float alpha=1;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!group)
        {
            group = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {        

        if (autoStart)
        {
            DoForward();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            group.alpha = alpha;
        }
    }

    public void DoForward()
    {
        play = true;
        StartCoroutine(Play(from, to, duration));
    }    

    public void DoBackward()
    {
        play = true;
        StartCoroutine(Play(to, from, duration));
    }
    
    public void Do(float from,float to,float t,float delay=0)
    {
        play = true;
        StartCoroutine(Play(from, to, t,delay));
    }

    IEnumerator Play(float from, float to, float duration, float delay = 0)
    {
        if (!play)
        {
            yield break;
        }
        alpha = from;
        yield return new WaitForSeconds(delay);
        float t = 0;
        while (t < 1)
        {
            alpha = Mathf.Lerp(from, to, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime / duration;
        }
        play = false;
    }
}
