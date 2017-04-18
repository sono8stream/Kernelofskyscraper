using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public string name;
    public Command command;//実行するコマンド
    public bool once;//一度しか作動しない
    public bool sanctuary;//ロボ配置可能位置かどうか
    public bool cannotBreak;//破壊可能
    public int campNo;//誰にはたらくパネルか
    public bool isTrap;

    // Use this for initialization
    void Start()
    {
        if (command != null)
        {
            name = command.name;
            Transform iconT = transform.FindChild("Icon");
            iconT.GetComponent<SpriteRenderer>().sprite = command.sprite;
            iconT.eulerAngles = command.angle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (command != null && command.isDestroyed)
        {
            Destroy(gameObject);
        }
    }

    public bool Run(MapObject obj)
    {
        if (command == null)
        {
            return true;
        }
        return command.Run(obj);
    }
}

public enum CampState
{
    ally = 0, enemy = 1, neutral
}
