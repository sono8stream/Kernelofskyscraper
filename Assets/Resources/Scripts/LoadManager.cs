using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator LoadScene(int index)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;    // シーン遷移をしない
        GameObject canvas = transform.FindChild("Canvas").gameObject;
        canvas.SetActive(true);
        
        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        canvas.SetActive(false);
        async.allowSceneActivation = true;    // シーン遷移許可
    }
}
