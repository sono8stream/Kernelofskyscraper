using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public bool[] aisleDireOn;
    public List<Vector2> vs, poss;
    public int x, y, w, h;
    public int rX, rY, rW, rH;//部屋Rect
    public List<Block> adjBlocks;//隣接ブロックの番号
    public List<int> adjDire;//隣接方向 down=0,right,up,left
    public int floorNo;
    public List<Vector2> sPos;//階段座標
    public int roomNo;
    public Block(int x, int y, int w, int h, int floorNo)
    {
        aisleDireOn = new bool[4];
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
