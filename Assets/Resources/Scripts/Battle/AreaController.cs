using UnityEngine;
using System.Collections;

public class AreaController : MonoBehaviour
{
    int at;//攻撃力
    bool mikata;
    public bool Mikata
    {
        get { return mikata; }
        set { mikata = value; }
    }
    //TerritoryController t;
    // Use this for initialization
    void Start()
    {
        //t = GameObject.Find("Territory").GetComponent<TerritoryController>();
        at = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*void OnTriggerStay2D(Collider2D other)
    {
        RobotController rc = other.GetComponent<RobotController>();
        if (other.tag == "Robot" && rc.Mikata != mikata&&!rc.Move)
        {
            t.trdata[t.w / 2 + (int)transform.position.x, t.h / 2 + (int)transform.position.y] = -1;
            rc.Damage(at);
            Destroy(gameObject);
        }
    }*/

    public void Vanish(GameObject other)
    {
        //t.trdata[t.w / 2 + (int)transform.position.x, t.h / 2 + (int)transform.position.y] = -1;
        other.GetComponent<RobotController>().Damage(at);
        //Destroy(gameObject);
    }
}
