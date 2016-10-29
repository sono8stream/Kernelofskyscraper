using UnityEngine;
using System.Collections;
using System;

#pragma warning disable
public class KernelController : MonoBehaviour {
    #region Property
    int kernelNo;
    public bool mikata;
    public int energy;
    public int enmax;//エネルギー最大値
    public int sp;//回復速度
    int sp_s;
    public int Speed_Sub
    {
        get { return sp_s; }
        set { sp_s = value; }
    }
    int sp_cn;
    public Sprite br_effect;
    public int effectCount;
    GameObject ef;
    public GameObject EffectObject
    {
        get { return ef; }
        set { ef = value; }
    }
    GameObject bar;
    bool breaking = false;
    GameObject ot;//接触した敵のロボット
    bool intake;//ロボット取り込み中か
    #region Automation
    public bool auto;//ロボを生成できるか
    public GameObject[] genRobots;//通常生成させるロボ
    public int genNo;
    public bool damaged;
    #endregion
    #endregion

    void Awake()
    {
        Transform robots = transform.FindChild("Robots");
        int count = robots.GetChildCount();
        genRobots = new GameObject[count];
        for (int i = 0; i < count; i ++)
        {
            genRobots[i] = robots.GetChild(i).gameObject;
            genRobots[i].SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
        bar = transform.FindChild("HpBar").gameObject;
        bar.transform.localPosition = new Vector3(-1f, -1.5f, -3f);
        bar.GetComponent<SpriteRenderer>().color = mikata ? Color.blue : Color.red;
        sp_s = sp;
        sp_cn = sp;
    }

    // Update is called once per frame
    void Update()
    {
        if (sp_cn <= 0)
        {
            sp_cn = sp_s;
            energy++;
            if (energy > enmax)
            {
                energy = enmax;
            }
            if (auto)
            {
                AI();
            }
        }
        sp_cn--;
        bar.transform.localScale = new Vector3(energy / (float)enmax*2, 2, 2);
    }

    Texture2D SetEffect(Sprite s, int num, Vector2 targetpos)
    {
        Texture2D t = new Texture2D(120, 120, TextureFormat.RGBA32, false);
        if (targetpos != new Vector2(transform.position.x, transform.position.y))
        {
            ef.transform.position = targetpos;
        }
        Color[] c = s.texture.GetPixels(120 * num, 0, 120, 120);
        t.SetPixels(0, 0, 120, 120, c);
        t.Apply();
        ef.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, 120, 120), new Vector2(0.5f, 0.5f), 90);
        return t;
    }

    public void Intake(GameObject other)
    {
        RobotController rbCon = other.GetComponent<RobotController>();
        if (other.tag == "Robot" && (mikata == rbCon.Mikata
            || rbCon.CheckBreaking))
        {
            return;
        }
        else if (other.tag == "Area")
        {
            StartCoroutine(Break(effectCount));
            return;
        }

        other.GetComponent<RobotController>().CheckBreaking = true;
        intake = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        sr.color = Color.red;
        Debug.Log("Start Break");
        other.GetComponent<RobotController>().Break();
        //yield return new WaitForSeconds(0.1f);
        Damage(other.GetComponent<RobotController>().attackCurrent);
        sr.color = c;
        intake = false;
        damaged = true;
    }

    public IEnumerator Break(int size)
    {
        for (int i = 0; i < size; i++)
        {
            SetEffect(br_effect, i, transform.position);
            yield return new WaitForSeconds(0.05f);
        }
        GetComponent<SpriteRenderer>().sprite = null;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Robot"))
        {
            if(g.GetComponent<RobotController>()!=null)
            {
                g.GetComponent<RobotController>().Break();
            }
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Kernel"))
        {
            g.GetComponent<KernelController>().energy = 100;
            g.GetComponent<KernelController>().enabled = false;
        }
        Destroy(gameObject);
    }

    public bool Damage(int Offence)
    {
        energy -= Offence;
        if (energy <= 0)
        {
            energy = 0;
            breaking = true;
            StartCoroutine(Break(effectCount));
        }
        return breaking;
    }

    //ロボ・パネルの生成
    public GameObject Generate(Vector3 genPos, bool enable = false, int direction = -1, params Vector2[] t_pos)
    {
        int c = genRobots[genNo].GetComponent<RobotController>().cost;
        GameObject ob = null;
        if (energy > c)
        {
            TerritoryController t
                = GameObject.Find("Territory").GetComponent<TerritoryController>();
            Vector2 s = genPos
                + new Vector3(Mathf.Floor(t.rbdata.GetLength(0) / 2), Mathf.Floor(t.rbdata.GetLength(1) / 2));
            if (t.rbdata[(int)s.x, (int)s.y] != -1)//ロボがいるか、陣地でない
            {
                return null;
            }
            ob = Instantiate(genRobots[genNo]);
            ob.transform.position = genPos;
            ob.SetActive(true);
            RobotController rc = ob.GetComponent<RobotController>();
            rc.Mikata = mikata;
            rc.auto = false;
            rc.number = t.SetRobotNumber();
            rc.dire = direction != -1 ? direction : rc.dire;
            t.rbdata[t.rbdata.GetLength(0) / 2 + (int)rc.transform.position.x,
                t.rbdata.GetLength(1) / 2 + (int)rc.transform.position.y] = rc.number;
            if (t_pos.GetLength(0) > 0)
            {
                rc.auto = true;
                rc.dst = t_pos;
            }
            energy -= c;
            //rc.Start();
            rc.enabled = enable;
            if(auto)
            {
                do
                {
                    genNo++;
                    if (genNo >= genRobots.Length)
                    {
                        genNo = 0;
                    }
                }
                while (!genRobots[genNo].GetComponent<RobotController>().isReady);
            }
        }
        return ob;
    }

    public void AI()
    {
        if (/*damaged && */energy > enmax / 2 && genRobots.Length > 0 && genRobots[genNo] != null
            && genRobots[genNo].GetComponent<RobotController>().isReady)
        {
            Generate(transform.position, true, -1);
            Debug.Log(genNo);
        }
    }
}
