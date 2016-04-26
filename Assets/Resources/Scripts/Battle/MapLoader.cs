﻿using UnityEngine;
using System.Collections;

public class MapLoader : MonoBehaviour {

    public TextAsset mp_layout;//マップ情報を記述したテキスト
    Texture2D MapImage;
    const int MAP_WIDTH = 16;
    const int MAP_HEIGHT = 9;
    public int[,] mapdata;
    public string[] mapdataDebug;
    public Sprite mapchips;
    Sprite map;
    const int MASU = 32;
    // Use this for initialization
    void Awake()
    {
        ReadMap();
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

        string[] eachInfo;
        for (int i = 0; i < layoutInfo.Length; i++)
        {
            layoutInfo[i]=layoutInfo[i].Remove(layoutInfo[i].Length - 1);
            eachInfo = layoutInfo[i].Split(',');
            if (i == 0)//mapdata初期化
            {
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
        MapImage = new Texture2D(MASU * MAP_WIDTH, MASU * MAP_HEIGHT, TextureFormat.RGBA32, false);//マップ初期化
        for (int i = 0; i < MAP_WIDTH; i++)
        {
            for (int j = 0; j < MAP_HEIGHT; j++)
            {
                Color[] c = mapchips.texture.GetPixels(MASU * (mapdata[i, j] % 8),
                    mapchips.texture.height - MASU * (1 + mapdata[i, j] / 8), MASU, MASU);
                MapImage.SetPixels(MASU * i, MapImage.height - MASU * (j + 1), MASU, MASU, c);
            }
        }
        MapImage.Apply();
        map = Sprite.Create(MapImage, new Rect(0, 0, MAP_WIDTH * MASU, MAP_HEIGHT * MASU), new Vector2(0.5f, 0.5f), MASU);
        GetComponent<SpriteRenderer>().sprite = map;
        AdjustMapData();
        return new Vector2(mapdata.GetLength(0), mapdata.GetLength(1));
    }

    /// <summary>
    /// マップデータをy座標上向きにとる,つまりひっくり返す
    /// </summary>
    void AdjustMapData()
    {
        for (int i = 0; i < mapdata.GetLength(0); i++)//よこループ
        {
            for (int j = 0; j < Mathf.FloorToInt(mapdata.GetLength(1) / 2); j++)//タテループ
            {
                mapdata[i, j] += mapdata[i, mapdata.GetLength(1) - j - 1];
                mapdata[i, mapdata.GetLength(1) - j - 1] = mapdata[i, j]
                    - mapdata[i, mapdata.GetLength(1) - j - 1];
                mapdata[i, j] -= mapdata[i, mapdata.GetLength(1) - j - 1];
            }
        }
    }
}
