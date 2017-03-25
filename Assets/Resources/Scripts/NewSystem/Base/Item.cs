using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    string name;
    public string Name
    {
        get { return name; }
    }
    int typeNo;
    int hp, lp, sp;
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }
    public int LP
    {
        get { return lp; }
        set { lp = value; }
    }
    public int SP
    {
        get { return sp; }
        set { sp = value; }
    }

    public Item(string name,int hp=0,int lp=0,int sp=0)
    {
        this.name = name;
        this.hp = hp;
        this.lp = lp;
        this.sp = sp;
    }
}
