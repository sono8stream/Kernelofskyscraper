using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelController : MapObject {

    // Use this for initialization
    new void Start()
    {
        range = 3;
        base.Start();
        foreach(Panel p in transform.FindChild("Sanctuaries").GetComponentsInChildren<Panel>())
        {
            map.SetPanelData(floor, transform.localPosition + p.transform.localPosition, p);
        }
        map.VisualizeRoom(floor, transform.localPosition);
        map.flrCon.UpdateMapImage();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
