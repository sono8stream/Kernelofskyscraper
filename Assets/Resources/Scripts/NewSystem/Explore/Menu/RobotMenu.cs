using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMenu : MonoBehaviour
{
    public GameObject robotOrigin;

    [SerializeField]
    GameObject robotOriginE;
    [SerializeField]
    Image selectImage;
    [SerializeField]
    float buttonPosX;
    [SerializeField]
    CameraSwiper swiper;
    [SerializeField]
    Text commandText;
    GameObject panelGOrigin;
    List<Button> commandBs;
    int robotNo;
    public int RobotNo { get { return robotNo; } }
    RobotController roboRC;
    public RobotController RoboRC { get { return roboRC; } }

    List<byte> commandCodes;//Command code on RobotController
    bool onFlag;
    int codeIndex;
    int waitLim = 5;
    int waitCo;

    void Awake()
    {
        commandCodes = new List<byte>();
    }

    // Use this for initialization
    void Start()
    {
        panelGOrigin = Resources.Load<GameObject>("Prefabs/Custom/PanelButton");
        InitiateCommandBs();
        robotNo = -1;
        roboRC = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (roboRC != null)
        {
            SetRobotDirection();
            SetCommandCode();
        }
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
    }

    public void SetPanelNo(int no)
    {
        robotNo = no;
        selectImage.sprite = UserData.instance.robotRecipe[robotNo].icon;
        selectImage.transform.eulerAngles = Vector3.zero;
        selectImage.enabled = true;
        swiper.onPanel = false;
    }

    void SetRobotDirection()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            roboRC.dire = 2;
            roboRC.transform.FindChild("mod").eulerAngles = Vector3.forward * 180;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            roboRC.dire = 3;
            roboRC.transform.FindChild("mod").eulerAngles = Vector3.forward * 270;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            roboRC.dire = 0;
            roboRC.transform.FindChild("mod").eulerAngles = Vector3.forward * 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            roboRC.dire = 1;
            roboRC.transform.FindChild("mod").eulerAngles = Vector3.forward * 90;
        }
    }

    void SetCommandCode()
    {
        if (waitCo < waitLim)
        {
            waitCo++;
            return;
        }
        /*if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))//flag
        {*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetFlag();
            commandCodes.Insert(codeIndex, (int)CodeName.Wall);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            SetFlag();
            commandCodes.Insert(codeIndex, (int)CodeName.Enemy);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            SetFlag();
            commandCodes.Insert(codeIndex, (int)CodeName.Trap);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            SetCommand();
            commandCodes.Insert(codeIndex, (int)CodeName.Left);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("?");
            SetCommand();
            commandCodes.Insert(codeIndex, (int)CodeName.Right);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            SetCommand();
            commandCodes.Insert(codeIndex, (int)CodeName.Turn);
            codeIndex++;
            UpdateText();
        }
        /*}
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            codeIndex = 0 < codeIndex ? codeIndex - 1 : 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            codeIndex = codeIndex < commandCodes.Count ? codeIndex + 1 : commandCodes.Count;
        }*/
        else if (Input.GetKey(KeyCode.Backspace) && 0 < codeIndex)
        {
            DelCode();
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon))//終点
        {
            waitCo = 0;
            commandCodes.Insert(codeIndex, (int)CodeName.End);
            codeIndex++;
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            roboRC.SetAction(commandCodes.ToArray());
            roboRC.canMove = true;
            roboRC = null;
            onFlag = false;
            commandText.transform.parent.gameObject.SetActive(false);
            commandCodes = new List<byte>();
        }
    }

    void SetFlag()
    {
        waitCo = 0;
        codeIndex = commandCodes.Count;

        if (onFlag)
        {
            commandCodes.Insert(codeIndex, (int)CodeName.And);
            codeIndex++;
        }
        else
        {
            commandCodes.Insert(codeIndex, (int)CodeName.If);
            codeIndex++;
            onFlag = true;
        }
    }

    void SetCommand()
    {
        waitCo = 0;
        codeIndex = commandCodes.Count;

        if (onFlag)
        {
            while (0 < codeIndex && commandCodes[codeIndex - 1] != (int)CodeName.End)
            {
                codeIndex--;
                if (commandCodes[codeIndex] == (int)CodeName.If)
                {

                    break;
                }
            }
        }
        if (0 < codeIndex && commandCodes[codeIndex - 1] != (int)CodeName.End)
        {
            commandCodes.Insert(codeIndex, (int)CodeName.And);
            codeIndex++;
        }
    }

    void DelCode()
    {
        codeIndex = commandCodes.Count;
        commandCodes.RemoveAt(codeIndex - 1);
        codeIndex--;
        if (0 < codeIndex && (commandCodes[codeIndex - 1] == (int)CodeName.If
            || commandCodes[codeIndex - 1] == (int)CodeName.And))
        {
            if (commandCodes[codeIndex - 1] == (int)CodeName.If)
            {
                onFlag = false;
            }
            commandCodes.RemoveAt(codeIndex - 1);
            codeIndex--;
        }
    }

    void UpdateText()
    {
        string t = "";
        for (int i = 0; i < commandCodes.Count; i++)
        {
            t += roboRC.codeList[commandCodes[i]].text + " ";
        }
        commandText.text = t;
    }

    public void GenerateRobot(Transform cursorTransform, GameObject robotGO = null)
    {
        GameObject g;
        if (robotGO)
        {
            g = Instantiate(robotGO);
        }
        else
        {
            g = Instantiate(robotOrigin);
        }
        roboRC = g.GetComponent<RobotController>();
        roboRC.robot = (Robot)UserData.instance.robotRecipe[RobotNo].DeepCopy();
        roboRC.robot.Initiate();
        roboRC.floor = swiper.Floor;
        roboRC.specialCom = new Slash();

        g.transform.position = cursorTransform.position;
        g.transform.SetParent(cursorTransform.parent);
        g.transform.localScale = Vector3.one;

        commandText.transform.parent.gameObject.SetActive(true);
        UpdateText();
    }
}
