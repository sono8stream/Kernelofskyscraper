using UnityEngine;
using System.Collections;

public class PanelController : MonoBehaviour {

    public int direction;
    public bool turnable;
    public int t_number;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// あらかじめ指定されたパネル効果を実行
    /// </summary>
    public void Run(GameObject other)
    {
        switch (t_number)
        {
            case 0://移動
                gameObject.GetComponent<Turn>().RunPanel(other);
                break;
            case 1:
                break;
                }
    }
}
