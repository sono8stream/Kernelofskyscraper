using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    int no;//割り当てられたアイテム番号
    MenuController menCon;

    // Use this for initialization
    void Start()
    {
        menCon = GameObject.Find("Menu").GetComponent<MenuController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Destroy()
    {
        menCon.GetItem(no);
        Destroy(gameObject);
    }
}
