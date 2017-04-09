using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelController : MapObject {

    // Use this for initialization
    new void Start()
    {
        range = 3;
        base.Start();
        int lightRange = 5;
        Vector3 iniPos = -Vector2.one * (lightRange - lightRange % 2) / 2;
        Vector3 corPos;
        for (int i = 0; i < lightRange * lightRange; i++)
        {
            corPos = new Vector3(i % lightRange, i / lightRange);
            map.SetTileData(floor, transform.localPosition + iniPos + corPos, true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
