using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEffect : MonoBehaviour {

    int frameCo, frameLim = 30;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += Vector3.forward*20;
        frameCo++;
        if(frameLim<= frameCo)
        {
            frameCo = 0;
            gameObject.SetActive(false);
        }
    }
}
