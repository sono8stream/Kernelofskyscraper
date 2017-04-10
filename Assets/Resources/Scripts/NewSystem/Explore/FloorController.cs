using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    [SerializeField]
    GameObject cursorGO;
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
    int floorNo;
    public int FloorNo
    {
        get { return floorNo; }
    }
    float barDivision;

    // Use this for initialization
    void Start()
    {
        int length = map.MapData.Length;
        scrollBar.numberOfSteps = length;
        barDivision = 1 < length ? 1.0f / (2 * length - 2) : 1;
        UpdateMapImage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMapImage()
    {
        if (mapImage == null)
        { Debug.Log("koko"); return; }
        int texSize = (int)mapImage.GetComponent<RectTransform>().sizeDelta.x;//テクスチャサイズ
        int selSize;//1マス分のピクセルサイズ
        int iniX, iniY;
        Texture2D texture = new Texture2D(texSize, texSize);
        Color[] colors;
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
        colors = new Color[selSize * selSize];
        StaticMethodsCollection.ForArray(ref colors, x => { return Color.white; });

        for (int i = 0; i < map.MapWidth * map.MapHeight; i++)
        {
            if (map.MapData[floorNo][i % map.MapWidth, i / map.MapWidth].tile.activeSelf
                && map.MapData[floorNo][i % map.MapWidth, i / map.MapWidth].partNo == (int)MapPart.floor)
            {
                texture.SetPixels(iniX + i % map.MapWidth * selSize,
                    texSize - iniY - (i / map.MapWidth + 1) * selSize, selSize, selSize, colors);
            }
        }
        Debug.Log("OK?");
        texture.Apply();
        mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texSize, texSize), Vector2.zero);

        UpdateCameraPos();
    }

    void UpdateCameraPos()
    {
        Vector3 pos = camera.transform.localPosition;
        camera.transform.SetParent(map.transform.FindChild("Floor" + (floorNo + 1).ToString()));
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
