﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDetecter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        Debug.Log("ここやで！");
    }
}
