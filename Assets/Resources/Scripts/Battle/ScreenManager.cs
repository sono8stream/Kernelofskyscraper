using UnityEngine;
using System.Collections;

public class ScreenManager : MonoBehaviour {

    GameObject obj = null;
    public GameObject Object
    {
        get { return obj; }
        set { obj = value; }
    }
    public int mapSizeX, mapSizeY;
    const int cameraSizeX = 16;
    const int cameraSizeY = 9;
    Vector2 keyDownPos;//押下座標格納
    Vector2 velocity;
    Vector2 accel;
    bool cameraIsFixing;//カメラ固定状態
                       // Use this for initialization
    void Start()
    {
        keyDownPos = new Vector2(-1000, 0);
        velocity = Vector2.zero;
        accel = Vector2.zero;
        Debug.Log(keyDownPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//選択したオブジェクトの情報読み取り
        {
            Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D aCollider2d = Physics2D.OverlapPoint(aTapPoint);

            if (aCollider2d)
            {
                obj = aCollider2d.transform.gameObject;
                Debug.Log(obj.name);
            }
            else
            {
                obj = null;
            }
            keyDownPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))//カメラスクロール
        {
            velocity = keyDownPos / 160 - (Vector2)Input.mousePosition / 160;
            accel = velocity / (-10);
            transform.position = (Vector2)transform.position + velocity;
            transform.position += new Vector3(0, 0, -10);
            LimitScroll(mapSizeX, mapSizeY);
            keyDownPos = Input.mousePosition;
        }
        else if (velocity != Vector2.zero)//余韻スクロール
        {
            transform.position = (Vector2)transform.position + velocity;
            transform.position += new Vector3(0, 0, -10);
            LimitScroll(mapSizeX, mapSizeY);
            velocity += accel;
        }
    }

    void LimitScroll(int sizeX,int sizeY)
    {
        float correctionX = -0.5f;
        float speed = 0.1f;
        float marginX = 1;
        float marginY = 1;
        float rangeX = (sizeX - cameraSizeX) / 2 + marginX;
        float rangeY = (sizeY - cameraSizeY) / 2 + marginY;
        float correctionDown = 2;
        if (transform.position.x < correctionX - rangeX)
        {
            transform.position = new Vector3(correctionX - rangeX, transform.position.y, -10);
            velocity.x = speed;
            accel = velocity / (-10);
        }
        if (transform.position.x > correctionX + rangeX)
        {
            transform.position = new Vector3(correctionX + rangeX, transform.position.y, -10);
            velocity.x = -speed;
            accel = velocity / (-10);
        }
        if (transform.position.y <  - (rangeY+correctionDown))
        {
            transform.position = new Vector3(transform.position.x, -(rangeY + correctionDown), -10);
            velocity.y = speed;
            accel = velocity / (-10);
        }
        if (transform.position.y > rangeY)
        {
            transform.position = new Vector3(transform.position.x, rangeY, -10);
            velocity.y = -speed;
            accel = velocity / (-10);
        }
    }
}
