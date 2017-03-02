using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Command
{
    public string name;//名前
    public Sprite sprite;//アイコン
    public Vector3 angle;
    public Command(string name, string sPath, Vector3 angle)
    {
        this.name = name;
        sprite = Resources.Load<Sprite>(sPath);
        this.angle = angle;
    }
    public abstract bool Run(MapObject obj);
}

public class North : Command
{
    float sp;
    const float ANGLE = 180f;

    public North() : base("North", "Sprites/Battle/Panel/panel_2", Vector3.zero)
    {
        sp = 10f;
    }

    public override bool Run(MapObject obj)
    {
        if (obj.Dire != 2)
        {
            float dif = ANGLE - obj.transform.eulerAngles.z;
            Vector3 mPos = Vector3.forward * dif / Mathf.Abs(dif);
            obj.transform.eulerAngles += mPos * sp;
            if (obj.transform.eulerAngles.z != ANGLE)
            {
                obj.Dire = 2;
                obj.transform.eulerAngles = new Vector3(0, 0, ANGLE);
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class South : Command
{
    float sp;
    const float ANGLE = 0f;

    public South() : base("South",
        "Sprites/Battle/Panel/panel_2", Vector3.forward*180)
    {
        sp = 10f;
    }

    public override bool Run(MapObject obj)
    {
        if (obj.Dire != 0)
        {
            float dif = ANGLE - obj.transform.eulerAngles.z;
            Vector3 mPos = Vector3.forward * dif / Mathf.Abs(dif);
            obj.transform.eulerAngles += mPos * sp;
            if (obj.transform.eulerAngles.z != ANGLE)
            {
                obj.Dire = 0;
                obj.transform.eulerAngles = new Vector3(0, 0, ANGLE);
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class East : Command
{
    float sp;
    const float ANGLE = 90f;

    public East() : base("East",
        "Sprites/Battle/Panel/panel_2", Vector3.forward * 270)
    {
        sp = 10f;
    }

    public override bool Run(MapObject obj)
    {
        if (obj.Dire != 1)
        {
            float dif = ANGLE - obj.transform.eulerAngles.z;
            Vector3 mPos = Vector3.forward * dif / Mathf.Abs(dif);
            obj.transform.eulerAngles += mPos * sp;
            if (obj.transform.eulerAngles.z != ANGLE)
            {
                obj.Dire = 1;
                obj.transform.eulerAngles = new Vector3(0, 0, ANGLE);
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class West : Command
{
    float sp;
    const float ANGLE = 270f;

    public West() : base("West",
        "Sprites/Battle/Panel/panel_2", Vector3.forward * 90)
    {
        sp = 10f;
    }

    public override bool Run(MapObject obj)
    {
        if (obj.Dire != 3)
        {
            float dif = ANGLE - obj.transform.eulerAngles.z;
            Vector3 mPos = Vector3.forward * dif / Mathf.Abs(dif);
            obj.transform.eulerAngles += mPos * sp;
            if (obj.transform.eulerAngles.z != ANGLE)
            {
                obj.Dire = 3;
                obj.transform.eulerAngles = new Vector3(0, 0, ANGLE);
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}

public class Left : Command
{
    float sp;
    int period;
    int count;

    public Left() : base("Left", "Sprites/Battle/Panel/left", Vector3.zero)
    {
        sp = 15f;
        period = (int)(90f / sp);
        count = 0;
    }

    public override bool Run(MapObject obj)
    {
        Transform mod = obj.transform.FindChild("mod");
        mod.transform.eulerAngles += Vector3.forward * sp;
        count++;
        if (count == period)
        {
            obj.Dire = (obj.Dire + 1) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.Dire * 90);
            count = 0;
            return true;
        }
        return false;
    }
}

public class Right : Command
{
    float sp;
    int period;
    int count;

    public Right() : base("Right", "Sprites/Battle/Panel/left", Vector3.up * 180)
    {
        sp = 15f;
        period = (int)(90f / sp);
        count = 0;
    }

    public override bool Run(MapObject obj)
    {
        Transform mod = obj.transform.FindChild("mod");
        mod.transform.eulerAngles += Vector3.back * sp;
        count++;
        if (count == period)
        {
            obj.Dire = (obj.Dire + 3) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.Dire * 90);
            count = 0;
            return true;
        }
        return false;
    }
}

public class Turn : Command
{
    float sp;
    int period;
    int count;

    public Turn() : base("Turn", "Sprites/Battle/Panel/turn", Vector3.zero)
    {
        sp = 30f;
        period = (int)(180f / sp);
        count = 0;
    }

    public override bool Run(MapObject obj)
    {
        Transform mod = obj.transform.FindChild("mod");
        mod.transform.eulerAngles += Vector3.forward * sp;
        count++;
        if (count == period)
        {
            obj.Dire = (obj.Dire + 2) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.Dire * 90);
            count = 0;
            return true;
        }
        return false;
    }
}

public class Go : Command
{
    float sp;
    int period;
    int count;
    Vector3 mPos = Vector3.zero;

    public Go() : base("Go", "Sprites/Battle/Panel/go", Vector3.zero)
    {
        sp = 0.1f;
        period = (int)(1f / sp);
        count = 0;
    }

    public override bool Run(MapObject obj)
    {
        if (count == 0)
        {
            switch (obj.Dire)
            {
                case 0:
                    mPos = Vector3.down;
                    break;
                case 1:
                    mPos = Vector3.right;
                    break;
                case 2:
                    mPos = Vector3.up;
                    break;
                case 3:
                    mPos = Vector3.left;
                    break;
            }
            if (obj.Map.GetMapData(obj.transform.position + mPos) == 1)
            {
                obj.Map.SetObjData(obj.transform.position + mPos, obj.No);
            }
            else
            {
                return true;
            }
        }
        obj.transform.position += mPos * sp;
        count++;
        if (count == period)
        {
            obj.transform.position 
                = new Vector3(Mathf.Round(obj.transform.position.x),
                Mathf.Round(obj.transform.position.y), 0);
            obj.Map.SetObjData(obj.transform.position - mPos, 0);
            count = 0;
            return true;
        }
        return false;
    }
}
