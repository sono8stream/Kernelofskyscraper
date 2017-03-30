using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMethodsCollection
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void ForList<T>(ref List<T> collection, Action<T> action, int iniValue = 0)
    {
        for (int i = iniValue; i < collection.Count; i++)
        {
            action(collection[i]);
        }
    }

    public static void ForArray<T>(ref T[] array, Func<T,T> func, int iniValue = 0)
    {
        for (int i = iniValue; i < array.Length; i++)
        {
            array[i] = func(array[i]);
        }
    }
}
