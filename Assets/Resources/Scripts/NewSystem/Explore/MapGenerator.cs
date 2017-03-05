using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップを自動生成するクラスです。
/// </summary>
public class MapGenerator : MonoBehaviour
{
    SpriteRenderer sr;
    int[,] mapdata;
    public int[,] Mapdata { get { return mapdata; } }
    [SerializeField]
    GameObject wireH, wireV;
    [SerializeField]
    int width, height;
    const int MASU = 32;
    int stX, stY;//初期座標
    int minRectSize = 7;//部屋サイズ最小値
    int roomLim = 4;//最大部屋数基準+-5
    int roomRan = 3;
    int roomCo = 1;
    struct Block
    {
        public int x, y, w, h;
        public Block(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
    List<Block> blocks = new List<Block>();//区切りリスト
    List<Block> rooms = new List<Block>();//完全に区分けられた部屋リスト


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitiateMap()
    {
        mapdata = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                mapdata[x, y] = 0;
            }
        }
        blocks.Add(new Block(0, 0, width, height));
        roomLim += Random.Range(0, roomRan);
        SplitMap();
        rooms.AddRange(blocks);
        for (int i = 0; i < rooms.Count; i++)
        {
            MakeRoom(i);
        }
        MakeWall();
        //mapdata[stX, stY] = 1;//初期位置に道
        //DrawWire();
    }

    //自動生成アルゴリズム
    void AutoMapping(int rmCo/*部屋数*/, int x, int y)
    {
        /*//まずは単純通路のみ
        //方向取得
        switch (Random.Range(0, 4))
        {
            case 0:
                x++;
                break;
            case 1:
                y++;
                break;
            case 2:
                x--;
                break;
            case 3:
                y--;
                break;
        }*/
    }

    void SplitMap()
    {
        int blockNo;
        do
        {
            blockNo = Random.Range(0, blocks.Count);
            if (blocks[blockNo].w <= minRectSize * 2
                && blocks[blockNo].h <= minRectSize * 2)
            {
                rooms.Add(blocks[blockNo]);
                blocks.RemoveAt(blockNo);
                if (blocks.Count == 0)
                {
                    return;
                }
                blockNo = -1;
            }
        }
        while (blockNo == -1);//ブロック選択
        bool vertical;
        if (Random.value < 0.5f)
        {
            vertical = minRectSize * 2 < blocks[blockNo].w;
        }
        else
        {
            vertical = !(minRectSize * 2 < blocks[blockNo].h);
        }
        int splitX, splitY;
        if (vertical)
        {
            splitX = Random.Range(blocks[blockNo].x + minRectSize,
                blocks[blockNo].w + blocks[blockNo].x - minRectSize);
            splitY = blocks[blockNo].y;
        }
        else
        {
            splitX = blocks[blockNo].x;
            splitY = Random.Range(blocks[blockNo].y + minRectSize,
                blocks[blockNo].h + blocks[blockNo].y - minRectSize);
        }
        SplitBlock(blockNo, splitX, splitY, vertical);
        if (roomCo < roomLim)
        {
            SplitMap();
        }
    }

    void SplitBlock(int blockNo,int splitX,int splitY,bool vertical)
    {
        /*Debug.Log("Block:" + blocks[blockNo].x.ToString()
            + blocks[blockNo].y.ToString()
            + blocks[blockNo].w.ToString() + blocks[blockNo].h.ToString());
        Debug.Log("Split:"+splitX.ToString() + splitY.ToString());*/
        int w1, w2, h1, h2;
        w1 = blocks[blockNo].x - splitX;
        if (vertical)
        {
            for (int i = 0; i < blocks[blockNo].h; i++)
            {
                mapdata[splitX, splitY + i] = -1;
                //Debug.Log(new Vector2(splitX, splitY + i));
            }
            w1 = splitX - blocks[blockNo].x;
            h1 = blocks[blockNo].h;
            splitX++;
            w2 = blocks[blockNo].x + blocks[blockNo].w - splitX;
            h2 = h1;
        }
        else
        {
            for (int i = 0; i < blocks[blockNo].w; i++)
            {
                //Debug.Log(new Vector2(splitX + i, splitY));
                mapdata[splitX + i, splitY] = -1;
            }
            w1 = blocks[blockNo].w;
            h1 = splitY - blocks[blockNo].y;
            splitY++;
            w2 = w1;
            h2 = blocks[blockNo].y + blocks[blockNo].h - splitY;
        }
        blocks.Insert(blockNo, 
            new Block(blocks[blockNo].x, blocks[blockNo].y, w1, h1));
        //Debug.Log(blocks[blockNo].w.ToString() + blocks[blockNo].h.ToString());
        blocks.RemoveAt(blockNo + 1);//旧
        blocks.Insert(blockNo + 1, new Block(splitX, splitY, w2, h2));
        roomCo++;
    }

    void MakeRoom(int blockNo)
    {
        int w = Random.Range(minRectSize - 2, rooms[blockNo].w - 1);
        int h = Random.Range(minRectSize - 2, rooms[blockNo].h - 1);
        int x = rooms[blockNo].x + Random.Range(1, rooms[blockNo].w - w);
        int y = rooms[blockNo].y + Random.Range(1, rooms[blockNo].h - h);
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Debug.Log(new Vector2(x + i, y + j));
                mapdata[x + i, y + j] = 1;//床にする
            }
        }
    }

    void MakeWall()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapdata[x, y] != 1)
                {
                    mapdata[x, y] = 2;//壁にする
                }
            }
        }
    }

    void DrawWire()
    {
        float iniX = -(width - width % 2) * 0.5f;
        float iniY = (height - height % 2) * 0.5f;
        for (int x = 0; x < width - 1; x++)
        {
            GameObject w = Instantiate(wireV, transform);
            w.transform.position = new Vector3(iniX + x + 0.5f, 0, 0.1f);
            w.transform.localScale = new Vector3(0.01f, 1, height * 0.1f);
        }
        for (int y = 0; y < height - 1; y++)
        {
            GameObject w = Instantiate(wireH, transform);
            w.transform.position = new Vector3(0, iniY - y - 0.5f, 0.1f);
            w.transform.localScale = new Vector3(width * 0.1f, 1, 0.01f);
        }
    }
}
