using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// タイトル画面に付与
/// </summary>
public class DataManager : MonoBehaviour {

    public List<GameObject> panels;//各ステージでゲットしたパネル
    public int[] scores;//各ステージのスコア

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Save()
    {

    }

    void Load()
    {

    }
}
