using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    [SerializeField]
    GameObject stageSelect;
    [SerializeField]
    GameObject stgButtons;
    [SerializeField]
    AudioClip decisionSE;
    bool stageOn;

    // Use this for initialization
    void Start()
    {
        int counter = 0;
        foreach(Transform stgButton in stgButtons.transform)
        {
            int c = counter;
            stgButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(c));
            counter++;
            string nm = stgButton.name;
            /*stgButton.FindChild("BldgNo").GetComponent<Image>().sprite
                = Resources.LoadAll<Sprite>("Sprites/Title/numberTEST_s")[(nm[0] - '0' + 9) % 10];
            stgButton.FindChild("AreaNo").GetComponent<Image>().sprite
                = Resources.LoadAll<Sprite>("Sprites/Title/numberTEST_s")[(nm[2] - '0' + 9) % 10];*/
            SetImage(nm, stgButton.FindChild("Image").gameObject);
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
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
    }

    public void OnBack()
    {
        stageOn = false;
        GetComponent<Animator>().SetTrigger("ReturnTitle");
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
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

    public void LoadLevel(int stageNo)
    {
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(
            DataManager.dataInstance.stageList[stageNo]));
        DataManager.dataInstance.onTutorial = DataManager.dataInstance.hasTutorial[stageNo];
        //    && tutoToggle.isOn;
    }

    public void SelectStage()
    {
        transform.FindChild("StageButtons").gameObject.SetActive(stageOn);
        transform.FindChild("Back").gameObject.SetActive(stageOn);
        Color c = stageOn ? Color.white * 0.5f : Color.white;
        transform.FindChild("StageSelect").GetComponent<Image>().color = c;
        transform.FindChild("StageSelect").GetComponent<Button>().enabled = !stageOn;
        transform.FindChild("Exit").gameObject.SetActive(!stageOn);
        transform.FindChild("Image").gameObject.SetActive(!stageOn);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
