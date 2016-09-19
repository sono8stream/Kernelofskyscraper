using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerritoryController : MonoBehaviour
{

    public int[,] trdata;//領土データ
    public int[,] rbdata;//すべてのロボの位置を管理
    public string[] rbDataDebug;
    public int[,] pndata;//すべてのパネルの位置を管理
    public int rCount;//ロボット数

    int ar_cn;//所持エリア数
    public int Area_count
    {
        get { return ar_cn; }
        set { ar_cn = value; }
    }
    Texture2D t;
    GameObject ar;
    Sprite s;
    int size = 32;
    public int w;
    public int h;
    public KernelController kerCon;
    public KernelController kerConEnemy;
    List<GameObject> ars;

    // Use this for initialization
    void Start()
    {
        int[,] mpd = GameObject.Find("MapLayer1").GetComponent<MapLoader>().mapdata;
        trdata = new int[mpd.GetLength(0), mpd.GetLength(1)];
        rbdata = new int[mpd.GetLength(0), mpd.GetLength(1)];
        Debug.Log(rbdata.GetLength(0));
        Debug.Log(rbdata.GetLength(1));
        rCount = 0;
        t = new Texture2D(trdata.GetLength(0) * size, trdata.GetLength(1) * size, TextureFormat.RGBA32, false);
        ar = (GameObject)Instantiate(Resources.Load("Prefabs/area"), Vector2.zero, transform.rotation);//ライン生成
        ar.transform.position = new Vector2(-100, -100);
        s = Resources.Load<Sprite>("Sprites/area");
        Color[] nc = Resources.Load<Sprite>("Sprites/nullImage").texture.GetPixels();
        for (int i = 0; i < trdata.GetLength(0); i++)
        {
            for (int j = 0; j < trdata.GetLength(1); j++)
            {
                trdata[i, j] = -1;
                t.SetPixels(i * size, j * size, size, size, nc);//エリア画像初期化
                rbdata[i, j] = -1;
            }
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Robot"))
        {
            if (g.activeSelf)
            {
                RobotController rCon = g.GetComponent<RobotController>();
                if (rCon != null)
                {
                    rCon.number = rCount;
                    rbdata[rbdata.GetLength(0) / 2 + (int)rCon.transform.position.x,
                        rbdata.GetLength(1) / 2 + (int)rCon.transform.position.y] = rCon.number;
                    rCount++;
                }
            }
        }
        ars = new List<GameObject>();
        rbDataDebug = new string[mpd.GetLength(1)];
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<rbDataDebug.GetLength(0);i++)
        {
            string sub="";
            for (int j = 0; j < rbdata.GetLength(0); j++)
            {
                sub += rbdata[j, rbdata.GetLength(1) - i - 1].ToString() + ",";
            }
            rbDataDebug[i] = sub;
        }
    }

    public IEnumerator Refresh(int mix, int miy, int max, int may, bool[,] sp_data)
    {
        GameObject ker = null;
        GameObject sub;
        for (int i = mix; i <= max; i++)
        {
            for (int j = miy; j <= may; j++)
            {
                if (sp_data[i - mix + 1, j - miy + 1])//マスが空白
                {
                    sub = Generate(new Vector2(i, j), true);
                    if(sub!=null)
                    {
                        ker = sub;
                    }
                }
            }
            kerCon.Speed_Sub = kerCon.sp - ar_cn;
            yield return new WaitForSeconds(0.1f);
        }
        if (ker != null)
        {
            KernelController k = ker.GetComponent<KernelController>();
            k.StartCoroutine(k.Break(k.effectCount));
        }
    }

    public GameObject Generate(Vector2 pos, bool mikata, GameObject robot = null)
    {
        GameObject ker = null;
        Collider2D[] col = Physics2D.OverlapPointAll(pos);//カーネル上にパネルは置かない
        foreach (Collider2D c in col)
        {
            if (c.tag == "Kernel")
            {
                KernelController k = c.GetComponent<KernelController>();
                if (k.mikata != mikata
                    && Mathf.RoundToInt(pos.x - c.transform.position.x) == 0
                    && Mathf.RoundToInt(pos.y - c.transform.position.y) == 0)
                {
                    ker = c.gameObject;
                }
            }
        }
        if (trdata[w / 2 + (int)pos.x, h / 2 + (int)pos.y] == -1)
        {
            trdata[w / 2 + (int)pos.x, h / 2 + (int)pos.y] = 1;
            GameObject a = (GameObject)Instantiate(ar, Vector2.zero, transform.rotation);//ライン生成
            a.name = "a";
            a.transform.position = new Vector2((int)pos.x + 0.5f, (int)pos.y);
            a.transform.SetParent(transform, false);
            a.GetComponent<SpriteRenderer>().sortingLayerName = "Territory";
            a.GetComponent<SpriteRenderer>().sprite = s;
            a.GetComponent<AreaController>().Mikata = mikata;
            ars.Add(a);
            ar_cn += mikata ? 1 : 0;
            if (!mikata)
            {
                a.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        else
        {
            foreach (GameObject a in ars)
            {
                if (a.GetComponent<AreaController>().Mikata != mikata
                    && ((int)a.transform.position.x == (int)pos.x && (int)a.transform.position.y == (int)pos.y))
                {
                    a.GetComponent<AreaController>().Mikata = mikata;
                    if (robot != null)
                        a.gameObject.GetComponent<AreaController>().Vanish(robot);
                    if (!mikata)//敵が踏んだら
                    {
                        ar_cn--;
                        a.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        ar_cn++;
                        a.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
        }
        return ker;
    }

    public int SetRobotNumber()
    {
        rCount++;
        return rCount - 1;
    }

    /// <summary>
    /// ロボット数を更新
    /// ロボット破壊時に呼び出し
    /// </summary>
    /// <param name="robotnumber"></param>
    public void AdjustRobotNumber(int robotnumber)
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject g in go)
        {
            RobotController rc = g.GetComponent<RobotController>();
            if (rc != null && rc.number > robotnumber)
            {
                rc.number--;
            }
        }
        for (int i = 0; i < rbdata.GetLength(0); i++)
        {
            for (int j = 0; j < rbdata.GetLength(1); j++)
            {
                if (rbdata[i, j] == robotnumber)
                {
                    rbdata[i, j] = -1;
                }
                else if (rbdata[i, j] > robotnumber)
                {
                    rbdata[i, j]--;
                }
            }
        }
        rCount--;
    }
}
