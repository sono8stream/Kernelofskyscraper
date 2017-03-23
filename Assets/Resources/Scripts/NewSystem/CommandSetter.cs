using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CommandSetter : MonoBehaviour
{
    Robot robot;
    [SerializeField]
    RobotController rCon;
    [SerializeField]
    GameObject cButtonOrigin, defaultButton, modeButton, returnButton, margin, comModeGO;
    [SerializeField]
    RectTransform lWindowRT, rWindowRT;
    [SerializeField]
    Camera roboC;
    List<Button> panelButtons;
    List<Button> menuButtons;
    List<int> panelNo;
    List<int> priNo;//優先順位
    List<int> priNoTemp;
    int choiceNo;
    int comListNo;
    Transform comMenu;
    [SerializeField]
    GameObject mButtonOrigin;
    float modeFadeCo;
    float modeFadeTime;
    float comMenuH;//コマンドメニュー高さ
    float menuFadeSp;
    float menuFadeTime;
    const float comButtonH = 90;
    bool isOnMode;//このモード(コマンドモード)on_off
    bool isOnMenu;//メニュー表示on_off
    bool isOnPriNo;//優先度選択モード

    void Awake()
    {
        UpdateRobot(UserData.robotRecipe[0]);
        comMenu = transform.FindChild("WinL").FindChild("ComMenu");
        Debug.Log(rCon);
    }

    // Use this for initialization
    void Start()
    {
        modeFadeCo = 0;
        modeFadeTime = 6f;
        menuFadeSp = 4f;
        menuFadeSp = comMenuH / menuFadeSp;
        isOnMode = true;
        comModeGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnMenu)
        {
            SetMenu();
        }
        if(isOnMode)
        {
            SetWindows();
        }
    }

    public void UpdateRobot(Robot robot)
    {
        this.robot = robot;
        rCon.Robot = robot;
        comListNo = 0;
        choiceNo = -1;
        isOnPriNo = false;
        priNoTemp = new List<int>();
        InitiatePanelButtons();
        comMenuH = robot.Command.Count * comButtonH + 30;
        InitiateMenuButtons();
    }

    void InitiatePanelButtons()
    {
        if(panelButtons!=null&&0<panelButtons.Count)
        {
            panelButtons.ForEach(x => Destroy(x.gameObject));
        }
        panelNo = new List<int>();
        priNo = new List<int>();
        panelButtons = new List<Button>();
        Vector2 cPos = rCon.GetComCPos();
        int masu = 150;//ボタン位置基準
        int cX = (int)cPos.x;//ComList内の中心座標
        int cY = (int)cPos.y;
        int counter = 0;
        Debug.Log(robot.head.Range);
        for (int y = 0; y < robot.head.Range; y++)
        {
            for (int x = 0; x < robot.head.Range; x++)
            {
                Debug.Log("OK??");
                if ((int)ComNo.Default <= robot.head.ComList[comListNo][y, x])
                {
                    Button button = Instantiate(cButtonOrigin,
                        transform.FindChild("WinL").FindChild("PanelMenu")).GetComponent<Button>();
                    panelButtons.Add(button);
                    panelNo.Add(y * robot.head.Range + x);
                    priNo.Add(counter);//優先順位指定
                    button.GetComponent<RectTransform>().anchoredPosition
                        = new Vector3(cX - x, -(cY - y), 0) * masu;
                    button.GetComponent<RectTransform>().localScale = Vector3.one;
                    Transform icon = button.transform.FindChild("Icon");
                    icon.GetComponent<Image>().sprite
                        = robot.Command[robot.head.ComList[comListNo][y, x]].sprite;
                    icon.GetComponent<RectTransform>().eulerAngles
                        = robot.Command[robot.head.ComList[comListNo][y, x]].angle;
                    Transform priNoText = button.transform.FindChild("PriNo");
                    priNoText.GetComponent<Text>().text = counter.ToString();
                    int no = counter;
                    button.onClick.AddListener(() => PanelClick(no));
                    counter++;
                }
            }
        }
    }

    void InitiateMenuButtons()
    {
        if (menuButtons != null && 0 < menuButtons.Count)
        {
            menuButtons.ForEach(x => Destroy(x.gameObject));
        }
        menuButtons = new List<Button>();
        for (int i = 0; i < robot.Command.Count; i++)
        {
            Button button
                = Instantiate(mButtonOrigin, comMenu).GetComponent<Button>();
            button.GetComponent<RectTransform>().anchoredPosition = Vector2.up
                * (robot.Command.Count * 0.5f - 0.5f - i) * comButtonH;
            button.GetComponent<RectTransform>().localScale = Vector2.one;
            button.transform.FindChild("Text").GetComponent<Text>().text
                = robot.Command[i].name;
            int no = i;
            button.onClick.AddListener(() => SetPanel(no));
            menuButtons.Add(button);
        }
    }

    void SetWindows()
    {
        if (modeFadeTime == modeFadeCo || modeFadeTime == -modeFadeCo)//modeFadeTimeの正負で移動方向を処理
        {
            modeFadeCo = 0;
            modeFadeTime *= -1;
            isOnMode = 0 < modeFadeTime;
            comModeGO.SetActive(modeFadeTime < 0);
        }
        else
        {
            lWindowRT.anchoredPosition += Vector2.right * lWindowRT.sizeDelta.x / modeFadeTime;
            rWindowRT.anchoredPosition += Vector2.left * rWindowRT.sizeDelta.x / modeFadeTime;
            roboC.rect = new Rect(roboC.rect.x - roboC.rect.width / modeFadeTime,
                roboC.rect.y, roboC.rect.width, roboC.rect.height);
            modeFadeCo++;
        }
    }

    public void ReturnMode()
    {
        if(!isOnMode)
        {
            isOnMode = true;
        }
    }

    public void PanelClick(int buttonNo)
    {
        Debug.Log("??");
        if (isOnPriNo)
        {
            SetPriNo(buttonNo);
        }
        else
        {
            SwitchMenu(buttonNo);
        }
    }

    public void SwitchMenu(int buttonNo)
    {
        if (!isOnMenu)
        {
            if (-1 <= buttonNo)
            {
                isOnMenu = true;
                if (0 < menuFadeSp)
                {
                    choiceNo = buttonNo;
                    Vector2 pos = 0 <= buttonNo ?
                        panelButtons[buttonNo].GetComponent<RectTransform>().anchoredPosition
                        : defaultButton.GetComponent<RectTransform>().anchoredPosition;
                    float cor = 0 < pos.x ? -250 : 250;
                    comMenu.GetComponent<RectTransform>().anchoredPosition
                        = pos + new Vector2(cor, 0);
                }
                menuButtons[0].interactable = buttonNo != -1;
                margin.SetActive(true);
            }
            else
            {
                isOnMenu = menuFadeSp < 0;
                margin.SetActive(false);
            }
        }
    }

    void SetMenu()
    {
        RectTransform rectT = comMenu.GetComponent<RectTransform>();
        if ((rectT.sizeDelta.y < comMenuH && 0 < menuFadeSp)
            || (0 < rectT.sizeDelta.y && menuFadeSp < 0))
        {
            rectT.sizeDelta += Vector2.up * menuFadeSp;
        }
        else
        {
            menuFadeSp *= -1;
            isOnMenu = false;
        }
    }

    void SetPanel(int comNo)
    {
        Transform icon;
        if (choiceNo == -1)
        {
            robot.head.DefaultComNo = comNo;
            icon = defaultButton.transform.FindChild("Icon");
        }
        else
        {
            robot.head.ComList[comListNo][panelNo[choiceNo] / robot.head.Range,
                panelNo[choiceNo] % robot.head.Range] = comNo;
            icon = panelButtons[choiceNo].transform.FindChild("Icon");
        }
        icon.GetComponent<Image>().sprite = robot.Command[comNo].sprite;
        icon.GetComponent<RectTransform>().eulerAngles
            = robot.Command[comNo].angle;
        SwitchMenu(choiceNo);
    }

    public void ChangeMode()
    {
        isOnPriNo = !isOnPriNo;
        foreach(Button b in panelButtons)
        {
            if (isOnPriNo)
            {
                b.transform.FindChild("Icon").SetSiblingIndex(0);
            }
            else
            {
                b.transform.FindChild("PriNo").SetSiblingIndex(0);
            }
        }
        modeButton.transform.FindChild("Text").GetComponent<Text>().text = isOnPriNo ? "Command" : "Priority";
    }

    /// <summary>
    /// 優先順位選択
    /// </summary>
    /// <param name="panelNo">コマンド番号</param>
    void SetPriNo(int buttonNo)
    {
        Text t = panelButtons[buttonNo].transform.FindChild("PriNo").GetComponent<Text>();
        int index = t.text[0] - '0';
        if (t.text[0] == '-')
        {
            robot.head.ComPriList[comListNo][priNoTemp[0]] = panelNo[buttonNo];
            t.text = priNoTemp[0].ToString();
            priNoTemp.RemoveAt(0);
            if(priNoTemp.Count==0)
            {
                modeButton.GetComponent<Button>().interactable = true;
                returnButton.GetComponent<Button>().interactable = true;
            }
        }
        else if (0 <= index && index < robot.head.ComPriList[comListNo].Length)
        {
            priNoTemp.Insert(GetSortedIndex(priNoTemp, index), index);
            t.text = "-";
            modeButton.GetComponent<Button>().interactable = false;
            returnButton.GetComponent<Button>().interactable = false;
        }
        string text = "";
        for (int i = 0; i < robot.head.ComPriList[comListNo].Length; i++)
        {
            text += robot.head.ComPriList[comListNo][i].ToString() + ",";
        }
        Debug.Log(text);
    }

    int GetSortedIndex(List<int> list, int value)
    {
        int index = 0;
        while (index < list.Count && list[index] < value)
        {
            index++;
        }
        return index;
    }
}
