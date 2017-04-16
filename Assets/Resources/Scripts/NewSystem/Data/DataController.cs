using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{

    static SaveManager saveManager;

    public DataController()
    {
    }

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void InitiateData()
    {
        Data.Initiate();
        Debug.Log(Data.items.Count);
        UserData.instance = new UserData();
        saveManager = new SaveManager();
        saveManager.load();
    }

    private void OnApplicationQuit()
    {
        saveManager.save(UserData.instance);
    }
}
