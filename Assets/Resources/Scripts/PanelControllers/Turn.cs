using UnityEngine;
using System.Collections;
using System;

public class Turn : MonoBehaviour
{
    [SerializeField]
    int direction;
    bool process = false;
    public Sprite s;
    GameObject ef;
    [SerializeField]
    bool reverse, uTurn, direct;//右折、uターン方向を直接向くか
    // Use this for initialization
    void Start()
    {
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RunPanel(GameObject other)
    {
        RobotController rc = other.GetComponent<RobotController>();
        if(!direct)
        {
            if (reverse)
            {
                direction = rc.dire == 0 ? 3 : rc.dire - 1;
            }
            else
            {
                direction = rc.dire >= 3 ? 0 : rc.dire + 1;
            }
            if (uTurn)
            {
                direction = (rc.dire + 2) % 4;
                Debug.Log("Uturn");
            }
        }
        Debug.Log("方向" + direction);
        rc.Turn(direction);
        //rc.Zoning();
    }
}
