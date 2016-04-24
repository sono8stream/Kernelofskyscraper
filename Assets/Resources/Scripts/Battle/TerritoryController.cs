using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerritoryController : MonoBehaviour
{

    public int[,] trdata;//領土データ
    public int[,] rbdata;//すべてのロボの位置を管理
    public string[] rbDataDebug;
    public int[,] pndata;//すべてのパネルの位置を管理
    public int r_count;//ロボット数

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
    public GameObject k;
    public GameObject k_e;
    List<GameObject> ars;

    // Use this for initialization
    void Start()
    {
        int[,] mpd = GameObject.Find("MapLayer1").GetComponent<MapLoader>().mapdata;
        trdata = new int[mpd.GetLength(0), mpd.GetLength(1)];
        rbdata = new int[mpd.GetLength(0), mpd.GetLength(1)];
        r_count = 0;
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
                sub += rbdata[j, i].ToString() + ",";
            }
            rbDataDebug[i] = sub;
        }
    }

    public IEnumerator Refresh(int mix, int miy, int max, int may, bool[,] sp_data)
    {
        for (int i = mix; i <= max; i++)
        {
            for (int j = miy; j <= may; j++)
            {
                /*t.SetPixels((i + w / 2) * size, (j + h / 2) * size, size, size, c);
                t.Apply();*/
                if (sp_data[i - mix + 1, j - miy + 1]/* && trdata[w / 2 + i, h / 2 + j] == -1*/)//マスが空白
                {
                    Generate(new Vector2(i, j), true);
                }
            }
            k.GetComponent<KernelController>().Speed_Sub = k.GetComponent<KernelController>().sp - ar_cn;
            /*GetComponent<SpriteRenderer>().sprite
            = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), 32);*/
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void Reset(Vector2 pos, bool mikata,GameObject rb)
    {
        /*if (mikata)
        {*/
            Generate(pos,mikata,rb);
        /*}
        else
        {
            Collider2D[] cs = Physics2D.OverlapPointAll(rb.transform.position);
            Collider2D c = null;
            foreach (Collider2D col in cs)
            {
                if (col.gameObject.tag == "Area")
                {
                    c = col;
                }
            }
            if (c != null)
            {
                c.gameObject.GetComponent<AreaController>().Vanish(rb);
            }
        }*/
    }

    public void Generate(Vector2 pos,bool mikata,GameObject robot=null)
    {
        if (((int)k.transform.position.x != (int)pos.x || (int)k.transform.position.y != (int)pos.y)
            && ((int)k_e.transform.position.x != (int)pos.x || (int)k_e.transform.position.y != (int)pos.y))//新たにパネルを生成
        {
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
                if(!mikata)
                {
                    a.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                foreach(GameObject a in ars)
                {
                    if (a.GetComponent<AreaController>().Mikata != mikata
                        && ((int)a.transform.position.x == (int)pos.x && (int)a.transform.position.y == (int)pos.y))
                    {
                        a.GetComponent<AreaController>().Mikata = mikata;
                        if(robot!=null)
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
        }
    }

    public int SetRobotNumber()
    {
        r_count++;
        return r_count - 1;
    }

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
        r_count--;
    }
}
