using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotController : MapObject
{
    Robot robot;
    public Robot Robot
    {
        get { return robot; }
        set { robot = value; }
    }
    int comCX, comCY;//コマンドリストの中心
    [SerializeField]
    int comNo;//実行中のコマンド番号
    int comListNo;
    int waitCo;
    int waitLim;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        GetComCPos();
        ReadCommand();
        waitCo = 0;
        waitLim = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitCo == waitLim)
        {
            waitCo = 0;
            if (0 <= comNo && robot.Command[comNo].Run(this))
            {
                Debug.Log(robot.Command[comNo].name);
                ReadCommand();
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
                /*switch (d)//dire=0が上、反時計回り
                { 
                    case 0:miss
                        tY -= r;
                        break;
                    case 1:
                        tX -= r;
                        tY -= r;
                        break;
                    case 2:
                        tX -= r;
                        break;
                    case 3:
                        tX -= r;
                        tY += r;
                        break;
                    case 4:
                        tY += r;
                        break;
                    case 5:
                        tX += r;
                        tY += r;
                        break;
                    case 6:
                        tX += r;
                        break;
                    case 7:
                        tX += r;
                        tY -= r;
                        break;
                }*/
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
                    && map.GetMapData(floor, transform.position
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
}
