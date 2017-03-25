using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMenu : MonoBehaviour {

    GameObject commandGOrigin;
    List<Button> commandBs;

    // Use this for initialization
    void Start()
    {
        commandGOrigin = Resources.Load<GameObject>("Prefabs/Custom/ComButton");
        InitiateCommandBs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitiateCommandBs()
    {
        commandBs = new List<Button>();
        Transform winT = transform.FindChild("Win");
        commandBs.Add(Instantiate(commandGOrigin,winT).GetComponent<Button>());
    }
}
