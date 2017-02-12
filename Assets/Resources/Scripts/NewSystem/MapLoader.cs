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
                        AdjustCeiling(x, y, obj);
                        break;
                }
                if (obj != null)
                {
                    obj.transform.position = new Vector2(iniX + x, iniY - y);
                }
            }
        }
    }

    void AdjustCeiling(int x,int y,GameObject g)//天井のオートマップ編集
    {
        Renderer r = g.transform.Find("Top").GetComponent<Renderer>();
        Texture2D tOrigin = (Texture2D)r.material.mainTexture;
        int size = tOrigin.width / 2;
        Texture2D t = new Texture2D(size, size);
        bool[] sur = new bool[9];
        sur[4] = true;
        for (int i = 0; i < sur.Length; i++)//周囲マスデータ
        {
            if (i != 4)
            {
                int xc = i % 3 - 1 + x;
                int yc = i / 3 - 1 + y;
                if (0 <= xc && xc < mapdata.GetLength(0)
                    && 0 <= yc && yc < mapdata.GetLength(1))
                {
                    sur[i] = mapdata[xc, yc] == 2;
                }
            }
        }
        int[] surNo = new int[4] {GetSurPoint(sur[0],sur[3],sur[1]),
        GetSurPoint(sur[2],sur[5],sur[1]),
        GetSurPoint(sur[6],sur[3],sur[7]),
        GetSurPoint(sur[8],sur[5],sur[7])};//左上、右上、左下、右下の順に角データ
        int masu = size / 2;
        for (int i = 0; i < surNo.Length; i++)
        {
            Color[] c = new Color[0];
            int posX = i % 2 * masu;
            int posY = i / 2 * masu;
            switch(surNo[i])
            {
                case 0:
                    c = tOrigin.GetPixels(posX, masu * 5 - posY, masu, masu);
                    break;
                case 1:
                    c = tOrigin.GetPixels(posX * 3, masu * 2 - posY, masu, masu);
                    break;
                case 2:
                    c = tOrigin.GetPixels(masu + posX, masu * 3 - posY * 3,
                        masu, masu);
                    break;
                case 3:
                    c = tOrigin.GetPixels(masu * 2 + posX, masu * 5 - posY,
                        masu, masu);
                    break;
                case 4:
                    c = tOrigin.GetPixels(masu + posX, masu * 2 - posY, masu, masu);
                    break;
            }
            t.SetPixels(posX, masu - posY, masu, masu, c);
        }
        t.Apply();
        r.material.mainTexture = t;
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
