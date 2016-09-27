using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }

    public void OnClick(int levelNo)
    {
        StartCoroutine(GameObject.Find("Loading").GetComponent<LoadManager>().LoadScene(levelNo));
    }
}
