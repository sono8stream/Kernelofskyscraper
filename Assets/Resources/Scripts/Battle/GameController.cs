using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable
public class GameController : MonoBehaviour {

    public GameObject Robot_origin;//ロボの基本プレハブ
    public GameObject Panel_origin;//パネルの"
    public const int MASU = 32;//マスの大きさ
    private int[,] m_robo_data;
    private int[,] m_panel_data;
    private List<GameObject> robots;
    private List<GameObject> panels;

    // Use this for initialization
    void Start()
    {
        Vector2 t = Vector2.zero;//マップサイズ
        //マップ読み込み
        GameObject[] mplayer = GameObject.FindGameObjectsWithTag("MapLayer");
        foreach(GameObject i in mplayer)
        {
            t = i.GetComponent<MapLoader>().ReadMap();
        }
        m_robo_data = new int[(int)t.x, (int)t.y];
        m_panel_data = new int[(int)t.x, (int)t.y];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetRobotNumber(Vector3 pos)
    {
        //m_robo_data[(int)pos.x/MASU,(int)pos.y/MASU]=
    }
    public void GetRobotNumber(Vector3 pos)
    {

    }

    public void SetPanelNumber(Vector3 pos)
    {

    }
    public void GetPanelNumber(Vector3 pos)
    {

    }

    public bool EndGame()
    {
        int rbCount = 0;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Robot"))
        {
            if(g.GetComponent<RobotController>().Mikata)
            {
                return true;
            }
        }
        return false;
    }
}
