using UnityEngine;
using System.Collections;

public class GetObject : MonoBehaviour {

    GameObject obj = null;
    public GameObject Object
    {
        get { return obj; }
        set { obj = value; }
    }
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D aCollider2d = Physics2D.OverlapPoint(aTapPoint);

            if (aCollider2d)
            {
                obj = aCollider2d.transform.gameObject;
                Debug.Log(obj.name);
            }
            else
            {
                obj = null;
            }
        }
    }
}
