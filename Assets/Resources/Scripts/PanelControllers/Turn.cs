using UnityEngine;
using System.Collections;
using System;

public class Turn : MonoBehaviour
{
    int direction;
    bool process = false;
    public Sprite s;
    GameObject ef;
    [SerializeField]
    bool reverse,uTurn;//右折、uターン
    // Use this for initialization
    void Start()
    {
        //direction = GetComponent<PanelController>().direction;
        //transform.Rotate(new Vector3(0, 0, 90 * direction));
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
        //ef.transform.Rotate(new Vector3(0, 0, -90 * direction));
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RunPanel(GameObject other)
    {
        RobotController rc = other.GetComponent<RobotController>();
        if (reverse)
        {
            direction = rc.dire == 0 ? 3 : rc.dire - 1;
        }
        else
        {
            direction = rc.dire >= 3 ? 0 : rc.dire + 1;
        }
        if (uTurn)
        {
            direction = (rc.dire + 2) % 4;
            Debug.Log("Uturn");
        }
        Debug.Log("方向" + direction);
        rc.Turn(direction);
        //rc.Zoning();
        //StartCoroutine("Effect");
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
