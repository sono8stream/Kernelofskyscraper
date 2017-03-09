using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップを自動生成するクラスです。
/// </summary>
public class MapGenerator : MonoBehaviour
{
    SpriteRenderer sr;
    int[][,] mapdata;
    public int[][,] Mapdata { get { return mapdata; } }
    [SerializeField]
    GameObject wireH, wireV;
    [SerializeField]
    int width, height;
    const int MASU = 32;
    int stX, stY;//初期座標
    int minRectSize = 7;//ブロックサイズ最小値,実際部屋サイズは-2
    int roomLim = 4;//最大部屋数基準+3
    int roomRan = 3;
    int roomCo = 1;
    int lineSize = 2;
    int floors = 3;//3階立て
    class Block
    {
        public int x, y, w, h;
        public int rX, rY, rW, rH;//部屋Rect
        public List<int> adjNos;//隣接ブロックの番号
        public List<int> adjDire;//隣接方向 down=0,right,up,left
        public Block(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            adjNos = new List<int>();
            adjDire = new List<int>();
        }
        public void Shrink(int newW, int newH)
        {
            w = newW;
            h = newH;
        }
        public void SetRoom(int rX, int rY, int rW, int rH)
        {
            this.rX = rX;
            this.rY = rY;
            this.rW = rW;
            this.rH = rH;
        }
    }
    List<Block> blocks;//区切りリスト
    List<Block> rooms;//完全に区分けられた部屋リスト


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
        mapdata = new int[floors][,];
        for (int i = 0; i < floors; i++)
        {
            mapdata[i] = new int[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mapdata[i][x, y] = 0;
                }
            }
        }
        blocks = new List<Block>();
        rooms = new List<Block>();
        blocks.Add(new Block(0, 0, width, height));
        roomLim = 4 + Random.Range(0, roomRan + 1);
        roomCo = 1;
        SplitMap();
        rooms.AddRange(blocks);
        for (int i = 0; i < rooms.Count; i++)
        {
            MakeRoom(i);
        }
        CheckAdjacent();
        MakeAllAisles();
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
            if (blocks[blockNo].w < minRectSize * 2+lineSize
                && blocks[blockNo].h < minRectSize * 2+lineSize)
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
            vertical = minRectSize * 2+lineSize <= blocks[blockNo].w;
        }
        else
        {
            vertical = !(minRectSize * 2+lineSize <= blocks[blockNo].h);
        }
        int splitX, splitY;
        if (vertical)
        {
            splitX = Random.Range(blocks[blockNo].x + minRectSize,
                blocks[blockNo].w + blocks[blockNo].x
                - minRectSize - lineSize + 1);
            splitY = blocks[blockNo].y;
        }
        else
        {
            splitX = blocks[blockNo].x;
            splitY = Random.Range(blocks[blockNo].y + minRectSize,
                blocks[blockNo].h + blocks[blockNo].y
                - minRectSize - lineSize + 1);
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
                for (int j = 0; j < lineSize; j++)
                {
                    mapdata[splitX + j, splitY + i] = -1;
                    //Debug.Log(new Vector2(splitX, splitY + i));
                }
            }
            w1 = splitX - blocks[blockNo].x;
            h1 = blocks[blockNo].h;
            splitX += lineSize;
            w2 = blocks[blockNo].x + blocks[blockNo].w - splitX;
            h2 = h1;
        }
        else
        {
            for (int i = 0; i < blocks[blockNo].w; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    //Debug.Log(new Vector2(splitX + i, splitY));
                    mapdata[splitX + i, splitY + j] = -1;
                }
            }
            w1 = blocks[blockNo].w;
            h1 = splitY - blocks[blockNo].y;
            splitY += lineSize;
            w2 = w1;
            h2 = blocks[blockNo].y + blocks[blockNo].h - splitY;
        }
        blocks[blockNo].Shrink(w1, h1);
        //Debug.Log(blocks[blockNo].w.ToString() + blocks[blockNo].h.ToString());
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
                //Debug.Log(new Vector2(x + i, y + j));
                mapdata[x + i, y + j] = 1;//床にする
            }
        }
        rooms[blockNo].SetRoom(x, y, w, h);
    }

    void CheckAdjacent()//隣接チェック
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i; j < rooms.Count; j++)
            {
                int dire = -1;
                if(rooms[i].y + rooms[i].h + lineSize == rooms[j].y
                    && OnAdjacent(rooms[i].x, rooms[i].w,
                    rooms[j].x, rooms[j].w))
                {
                    dire = 0;
                }
                else if(rooms[i].x + rooms[i].w + lineSize == rooms[j].x
                    && OnAdjacent(rooms[i].y, rooms[i].h,
                    rooms[j].y, rooms[j].h))
                {
                    dire = 1;
                }
                else if(rooms[j].y + rooms[j].h + lineSize == rooms[i].y
                    && OnAdjacent(rooms[i].x, rooms[i].w,
                    rooms[j].x, rooms[j].w))
                {
                    dire = 2;
                }
                else if(rooms[j].x + rooms[j].w + lineSize == rooms[i].x
                    && OnAdjacent(rooms[i].y, rooms[i].h,
                    rooms[j].y, rooms[j].h))
                {
                    dire = 3;
                }
                if (0<=dire)
                {
                    rooms[i].adjNos.Add(j);
                    rooms[i].adjDire.Add(dire);
                    Debug.Log(rooms[i].x);
                    Debug.Log(rooms[i].y);
                    Debug.Log(rooms[i].adjNos[rooms[i].adjNos.Count-1]);
                    Debug.Log(rooms[i].adjDire[rooms[i].adjDire.Count-1]);
                }
            }
        }
    }

    bool OnAdjacent(int cr, int crRange, int val, int valRange)
    {
        int crMax = cr + crRange;
        int valMax = val + valRange;
        return (val <= cr && cr <= valMax)
            || (val <= crMax && crMax <= valMax)
            || (cr <= val && valMax <= crMax);
    }

    void MakeAllAisles()//通路
    {
        List<Vector2> v = new List<Vector2>();
        List<Vector2> pos1 = new List<Vector2>();
        List<Vector2> pos2 = new List<Vector2>();
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms[i].adjNos.Count; j++)
            {
                Vector2 p1, p2;
                p1 = Vector2.zero;
                p2 = Vector2.zero;
                v.Add(MakeAisle(i, rooms[i].adjDire[j], ref p1));
                MakeAisle(rooms[i].adjNos[j], (rooms[i].adjDire[j] + 2) % 4,ref p2);//相手
                pos1.Add(new Vector2(p1.x, p1.y));
                pos2.Add(new Vector2(p2.x, p2.y));
            }
        }
        for (int i = 0; i < v.Count; i++)
        {
            MakeAisleOnSplit(pos1[i], pos2[i], v[i]);
        }
    }

    Vector2 MakeAisle(int roomNo, int dire, ref Vector2 p)
    {
        Vector2 v = Vector2.zero;//通路を掘る方向
        p = Vector2.zero;//掘り始め位置
        do
        {
            switch (dire)
            {
                case 0://down
                    v = Vector2.up;
                    p = new Vector2(rooms[roomNo].rX + Random.Range(0, rooms[roomNo].rW - lineSize + 1),
                        rooms[roomNo].rY + rooms[roomNo].rH - 1);
                    break;
                case 1://right
                    v = Vector2.right;
                    p = new Vector2(rooms[roomNo].rX + rooms[roomNo].rW - 1,
                        rooms[roomNo].rY + Random.Range(0, rooms[roomNo].rH - lineSize + 1));
                    break;
                case 2://up
                    v = Vector2.down;
                    p = new Vector2(rooms[roomNo].rX + Random.Range(0, rooms[roomNo].rW - lineSize + 1),
                        rooms[roomNo].rY);
                    break;
                case 3:
                    v = Vector2.left;
                    p = new Vector2(rooms[roomNo].rX,
                        rooms[roomNo].rY + Random.Range(0, rooms[roomNo].rH - lineSize + 1));
                    break;
            }
        } while (/*mapdata[(int)(p.x + v.x), (int)(p.y + v.y)] == 1
        ||*/ mapdata[(int)(p.x + v.x - Mathf.Abs(v.y)), (int)(p.y + v.y - Mathf.Abs(v.x))] == 1
        /*|| mapdata[(int)(p.x + v.x + Mathf.Abs(v.y) * (lineSize - 1)),
        (int)(p.y + v.y + Mathf.Abs(v.x) * (lineSize - 1))] == 1*/
        || mapdata[(int)(p.x + v.x + Mathf.Abs(v.y) * lineSize), (int)(p.y + v.y + Mathf.Abs(v.x) * lineSize)] == 1);
        int k = 1;
        while (mapdata[(int)(p.x + v.x * k), (int)(p.y + v.y * k)] != -1)
        {
            //Debug.Log(p + v * k);
            for (int i = 0; i < lineSize; i++)
            {
                mapdata[(int)(p.x + v.x * k + Mathf.Abs(v.y) * i),
                    (int)(p.y + v.y * k + Mathf.Abs(v.x) * i)] = 1;
            }
            k++;
        }
        p += v * k;
        return v;
    }

    void MakeAisleOnSplit(Vector2 p1, Vector2 p2,Vector2 v)//区切り線上に通路生成
    {
        if (v.x != 0)//p1,p2位置調整
        {
            if (p2.x < p1.x)
            {
                p1.x = p2.x;
            }
            else
            {
                p2.x = p1.x;
            }
        }
        else
        {
            if (p2.y < p1.y)
            {
                p1.y = p2.y;
            }
            else
            {
                p2.y = p1.y;
            }
        }
        int length = (int)((p1 - p2).magnitude);
        Vector2 dire = length == 0 ? Vector2.right : (p2 - p1) / length;
        if (dire.x + dire.y < 0)
        {
            p1 -= dire * (lineSize - 1);
        }
        Debug.Log(p1);
        Debug.Log(p2);
        Debug.Log(dire);
        Debug.Log(length);
        for (int i = 0; i <= length + lineSize - 1; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                mapdata[(int)(p1.x + dire.x * i + Mathf.Abs(dire.y) * j),
                    (int)(p1.y + dire.y * i + Mathf.Abs(dire.x) * j)] = 1;
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
