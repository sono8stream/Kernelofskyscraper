using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class ButtonController : MonoBehaviour {

    [SerializeField]
    GameObject stageSelect;
    [SerializeField]
    GameObject stgButtons;
    [SerializeField]
    AudioClip decisionSE;
    bool stageOn;
    int selectedStage = 0;

    // Use this for initialization
    void Start()
    {
        int counter = 0;
        foreach(Transform stgButton in stgButtons.transform)
        {
            int c = counter;
            stgButton.GetComponent<Button>().onClick.AddListener(() => SetStageNo(c));
            string nm = stgButton.name;
            /*stgButton.FindChild("BldgNo").GetComponent<Image>().sprite
                = Resources.LoadAll<Sprite>("Sprites/Title/numberTEST_s")[(nm[0] - '0' + 9) % 10];
            stgButton.FindChild("AreaNo").GetComponent<Image>().sprite
                = Resources.LoadAll<Sprite>("Sprites/Title/numberTEST_s")[(nm[2] - '0' + 9) % 10];*/
            SetImage(nm, stgButton.FindChild("Image").gameObject);
            counter++;
            if (DataManager.dataInstance.stageCount < counter)
            {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
        {
            EndGame();
        }
    }

    void SetImage(string nm, GameObject g)
    {
        int w = 40;
        int h = 60;
        int mergin = 15;
        Color[] c;
        Texture2D t = new Texture2D(w * 3 + mergin * 2, h, TextureFormat.RGBA32, false);
        Sprite s = Resources.Load<Sprite>("Sprites/Title/numberTEST_s");
        Sprite nullImage= Resources.Load<Sprite>("Sprites/nullImage");
        c = s.texture.GetPixels((nm[0] - '0' + 9) % 10 * w, 0, w, h);
        t.SetPixels(0, 0, w, h, c);
        c = s.texture.GetPixels(10 * w, 0, w, h);
        t.SetPixels(w + mergin, 0, w, h, c);
        c = s.texture.GetPixels((nm[2] - '0' + 9) % 10 * w, 0, w, h);
        t.SetPixels((w + mergin) * 2, 0, w, h, c);
        c = nullImage.texture.GetPixels(0, 0, mergin, h);
        t.SetPixels(w, 0, mergin, h, c);
        t.SetPixels(w*2 + mergin, 0, mergin, h, c);
        t.Apply();
        g.GetComponent<Image>().sprite
            = Sprite.Create(t, new Rect(0, 0, w * 3 + mergin * 2, h), new Vector2(0.5f, 0.5f), 100);
    }

    public void OnClick()
    {
        stageOn = true;
        GetComponent<Animator>().SetTrigger("ChangeSelect");
        //stgButtons.transform./*GetChild(selectedStage)*/FindChild("0-1").GetComponent<Button>().Select();
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void OnBack()
    {
        stageOn = false;
        GetComponent<Animator>().SetTrigger("ReturnTitle");
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
        transform.FindChild("Info").gameObject.SetActive(false);
    }

    public void OnEnd()
    {
        transform.FindChild("PopUp").gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("OnPopUp");
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void OnPopUpN()
    {
        GetComponent<Animator>().SetTrigger("OffPopUp");
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void DisablePopUp()
    {
        transform.FindChild("PopUp").gameObject.SetActive(false);
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void SetStageNo(int stageNo)
    {
        selectedStage = stageNo;
        transform.FindChild("Info").gameObject.SetActive(true);
        SetDescription();
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void LoadLevel()
    {
        Debug.Log(selectedStage);
        Toggle tutoToggle = transform.FindChild("Info").FindChild("onTutorial").GetComponent<Toggle>();
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(
            DataManager.dataInstance.stageList[selectedStage]));
        DataManager.dataInstance.onTutorial = DataManager.dataInstance.hasTutorial[selectedStage]
        && tutoToggle.isOn;
    }

    public void SelectStage()
    {
        transform.FindChild("StageButtons").gameObject.SetActive(stageOn);
        transform.FindChild("Back").gameObject.SetActive(stageOn);
        Color c = stageOn ? Color.white * 0.5f : Color.white;
        transform.FindChild("StageSelect").GetComponent<Image>().color = c;
        transform.FindChild("StageSelect").GetComponent<Button>().enabled = !stageOn;
        transform.FindChild("Exit").gameObject.SetActive(!stageOn);
    }

    public void SetDescription()
    {
        Debug.Log(selectedStage);
        
        Transform info = transform.FindChild("Info");
        info.gameObject.SetActive(true);
        info.FindChild("Level").GetComponent<Text>().text
            = "Kernel Level. " + DataManager.dataInstance.level.ToString();
        info.FindChild("Stage Name").GetComponent<Text>().text
            = DataManager.dataInstance.stageNames[selectedStage];
        info.FindChild("Description").GetComponent<Text>().text
            = DataManager.dataInstance.descriptions[selectedStage];//ステージ情報をセット
        info.FindChild("onTutorial").gameObject.SetActive(
            DataManager.dataInstance.hasTutorial[selectedStage]);
        int comboCount = DataManager.dataInstance.combos[selectedStage];
        int panelCount = DataManager.dataInstance.panelCounts[selectedStage];
        Text t = info.transform.FindChild("Score").transform.FindChild("Text").GetComponent<Text>();
        Text p = info.transform.FindChild("Score").transform.FindChild("Point").GetComponent<Text>();
        if (DataManager.dataInstance.rank[selectedStage]==-1)
        {
            t.text = "";
            p.alignment = TextAnchor.MiddleCenter;
            p.text = "Not Cleared!";
        }
        else
        {
            t.text = "High Score\n   Combo\n   Panels\nGrade";
            p.alignment = TextAnchor.UpperRight;
            p.text = (comboCount * 1000 - panelCount * 200).ToString()
            + "\n" + comboCount.ToString()
            + "\n" + panelCount.ToString()
            + "\n" + (Rank)Enum.ToObject(typeof(Rank), DataManager.dataInstance.rank[selectedStage]);
        }
    }

    public void EndGame()
    {
        DataManager.dataInstance.Save();
        Application.Quit();
    }
}
