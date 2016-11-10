using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面に付与
/// </summary>
public class DataManager : MonoBehaviour {

    static public DataManager dataInstance;

    public List<GameObject> panels;//各ステージでゲットしたパネル
    public List<int> stageList;//解放されているステージのリスト
    public int[] combos;//各ステージのコンボ数
    public int[] panelCounts;//各ステージのコンボ数
    public string[] stageNames;
    public string[] descriptions;//ステージ説明
    public bool[] hasTutorial;
    public int level;//ロボの規定レベル
    public bool onTutorial;

    void Awake()
    {
        if (dataInstance == null)
        {
            dataInstance = this;
            DontDestroyOnLoad(gameObject);
            combos = new int[SceneManager.sceneCountInBuildSettings - 2];
            panelCounts = new int[combos.Length];
            stageNames = new string[combos.Length];
            stageNames[0] = "異空間-第1階";
            stageNames[1] = "異空間-第2階";
            stageNames[2] = "異空間-第3階";
            descriptions = new string[combos.Length];
            descriptions[0] = "　摩天楼の心臓へ挑む前に、\nちょっと話を聞いていかないかい？";
            descriptions[1] = "　焦るな、摩天楼が\n逃げることはない";
            descriptions[2] = "　摩天楼へ向かう君に、\n最後のテクニックを伝授しよう";
            hasTutorial = new bool[combos.Length];
            hasTutorial[0] = true;
            hasTutorial[1] = true;
            hasTutorial[2] = true;
            level = 1;
            stageList = new List<int>();
            stageList.Add(2);
            stageList.Add(3);
            stageList.Add(4);
            stageList.Add(5);
            stageList.Add(6);
            stageList.Add(7);
            stageList.Add(8);
            stageList.Add(9);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start()
    {

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
