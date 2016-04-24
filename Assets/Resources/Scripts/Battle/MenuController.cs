using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{

    public GameObject[] robots;//生成するロボ
    List<Button> br;
    public GameObject[] panels;//生成するパネル
    List<Button> bp;
    int generateNo;
    bool isRobot;
    bool isOnMenu;
    bool gameStop;
    int panelDire = 0;//パネルの向き、基本値は0
    GameObject g;//オブジェの配置予想
    List<GameObject> setDire;
    public KernelController kerCon, kerConEnemy;
    public Sprite loseImage, winImage;
    Vector2 setPos,setPosSub;//subで押下時の座標を取り、setposがそれと一致したときにパネル生成
    public Text tabText;

    // Use this for initialization
    void Start()
    {
        //各コマンド初期化
        Transform commandList = transform.FindChild("CommandList");
        GameObject bt;
        EventTrigger trigger;
        EventTrigger.Entry entryDown, entryDrag, entryEndDrag;
        br = new List<Button>();//ロボコマンド初期化
        for (int i = 0; i < robots.GetLength(0); i++)
        {
            bt = (GameObject)Instantiate(Resources.Load("Prefabs/RoboButton"),
                transform.position, transform.rotation);//新たなロボボタン 
            br.Add(bt.GetComponent<Button>());
            br[i].transform.SetParent(commandList, true);
            br[i].transform.localPosition = new Vector2(-500 + 250 * (i % 5), 0);
            br[i].transform.localScale = Vector3.one;
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
            //br[i].onClick.AddListener(() => SetNumber(i, true, 0));
            trigger = br[i].GetComponent<EventTrigger>();
            entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            int genNo = i;
            entryDown.callback.AddListener((x) => SetNumber(genNo, true, 0, genNo));
            trigger.triggers.Add(entryDown);
        }
        bp = new List<Button>();//パネルコマンド初期化
        int panelCount = 0;
        for (int i = 0; i < panels.GetLength(0); i++)
        {
            int generateCount = 1;
            if (panels[i].GetComponent<PanelController>().turnable)//回転可能ならば、その分だけボタンを生成
            {
                generateCount = 4;
            }
            for (int j = 0; j < generateCount; j++)
            {
                bt = (GameObject)Instantiate(Resources.Load("Prefabs/PanButton"),
                    transform.position, Quaternion.Euler(0, 0, 90 * j));//新たなパネルボタン
                bp.Add(bt.GetComponent<Button>());
                bp[panelCount].transform.SetParent(commandList, true);
                bp[panelCount].transform.localPosition = new Vector2(-500 + 250 * (panelCount % 5), 0);
                bp[panelCount].transform.localScale = Vector3.one;
                bp[panelCount].GetComponent<Image>().sprite = panels[i].GetComponent<SpriteRenderer>().sprite;
                bp[panelCount].transform.FindChild("Text").gameObject.SetActive(false);
                //bp[panelCount].onClick.AddListener(() => SetNumber(i, false, j));
                trigger = bp[panelCount].GetComponent<EventTrigger>();
                entryDown = new EventTrigger.Entry();
                entryDown.eventID = EventTriggerType.PointerDown;
                int genNo = i, dire = j, panelNo = panelCount;
                entryDown.callback.AddListener((x) => SetNumber(genNo, false, dire, panelNo));
                trigger.triggers.Add(entryDown);
                panelCount++;
            }
        }
        g = GameObject.Find("ObjectExpectation");
        setDire = new List<GameObject>();
        setDire.Add(GameObject.Find("SetDirection"));
        for (int i = 0; i < 4; i++)//各方向の矢印を設定
        {
            if (0 < i)
            {
                setDire.Add((GameObject)Instantiate(setDire[0],
                        setDire[0].transform.position, Quaternion.Euler(0, 0, 90 * i)));
            }
            setDire[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            trigger = setDire[i].GetComponent<EventTrigger>();
            if (0 < i)
            {
                trigger.triggers.RemoveAt(0);
            }
            entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            int dire = i;
            entryDown.callback.AddListener((x) => Generate(dire));
            trigger.triggers.Add(entryDown);
        }
        ChangeTab();
        OnMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (kerCon == null || kerConEnemy == null)
        {
            transform.FindChild("EndMessage").GetComponent<Image>().enabled = true;
            if (kerCon == null)
            {
                transform.FindChild("EndMessage").GetComponent<Image>().sprite = loseImage;
            }
            else
            {
                transform.FindChild("EndMessage").GetComponent<Image>().sprite = winImage;
            }
            if (!gameStop)
            {
                TimeHandle();
                isOnMenu = true;
                OnMenu();
            }
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

    //ロボ・パネルの種類と生成する番号をセット
    public void SetNumber(int generateNo, bool isRobot, int panelDire,int panelCount)
    {
        Debug.Log(generateNo);
        this.generateNo = generateNo;
        this.isRobot = isRobot;
        foreach (Transform child in transform)
        {
            /*if (child.tag == "UI_Object_Set" || child.tag == "UI_Regular")
            {
                child.gameObject.SetActive(true);
                if (child.name == "SelectObject")
                {*/
            Sprite s;
            if (isRobot)
            {
                s = br[generateNo].GetComponent<Image>().sprite;
                setPos = GameObject.Find("kernel").transform.position;
                if (robots[generateNo].GetComponent<RobotController>().typeNo == (int)RobotType.Figurine)
                {
                    transform.FindChild("SetObject").gameObject.SetActive(true);
                }
                else
                {
                    transform.FindChild("SetObject").gameObject.SetActive(false);
                    for (int i = 0; i < setDire.Count; i++)
                    {
                        setDire[i].GetComponent<SpriteRenderer>().enabled = true;
                        setDire[i].GetComponent<EventTrigger>().enabled = true;
                        setDire[i].transform.position = setPos;
                    }
                    g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
                    g.transform.position = setPos;
                    g.GetComponent<SpriteRenderer>().sprite = s;
                }
                panelDire = 0;
            }
            else
            {
                s = panels[generateNo].GetComponent<SpriteRenderer>().sprite;
                setPos = Vector2.zero;
                /*g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
                g.transform.position = setPos;*/
                transform.FindChild("SetObject").gameObject.SetActive(true);
                //SetPosition();
            }
            //g.GetComponent<SpriteRenderer>().sprite = s;
            //child.GetComponent<Image>().sprite = s;
            /*}
        }
        else
        {
            child.gameObject.SetActive(false);
        }*/
            g.transform.eulerAngles = new Vector3(0, 0, 90 * panelDire);
        }
        GameObject selecting = transform.FindChild("CommandList").FindChild("Selecting").gameObject;
        if (!selecting.activeSelf)
        {
            selecting.SetActive(true);
        }
        selecting.transform.localPosition = new Vector2(-500 + 250 * (panelCount % 5), 0);
        this.panelDire = panelDire;
    }

    public void SetPosition(bool twice/*二回目か*/)//ロボ、パネルの生成位置決定
    {
        Vector3 touch_pos = Input.mousePosition;
        Vector2 t_pos = Camera.main.ScreenToWorldPoint(touch_pos);
        t_pos.x = Mathf.Round(t_pos.x);
        t_pos.y = Mathf.Round(t_pos.y/* + 1.5f*/);
        Collider2D[] col = Physics2D.OverlapPointAll(t_pos);
        Collider2D area = null;
        foreach (Collider2D c in col)
        {
            if (c.tag == "Kernel" || (isRobot && c.tag == "Panel"))
            {
                return;
            }
            if(c.tag=="Area"&&c.GetComponent<AreaController>().Mikata)
            {
                area = c;
            }
        }
        if(isRobot&&area==null)
        {
            return;
        }
        //g.transform.position = t_pos;
        if (twice && t_pos == setPosSub)
        {
            setPos = t_pos;
            Debug.Log("setpos" + setPos);
            Generate(0);
        }
        else
        {
            setPosSub = t_pos;
            Debug.Log("setposSub" + setPos);
        }
    }

    public void Generate(int setDire=-1)
    {
        Debug.Log("Ok" + generateNo);
        if (isRobot)
        {
            Vector3 genPos;
            if (robots[generateNo].GetComponent<RobotController>().typeNo == (int)RobotType.Figurine)
            {
                genPos = setPos;
            }
            else
            {
                genPos = kerCon.transform.position;
            }
            kerCon.Generate(robots[generateNo], setDire, genPos, !gameStop);
        }
        else
        {
            Collider2D[] aCollider2d = Physics2D.OverlapPointAll(setPos);
            foreach (Collider2D c in aCollider2d)
            {
                if (c != null && c.tag == "Panel")//すでにパネルが存在していれば、上書き
                {
                    Destroy(c.gameObject);
                }
            }
            GameObject ob = (GameObject)Instantiate(panels[generateNo], setPos, transform.rotation);
            ob.GetComponent<PanelController>().direction = panelDire;
            GameObject ef = (GameObject)Instantiate(Resources.Load("Prefabs/effect_p"), Vector2.zero, transform.rotation);
            ef.name = "effect";
            ef.transform.position = ob.transform.position;
            ef.transform.SetParent(ob.transform);
            //transform.FindChild("SetObject").gameObject.SetActive(true);
        }
    }

    public void ChangeTab()
    {
        foreach (Transform child in transform.FindChild("CommandList"))
        {
            if (isRobot)
            {
                if (child.tag == "UI_Menu_Robo")
                {
                    child.gameObject.SetActive(false);
                }
                else if (child.tag == "UI_Menu_Pan")
                {
                    child.gameObject.SetActive(true);
                }
                for (int i = 0; i < setDire.Count; i++)
                {
                    setDire[i].GetComponent<SpriteRenderer>().enabled = false;
                    setDire[i].GetComponent<EventTrigger>().enabled = false;
                }
            }
            else
            {
                if (child.tag == "UI_Menu_Robo")
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.tag == "UI_Menu_Pan")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        isRobot = !isRobot;
        g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        tabText.text=isRobot ? "Robot" : "Panel";
        transform.FindChild("SetObject").gameObject.SetActive(false);
        transform.FindChild("CommandList").FindChild("Selecting").gameObject.SetActive(false);
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

    public void OnMenu()
    {
        g.GetComponent<SpriteRenderer>().sprite = null;
        for (int i = 0; i < setDire.Count; i++)
        {
            setDire[i].GetComponent<EventTrigger>().enabled = false;
            setDire[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
        isOnMenu = !isOnMenu;
        foreach (Transform child in transform)
        {
            if (child.name == "CommandList")
            {
                child.gameObject.SetActive(isOnMenu);
            }
            else if(child.name == "MenuSwitch")
            {
                child.FindChild("Text").GetComponent<Text>().text = isOnMenu ? "OFF" : "ON";
            }
        }
    }

    public void TimeHandle()
    {
        SetPause(gameStop);
        gameStop = !gameStop;
        Text t = transform.FindChild("TimeSwitch").FindChild("Text").GetComponent<Text>();
        if (gameStop)
        {
            t.text = "▼";
            t.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            t.text = "||";
            t.transform.eulerAngles = Vector3.zero;
        }
    }

    public void ReturnTitle()
    {
        if (transform.FindChild("EndMessage").GetComponent<Image>().enabled)
        {
            SceneManager.LoadScene("title");
        }
    }
}