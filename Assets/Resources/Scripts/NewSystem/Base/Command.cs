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
    public int cost;//生成コスト

    public Command(string name, Sprite sprite, Vector3 angle,int cost= 10)
    {
        this.name = name;
        this.sprite = sprite;
        this.angle = angle;
        this.cost = cost;
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
    int coLim = 10;

    Vector3 position;//z座標は階層を表す
    int co;

    public Warp(Vector3 pos) : base("Warp", null, Vector3.zero)
    {
        co = coLim;
        position = pos;
    }

    public Warp() : base("Warp", Data.panelSprites[(int)PanelSpritesName.warpGo], Vector3.zero, 15)
    {
        position = -Vector2.one * 101;
        co = coLim;
    }

    public override Command Copy()
    {
        return new Warp(position);
    }

    public override bool Run(MapObject obj)
    {
        if (position.x < -100) { return true; }

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

        if(obj.floor!=(int)position.z)
        {
            obj.transform.SetParent(obj.map.FloorGOs[(int)position.z].transform);
            obj.floor = (int)position.z;
        }
        obj.transform.localPosition = (Vector2)position;

        obj.map.SetObjData(obj.floor, obj.transform.localPosition, obj.no);
        obj.FlashViewRange();
    }

    public void UpdatePos(Vector3 pos)
    {
        position = pos;
    }
}

public class WarpDestination : Command
{
    const int coLim = 10;

    Vector3 position;//z座標は階層を表す

    public WarpDestination()
        : base("Warp", Data.panelSprites[(int)PanelSpritesName.warpCome], Vector3.zero, 15)
    {
    }

    public override Command Copy()
    {
        return new Warp(position);
    }

    public override bool Run(MapObject obj)
    {
        return true;
    }
}

public class Go : Command
{
    int period;
    int count;
    Vector3 mPos = Vector3.zero;

    public Go() : base("Go", Data.panelSprites[(int)PanelSpritesName.go], Vector3.zero)
    {
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
            period = (int)(1f / obj.speed);

            mPos = obj.DtoV(obj.dire);
            obj.map.VisualizeRoom(obj.floor, obj.transform.localPosition);

            if (!CheckAllFront(obj))
            {
                return true;
            }
        }
        obj.transform.localPosition += mPos * obj.speed;

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

    bool CheckAllFront(MapObject obj)
    {
        Vector3 verVec = new Vector3(obj.DtoV(obj.dire).y, obj.DtoV(obj.dire).x);
        Vector3 iniPos = (obj.DtoV(obj.dire) + verVec) * (obj.range - obj.range % 2) * 0.5f
            + obj.DtoV(obj.dire);
        CellData[] cArray = new CellData[obj.range];
        bool ok = true;

        for (int i = 0; i < obj.range; i++)
        {
            cArray[i] = obj.map.GetMapData(obj.floor, obj.transform.localPosition + iniPos - verVec * i);
            ok &= cArray[i].partNo != (int)MapPart.wall && cArray[i].objNo == (int)ObjType.can;
        }

        if (ok)
        {
            for (int i = 0; i < cArray.Length; i++)
            {
                cArray[i].objNo = obj.no;
            }
            obj.map.SetObjData(obj.floor, obj.transform.localPosition, (int)ObjType.cannot);
            if (obj.campNo == (int)CampState.ally)
            {
                obj.FlashViewRange();
                obj.map.flrCon.UpdateMapImage();
            }
        }
        else//移動不可
        {
            bool vanish = true;
            for (int i = 0; i < cArray.Length; i++)
            {
                vanish &= !(cArray[i].objNo == (int)ObjType.cannot
                    || ((int)ObjType.can < cArray[i].objNo && cArray[i].objNo < obj.map.Objs.Count
                && obj.GetComponent<RobotController>().robotType == (int)RobotType.fighter
                && obj.CheckEnemyOrNot(obj.map.Objs[cArray[i].objNo])));
            }
            if (vanish)
            {
                obj.waitVanishing = true;
            }
        }
        return ok;
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
public class Slash : Command
{
    int power;
    int co, lim;
    RobotController enemyRC;
    Transform effectT;

    public Slash() : base("Slash", null, Vector3.zero)
    {
        lim = 30;
        power = 30;
    }

    public override Command Copy()
    {
        return new Slash();
    }

    public override bool Run(MapObject obj)//敵を切る
    {
        Debug.Log("slash!");
        if (co == 0)
        {
            effectT = obj.transform.FindChild("mod").FindChild("SlashEffect").FindChild("par1");
            enemyRC = GetEnemy(obj);
            if (!enemyRC || enemyRC.waitVanishing) { Debug.Log("null"); return true; }
            co++;
        }
        else if (co < lim)
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
        return 0 <= no && no < obj.map.Objs.Count && obj.CheckEnemyOrNot(obj.map.Objs[no])
            ? obj.map.Objs[no] as RobotController : null;
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
        gimmick.isVanishing = true;
        isDestroyed = true;
        return true;
    }
}

public class StopSwitch : Command
{
    MapObject gimmick;
    int co, lim;

    public StopSwitch(MapObject g) : base("Switch", null, Vector3.zero)
    {
        gimmick = g;
        co = 0;
        lim = 10;
    }

    public override Command Copy()
    {
        return new StopSwitch(gimmick);
    }

    public override bool Run(MapObject obj)
    {
        if (co == 0)
        {
            gimmick.gameObject.SetActive(false);
        }
        co++;
        if (co == lim)
        {
            co = 0;
            gimmick.gameObject.SetActive(true);
        }
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
