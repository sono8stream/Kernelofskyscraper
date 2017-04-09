﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public bool isDestroyed;//the panel judges to destroy or not.
    public string name;//名前
    public Sprite sprite;//アイコン
    public Vector3 angle;

    public Command(string name, Sprite sprite, Vector3 angle)
    {
        this.name = name;
        this.sprite = sprite;
        this.angle = angle;
        Debug.Log(GetType());
    }

    public Command CreateInstance()
    {
        return (Command)Activator.CreateInstance(GetType());
    }

    public abstract Command Copy();

    public abstract bool Run(MapObject obj);
}

public class North : Command
{
    float sp;
    const float ANGLE = 180f;

    public North() : base("North", Data.panelSprites[(int)PanelSpritesName.go], Vector3.zero)
    {
        sp = 10f;
    }

    public override Command Copy()
    {
        return new North();
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

#region MoveType
public class South : Command
{
    float sp;
    const float ANGLE = 0f;

    public South() : base("South", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward*180)
    {
        sp = 10f;
    }

    public override Command Copy()
    {
        return new South();
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

    public East() : base("East", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward * 270)
    {
        sp = 10f;
    }

    public override Command Copy()
    {
        return new East();
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

    public West() : base("West", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward * 90)
    {
        sp = 10f;
    }

    public override Command Copy()
    {
        return new West();
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

    public Left() : base("Left", Data.panelSprites[(int)PanelSpritesName.left], Vector3.zero)
    {
        sp = 15f;
        period = (int)(90f / sp);
        count = 0;
    }

    public override Command Copy()
    {
        return new Left();
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

    public Right() : base("Right", Data.panelSprites[(int)PanelSpritesName.left], Vector3.up * 180)
    {
        sp = 15f;
        period = (int)(90f / sp);
        count = 0;
    }

    public override Command Copy()
    {
        return new Right();
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

    public Turn() : base("Turn", Data.panelSprites[(int)PanelSpritesName.turn], Vector3.zero)
    {
        sp = 30f;
        period = (int)(180f / sp);
        count = 0;
    }

    public override Command Copy()
    {
        return new Turn();
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

    public Go() : base("Go", Data.panelSprites[(int)PanelSpritesName.go], Vector3.zero)
    {
        sp = 0.1f;
        period = (int)(1f / sp);
        count = 0;
    }

    public override Command Copy()
    {
        return new Go();
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
            CellData c = obj.Map.GetMapData(obj.Floor, obj.transform.localPosition + mPos);
            if ((c.partNo == (int)MapPart.floor || c.partNo == (int)MapPart.stairD || c.partNo == (int)MapPart.stairU)
                && c.objNo == (int)ObjType.can)
            {
                c.objNo = obj.No;
                obj.Map.SetObjData(obj.Floor, obj.transform.localPosition, (int)ObjType.cannot);
                c.tile.SetActive(true);
            }
            else
            {
                return true;
            }
        }
        obj.transform.localPosition += mPos * sp;
        count++;
        if (count == period)
        {
            obj.transform.localPosition
                = new Vector3(Mathf.Round(obj.transform.localPosition.x), Mathf.Round(obj.transform.position.y), 0);
            obj.Map.SetObjData(obj.Floor, obj.transform.localPosition - mPos, (int)ObjType.can);
            CellData c = obj.Map.GetMapData(obj.Floor, obj.transform.localPosition);
            if (c.partNo == (int)MapPart.stairD||c.partNo==(int)MapPart.stairU)//下る
            {
                Vector3 posTemp = obj.transform.localPosition;
                int floorTemp= c.partNo == (int)MapPart.stairD ? obj.Floor - 1 : obj.Floor + 1;
                if (obj.Map.GetMapData(floorTemp, posTemp).objNo == (int)ObjType.can)
                {
                    obj.Floor = floorTemp;
                    obj.transform.SetParent(
                        obj.transform.parent.parent.FindChild("Floor" + (obj.Floor + 1).ToString()));
                    obj.transform.localPosition = posTemp;
                    c.objNo = (int)ObjType.can;
                    obj.Map.SetObjData(obj.Floor, posTemp, obj.No);
                }
            }
            count = 0;
            return true;
        }
        return false;
    }
}
#endregion

#region RecoverType
public class EnergyRecover : Command
{
    StatusController status;

    public EnergyRecover()
        : base("EnergyRecover", Data.panelSprites[(int)PanelSpritesName.eRecover], Vector3.zero)
    {
        status = GameObject.Find("StatusMenu").GetComponent<StatusController>();
    }

    public override Command Copy()
    {
        return new EnergyRecover();
    }

    public override bool Run(MapObject obj)
    {
        status.ChangeEnergy(10);
        isDestroyed = true;
        return true;
    }
}

public class CapacityRecover : Command
{
    StatusController status;

    public CapacityRecover()
        : base("CapacityRecover", Data.panelSprites[(int)PanelSpritesName.cRecover], Vector3.zero)
    {
        status = GameObject.Find("StatusMenu").GetComponent<StatusController>();
    }

    public override Command Copy()
    {
        return new CapacityRecover();
    }

    public override bool Run(MapObject obj)
    {
        status.ChangeCapacity(10);
        isDestroyed = true;
        return true;
    }
}
#endregion

public class DefaultCommand : Command
{
    public DefaultCommand() : base("Default", Data.panelSprites[(int)PanelSpritesName.def], Vector3.zero)
    {

    }

    public override Command Copy()
    {
        return new DefaultCommand();
    }

    public override bool Run(MapObject obj)
    {
        return true;
    }
}