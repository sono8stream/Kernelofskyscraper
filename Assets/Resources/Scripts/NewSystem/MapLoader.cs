using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLoader : MonoBehaviour
{

    public TextAsset mp_layout;//マップ情報を記述したテキスト
    public MapGenerator generator;
    int mapWidth;
    int mapHeight;
    int[][,] mapdata;//サイズは縦横いずれも奇数推奨
    List<MapObject> objs;
    int[,] objData;//マップとオブジェクトの対応値
    public string[] mapdataDebug;
    public Sprite mapchips;
    Texture2D ceilingTexture;
    const int MASU = 32;
    [SerializeField]
    bool onTest;
    GameObject[][] tiles;
    int f,x, y;

    // Use this for initialization
    void Awake()
    {
        //ReadMap();
        if (generator != null)
        {
            generator.InitiateMap();
            mapdata = generator.Mapdata;
        }
        else
        {
            ReadMap();
        }
        mapWidth = mapdata[0].GetLength(0);
        mapHeight = mapdata[0].GetLength(1);
        tiles = new GameObject[mapdata.Length][];
        DrawMap();
        objs = new List<MapObject>();
        objData = new int[mapWidth, mapHeight];
        /*if ((mapdata.GetLength(0) & 1) == 0)//x位置補正
        {
            transform.position += Vector3.left * 0.5f;
        }
        if ((mapdata.GetLength(1) & 1) == 0)//x位置補正
        {
            transform.position += Vector3.up * 0.5f;
        }*/
    }

    void Start()
    {
        mapdataDebug = new string[mapHeight];
        for (int i = 0; i < mapdataDebug.Length; i++)//タテループ
        {
            string sub = "";
            for (int j = 0; j < mapWidth; j++)//よこループ
            {
                string c = mapdata[0][j, i] == 1 ? " " : "■";
                sub += mapdata[0][j, i].ToString();
            }
            mapdataDebug[i] = sub;
        }
        f = 0;
        x = 0;
        y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && generator != null)
        {
            DelMap();
            generator.InitiateMap();
            mapdata = generator.Mapdata;
            DebugMapdata();
            DrawMap();
        }
        if (!onTest&&f < mapdata.Length)
        {
            tiles[f][x + mapWidth * y].SetActive(true);
            x++;
            if (x == mapWidth)
            {
                x = 0;
                y++;
                if(y==mapHeight)
                {
                    y = 0;
                    f++;
                }
            }
        }
    }

    void DebugMapdata()
    {
        mapdataDebug = new string[mapHeight];
        for (int i = 0; i < mapdataDebug.Length; i++)//タテループ
        {
            string sub = "";
            for (int j = 0; j < mapWidth; j++)//よこループ
            {
                string c = mapdata[0][j, i] == 1 ? " " : "■";
                sub += mapdata[0][j, i].ToString();
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
        string[] layoutInfo = mp_layout.text.Split(kugiri);
        mapHeight = layoutInfo.GetLength(0);

        string[] eachInfo;
        for (int i = 0; i < layoutInfo.Length; i++)
        {
            eachInfo = layoutInfo[i].Split(',');
            if (i == 0)//mapdata初期化
            {
                mapWidth = eachInfo.Length;
                mapdata = new int[1][,] { new int[eachInfo.Length, layoutInfo.Length] };
            }
            for (int j = 0; j < eachInfo.Length; j++)
            {
                if (eachInfo[j] != "")
                {
                    mapdata[0][j, i] = int.Parse(eachInfo[j]);
                }
            }
        }
        AdjustMapData(mapdata[0]);
        return new Vector2(mapdata[0].GetLength(0), mapdata[0].GetLength(1));
    }

    /// <summary>
    /// マップデータをy座標上向きにとる,つまりy座標をひっくり返す
    /// </summary>
    void AdjustMapData(int[,] map)
    {
        for (int x = 0; x < mapdata[0].GetLength(0); x++)//よこループ
        {
            for (int y = 0;
                y < (mapdata[0].GetLength(1) - mapdata[0].GetLength(1) % 2) / 2; y++)//タテループ
            {
                map[x, y] += map[x, map.GetLength(1) - y - 1];
                map[x, map.GetLength(1) - y - 1] = map[x, y] - map[x, map.GetLength(1) - y - 1];
                map[x, y] -= map[x, map.GetLength(1) - y - 1];
            }
        }
    }

    void DrawMap()
    {
        string floorPath = "Prefabs/New3D/Floor";
        string wallPath = "Prefabs/New3D/Wall";
        string stairUPath = "Prefabs/New3D/StairU";
        string stairDPath = "Prefabs/New3D/StairD";
        float iniX = -(mapWidth - mapWidth % 2) * 0.5f;
        float iniY = (mapHeight - mapHeight % 2) * 0.5f;
        tiles = new GameObject[mapdata.Length][];
        Texture2D[] ceilingTextures = GetCeilingTexture(Resources.Load<Sprite>("Sprites/Textures/ceiling"));
        Debug.Log(ceilingTextures.Length);
        for (int i = 0; i < mapdata.Length; i++)
        {
            tiles[i] = new GameObject[mapWidth * mapHeight];
            GameObject map = new GameObject("Floor" + (i+1).ToString());
            map.transform.SetParent(transform);
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    tiles[i][x + mapWidth * y] = null;
                    switch (mapdata[i][x, y])
                    {
                        case 1://floor
                            tiles[i][x + mapWidth * y] = Instantiate(Resources.Load<GameObject>(floorPath),
                                map.transform);
                            break;
                        case 2://wall
                            tiles[i][x + mapWidth * y] = Instantiate(Resources.Load<GameObject>(wallPath),
                                map.transform);
                            //AdjustCeiling(x, y, obj.transform, ceilingTextures);
                            break;
                        case 3://下階段
                            tiles[i][x + mapWidth * y] = Instantiate(Resources.Load<GameObject>(stairDPath),
                                map.transform);
                            break;
                        case 4://上階段
                            tiles[i][x + mapWidth * y] = Instantiate(Resources.Load<GameObject>(stairUPath),
                                map.transform);
                            break;
                    }
                    if (tiles[i][x + mapWidth * y] != null)
                    {
                        tiles[i][x + mapWidth * y].transform.position = new Vector3(iniX + x, iniY - y, -i * 2.4f);
                        if (onTest)
                        {
                            tiles[i][x + mapWidth * y].SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void DelMap()
    {
        foreach(Transform child in transform)
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
                (hSplits-1 - i / vSplits) * masu, masu, masu));
            textures[i].Apply();
        }
        return textures;
    }

    void AdjustCeiling(int floorNo,int x,int y,Transform t,Texture2D[] sprites)//天井のオートマップ編集
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
                if (0 <= xc && xc < mapdata.GetLength(0)
                    && 0 <= yc && yc < mapdata.GetLength(1))
                {
                    sur[i] = mapdata[floorNo][xc, yc] == 2;
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

    int GetSurPoint(bool kado,bool faceH,bool faceV)
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

    public int GetMapData(int floorNo,Vector2 pos)
    {
        int x = PosToIndex(pos.x, mapdata[floorNo].GetLength(0), false);
        int y = PosToIndex(pos.y, mapdata[floorNo].GetLength(1), true);
        if (0 <= x && x < mapdata[floorNo].GetLength(0)
            && 0 <= y && y < mapdata[floorNo].GetLength(1))
        {
            return mapdata[floorNo][x, y];
        }
        else
        {
            return -1;
        }
    }

    public void SetMapData(int floorNo,Vector2 pos, int value)
    {
        int x = PosToIndex(pos.x, mapdata[floorNo].GetLength(0),false);
        int y = PosToIndex(pos.y, mapdata[floorNo].GetLength(1),true);
        if (0 <= x && x < mapdata[floorNo].GetLength(0)
            && 0 <= y && y < mapdata[floorNo].GetLength(1))
        {
            mapdata[floorNo][x, y] = value;
        }
    }

    public int GetObjData(int floorNo, Vector2 pos)
    {
        int x = PosToIndex(pos.x, mapdata[floorNo].GetLength(0),false);
        int y = PosToIndex(pos.y, mapdata[floorNo].GetLength(1),true);
        if (0 <= x && x < mapdata[floorNo].GetLength(0)
            && 0 <= y && y < mapdata[floorNo].GetLength(1))
        {
            return objData[x, y];
        }
        else
        {
            return -1;
        }
    }

    public void SetObjData(int floorNo,Vector2 pos, int value)
    {
        int x = PosToIndex(pos.x, mapdata[floorNo].GetLength(0), false);
        int y = PosToIndex(pos.y, mapdata[floorNo].GetLength(1), true);
        if (0 <= x && x < mapdata[floorNo].GetLength(0)
            && 0 <= y && y < mapdata[floorNo].GetLength(1))
        {
            objData[x, y] = value;
        }
    }

    int PosToIndex(float v, int length,bool reverse)
    {
        int val = Mathf.RoundToInt(v);
        val *= reverse ? -1 : 1;
        return val + (length - length % 2) / 2;
    }

    public int RecObj(MapObject obj)//オブジェクトに番号をセットする用
    {
        objs.Add(obj);
        SetObjData(obj.Floor,obj.transform.position, objs.Count);
        return objs.Count;
    }

    public void DelObjNo(int no)//オブジェクト番号をつぶして更新
    {
        for (int i = no; i < objs.Count; i++)
        {
            for (int x = 0; x < objData.GetLength(0); x++)
            {
                for (int y = 0; y < objData.GetLength(1); y++)
                {
                    if (objData[x, y] == i)
                    {
                        objData[x, y] = i == no ? 0 : i - 1;
                    }
                }
            }
        }
        objs.RemoveAt(no);
    }
}
