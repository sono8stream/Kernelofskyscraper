using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Linq;

//New System
[Serializable]
public class Recipe//パーツ
{
    [NonSerialized]
    protected List<Command> c;
    public List<Command> Command
    {
        get { return c; }
    }
    protected Item[] mats;
    protected string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    protected int hp, lp, sp;
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

    public Recipe(params Item[] items)
    {
        c = new List<Command>();
        mats = new Item[4];
        for (int i = 0; i < items.Length && i < mats.Length; i++)
        {
            mats[i] = items[i];
        }
        hp = 0;
        lp = 0;
        sp = 0;
    }
}

[Serializable]
public class Robot :Recipe
{
    public Head head;
    public Body body;
    public Arm arm;
    public Leg leg;
    [NonSerialized]
    public Sprite icon;

    public Robot(Head h,Body b,Arm a,Leg l)
    {
        InitiateRobot(h, b, a, l);
    }

    void InitiateRobot(Head h, Body b, Arm a, Leg l)
    {
        c = new List<Command>();
        c.Add(new DefaultCommand());
        head = h;
        body = b;
        arm = a;
        leg = l;
        c.AddRange(head.Command);
        c.AddRange(body.Command);
        c.AddRange(arm.Command);
        c.AddRange(leg.Command);
        name = "rb-000";
        hp = h.HP + b.HP + a.HP + l.HP;
        lp = h.LP + b.LP + a.LP + l.LP;
        sp = h.SP + b.SP + a.SP + l.SP;
        icon = Data.roboSprites[0];
    }
}

[Serializable]
public class Head : Recipe
{
    List<int[,]> comList;//コマンド番号リスト
    public List<int[,]> ComList
    {
        get { return comList; }
        set { comList = value; }
    }
    List<int[]> comPriList;//コマンド優先リスト、より小さい位置に、優先するコマンド番号の位置が入る
    //呼び出し時は[value/range,value%range]
    public List<int[]> ComPriList
    {
        get { return comPriList; }
        set { comPriList = value; }
    }
    int defComNo;//デフォルトのコマンド番号
    public int DefaultComNo
    {
        get { return defComNo; }
        set { defComNo = value; }
    }
    int range;//コマンドリスト大きさ
    public int Range
    {
        get { return range; }
        set { range = value; }
    }
    Dictionary<int, int[,]> rangeType;
    int typeNo;

    public Head(int range, int typeNo, params Item[] i) : base(i)
    {
        this.range = range;
        comList = new List<int[,]>();
        comPriList = new List<int[]>();
        rangeType = new Dictionary<int, int[,]>();
        rangeType.Add(0, new int[3, 3] { //十字
            { -2, 0, -2 },
            { 3, -3, 1 },
            { -2, 2, -2 } });
        rangeType.Add(1, new int[3, 3] { //円
            { 7, 0, 1 },
            { 6, -3, 2 },
            { 5, 4, 3 } });
        rangeType.Add(2, new int[3, 3] { //放射
            { 1, 2, 3 },
            { -2, 0, -2 },
            { -2, -3, -2 } });
        this.typeNo = typeNo;
        AddComList();
        mats = i;
        name = "hd-000";
        float comp = -0.5f;//補正
        sp = (int)(mats[0].HP * comp);
        defComNo = 1;
    }

    public void AddComList()
    {
        comList.Add(rangeType[typeNo]);
        List<int> priListTemp = new List<int>();
        int no = comList.Count - 1;
        //int count= rangeType[typeNo].
        for (int k = 0; k < range * range; k++)
        {
            if ((int)ComNo.Default <= comList[no][k / range, k % range])
            {
                comList[no][k / range, k % range] = (int)ComNo.Default;//デフォルトに設定
                priListTemp.Add(k);
            }
        }
        comPriList.Add(priListTemp.ToArray());
    }
}

public enum ComNo { Myself = -3, None = -2, Default = 0 }

[Serializable]
public class Body : Recipe
{
    public Body(params Item[] i) : base(i)
    {
        mats = i;
        name = "bd-000";
        float comp = 10f;//補正
        hp = (int)(mats[0].HP * comp);
        lp = (int)(mats[0].HP * comp);
        sp = -(int)(mats[0].HP * comp);
    }
}

[Serializable]
public class Arm : Recipe
{
    int pow;//火力

    public Arm(params Item[] i) : base(i)
    {
        mats = i;
        name = "am-000";
        float comp = 10f;
        switch(mats[0].HP)
        {
            case -1://Pioneer
                pow = (int)(mats[0].LP * comp);
                break;
        }
    }
}

[Serializable]
public class Leg : Recipe
{

    public Leg(params Item[] i) : base(i)
    {
        /*c.Add(new North());
        c.Add(new South());
        c.Add(new East());
        c.Add(new West());*/
        c.Add(new Go());
        c.Add(new Left());
        c.Add(new Right());
        c.Add(new Turn());
        mats = i;
        name = "lg-000";
        float comp = 15f;//補正
        sp = (int)(mats[0].HP * comp);
    }
}
