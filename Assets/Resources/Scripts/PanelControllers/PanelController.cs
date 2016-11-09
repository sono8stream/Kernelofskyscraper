using UnityEngine;
using System.Collections;

public class PanelController : MonoBehaviour {

    public int direction;
    public bool mikata;
    public bool turnable;
    public int targetNo;
    bool process = false;
    public Sprite s;
    public string description;
    GameObject ef;

    // Use this for initialization
    void Start()
    {
        int spriteNo = mikata ? 0 : 1;
        GetComponent<SpriteRenderer>().sprite 
            = Resources.LoadAll<Sprite>("Sprites/Battle/Panel/panel")[spriteNo];
        ef = transform.FindChild("effect").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// あらかじめ指定されたパネル効果を実行
    /// </summary>
    public void Run(RobotController other)
    {
        switch (targetNo)
        {
            case 0://移動
                Turn(other);
                break;
            case 1:
                break;
        }
    }

    public void Turn(RobotController other)
    {
        RobotController rc = other;
        if (mikata == rc.Mikata && !process)
        {
            process = true;
            /*direction = rc.dire >= 3 ? 0 : rc.dire + 1;
            rc.Turn(direction);
            rc.Zoning();*/
            GetComponent<Turn>().RunPanel(other.gameObject);
            GetComponent<Animator>().SetTrigger("PanelEffect");
        }
    }

    public void End()
    {
        process = false;
    }

    public void Break()
    {
        Destroy(gameObject);
    }
}
