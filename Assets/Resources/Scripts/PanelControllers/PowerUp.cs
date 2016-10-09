using UnityEngine;
using System.Collections;
using System;

public class PowerUp : MonoBehaviour {

    int direction;
    bool process = false;
    public Sprite s;
    GameObject ef;
    // Use this for initialization
    void Start()
    {
        direction = GetComponent<PanelController>().direction;
        transform.Rotate(new Vector3(0, 0, 90 * direction));
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
        ef.transform.Rotate(new Vector3(0, 0, -90 * direction));
    }

    // Update is called once per frame
    void Update()
    {
        /*if(process)
        {
            StartCoroutine("Effect");
        }
        else
        {
            StopCoroutine("Effect");
        }*/
    }

    /*void OnTriggerStay2D(Collider2D other)
    {
        RobotController rc = other.GetComponent<RobotController>();
        if (other.tag == "Robot" && rc.Mikata
            && Math.Round(other.transform.position.x) == Math.Round(transform.position.x)
            && Math.Round(other.transform.position.y) == Math.Round(transform.position.y)
            && rc.Speed_Count != rc.speed
            && !rc.Move
            && !process)
        {
            process = true;
            other.GetComponent<RobotController>().Turn(direction);
            /*other.GetComponent<Rigidbody2D>().transform.position
                = new Vector3(this.transform.position.x, this.transform.position.y);
            other.GetComponent<RobotController>().Zoning();
            StartCoroutine("Effect");
        }
    }*/

    public void RunPanel(GameObject other)
    {
        RobotController rc = other.GetComponent<RobotController>();
        if (rc.Mikata&&!process)
        {
            process = true;
            rc.Turn(direction);
            /*other.GetComponent<Rigidbody2D>().transform.position
                = new Vector3(this.transform.position.x, this.transform.position.y);*/
            //rc.Zoning();
            StartCoroutine("Effect");
        }
    }

    IEnumerator Effect()
    {
        Texture2D t = new Texture2D(120, 120, TextureFormat.RGBA32, false);
        for (int i = 0; i < 10; i++)
        {
            Color[] c = s.texture.GetPixels(120 * i, 0, 120, 120);
            t.SetPixels(0, 0, 120, 120, c);
            t.Apply();
            ef.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, 120, 120), new Vector2(0.5f, 0.5f), 120);
            if (i == 9)
            {
                process = false;
                ef.GetComponent<SpriteRenderer>().sprite = null;
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
