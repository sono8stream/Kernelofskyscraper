using UnityEngine;
using System.Collections;

public class BackImageController : MonoBehaviour {
    const float sp = -0.01f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(sp, 0, 0));
        if(transform.position.x<-16.5f)
        {
            transform.position = new Vector3(7.5f, 0);
        }
	}
}
