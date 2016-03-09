using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Attack : MonoBehaviour {

    bool process = false;
    public Sprite s;
    GameObject ef;
    Collider2D ot;
    // Use this for initialization
    void Start()
    {
        ef = transform.FindChild("effect").gameObject;//エフェクトオブジェ取得
    }

    // Update is called once per frame
    void Update()
    {
        /*if (process)
        {
            StartCoroutine("Effect");
        }
        else
        {
            StopCoroutine("Effect");
        }*/
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Math.Round(other.transform.position.x, 1) == Math.Round(transform.position.x, 1)
            && Math.Round(other.transform.position.y, 1) == Math.Round(transform.position.y, 1)
            && !process
            && !other.GetComponent<RobotController>().CheckAttack)
        {
            process = true;
            StartCoroutine("Effect");
            ot = other;
            Debug.Log("goo");
        }
        /*Debug.Log(Math.Round(other.transform.position.x, 1) == Math.Round(transform.position.x, 1)
            && Math.Round(other.transform.position.y, 1) == Math.Round(transform.position.y, 1));*/
    }

    IEnumerator Effect()
    {
        /*List<Collider2D> targets = new List<Collider2D>();
        targets.Add(Physics2D.OverlapPoint
            (new Vector2(transform.position.x, transform.position.y) + ot.GetComponent<RobotController>().DtoV()));
        Vector2 tpos;
        try
        {
            tpos = targets[0].transform.position;
        }
        catch
        {
            tpos = Vector2.zero;
        }
        Texture2D t = new Texture2D(120, 120, TextureFormat.RGBA32, false);
        try
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        catch
        { }
        for (int i = 0; i < 9; i++)
        {
            ot.GetComponent<RobotController>().EffectObject.transform.position = tpos;
            Color[] c = ot.GetComponent<RobotController>().AttackEffect.texture.GetPixels(120 * i, 0, 120, 120);
            t.SetPixels(0, 0, 120, 120, c);
            t.Apply();
            ot.GetComponent<RobotController>().EffectObject.GetComponent<SpriteRenderer>().sprite = 
                Sprite.Create(t, new Rect(0, 0, 120, 120), new Vector2(0.5f, 0.5f), 120);
            if (i == 8)
            {
                try
                {
                    for (int j = 0; j < targets.Count; j++)
                    {
                        targets[i].GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                catch { }
                ot.GetComponent<RobotController>().EffectObject.GetComponent<SpriteRenderer>().sprite = null;
            }
            yield return new WaitForSeconds(0.1f);
        }*/
        Texture2D t = new Texture2D(120, 120, TextureFormat.RGBA32, false);
        for (int i = 0; i < 10; i++)
        {
            Color[] c = s.texture.GetPixels(120 * i, 0, 120, 120);
            t.SetPixels(0, 0, 120, 120, c);
            t.Apply();
            ef.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, 120, 120), new Vector2(0.5f, 0.5f),120);
            if(i==5)
            {
                ot.GetComponent<RobotController>().StartCoroutine("Attack");
            }
            yield return new WaitForSeconds(0.1f);
        }
        while(ot.GetComponent<RobotController>().CheckAttack)
        {
            yield return null;
        }
        process = false;
        ef.GetComponent<SpriteRenderer>().sprite = null;
        Debug.Log("fire");
    }
}
