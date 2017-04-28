using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour {

    #region Members
    //public
    public int no;//オブジェクト番号
    public int campNo;
    public int dire;//方向
    public int floor;
    public MapLoader map;
    public bool isVanishing;
    public bool waitVanishing;

    protected int range = 1;//大きさ
    public int Range { get { return range; } }
    protected int viewRange;//視野の大きさ
    public int ViewRange { get { return viewRange; } }
    #endregion

    void Awake()
    {
        map = GameObject.Find("Map").GetComponent<MapLoader>();
    }

    // Use this for initialization
    protected void Start()
    {
        no = map.RecObj(this, range);
        viewRange = 1;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (isVanishing) { Vanish(); return; }
    }

    protected Vector3 DtoV(int direction)
        {
        Vector2 pos = Vector2.zero;
        switch(direction)
        {
            case 0:
                pos = Vector2.down;
                break;
            case 1:
                pos = Vector2.right;
                break;
            case 2:
                pos = Vector2.up;
                break;
            case 3:
                pos = Vector2.left;
                break;
        }
        return pos;
    }
    
    protected void Vanish()
    {
        transform.FindChild("mod").localScale *= 0.5f;
        if (transform.FindChild("mod").localScale.x < 0.01)
        {
            map.DelObjNo(no);
            Destroy(gameObject);
        }
    }
}
