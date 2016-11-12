using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// タイトル画面に付与
/// </summary>
[System.Serializable]
public class DataManager : MonoBehaviour {

    static public DataManager dataInstance;

    public List<GameObject> panels;//各ステージでゲットしたパネル
    public GameObject[] allPanels;
    public bool[] gettedPanels;
    public List<int> stageList;//解放されているステージのリスト
    public int[] rank;//クリア時のランク
    public int[] combos;//各ステージのコンボ数
    public int[] panelCounts;//各ステージのコンボ数
    public string[] stageNames;
    public string[] descriptions;//ステージ説明
    public bool[] hasTutorial;
    public int level;//ロボの規定レベル
    public bool onTutorial;
    public int stageCount;

    void Awake()
    {
        if (dataInstance == null)
        {
            dataInstance = this;
            DontDestroyOnLoad(gameObject);
            gettedPanels = new bool[allPanels.Length];
            rank=new int[SceneManager.sceneCountInBuildSettings - 2];
            for(int i=0;i<rank.Length;i++)
            {
                rank[i] = -1;
            }
            combos = new int[rank.Length];
            panelCounts = new int[rank.Length];
            stageNames = new string[rank.Length];
            stageNames[0] = "異空間-第1階";
            stageNames[1] = "異空間-第2階";
            stageNames[2] = "異空間-第3階";
            stageNames[3] = "Kawazビル裏口";
            stageNames[4] = "Kawazビル2Fロビー";
            stageNames[5] = "Kawazビル3F廊下";
            stageNames[6] = "Kawazビル4Fオフィス";
            stageNames[7] = "Kawazビル5F展望台";
            descriptions = new string[rank.Length];
            descriptions[0] = "　摩天楼の心臓へ挑む前に、\nちょっと話を聞いていかないかい？";
            descriptions[1] = "　焦るな、摩天楼が\n逃げることはない";
            descriptions[2] = "　摩天楼へ向かう君に、\n最後のテクニックを伝授しよう";
            hasTutorial = new bool[rank.Length];
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
            stageCount = 0;
            Load();
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

    public void Save()
    {
        SaveData.instance = new SaveData();
        for(int i=0;i<allPanels.Length;i++)
        {
            for(int j=0;j<panels.Count;j++)
            {
                if(panels[j]==allPanels[i])
                {
                    gettedPanels[i] = true;
                    break;
                }
            }
        }
        SaveData.instance.gettedPanels = dataInstance.gettedPanels;
        SaveData.instance.rank = dataInstance.rank;
        SaveData.instance.combos = dataInstance.combos;
        SaveData.instance.panelCounts = dataInstance.panelCounts;
        SaveData.instance.level = dataInstance.level;
        SaveData.instance.stageCount = dataInstance.stageCount;
        SaveData.instance.Save();
        /*FileController.Save<SaveData>("data", SaveData.instance);
        string path = "data100";
        for (int i = 0; i < rank.Length; i++)
        {
            PlayerPrefs.SetInt(path, rank[i]);
            PlayerPrefs.SetInt(path, combos[i]);
            PlayerPrefs.SetInt(path, panelCounts[i]);
        }
        PlayerPrefs.SetInt(path, level);
        PlayerPrefs.SetInt(path, stageCount);
        PlayerPrefs.Save();*/
    }
    public void Load()
    {
        SaveData.instance = new SaveData();
        SaveData.instance.Load();
        if (SaveData.instance != null)
        {
            dataInstance.gettedPanels = SaveData.instance.gettedPanels;
            for (int i = 0; i < allPanels.Length; i++)
            {
                if (gettedPanels[i])
                {
                    dataInstance.panels.Add(allPanels[i]);
                }
            }
            dataInstance.rank = SaveData.instance.rank;
            dataInstance.combos = SaveData.instance.combos;
            dataInstance.panelCounts = SaveData.instance.panelCounts;
            dataInstance.level = SaveData.instance.level;
            dataInstance.stageCount = SaveData.instance.stageCount;
        }
        Save();
        string path = "data100";
        /*if (!PlayerPrefs.HasKey(path)) return;
        for (int i = 0; i < rank.Length; i++)
        {
            PlayerPrefs.GetInt(path, rank[i]);
            PlayerPrefs.GetInt(path, combos[i]);
            PlayerPrefs.GetInt(path, panelCounts[i]);
        }
        PlayerPrefs.GetInt(path, level);
        PlayerPrefs.GetInt(path, stageCount);*/
    }
}

[System.Serializable]
public class SaveData
{
    [System.NonSerialized]
    public static SaveData instance;
    public bool[] gettedPanels;//各ステージでゲットしたパネル
    public int[] rank;//クリア時のランク
    public int[] combos;//各ステージのコンボ数
    public int[] panelCounts;//各ステージのコンボ数
    public int level;
    public int stageCount;
    public string savePath;
    public SaveData()
    {
        savePath = Application.dataPath + "/save.bytes";
    }

    public void Save()
    {
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, SaveData.instance);
        }
        Debug.Log("save");
    }

    public void Load()
    {
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        instance = null;
        using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            instance = bf.Deserialize(fs) as SaveData;
        }
        Debug.Log("load");
    }
}

public class FileController : MonoBehaviour
{

    public FileController()
    {
    }

    public static bool Save<T>(string prefKey, T serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream();
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(memoryStream, serializableObject);

        string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
        try
        {
            PlayerPrefs.SetString(prefKey, tmp);
        }
        catch (PlayerPrefsException)
        {
            return false;
        }
        return true;
    }

    public static T Load<T>(string prefKey)
    {
        if (!PlayerPrefs.HasKey(prefKey)) return default(T);
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        BinaryFormatter bf = new BinaryFormatter();
        string serializedData = PlayerPrefs.GetString(prefKey);

        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));
        T deserializedObject = (T)bf.Deserialize(dataStream);

        return deserializedObject;
    }

    public void save(DataManager data)
    {
        // 保存用クラスにデータを格納.
        FileController.Save("sv100", data);
        PlayerPrefs.Save();
    }

    public DataManager load()
    {
        DataManager data_tmp = FileController.Load<DataManager>("sv100");
        if (data_tmp != null)
        {
            return data_tmp;
        }
        else
        {
            return null;
        }
    }
}
