using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotController : MapObject
{
    #region Property
    //public
    public Robot robot;
    public bool canMove;

    //private
    [SerializeField]
    int comNo;//実行中のコマンド番号
    int comCX, comCY;//コマンドリストの中心
    int comListNo;
    int waitCo;
    int waitLim;

    List<Code> codeList;
    public List<Code> CodeList { get { return codeList; } }
    List<List<Func<bool>>> flag;
    List<List<Command>> c;
    int orderNo, codeNo;
    #endregion

    // Use this for initialization
    new void Start()
    {
        base.Start();
        GetComCPos();
        ReadCommand();
        waitCo = 0;
        waitLim = 1;
        viewRange = 3;
        Debug.Log("viewRange" + viewRange.ToString());
        InitiateCodeList();
        Debug.Log(codeList.Count);
        codeNo = -1;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (!canMove) { return; }
        if (waitCo == waitLim)
        {
            waitCo = 0;
            if (comNo == -2)//パネル読み込み
            {
                Panel p = map.GetMapData(floor, transform.localPosition).panel;
                if (p != null && p.campNo != (int)CampState.neutral && p.campNo != campNo)
                {
                    p = null;
                }
                if (p == null || p.Run(this))//足元見るよ
                {
                    comNo = -1;
                }
            }
            else if (comNo == -1)//コマンド読み込み
            {
                if (0 <= codeNo || CheckFlags(flag[orderNo]))
                {
                    Debug.Log(codeNo);
                    if (c[orderNo].Count == 0 || c[orderNo][codeNo].Run(this))
                    {
                        codeNo++;
                        if (codeNo == c.Count)
                        {
                            codeNo = -1;
                        }
                    }
                }
                else
                {
                    orderNo++;
                    if (orderNo == flag.Count)
                    {
                        orderNo = 0;
                        ReadCommand();
                    }
                }
            }
            else if (0 <= comNo && robot.Command[comNo].Run(this))//自分のコマンド見るよ
            {
                isVanishing = waitVanishing;
                comNo = -2;
            }
        }
        else
        {
            waitCo++;
        }
    }

    void ReadCommand(/*Predicate<Vector2> flag*/)
    {
        comNo = (int)ComNo.Default;//コマンド無し
        for (int i = 0; i < robot.head.ComList.Count; i++)
        {
            comNo = ReadComNo(i);
            if (0 <= comNo)
            {
                break;
            }
        }
        if (comNo == (int)ComNo.Default)
        {
            comNo = robot.head.DefaultComNo;
        }
        Transform icon = transform.FindChild("Command").FindChild("Icon");
        icon.GetComponent<SpriteRenderer>().sprite = robot.Command[comNo].sprite;
        icon.eulerAngles = robot.Command[comNo].angle;
    }

    int ReadComNo(int comListNo)
    {
        for (int r = 1; r < robot.head.Range; r++)
        {
            for (int d = 0; d < robot.head.ComPriList[comListNo].Length; d++)
            {
                Vector2 pos = Vector2.zero;
                int tX = comCX;//初期位置X
                int tY = comCY;
                tX = robot.head.ComPriList[comListNo][d] % robot.head.Range;
                tY = robot.head.ComPriList[comListNo][d] / robot.head.Range;
                int ttX = tX - comCX;
                int ttY = tY - comCY;
                switch (dire)//方向で回転させる
                {
                    case 0:
                        ttX *= -1;
                        break;
                    case 1:
                        ttY *= -1;
                        ttX += ttY;
                        ttY -= ttX;
                        ttX += ttY;
                        break;
                    case 2:
                        ttY *= -1;
                        break;
                    case 3:
                        ttY *= -1;
                        ttX -= ttY;
                        ttY += ttX;
                        ttX -= ttY;
                        break;
                }
                if (0 <= tX && tX < robot.head.Range
                    && 0 <= tY && tY < robot.head.Range
                    && 0 <= robot.head.ComList[comListNo][tY, tX]
                    && map.GetMapData(floor, transform.localPosition
                    + new Vector3(ttX, ttY, 0)).partNo == (int)MapPart.wall/*かべあるとき、条件子*/)
                {
                    return robot.head.ComList[comListNo][tY, tX];
                }
            }
        }
        return (int)ComNo.Default;
    }

    public Vector2 GetComCPos()
    {
        for (int y = 0; y < robot.head.Range; y++)
        {
            for (int x = 0; x < robot.head.Range; x++)
            {
                if (robot.head.ComList[0][y, x] == (int)ComNo.Myself)
                {
                    comCX = x;
                    comCY = y;
                    break;
                }
            }
        }
        return new Vector2(comCX, comCY);
    }

    bool PartInFront(int no)
    {
        Debug.Log("CheckWall");
        return map.GetMapData(floor, transform.localPosition + DtoV(dire)).partNo == no;
    }

    bool EnemyInFront()
    {
        return map.Objs[map.GetMapData(floor, transform.localPosition).objNo].campNo == (int)CampState.enemy;
    }

    bool TrapInFront()
    {
        return map.GetMapData(floor, transform.localPosition).panel.isTrap;
    }

    bool CheckFlags(List<Func<bool>> flags)
    {
        bool b = false;
        if (0<flag[orderNo].Count)
        {
            b = true;
            for (int i = 0; i < flags.Count; i++)
            {
                b &= flags[i]();
            }
        }
        codeNo = b ? 0 : -1;
        return b;
    }

    public void SetAction(byte[] codes)//コマンドコードを読んで条件をセット
    {
        bool onFlag = false;

        flag = new List<List<Func<bool>>>();
        flag.Add(new List<Func<bool>>());

        c = new List<List<Command>>();
        c.Add(new List<Command>());

        orderNo = 0;

        for (int i = 0; i < codes.Length; i++)
        {
            Debug.Log("おかしくね？" + codes[i]);
            if (codeList[codes[i]].value == 0xff)//Ifのばあい
            {
                Debug.Log("IF on");
                onFlag = true;
            }
            else if (codeList[codes[i]].value == 0xfd)//;のばあい
            {
                orderNo++;
            }
            else
            {
                if (onFlag)
                {
                    switch (codeList[codes[i]].value)
                    {
                        case 0x01:
                            flag[orderNo].Add(() => PartInFront((int)MapPart.wall));
                            break;
                        case 0x02:
                            flag[orderNo].Add(() => EnemyInFront());
                            break;
                        case 0x03:
                            flag[orderNo].Add(() => TrapInFront());
                            break;
                    }
                    Debug.Log("Flag Setter");
                }
                else
                {
                    switch (codeList[codes[i]].value)
                    {
                        case 0x01:
                            c[orderNo].Add(new Left());
                            break;
                        case 0x02:
                            c[orderNo].Add(new Right());
                            break;
                        case 0x03:
                            c[orderNo].Add(new Turn());
                            break;
                    }
                }
            }
        }

        Debug.Log(flag.Count);
        Debug.Log(flag[0].Count);

        orderNo = 0;
    }

    void InitiateCodeList()
    {
        codeList = new List<Code>();
        codeList.Add(new Code(0xff,"If"));
        codeList.Add(new Code(0xfe, "&"));
        codeList.Add(new Code(0xfd, ";"));
        codeList.Add(new Code(0x01, "Wall in front"));
        codeList.Add(new Code(0x02, "Enemy in front"));
        codeList.Add(new Code(0x03, "Trap in front"));
        codeList.Add(new Code(0x01, "Left"));
        codeList.Add(new Code(0x02, "Right"));
        codeList.Add(new Code(0x03, "Turn"));

    }
}

public class Code
{
    public byte value;
    public string text;

    public Code(byte no,string text)
    {
        value = no;
        this.text = text;
    }
}

public enum CodeName
{
    If = 0, And, End, Wall, Enemy, Trap, Left, Right, Turn
}
