using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#pragma warning disable
public class RobotController : MonoBehaviour {

    #region　プロパティ
    public Collider2D tar;
    public int number;//
    public Sprite image_all;//キャラチップ全パターン
    public int im_num;//画像番号
    Color[] c;//色データ
    Vector2[] points;
    Texture2D image;//瞬間のキャラチップ
    int ani_pat;//歩行パターン
    int last_ani_pat;
    int dire;//方向
    public int Direction
    {
        get { return dire; }
        set { dire = value; }
    }
    bool mikata;//自陣営かどうか
    public bool Mikata
    {
        get { return mikata; }
        set { mikata = value; }
    }
    int ani_count;//歩行パターン切り替えウェイト
    const int SIZE = 32;//画像サイズ
    int aniSpan;//アニメーションの遷移間隔
    public bool paneling;//パネル上の処理を実行中か
    public int speed;//移動スピード
    public int sp_count;
    public int Speed_Count
    {
        get { return sp_count; }
        set { sp_count = value; }
    }
    public bool go;//進むかどうか
    public Sprite att_effect;
    public Sprite AttackEffect
    {
        get { return att_effect; }
        set { att_effect = value; }
    }
    public int attEffectPat;
    public Sprite br_effect;
    public int cost;//召喚コスト
    public int mhp;
    public int hp;
    public int attack;
    public int defence;
    GameObject ef;
    public GameObject EffectObject
    {
        get { return ef; }
        set { ef = value; }
    }
    GameObject bar;
    Vector2 ppos;//前回踏んだパネルの座標
    List<Vector2> ex_panels;//超えたパネルの座標リスト
    GameObject ln;//通過ライン
    GameObject pt;//通過ポイント
    bool move;
    public bool Move
    {
        get { return move; }
        set { move = value;}
    }
    bool at;
    public bool CheckAttack
    {
        get
        {
            return at;
        }
        set
        {
            at = value;
        }
    }
    bool attacking;
    bool breaking = false;
    public bool CheckBreaking
    {
        get
        {
            return breaking;
        }
        set
        {
            breaking = value;
        }
    }
    GameObject mp_lay2;//マップレイヤー2
    public GameObject ek;//敵のカーネル情報
    bool dmas;//マス移動時、一度だけ呼び出し
    TerritoryController t;
    GameObject cother;//衝突した他のロボット
    #region 自動移動用
    public Vector2[] dst;
    List<Vector2> dst_lt;
    public bool auto;
    #endregion
    public KernelController kerCon;
    public int fnumber;
    public Vector2 v;
    public int typeNo;//robotのタイプ
    public bool isReady;//生成可能か（主に生成に条件があるとき）
    public int triggerNo = -1;//破壊時にセットするロボの番号
    #endregion

    // Use this for initialization
    public void Start ()
    {
        ani_pat = 1;//真ん中に調整
        last_ani_pat = 0;
        ani_count = 0;
        //dire = 0;//方向を下向きに
        image = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
        SetImage();
        paneling = false;
        /*if(go)
        {
            Go();
        }*/
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
        bar = transform.FindChild("HpBar").gameObject;
        ex_panels = new List<Vector2>();
        at = false;
        attacking = false;
        mp_lay2 = GameObject.Find("MapLayer2");
        ln = transform.FindChild("line").gameObject;
        ln.GetComponent<SpriteRenderer>().sprite = null;
        pt = transform.FindChild("point").gameObject;
        pt.GetComponent<SpriteRenderer>().sprite = null;
        ppos = new Vector2(-100, -100);
        dst_lt = new List<Vector2>();
        dst_lt.AddRange(dst);
        t = GameObject.Find("Territory").GetComponent<TerritoryController>();
        move = false;
        sp_count = 0;
        cother = null;
        if(typeNo==(int)RobotType.Human||typeNo==(int)RobotType.Bomb)
        {
            aniSpan = 10;
        }
        else
        {
            aniSpan = 20;
        }
        GameObject hpBar = transform.FindChild("HpBar").gameObject;
        hpBar.transform.localPosition = new Vector3(-0.5f, -0.3f, 0);
        hpBar.GetComponent<SpriteRenderer>().color = mikata ? Color.blue : Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if ((typeNo == (int)RobotType.Human || typeNo == (int)RobotType.Bomb)
            && !breaking)//人間タイプ、ボムタイプなら移動
        {
            if (move)//実際に移動処理
            {
                if (sp_count < speed)
                {
                    transform.Translate(DtoV() / speed);
                    SetImage();
                    sp_count++;
                }
                if (sp_count == speed)
                {
                    move = false;
                    sp_count = 0;
                    transform.position =
                        new Vector2((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y));
                    Vector2 sub = transform.position - DtoV()
                        + new Vector3(Mathf.Floor(t.rbdata.GetLength(0) / 2), Mathf.Floor(t.rbdata.GetLength(1) / 2));
                    if (0 <= sub.x && sub.x < t.rbdata.GetLength(0) && 0 <= sub.y && sub.y < t.rbdata.GetLength(1))
                    {
                    t.rbdata[(int)sub.x, (int)sub.y] = -1;
                    }
                    if (auto)
                    {
                        AI();
                    }
                    CheckDot();
                }
            }
            else//停止し、パネル、カーネルと接触処理
            {
                Vector2 sub = new Vector3(Mathf.Floor(t.rbdata.GetLength(0) / 2), Mathf.Floor(t.rbdata.GetLength(1) / 2));
                Vector3 tarPos = new Vector3(transform.position.x + DtoV().x, transform.position.y + DtoV().y);
                int posX, posY;
                posX = (int)(sub.x + tarPos.x);
                posY = (int)(sub.y + tarPos.y);
                if (0 <= posX && posX < t.rbdata.GetLength(0) && 0 <= posY && posY < t.rbdata.GetLength(1))//前進を試みる
                {
                    if (t.rbdata[posX, posY] == -1)//目の前にロボがいない
                    {
                        t.rbdata[posX, posY] = number;
                        int[,] m_s = mp_lay2.GetComponent<MapLoader>().mapdata;
                        if (m_s[posX, posY] != 7)//移動不可マスに衝突
                        {
                            breaking = true;
                            StartCoroutine(Break());
                        }
                        else
                        {
                            move = true;
                        }
                    }
                    else if (!at)
                    {
                        StartCoroutine(Attack());
                    }
                }
                else
                {
                    breaking = true;
                    StartCoroutine(Break());
                }
            }
            ln.transform.position = Vector2.zero;
            pt.transform.position = Vector2.zero;
        }
        else if (!at)//フィギュリンタイプの攻撃
        {
            dire++;
            if (dire >= 4)
            {
                dire = 0;
            }
            StartCoroutine(Attack());
        }
        bar.transform.localScale = new Vector3((float)hp / (float)mhp, 1, 1);
    }

    void FixedUpdate()
    {
        if (typeNo != (int)RobotType.Figurine)
        {
            ani_count++;
            if (ani_count >= aniSpan)
            {
                ani_count = 0;
                int ani_sub = last_ani_pat;
                last_ani_pat = ani_pat;
                ani_pat = Mathf.Abs(ani_sub - 2);
                SetImage();
            }
        }
    }

    /// <summary>
    /// 足元のパネル、カーネルの情報を取得、処理
    /// </summary>
    void CheckDot()
    {
        Collider2D[] cs = Physics2D.OverlapPointAll(transform.position);//ターゲット
        Collider2D cp = null;//接触したパネル
        Collider2D ck = null;//接触したカーネル
        if (mikata)
        {
            foreach (Collider2D col in cs)
            {
                if (col.gameObject.tag == "Panel")
                {
                    cp = col;
                }
            }
            if (cp != null)
            {
                cp.gameObject.GetComponent<PanelController>().Run(gameObject);//パネル効果実行
            }
        }
        foreach (Collider2D col in cs)
        {
            if (col.gameObject.tag == "Kernel")
            {
                ck = col;
                break;
            }
        }
        if (ck != null)
        {
            KernelController ker = ck.gameObject.GetComponent<KernelController>();
            if (Mathf.RoundToInt(ker.transform.position.x - transform.position.x) == 0
                && Mathf.RoundToInt(ker.transform.position.y - transform.position.y) == 0)
            {
                ker.StartCoroutine(ker.Intake(gameObject));
            }
        }
        else
        {
            t.Generate(transform.position, mikata, gameObject);
        }
    }

    public void SetImage()
    {
        c = image_all.texture.GetPixels(SIZE * (ani_pat + (im_num % 4) * 3), SIZE * (7 - 4 * (im_num / 4) - dire), SIZE, SIZE);
        image.SetPixels(0, 0, SIZE, SIZE, c);
        image.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(image, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), 32);
    }

    /// <summary>
    /// direをintからvector2へ
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Vector3 DtoV(int direction = -1)
    {
        if (direction == -1)
        {
            direction = dire;
        }
        Vector2 pos = Vector2.zero;
        switch(direction)
        {
            case 0:
                pos.y = -1;
                break;
            case 1:
                pos.x = 1 /* speed*/;
                break;
            case 2:
                pos.y = 1 /* speed*/;
                break;
            case 3:
                pos.x = -1 /* speed*/;
                break;
        }
        return pos;
    }

    /// <summary>
    /// パネル効果呼び出し
    /// </summary>
    /// <param name="r"></param>
    void Call_Panel(RobotController r)
    {
        //ここにパネルの処理
    }

    /// <summary>
    /// 方向転換
    /// </summary>
    /// <param name="direction"></param>
    public void Turn(int direction)
    {
        dire = direction;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    IEnumerator Attack()
    {
        at = true;
        Collider2D[] targets = Physics2D.OverlapPointAll(transform.position + DtoV());
        Collider2D target = null;
        foreach (Collider2D t in targets)
        {
            if (t.tag == "Robot")
            {
                target = t;
                break;
            }
        }
        if (target == null)
        {
            at = false;
            ef.GetComponent<SpriteRenderer>().sprite = null;
            yield break;
        }
        RobotController r = target.GetComponent<RobotController>();
        target.GetComponent<SpriteRenderer>().color = Color.red;
        Vector2 tpos = Vector2.zero;
        if (/*target != null && target.tag == "Robot" &&*/ !r.breaking && r.Mikata != mikata)
        {
            tpos = target.transform.position;
            target.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            tpos = Vector2.zero;
            at = false;
            ef.GetComponent<SpriteRenderer>().sprite = null;
            yield break;
        }
        for (int i = 0; i < attEffectPat; i++)
        {
            SetEffect(att_effect, i, tpos);
            if (i == attEffectPat/2 &&!target.GetComponent<RobotController>().CheckBreaking)
            {
                target.GetComponent<RobotController>().Damage(attack);
            }
            yield return new WaitForSeconds(0.01f);
        }
        if (target != null)
        {
            target.GetComponent<SpriteRenderer>().color = Color.white;
        }
        yield return new WaitForSeconds(1);
        at = false;
        ef.GetComponent<SpriteRenderer>().sprite = null;
    }

    public IEnumerator Break()
    {
        StopCoroutine(Attack());
        at = true;
        ef.transform.position = transform.position;
        GetComponent<BoxCollider2D>().isTrigger = true;
        if (auto && triggerNo != -1)
        {
            kerCon.genNo = triggerNo;
            kerCon.genRobots[kerCon.genNo].gameObject.GetComponent<RobotController>().isReady = true;
        }
        for (int i = 0; i < 7; i++)
        {
            SetEffect(br_effect, i, transform.position);
            yield return new WaitForSeconds(0.05f);
        }
        GetComponent<SpriteRenderer>().sprite = null;
        Burst();
    }

    public bool Damage(int Attack)
    {
        hp -= Attack - defence;
        if (hp <= 0)
        {
            breaking = true;
            StartCoroutine(Break());
        }
        return breaking;
    }

    Texture2D SetEffect(Sprite s, int num, Vector2 targetpos)
    {
        Texture2D t = new Texture2D(120, 120, TextureFormat.RGBA32, false);
        if (targetpos != (Vector2)transform.position)
        {
            ef.transform.position = targetpos;
        }
        Color[] c = s.texture.GetPixels(120 * num, 0, 120, 120);
        t.SetPixels(0, 0, 120, 120, c);
        t.Apply();
        ef.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, 120, 120), new Vector2(0.5f, 0.5f), 90);
        return t;
    }

    /// <summary>
    /// エリア選択、制圧メソッド
    /// </summary>
    public void Zoning()
    {
        bool t = false;
        for (int i = 0; i < ex_panels.Count; i++)
        {
            if (ex_panels[i].x == transform.position.x && ex_panels[i].y == transform.position.y)
            {
                t = true;
            }
        }
        //新しい座標を追加し、ライン描画
        if (ppos == new Vector2(-100, -100))
        {
            ppos = transform.position;
            ex_panels.Add(ppos);
        }
        Liner(ppos, transform.position, 32);
        if (t)//ラインで囲まれたエリア内を制圧
        {
            Suppresssion();
        }
    }

    /// <summary>
    /// 通過したラインを描画
    ///pos1:前
    /// pos2:後
    /// </summary>
    void Liner(Vector2 pos1, Vector2 pos2, int size)
    {
        Sprite point = Resources.Load<Sprite>("Sprites/pole");//ポイント画像
        Sprite line = Resources.Load<Sprite>("Sprites/bar");//ライン画像読み込み
        if (pos1 != pos2)
        {
            GameObject l = (GameObject)Instantiate(Resources.Load("Prefabs/line"), Vector2.zero, transform.rotation);//ライン生成
            l.name = "l";
            l.transform.position = new Vector2((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2);
            l.transform.SetParent(ln.transform, false);
            l.GetComponent<SpriteRenderer>().sprite = line;
            int length;//ラインの長さ
            bool v = (pos1.x == pos2.x);//縦かどうか
            if(v)
            {
                length = (int)(pos2.y - pos1.y);//ラインの長さ決定
                for (int i = 0; i < Mathf.Abs(length); i++)//ライン描画
                {
                    ex_panels.Add(new Vector2(pos1.x, pos1.y + (i + 1) * Mathf.Abs(length) / length));//ライン上の点を座標系に追加
                }
            }
            else
            {
                length = (int)(pos2.x - pos1.x);
                l.transform.eulerAngles = new Vector3(0, 0, 90);
                for (int i = 0; i < Mathf.Abs(length); i++)//ライン描画
                {
                    ex_panels.Add(new Vector2(pos1.x + (i + 1) * Mathf.Abs(length) / length, pos1.y));//ライン上の点を座標系に追加
                }
            }
            l.transform.localScale = new Vector3(1, Math.Abs(length), 1);
        }
        GameObject p = (GameObject)Instantiate(Resources.Load("Prefabs/line"), Vector2.zero, transform.rotation);//ポイント生成
        p.name = "p";
        p.transform.position = new Vector3(pos2.x, pos2.y, -1);
        p.transform.SetParent(pt.transform, false);
        ppos = transform.position;
    }

    /// <summary>
    /// エリアの制圧メソッド
    /// </summary>
    void Suppresssion()
    {
        int[] at = GetAreaSize();
        int mix = at[0];
        int miy = at[1];
        int max = at[2];
        int may = at[3];
        bool[,] area = new bool[max - mix + 3, may - miy + 3];//エリアデータ
        for (int i = 0; i < area.GetLength(0); i++)//初期化
        {
            for (int j = 0; j < area.GetLength(1); j++)
            {
                area[i, j] = true;
            }
        }
        for (int i = 0; i < ex_panels.Count; i++)//まず、ライン上を偽にする
        {
            try
            {
                area[(int)ex_panels[i].x - mix + 1, (int)ex_panels[i].y - miy + 1] = false;
            }
            catch { }
        }
        Search(0, 0, -1, ref area);//エリア外を調査
        for (int i = 0; i < ex_panels.Count; i++)//最後に、ライン上を真にする
        {
            try
            {
                area[(int)ex_panels[i].x - mix + 1, (int)ex_panels[i].y - miy + 1] = true;
            }
            catch { }
        }
        GameObject g = GameObject.Find("Territory");
        foreach (Transform child in transform)
        {
            if (child.name == "line" || child.name == "point")
            {
                foreach(Transform c in child.transform)
                {
                    Destroy(c.gameObject);
                }
            }
        }
        while (ex_panels.Count != 0)
        {
            ex_panels.RemoveAt(0);
        }
        ppos = new Vector2(-100, -100);
        StartCoroutine(g.GetComponent<TerritoryController>().Refresh(mix, miy, max, may,area));
    }

    void Search(int x, int y, int d/*調査方向*/, ref bool[,] areadata)
    {
        if (!areadata[x, y])
        {
            return;
        }
        else
        {
            areadata[x, y] = false;
            if (d != 2 && y > 0)
            {
                Search(x, y - 1, 0, ref areadata);
            }
            if (d != 3 && x < areadata.GetLength(0) - 1)
            {
                Search(x + 1, y, 1, ref areadata);
            }
            if (d != 0 && y < areadata.GetLength(1) - 1)
            {
                Search(x, y + 1, 2, ref areadata);
            }
            if (d != 1 && x > 0)
            {
                Search(x - 1, y, 3, ref areadata);
            }
        }
    }

    int[] GetAreaSize()
    {
        int mix = 100;
        int miy = 100;
        int max = -100;
        int may = -100;
        for (int i = 0; i < ex_panels.Count; i++)//エリア範囲座標の最小値、最大値取得
        {
            if (mix > ex_panels[i].x)
            {
                mix = (int)ex_panels[i].x;
            }
            if (miy > ex_panels[i].y)
            {
                miy = (int)ex_panels[i].y;
            }
            if (max < ex_panels[i].x)
            {
                max = (int)ex_panels[i].x;
            }
            if (may < ex_panels[i].y)
            {
                may = (int)ex_panels[i].y;
            }
        }
        return new int[4] { mix, miy, max, may };
    }

    /// <summary>
    /// 自動移動
    /// dstの座標を順に移動
    /// </summary>
    void AI()
    {
        bool c = false;
        float ln;
        if (dst_lt[0].x == (int)Math.Round(transform.position.x, 0))//x座標が達しているなら
        {
            ln = dst_lt[0].y - transform.position.y;
            dire = ln < 0 ? 0 : 2;//上下で方向を取得
            if (dst_lt[0].y == (int)Math.Round(transform.position.y, 0))
            {
                c = true;
            }
        }
        else//x座標が達していないなら
        {
            ln = dst_lt[0].x - transform.position.x;
            dire = ln < 0 ? 3 : 1;//左右でで方向を取得
        }
        if (c)
        {
            dst_lt.RemoveAt(0);
            if (dst_lt.Count == 0)
            {
                auto = false;
            }
        }
    }

    /// <summary>
    /// 破壊されたとき
    /// </summary>
    public void Burst()
    {
        t.AdjustRobotNumber(number);
        Destroy(gameObject);
    }

    public void CompleteGeneration()
    {
        GetComponent<Animator>().SetBool("Generated", true);
        //GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }
}

public enum RobotType
{
    Human = 0, Bomb, Figurine
}
