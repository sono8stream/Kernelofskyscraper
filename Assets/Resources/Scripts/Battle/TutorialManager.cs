using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

    GameObject menu;
    MenuController menuCon;
    GameObject filter;//入力受付用フィルター
    bool filterOn;
    [SerializeField]
    int tutoNo;//チュートリアル番号
    [SerializeField]
    int phase;

    // Use this for initialization
    void Start()
    {
        menu = GameObject.Find("Menu");
        menuCon = menu.GetComponent<MenuController>();
        filter = transform.FindChild("Filter").gameObject;
        phase = 0;
        Tutorial1();
        filterOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!filterOn)
        {
            SelectTutorial();
        }
    }

    void SelectTutorial()
    {
        switch(tutoNo)
        {
            case 1:
                Tutorial1();
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    void Tutorial1()
    {
        switch (phase)
        {
            case 0:
                menuCon.WriteMessage("ようこそ、摩天楼に選ばれし者よ！",
                    true, Color.white, 0, 0);
                break;
            case 1:
                menuCon.WriteMessage(
                    "これから君は、摩天楼を1フロアずつ攻略し、屋上を目指すことになる。",
                    false, Color.white,0,0);
                break;
            case 2:
                menuCon.WriteMessage(
                    "親切な私は、これから\n数フロアに分けてその攻略方法を教えよう。"
                    ,false, Color.white,0,0);
                break;
            case 3:
                menuCon.WriteMessage(
                    "まず、フロアを攻略する方法。\nそれはただ一つ、\nエネミーを全滅させることだ。",
                    true, Color.yellow,500,400,800,220);
                break;
            case 4:
                menuCon.WriteMessage(
                    "このフロアには４体のロボットが君を待ち構えている。\n奴らがエネミーだ。",
                    false, Color.white, 500, 400, 800, 220);
                menuCon.SetCautionCursor(-1, 3);
                menuCon.SetCautionCursor(-1, -3);
                menuCon.SetCautionCursor(2, 0);
                menuCon.SetCautionCursor(-4, 0);
                break;
            case 5:
                menuCon.WriteMessage(
                    "奴らを全滅させることで、君は\n次のフロアへと進むことができる。",
                    false, Color.yellow, 500, 400, 800, 220);
                break;
            case 6:
                for (int i = 0; i < 4; i++)
                {
                    menuCon.RemoveCautionCursor();
                }
                menuCon.WriteMessage(
                    "エネミーを倒すためには、\nこちらもロボットを扱う。\n画面下のロボットをタップしよう。",
                    false, Color.green, 500, 400, 800, 220);
                filter.SetActive(false);
                filterOn = false;
                phase++;
                break;
            case 7:
                if(menuCon.transform.FindChild("CommandList").FindChild("Selecting").gameObject.activeSelf)
                {
                    filter.SetActive(true);
                    filterOn = true;
                    filterClick();
                }
                break;
            case 8:
                menuCon.WriteMessage(
                    "いいぞ。\nあとはロボを動かしたい\n方向カーソルをタップしよう。",
                    true, Color.green, -500, 360, 800, 300);
                filter.SetActive(false);
                filterOn = false;
                phase++;
                break;
            case 9:
                if (menuCon.terCon.rCount == 5)
                {
                    filter.SetActive(true);
                    filterOn = true;
                    filterClick();
                }
                break;
            case 10:
                menuCon.WriteMessage(
                    "OK.\nロボが出現し、前方のエネミーを倒す。\nこれで君は、エネミーを倒せる。",
                    false, Color.white, -500, 360, 800, 300);
                break;
            case 11:
                menuCon.WriteMessage(
                    "残りは3体だ。\nさあ、蹴散らせ！",
                    false, Color.white, -500, 360, 800, 300);
                filter.SetActive(false);
                filterOn = false;
                phase++;
                break;
            case 12:
                if (menuCon.eCount == 0)
                {
                    menuCon.CloseMessage();
                }
                break;


        }
    }

    public void filterClick()
    {
        phase++;
        SelectTutorial();
    }
}
