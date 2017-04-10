using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public string name;
    public Command command;//実行するコマンド
    public bool once;//一度しか作動しない

    // Use this for initialization
    void Start()
    {
        name = command.name;
        Transform iconT = transform.FindChild("Icon");
        iconT.GetComponent<SpriteRenderer>().sprite = command.sprite;
        iconT.eulerAngles = command.angle;
    }

    // Update is called once per frame
    void Update()
    {
        if(command.isDestroyed)
        {
            Destroy(gameObject);
        }
    }
}
