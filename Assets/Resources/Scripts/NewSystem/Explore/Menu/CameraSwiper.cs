using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSwiper : MonoBehaviour
{
    #region Property
    [SerializeField]
    Camera camera;
    [SerializeField]
    MapLoader map;
    [SerializeField]
    PanelMenu panelMenu;
    [SerializeField]
    RobotMenu robotMenu;
    [SerializeField]
    FloorController floorCon;
    public int Floor { get { return floorCon.FloorNo; } }
    [SerializeField]
    GameObject panelOrigin;
    [SerializeField]
    GameObject cursorGO;
    MapObject selectedObject;
    [SerializeField]
    StatusController status;
    GameObject kernel;
    Vector3 keyDownPos;
    Vector3 touchingPos;
    Vector3 tappedMapPos;
    Vector3 mapCorrectionPos;//カメラ→マップ座標系変化時の補正
    Vector3 velocity;
    Vector3 accel;
    bool cameraIsFixing;//カメラ固定状態
    public bool onPanel;
    #region for Scroll
    float swipeMargin;
    float period;//スクロール時間
    float speed = 0.1f;
    float marginY = -8;
    float rangeX;
    float rangeY;
    float correctionDown = /*2*/16;
    float posZ;
    #endregion
    #endregion

    // Use this for initialization
    void Start()
    {
        swipeMargin = 5;
        mapCorrectionPos = Vector3.back * 0.01f - camera.transform.position;
        period = 120;
        rangeX = map.MapWidth * 0.5f;
        rangeY = map.MapHeight * 0.5f + marginY;
        posZ = camera.transform.localPosition.z;
        kernel = GameObject.Find("Kernel_3d(Clone)");
        camera.transform.localPosition = kernel.transform.localPosition - mapCorrectionPos;
        LimitScroll(false);
        Debug.Log(mapCorrectionPos);
        Debug.Log(kernel.transform.localPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedObject != null)//ステータス表示中、ロボを追従
        {
            RectTransform canvasRect = GetComponent<RectTransform>();
            /*transform.FindChild("SelectingRobot").GetComponent<RectTransform>().anchoredPosition
                = SetToCanvasPos(selectedObject.transform.position);*/
        }
        if (velocity != Vector3.zero)//余韻スクロール
        {
            camera.transform.localPosition += velocity;
            LimitScroll(false);
            velocity += accel;
        }
        MoveCamera();
        floorCon.MoveMapCursor();

        if(Input.GetKeyDown(KeyCode.Space))//カメラ位置=カーネル位置に
        {
            camera.transform.localPosition = kernel.transform.localPosition - mapCorrectionPos;
            LimitScroll(false);
            floorCon.UpdateMapImage(0);
            floorCon.MoveMapCursor();
            velocity = Vector2.zero;
            accel = Vector2.zero;
        }

        cursorGO.transform.position = SetToMapPos();
    }


    public void TouchDownScreen()
    {
        keyDownPos = Input.mousePosition;
        touchingPos = keyDownPos;
    }

    public void TouchingScreen()
    {
        camera.transform.localPosition += (touchingPos - Input.mousePosition) / period * 1.5f;
        LimitScroll(false);
        touchingPos = Input.mousePosition;
    }

    public void TouchUpScreen()
    {
        Vector3 posTemp = Input.mousePosition;
        if (keyDownPos.x - swipeMargin < posTemp.x && posTemp.x < keyDownPos.x + swipeMargin
            && keyDownPos.y - swipeMargin < posTemp.y && posTemp.y < keyDownPos.y + swipeMargin)
        {
            tappedMapPos = camera.ScreenToWorldPoint(touchingPos) + mapCorrectionPos;
            Debug.Log(tappedMapPos);
            CellData c = map.GetMapData(floorCon.FloorNo, cursorGO.transform.localPosition);
            if (Input.GetMouseButtonUp(0))
            {
                if (onPanel && 0 <= panelMenu.PanelNo && c != null && c.panel == null
                    && c.partNo == (int)MapPart.floor
                    && status.ChangeCapacity(-10))//Generate a panel
                {
                    GameObject g = Instantiate(panelOrigin);
                    g.GetComponent<Panel>().command = Data.commands[panelMenu.PanelNo].CreateInstance();
                    g.transform.position = cursorGO.transform.position + Vector3.back * 0.05f;
                    g.transform.SetParent(cursorGO.transform.parent);
                    g.transform.localScale = Vector3.one;
                    c.panel = g.GetComponent<Panel>();
                }
                else if (!robotMenu.RoboRC && !onPanel && 0 <= robotMenu.RobotNo // Generate a robot
                    && c != null && ((c.panel != null
                    && c.objNo == -1 && c.panel.sanctuary && status.ChangeEnergy(-10))
                    || Input.GetKey(KeyCode.E)))
                {
                    robotMenu.GenerateRobot(cursorGO.transform);
                }
            }
            else if (Input.GetMouseButtonUp(1) && c.panel && !c.panel.cannotBreak)//右クリック、パネル削除
            {
                Destroy(c.panel.gameObject);
            }
        }
        else
        {
            velocity = (touchingPos - posTemp) / period;
            accel = -velocity / 10;
        }
        //SetStatus(selectedObject);
        LimitScroll();
    }

    //カメラのスクロール限界
    void LimitScroll(bool bound = true)
    {
        if (camera.transform.localPosition.x <  - rangeX)
        {
            camera.transform.localPosition = new Vector3( - rangeX, camera.transform.localPosition.y, posZ);
            if (bound)
            {
                velocity.x = speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.localPosition.x > rangeX)
        {
            camera.transform.localPosition
                = new Vector3(rangeX, camera.transform.localPosition.y, posZ);
            if (bound)
            {
                velocity.x = -speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.localPosition.y < -(rangeY + correctionDown))
        {
            camera.transform.localPosition 
                = new Vector3(camera.transform.localPosition.x, -(rangeY + correctionDown), posZ);
            if (bound)
            {
                velocity.y = speed;
            }
            accel = velocity / (-10);
        }
        if (camera.transform.localPosition.y > rangeY)
        {
            camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, rangeY, posZ);
            if (bound)
            {
                velocity.y = -speed;
            }
            accel = velocity / (-10);
        }
    }

    void MoveCamera()
    {
        if (Input.GetKey(KeyCode.T))//移動
        {
            velocity += Vector3.up * speed / 2;
            accel = -velocity / 10;
        }
        if (Input.GetKey(KeyCode.F))//移動
        {
            velocity += Vector3.left * speed / 2;
            accel = -velocity / 10;
        }
        if (Input.GetKey(KeyCode.G))//移動
        {
            velocity += Vector3.down * speed / 2;
            accel = -velocity / 10;
        }
        if (Input.GetKey(KeyCode.H))//移動
        {
            velocity += Vector3.right * speed / 2;
            accel = -velocity / 10;
        }
    }

    Vector3 SetToMapPos()
    {
        float centerX = camera.rect.width / 2 - 0.5f;
        float cameraAngle = 360 - camera.transform.eulerAngles.x;
        float angleX, angleZ;
        float aspectRatio;
        Vector2 posView = camera.ScreenToViewportPoint(Input.mousePosition)
            - new Vector3(/*camera.rect.width */ 0.5f, 0.5f);
        Vector2 screenSize = camera.ViewportToScreenPoint(Vector3.one);
        angleX = (cameraAngle + camera.fieldOfView * posView.y) * Mathf.PI / 180;
        aspectRatio = screenSize.x / screenSize.y;
        angleZ = camera.fieldOfView * aspectRatio * posView.x * Mathf.PI / 180;
        Vector3 targetPos = camera.transform.position
            + new Vector3(mapCorrectionPos.z * Mathf.Tan(angleZ) / Mathf.Cos(angleX),
            mapCorrectionPos.z * Mathf.Tan(angleX), mapCorrectionPos.z);
        targetPos = new Vector3(Mathf.Round(targetPos.x), Mathf.Round(targetPos.y), targetPos.z);
        return targetPos;
    }

    Vector2 SetToCanvasPos(Vector2 pos)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector2 viewportPosition = camera.WorldToViewportPoint(pos);
        Vector2 worldObject_ScreenPosition = new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        return worldObject_ScreenPosition;
    }
}
