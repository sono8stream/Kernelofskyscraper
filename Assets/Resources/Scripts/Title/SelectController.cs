using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SelectController : MonoBehaviour
{
    [SerializeField]
    GameObject[] floors;
    [SerializeField]
    GameObject floorOrigin;
    [SerializeField]
    GameObject title;
    [SerializeField]
    GameObject info;
    [SerializeField]
    Toggle tutoToggle;
    Vector2 keyDownPos;//キー押下位置
    int selectedStage;//選択中のステージ番号
    int waitLimit = 10;
    int waitCounter;//
    int iniX = 250;
    int iniY = -350;
    int dx = 200;
    int dy = 150;
    int limitFloor = 2;
    bool buttonClick;//ボタンタップ中か

    // Use this for initialization
    void Start()
    {
        floors = new GameObject[/*DataManager.dataInstance.stageList.Count*/3];
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i] = Instantiate(floorOrigin);
            int no = i;
            floors[i].GetComponent<Button>().onClick.AddListener(() => SetCenter(no));
            floors[i].transform.SetParent(transform.FindChild("Floors"));
            floors[i].transform.localScale = new Vector3(4, 2, 1);
            floors[i].transform.localPosition = new Vector2(iniX + dx * i, iniY + dy * i);
        }
        waitCounter = 0;
        selectedStage = 0;
        SetDescription();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TouchDownScreen();
        }
        else if(Input.GetMouseButton(0))
        {
            TouchingScreen();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            TouchUpScreen();
        }
    }

    public void TouchDownScreen()
    {
        keyDownPos = Input.mousePosition;
        buttonClick = true;
    }

    public void TouchingScreen()
    {
        Vector2 touchPos = Input.mousePosition;
        float bure = 30;
        if (bure < Mathf.Abs(touchPos.y - keyDownPos.y))
        {
            if (buttonClick)
            {
                buttonClick = false;
            }
            Vector3 vel = new Vector2(dx, dy) / waitLimit;
            if (bure * 2 < Mathf.Abs(touchPos.y - keyDownPos.y))
            {
                vel *= 2;
            }
            if (touchPos.y > keyDownPos.y)
            {
                vel *= -1;
            }
            if (floors[0].transform.localPosition.x <= iniX
                && (floors.Length < limitFloor || (limitFloor <= floors.Length
                && iniX + dx * limitFloor <= floors[floors.Length - 1].transform.localPosition.x)))
            {
                for (int i = 0; i < floors.Length; i++)
                {
                    floors[i].transform.position += vel;
                }
            }
        }
    }

    public void TouchUpScreen()
    {
        if (buttonClick)
        {
            buttonClick = false;
        }
        else
        {
            int xS = 0;
            xS = Mathf.RoundToInt((floors[0].transform.localPosition.x - iniX) / dx);
            if (limitFloor < floors.Length && xS + floors.Length - 1 < limitFloor)
            {
                xS = limitFloor - (floors.Length - 1);
            }
            if (0 < xS)
            {
                xS = 0;
            }
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].transform.localPosition
                    = new Vector2(iniX + dx * (xS + i), iniY + dy * (xS + i));
            }
            selectedStage = limitFloor < floors.Length ? limitFloor - xS : -xS;
            SetDescription();
        }
    }

    public void SetCenter(int no)
    {
        Debug.Log(no);
        if (limitFloor < no)
        {
            for (int i = 0; i < floors.Length; i++)
            {
                floors[i].transform.localPosition
                    = new Vector2(iniX + dx * (i + limitFloor - no), iniY + dy * (i + limitFloor - no));
            }
        }
        buttonClick = true;
        selectedStage = no;
        SetDescription();
    }

    public void SetDescription()
    {
        Debug.Log(selectedStage);
        info.transform.FindChild("Level").GetComponent<Text>().text
            = "Level. "+DataManager.dataInstance.level.ToString();
        info.transform.FindChild("Stage Name").GetComponent<Text>().text
            = DataManager.dataInstance.stageNames[selectedStage];
        info.transform.FindChild("Description").GetComponent<Text>().text
            = DataManager.dataInstance.descriptions[selectedStage];//ステージ情報をセット
        info.transform.FindChild("onTutorial").gameObject.SetActive(
            DataManager.dataInstance.hasTutorial[selectedStage]);
        int comboCount = DataManager.dataInstance.combos[selectedStage];
        int panelCount = DataManager.dataInstance.panelCounts[selectedStage];
        Text t= info.transform.FindChild("Score").transform.FindChild("Text").GetComponent<Text>();
        Text p = info.transform.FindChild("Score").transform.FindChild("Point").GetComponent<Text>();
        if (comboCount == 0 && panelCount == 0)
        {
            t.text = "";
            p.alignment = TextAnchor.MiddleCenter;
            p.text = "Not Cleared!";
        }
        else
        {
            t.text = "High Score\n   Combo\n   Panels";
            p.alignment = TextAnchor.UpperRight;
            p.text = (comboCount * 1000 - panelCount * 200).ToString()
            + "\n" + comboCount.ToString()
            + "\n" + panelCount.ToString();
        }
    }

    public void LoadLevel()
    {
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(
            DataManager.dataInstance.stageList[selectedStage]));
        DataManager.dataInstance.onTutorial = DataManager.dataInstance.hasTutorial[selectedStage]
            && tutoToggle.isOn;
    }

    public void SetReturn()
    {
        GetComponent<Animator>().SetTrigger("Return");
    }

    public void ReturnTitle()
    {
        GameObject.Find("StageSelect").SetActive(false);
        title.SetActive(true);
    }
}
