using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

    GameObject menu;
    MenuController menuCon;
    GameObject filter;//入力受付用フィルター
    int phase;

    // Use this for initialization
    void Start()
    {
        menu = GameObject.Find("Menu");
        menuCon = menu.GetComponent<MenuController>();
        filter = transform.FindChild("Filter").gameObject;
        phase = 0;
        Tutorial1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Tutorial1()
    {
        switch(phase)
        {
            case 0:
                menuCon.WriteMessage("ようこそ、摩天楼に選ばれし者よ！");
                break;
            case 1:
                menuCon.WriteMessage(
                    "天高くそびえる摩天楼を攻略するため、\n君は摩天楼を1フロアずつ登っていくことになる。");
                break;
            case 2:
                menuCon.WriteMessage(
                    "ではこれから数階に分けて、そのフロアの攻略方法を伝えよう。");
                break;
            case 3:
                menuCon.WriteMessage(
                    "まずフロアを攻略する方法はただ一つ存在する。\nそれは、エネミーを全滅させることだ。");
                break;
            case 4:
                menuCon.WriteMessage(
                    "このフロアには４体のロボが君を待ち構えているのがわかるだろう。\n奴らがエネミーである。");
                break;
            case 5:
                menuCon.WriteMessage(
                    "摩天楼には生身の人間は危険すぎて入れない。\nその為、カーネルを用いるのだ。");
                break;

        }
    }

    public void filterClick()
    {
        phase++;
        Tutorial1();
    }
}
