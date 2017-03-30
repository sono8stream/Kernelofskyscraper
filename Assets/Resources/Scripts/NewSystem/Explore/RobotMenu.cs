using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMenu : MonoBehaviour
{

    [SerializeField]
    Image selectImage;
    [SerializeField]
    float buttonPosX;
    GameObject panelGOrigin;
    List<Button> commandBs;
    int panelNo;
    public int PanelNo
    {
        get { return panelNo; }
    }

    void Awake()
    {
        DataController.InitiateData();
    }

    // Use this for initialization
    void Start()
    {
        panelGOrigin = Resources.Load<GameObject>("Prefabs/Custom/PanelButton");
        InitiateCommandBs();
        panelNo = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitiateCommandBs()
    {
        commandBs = new List<Button>();
        for (int i = 0; i < UserData.instance.robotRecipe.Count; i++)
        {
            if (UserData.instance.gotComs[i])
            {
                Button b = Instantiate(panelGOrigin, transform).GetComponent<Button>();
                int x = i;
                b.onClick.AddListener(() => SetPanelNo(x));
                Transform iconT = b.transform.FindChild("Icon");
                iconT.GetComponent<Image>().sprite = UserData.instance.robotRecipe[i].icon;
                iconT.eulerAngles = Data.commands[i].angle;
                RectTransform rt = b.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(buttonPosX + i * 120, -82 - i * 120);
                rt.localScale = Vector3.one;
                commandBs.Add(b);
            }
        }
        /*StaticMethodsCollection.ForList(UserData.instance.commands,
            (x) =>
            {
                Button b = Instantiate(panelGOrigin, transform).GetComponent<Button>();
                b.GetComponent<RectTransform>().anchoredPosition=new Vector2(-100+200*(|1)
                commandBs.Add(b);
            });*/
    }

    public void SetPanelNo(int no)
    {
        panelNo = no;
        selectImage.sprite = UserData.instance.robotRecipe[panelNo].icon;
        selectImage.transform.eulerAngles = Vector3.zero;
        selectImage.enabled = true;
    }
}
