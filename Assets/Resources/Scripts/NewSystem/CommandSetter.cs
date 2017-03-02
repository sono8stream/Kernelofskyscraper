using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandSetter : MonoBehaviour
{
    Robot robot;
    [SerializeField]
    RobotController rCon;
    [SerializeField]
    GameObject cButtonOrigin;
    List<Button> panelButtons;
    List<int> panelNo;
    int choiceNo;
    Transform comMenu;
    [SerializeField]
    GameObject mButtonOrigin;
    float comMenuH;//コマンドメニュー高さ
    const float comButtonH = 90;
    bool isOnFade;
    float fadeSp;
    float fadeTime;

    void Awake()
    {
        robot = UserData.robotRecipe[0];
        Debug.Log(rCon);
        rCon.Robot = robot;
        panelButtons = new List<Button>();
        panelNo = new List<int>();
        InitiatepanelButtons();
        choiceNo = -1;
    }

    // Use this for initialization
    void Start()
    {
        fadeSp = 4f;
        comMenu = transform.FindChild("WinL").FindChild("ComMenu");
        comMenuH = robot.Command.Count * comButtonH + 30;
        fadeSp = comMenuH / fadeSp;
        InitiateMenuButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnFade)
        {
            SetMenu();
        }
    }

    void InitiatepanelButtons()
    {
        int masu = 150;//ボタン位置基準
        int cX = 0, cY = 0;//ComList内の中心座標
        for (int x = 0; x < robot.head.ComList.GetLength(1); x++)
        {
            for (int y = 0; y < robot.head.ComList.GetLength(0); y++)
            {
                if (robot.head.ComList[y, x] == -2)
                {
                    cX = x;
                    cY = y;
                    break;
                }
            }
        }
        for (int x = 0; x < robot.head.ComList.GetLength(1); x++)
        {
            for (int y = 0; y < robot.head.ComList.GetLength(0); y++)
            {
                if (0 <= robot.head.ComList[y, x])
                {
                    Button button = Instantiate(cButtonOrigin,
                        transform.FindChild("WinL").FindChild("PanelMenu"))
                        .GetComponent<Button>();
                    panelButtons.Add(button);
                    panelNo.Add(y * robot.head.ComList.GetLength(0) + x);
                    button.GetComponent<RectTransform>().anchoredPosition
                        = new Vector3(cX - x, -(cY - y), 0) * masu;
                    button.GetComponent<RectTransform>().localScale = Vector3.one;
                    Transform icon = button.transform.FindChild("Icon");
                    icon.GetComponent<Image>().sprite
                        = robot.Command[robot.head.ComList[y, x]].sprite;
                    icon.GetComponent<RectTransform>().eulerAngles
                        = robot.Command[robot.head.ComList[y, x]].angle;
                    int no = panelButtons.Count - 1;
                    button.onClick.AddListener(() => SwitchMenu(no));
                }
            }
        }
    }

    void InitiateMenuButtons()
    {
        for(int i=0;i<robot.Command.Count;i++)
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
        }
    }

    public void SwitchMenu(int buttonNo)
    {
        if (!isOnFade)
        {
            if (0 <= buttonNo)
            {
                isOnFade = true;
                if (0<fadeSp)
                {
                    choiceNo = buttonNo;
                    float cor
                        = 0 < panelButtons[buttonNo].GetComponent<RectTransform>()
                        .anchoredPosition.x ? -250 : 250;
                    comMenu.GetComponent<RectTransform>().anchoredPosition
                        = panelButtons[buttonNo].GetComponent<RectTransform>()
                        .anchoredPosition + new Vector2(cor, 0);
                }
            }
            else
            {
                isOnFade = fadeSp < 0;
            }
        }
    }

    void SetMenu()
    {
        RectTransform rectT = comMenu.GetComponent<RectTransform>();
        if ((rectT.sizeDelta.y < comMenuH && 0 < fadeSp)
            || (0 < rectT.sizeDelta.y && fadeSp < 0))
        {
            rectT.sizeDelta += Vector2.up * fadeSp;
        }
        else
        {
            fadeSp *= -1;
            isOnFade = false;
        }
    }

    void SetPanel(int comNo)
    {
        robot.head.ComList[panelNo[choiceNo] / robot.head.ComList.GetLength(0),
            panelNo[choiceNo] % robot.head.ComList.GetLength(0)] = comNo;
        Transform icon = panelButtons[choiceNo].transform.FindChild("Icon");
        icon.GetComponent<Image>().sprite = robot.Command[comNo].sprite;
        icon.GetComponent<RectTransform>().eulerAngles 
            = robot.Command[comNo].angle;
        SwitchMenu(choiceNo);
    }
}
