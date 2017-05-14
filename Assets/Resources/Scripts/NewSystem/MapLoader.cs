using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapLoader : MonoBehaviour
{
    public TextAsset[] mp_layout, gimmickLayout;//マップ情報を記述したテキスト
    public MapGenerator generator;
    public FloorController flrCon;
    public string[] mapdataDebug;

    [SerializeField]
    bool onTest;
    [SerializeField]
    KernelController kernel;
    [SerializeField]
    GameObject cursorGO;
    [SerializeField]
    GameObject[] mapParts;
    [SerializeField]
    GameObject[] mapGimmicks;

    int mapWidth;
    int mapHeight;
    public int MapWidth { get { return mapWidth; } }
    public int MapHeight { get { return mapHeight; } }
    int floorMargin;//フロア間の距離
    int f, x, y;//階数,x,y座標

    CellData[][,] mapData;//サイズは縦横いずれも奇数推奨
    public CellData[][,] MapData { get { return mapData; } }

    [SerializeField]
    List<MapObject> objs;
    public List<MapObject> Objs { get { return objs; } }

    Texture2D ceilingTexture;
    GameObject[] flrGOs;
    public GameObject[] FloorGOs { get { return flrGOs; } }

    // Use this for initialization
    void Awake()
    {
        floorMargin = 20;
        objs = new List<MapObject>();

        if (generator == null)
        {
            ReadMap();
        }
        else
        {
            generator.InitiateMap();
            mapWidth = generator.MapData[0].GetLength(0);
            mapHeight = generator.MapData[0].GetLength(1);
            InitiateMapData();
            /*for (int i = 0; i < generator.rooms[0].Count; i++)
            {
                Block b = generator.rooms[0][i];
                Debug.Log(new Rect(b.x, b.y, b.w, b.h));
            }*/
            DrawMap(generator.MapGimmickData);
        }
    }

    void Start()
    {
        DebugMapData();
        f = 0;
        x = 0;
        y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space) && generator != null)
        {
            DelMap();
            generator.InitiateMap();
            UpdateMapData();
            DrawMap();
        }*/
        DebugMapData();
    }

    void DebugMapData()
    {
        mapdataDebug = new string[mapHeight];
        for (int i = 0; i < mapdataDebug.Length; i++)//タテループ
        {
            string sub = "";
            for (int j = 0; j < mapWidth; j++)//よこループ
            {
                //string c = mapData[1][j, i].objNo == (int)MapPart.floor ? " " : "■";
                sub += mapData[0][j, i].objNo.ToString();
            }
            mapdataDebug[i] = sub;
        }
    }

    /// <summary>
    /// マップ読み込みメソッド
    /// 戻り値はマップサイズ
    /// </summary>
    /// <returns></returns>
    public Vector2 ReadMap()
    {
        char[] kugiri = { '\r' };
        string[] layoutInfo, gimLayoutInfo;
        string[] eachInfo, gimInfo;
        int[][,] gimmickData = new int[gimmickLayout.Length][,];
        mapData = new CellData[mp_layout.Length][,];

        for (int h = 0; h < mp_layout.Length; h++)
        {
            layoutInfo = mp_layout[h].text.Split(kugiri);
            gimLayoutInfo = gimmickLayout[h].text.Split(kugiri);
            mapHeight = layoutInfo.GetLength(0);

            for (int i = 0; i < layoutInfo.Length; i++)
            {
                eachInfo = layoutInfo[i].Split(',');
                gimInfo = gimLayoutInfo[i].Split(',');

                if (i == 0)//mapdata初期化
                {
                    mapWidth = eachInfo.Length;
                    mapData[h] = new CellData[gimInfo.Length, mapHeight];
                    gimmickData[h] = new int[eachInfo.Length, mapHeight];
                }

                for (int j = 0; j < eachInfo.Length; j++)
                {
                    if (eachInfo[j] != "")
                    {
                        mapData[h][j, i] = new CellData(int.Parse(eachInfo[j]));
                        gimmickData[h][j, i] = int.Parse(gimInfo[j]);
                    }
                }
            }

            //AdjustMapData(mapData[h]);
        }

        DrawMap(gimmickData);
        return new Vector2(mapData[0].GetLength(0), mapData[0].GetLength(1));
    }

    /// <summary>
    /// マップデータをy座標上向きにとる,つまりy座標をひっくり返す
    /// </summary>
    void AdjustMapData(CellData[,] map)
    {
        CellData c1, c2;
        for (int x = 0; x < mapData[0].GetLength(0); x++)//よこループ
        {
            for (int y = 0;
                y < (mapData[0].GetLength(1) - mapData[0].GetLength(1) % 2) / 2; y++)//タテループ
            {//Swap
                c1 = map[x, y];
                c2 = map[x, map.GetLength(1) - y - 1];
                c1.partNo += c2.partNo;
                c2.partNo = c1.partNo - c2.partNo;
                c1.partNo -= c2.partNo;
            }
        }
    }

    void InitiateMapData()
    {
        mapData = new CellData[generator.MapData.Length][,];
        for (int i = 0; i < generator.MapData.Length; i++)
        {
            mapData[i] = new CellData[mapWidth, mapHeight];
            for (int j = 0; j < mapWidth; j++)
            {
                for (int k = 0; k < mapHeight; k++)
                {
                    mapData[i][j, k] = new CellData(generator.MapData[i][j, k]);
                }
            }
        }
    }

    void UpdateMapData()
    {
        for (int i = 0; i < generator.MapData.Length; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                for (int k = 0; k < mapHeight; k++)
                {
                    mapData[i][j, k].Update(generator.MapData[i][j, k]);
                }
            }
        }
    }

    void DrawMap(int[][,] gimmickData)
    {
        Texture2D[] ceilingTextures = GetCeilingTexture(Resources.Load<Sprite>("Sprites/Textures/ceiling"));
        float iniX = -(mapWidth - mapWidth % 2) * 0.5f;
        float iniY = (mapHeight - mapHeight % 2) * 0.5f;

        flrGOs = new GameObject[mapData.Length];

        for (int i = 0; i < mapData.Length; i++)
        {
            GameObject map = new GameObject("Floor" + (i + 1).ToString());
            map.transform.SetParent(transform);
            map.transform.position += Vector3.right * (floorMargin + mapWidth) * i;
            map.AddComponent<AudioSource>();
            map.GetComponent<AudioSource>().maxDistance = 20;

            flrGOs[i] = map;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    SetMapPart(map, i, x, y, iniX, iniY);
                    SetGimmick(map, i, x, y, iniX, iniY, gimmickData);
                }
            }
        }
    }

    void SetMapPart(GameObject mapGO, int floor, int x, int y, float iniX, float iniY)
    {
        //Debug.Log("x:"+x+"y:"+y);
        if (mapData[floor][x, y].partNo < 0 || mapParts.Length <= mapData[floor][x, y].partNo)
        {
            return;
        }
        GameObject g = Instantiate(mapParts[mapData[floor][x, y].partNo], mapGO.transform);
        g.transform.localPosition = new Vector3(iniX + x, iniY - y, 0);
        if (onTest)
        {
            g.SetActive(true);
        }
        switch (mapData[floor][x, y].partNo)
        {
            case (int)MapPart.stairD:
                g.GetComponent<Panel>().command
                    = new Warp(g.transform.localPosition + Vector3.forward * (floor - 1));
                SetPanelData(floor, g.transform.localPosition, g.GetComponent<Panel>());
                break;
            case (int)MapPart.stairU:
                g.GetComponent<Panel>().command
                    = new Warp(g.transform.localPosition + Vector3.forward * (floor + 1));
                SetPanelData(floor, g.transform.localPosition, g.GetComponent<Panel>());
                break;
            case (int)MapPart.kernel:
                Debug.Log("DetectedKernel");
                break;
            case (int)MapPart.fall:
                g.GetComponent<Panel>().command
                    = new Warp(g.transform.localPosition + Vector3.forward * (floor - 1));
                SetPanelData(floor, g.transform.localPosition, g.GetComponent<Panel>());
                break;
        }
        mapData[floor][x, y].tile = g;
    }

    void SetGimmick(GameObject mapGO, int floor, int x, int y, float iniX, float iniY,int[][,] gimmickData)
    {
        GameObject g = null;
        GameObject t = null;
        Panel p;
        int no = gimmickData[floor][x, y];

        switch (no & 0xff)//下位ビット
        {
            case (int)GimmickType.eRecovPanel:
                g = Instantiate(mapGimmicks[0], mapGO.transform);
                p = g.GetComponent<Panel>();
                p.command = new EnergyRecover();
                p.cannotBreak = true;
                p.campNo = (int)CampState.ally;
                break;
            case (int)GimmickType.cRecovPanel:
                g = Instantiate(mapGimmicks[0], mapGO.transform);
                p = g.GetComponent<Panel>();
                p.command = new CapacityRecover();
                p.cannotBreak = true;
                p.campNo = (int)CampState.ally;
                break;
            case (int)GimmickType.destroySwitch:
                g = Instantiate(mapGimmicks[0], mapGO.transform);
                t = Instantiate(mapGimmicks[1], mapGO.transform);
                t.transform.localPosition
                    = new Vector3(iniX + ((no & 0xff0000) >> 16), iniY - ((no & 0xff00) >> 8), -0.01f);
                t.GetComponent<MapObject>().floor = floor;
                p = g.GetComponent<Panel>();
                p.command = new DestroySwitch(t.GetComponent<MapObject>());
                p.cannotBreak = true;
                p.campNo = (int)CampState.ally;

                break;
            case (int)GimmickType.door:

                break;
            case (int)GimmickType.enemy:
                g = GenerateEnemy(mapGimmicks[2], floor);
                Debug.Log(new Vector2(x, y));
                break;
        }

        if (g != null)
        {
            g.transform.localPosition = new Vector3(iniX + x, iniY - y);
            if (g.GetComponent<Panel>() != null)
            {
                g.transform.localPosition = new Vector3(iniX + x, iniY - y, -0.01f);
                mapData[floor][x, y].panel = g.GetComponent<Panel>();
            }
        }
    }

    void DelMap()
    {
        GameObject.Find("MainCamera").transform.SetParent(null);
        cursorGO.transform.SetParent(null);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    Texture2D[] GetCeilingTexture(Sprite ceilingSprite)
    {
        int vSplits = 4;
        int hSplits = 6;
        int masu = ceilingSprite.texture.width / vSplits;
        Texture2D[] textures = new Texture2D[vSplits * hSplits];
        for (int i = 0; i < vSplits * hSplits; i++)
        {
            textures[i] = new Texture2D(masu, masu);
            textures[i].SetPixels(ceilingSprite.texture.GetPixels(i % vSplits * masu,
                (hSplits - 1 - i / vSplits) * masu, masu, masu));
            textures[i].Apply();
        }
        return textures;
    }

    void AdjustCeiling(int floorNo, int x, int y, Transform t, Texture2D[] sprites)//天井のオートマップ編集
    {
        Renderer[] rs = new Renderer[4] {t.FindChild("TopLU").GetComponent<Renderer>(),
        t.FindChild("TopRU").GetComponent<Renderer>(),
        t.FindChild("TopLD").GetComponent<Renderer>(),
        t.FindChild("TopRD").GetComponent<Renderer>()};
        bool[] sur = new bool[9];
        sur[4] = true;//中心
        for (int i = 0; i < sur.Length; i++)//周囲マスデータ
        {
            if (i != 4)
            {
                int xc = i % 3 - 1 + x;
                int yc = i / 3 - 1 + y;
                if (0 <= xc && xc < mapData.GetLength(0)
                    && 0 <= yc && yc < mapData.GetLength(1))
                {
                    sur[i] = mapData[floorNo][xc, yc].partNo == 2;
                }
            }
        }
        int[] surNo = new int[4] {GetSurPoint(sur[0],sur[3],sur[1]),
        GetSurPoint(sur[2],sur[5],sur[1]),
        GetSurPoint(sur[6],sur[3],sur[7]),
        GetSurPoint(sur[8],sur[5],sur[7])};//左上、右上、左下、右下の順に角データ
        int masu = 4;
        /*for (int i = 0; i < surNo.Length; i++)
        {
            int posX = i % 2;
            int posY = i / 2;
            switch(surNo[i])
            {
                case 0://隣接無し
                    Debug.Log(posX + posY * masu);
                    rs[i].material.mainTexture = sprites[posX + posY * masu];
                    break;
                case 1://縦隣接
                    Debug.Log(posX * (masu - 1) + (posY + 3) * masu);
                    rs[i].material.mainTexture = sprites[posX * (masu - 1) + (posY + 3) * masu];
                    break;
                case 2://横隣接
                    Debug.Log(posX + 1 + (posY * 3 + 2) * masu);
                    rs[i].material.mainTexture = sprites[posX + 1 + (posY * 3 + 2) * masu];
                    break;
                case 3://横縦隣接
                    Debug.Log(posX + 2 + posY * masu);
                    rs[i].material.mainTexture = sprites[posX + 2 + posY * masu];
                    break;
                case 4://すべて隣接
                    Debug.Log(posX + 1 + (posY + 3) * masu);
                    rs[i].material.mainTexture = sprites[posX + 1 + (posY + 3) * masu];
                    break;
            }
        }*/
    }

    int GetSurPoint(bool kado, bool faceH, bool faceV)
    {
        int point = 0;
        if (kado && faceH && faceV)
        {
            point = 4;
        }
        else if (faceH && faceV)
        {
            point = 3;
        }
        else if (faceH)
        {
            point = 2;
        }
        else if (faceV)
        {
            point = 1;
        }
        return point;
    }

    public CellData GetMapData(int floorNo, Vector2 pos)
    {
        int x = 0, y = 0;
        PosToMapIndex(pos, ref x, ref y);
        if (InMap(x, y))
        {
            return mapData[floorNo][x, y];
        }
        else
        {
            return null;
        }
    }

    public void SetObjData(int floorNo, Vector2 pos, int objNo)
    {
        int x = 0, y = 0;
        PosToMapIndex(pos, ref x, ref y);
        if (InMap(x, y))
        { mapData[floorNo][x, y].objNo = objNo; }
    }

    public void SetPanelData(int floorNo, Vector2 pos, Panel panel)
    {
        int x = 0, y = 0;
        PosToMapIndex(pos, ref x, ref y);
        if (InMap(x, y))
        { mapData[floorNo][x, y].panel = panel; }
    }

    public void SetTileData(int floorNo, Vector2 pos, bool on)
    {
        int x = 0, y = 0;
        PosToMapIndex(pos, ref x, ref y);
        if (InMap(x, y))
        { mapData[floorNo][x, y].tile.SetActive(on); }
    }

    void PosToMapIndex(Vector2 pos, ref int x, ref int y)
    {
        x = Mathf.RoundToInt(pos.x) + (mapWidth - mapWidth % 2) / 2;
        y = -Mathf.RoundToInt(pos.y) + (mapHeight - mapHeight % 2) / 2;
    }

    bool InMap(int x, int y)
    {
        return 0 <= x && x < mapWidth && 0 <= y && y < mapHeight;
    }

    public int RecObj(MapObject obj, int range)//オブジェクトに番号をセットする用
    {
        objs.Add(obj);
        Vector3 iniPos = -Vector2.one * (range - range % 2) / 2;
        Vector3 corPos;
        obj.floor = obj.transform.parent.name[5] - '1';
        Debug.Log(obj.floor);

        for (int i = 0; i < range * range; i++)
        {
            corPos = new Vector3(i % range, i / range);
            SetObjData(obj.floor, obj.transform.localPosition + iniPos + corPos, objs.Count - 1);
        }
        return objs.Count - 1;
    }

    public void DelObjNo(int no)//オブジェクト番号をつぶして更新
    {
        for (int i = no + 1; i < objs.Count; i++)
        {
            SetObjData(objs[i].floor, objs[i].transform.localPosition, i - 1);
            objs[i].no--;
        }

        SetObjData(objs[no].floor, objs[no].transform.localPosition, (int)ObjType.can);
        objs.RemoveAt(no);
    }

    public void VisualizeRoom(int floor, Vector2 pos)//部屋を照らす
    {
        Block b = GetPosRoom(floor, pos);
        if (b == null) { return; }
        //Vector2[] rPos = GetRoomRobots(b);
        float iniX = -(mapWidth - mapWidth % 2) * 0.5f;
        float iniY = (mapHeight - mapHeight % 2) * 0.5f;
        int robots
            = (int)Random.Range(Mathf.Round(generator.enemyRates[floor]), generator.enemyRates[floor]);
        generator.rooms[floor].Remove(b);

        for (int i = 0; i < (b.rW + 2) * (b.rH + 2); i++)
        {
            mapData[floor][b.rX - 1 + i % (b.rW + 2), b.rY - 1 + i / (b.rW + 2)].tile.SetActive(true);
        }
        /*for (int i = 0; i < rPos.Length; i++)
        {
            GameObject g = GenerateEnemy(mapGimmicks[2], floor);
            g.transform.localPosition = new Vector3(iniX + rPos[i].x, iniY - rPos[i].y);
            //robots[i].transform.FindChild("mod").gameObject.SetActive(true);
        }*/
        Debug.Log("this index is " + generator.rooms[floor].IndexOf(b));
        Debug.Log("kRoom is " + generator.KroomIndex);
        if (floor == 0 && generator.rooms[floor].IndexOf(b) == generator.KroomIndex) { return; }

        for (int i = 0; i < robots; i++)
        {
            pos = new Vector2(b.rX + Random.Range(0, b.rW - 1),
                 b.rY + Random.Range(0, b.rH - 1));
            if (generator.CheckObject(b, pos))
            {
                GameObject enemy = GenerateEnemy(mapGimmicks[2], floor);
                enemy.transform.localPosition
                    = new Vector3(iniX + pos.x, iniY - pos.y);
            }
        }
    }

    public Block GetPosRoom(int floor, Vector2 pos)
    {
        if (generator == null) { return null; }
        Block b;
        int x = 0, y = 0;
        PosToMapIndex(pos, ref x, ref y);
        for (int i = 0; i < generator.rooms[floor].Count; i++)
        {
            b = generator.rooms[floor][i];
            if (b.rX <= x && x <= b.rX + b.rW - 1 && b.rY <= y && y <= b.rY + b.rH - 1)
            {
                //generator.rooms[floor].RemoveAt(i);
                return b;
            }
        }
        return null;
    }

    public Vector2[] GetRoomRobots(Block b)
    {
        List<Vector2> rPos = new List<Vector2>();
        int no;

        for (int i = 0; i < b.sPos.Count; i++)
        {
            no = generator.MapGimmickData[b.floorNo][(int)b.sPos[i].x, (int)b.sPos[i].y];
            if (no == (int)GimmickType.enemy)
            {
                rPos.Add(b.sPos[i]);
            }
        }
        return rPos.ToArray();
    }

    public GameObject GenerateEnemy(GameObject robot, int floor)
    {
        GameObject g = Instantiate(robot);
        RobotController rc = g.GetComponent<RobotController>();
        rc.robot = (Robot)UserData.instance.robotRecipe[0].DeepCopy();
        rc.robot.Initiate();
        rc.robot.HP -= 50;
        rc.floor = floor;
        rc.canMove = true;
        rc.specialCom = new Slash();

        g.transform.SetParent(flrGOs[floor].transform);
        g.transform.localScale = Vector3.one;

        return g;
    }
}

/// <summary>
/// マス目情報
/// </summary>
public class CellData
{
    public int partNo;//マスの構造、床、階段など
    public int objNo;//MapObject番号、存在しなければ0
    public Panel panel;//パネル番号、存在しなければ-1
    public GameObject tile;

    public CellData(int partNo, int objNo = (int)ObjType.can)
    {
        this.partNo = partNo;
        this.objNo = objNo;
        panel = null;
    }

    public void Update(int partNo)
    {
        this.partNo = partNo;
        objNo = (int)ObjType.can;
        panel = null;
    }
}

public enum MapPart
{
    none = -1, floor, wall, stairD, stairU, kernel, fall
}

public enum GimmickType
{
    none = 0, eRecovPanel, cRecovPanel, destroySwitch, door, enemy
}

public enum ObjType
{
    cannot = -2, can
}
