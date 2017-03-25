using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class UserData
{
    [NonSerialized]
    public static UserData instance;
    public List<Item> items;
    public List<int> itemCo;//アイテム数
    public List<Robot> robotRecipe;//設計図
    public List<Head> heads;
    public List<Body> bodies;
    public List<Arm> arms;
    public List<Leg> legs;
    public List<Robot> robots;//本体
    public List<Command> commands;//パネルセット

    public UserData()
    {
        heads = new List<Head>();
        bodies = new List<Body>();
        arms = new List<Arm>();
        legs = new List<Leg>();
        robotRecipe = new List<Robot>();
        robots = new List<Robot>();
        commands = new List<Command>();
        heads.Add(new Head(3, 0, Data.items[0]));
        bodies.Add(new Body(Data.items[0]));
        arms.Add(new Arm(Data.items[0]));
        legs.Add(new Leg(Data.items[0]));
        robotRecipe.Add(new Robot(heads[0], bodies[0], arms[0], legs[0]));
        commands.Add(new North());
        commands.Add(new South());
        commands.Add(new East());
        commands.Add(new West());
    }
}
