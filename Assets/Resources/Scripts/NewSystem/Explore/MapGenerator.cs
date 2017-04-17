using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// マップを自動生成するクラスです。
/// </summary>
public class MapGenerator : MonoBehaviour
{
    SpriteRenderer sr;
    int[][,] mapData;
    public int[][,] MapData { get { return mapData; } }
    int[][,] mapGimmickData;
    public int[][,] MapGimmickData { get { return mapGimmickData; } }
    [SerializeField]
    GameObject wireH, wireV;
    [SerializeField]//ブロックサイズ最小値,実際部屋サイズは-2,最大部屋数基準+roomRan
    int width, height, minRectSize = 5, roomLim = 15, floors = 3;
    [SerializeField]
    int[] eRecovLims, cRecovLims;
    int eRecovCo = 0, cRecovCo = 0;//回復パネルカウンタ
    int stX, stY;//初期座標
    int roomRan = 3;
    int roomLimTemp;
    int roomCo = 1;
    int lineSize = 1;
    int kRoomIndex;//カーネルのある部屋番号
    List<Block>[] blocks;//区切りリスト
    public List<Block>[] rooms;//完全に区分けられた部屋リスト

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
        mapData = new int[floors][,];
        mapGimmickData = new int[floors][,];
        blocks = new List<Block>[floors];
        rooms = new List<Block>[floors];

        for (int i = 0; i < floors; i++)//マップ初期化
        {
            mapData[i] = new int[width, height];
            mapGimmickData[i] = new int[width, height];
            blocks[i] = new List<Block>();
            blocks[i].Add(new Block(0, 0, width, height, i));
            rooms[i] = new List<Block>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mapData[i][x, y] = (int)MapPart.none;
                    mapGimmickData[i][x, y] = (int)GimmickType.none;
                }
            }
        }

        for (int i = 0; i < floors; i++)
        {
            roomLimTemp = roomLim + Random.Range(0, roomRan + 1);
            roomCo = 1;
            SplitMap(i);
            rooms[i].AddRange(blocks[i]);
        }
        CheckAdjacent();
        DelAdjacents();
        //Debug.Log(CheckRoute());
        SetKernelPos();//カーネル位置設定
        MakeAllStairs();
        MakeAllRooms();
        MakeAllAisles();
        MakeWalls();
        SetPanels();
        SetDoors();
    }

    #region GenerateMapPart
    void SplitMap(int floorNo)
    {
        int blockNo;
        do
        {
            blockNo = Random.Range(0, blocks[floorNo].Count);
            if (blocks[floorNo][blockNo].w < minRectSize * 2 + lineSize
                && blocks[floorNo][blockNo].h < minRectSize * 2 + lineSize)
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
            vertical = minRectSize * 2 + lineSize <= blocks[floorNo][blockNo].w;
        }
        else
        {
            vertical = !(minRectSize * 2 + lineSize <= blocks[floorNo][blockNo].h);
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
        if (roomCo < roomLimTemp)
        {
            SplitMap(floorNo);
        }
    }

    void SplitBlock(int floorNo, int blockNo, int splitX, int splitY, bool vertical)
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
                    mapData[floorNo][splitX + j, splitY + i] = -1;
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
                    mapData[floorNo][splitX + i, splitY + j] = -1;
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
        float rate = 0.2f;
        int loopLim = 200;
        int tarAdjCo = (int)(CountAdjacents() * rate);
        Debug.Log(tarAdjCo);
        int adjCo = CountAdjacents();
        int fNo, rNo, index1, index2, dire1, dire2;
        Block b1, b2;
        for (int i = 0; i < loopLim && tarAdjCo < adjCo; i++)
        {
            fNo = Random.Range(0, floors);//ランダムでルート消去
            rNo = Random.Range(0, rooms[fNo].Count);

            if (1 < rooms[fNo][rNo].adjBlocks.Count)
            {
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

                if (CheckRoute() && b1.adjDire[0] <= 3
                    && b2.adjDire[0] <= 3)//階段だけでつながっていない
                {
                    adjCo -= 1;
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
                rooms[i][j].roomNo = -1;
            }
        }
        SetNo(rooms[0][kRoomIndex], 0);
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                if (rooms[i][j].roomNo == -1)
                { return false; }
            }
        }
        return true;
    }

    void SetNo(Block b, int no)
    {
        b.roomNo = no;
        for (int i = 0; i < b.adjBlocks.Count; i++)
        {
            if (b.adjBlocks[i].roomNo == -1 || no + 1 < b.adjBlocks[i].roomNo)
            {
                SetNo(b.adjBlocks[i], no + 1);
            }
        }
    }

    void CheckAdjacent()//隣接チェック
    {
        int dire;
        for (int i = 0; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                for (int k = j; k < rooms[i].Count; k++)
                {
                    dire = -1;
                    if (OnAdjacent(rooms[i][j].x, rooms[i][j].w, rooms[i][k].x, rooms[i][k].w))//タテ隣接
                    {
                        if (rooms[i][j].y + rooms[i][j].h + lineSize == rooms[i][k].y)
                        {
                            dire = (int)Direction.Down;
                        }
                        else if (rooms[i][k].y + rooms[i][k].h + lineSize == rooms[i][j].y)
                        {
                            dire = (int)Direction.Up;
                        }
                    }
                    else if (OnAdjacent(rooms[i][j].y, rooms[i][j].h, rooms[i][k].y, rooms[i][k].h))//ヨコ隣接
                    {
                        if (rooms[i][j].x + rooms[i][j].w + lineSize == rooms[i][k].x)
                        {
                            dire = (int)Direction.Right;
                        }
                        else if (rooms[i][k].x + rooms[i][k].w + lineSize == rooms[i][j].x)
                        {
                            dire = (int)Direction.Left;
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
                            rooms[i][j].adjBlocks.Add(rooms[i - 1][k]);
                            rooms[i][j].adjDire.Add((int)Direction.Fdown);//階層下,方向5
                            rooms[i - 1][k].adjBlocks.Add(rooms[i][j]);
                            rooms[i - 1][k].adjDire.Add((int)Direction.Fup);
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

    bool SetKernelPos()//ランダムで部屋を選び、カーネル座標をセット
    {
        int loopLim = 200;
        Block room;
        for (int i = 0; i < loopLim; i++)
        {
            kRoomIndex = Random.Range(0, rooms[0].Count);
            room = rooms[0][Random.Range(0, rooms[0].Count)];
            if (room.adjDire.IndexOf((int)Direction.Fdown) == -1
                && room.adjDire.IndexOf((int)Direction.Fup) == -1)//階段のない部屋を選択
            {
                Vector2 pos
                    = new Vector2(room.x + Random.Range(3, room.w - 4), room.y + Random.Range(3, room.h - 4));
                mapData[0][(int)pos.x, (int)pos.y] = (int)MapPart.kernel;
                Vector2 iniPos = pos - Vector2.one * 2;
                for (int j = 0; j < 25; j++)
                {
                    room.sPos.Add(iniPos + Mathf.Round(j / 5) * Vector2.up + j % 5 * Vector2.right);
                }
                room.sPos.Add(pos - Vector2.one);
                room.sPos.Add(pos + Vector2.one);
                room.sPos.Add(pos + Vector2.right + Vector2.down);
                room.sPos.Add(pos + Vector2.left + Vector2.up);
                Debug.Log("Set Kernel");
                return true;
            }
        }
        return false;
    }

    void MakeAllStairs()
    {
        for (int i = 1; i < floors; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                for (int k = 0; k < rooms[i][j].adjBlocks.Count; k++)
                {
                    if (rooms[i][j].adjDire[k] == (int)Direction.Fdown)//下とつながるときだけ
                    {
                        MakeStair(rooms[i][j], rooms[i][j].adjBlocks[k]);
                        int delIndex = rooms[i][j].adjBlocks[k].adjBlocks.IndexOf(rooms[i][j]);
                        rooms[i][j].adjBlocks[k].adjBlocks.RemoveAt(delIndex);//相手の隣接削除
                        rooms[i][j].adjBlocks[k].adjDire.RemoveAt(delIndex);
                        rooms[i][j].adjBlocks.RemoveAt(k);//自分の隣接削除
                        rooms[i][j].adjDire.RemoveAt(k);
                        k--;
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
        while (1 < mapData[b1.floorNo][x, y] && 1 < mapData[b2.floorNo][x, y]);
        mapData[b1.floorNo][x, y] = (int)MapPart.stairD;
        mapData[b2.floorNo][x, y] = (int)MapPart.stairU;
        Vector2 pos = new Vector2(x, y);
        b1.sPos.Add(pos);
        b2.sPos.Add(pos);
    }

    bool CheckObject(Block b, Vector2 pos)//blockのroom内におけるオブジェクト存在チェック,room内にすべて含有しているか
    {
        for (int i = 0; i < b.sPos.Count; i++)
        {
            if ((pos.x + pos.y < 0) && (b.sPos[i].x < b.rX || b.rX + b.rW - 1 < b.sPos[i].x
                || b.sPos[i].y < b.rY || b.rY + b.rH - 1 < b.sPos[i].y)//部屋解析
                    || b.sPos[i] == pos)
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
                MakeRoom(rooms[i][j]);
            }
        }
    }

    void MakeRoom(Block block)
    {
        int w, h, x, y;
        do
        {
            w = Random.Range(minRectSize - 2, block.w - 1);
            h = Random.Range(minRectSize - 2, block.h - 1);
            x = block.x + Random.Range(1, block.w - w);
            y = block.y + Random.Range(1, block.h - h);
            block.SetRoom(x, y, w, h);
        }
        while (!CheckObject(block, -Vector2.one));
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                //Debug.Log(new Vector2(x + i, y + j));
                if (mapData[block.floorNo][x + i, y + j] == (int)MapPart.none)
                {
                    mapData[block.floorNo][x + i, y + j] = (int)MapPart.floor;//床にする
                }
            }
        }
    }

    void MakeAllAisles()//通路
    {
        Vector2 p1 = Vector2.zero, p2 = Vector2.zero;
        for (int h = 0; h < floors; h++)
        {
            for (int i = 0; i < rooms[h].Count; i++)
            {
                for (int j = 0; j < rooms[h][i].adjBlocks.Count; j++)
                {
                    MakeAisle(rooms[h][i], j);
                    int delIndex = rooms[h][i].adjBlocks[j].adjBlocks.IndexOf(rooms[h][i]);
                    rooms[h][i].adjBlocks[j].adjBlocks.RemoveAt(delIndex);
                    rooms[h][i].adjBlocks[j].adjDire.RemoveAt(delIndex);
                }
            }
        }
    }

    void MakeAisle(Block room, int index)//通路設定
    {
        Block room2 = room.adjBlocks[index];
        Vector2 v = Vector2.zero, vh;//通路を掘る方向,その垂直方向
        Vector2 p1 = Vector2.zero, p2 = Vector2.zero;//掘り始め位置
        int length1 = 0, length2 = 0;
        v = GetAislePos(room, room.adjDire[index], ref p1, ref length1,Vector2.zero);//位置1,方向取得
        GetAislePos(room2, (room.adjDire[index] + 2) % 4, ref p2, ref length2, p1);//位置2取得
        vh = GetAisleSplitVector(p1, p2, v);
        p1 = CheckOtherAisles(room, p1, vh, v, length1);
        p2 = CheckOtherAisles(room2, p2, -vh, -v, length2);
        //Debug.Log(p1 + "and" + p2);
        DigAisle(room.floorNo, ref p1, v, length1);
        DigAisle(room2.floorNo, ref p2, -v, length2);
        MakeAisleOnSplit(room.floorNo, p1, p2, v);
    }

    Vector2 GetAislePos(Block room, int dire, ref Vector2 p, ref int length,Vector2 pairPos)
    {
        Vector2 v = Vector2.zero;
        Vector2 velTemp = Vector2.zero;
        Vector2 posOrigin = Vector2.zero;
        int range = 0, ranNext = 0;

        switch (dire)
        {
            case (int)Direction.Down:
                length = room.y + room.h - room.rY - room.rH;
                range = room.rW - lineSize;
                v = Vector2.up;
                velTemp = Vector2.right;
                posOrigin = new Vector2(room.rX, room.rY + room.rH - 1);
                if(pairPos!=Vector2.zero)
                {
                    p = new Vector2(pairPos.x, posOrigin.y);
                }
                break;
            case (int)Direction.Right:
                length = room.x + room.w - room.rX - room.rW;
                range = room.rH - lineSize;
                v = Vector2.right;
                velTemp = Vector2.up;
                posOrigin = new Vector2(room.rX + room.rW - 1, room.rY);
                if (pairPos != Vector2.zero)
                {
                    p = new Vector2(posOrigin.x, pairPos.y);
                }
                break;
            case (int)Direction.Up:
                length = room.rY - room.y;
                range = room.rW - lineSize;
                v = Vector2.down;
                velTemp = Vector2.right;
                posOrigin = new Vector2(room.rX, room.rY);
                if (pairPos != Vector2.zero)
                {
                    p = new Vector2(pairPos.x, posOrigin.y);
                }
                break;
            case (int)Direction.Left:
                length = room.rX - room.x;
                range = room.rH - lineSize;
                v = Vector2.left;
                velTemp = Vector2.up;
                posOrigin = new Vector2(room.rX, room.rY);
                if (pairPos != Vector2.zero)
                {
                    p = new Vector2(posOrigin.x, pairPos.y);
                }
                break;
        }

        if(p.x<room.rX||room.rX+room.rW-1<p.x||p.y<room.rY||room.rY+room.rH-1<p.y)
        {
            pairPos = Vector2.zero;
        }

        do
        {
            if (pairPos!=Vector2.zero)
            {
                pairPos = Vector2.zero;
            }
            else
            {
                ranNext = Random.Range(0, range + 1);
                p = posOrigin + velTemp * ranNext;
            }
            //Debug.Log(dire+"and"+posOrigin+"and"+ room.rW + "and" + room.rH + "and" + range +"and"+ranNext + "and" + p);
        } while (/*(int)MapPart.wall < mapData[room.floorNo][(int)p.x, (int)p.y]*///通路に障害物
        !CheckObject(room, p)
        || mapData[room.floorNo][(int)(p.x + v.x - Mathf.Abs(v.y)),
        (int)(p.y + v.y - Mathf.Abs(v.x))] == (int)MapPart.floor
        || mapData[room.floorNo][(int)(p.x + v.x + Mathf.Abs(v.y) * lineSize),
        (int)(p.y + v.y + Mathf.Abs(v.x) * lineSize)] == (int)MapPart.floor);
        p += v;//pを一つ分移動
        return v;
    }

    //各部屋において、同じ方向に重複している通路を検出し、その座標を返す
    Vector2 CheckOtherAisles(Block block, Vector2 pos, Vector2 vh, Vector2 v, int length)
    {
        int lim = 0;
        Vector2 posTemp = pos;
        //既に通路が出来ている場合、位置を部屋位置端まで移動させて調査する
        if (mapData[block.floorNo][(int)(pos.x + v.x * length), (int)(pos.y + v.y * length)] == (int)MapPart.floor
            || mapData[block.floorNo][(int)(pos.x + v.x * length + vh.x),
            (int)(pos.y + v.y * length + vh.y)] == (int)MapPart.floor
            || mapData[block.floorNo][(int)(pos.x + v.x * length - vh.x),
            (int)(pos.y + v.y * length - vh.y)] == (int)MapPart.floor)
        {
            if (vh.x != 0)
            {
                posTemp.x = 0 < vh.x ? block.rX : block.rX + block.rW - lineSize;
            }
            else
            {
                posTemp.y = 0 < vh.y ? block.rY : block.rY + block.rH - lineSize;
            }
        }
        if (vh.x != 0)//ヨコ
        {
            lim = 0 < vh.x ? block.rX + block.rW - (int)posTemp.x - lineSize + 1 : (int)posTemp.x - block.rX + 1;
        }
        else//タテ
        {
            lim = 0 < vh.y ? block.rY + block.rH - (int)posTemp.y - lineSize + 1 : (int)posTemp.y - block.rY + 1;
        }
        for (int i = 0; i < lim; i++)
        {
            //Debug.Log(pos + v * i);
            if (mapData[block.floorNo][(int)(posTemp.x + vh.x * i),
                (int)(posTemp.y + vh.y * i)] == (int)MapPart.floor)
            {
                return posTemp + vh * i;
            }
        }
        return pos;
    }

    void DigAisle(int floor, ref Vector2 pos, Vector2 vec, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (mapData[floor][(int)(pos.x + vec.x * i), (int)(pos.y + vec.y * i)] == (int)MapPart.floor)
            {
                break;
            }
            else
            {
                mapData[floor][(int)(pos.x + vec.x * i), (int)(pos.y + vec.y * i)] = (int)MapPart.floor;
            }
        }
        pos += vec * length;
    }

    Vector2 GetAisleSplitVector(Vector2 p1, Vector2 p2, Vector2 vec)
    {
        if (vec.x != 0)
        {
            if (p2.y < p1.y)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.up;
            }
        }
        else
        {
            if (p2.x < p1.x)
            {
                return Vector2.left;
            }
            else
            {
                return Vector2.right;
            }
        }
    }

    void MakeAisleOnSplit(int floorNo, Vector2 p1, Vector2 p2, Vector2 v)//区切り線上に通路生成
    {
        //p1,p2位置調整
        if (v.x != 0)//横方向だったら
        {
            if (p2.x < p1.x)//x座標そろえる
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
            if (p2.y < p1.y)//y座標そろえる
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
        if (dire.x + dire.y < 0)//
        {
            p1 -= dire * (lineSize - 1);
        }
        for (int i = 0; i <= length + lineSize - 1; i++)
        {
            for (int j = 0; j < lineSize; j++)
            {
                mapData[floorNo][(int)(p1.x + dire.x * i + Mathf.Abs(dire.y) * j),
                    (int)(p1.y + dire.y * i + Mathf.Abs(dire.x) * j)] = (int)MapPart.floor;
            }
        }
    }

    void MakeWalls()
    {
        for (int i = 0; i < floors; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mapData[i][x, y] < (int)MapPart.floor)
                    {
                        mapData[i][x, y] = (int)MapPart.wall;//壁にする
                    }
                }
            }
        }
    }
    #endregion

    #region GenerateGimmick
    void SetPanels()//回復パネル設置
    {
        int index, x, y;
        for (int i = 0; i < floors; i++)
        {
            while (eRecovCo < eRecovLims[i])
            {
                index = Random.Range(0, rooms[i].Count);
                x = Random.Range(1, rooms[i][index].rW-1);
                y = Random.Range(1, rooms[i][index].rH-1);
                Vector2 p = new Vector2(x + rooms[i][index].rX, y + rooms[i][index].rY);
                if (CheckObject(rooms[i][index], p))
                {
                    mapGimmickData[i][(int)p.x, (int)p.y] = (int)GimmickType.eRecovPanel;
                    rooms[i][index].sPos.Add(p);
                    eRecovCo++;
                }
            }
            while (cRecovCo < cRecovLims[i])
            {
                index = Random.Range(0, rooms[i].Count);
                x = Random.Range(1, rooms[i][index].rW-1);
                y = Random.Range(1, rooms[i][index].rH-1);
                Vector2 p = new Vector2(x + rooms[i][index].rX, y + rooms[i][index].rY);
                if (CheckObject(rooms[i][index], p))
                {
                    mapGimmickData[i][(int)p.x, (int)p.y] = (int)GimmickType.cRecovPanel;
                    rooms[i][index].sPos.Add(p);
                    cRecovCo++;
                }
            }
            eRecovCo = 0;
            cRecovCo = 0;
        }
    }

    void SetDoors()
    {
        int index, x, y, aIndex, co = 0, loopCo = 0, loopLim = 20;
        Vector2 p;
        Vector2[] dPosArray;
        for (int i = 0; i < floors; i++)
        {
            while (co < 2&&loopCo<loopLim)
            {
                index = Random.Range(0, rooms[i].Count);
                x = Random.Range(1, rooms[i][index].rW - 1);
                y = Random.Range(1, rooms[i][index].rH - 1);
                p = new Vector2(x + rooms[i][index].rX, y + rooms[i][index].rY);
                dPosArray = AislePosArrayOnRoomEdge(rooms[i][index]);
                if (CheckObject(rooms[i][index], p) && 0 < dPosArray.Length)
                {
                    aIndex = Random.Range(0, dPosArray.Length);
                    mapGimmickData[i][(int)dPosArray[aIndex].x, (int)dPosArray[aIndex].y] = (int)GimmickType.door;
                    mapGimmickData[i][(int)p.x, (int)p.y]
                        = (int)GimmickType.destroySwitch + (int)(dPosArray[aIndex].x * 0x10000
                        + dPosArray[aIndex].y * 0x100);
                    rooms[i][index].sPos.Add(p);
                    co++;
                }
                loopCo++;
            }
            co = 0;
            loopCo = 0;
        }
    }

    Vector2[] AislePosArrayOnRoomEdge(Block b)
    {
        int i = 0;
        int iniX = b.rX - 1;
        int iniY = b.rY - 1;
        int adjIndex = -1;
        List<Vector2> posList = new List<Vector2>();
        Vector2 pos;

        while (i < (b.rW + 2) * 2 + b.rH * 2)
        {
            pos = new Vector2(iniX + i % (b.rW + 2), iniY + i / (b.rW + 2));
            int y = i / (b.rW + 2);
            adjIndex = -1;
            if (y == 0)//上
            {
                adjIndex = b.adjDire.IndexOf((int)Direction.Up);
                i++;
            }
            else if (y == b.rH + 1)//下
            {
                adjIndex = b.adjDire.IndexOf((int)Direction.Down);
                i++;
            }
            else if (i % (b.rW + 2) == 0)//左
            {
                adjIndex = b.adjDire.IndexOf((int)Direction.Left);
                i += b.rW + 1;
            }
            else//右
            {
                adjIndex = b.adjDire.IndexOf((int)Direction.Right);
                i++;
            }
            if (mapGimmickData[b.floorNo][(int)pos.x, (int)pos.y] == (int)GimmickType.none
                && mapData[b.floorNo][(int)pos.x, (int)pos.y] == (int)MapPart.floor
                && 0 <= adjIndex && b.roomNo <= b.adjBlocks[adjIndex].roomNo)
            {
                posList.Add(pos);
            }
        }
        return posList.ToArray();
    }


    #endregion

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

    enum Direction
    {
        Down = 0, Right, Up, Left, Fdown = 5, Fup = 6
    }
}
