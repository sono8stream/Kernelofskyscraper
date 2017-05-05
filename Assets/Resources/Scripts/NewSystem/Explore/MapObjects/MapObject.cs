using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{

    #region Members
    //public
    public int no;//オブジェクト番号
    public int campNo;
    public int dire;//方向
    public int floor;
    public MapLoader map;
    public bool isVanishing;
    public bool waitVanishing;
    public int viewRange = 1;//視野の大きさ

    [SerializeField]
    protected GameObject breakEffect;

    protected int range = 1;//大きさ
    public int Range { get { return range; } }
    #endregion

    void Awake()
    {
        map = GameObject.Find("Map").GetComponent<MapLoader>();
    }

    // Use this for initialization
    protected void Start()
    {
        no = map.RecObj(this, range);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (isVanishing) { Vanish(); return; }
    }

    public Vector3 DtoV(int direction)
    {
        Vector2 pos = Vector2.zero;
        switch (direction)
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

    protected int VtoD(Vector3 vector)
    {
        if (vector.x != 0)
        {
            return 0 < vector.x ? 1 : 3;
        }
        else
        {
            return 0 < vector.y ? 2 : 0;
        }
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

    public void FlashViewRange()
    {
        Vector3 iniPos = -Vector2.one * (viewRange - viewRange % 2) / 2;
        Vector3 corPos;
        CellData c;
        for (int i = 0; i < viewRange * viewRange; i++)
        {
            corPos = new Vector2(i % viewRange, i / viewRange);
            c = map.GetMapData(floor,
                transform.localPosition + iniPos + corPos);
            if (c != null) { c.tile.SetActive(true); }
        }
        map.VisualizeRoom(floor, transform.localPosition);
    }
}
