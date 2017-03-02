using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data//基幹データ部
{
    public static List<Item> items;

    public Data()
    {

    }

    public static void InitiateItems()
    {
        items = new List<Item>();
        items.Add(new Item("Bマテリアル", 10));
        items.Add(new Item("Sマテリアル", 12));
        items.Add(new Item("マテリアルE", 15));
        items.Add(new Item("マテリアルD", 16));
        items.Add(new Item("マテリアルC", 17));
        items.Add(new Item("マテリアルB", 18));
        items.Add(new Item("マテリアルA", 19));
        items.Add(new Item("Mマテリアル", 20));
        items.Add(new Item("スピリットP", -1,10));//Pioneer
        items.Add(new Item("スピリットS", -2,5));//Searcher
        items.Add(new Item("スピリットCh", -3,5));//Charger
        items.Add(new Item("スピリットCo", -4,5));//Collecter
    }
}
