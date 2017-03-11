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
        public List<Block> adjBlocks;//隣接ブロックの番号
        public List<int> adjDire;//隣接方向 down=0,right,up,left
        public int floorNo;
        public List<Vector2> sPos;//階段座標
        public int roomNo;
        public Block(int x, int y, int w, int h,int floorNo)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.floorNo = floorNo;
            adjBlocks = new List<Block>();
            adjDire = new List<int>();
            sPos = new List<Vector2>();
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
    List<Block>[] blocks;//区切りリスト
    List<Block>[] rooms;//完全に区分けられた部屋リスト


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
        blocks = new List<Block>[floors];
        rooms = new List<Block>[floors];
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
            blocks[i] = new List<Block>();
            blocks[i].Add(new Block(0, 0, width, height, i));
            rooms[i] = new List<Block>();
        }
        roomLim = 4 + Random.Range(0, roomRan + 1);
        roomCo = 1;
        for (int i = 0; i < floors; i++)
        {
            SplitMap(i);
            rooms[i].AddRange(blocks[i]);
            roomCo = 0;
        }
        CheckAdjacent();
        DelAdjacents();
        //Debug.Log(CheckRoute());
        MakeAllStairs();
        MakeAllRooms();
        MakeAllAisles();
        MakeWall();
    }

    void SplitMap(int floorNo)
    {
        int blockNo;
        do
        {
            blockNo = Random.Range(0, blocks[floorNo].Count);
            if (blocks[floorNo][blockNo].w < minRectSize * 2+lineSize
                && blocks[floorNo][blockNo].h < minRectSize * 2+lineSize)
            {
                rooms[floorNo].Add(blocks[floorNo][blockNo]);
                blocks[floorNo].RemoveAt(blockNo);
                if (blocks[floorNo].Count == 0)
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
            vertical = minRectSize * 2+lineSize <= blocks[floorNo][blockNo].w;
        }
        else
        {
            vertical = !(minRectSize * 2+lineSize <= blocks[floorNo][blockNo].h);
        }
        int splitX, splitY;
        if (vertical)
        {
            splitX = Random.Range(blocks[floorNo][blockNo].x + minRectSize,
                blocks[floorNo][blockNo].w + blocks[floorNo][blockNo].x
                - minRectSize - lineSize + 1);
            splitY = blocks[floorNo][blockNo].y;
        }
        else
        {
            splitX = blocks[floorNo][blockNo].x;
            splitY = Random.Range(blocks[floorNo][blockNo].y + minRectSize,
                blocks[floorNo][blockNo].h + blocks[floorNo][blockNo].y
                - minRectSize - lineSize + 1);
        }
        SplitBlock(floorNo, blockNo, splitX, splitY, vertical);
        if (roomCo < roomLim)
        {
            SplitMap(floorNo);
        }
    }

    void SplitBlock(int floorNo, int blockNo,int splitX,int splitY,bool vertical)
    {
        /*Debug.Log("Block:" + blocks[blockNo].x.ToString()
            + blocks[blockNo].y.ToString()
            + blocks[blockNo].w.ToString() + blocks[blockNo].h.ToString());
        Debug.Log("Split:"+splitX.ToString() + splitY.ToString());*/
        int w1, w2, h1, h2;
        w1 = blocks[floorNo][blockNo].x - splitX;
        if (vertical)
        {
            for (int i = 0; i < blocks[floorNo][blockNo].h; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    mapdata[floorNo][splitX + j, splitY + i] = -1;
                    //Debug.Log(new Vector2(splitX, splitY + i));
                }
            }
            w1 = splitX - blocks[floorNo][blockNo].x;
            h1 = blocks[floorNo][blockNo].h;
            splitX += lineSize;
            w2 = blocks[floorNo][blockNo].x + blocks[floorNo][blockNo].w - splitX;
            h2 = h1;
        }
        else
        {
            for (int i = 0; i < blocks[floorNo][blockNo].w; i++)
            {
                for (int j = 0; j < lineSize; j++)
                {
                    //Debug.Log(new Vector2(splitX + i, splitY));
                    mapdata[floorNo][splitX + i, splitY + j] = -1;
                }
            }
            w1 = blocks[floorNo][blockNo].w;
            h1 = splitY - blocks[floorNo][blockNo].y;
            splitY += lineSize;
            w2 = w1;
            h2 = blocks[floorNo][blockNo].y + blocks[floorNo][blockNo].h - splitY;
        }
        blocks[floorNo][blockNo].Shrink(w1, h1);
        //Debug.Log(blocks[blockNo].w.ToString() + blocks[blockNo].h.ToString());
        blocks[floorNo].Insert(blockNo + 1, new Block(splitX, splitY, w2, h2, floorNo));
        roomCo++;
    }

    void DelAdjacents()//指定割合まで隣接消去
    {
        float rate = 0.5f;
        int loopLim = 100;
        int tarAdjCo = (int)(CountAdjacents() * rate);
        int adjCo = CountAdjacents();
        int fNo, rNo, index1, index2, dire1, dire2;
        Block b1, b2;
        for (int i = 0; i < loopLim && tarAdjCo < adjCo; i++)
        {
            fNo = Random.Range(0, floors);//ランダムでルート消去
            rNo = Random.Range(0, rooms[fNo].Count);
            index1 = Random.Range(0, rooms[fNo][rNo].adjBlocks.Count);
            b1 = rooms[fNo][rNo];
            b2 = b1.adjBlocks[index1];
            index2 = b2.adjBlocks.IndexOf(b1);
            dire1 = b1.adjDire[index1];
            dire2 = b2.adjDire[index2];
            b1.adjBlocks.RemoveAt(index1);
            b1.adjDire.RemoveAt(index1);
            b2.adjBlocks.RemoveAt(index2);
            b2.adjDire.RemoveAt(index2);
            if(CheckRoute())
            {
                adjCo -= 2;
            }
            else
            {
                b1.adjBlocks.Add(b2);
                b1.adjDire.Add(dire1);
                b2.adjBlocks.Add(b1);
                b2.adjDire.Add(dire2);
            }
        }
    }

    int CountAdjacents()
    {
        int adjCo = 0;
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                adjCo += rooms[i][j].adjBlocks.Count;
            }
        }
        return adjCo;
    }

    bool CheckRoute()//ルートチェック
    {
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                rooms[i][j].roomNo = 0;
            }
        }
        SetNo(rooms[0][0]);
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                if (rooms[i][j].roomNo == 0)
                { return false; }
            }
        }
        return true;
    }

    void SetNo(Block b)
    {
        if (b.roomNo == 1)
        {
            return;
        }
        b.roomNo = 1;
        for (int i = 0; i < b.adjBlocks.Count; i++)
        {
            SetNo(b.adjBlocks[i]);
        }
    }

    void CheckAdjacent()//隣接チェック
    {
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                for (int k = j; k < rooms[i].Count; k++)
                {
                    int dire = -1;
                    if(OnAdjacent(rooms[i][j].x, rooms[i][j].w,rooms[i][k].x, rooms[i][k].w))//タテ隣接
                    {
                        if(rooms[i][j].y + rooms[i][j].h + lineSize == rooms[i][k].y)
                        {
                            dire = 0;
                        }
                        else if(rooms[i][k].y + rooms[i][k].h + lineSize == rooms[i][j].y)
                        {
                            dire = 2;
                        }
                    }
                    else if(OnAdjacent(rooms[i][j].y, rooms[i][j].h, rooms[i][k].y, rooms[i][k].h))//ヨコ隣接
                    {
                        if (rooms[i][j].x + rooms[i][j].w + lineSize == rooms[i][k].x)
                        {
                            dire = 1;
                        }
                        else if (rooms[i][k].x + rooms[i][k].w + lineSize == rooms[i][j].x)
                        {
                            dire = 3;
                        }
                    }
                    if (0 <= dire)
                    {
                        rooms[i][j].adjBlocks.Add(rooms[i][k]);
                        rooms[i][j].adjDire.Add(dire);
                        rooms[i][k].adjBlocks.Add(rooms[i][j]);
                        rooms[i][k].adjDire.Add((dire + 2) % 4);
                    }
                }
                if (0 < i)//下の階との隣接チェック
                {
                    for (int k = 0; k < rooms[i - 1].Count; k++)
                    {
                        if (OnFloorAdjucent(rooms[i][j], rooms[i - 1][k]))
                        {
                            rooms[i][j].adjBlocks.Add(rooms[i-1][k]);
                            rooms[i][j].adjDire.Add(5);//階層下,方向5
                            rooms[i-1][k].adjBlocks.Add(rooms[i][j]);
                            rooms[i-1][k].adjDire.Add(6);
                        }
                    }
                }
            }
        }
    }

    bool OnAdjacent(int cr, int crRange, int val, int valRange)
    {
        int crMax = cr + crRange - 1;
        int valMax = val + valRange - 1;
        return (val <= cr && cr <= valMax)
            || (val <= crMax && crMax <= valMax)
            || (cr <= val && valMax <= crMax);
    }

    bool OnFloorAdjucent(Block b1, Block b2)
    {
        return OnAdjacent(b1.x + 1, b1.w - 2, b2.x + 1, b2.w - 2) 
            && OnAdjacent(b1.y + 1, b1.h - 2, b2.y + 1, b2.h - 2);
    }

    void MakeAllStairs()
    {
        for (int i = 1; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                for (int k = 0; k < rooms[i][j].adjBlocks.Count; k++)
                {
                    if (rooms[i][j].adjDire[k] == 5)//下とつながるときだけ
                    {
                        MakeStair(rooms[i][j], rooms[i][j].adjBlocks[k]);
                    }
                }
            }
        }
    }

    void MakeStair(Block b1, Block b2)//下の階と接続
    {
        int cX = b2.x < b1.x ? b1.x + 1 : b2.x + 1;
        int cY = b2.y < b1.y ? b1.y + 1 : b2.y + 1;
        int cXlim = b1.x + b1.w < b2.x + b2.w ? b1.x + b1.w - 1 : b2.x + b2.w - 1;
        int cYlim = b1.y + b1.h < b2.y + b2.h ? b1.y + b1.h - 1 : b2.y + b2.h - 1;
        int x, y;
        do
        {
            x = Random.Range(cX, cXlim);
            y = Random.Range(cY, cYlim);
        }
        while (1 < mapdata[b1.floorNo][x, y] && 1 < mapdata[b2.floorNo][x, y]);
        mapdata[b1.floorNo][x, y] = 3;
        mapdata[b2.floorNo][x, y] = 4;
        Vector2 pos = new Vector2(x, y);
        b1.sPos.Add(pos);
        b2.sPos.Add(pos);
    }

    bool CheckStair(Block b)
    {
        for (int i = 0; i < b.sPos.Count; i++)
        {
            if (b.sPos[i].x < b.rX || b.rX + b.rW - 1 < b.sPos[i].x
                || b.sPos[i].y < b.rY || b.rY + b.rH - 1 < b.sPos[i].y)
            {
                return false;
            }
        }
        return true;
    }

    void MakeAllRooms()
    {
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                MakeRoom(i, j);
            }
        }
    }

    void MakeRoom(int floorNo, int blockNo)
    {
        int w, h, x, y;
        do
        {
            w = Random.Range(minRectSize - 2, rooms[floorNo][blockNo].w - 1);
            h = Random.Range(minRectSize - 2, rooms[floorNo][blockNo].h - 1);
            x = rooms[floorNo][blockNo].x + Random.Range(1, rooms[floorNo][blockNo].w - w);
            y = rooms[floorNo][blockNo].y + Random.Range(1, rooms[floorNo][blockNo].h - h);
            rooms[floorNo][blockNo].SetRoom(x, y, w, h);
        }
        while (!CheckStair(rooms[floorNo][blockNo]));
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                //Debug.Log(new Vector2(x + i, y + j));
                if (mapdata[floorNo][x + i, y + j] == 0)
                {
                    mapdata[floorNo][x + i, y + j] = 1;//床にする
                }
            }
        }
    }

    void MakeAllAisles()//通路
    {
        for (int h = 0; h < floors; h++)
        {
            List<Vector2> v = new List<Vector2>();
            List<Vector2> pos1 = new List<Vector2>();
            List<Vector2> pos2 = new List<Vector2>();
            for (int i = 0; i < rooms[h].Count; i++)
            {
                for (int j = 0; j < rooms[h][i].adjBlocks.Count; j++)
                {
                    if (rooms[h][i].adjDire[j] <= 3)
                    {
                        Vector2 p1, p2;
                        p1 = Vector2.zero;
                        p2 = Vector2.zero;
                        v.Add(MakeAisle(rooms[h][i],rooms[h][i].adjDire[j], ref p1));
                        int delIndex = rooms[h][i].adjBlocks[j].adjBlocks.IndexOf(rooms[h][i]);
                        rooms[h][i].adjBlocks[j].adjBlocks.RemoveAt(delIndex);
                        rooms[h][i].adjBlocks[j].adjDire.RemoveAt(delIndex);
                        MakeAisle(rooms[h][i].adjBlocks[j], (rooms[h][i].adjDire[j] + 2) % 4, ref p2);//相手
                        pos1.Add(new Vector2(p1.x, p1.y));
                        pos2.Add(new Vector2(p2.x, p2.y));
                    }
                }
            }
            for (int i = 0; i < v.Count; i++)
            {
                MakeAisleOnSplit(h, pos1[i], pos2[i], v[i]);
            }
        }
    }

    Vector2 MakeAisle(Block room,int dire,ref Vector2 p)
    {
        Vector2 v = Vector2.zero;//通路を掘る方向
        p = Vector2.zero;//掘り始め位置
        do
        {
            switch (dire)
            {
                case 0://down
                    v = Vector2.up;
                    p = new Vector2(room.rX 
                        + Random.Range(0, room.rW - lineSize + 1),
                        room.rY + room.rH - 1);
                    break;
                case 1://right
                    v = Vector2.right;
                    p = new Vector2(room.rX + room.rW - 1,
                        room.rY + Random.Range(0, room.rH - lineSize + 1));
                    break;
                case 2://up
                    v = Vector2.down;
                    p = new Vector2(room.rX
                        + Random.Range(0, room.rW - lineSize + 1), room.rY);
                    break;
                case 3:
                    v = Vector2.left;
                    p = new Vector2(room.rX,
                        room.rY + Random.Range(0, room.rH - lineSize + 1));
                    break;
            }
        } while (mapdata[room.floorNo][(int)(p.x + v.x - Mathf.Abs(v.y)), (int)(p.y + v.y - Mathf.Abs(v.x))] == 1
        || mapdata[room.floorNo][(int)(p.x + v.x + Mathf.Abs(v.y) * lineSize),
        (int)(p.y + v.y + Mathf.Abs(v.x) * lineSize)] == 1);
        int k = 1;
        while (mapdata[room.floorNo][(int)(p.x + v.x * k), (int)(p.y + v.y * k)] != -1)
        {
            for (int i = 0; i < lineSize; i++)
            {
                mapdata[room.floorNo][(int)(p.x + v.x * k + Mathf.Abs(v.y) * i),
                    (int)(p.y + v.y * k + Mathf.Abs(v.x) * i)] = 1;
            }
            k++;
        }
        p += v * k;
        return v;
    }

    void MakeAisleOnSplit(int floorNo,Vector2 p1, Vector2 p2,Vector2 v)//区切り線上に通路生成
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
        for (int i = 0; i <= length + lineSize - 1; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                mapdata[floorNo][(int)(p1.x + dire.x * i + Mathf.Abs(dire.y) * j),
                    (int)(p1.y + dire.y * i + Mathf.Abs(dire.x) * j)] = 1;
            }
        }
    }

    void MakeWall()
    {
        for (int i = 0; i < floors; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapdata[i][x, y]<1)
                    {
                        mapdata[i][x, y] = 2;//壁にする
                    }
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
