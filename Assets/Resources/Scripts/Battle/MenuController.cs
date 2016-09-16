using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{

    //public GameObject[] robots;//生成するロボ
    public GameObject[] panels;//生成するパネル
    List<Button> br;
    List<Button> bp;
    int generateNo;
    bool isRobot;
    bool isOnMenu;
    bool gameStop;
    bool isSetting;//ロボ、パネルをセットする状態か
    int panelDire = 0;//パネルの向き、基本値は0
    public int eCount = 0;//敵の数　0になったらゲームクリア
    GameObject g;//オブジェの配置予想
    public GameObject setDire;//方向オブジェ
    #region 各種コントローラの参照
    public KernelController kerCon/*, kerConEnemy*/;
    public TerritoryController terCon;
    public RobotController sRobo;//選択中のロボ
    #endregion
    public Sprite loseImage, winImage;
    Vector2 setPos, setPosSub;//subで押下時の座標を取り、setposがそれと一致したときにパネル生成
    public Text tabText;
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

    // Use this for initialization
    void Start()
    {
        #region 各コマンド初期化
        Transform commandList = transform.FindChild("CommandList");
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
            br[i].transform.SetParent(commandList, true);
            br[i].transform.localPosition = new Vector2(-500 + 150 * (i % 5), 0);
            br[i].transform.localScale = Vector3.one;
            RobotController r = robots[i].GetComponent<RobotController>();
            int SIZE = 32;
            Texture2D image = new Texture2D(SIZE, SIZE, TextureFormat.ARGB32, false);
            Color[] c;
            if (r.is3d)
            {
                c = r.image_all.texture.GetPixels(
                SIZE * (1 + (r.im_num % 4) * 3),
                SIZE * (7 - 4 * (r.im_num / 4) - r.dire),
                SIZE, SIZE);
            }
            else
            {
                c = robots[i].GetComponent<SpriteRenderer>().sprite.texture.GetPixels(
                   SIZE * (1 + (r.im_num % 4) * 3),
                   SIZE * (7 - 4 * (r.im_num / 4) - r.dire),
                   SIZE, SIZE);
            }
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
        #endregion
        g = GameObject.Find("ObjectExpectation");
        kerCon = GameObject.Find("Kernel").GetComponent<KernelController>();
        terCon = GameObject.Find("Territory").GetComponent<TerritoryController>();
        setDire.SetActive(false);
        ChangeTab();
        OnMenu();
        LimitScroll(mapSizeX, mapSizeY, false);
    }

    // Update is called once per frame
    void Update()
    {
        #region 終了処理
        if (kerCon == null || eCount == 0)
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
        #endregion
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
        if (sRobo != null && sRobo.gameObject != null)//ステータス表示中、ロボを追従
        {
            RectTransform canvasRect = GetComponent<RectTransform>();
            /*Vector2 viewportPosition = camera.WorldToViewportPoint(sRobo.transform.position);
            Vector2 worldObject_ScreenPosition = new Vector2(
                ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));*/
            transform.FindChild("SelectingRobot").GetComponent<RectTransform>().anchoredPosition
                = SetToScreenPos(sRobo.transform.position);
        }
        if (sRobo != null)//ロボが選択されているとき、ステータスを表示
        {
            SetStatus(sRobo);
        }
        if (velocity != Vector2.zero)//余韻スクロール
        {
            camera.transform.position = (Vector2)camera.transform.position + velocity;
            camera.transform.position += new Vector3(0, 0, -10);
            LimitScroll(mapSizeX, mapSizeY, false);
            velocity += accel;
        }
    }

    //ロボ・パネルの種類と生成する番号をセット
    public void SetNumber(int generateNo, bool isRobot, int panelDire, int panelCount)
    {
        this.generateNo = generateNo;
        this.isRobot = isRobot;
        foreach (Transform child in transform)
        {
            Sprite s;
            if (isRobot)
            {
                s = br[generateNo].GetComponent<Image>().sprite;
                setPos = GameObject.Find("Kernel").transform.position;
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
            }
            else
            {
                s = panels[generateNo].GetComponent<SpriteRenderer>().sprite;
                setPos = Vector2.zero;
                isSetting = true;
            }
            g.transform.eulerAngles = new Vector3(0, 0, 90 * panelDire);
        }
        GameObject selecting = transform.FindChild("CommandList").FindChild("Selecting").gameObject;
        if (!selecting.activeSelf)
        {
            selecting.SetActive(true);
        }
        selecting.transform.localPosition = new Vector2(-500 + 250 * (panelCount % 5), 0);
        this.panelDire = panelDire;
        SetStatus(kerCon.genRobots[generateNo].GetComponent<RobotController>());
    }

    public void SetPosition(bool twice/*二回目か*/)//ロボ、パネルの生成位置決定
    {
        if (!isSetting)
        {
            return;
        }
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

    public void Generate(int setDire = -1)
    {
        if (isRobot)
        {
            Vector3 genPos;
            genPos = kerCon.genRobots[generateNo].GetComponent<RobotController>().typeNo == (int)RobotType.Figurine
                ? setPos : (Vector2)kerCon.transform.position;
            kerCon.genNo = generateNo;
            kerCon.Generate(genPos, !gameStop, setDire);
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
            ob.transform.position += new Vector3(0, 0, -5);
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
                /*setDire.GetComponent<Image>().enabled = false;
                setDire.GetComponent<EventTrigger>().enabled = false;*/
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
        setDire.SetActive(false);
        isOnMenu = !isOnMenu;
        transform.FindChild("CommandList").gameObject.SetActive(isOnMenu);
        transform.FindChild("MenuSwitch").FindChild("Text").GetComponent<Text>().text
            = isOnMenu ? "OFF" : "ON";
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
        transform.FindChild("CommandList").gameObject.SetActive(!gameStop);
        transform.FindChild("PauseMenu").gameObject.SetActive(gameStop);
    }

    public void SetStatus(RobotController robot)
    {
        GameObject status = transform.FindChild("Status").gameObject;
        GameObject select = transform.FindChild("SelectingRobot").gameObject;
        if (robot == null)//ロボが選択されていなければ終了
        {
            status.SetActive(false);
            select.SetActive(false);
            return;
        }
        status.SetActive(true);
        select.SetActive(sRobo != null);
        string statusHP, statusATK, statusDEF, statusSPD;
        status.transform.FindChild("hp").GetComponent<Text>().text
            = robot.hp.ToString() + "/" + robot.mhp.ToString();
        status.transform.FindChild("atk").GetComponent<Text>().text = robot.attack.ToString();
        status.transform.FindChild("def").GetComponent<Text>().text = robot.defence.ToString();
        status.transform.FindChild("spd").GetComponent<Text>().text = robot.speed.ToString();
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
        Debug.Log("srobo");
        float bure = 5;
        Vector2 touchPos = Input.mousePosition;
        if (keyDownPos.x - bure < touchPos.x && touchPos.x < keyDownPos.x + bure
            && keyDownPos.y - bure < touchPos.y && touchPos.y < keyDownPos.y + bure)
        {

            Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] aCollider2d = Physics2D.OverlapPointAll(aTapPoint);
            sRobo = null;
            foreach (Collider2D col in aCollider2d)
            {
                if (col && col.tag == "Robot")
                {
                    sRobo = col.GetComponent<RobotController>();
                }
            }
        }
        SetStatus(sRobo);
        LimitScroll(mapSizeX, mapSizeY);
    }

    public void ToTitle()//タイトルへ戻る処理
    {
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(0));
    }

    public void Retry()
    {
        StartCoroutine(
            GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(
                SceneManager.GetActiveScene().buildIndex));
    }

    void LimitScroll(int sizeX, int sizeY, bool bound = true)
    {
        float correctionX = -0.5f;
        float speed = 0.1f;
        float marginX = 1;
        float marginY = 1;
        float rangeX = Mathf.Floor((sizeX - cameraSizeX) / 2) + marginX;
        float rangeY = Mathf.Floor((sizeY - cameraSizeY) / 2) + marginY;
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

    Vector2 SetToScreenPos(Vector2 pos)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector2 viewportPosition = camera.WorldToViewportPoint(pos);
        Vector2 worldObject_ScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        return worldObject_ScreenPosition;
    }

    public void WriteMessage(string text, float x = 300, float y = 400,
        float width = 1200, float height = 220)
    {
        Transform messageBox = transform.FindChild("MessageBox");
        messageBox.gameObject.SetActive(true);
        RectTransform rect = messageBox.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(x, y,0);
        rect.sizeDelta = new Vector2(width, height);
        messageBox.FindChild("Text").GetComponent<Text>().text = text;
    }

    public void CloseMessage()
    {
        Transform messageBox = transform.FindChild("MessageBox");
        messageBox.gameObject.SetActive(false);
    }
}