﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour {

    bool fadeIn;
    int sceneIndex;
    [SerializeField]
    AudioClip decisionSE;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //transform.FindChild("Canvas").gameObject.SetActive(false);
        fadeIn = true;
        sceneIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn && sceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            fadeIn = false;
            sceneIndex = SceneManager.GetActiveScene().buildIndex;
            GetComponent<Animator>().SetTrigger("FadeOut");
        }
    }

    public IEnumerator LoadScene(int index)
    {
        if (sceneIndex == index)
        {
            sceneIndex = -1;
        }
        GetComponent<AudioSource>().PlayOneShot(decisionSE);
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;    // シーン遷移をしない
        transform.FindChild("Canvas").gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("FadeIn");

        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.7f);
        async.allowSceneActivation = true;    // シーン遷移許可
        fadeIn = true;
    }

    public void Disable()
    {
        transform.FindChild("Canvas").gameObject.SetActive(false);
        //GetComponent<Animator>().SetTrigger("FadeIn");
    }
}
