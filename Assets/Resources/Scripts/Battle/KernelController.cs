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
    public int effect_count;
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
    bool auto;//ロボを生成できるか
    public GameObject genRobotNo;//通常生成させるロボ
    public Vector2[] target_pos;//ロボにアタックさせる座標
    public bool touch;//ほかのオブジェに触れているか
    public bool damaged;
    #endregion
    #endregion

    // Use this for initialization
    void Start ()
    {
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
        bar = transform.FindChild("HpBar").gameObject;
        bar.transform.localPosition = new Vector3(-1, 0.6f, 0);
        bar.GetComponent<SpriteRenderer>().color = mikata ? Color.blue : Color.red;
        sp_s = sp;
        sp_cn = sp;
        auto = target_pos.GetLength(0) > 0;
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
        bar.transform.localScale = new Vector3((float)energy / (float)enmax*2, 2, 2);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Robot")
        {
            touch = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        touch = false;
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

    public IEnumerator Intake(GameObject other)
    {
        if (mikata == other.GetComponent<RobotController>().Mikata)
        {
            yield break;
        }
        other.GetComponent<RobotController>().CheckBreaking = true;
        intake = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        Damage(other.GetComponent<RobotController>().offence);
        sr.color = c;
        other.GetComponent<RobotController>().Burst();
        intake = false;
        damaged = true;
        touch = false;
    }

    IEnumerator Break(int size)
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
                StartCoroutine(g.GetComponent<RobotController>().Break());
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
            breaking = true;
            StartCoroutine(Break(effect_count));
        }
        return breaking;
    }

    //ロボ・パネルの生成
    public GameObject Generate(/*int gen_num*/GameObject g, int direction,
        Vector3 genPos, bool enable = false, params Vector2[] t_pos)
    {
        int c = g.GetComponent<RobotController>().cost;
        GameObject ob = null;
        if (energy > c)
        {
            GameObject ef;
            GameObject br;
            GameObject tr = GameObject.Find("Territory");
            TerritoryController t = tr.GetComponent<TerritoryController>();
            Vector2 s = genPos 
                + new Vector3(Mathf.Floor(t.rbdata.GetLength(0) / 2), Mathf.Floor(t.rbdata.GetLength(1) / 2));
            if (t.rbdata[(int)s.x, (int)s.y] != -1)//ロボがいるか、陣地でない
            {
                Debug.Log("dame!");
                return null;
            }
            ob = (GameObject)Instantiate(g, transform.position, transform.rotation);
            ob.transform.position = genPos;
            RobotController rc = ob.GetComponent<RobotController>();
            rc.Mikata = mikata;
            rc.auto = false;
            rc.number = t.SetRobotNumber();
            rc.Direction = direction;
            t.rbdata[t.rbdata.GetLength(0) / 2 + (int)rc.transform.position.x,
                t.rbdata.GetLength(1) / 2 + (int)rc.transform.position.y] = rc.number;
            if (t_pos.GetLength(0) > 0)
            {
                rc.auto = true;
                rc.dst = t_pos;
            }
            ef = (GameObject)Instantiate(Resources.Load("Prefabs/effect_r"), Vector2.zero, transform.rotation);
            ef.name = "effect";
            ef.transform.position = ob.transform.position;
            ef.transform.SetParent(ob.transform);
            br =
                (GameObject)Instantiate(Resources.Load("Prefabs/HpBar"), Vector2.zero, transform.rotation);
            br.name = "HpBar";
            br.transform.position = new Vector3(-0.5f, -0.3f, 0);
            br.transform.SetParent(ob.transform, false);
            br.GetComponent<SpriteRenderer>().color = ob.GetComponent<RobotController>().Mikata ? Color.blue : Color.red;
            GameObject ln =
                (GameObject)Instantiate(Resources.Load("Prefabs/line"), Vector2.zero, transform.rotation);
            ln.name = "line";
            ln.transform.position = Vector2.zero;
            ln.transform.SetParent(ob.transform, false);
            GameObject ps =
                (GameObject)Instantiate(Resources.Load("Prefabs/line"), Vector2.zero, transform.rotation);
            ps.name = "point";
            ps.transform.position = Vector2.zero;
            ps.transform.SetParent(ob.transform, false);
            energy -= c;
            rc.Start();
            rc.enabled = enable;
        }
        return ob;
    }

    public void AI()
    {
        if (/*damaged && */energy > enmax / 2)
        {
            Generate(genRobotNo, 2, transform.position, true, target_pos);
        }
    }
}
