using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data//基幹データ部
{
    public static List<Item> items;
    public static List<Sprite> panelSprites;
    public static List<Command> commands;
    public static List<Sprite> roboSprites;

    public static void Initiate()
    {
        Debug.Log("OK");
        items = new List<Item>();
        items.Add(new Item("Bマテリアル", 10));
        items.Add(new Item("Sマテリアル", 12));
        items.Add(new Item("マテリアルE", 15));
        items.Add(new Item("マテリアルD", 16));
        items.Add(new Item("マテリアルC", 17));
        items.Add(new Item("マテリアルB", 18));
        items.Add(new Item("マテリアルA", 19));
        items.Add(new Item("Mマテリアル", 20));
        items.Add(new Item("スピリットP", -1, 10));//Pioneer
        items.Add(new Item("スピリットS", -2, 5));//Searcher
        items.Add(new Item("スピリットCh", -3, 5));//Charger
        items.Add(new Item("スピリットCo", -4, 5));//Collecter
        panelSprites = new List<Sprite>();
        panelSprites.Add(Resources.Load<Sprite>("Sprites/Battle/Panel/default"));
        panelSprites.Add(Resources.Load<Sprite>("Sprites/Battle/Panel/left"));
        panelSprites.Add(Resources.Load<Sprite>("Sprites/Battle/Panel/turn"));
        panelSprites.Add(Resources.Load<Sprite>("Sprites/Battle/Panel/go"));
        commands = new List<Command>();
        commands.Add(new North());
        commands.Add(new South());
        commands.Add(new East());
        commands.Add(new West());
        commands.Add(new Left());
        commands.Add(new Right());
        commands.Add(new Turn());
        roboSprites = new List<Sprite>();
        roboSprites.Add(Resources.Load<Sprite>("Sprites/Battle/robo_b10_icon"));
    }

}

public enum PanelSpritesName
{
    def = 0, left, turn, go
}

public enum CommandType
{
    north = 0, south, east, west, left, right, turn
}
