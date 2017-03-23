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
    List<Button> menuButtons;
    const float height = 100;
    List<Button> matButtons;
    bool isOnRobot;
    int selNo;//選択中レシピ番号
    int pSelNo;
    float menPosY;

    // Use this for initialization
    void Start()
    {
        menuButtons = new List<Button>();
        matButtons = new List<Button>();
        isOnRobot = true;
        InitiateMenu();
        menPosY = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!winsGO.activeSelf&& !comModeGO.activeSelf)
        {
            winsGO.SetActive(true);
        }
    }

    void InitiateMenu()
    {
        for (int i = 0; i < UserData.robotRecipe.Count; i++)
        {
            AddMenu(UserData.robotRecipe[i]);
        }
    }

    void AddMenu(Recipe r)
    {
        if (isOnRobot)
        {
            menuButtons.Add(Instantiate(bOrigin,
                transform.FindChild("WinR")).GetComponent<Button>());
            SetSelNo(menuButtons.Count - 1);
            menuButtons[selNo].transform
                .FindChild("Text").GetComponent<Text>().text = r.Name;
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
        if (0 <= selNo && selNo < menuButtons.Count)
        {
            menuButtons[selNo].GetComponent<Image>().sprite = selSprite;
        }
    }

    void SetStatus()
    {
        Recipe recipe = UserData.robotRecipe[selNo];
        if (isOnRobot)
        {
            Robot r = UserData.robotRecipe[selNo];
            component[0].FindChild("Text").GetComponent<Text>().text
                = r.head.Name.ToString();
            component[1].FindChild("Text").GetComponent<Text>().text
                = r.body.Name.ToString();
            component[2].FindChild("Text").GetComponent<Text>().text
                = r.arm.Name.ToString();
            component[3].FindChild("Text").GetComponent<Text>().text
                = r.leg.Name.ToString();
        }
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

    public void SetSelNo(int no)
    {
        pSelNo = selNo;
        selNo = no;
        Debug.Log(no);
        UpdateMenu();
        SetStatus();
    }

    public void NewRecipe()
    {
        if (isOnRobot)
        {
            UserData.robotRecipe.Add(UserData.robotRecipe[0]);
            AddMenu(UserData.robotRecipe[UserData.robotRecipe.Count - 1]);
        }
    }

    public void CopyRecipe()
    {
        if (isOnRobot)
        {
            UserData.robotRecipe.Add(UserData.robotRecipe[selNo]);
            AddMenu(UserData.robotRecipe[UserData.robotRecipe.Count - 1]);
        }
    }

    public void DeleteRecipe()
    {
        if (isOnRobot && 1 < menuButtons.Count)
        {
            GameObject g = menuButtons[selNo].gameObject;
            menuButtons.RemoveAt(selNo);
            Destroy(g);
            UserData.robotRecipe.RemoveAt(selNo);
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
        if (isOnRobot)
        {
            UserData.robotRecipe[selNo].Name = nameField.text;
        }
        menuButtons[selNo].transform.FindChild("Text").GetComponent<Text>().text
            = nameField.text;
    }

    public void CallComMode()
    {
        if(isOnRobot)
        {
            comModeGO.SetActive(true);
            winsGO.SetActive(false);
        }
    }
}
