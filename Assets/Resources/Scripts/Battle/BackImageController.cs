using UnityEngine;
using System.Collections;

public class BackImageController : MonoBehaviour {
    const float sp = -0.01f;
    float boundary = 12;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(sp, 0, 0));
        if(transform.localPosition.x<-boundary)
        {
            transform.localPosition = new Vector3(boundary, 0,10);
        }
	}
}
