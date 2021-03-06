﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    #region property
    //public GameObject[] robots;//生成するロボ
    public GameObject[] panels;//生成するパネル
    GameObject[] settedPanels;
    List<Button> br;
    List<Button> bp;
    int generateNo;
    [SerializeField]
    bool isRobot;
    public int GenNo
    {
        get { return generateNo; }
    }
    public bool IsRobot
    {
        get { return isRobot; }
    }
    bool isOnMenu;
    bool gameStop;
    bool isSetting;//ロボ、パネルをセットする状態か
    int panelDire = 0;//パネルの向き、基本値は0
    public int eCount = 0;//敵の数　0になったらゲームクリア
    public int eCountMax = 0;
    public int myRobotCount = 0;//味方ロボットの生成数
    GameObject g;//オブジェの配置予想
    public GameObject setDire;//方向オブジェ
    #region 各種コントローラの参照
    public KernelController kerCon/*, kerConEnemy*/;
    public TerritoryController terCon;
    public GameObject sObject;//選択中のオブジェクト
    #endregion
    public Sprite loseImage, winImage;
    Vector2 setPos, setPosSub;//subで押下時の座標を取り、setposがそれと一致したときにパネル生成
    public Text tabText;
    List<GameObject> cautionCursors;
    bool windowClosing;
    #region カメラ関連
    public Camera camera;
    public int mapSizeX, mapSizeY;//マップの大きさ
    const int cameraSizeX = 16;
    const int cameraSizeY = 9;
    Vector2 keyDownPos;
    Vector2 velocity;
    Vector2 accel;
    bool cameraIsFixing;//カメラ固定状態
    #endregion
    #region サウンド
    [SerializeField]
    AudioClip result, decisionSE, setPanelSE, delPanelSE;
    public AudioClip destSE;
    #endregion
    public int comboCount;
    public int comboCountMax;
    public int providedScore;
    [SerializeField]
    int comboWait;//コンボを持続する時間、短いほど難しい
    int comboWaitCount;
    [SerializeField]
    GameObject[] items;//マップ上のアイテム
    bool[] gettedItems;
    #endregion

    // Use this for initialization
    void Start()
    {
        try
        {
            DataManager.dataInstance = GameObject.Find("Loading").GetComponent<DataManager>();
            if (panels.Length == 0)
            {
                panels = DataManager.dataInstance.panels.ToArray();
            }
            else//パネル保存
            {
                foreach (GameObject panel in panels)
                {
                    bool exist = false;
                    foreach (GameObject dataP in DataManager.dataInstance.panels)
                    {
                        if (dataP == panel)
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        DataManager.dataInstance.panels.Add(panel);
                    }
                }
            }
        }
        catch { }
        g = GameObject.Find("ObjectExpectation");
        foreach(GameObject k in GameObject.FindGameObjectsWithTag("Kernel"))
        {
            if(k.GetComponent<KernelController>().mikata)
            {
                kerCon
                    = k.GetComponent<KernelController>();
                break;
            }

        }
        terCon = GameObject.Find("Territory").GetComponent<TerritoryController>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        #region 各コマンド初期化
        Transform robotList = transform.FindChild("RobotList");
        GameObject bt;
        EventTrigger trigger;
        EventTrigger.Entry entryDown, entryDrag, entryEndDrag;
        br = new List<Button>();//ロボコマンド初期化
        GameObject[] robots = kerCon.genRobots;
        for (int i = 0; i < robots.GetLength(0); i++)
        {
            bt = (GameObject)Instantiate(Resources.Load("Prefabs/RoboButton"),
                transform.position, transform.rotation);//新たなロボボタン 
            br.Add(bt.GetComponent<Button>());
            br[i].transform.SetParent(robotList, true);
            br[i].transform.localPosition = new Vector2(-400 + 160 * (i % 5), 0);
            br[i].transform.localScale = Vector3.one;
            RobotController r = robots[i].GetComponent<RobotController>();
            int SIZE = 32;
            Color[] c;
            Sprite iconSprite;
            if (r.is3d)
            {
                iconSprite = r.image_all;
            }
            else
            {
                Texture2D image = new Texture2D(SIZE, SIZE, TextureFormat.ARGB32, false);
                c = robots[i].GetComponent<SpriteRenderer>().sprite.texture.GetPixels(
                   SIZE * (1 + (r.im_num % 4) * 3),
                   SIZE * (7 - 4 * (r.im_num / 4) - r.dire),
                   SIZE, SIZE);
                image.SetPixels(0, 0, SIZE, SIZE, c);
                image.Apply();
                iconSprite = Sprite.Create(image, new Rect(0, 0, SIZE, SIZE),
                new Vector2(0.5f, 0.5f), SIZE);
            }
            br[i].GetComponent<Image>().sprite = iconSprite;
            br[i].transform.FindChild("Text").gameObject.SetActive(false);
            //br[i].onClick.AddListener(() => SetNumber(i, true, 0));
            trigger = br[i].GetComponent<EventTrigger>();
            entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            int genNo = i;
            entryDown.callback.AddListener((x) => SetNumber(genNo, true, 0, genNo));
            trigger.triggers.Add(entryDown);
        }
        Transform panelList = transform.FindChild("PanelList");
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
                bp[panelCount].transform.SetParent(panelList, true);
                bp[panelCount].GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(0, -100 - 160 * i);
                bp[panelCount].transform.localScale
                    = panels[i].transform.FindChild("Icon").localScale;
                bp[panelCount].transform.localEulerAngles
                    = panels[i].transform.FindChild("Icon").localEulerAngles;
                bp[panelCount].GetComponent<Image>().sprite 
                    = panels[i].transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite;
                bp[panelCount].transform.FindChild("Text").gameObject.SetActive(false);
                //bp[panelCount].onClick.AddListener(() => SetNumber(i, false, j));
                trigger = bp[panelCount].GetComponent<EventTrigger>();
                entryDown = new EventTrigger.Entry();
                entryDown.eventID = EventTriggerType.PointerDown;
                int genNo = i, dire = j, panelNo = panelCount;
                entryDown.callback.AddListener((x) => SetNumber(genNo, false, dire, panelNo));
                entryDown.callback.AddListener((x) => TouchDownScreen());
                entryDrag= new EventTrigger.Entry();
                entryDrag.eventID = EventTriggerType.Drag;
                entryDrag.callback.AddListener((x) => TouchingPanelList());
                trigger.triggers.Add(entryDown);
                trigger.triggers.Add(entryDrag);
                panelCount++;
            }
        }
        #endregion
        settedPanels = new GameObject[panels.Length];
        setDire.SetActive(false);
        //ChangeTab();
        //OnMenu();
        LimitScroll(mapSizeX, mapSizeY, false);
        cautionCursors = new List<GameObject>();
        comboCount = 0;
        comboCountMax = 0;
        comboWaitCount = 0;
        gettedItems = new bool[items.Length];
        SetCombo();
        SetGetItem();
    }

    // Update is called once per frame
    void Update()
    {
        #region 終了処理
        if ((kerCon == null || eCount == 0) && !gameStop)
        {
            SetScore();
            transform.FindChild("Result").gameObject.SetActive(true);
            /*if (kerCon == null)//敗北時
            {

            }
            else//勝利時
            {
            }
            if (!gameStop)
            {*/
            TimeHandle();
            //isOnMenu = true;
            //OnMenu();
            transform.FindChild("Selecting").gameObject.SetActive(false);
            transform.FindChild("SetDirection1").gameObject.SetActive(false);
            //}
        }
        #endregion
        /*#region 中断処理
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
        #endregion*/
        if (sObject != null)//ステータス表示中、ロボを追従
        {
            RectTransform canvasRect = GetComponent<RectTransform>();
            transform.FindChild("SelectingRobot").GetComponent<RectTransform>().anchoredPosition
                = SetToScreenPos(sObject.transform.position);
        }
        /*if (sRobo != null)//ロボが選択されているとき、ステータスを表示
        {
            SetStatus(sRobo);
        }*/
        if (velocity != Vector2.zero)//余韻スクロール
        {
            camera.transform.position = (Vector2)camera.transform.position + velocity;
            camera.transform.position += new Vector3(0, 0, -10);
            LimitScroll(mapSizeX, mapSizeY, false);
            velocity += accel;
        }
        if (windowClosing)//ウィンドウ終了後に非アクティブに
        {
            Transform messageBox = transform.FindChild("MessageBox");
            if (messageBox.GetComponent<RectTransform>().localScale.x <= 0.2)
            {
                messageBox.gameObject.SetActive(false);
                windowClosing = false;
            }
        }
    }

    //ロボ・パネルの種類と生成する番号をセット
    public void SetNumber(int generateNo, bool isRobot, int panelDire, int panelCount)
    {
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
        this.generateNo = generateNo;
        this.isRobot = isRobot;
        GameObject selecting = transform.FindChild("Selecting").gameObject;
        if (!selecting.activeSelf)
        {
            selecting.SetActive(true);
        }
        Sprite s;
        if (isRobot)
        {
            s = br[generateNo].GetComponent<Image>().sprite;
            setPos = kerCon.transform.position;
            if (kerCon.genRobots[generateNo].GetComponent<RobotController>().typeNo == (int)RobotType.Figurine)
            {
                isSetting = true;
                setDire.SetActive(false);
            }
            else
            {
                isSetting = false;
                setDire.SetActive(true);
                setDire.GetComponent<RectTransform>().anchoredPosition = SetToScreenPos(setPos + Vector2.down);
                g.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
                g.transform.position = setPos;
                g.GetComponent<SpriteRenderer>().sprite = s;
            }
            panelDire = 0;
            selecting.transform.localPosition
                = new Vector2(-190 + 160 * (panelCount % 5), -430);
            SetStatus(kerCon.genRobots[generateNo]);
        }
        else
        {
            setDire.SetActive(false);
            s = panels[generateNo].GetComponent<SpriteRenderer>().sprite;
            setPos = Vector2.zero;
            isSetting = true;
            /*selecting.transform.localPosition
                = new Vector2(840, 400 - 160 * (panelCount % 5));*/
            selecting.transform.position = bp[generateNo].transform.position;
            SetStatus(panels[generateNo]);
        }
        g.transform.eulerAngles = new Vector3(0, 0, 90 * panelDire);
        this.panelDire = panelDire;
        this.isRobot = isRobot;
    }

    public void SetPosition(bool twice/*二回目か*/)//ロボ、パネルの生成位置決定
    {
        Vector3 touch_pos = Input.mousePosition;
        Vector2 t_pos = Camera.main.ScreenToWorldPoint(touch_pos);
        t_pos.x = Mathf.Round(t_pos.x);
        t_pos.y = Mathf.Round(t_pos.y/* + 1.5f*/);
        if (isSetting)
        {
            Collider2D[] col = Physics2D.OverlapPointAll(t_pos);
            Collider2D area = null;
            foreach (Collider2D c in col)
            {
                if (c.tag == "Kernel" || (isRobot && c.tag == "Panel"))
                {
                    return;
                }
                if (c.tag == "Area" && c.GetComponent<AreaController>().Mikata)
                {
                    area = c;
                }
            }
            if (isRobot && area == null)
            {
                return;
            }
            if (twice && t_pos == setPosSub)
            {
                setPos = t_pos;
                Generate(0);
            }
            else
            {
                setPosSub = t_pos;
            }
        }
        else if (isRobot)
        {
            Debug.Log("Calling?");
        }
    }

    public void Generate(int setDire = -1)
    {
        Debug.Log("Generate");
        if (isRobot)
        {
            Vector3 genPos;
            genPos = kerCon.genRobots[generateNo].GetComponent<RobotController>().typeNo == (int)RobotType.Figurine
                ? setPos : (Vector2)kerCon.transform.position;
            kerCon.genNo = generateNo;
            kerCon.Generate(genPos, !gameStop, setDire);
        }
        else if (myRobotCount == 0)
        {
            Collider2D[] aCollider2d = Physics2D.OverlapPointAll(setPos);
            foreach (Collider2D c in aCollider2d)
            {
                if (c != null && c.tag == "Panel")//すでにパネルが存在していれば、戻る
                {
                    return;
                }
            }
            Collider[] aCollider = Physics.OverlapSphere(setPos, 0.4f);
            foreach (Collider c in aCollider)
            {
                if (c != null && c.tag == "Robot")//すでにパネルが存在していれば、戻る
                {
                    return;
                }
            }
            #region パネル1種1枚
            /*if (settedPanels[generateNo] != null)
            {
                sObject = settedPanels[generateNo];
                DestroyPanel();
            }*/
            #endregion
            settedPanels[generateNo]
                = (GameObject)Instantiate(panels[generateNo], setPos, transform.rotation);
            settedPanels[generateNo].transform.position += new Vector3(0, 0, 1);
            settedPanels[generateNo].GetComponent<PanelController>().mikata = true;
            GetComponent<AudioSource>().PlayOneShot(setPanelSE);
        }
    }

    /*
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
                setDire.SetActive(false);
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
        tabText.text = isRobot ? "Robot" : "Panel";
        isSetting = false;
        transform.FindChild("CommandList").FindChild("Selecting").gameObject.SetActive(false);
    }*/

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

    /*public void OnMenu()
    {
        g.GetComponent<SpriteRenderer>().sprite = null;
        setDire.SetActive(false);
        isOnMenu = !isOnMenu;
        transform.FindChild("RobotList").gameObject.SetActive(isOnMenu);
        transform.FindChild("PanelList").gameObject.SetActive(isOnMenu);
    }*/

    public void TimeHandle()
    {
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
        SetPause(gameStop);
        gameStop = !gameStop;
        /*Text t = transform.FindChild("TimeSwitch").FindChild("Text").GetComponent<Text>();
        if (gameStop)
        {
            t.text = "▼";
            t.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            t.text = "||";
            t.transform.eulerAngles = Vector3.zero;
        }*/
        Image i = transform.FindChild("TimeSwitch").GetComponent<Image>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Battle/timeswitch");
        if (gameStop)
        {
            i.sprite = sprites[0];
        }
        else
        {
            i.sprite = sprites[1];
        }
        transform.FindChild("RobotList").gameObject.SetActive(!gameStop);
        transform.FindChild("PanelList").gameObject.SetActive(!gameStop);
        transform.FindChild("Selecting").gameObject.SetActive(/*!gameStop*/false);
        transform.FindChild("Status").gameObject.SetActive(!gameStop);
        transform.FindChild("PauseMenu").gameObject.SetActive(gameStop);
    }

    public void SetStatus(GameObject target)
    {
        GameObject status = transform.FindChild("Status").gameObject;
        GameObject select = transform.FindChild("SelectingRobot").gameObject;
        if (target == null)//ロボが選択されていなければ終了
        {
            status.SetActive(false);
            select.SetActive(false);
            return;
        }
        status.SetActive(true);
        select.SetActive(sObject != null);
        string statusHP, statusATK, statusDEF, statusSPD;
        if (target.tag == "Robot")
        {
            RobotController robot = target.GetComponent<RobotController>();
            status.transform.FindChild("hp").GetComponent<Text>().text
                = robot.hp.ToString() + "/" + robot.mhpCurrent.ToString();
            status.transform.FindChild("atk").GetComponent<Text>().text = robot.attackCurrent.ToString();
            status.transform.FindChild("def").GetComponent<Text>().text = robot.defenceCurrent.ToString();
            status.transform.FindChild("spd").GetComponent<Text>().text = robot.speed.ToString();
            status.transform.FindChild("HP").GetComponent<Text>().text = "HP";
            status.transform.FindChild("Attack").GetComponent<Text>().text = "AT";
            status.transform.FindChild("Defence").GetComponent<Text>().text = "DF";
            status.transform.FindChild("Speed").GetComponent<Text>().text = "SP";
        }
        else if (target.tag == "Panel")
        {
            status.transform.FindChild("hp").GetComponent<Text>().text
                = target.GetComponent<PanelController>().description;
            status.transform.FindChild("atk").GetComponent<Text>().text = "";
            status.transform.FindChild("def").GetComponent<Text>().text = "";
            status.transform.FindChild("spd").GetComponent<Text>().text = "";
            status.transform.FindChild("HP").GetComponent<Text>().text = "";
            status.transform.FindChild("Attack").GetComponent<Text>().text = "";
            status.transform.FindChild("Defence").GetComponent<Text>().text = "";
            status.transform.FindChild("Speed").GetComponent<Text>().text = "";
        }
    }

    public void TouchDownScreen()
    {
        keyDownPos = Input.mousePosition;
    }

    public void TouchingScreen()
    {
        velocity = keyDownPos / 160 - (Vector2)Input.mousePosition / 160;
        accel = velocity / (-10);
        camera.transform.position = (Vector2)camera.transform.position + velocity;
        camera.transform.position += new Vector3(0, 0, -10);
        LimitScroll(mapSizeX, mapSizeY, false);
        keyDownPos = Input.mousePosition;
    }

    public void TouchUpScreen()
    {
        float bure = 5;
        Vector2 touchPos = Input.mousePosition;
        if (keyDownPos.x - bure < touchPos.x && touchPos.x < keyDownPos.x + bure
            && keyDownPos.y - bure < touchPos.y && touchPos.y < keyDownPos.y + bure)
        {

            Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            sObject = null;
            Collider2D[] aCollider2d = Physics2D.OverlapPointAll(aTapPoint);
            foreach (Collider2D col in aCollider2d)
            {
                if (col && (col.tag == "Robot" || col.tag == "Panel"))
                {
                    sObject = col.gameObject;
                }
            }
            aTapPoint = new Vector3(aTapPoint.x, aTapPoint.y, 0);
            Collider[] aCollider = Physics.OverlapSphere(aTapPoint, 0.4f);
            foreach (Collider col in aCollider)
            {
                if (col && col.tag == "Robot")
                {
                    sObject = col.gameObject;
                }
            }
            Debug.Log(aTapPoint);
        }
        SetStatus(sObject);
        LimitScroll(mapSizeX, mapSizeY);
    }

    public void TouchingPanelList()
    {
        float spY = (keyDownPos.y - Input.mousePosition.y);
        //accel = velocity / (-10);
        //camera.transform.position = (Vector2)camera.transform.position + velocity;
        Debug.Log(spY);
            foreach (Transform child in transform.FindChild("PanelList"))
            {
            child.GetComponent<RectTransform>().transform.localPosition += Vector3.down * spY;
        }
        //transform.FindChild("Selecting").localPosition += Vector3.down * spY;
        Transform selecting = transform.FindChild("Selecting");
        LimitScrollP();
        if (selecting.gameObject.activeSelf)
        {
            transform.FindChild("Selecting").position = bp[generateNo].transform.position;
        }
        keyDownPos = Input.mousePosition;
    }

    public void TouchUpPanelList()
    {
        float bure = 5;
        Vector2 touchPos = Input.mousePosition;
        if (keyDownPos.x - bure < touchPos.x && touchPos.x < keyDownPos.x + bure
            && keyDownPos.y - bure < touchPos.y && touchPos.y < keyDownPos.y + bure)
        {

            Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            sObject = null;
            Collider2D[] aCollider2d = Physics2D.OverlapPointAll(aTapPoint);
            foreach (Collider2D col in aCollider2d)
            {
                if (col && (col.tag == "Robot" || col.tag == "Panel"))
                {
                    sObject = col.gameObject;
                }
            }
            aTapPoint = new Vector3(aTapPoint.x, aTapPoint.y, 0);
            Collider[] aCollider = Physics.OverlapSphere(aTapPoint, 0.4f);
            foreach (Collider col in aCollider)
            {
                if (col && col.tag == "Robot")
                {
                    sObject = col.gameObject;
                }
            }
            Debug.Log(aTapPoint);
        }
        SetStatus(sObject);
        LimitScroll(mapSizeX, mapSizeY);
    }

    public void ToTitle()//タイトルへ戻る処理
    {
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(1));
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void Retry()
    {
        StartCoroutine(
            GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(
                SceneManager.GetActiveScene().buildIndex));
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    //カメラのスクロール限界
    void LimitScroll(int sizeX, int sizeY, bool bound = true)
    {
        float correctionX = -0.5f;
        float speed = 0.1f;
        float marginX = 1;
        float marginY = 1;
        float rangeX = Mathf.Floor((sizeX - cameraSizeX) / 2) + marginX;
        float rangeY = Mathf.Round((sizeY - cameraSizeY) / 2) + marginY;
        float correctionRight = 4;
        float correctionDown = 2;
        if (camera.transform.position.x < correctionX - rangeX)
        {
            camera.transform.position = new Vector3(correctionX - rangeX, camera.transform.position.y, -10);
            if (bound)
            {
                velocity.x = speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.position.x > correctionX + rangeX + correctionRight)
        {
            camera.transform.position = new Vector3(correctionX + rangeX + correctionRight, camera.transform.position.y, -10);
            if (bound)
            {
                velocity.x = -speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.position.y < -(rangeY + correctionDown))
        {
            camera.transform.position = new Vector3(camera.transform.position.x, -(rangeY + correctionDown), -10);
            if (bound)
            {
                velocity.y = speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.position.y > rangeY)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, rangeY, -10);
            if (bound)
            {
                velocity.y = -speed;
            }
            accel = velocity / (-10);
        }
        setDire.GetComponent<RectTransform>().anchoredPosition = SetToScreenPos(setPos + Vector2.down);
    }

    //パネルリストのスクロール限界
    void LimitScrollP()
    {
        if ((bp.Count < 6 && -100 < bp[0].GetComponent<RectTransform>().anchoredPosition.y)
            || (6 <= bp.Count && bp[4].GetComponent<RectTransform>().anchoredPosition.y < -740))
        {
            for (int i = 0; i < bp.Count; i++)
            {
                bp[i].GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(0, -100 - 160 * i);
            }
        }
        else if ((bp.Count < 6 && bp[bp.Count - 1].GetComponent<RectTransform>().anchoredPosition.y < -740)
            || (6 <= bp.Count && -100 < bp[bp.Count - 5].GetComponent<RectTransform>().anchoredPosition.y))
        {
            for (int i = 0; i < bp.Count; i++)
            {
                bp[i].GetComponent<RectTransform>().anchoredPosition
                    = new Vector2(0, -100 - 160 * (i + 5 - bp.Count));
            }
        }
    }

    Vector2 SetToScreenPos(Vector2 pos)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector2 viewportPosition = camera.WorldToViewportPoint(pos);
        Vector2 worldObject_ScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        return worldObject_ScreenPosition;
    }

    public void WriteMessage(string text, bool on, Color textColor, bool wait = true, float x = 300, float y = 400,
        float width = 1200, float height = 220)
    {
        Transform messageBox = transform.FindChild("MessageBox");
        messageBox.gameObject.SetActive(true);
        RectTransform rect = messageBox.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(x, y, 0);
        rect.sizeDelta = new Vector2(width, height);
        messageBox.FindChild("Text").GetComponent<Text>().text = text;
        messageBox.FindChild("Text").GetComponent<Text>().color = textColor;
        if (on)
        {
            messageBox.GetComponent<Animator>().SetTrigger("On");
        }
        messageBox.FindChild("Next").gameObject.SetActive(wait);
    }

    public void CloseMessage()
    {
        Transform messageBox = transform.FindChild("MessageBox");
        messageBox.GetComponent<Animator>().SetTrigger("Off");
        windowClosing = true;
    }

    public void SetCautionCursor(float x, float y)//注目位置にカーソル表示
    {
        cautionCursors.Add(Instantiate(transform.FindChild("SelectingRobot").gameObject));
        cautionCursors[cautionCursors.Count - 1].transform.SetParent(transform);
        cautionCursors[cautionCursors.Count - 1].SetActive(true);
        cautionCursors[cautionCursors.Count - 1].GetComponent<RectTransform>().position
            = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector3(x, y, 0));
        cautionCursors[cautionCursors.Count - 1].GetComponent<RectTransform>().localScale
            = Vector3.one;
    }

    public void RemoveCautionCursor(int index = 0)
    {
        GameObject sub = cautionCursors[index];
        cautionCursors.RemoveAt(index);
        Destroy(sub);
    }

    void SetScore()
    {
        if (eCount == 0)
        {
            if (comboCountMax < comboCount)
            {
                comboCountMax = comboCount;
                SetCombo();
            }
            int panelCount = 0;
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Panel"))
            {
                if (g.GetComponent<PanelController>().mikata)
                {
                    panelCount++;
                }
            }
            int score = 1000 * comboCountMax - 200 * panelCount;
            Transform res = transform.FindChild("Result");
            res.FindChild("ComboCount").GetComponent<Text>().text
                = "Combo Count   =   1000×" + comboCountMax.ToString();
            res.FindChild("PanelCount").GetComponent<Text>().text
                = "Panel Count     =  -200×" + panelCount.ToString();
            res.FindChild("TotalScore").GetComponent<Text>().text
                = "Total Score      =   " + score.ToString();
            #region レベルアップ
            if (DataManager.dataInstance != null)
            {
                /*int currentScore
                    = DataManager.dataInstance.combos[SceneManager.GetActiveScene().buildIndex - 2] * 1000
                    - DataManager.dataInstance.panelCounts[SceneManager.GetActiveScene().buildIndex - 2] * 200;*///現在のスコア
                //ランク計算
                int sceneNo = SceneManager.GetActiveScene().buildIndex - 2;
                int currentRank = DataManager.dataInstance.rank[sceneNo];
                int rank = Mathf.FloorToInt(score / (eCountMax * 200));
                if (/*currentScore < score*/currentRank < rank)
                {
                    if(currentRank==-1)
                    {
                        DataManager.dataInstance.stageCount++;
                    }
                    //int currentLevel = DataManager.dataInstance.level;//現在レベル
                    DataManager.dataInstance.rank[sceneNo] = rank;
                    DataManager.dataInstance.level += rank-currentRank;
                    DataManager.dataInstance.combos[sceneNo]
                        = comboCountMax;
                    DataManager.dataInstance.panelCounts[sceneNo] = panelCount;
                    Debug.Log(rank + "and" + DataManager.dataInstance.level);
                    res.FindChild("LevelUp").GetComponent<Text>().text
                    = "Level Up !!" /*+ (rank - currentRank).ToString()*/;
                }
                string rankText = "";
                switch (rank)
                {
                    case 0:
                        rankText = "E";
                        break;
                    case 1:
                        rankText = "D";
                        break;
                    case 2:
                        rankText = "C";
                        break;
                    case 3:
                        rankText = "B";
                        break;
                    case 4:
                        rankText = "A";
                        break;
                    case 5:
                        rankText = "S";
                        break;

                }
                res.FindChild("Rank").GetComponent<Text>().text ="Grade "+rankText;
            }
            #endregion
            Transform item = res.FindChild("Item");
            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (gettedItems[i])
                {
                    GameObject itButton = (GameObject)Instantiate(Resources.Load("Prefabs/PanButton"));//新たなパネル
                    itButton.transform.SetParent(item);
                    itButton.transform.localPosition = new Vector2(-170 * count, 0);
                    itButton.GetComponent<Image>().sprite = items[i].GetComponent<SpriteRenderer>().sprite;
                    itButton.transform.localEulerAngles
                        = items[i].transform.FindChild("Icon").localEulerAngles;
                    itButton.transform.localScale
                        = items[i].transform.FindChild("Icon").localScale;
                    itButton.GetComponent<Image>().sprite
                        = items[i].transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite;
                    itButton.transform.localScale *= 0.6f;
                    count++;
                    bool exist = false;
                    foreach (GameObject g in DataManager.dataInstance.panels)
                    {
                        if (g == items[i])
                        {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist)
                    {
                        DataManager.dataInstance.panels.Add(items[i]);
                    }
                }
            }
            /*if (count == 0)
            {
                GameObject itButton = (GameObject)Instantiate(Resources.Load("Prefabs/PanButton"));//新たなパネル
                itButton.transform.SetParent(item);
                itButton.transform.localPosition = new Vector2(0, 0);
                itButton.transform.localScale = Vector3.one * 0.6f;
            }*/
            res.FindChild("Get").gameObject.SetActive(0 < count);
            res.FindChild("Item").gameObject.SetActive(0 < count);
        }
        else
        {
            Transform res = transform.FindChild("Result");
            res.FindChild("WinMessage").GetComponent<Text>().text = "Cancelled Explore";
            res.FindChild("Message").GetComponent<Text>().text = "You failed...";
        }
        GetComponent<AudioSource>().clip = result;
        GetComponent<AudioSource>().Play();
    }

    public void SetCombo()
    {
        Transform comboCounter = transform.FindChild("ComboCounter");
        comboCounter.FindChild("ComboCount").GetComponent<Text>().text
            = comboCount.ToString();
        comboCounter.FindChild("MaxCount").GetComponent<Text>().text
            = comboCountMax.ToString();
        comboCounter.GetComponent<Animator>().SetTrigger("Update");
    }

    public void SetGetItem()
    {
        Transform comboCounter = transform.FindChild("ComboCounter");
        int itemCount = 0;
        foreach (bool f in gettedItems)
        {
            if (f)
            {
                itemCount++;
            }
        }
        comboCounter.FindChild("ItemCount").GetComponent<Text>().text
            = itemCount.ToString();
        comboCounter.GetComponent<Animator>().SetTrigger("GetItem");
    }

    public void DestroyPanel()
    {
        if (myRobotCount == 0 && sObject != null && sObject.tag == "Panel"
            &&sObject.GetComponent<PanelController>().mikata)
        {
            for (int i = 0; i < settedPanels.Length; i++)
            {
                if (settedPanels[i] == sObject)
                {
                    settedPanels[i] = null;
                    break;
                }
            }
            sObject.GetComponent<Animator>().SetTrigger("PanelBreak");
            GetComponent<AudioSource>().PlayOneShot(delPanelSE);
            SetStatus(null);
        }
    }

    public void GetItem(int no)
    {
        gettedItems[no] = true;
        SetGetItem();
    }
}

public enum Rank { E = 0, D, C, B, A, S }