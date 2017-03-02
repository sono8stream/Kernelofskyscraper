using UnityEngine;
using System.Collections;

/// <summary>
/// マップを自動生成するクラスです。
/// </summary>
public class MapGenerator : MonoBehaviour
{
    SpriteRenderer sr;
    int[,] mapdata;
    [SerializeField]
    GameObject wireH, wireV;
    [SerializeField]
    int width, height;
    [SerializeField]
    Sprite flrSp;//床画像
    const int MASU = 32;
    int stX, stY;//初期座標

    // Use this for initialization
    void Start()
    {
        mapdata = new int[width, height];
        mapdata[stX, stY] = 1;//初期位置に道
        DrawWire();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //自動生成アルゴリズム
    void AutoMapping(int rmCo/*部屋数*/, int x, int y)
    {
        //まずは単純通路のみ
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
        }
    }

    void DrawMap()
    {
        Texture2D t = new Texture2D(width * MASU, height * MASU);
        Color[] cBase = flrSp.texture.GetPixels();
        for (int i = 0; i < width * height; i++)
        {
            if (mapdata[i % width, i / width] == 1)
            {
                t.SetPixels(MASU * (i % width), MASU * (i / width), MASU, MASU, cBase);
            }
        }
        t.Apply();
        sr.sprite = Sprite.Create(t, new Rect(0, 0, width * MASU, height * MASU),
            Vector2.one * 0.5f, MASU);
    }

    void DrawWire()
    {
        float iniX = -(width - width % 2) * 0.5f;
        float iniY = (height - height % 2) * 0.5f;
        for (int x = 0; x < width - 1; x++)
        {
            GameObject w = Instantiate(wireV, transform);
            w.transform.position = Vector3.right * (iniX + x + 0.5f);
            w.transform.localScale = new Vector3(0.01f, 1, height * 0.1f);
        }
        for (int y = 0; y < height - 1; y++)
        {
            GameObject w = Instantiate(wireH, transform);
            w.transform.position = Vector3.up * (iniY - y - 0.5f);
            w.transform.localScale = new Vector3(width * 0.1f, 1, 0.01f);
        }
    }
}
