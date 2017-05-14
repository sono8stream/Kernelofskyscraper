using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMenu : MonoBehaviour {

    [SerializeField]
    Image selectImage;
    [SerializeField]
    CameraSwiper swiper;
    [SerializeField]
    float panelX1, panelSpace;
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

    public void InitiateCommandBs()
    {
        commandBs = new List<Button>();
        for (int i = 0; i < UserData.instance.gotComs.Length; i++)
        {
            if (UserData.instance.gotComs[i])
            {
                Button b = Instantiate(panelGOrigin, transform).GetComponent<Button>();
                int x = i;
                b.onClick.AddListener(() => SetPanelNo(x));
                Transform iconT = b.transform.FindChild("Icon");
                iconT.GetComponent<Image>().sprite = Data.commands[i].sprite;
                iconT.eulerAngles = Data.commands[i].angle;
                b.transform.FindChild("Count").GetComponent<Text>().text = Data.commands[i].cost.ToString();
                RectTransform rt = b.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(panelX1 + i % 2 * panelSpace, -82 - i / 2 * 120);
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
        selectImage.sprite = Data.commands[panelNo].sprite;
        selectImage.transform.eulerAngles = Data.commands[panelNo].angle;
        selectImage.enabled = true;
        swiper.onPanel = true;
    }
}
