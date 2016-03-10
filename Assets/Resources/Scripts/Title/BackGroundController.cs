using UnityEngine;
using System.Collections;

public class BackGroundController : MonoBehaviour {
    float size;
    bool up;
    // Use this for initialization
    void Start()
    {
        size = 2;
        up = false;
    }

    // Update is called once per frame
    void Update()
    {
        float delta = up ? 0.005f : -0.005f;
        size += delta;
        transform.localScale = new Vector3(1.2f, 2f, 1) * size;
        if(size>2)
        {
            up = false;
        }
        else if(size<1)
        {
            up = true;
        }
        //transform.eulerAngles += new Vector3(0, 0, 0.1f);
    }
}
