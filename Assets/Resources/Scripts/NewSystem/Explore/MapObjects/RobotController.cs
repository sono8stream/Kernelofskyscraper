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
    public int robotType;
    public bool canMove;
    public List<Code> codeList { get; private set; }
    public Command specialCom;

    //private
    [SerializeField]
    int comNo;//実行中のコマンド番号
    int comCX, comCY;//コマンドリストの中心
    int comListNo;
    int waitCo;
    int waitLim;
    int damageCo;
    int damageLim;

    //Effect / Sound
    [SerializeField]
    GameObject genEffect;
    [SerializeField]
    AudioClip damageSE, attackSE, moveSE;

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
        viewRange = viewRange == 1 ? 3 : viewRange;
        InitiateCodeList();
        comNo = -3;
        codeNo = -1;
        damageLim = 10;
        damageCo = damageLim;
        
        audioSource.PlayOneShot(generateSE);
        Effect(genEffect);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (isVanishing) { return; }

        if (damageCo < damageLim)
        {
            transform.FindChild("mod").FindChild("model").GetComponent<Renderer>().material.color
                = Color.red * Mathf.Cos(damageCo * 4 * Mathf.PI / damageLim);
            damageCo++;
            if (damageCo == damageLim)
            {
                transform.FindChild("mod").FindChild("model").GetComponent<Renderer>().material.color
                    = Color.white;
            }
        }

        if (!canMove)
        {
            if(waitVanishing)
            {
                isVanishing = true;
                Break();
            }
            return;
        }
        #region Command
        if (waitCo == waitLim)
        {
            waitCo = 0;
            if (comNo == -3)//パネル読み込み
            {
                Panel p = map.GetMapData(floor, transform.localPosition).panel;
                if (p != null && p.campNo != (int)CampState.neutral && p.campNo != campNo)
                {
                    p = null;
                }
                if (p == null || p.Run(this))//足元見るよ
                {
                    comNo = -2;
                }
            }
            else if (comNo == -2 &&
                (specialCom == null || specialCom.Run(this)))//ロボタイプ専用コマンドを実行
            {
                Debug.Log(specialCom);
                comNo = -1;
            }
            else if (comNo == -1)//コマンド読み込み
            {
                switch (campNo)
                {
                    case (int)CampState.ally:
                        RunOrder();
                        break;
                    case (int)CampState.enemy:
                        RunAI();
                        break;
                }
            }
            else if (0 <= comNo && robot.Command[comNo].Run(this))//自分のコマンド見るよ
            {
                if (waitVanishing)
                {
                    isVanishing = true;
                    Break();
                }
                comNo = -3;
            }
        }
        else
        {
            waitCo++;
        }
        #endregion
    }

    void RunOrder()
    {
        if (0 < c[orderNo].Count && (0 <= codeNo || CheckFlags(flag[orderNo])))
        {
            if (c[orderNo][codeNo].Run(this))
            {
                codeNo++;
                if (c.Count <= codeNo)
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

    #region Methods of Order
    bool PartInFront(int no)
    {
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
        if (0 < flag[orderNo].Count)
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
        codeList.Add(new Code(0xff, "If"));
        codeList.Add(new Code(0xfe, "&"));
        codeList.Add(new Code(0xfd, ";"));
        codeList.Add(new Code(0x01, "Wall in front"));
        codeList.Add(new Code(0x02, "Enemy in front"));
        codeList.Add(new Code(0x03, "Trap in front"));
        codeList.Add(new Code(0x01, "Left"));
        codeList.Add(new Code(0x02, "Right"));
        codeList.Add(new Code(0x03, "Turn"));

    }
    #endregion

    void RunAI()//AIの処理だよ
    {
        int no = (int)ObjType.can;
        Vector3 iniPos = -Vector2.one * ((viewRange - viewRange % 2) / 2);
        Vector3 distance = Vector2.one;
        CellData c;
        for (int i = 0; i < viewRange * viewRange; i++)
        {
            if (i != (viewRange * viewRange - viewRange % 2) / 2)
            {
                distance = transform.localPosition + iniPos + new Vector3(i % viewRange, i / viewRange);
                c = map.GetMapData(floor, distance);
                no = c != null ? c.objNo : (int)ObjType.can;
                if ((int)ObjType.can < no && map.Objs[no].campNo != campNo
                    && map.Objs[no].GetType().Equals(GetType()))//自分に対して敵ロボット
                {
                    //Debug.Log(i % viewRange);
                    comNo = ApproachEnemy(distance);
                    break;
                }
            }
        }
        //if (no == (int)ObjType.can) { return; }
    }

    #region Methods of AI
    int ApproachEnemy(Vector3 enemyPos)
    {
        Vector2 vec = enemyPos - transform.localPosition;
        //Debug.Log(vec);
        int[,] costArray = new int[viewRange, viewRange];
        int radius = (viewRange - viewRange % 2) / 2;

        SetCost(ref costArray, radius, radius, 1, radius);
        int x = (int)vec.x + radius;
        int y = (int)vec.y + radius;

        if (costArray[x, y] == 0)
        {
            return -1;
        }
        else
        {
            GetRoute(costArray, ref x, ref y);
            int d = VtoD(new Vector2(x - radius, y - radius));
            int commandNo = -1;
            switch ((dire - d + 4) % 4)
            {
                case 0:
                    commandNo = (int)CommandID.go;
                    break;
                case 1:
                    commandNo = (int)CommandID.right;
                    break;
                case 2:
                    commandNo = (int)CommandID.turn;
                    break;
                case 3:
                    commandNo = (int)CommandID.left;
                    break;
            }
            return commandNo;
        }
    }

    void SetCost(ref int[,] costData, int x, int y, int cost, int radius)
    {
        costData[x, y] = cost;
        for (int i = 0; i < 4; i++)
        {
            if (!(i == 0 && y == 0) && !(i == 1 && x == viewRange - 1)
                && !(i == 2 && y == viewRange - 1) && !(i == 3 && x == 0)
                && map.GetMapData(floor, transform.localPosition
                + new Vector3(x - radius, y - radius, 0) + DtoV(i)).partNo
                == (int)MapPart.floor)
            {
                if (costData[x + (int)DtoV(i).x, y + (int)DtoV(i).y] == 0
                    ||cost+1 < costData[x + (int)DtoV(i).x, y + (int)DtoV(i).y])
                {
                    SetCost(ref costData, x + (int)DtoV(i).x, y + (int)DtoV(i).y, cost + 1, radius);
                }
            }
        }
    }

    void GetRoute(int[,] costData,ref int x,ref int y)
    {
        int cost = costData[x, y];
        while (2 < cost)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!(i == 0 && y == 0) && !(i == 1 && x == viewRange - 1)
                && !(i == 2 && y == viewRange - 1) && !(i == 3 && x == 0)
                && costData[x + (int)DtoV(i).x, y + (int)DtoV(i).y] == cost - 1)
                {
                    x += (int)DtoV(i).x;
                    y += (int)DtoV(i).y;
                    cost--;
                    break;
                }
            }
        }
    }
    #endregion

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
                    + new Vector3(ttX, ttY, 0)) != null
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

    public void Damaged(int value)
    {
        damageCo = 0;
        robot.HP -= value;
        Debug.Log(robot.HP);
        audioSource.PlayOneShot(damageSE);
        if (robot.HP <= 0)
        {
            robot.HP = 0;
            waitVanishing = true;
        }
    }

    public void ChangeMovable(bool movable)
    {
        canMove = movable;
        audioSource.PlayOneShot(moveSE);
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
