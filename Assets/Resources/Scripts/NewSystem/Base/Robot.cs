using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]//New System
public class Recipe//パーツ
{
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

public class Robot :Recipe
{
    public Head head;
    public Body body;
    public Arm arm;
    public Leg leg;

    public Robot(Head h,Body b,Arm a,Leg l)
    {
        c = new List<Command>();
        head = h;
        body = b;
        arm = a;
        leg = l;
        c.AddRange(head.Command);
        c.AddRange(body.Command);
        c.AddRange(arm.Command);
        c.AddRange(leg.Command);
        name = "新しいロボット";
        hp = h.HP + b.HP + a.HP + l.HP;
        lp = h.LP + b.LP + a.LP + l.LP;
        sp = h.SP + b.SP + a.SP + l.SP;
    }
}

public class Head :Recipe
{
    int[,] comList;//コマンド番号リスト
    public int[,] ComList
    {
        get { return comList; }
        set { comList = value; }
    }
    int range;
    public int Range
    {
        get { return range; }
    }
    Dictionary<int, int[,]> rangeType;

    public Head(int range, int typeNo, params Item[] i) : base(i)
    {
        range = this.range;
        comList = new int[range, range];
        rangeType = new Dictionary<int, int[,]>();
        rangeType.Add(0, new int[3, 3] { //十字
            { -1, 0, -1 },
            { 1, -2, 3 },
            { -1, 2, -1 } });
        rangeType.Add(1, new int[3, 3] { //円
            { 0, 0, 0 },
            { 0, -2, 0 },
            { 0, 0, 0 } });
        rangeType.Add(2, new int[3, 3] { //放射
            { 0, 0, 0 },
            { -1, 0, -1 },
            { -1, -2, -1 } });
        comList = rangeType[typeNo];
        mats = i;
        name = "新しいヘッド";
        float comp = -0.5f;//補正
        sp = (int)(mats[0].HP * comp);
    }
}

public class Body : Recipe
{
    public Body(params Item[] i) : base(i)
    {
        mats = i;
        name = "新しいボディ";
        float comp = 10f;//補正
        hp = (int)(mats[0].HP * comp);
        lp = (int)(mats[0].HP * comp);
        sp = -(int)(mats[0].HP * comp);
    }
}

public class Arm : Recipe
{
    int pow;//火力

    public Arm(params Item[] i) : base(i)
    {
        mats = i;
        name = "新しいアーム";
        float comp = 10f;
        switch(mats[0].HP)
        {
            case -1://Pioneer
                pow = (int)(mats[0].LP * comp);
                break;
        }
    }
}

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
        name = "新しいレッグ";
        float comp = 15f;//補正
        sp = (int)(mats[0].HP * comp);
    }
}
