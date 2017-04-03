using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelController : MapObject {

    // Use this for initialization
    new void Start()
    {
        base.Start();
        int range = 5;
        Vector3 iniPos = -Vector2.one * (range - range % 2) / 2;
        Vector3 corPos;
        for (int i = 0; i < range * range; i++)
        {
            corPos = new Vector3(i % range, i / range);
            map.SetTileData(floor, transform.position + iniPos + corPos, true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
