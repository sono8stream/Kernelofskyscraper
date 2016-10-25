using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    [SerializeField]
    GameObject stageSelect;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
        {
            EndGame();
        }
    }

    public void OnClick()
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public void SelectStage()
    {
        gameObject.SetActive(false);
        stageSelect.SetActive(true);
    }

    public void EndGame()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.Quit();
            return;
        }
    }
}
