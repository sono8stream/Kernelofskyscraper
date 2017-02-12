using UnityEngine;
using System.Collections;

public class MapLoader : MonoBehaviour
{

    public TextAsset mp_layout;//マップ情報を記述したテキスト
    Texture2D MapImage;
    int mapWidth;
    int mapHeight;
    public int[,] mapdata;//サイズは縦横いずれも奇数推奨
    public string[] mapdataDebug;
    public Sprite mapchips;
    Sprite map;
    const int MASU = 32;

    // Use this for initialization
    void Awake()
    {
        ReadMap();

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
        mapdataDebug = new string[mapdata.GetLength(1)];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mapdataDebug.GetLength(0); i++)//タテループ
        {
            string sub = "";
            for (int j = 0; j < mapdata.GetLength(0); j++)//よこループ
            {
                sub += mapdata[j, mapdata.GetLength(1) - i - 1].ToString() + ",";
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
            //layoutInfo[i]=layoutInfo[i].Remove(layoutInfo[i].Length - 1);
            eachInfo = layoutInfo[i].Split(',');
            if (i == 0)//mapdata初期化
            {
                mapWidth = eachInfo.Length;
                mapdata = new int[eachInfo.Length, layoutInfo.Length];
            }
            for (int j = 0; j < eachInfo.Length; j++)
            {
                if (eachInfo[j] != "")
                {
                    mapdata[j, i] = int.Parse(eachInfo[j]);
                }
            }
        }
        //AdjustMapData();
        DrawMap();
        return new Vector2(mapdata.GetLength(0), mapdata.GetLength(1));
    }

    /// <summary>
    /// マップデータをy座標上向きにとる,つまりy座標をひっくり返す
    /// </summary>
    void AdjustMapData()
    {
        for (int x = 0; x < mapdata.GetLength(0); x++)//よこループ
        {
            for (int y = 0; 
                y < (mapdata.GetLength(1)-mapdata.GetLength(1)%2) / 2; y++)//タテループ
            {
                mapdata[x, y] += mapdata[x, mapdata.GetLength(1) - y - 1];
                mapdata[x, mapdata.GetLength(1) - y - 1] = mapdata[x, y]
                    - mapdata[x, mapdata.GetLength(1) - y - 1];
                mapdata[x, y] -= mapdata[x, mapdata.GetLength(1) - y - 1];
            }
        }
    }

    void DrawMap()
    {
        string floorPath = "Prefabs/New3D/Floor";
        string wallPath = "Prefabs/New3D/Wall";
        float iniX = -(mapWidth - 1) * 0.5f;
        float iniY = (mapWidth - 1) * 0.5f;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                GameObject obj = null;
                switch (mapdata[x, y])
                {
                    case 1://floor
                        obj = Instantiate(Resources.Load<GameObject>(floorPath),
                            transform);
                        break;
                    case 2://wall
                        obj = Instantiate(Resources.Load<GameObject>(wallPath),
                            transform);
                        break;
                }
                if (obj != null)
                {
                    obj.transform.position = new Vector2(iniX + x, iniY - y);
                }
            }
        }
    }

    public int GetMapData(Vector2 pos)
    {
        int x = PosToIndex(pos.x, mapdata.GetLength(0));
        int y = PosToIndex(pos.y, mapdata.GetLength(1));
        return mapdata[x, y];
    }

    public void SetMapData(Vector2 pos, int value)
    {
        int x = PosToIndex(pos.x, mapdata.GetLength(0));
        int y = PosToIndex(pos.y, mapdata.GetLength(1));
        mapdata[x, y] = value;
    }

    int PosToIndex(float v, int length)
    {
        return Mathf.RoundToInt(v) + (length - length % 2) / 2;
    }
}
