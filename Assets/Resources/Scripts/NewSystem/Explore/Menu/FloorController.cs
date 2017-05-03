﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    [SerializeField]
    GameObject cursorGO;
        [SerializeField]
        RectTransform mapCursorRT;
    [SerializeField]
    MapLoader map;
    [SerializeField]
    Image mapImage;
    [SerializeField]
    Scrollbar scrollBar;
    [SerializeField]
    Camera camera;
    [SerializeField]
    Text floorText;
    float barDivision;

    int selSize;//1マス分のピクセルサイズ
    int texSize;//テクスチャサイズ
    int iniX, iniY;
    int floorNo;
    public int FloorNo { get { return floorNo; } }
    Texture2D texture;
    Color[] colors;
    float cCorrectionY = 12;

    // Use this for initialization
    void Start()
    {
        int length = map.MapData.Length;
        scrollBar.numberOfSteps = length;
        barDivision = 1 < length ? 1.0f / (2 * length - 2) : 1;

        InitiateMapImage();
        UpdateMapImage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitiateMapImage()
    {
        if (mapImage == null)
        { Debug.Log("koko"); return; }
        texSize = (int)mapImage.GetComponent<RectTransform>().sizeDelta.x;
        texture = new Texture2D(texSize, texSize);

        if (map.MapHeight < map.MapWidth)
        {
            iniX = 0;
            selSize = texSize / map.MapWidth;
            iniY = (texSize - selSize * map.MapHeight) / 2;
        }
        else
        {
            iniY = 0;
            selSize = texSize / map.MapHeight;
            iniX = (texSize - selSize * map.MapWidth) / 2;
        }

        mapCursorRT.sizeDelta = new Vector2(16, 12) * selSize;
        colors = new Color[selSize * selSize];
        StaticMethodsCollection.ForArray(ref colors, x => { return Color.white; });
    }

    public void UpdateMapImage(int floor=-1)
    {
        if (floor != -1)
        {
            floorNo = floor;
            scrollBar.value = floor;
        }

        for (int i = 0; i < map.MapWidth * map.MapHeight; i++)
        {
            if (map.MapData[floorNo][i % map.MapWidth, i / map.MapWidth].tile.activeSelf
                && map.MapData[floorNo][i % map.MapWidth, i / map.MapWidth].partNo == (int)MapPart.floor)
            {
                StaticMethodsCollection.ForArray(ref colors, x => { return Color.white; });
            }
            else
            {
                StaticMethodsCollection.ForArray(ref colors, x => { return Color.black; });
            }
            texture.SetPixels(iniX + i % map.MapWidth * selSize,
                texSize - iniY - (i / map.MapWidth + 1) * selSize, selSize, selSize, colors);
        }
        texture.Apply();
        mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texSize, texSize), Vector2.zero);

        UpdateCameraPos();
    }

    public void MoveMapCursor()//マップ上のカーソルを動かす,ヨコ17マス、タテ12マス 16:12でよいよ
    {
        mapCursorRT.anchoredPosition 
            = (camera.transform.localPosition + Vector3.up * cCorrectionY) * selSize;
    }

    void UpdateCameraPos()
    {
        Vector3 pos = camera.transform.localPosition;
        camera.transform.SetParent(map.FloorGOs[floorNo].transform);
        cursorGO.transform.SetParent(camera.transform.parent);
        camera.transform.localPosition = pos;
    }

    public void GetBarChange()
    {
        float val = scrollBar.value + barDivision;
        int no = Mathf.FloorToInt(val / barDivision / 2);
        if (floorNo != no)
        {
            floorNo = no;
            floorText.text = (floorNo + 1).ToString() + "F";
            UpdateMapImage();
        }
    }
}
