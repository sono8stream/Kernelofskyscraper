using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public KernelController k;
    public KernelController k_e;//敵カーネル
    public Text t;
    public Text t_e;
    public Sprite stg_clear_image;
    public Sprite game_over_image;
    public GameObject[] robots;//生成するロボ
    List<Button> br;
    public GameObject[] panels;//生成するパネル
    List<Button> bp;

    int mode = 0;//0:ロボット 1:パネル
    Vector2 set_pos;//オブジェクトをセットする座標
    int gen_num;//生成するオブジェクトの番号
    bool rbt;//ロボットかどうか
    int pn_dire = 0;//基本値は0
    GameObject g;//オブジェクト配置時、設置見込み位置
    GameObject setdr;

    TerritoryController tr;
    public Text gt;

    public bool ender;
    public bool clear;

    enum ModeText
    {
        UI_Menu_Robo = 0, UI_Menu_Pan
    }
    // Use this for initialization
    void Start()
    {
        #region ロボ、パネルボタン初期化
        GameObject bt;
        br = new List<Button>();
        for (int i = 0; i < robots.GetLength(0); i++)
        {
            bt = (GameObject)Instantiate(Resources.Load("Prefabs/RoboButton"),
                transform.position, transform.rotation);//新たなロボボタン 
            br.Add(bt.GetComponent<Button>());
            br[i].transform.SetParent(transform, false);
            br[i].transform.localPosition = new Vector2(100 + 60 * (i % 3), 70 - 55 * (i / 3));
            int SIZE = 32;
            Texture2D image = new Texture2D(SIZE, SIZE);
            RobotController r = robots[i].GetComponent<RobotController>();
            Color[] c = robots[i].GetComponent<SpriteRenderer>().sprite.texture.
                GetPixels(SIZE * (1 + (r.im_num % 4) * 3),
                SIZE * (7 - 4 * (r.im_num / 4) - r.Direction),
                SIZE, SIZE);
            image.SetPixels(0, 0, SIZE, SIZE, c);
            image.Apply();
            br[i].GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0, 0, SIZE, SIZE),
                new Vector2(0.5f, 0.5f), SIZE);
            br[i].transform.FindChild("Text").gameObject.SetActive(false);
            int num = i;
            br[i].onClick.AddListener(() => SetNumber(num, true));
        }
        bp = new List<Button>();
        for (int i = 0; i < panels.GetLength(0); i++)
        {
            bt = (GameObject)Instantiate(Resources.Load("Prefabs/PanButton"),
                transform.position, transform.rotation);//新たなパネルボタン
            bp.Add(bt.GetComponent<Button>());
            bp[i].transform.SetParent(transform, false);
            bp[i].transform.localPosition = new Vector2(100 + 60 * (i % 3), 70 - 55 * (i / 3));
            bp[i].GetComponent<Image>().sprite = panels[i].GetComponent<SpriteRenderer>().sprite;
            bp[i].transform.FindChild("Text").gameObject.SetActive(false);
            int num = i;
            bp[i].onClick.AddListener(() => SetNumber(num, false));
        }
        //通常画面uiの読み込み
        foreach (Transform child in transform)
        {
            if (child.name == "RoboTab" && mode == 0)
            {
                child.transform.SetSiblingIndex(7);
            }
            if (child.name == "PanTab" && mode == 1)
            {
                child.transform.SetSiblingIndex(7);
            }
            if (child.tag != "UI_Normal" && child.tag!="UI_Regular")//通常画面以外無効化
            {
                child.gameObject.SetActive(false);
            }
        }
        #endregion
        set_pos = Vector2.zero;
        g = GameObject.Find("ObjectExpectation");
        setdr = GameObject.Find("SetDirection");

        tr = GameObject.Find("Territory").GetComponent<TerritoryController>();
        //gt = GameObject.Find("SetObject").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        /*t.text = k.energy + "/" + k.enmax;
        t_e.text = k_e.energy + "/" + k_e.enmax;*/
        if (k == null || k_e == null)
        {
            if (k == null)
            {
                transform.FindChild("EndMessage").GetComponent<Image>().sprite = game_over_image;
            }
            else
            {
                transform.FindChild("EndMessage").GetComponent<Image>().sprite = stg_clear_image;
            }
            ChangeScene(false,"UI_End");
            SetPause();
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }

    void ChangeScene(bool regular,params string[] en_name)
    {
        bool cdn;
        foreach (Transform child in transform)
        {
            cdn = false;
            if (regular&&child.tag == "UI_Regular")
            {
                cdn = true;
            }
            else
            {
                for (int i = 0; i < en_name.GetLength(0); i++)
                {
                    cdn |= child.tag == en_name[i];
                }
            }
            if (cdn)//メニュー呼び出し
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void onMenu()
    {
        ChangeScene(true,"UI_Menu", Enum.GetName(typeof(ModeText), mode));
        g.GetComponent<SpriteRenderer>().sprite = null;
        setdr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetPause(false);
    }

    public void endMenu()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "UI_Normal"||child.tag=="UI_Regular")//通常画面呼び出し
            {
                child.gameObject.SetActive(true);
            }
            else//それ以外無効化
            {
                child.gameObject.SetActive(false);
            }
        }
        g.GetComponent<SpriteRenderer>().sprite = null;
        setdr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        SetPause(true);
    }

    public void ChangeTab(int enmode)
    {
        if (enmode == mode)
        {
            return;
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.name == "RoboTab" && enmode == 0)//前面に
                {
                    child.transform.SetSiblingIndex(7);
                }
                if (child.name == "PanTab" && enmode == 1)
                {
                    child.transform.SetSiblingIndex(7);
                }
                if (child.tag == Enum.GetName(typeof(ModeText), enmode))
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.tag == Enum.GetName(typeof(ModeText), mode))
                {
                    child.gameObject.SetActive(false);
                }
            }
            mode = enmode;
            set_pos.x = -100;
            set_pos.y -= 100;
        }
    }

    //ロボ・パネルの種類と生成する番号をセット
    public void SetNumber(int number, bool robot)
    {
        gen_num = number;
        rbt = robot;
        foreach (Transform child in transform)
        {
            if (child.tag == "UI_Object_Set" || child.tag == "UI_Regular")
            {
                child.gameObject.SetActive(true);
                if (child.name == "SelectObject")
                {
                    Sprite s;
                    if (rbt)
                    {
                        s = br[gen_num].GetComponent<Image>().sprite;
                        g.transform.position = GameObject.Find("kernel").transform.position;
                        g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
                        set_pos = g.transform.position;
                        setdr.transform.position = set_pos;
                        setdr.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        s = bp[gen_num].GetComponent<Image>().sprite;
                        g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        g.transform.position = Vector2.zero;
                    }
                    g.GetComponent<SpriteRenderer>().sprite = s;
                    child.GetComponent<Image>().sprite = s;
                }
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
        pn_dire = 0;
        setdr.transform.eulerAngles = Vector3.zero;
        GameObject.Find("SelectObject").transform.eulerAngles = Vector3.zero;
        set_pos.x = 100;
        set_pos.y = 100;
        g.transform.eulerAngles = Vector3.zero;
    }

    public void SetPosition()
    {
        Vector3 touch_pos = Input.mousePosition;
        Vector2 t_pos = Camera.main.ScreenToWorldPoint(touch_pos);
        t_pos.x = Mathf.Round(t_pos.x);
        t_pos.y = Mathf.Round(t_pos.y);
        //Debug.Log(t_pos);
        GameObject ob = GameObject.Find("Main Camera").GetComponent<ScreenManager>().Object;
        if (ob == null || (ob.tag != "Panel" && ob.tag != "Robot"))
        {
            if ((rbt && tr.rbdata[tr.rbdata.GetLength(0) / 2 + (int)k.transform.position.x
                , tr.rbdata.GetLength(1) / 2 + (int)k.transform.position.y] == -1)
                || (!rbt && set_pos == t_pos))//2回目のタップ？
            {
                Generate();
                //g.GetComponent<SpriteRenderer>().sprite = null;
                Debug.Log("aa");
            }
            else if (!rbt)
            {
                set_pos = t_pos;
                g.transform.position = t_pos;
                if (g.GetComponent<SpriteRenderer>().color.a == 0)
                {
                    g.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.6f);
                }
            }
        }
    }

    //ロボ・パネルの生成
    public void Generate()
    {
        Debug.Log(gen_num);
        if (rbt)
        {
            //k.Generate(robots[gen_num],pn_dire,);
            endMenu();
        }
        else
        {
            GameObject ob = (GameObject)Instantiate(panels[gen_num], set_pos, transform.rotation);
            ob.GetComponent<PanelController>().direction = pn_dire;
            GameObject ef = (GameObject)Instantiate(Resources.Load("Prefabs/effect_p"), Vector2.zero, transform.rotation);
            ef.name = "effect";
            ef.transform.position = ob.transform.position;
            ef.transform.SetParent(ob.transform);
        }
    }

    public void ReturnSelect()
    {
        ChangeScene(true, "UI_Menu", Enum.GetName(typeof(ModeText), mode));
    }

    public void RotatePanel()
    {
        pn_dire++;
        pn_dire %= 4;
        Vector3 an = new Vector3(0, 0, 90 * pn_dire);
        if (!rbt && panels[gen_num].GetComponent<PanelController>().turnable)
        {
            GameObject.Find("SelectObject").transform.eulerAngles = an;
            g.transform.eulerAngles = an;
        }
        else if (rbt)
        {
            setdr.transform.eulerAngles = an;
        }
    }

    void RotateImage(ref Sprite s,int size)
    {
        Color[] c = s.texture.GetPixels(0, 0, size, size);
        Color[] cs = new Color[size*size];
        for (int i = 0; i < size; i++)//y
        {
            for (int j = 0; j < size; j++)//x
            {
                cs[j + size * (size - i)] = cs[i + size * j];
            }
        }
        s.texture.SetPixels(0, 0, size, size, c);
        s.texture.Apply();
    }

    public void RemovePanel()
    {
        Vector3 touch_pos = Input.mousePosition;
        Vector2 t_pos = Camera.main.ScreenToWorldPoint(touch_pos);
        Collider2D[] c = Physics2D.OverlapPointAll(t_pos);
        try
        {
            GameObject g = new GameObject();
            foreach (Collider2D cs in c)
            {
                if (cs.gameObject.tag == "Panel")
                {
                    g = cs.gameObject;
                }
            }
            Debug.Log(g.name);
            Destroy(g);
        }
        catch { }
    }

    void SetPause(bool pause = false)
    {
        GameObject[] gk = GameObject.FindGameObjectsWithTag("Kernel");
        foreach (GameObject gs in gk)
        {
            gs.GetComponent<KernelController>().enabled = pause;
        }
        GameObject[] gr = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject gs in gr)
        {
            if (gs.GetComponent<RobotController>() != null)
            {
                gs.GetComponent<RobotController>().enabled = pause;
            }
        }
        GameObject gt = GameObject.Find("Territory");
        gt.GetComponent<TerritoryController>().enabled = pause;
    }
}
