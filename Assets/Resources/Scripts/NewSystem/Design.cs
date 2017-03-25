using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Design : MonoBehaviour
{
    [SerializeField]
    GameObject bOrigin, comModeGO, winsGO;
    [SerializeField]
    Transform[] status, component;
    [SerializeField]
    InputField nameField;
    [SerializeField]
    Sprite defSprite, selSprite;
    [SerializeField]
    Scrollbar scrollbar;
    [SerializeField]
    CommandSetter comSetter;
    List<Button> menuButtons;
    const float height = 100;
    List<Button> matButtons;
    bool isOnRobot;
    bool isOnFade;
    int selNo;//選択中レシピ番号
    int pSelNo;
    float menPosY;
    float fadeSp;
    float fadeTime;
    float fadeCo;

    // Use this for initialization
    void Start()
    {
        menuButtons = new List<Button>();
        matButtons = new List<Button>();
        isOnRobot = true;
        InitiateMenu();
        menPosY = 1f;
        fadeSp = 180f;
        fadeTime = 1080 / fadeSp;
        fadeCo = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!winsGO.activeSelf&& !comModeGO.activeSelf)
        {
            winsGO.SetActive(true);
            isOnFade = true;
        }
        else if(isOnFade)
        {
            FadeMenu();
        }
    }

    void InitiateMenu()
    {
        for (int i = 0; i < UserData.instance.robotRecipe.Count; i++)
        {
            AddMenu(UserData.instance.robotRecipe[i]);
        }
    }

    void AddMenu(Recipe r)
    {
        if (isOnRobot)
        {
            menuButtons.Add(Instantiate(bOrigin,
                transform.FindChild("Wins").FindChild("WinR")).GetComponent<Button>());
            SetSelNo(menuButtons.Count - 1);
            menuButtons[selNo].transform
                .FindChild("Text").GetComponent<Text>().text = r.Name;
            UpdateMenu();
        }
    }

    void UpdateMenu()
    {
        scrollbar.size = 5 < menuButtons.Count ? 0.5f : 1f;
        int scrLim = 5 < menuButtons.Count ? menuButtons.Count - 5 : 0;
        float posY = 190f + height * (1 - menPosY) * scrLim;
        Debug.Log(posY);
        for (int i = 0; i < menuButtons.Count; i++)
        {
            menuButtons[i].GetComponent<RectTransform>().anchoredPosition
                = new Vector2(-30, posY - height * i);
            menuButtons[i].transform.localScale = Vector3.one;
            menuButtons[i].GetComponent<Image>().sprite = defSprite;
            int no = i;
            menuButtons[i].onClick.RemoveAllListeners();
            menuButtons[i].onClick.AddListener(() => SetSelNo(no));
        }
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (0 <= pSelNo && pSelNo < menuButtons.Count)
        {
            menuButtons[pSelNo].GetComponent<Image>().sprite = defSprite;
        }
        if (0 <= selNo && selNo < menuButtons.Count)
        {
            menuButtons[selNo].GetComponent<Image>().sprite = selSprite;
        }
    }

    void SetStatus()
    {
        Recipe recipe = UserData.instance.robotRecipe[selNo];
        if (isOnRobot)
        {
            Robot r = UserData.instance.robotRecipe[selNo];
            component[0].FindChild("Text").GetComponent<Text>().text
                = r.head.Name.ToString();
            component[1].FindChild("Text").GetComponent<Text>().text
                = r.body.Name.ToString();
            component[2].FindChild("Text").GetComponent<Text>().text
                = r.arm.Name.ToString();
            component[3].FindChild("Text").GetComponent<Text>().text
                = r.leg.Name.ToString();
        }
        Debug.Log(recipe.Name);
        nameField.text = recipe.Name;
        status[0].FindChild("Status").GetComponent<Text>().text
            = "HP";
        status[0].FindChild("Param").GetComponent<Text>().text
            = recipe.HP.ToString();
        status[1].FindChild("Status").GetComponent<Text>().text
            = "LP";
        status[1].FindChild("Param").GetComponent<Text>().text
            = recipe.LP.ToString();
        status[2].FindChild("Status").GetComponent<Text>().text
            = "SP";
        status[2].FindChild("Param").GetComponent<Text>().text
            = recipe.SP.ToString();

    }

    void FadeMenu()
    {
        if (fadeCo == fadeTime)
        {
            if(0<fadeSp)//自分が消え、コマンドを呼び出すとき
            {
                comModeGO.SetActive(true);
                winsGO.SetActive(false);
                comSetter.UpdateRobot(UserData.instance.robotRecipe[selNo]);
            }
            else//自分を表示
            {
            }
            isOnFade = false;
            fadeCo = 0;
            fadeSp *= -1;
        }
        else
        {
            winsGO.GetComponent<RectTransform>().anchoredPosition += Vector2.down * fadeSp;
            fadeCo++;
        }
    }

    public void SetSelNo(int no)
    {
        pSelNo = selNo;
        selNo = no;
        Debug.Log(selNo);
        UpdateSprite();
        SetStatus();
    }

    public void NewRecipe()
    {
        if (isOnRobot)
        {
            UserData.instance.robotRecipe.Add(new Robot(UserData.instance.heads[0], UserData.instance.bodies[0],
                UserData.instance.arms[0], UserData.instance.legs[0]));
            AddMenu(UserData.instance.robotRecipe[UserData.instance.robotRecipe.Count - 1]);
        }
    }

    public void CopyRecipe()
    {
        if (isOnRobot)
        {
            UserData.instance.robotRecipe.Add(UserData.instance.robotRecipe[selNo]);
            AddMenu(UserData.instance.robotRecipe[UserData.instance.robotRecipe.Count - 1]);
        }
    }

    public void DeleteRecipe()
    {
        if (isOnRobot && 1 < menuButtons.Count)
        {
            GameObject g = menuButtons[selNo].gameObject;
            menuButtons.RemoveAt(selNo);
            Destroy(g);
            UserData.instance.robotRecipe.RemoveAt(selNo);
            pSelNo = selNo == 0 ? 0 : selNo - 1;
            SetSelNo(pSelNo);
        }
    }

    public void Scroll()
    {
        /*int d = 0;
        int viewMenCo = 5;
        if (up && viewMenCo <= menuButtons.Count - menPosNo)//up
        {
            d = 1;
        }
        else if (!up && 1 <= menPosNo)//down
        {
            d = -1;
        }
        menPosNo += d;
        scrollCo = 0;*/
        menPosY = scrollbar.value;
        UpdateMenu();
    }

    public void SetName()
    {
        Debug.Log(nameField.text);
        if (isOnRobot)
        {
            UserData.instance.robotRecipe[selNo].Name = nameField.text;
        }
        menuButtons[selNo].transform.FindChild("Text").GetComponent<Text>().text = nameField.text;
    }

    public void CallComMode()
    {
        if(isOnRobot)
        {
            isOnFade = true;
        }
    }
}
