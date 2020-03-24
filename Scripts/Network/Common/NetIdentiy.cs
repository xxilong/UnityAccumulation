using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetIdentiy : MonoBehaviour {

    int identiy = -1;
	// Use this for initialization
	void Start () {
        NetManager.Instant.netGameObjectList.Add(gameObject);
        identiy = NetManager.Instant.netGameObjectList.Count;  
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
