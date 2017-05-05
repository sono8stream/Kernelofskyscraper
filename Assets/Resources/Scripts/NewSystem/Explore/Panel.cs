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

    bool onGenerate;
    bool onDestroy;
    int co;
    int lim;

    [SerializeField]
    GameObject genEffect, delEffect;

    // Use this for initialization
    void Start()
    {
        if (command != null)
        {
            name = command.name;
            Transform iconT = transform.FindChild("Icon");
            if (iconT != null)
            {
                iconT.GetComponent<SpriteRenderer>().sprite = command.sprite;
                iconT.eulerAngles = command.angle;
            }
        }

        lim = 10;
        onGenerate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (onGenerate && genEffect)
        {
            GameObject g = Instantiate(genEffect);
            g.transform.position = transform.position;
            g.transform.localScale = Vector3.one;
            onGenerate = false;
        }

        if (onDestroy && delEffect)
        {
            GameObject g = Instantiate(delEffect);
            g.transform.position = transform.position;
            g.transform.localScale = Vector3.one;
            Destroy(gameObject);
        }

        if (command != null && command.isDestroyed
            && !transform.FindChild("Particle").gameObject.activeSelf)
        {
            onDestroy = true;
        }
    }

    public bool Run(MapObject obj)
    {
        if (command == null || command.isDestroyed)
        {
            return true;
        }
        if(transform.FindChild("Particle"))
        {
            transform.FindChild("Particle").gameObject.SetActive(true);
        }
        return command.Run(obj);
    }
}

public enum CampState
{
    ally = 0, enemy = 1, neutral
}
