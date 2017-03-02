using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public static List<Item> items;
    public static List<int> itemCo;//アイテム数
    public static List<Robot> robotRecipe;
    public static List<Head> heads;
    public static List<Body> bodys;
    public static List<Arm> arms;
    public static List<Leg> legs;
    public static List<Robot> robots;
    static SaveManager saveManager;

    static UserData()
    {
        Data.InitiateItems();
        saveManager = new SaveManager();
        if (saveManager.load() == null)
        {
            heads = new List<Head>();
            bodys = new List<Body>();
            arms = new List<Arm>();
            legs = new List<Leg>();
            robotRecipe = new List<Robot>();
            robots = new List<Robot>();
            heads.Add(new Head(3, 0, Data.items[0]));
            bodys.Add(new Body(Data.items[0]));
            arms.Add(new Arm(Data.items[0]));
            legs.Add(new Leg(Data.items[0]));
            robotRecipe.Add(new Robot(heads[0], bodys[0],
                arms[0], legs[0]));
        }
    }

    /*private void OnApplicationQuit()
    {
        Debug.Log("Loaded??");
        saveManager.save(this);
    }*/
}
