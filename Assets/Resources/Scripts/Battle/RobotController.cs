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
    public Sprite br_effect;
    public int cost;//召喚コスト
    public int mhp;
    public int hp;
    public int offence;
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
    public int fnumber;
    public Vector2 v;
    public int typeNo;//robotのタイプ
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
                }
            }
            else//停止し、パネル、カーネルと接触処理
            {
                Vector2 s = transform.position + new Vector3(t.rbdata.GetLength(0) / 2, t.rbdata.GetLength(1) / 2, 0);
                if (sp_count >= speed)//停止処理
                {
                    transform.position = new Vector2((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y));
                    try
                    {
                        t.rbdata[(int)Math.Round(s.x - DtoV().x), (int)Math.Round(s.y - DtoV().y)] = -1;
                    }
                    catch { }
                    Collider2D[] cs = Physics2D.OverlapPointAll(transform.position);//ターゲット
                    Collider2D cp = null;//接触したパネル
                    Collider2D ck = null;//接触したカーネル
                    if (auto)
                    {
                        AI(false);
                    }
                    else
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
                            cp.gameObject.GetComponent<PanelController>().Run(gameObject);
                        }
                    }
                    foreach (Collider2D col in cs)
                    {
                        if (col.gameObject.tag == "Kernel")
                        {
                            ck = col;
                        }
                    }
                    if (ck != null)
                    {
                        ck.gameObject.GetComponent<KernelController>().StartCoroutine(
                            ck.gameObject.GetComponent<KernelController>().Intake(gameObject));
                    }
                    t.Reset(transform.position, mikata, gameObject);
                    sp_count = 0;
                }
                else
                {
                    try//前進を試みる
                    {
                        if (t.rbdata[(int)Math.Round(s.x + DtoV().x), (int)Math.Round(s.y + DtoV().y)] == -1)//目の前にロボがいない
                        {
                            t.rbdata[(int)Math.Round(s.x + DtoV().x), (int)Math.Round(s.y + DtoV().y)] = number;
                            move = true;
                            int mpx, mpy;
                            mpx = (int)(transform.position.x + DtoV(dire).x);
                            mpy = (int)(transform.position.y + DtoV(dire).y);
                            int[,] m_s = mp_lay2.GetComponent<MapLoader>().mapdata;
                            if (m_s[m_s.GetLength(0) / 2 + mpx, m_s.GetLength(1) / 2 - mpy] != 7)
                            {
                                //Stop();
                                breaking = true;
                                StartCoroutine(Break());
                            }
                        }
                        else if (!at)
                        {
                            StartCoroutine(Attack());
                        }
                    }
                    catch
                    {
                        //Stop();
                        v = Vector2.zero;
                        breaking = true;
                        StartCoroutine(Break());
                    }
                    /*if (!at)
                    {
                        StartCoroutine(Attack());
                    }*/
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

    public void Go(bool onpanel = false)
    {
        GetComponent<Rigidbody2D>().AddForce(DtoV() * speed);
        if (onpanel)
        {
            transform.Translate(DtoV() * 0.05f);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other">衝突したロボを引数にしてね</param>
    public void Stop(GameObject other = null)
    {
        if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            GetComponent<Rigidbody2D>().AddForce(DtoV() * -speed);
        }
        Vector2 r = new Vector2((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.y));
        transform.position = r;
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
            }
        }
        if(target==null)
        {
            at = false;
            ef.GetComponent<SpriteRenderer>().sprite = null;
            yield break;
        }
        RobotController r = target.GetComponent<RobotController>();
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
        for (int i = 0; i < 9; i++)
        {
            SetEffect(att_effect, i, tpos);
            if (i == 5 && target.GetComponent<RobotController>() != null && !target.GetComponent<RobotController>().CheckBreaking)
            {
                target.GetComponent<RobotController>().Damage(offence);
            }
            yield return new WaitForSeconds(0.01f);
        }
        try
        {
            target.GetComponent<SpriteRenderer>().color = Color.white;
        }
        catch { }
        yield return new WaitForSeconds(1);
        at = false;
        ef.GetComponent<SpriteRenderer>().sprite = null;
        //ef.transform.position = transform.position;
        Debug.Log("end");
    }

    public IEnumerator Break()
    {
        StopCoroutine(Attack());
        at = true;
        ef.transform.position = transform.position;
        StopCoroutine(Attack());
        GetComponent<BoxCollider2D>().isTrigger = true;
        for (int i = 0; i < 7; i++)
        {
            SetEffect(br_effect, i, transform.position);
            yield return new WaitForSeconds(0.05f);
        }
        GetComponent<SpriteRenderer>().sprite = null;
        Burst();
    }

    public bool Damage(int Offence)
    {
        hp -= Offence - defence;
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

    /*public void RouteSet()//通過ルートを処理
    {
        bool t = false;
        for (int i = 0; i < ex_panels.Count; i++)
        {
            if (ex_panels[i].x == transform.position.x && ex_panels[i].y == transform.position.y)
            {
                t = true;
            }
            Debug.Log("Check");
        }
        if (t)//ラインで囲まれたエリア内を制圧
        {
            Suppresssion();
            Debug.Log("Suppression");
        }
        else//新しい座標を追加し、ライン描画
        {
            if (ppos == new Vector2(-100, -100))
            {
                ppos = transform.position;
            }
            ex_panels.Add(ppos);
            Liner(ppos, transform.position, 32);
            Debug.Log("Line");
        }
    }*/

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
            Debug.Log("Check");
            ex_panels.Add(ppos);
        }
        Liner(ppos, transform.position, 32);
        if (t)//ラインで囲まれたエリア内を制圧
        {
            for(int i=0;i<ex_panels.Count;i++)
            {
                Debug.Log(ex_panels[i]);
            }
            Suppresssion();
            Debug.Log("Suppression");
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
        Color[] null_color = Resources.Load<Sprite>("SPrites/nullImage").texture.GetPixels(0, 0, size, size);
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
        /*Texture2D ln_t;
        Color[] b = line.texture.GetPixels(0, 0, size, size);//ライン画像データ読み込み
        Vector2 pos_l_cm = Vector2.zero;//座標補正、compensation
        if (v)
        {
            ln_t = new Texture2D(size, size * (Math.Abs(length) + 1), TextureFormat.RGBA32, false);
            ln_t.filterMode = FilterMode.Point;
            for (int i = 0; i <= Mathf.Abs(length); i++)
            {
                ln_t.SetPixels(0, i * size, size, size, null_color);//画像初期化
            }
            pos_l_cm.y = /*length < 0 ? -0.5f : 0.5f;
            for (int i = 0; i < Mathf.Abs(length); i++)//ライン描画
            {
                ex_panels.Add(new Vector2(pos1.x, pos1.y + (i + 1) * Mathf.Abs(length) / length));//ライン上の点を座標系に追加
                ln_t.SetPixels(0, size * i + size / 2, size, size, b);
                ln_t.Apply();
            }
        }
        else
        {
            length = (int)(pos2.x - pos1.x);
            ln_t = new Texture2D(size * (Math.Abs(length) + 1), size, TextureFormat.RGBA32, false);
            ln_t.filterMode = FilterMode.Point;
            for (int i = 0; i <= Mathf.Abs(length); i++)
            {
                ln_t.SetPixels(i * size, 0, size, size, null_color);//画像初期化
            }
            pos_l_cm.x = length < 0 ? -0.5f : 0.5f;
            Color[] b_s = new Color[b.GetLength(0)];
            for (int i = 0; i < b.GetLength(0); i++)//ライン画像を90度回転
            {
                b_s[i] = b[size * (i % size + 1) - 1 - i / size];
            }
            for (int i = 0; i < Mathf.Abs(length); i++)//ライン描画
            {
                ex_panels.Add(new Vector2(pos1.x + (i + 1) * Mathf.Abs(length) / length, pos1.y));
                ln_t.SetPixels(size * i + size / 2, 0, size, size, b_s);
                ln_t.Apply();
            }
            l.transform.eulerAngles = new Vector3(0, 0, 90);
            
        }*/
        /*l.GetComponent<SpriteRenderer>().sprite = Sprite.Create(ln_t, new Rect(0, 0, ln_t.width, ln_t.height),
            new Vector2(0.5f, 0.5f), size);*/
        //p.GetComponent<SpriteRenderer>().sprite = point;
        //int[] at = GetAreaSize();
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
//Debug.Log(new Vector2((int)ex_panels[i].x - mix + 1, (int)ex_panels[i].y - miy + 1));
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
        Debug.Log("Panels" + ex_panels.Count);
        Debug.Log(mix);
        Debug.Log(miy);
        Debug.Log(max);
        Debug.Log(may);
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
    void AI(bool tate/*移動方向*/)
    {
        bool c = false;
        float ln;
        if (dst_lt[0].x == (int)Math.Round(transform.position.x, 0))//横方向の移動を考慮
        {
            if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                Stop();
            }
            ln = dst_lt[0].y - transform.position.y;
            if (ln < 0)//下方向に移動
            {
                Turn(0);
            }
            else
            {
                Turn(2);
            }
            if (dst_lt[0].y == (int)Math.Round(transform.position.y, 0))
            {
                c = true;
            }
            //Go();
        }
        else
        /*{
            ln = dst_lt[0].x - transform.position.x;
        }
        ln = dst_lt[0].x - transform.position.x;
        if (dst_lt[0].y == (int)Math.Round(transform.position.y, 0))*/
        {
            if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                Stop();
            }
            ln = dst_lt[0].x - transform.position.x;
            if (ln < 0)//左方向
            {
                Turn(3);
            }
            else
            {
                Turn(1);
            }
            //Go();
        }
        if(c)
        {
            dst_lt.RemoveAt(0);
            if(dst_lt.Count==0)
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
