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
    int waitCo;
    int waitLim;

    // Use this for initialization
    new void Start()
    {
        base.Start();
        comCX = 0;
        comCY = 0;
        for (int y = 0; y < robot.head.ComList.GetLength(0); y++)
        {
            for (int x = 0; x < robot.head.ComList.GetLength(1); x++)
            {
                if (robot.head.ComList[y, x] == -2)
                {
                    comCX = x;
                    comCY = y;
                    break;
                }
            }
        }
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
        comNo = -1;
        for (int r = 1; r < robot.head.ComList.GetLength(0); r++)
        {
            for (int d = 0; d < 8; d++)
            {
                Vector2 pos = Vector2.zero;
                int tX = comCX;
                int tY = comCY;
                switch (d % 4)//dire=0が上
                {
                    case 0:
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
                }
                int ttX = tX - comCX;
                int ttY = tY - comCY;
                switch (dire)//方向で回転させる
                {
                    case 1:
                        ttX -= ttY;
                        ttY += ttX;
                        ttX -= ttY;
                        break;
                    case 2:
                        ttX *= -1;
                        ttY *= -1;
                        break;
                    case 3:
                        ttX += ttY;
                        ttY -= ttX;
                        ttX += ttY;
                        break;
                }
                if (0 <= tX && tX < robot.head.ComList.GetLength(1)
                    && 0 <= tY && tY < robot.head.ComList.GetLength(0)
                    && 0 <= robot.head.ComList[tY, tX]
                    && map.GetMapData(floor,transform.position
                    + new Vector3(ttX,ttY,0)) == 2)
                {
                    comNo = robot.head.ComList[tY, tX];
                    break;
                }
            }
            if (comNo != -1)
            {
                break;
            }
        }
        if(comNo==-1)
        {
            comNo = 0;
        }
        transform.FindChild("Command").FindChild("Icon")
            .GetComponent<SpriteRenderer>().sprite = robot.Command[comNo].sprite;
    }
}
