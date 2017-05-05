using System;
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
    }

    public Command CreateInstance()
    {
        return (Command)Activator.CreateInstance(GetType());
    }

    public abstract Command Copy();

    public abstract bool Run(MapObject obj);
}


#region MoveType
public class North : Command
{
    float sp;
    const float ANGLE = 180f;
    bool initial;
    Transform mod;

    public North() : base("North", Data.panelSprites[(int)PanelSpritesName.go], Vector3.zero)
    {
        sp = 10f;
        initial = true;
    }

    public override Command Copy()
    {
        return new North();
    }

    public override bool Run(MapObject obj)
    {
        if (obj.dire == 2)
        {
            return true;
        }
        if (initial)
        {
            mod = obj.transform.FindChild("mod");
            initial = false;
        }
        if (Mathf.Round(mod.eulerAngles.z) == ANGLE)
        {
            obj.dire = 2;
            initial = true;
            return true;
        }
        else
        {
            mod.eulerAngles = (mod.eulerAngles + Vector3.forward * ANGLE) / 2;
            return false;
        }
    }
}

public class South : Command
{
    float sp;
    const float ANGLE = 0f;
    bool initial;
    Transform mod;

    public South() : base("South", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward * 180)
    {
        sp = 10f;
        initial = true;
    }

    public override Command Copy()
    {
        return new South();
    }

    public override bool Run(MapObject obj)
    {
        if (obj.dire == 0)
        {
            return true;
        }
        if (initial)
        {
            mod = obj.transform.FindChild("mod");
            if(mod.eulerAngles.z==270)
            {
                mod.eulerAngles = Vector3.back * 90;
            }
            initial = false;
        }
        if (Mathf.Round(mod.eulerAngles.z) == ANGLE)
        {
            obj.dire = 0;
            initial = true;
            return true;
        }
        else
        {
            mod.eulerAngles = (mod.eulerAngles + Vector3.forward * ANGLE) / 2;
            return false;
        }
    }
}

public class East : Command
{
    float sp;
    const float ANGLE = 90f;
    bool initial;
    Transform mod;

    public East() : base("East", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward * 270)
    {
        sp = 10f;
        initial = true;
    }

    public override Command Copy()
    {
        return new East();
    }

    public override bool Run(MapObject obj)
    {
        if (obj.dire == 1)
        {
            return true;
        }
        if (initial)
        {
            mod = obj.transform.FindChild("mod");
            initial = false;
        }
        if (Mathf.Round(mod.eulerAngles.z) == ANGLE)
        {
            obj.dire = 1;
            initial = true;
            return true;
        }
        else
        {
            mod.eulerAngles = (mod.eulerAngles + Vector3.forward * ANGLE) / 2;
            return false;
        }
    }
}

public class West : Command
{
    float sp;
    const float ANGLE = 270f;
    bool initial;
    Transform mod;

    public West() : base("West", Data.panelSprites[(int)PanelSpritesName.go], Vector3.forward * 90)
    {
        sp = 10f;
        initial = true;
    }

    public override Command Copy()
    {
        return new West();
    }

    public override bool Run(MapObject obj)
    {
        if (obj.dire == 3)
        {
            return true;
        }
        if(initial)
        {
            mod = obj.transform.FindChild("mod");
            if (mod.eulerAngles.z == 0)
            {
                mod.eulerAngles = Vector3.forward * 360;
            }
            initial = false;
        }
        if (Mathf.Round(mod.eulerAngles.z) == ANGLE)
        {
            obj.dire = 3;
            //obj.transform.eulerAngles = new Vector3(0, 0, ANGLE);
            initial = true;
            return true;
        }
        else
        {
            mod.eulerAngles = (mod.eulerAngles + Vector3.forward * ANGLE) / 2;
            return false;
        }
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
            obj.dire = (obj.dire + 1) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.dire * 90);
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
            obj.dire = (obj.dire + 3) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.dire * 90);
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
            obj.dire = (obj.dire + 2) % 4;
            mod.transform.eulerAngles = new Vector3(0, 0, obj.dire * 90);
            count = 0;
            return true;
        }
        return false;
    }
}

public class Warp : Command
{
    const int coLim = 10;

    Vector3 position;//z座標は階層を表す
    int co;

    public Warp(Vector3 pos) : base("Warp", null, Vector3.zero)
    {
        co = coLim;
        position = pos;
    }

    public override Command Copy()
    {
        return new Warp(position);
    }

    public override bool Run(MapObject obj)
    {
        if (obj.map.GetMapData((int)position.z, position).objNo == (int)ObjType.can)
        {
            co--;
            obj.transform.localScale += Vector3.one * (position.z - obj.floor) / coLim * 0.5f;
            obj.transform.position += Vector3.back * (position.z - obj.floor) / coLim;
            if (co == 0)
            {
                obj.transform.localScale = Vector3.one;
                MoveFloor(obj);
                co = coLim;
                return true;
            }
        }
        return false;
    }

    void MoveFloor(MapObject obj)
    {
        obj.map.SetObjData(obj.floor, obj.transform.localPosition, (int)ObjType.can);

        obj.transform.SetParent(obj.map.FloorGOs[(int)position.z].transform);
        obj.transform.localPosition = (Vector2)position;
        obj.floor = (int)position.z;

        obj.map.SetObjData(obj.floor, obj.transform.localPosition, obj.no);
        obj.FlashViewRange();
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
            switch (obj.dire)
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
            CellData c = obj.map.GetMapData(obj.floor, obj.transform.localPosition + mPos);
            if (c.partNo != (int)MapPart.wall && c.objNo == (int)ObjType.can)
            {
                c.objNo = obj.no;
                obj.map.SetObjData(obj.floor, obj.transform.localPosition, (int)ObjType.cannot);
                if (obj.campNo == (int)CampState.ally)
                {
                    obj.FlashViewRange();
                    obj.map.VisualizeRoom(obj.floor, obj.transform.localPosition);
                    obj.map.flrCon.UpdateMapImage();
                }
            }
            else//移動不可
            {
                if (c.partNo == (int)MapPart.wall)
                {
                    obj.waitVanishing = true;
                }
                return true;
            }
        }
        obj.transform.localPosition += mPos * sp;

        if (count == period)
        {
            obj.transform.localPosition = new Vector3(Mathf.Round(obj.transform.localPosition.x),
                Mathf.Round(obj.transform.position.y), 0);
            obj.map.SetObjData(obj.floor, obj.transform.localPosition - mPos, (int)ObjType.can);
            CellData c = obj.map.GetMapData(obj.floor, obj.transform.localPosition);
            count = 0;
            return true;
        }
        count++;
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

#region BattleType
public class Slash:Command
{
    int power;
    int co, lim;
    RobotController enemyRC;
    Transform effectT;

    public Slash() : base("Slash", null, Vector3.zero)
    {
        lim = 10;
        power = 50;
    }

    public override Command Copy()
    {
        return new Slash();
    }

    public override bool Run(MapObject obj)//敵を切る
    {
        Debug.Log("slash!");
        if (co==0)
        {
            effectT = obj.transform.FindChild("mod").FindChild("SlashEffect").FindChild("par1");
            enemyRC = GetEnemy(obj);
            if (!enemyRC) { Debug.Log("null"); return true; }
            co++;
        }
        else if(co<lim)
        {
            co++;
        }
        else
        {
            enemyRC.Damaged(power);
            effectT.GetComponent<ParticleSystem>().Play();
            co = 0;
            return true;
        }
        return false;
    }

    RobotController GetEnemy(MapObject obj)
    {
        int no = obj.map.GetMapData(obj.floor, obj.transform.localPosition + obj.DtoV(obj.dire)).objNo;
        Debug.Log(no);
        return 0 <= no ? obj.map.Objs[no] as RobotController : null;
    }
}

#endregion

#region gimmickType
public class DestroySwitch : Command
{
    MapObject gimmick;

    public DestroySwitch(MapObject g) 
        : base("Switch", Data.panelSprites[(int)PanelSpritesName.switchPanel], Vector3.zero)
    {
        gimmick = g;
    }

    public override Command Copy()
    {
        return new DestroySwitch(gimmick);
    }

    public override bool Run(MapObject obj)
    {
        UnityEngine.Object.Destroy(gimmick.gameObject);
        isDestroyed = true;
        return true;
    }
}

public class StopSwitch : Command
{
    MapObject gimmick;

    public StopSwitch(MapObject g) : base("Switch", null, Vector3.zero)
    {
        gimmick = g;
    }

    public override Command Copy()
    {
        return new StopSwitch(gimmick);
    }

    public override bool Run(MapObject obj)
    {

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
