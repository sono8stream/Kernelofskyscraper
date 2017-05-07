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
    public bool isTrap;
    public bool onDestroy;
    public int campNo;//誰にはたらくパネルか
    
    int co;
    int lim;
    GameObject particle;

    [SerializeField]
    GameObject genEffect, delEffect;
    [SerializeField]
    AudioClip generateSE, breakSE, runSE;
    AudioSource audioSource;

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
        particle = transform.FindChild("Particle") ? transform.FindChild("Particle").gameObject : null;

        audioSource = GetComponent<AudioSource>();
        Effect(genEffect);
        audioSource.PlayOneShot(generateSE);
    }

    // Update is called once per frame
    void Update()
    {
        if (onDestroy && delEffect)
        {
            Effect(delEffect);
            Break();
            Destroy(gameObject);
        }

        if (command != null && command.isDestroyed
            && !particle.activeSelf)
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
        if (particle&&!particle.activeSelf)
        {
            transform.FindChild("Particle").gameObject.SetActive(true);
            audioSource.PlayOneShot(runSE);
        }
        return command.Run(obj);
    }

    void Effect(GameObject effect)
    {
        if (!effect) { return; }
        GameObject g = Instantiate(effect);
        g.transform.position = transform.position;
        g.transform.localScale = Vector3.one;
    }

    public void Break()
    {
        Debug.Log("Called");
        GameObject g = new GameObject();
        g.transform.position = transform.position;
        g.AddComponent(typeof(AudioSource));
        audioSource = g.GetComponent<AudioSource>();
        audioSource.maxDistance = 30;
        audioSource.PlayOneShot(breakSE);
        Destroy(g, breakSE.length);
        Effect(delEffect);
    }
}

public enum CampState
{
    ally = 0, enemy = 1, neutral
}
