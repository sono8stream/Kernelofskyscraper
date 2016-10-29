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
        if (DataManager.dataInstance != null && DataManager.dataInstance.onTutorial)
        {
            SelectTutorial();
            filterOn = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
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
                Tutorial2();
                break;
            case 3:
                Tutorial3();
                break;
        }
    }

    void Tutorial1()
    {
        switch (phase)
        {
            case 0:
                menuCon.WriteMessage("ようこそ、摩天楼の心臓へ！",
                    true, Color.white,true, 0, 0);
                break;
            case 1:
                menuCon.WriteMessage(
                    "このゲームは、\n迷宮となった摩天楼を1フロアずつ攻略していくゲームだ。",
                    false, Color.white, true, 0,0);
                break;
            case 2:
                menuCon.WriteMessage(
                    "では、これからこのゲームの\n進め方をお教えしよう。"
                    ,false, Color.white, true, 0,0);
                break;
            case 3:
                menuCon.WriteMessage(
                    "まず、フロアを攻略する条件。\nそれはシンプルに、\nエネミーを全滅させることだ。",
                    true, Color.yellow, true, -530,400,800,220);
                break;
            case 4:
                menuCon.WriteMessage(
                    "このフロアには４体の\nロボットが君を待ち構えている。\n奴らがエネミーだ。",
                    false, Color.white, true, -530, 400, 800, 220);
                menuCon.SetCautionCursor(-1, 3);
                menuCon.SetCautionCursor(-1, -3);
                menuCon.SetCautionCursor(2, 0);
                menuCon.SetCautionCursor(-4, 0);
                break;
            case 5:
                menuCon.WriteMessage(
                    "奴らを全滅させることで、\n君は次のフロアへと\n進むことができる。",
                    false, Color.yellow, true, -530, 400, 800, 220);
                break;
            case 6:
                for (int i = 0; i < 4; i++)
                {
                    menuCon.RemoveCautionCursor();
                }
                menuCon.WriteMessage(
                    "エネミーを倒すためには、\nこちらもロボットを扱う。\n画面下のロボットをタップしよう。",
                    false, Color.green, false, -530, 400, 800, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 7:
                if(menuCon.transform.FindChild("Selecting").gameObject.activeSelf)
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 8:
                menuCon.WriteMessage(
                    "いいぞ。あとはロボを動かしたい\n方向のカーソルをタップしよう。",
                    false, Color.green, false, -530, 400, 850, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 9:
                if (menuCon.terCon.rCount == 5)
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 10:
                menuCon.WriteMessage(
                    "その調子だ。\nロボが出現し、正面のエネミーを倒す。\nこれで君はもう、エネミーを倒せる。",
                    false, Color.white, true, -530, 400, 850, 220);
                break;
            case 11:
                menuCon.WriteMessage(
                    "残りは3体だ。\nさあ、蹴散らせ！",
                    false, Color.white, true, -530, 400, 850, 220);
                break;
            case 12:
                menuCon.CloseMessage();
                SetFilterActive(false);
                phase++;
                break;


        }
    }

    void Tutorial2()
    {
        switch (phase)
        {
            case 0:
                menuCon.WriteMessage(
                    "ここでは、「パネル」を使ってみよう。",
                    true, Color.white,true, -400,400,1000, 220);
                menuCon.transform.FindChild("ScreenToucher").gameObject.SetActive(false);
                break;
            case 1:
                menuCon.WriteMessage(
                    "まずは「パネル」の前に\nこのマップを見てほしい。",
                    false, Color.yellow, true, -400,400,1000, 220);
                break;
            case 2:
                menuCon.WriteMessage(
                    "このフロアーでは、ただロボを\n生成するだけではエネミーを倒せない。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 3:
                menuCon.WriteMessage(
                    "そこで、「パネル」が必要になる。\n画面右のメニューに\n表示されているのが「パネル」だ。",
                    false, Color.yellow, true, -400,400,1000, 220);
                break;
            case 4:
                menuCon.WriteMessage(
                    "「パネル」はマップに設置し、\nそれを踏んだロボットに\n様々な効果を付与することができる。",
                    false, Color.yellow, true, -400,400,1000, 220);
                break;
            case 5:
                menuCon.WriteMessage(
                    "まずは実際に使用してみて、\nイメージをつかんでみよう。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 6:
                menuCon.WriteMessage(
                    "今回は2段目の「右折パネル」\nを使ってみよう。\nロボの時と同様に、タップして選択だ。",
                    false, Color.green, false, -400,400,1000, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 7:
                if (menuCon.transform.FindChild("Selecting").gameObject.activeSelf)
                {
                    if (!menuCon.IsRobot && menuCon.GenNo == 1)
                    {
                        SetFilterActive(true);
                        ClickFilter();
                    }
                    else
                    {
                        menuCon.WriteMessage(
                            "それじゃない、右メニューの\n2段目のパネルを選んでくれ。",
                            false, Color.green, true, -400,400,1000, 220);
                    }
                }
                break;
            case 8:
                menuCon.WriteMessage(
                    "そのまま、ここをタップしてみよう。",
                    false, Color.green, false, -400,400,1000, 220);
                menuCon.SetCautionCursor(1, 4);
                SetFilterActive(false);
                phase++;
                break;
            case 9:
                if (!menuCon.IsRobot && menuCon.GenNo == 1)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 tPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (0.5f < tPos.x && tPos.x < 1.5f && 3.5f < tPos.y && tPos.y < 4.5f)
                        {
                            menuCon.SetPosition(false);
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        Vector2 tPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (0.5f < tPos.x && tPos.x < 1.5f && 3.5f < tPos.y && tPos.y < 4.5f)
                        {
                            SetFilterActive(true);
                            ClickFilter();
                            menuCon.RemoveCautionCursor();
                            menuCon.SetPosition(true);
                        }
                    }
                }
                break;
            case 10:
                menuCon.WriteMessage(
                    "これでパネルが設置されたはずだ。\n今度はロボットを生成し、\n上に動かしてみよう。",
                    false, Color.green, false, -400,400,1000, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 11:
                if (menuCon.terCon.rbdata[9, 8] == -1)//上にあるロボットの破壊を判定
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 12:
                menuCon.WriteMessage(
                    "「右折パネル」を踏んだロボは、\nそれに従って\n右へと曲がり、エネミーを倒した。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 13:
                menuCon.WriteMessage(
                    "このように、ロボに対して\n特定の効果を及ぼすのが\nパネルである。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 14:
                menuCon.WriteMessage(
                    "また、パネルには「右折パネル」\nのような「移動型」以外にも\n様々なタイプがある。",
                    false, Color.yellow, true, -400,400,1000, 220);
                break;
            case 15:
                menuCon.WriteMessage(
                    "これらをいかに組み合わせるかが、\n摩天楼の攻略において\n大きなカギとなるはずだ。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 16:
                menuCon.WriteMessage(
                    "これで君は、このフロアーを\n攻略できるようになった。",
                    false, Color.white, true, -400,400,1000, 220);
                break;
            case 17:
                menuCon.WriteMessage(
                    "おっと、忘れていた。\nパネルはダブルタップで\n除去可能だぞ。",
                    false, Color.yellow, true, -400,400,1000, 220);
                break;
            case 18:
                menuCon.WriteMessage(
                    "ロボを動かしている間は\n除去できないので\n注意してくれ。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 19:
                menuCon.WriteMessage(
                    "それでは、パネルを駆使して蹂躙せよ！",
                    false, Color.white, true, -400,400,1000, 220);
                menuCon.transform.FindChild("ScreenToucher").gameObject.SetActive(true);
                break;
            case 20:
                menuCon.CloseMessage();
                SetFilterActive(false);
                phase++;
                break;
        }
    }

    void Tutorial3()
    {
        switch(phase)
        {
            case 0:
                menuCon.WriteMessage("嬉しいことに、\n解説つきフロアはここまで。",
                    true, Color.white, true, -400, 400, 1000, 220);
                break;
            case 1:
                menuCon.WriteMessage("次のマップからは\nいよいよ摩天楼に挑んでいこう。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 2:
                menuCon.WriteMessage("ここでは最後に、\n「アイテム」と「スコア」について\nお教えしよう。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 3:
                menuCon.WriteMessage("まず、このようにマップ上に\n配置されているのがアイテムだ。",
                    false, Color.white, true, -400, 400, 1000, 220);
                menuCon.SetCautionCursor(-1, 0);
                break;
            case 4:
                menuCon.WriteMessage("アイテムはロボで触れるとゲットでき、\nクリア時に中身が判明する。\n非常にシンプルだ。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 5:
                menuCon.WriteMessage("しかし、フロアをクリアする前でなければ\nアイテムはゲットできないので、\n注意が必要だ。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 6:
                menuCon.WriteMessage("早速ロボを右方向に生成し、\nアイテムをゲットしてみよう。",
                    false, Color.green, false, -400, 400, 1000, 220);
                menuCon.RemoveCautionCursor();
                SetFilterActive(false);
                phase++;
                break;
            case 7:
                if (menuCon.terCon.rCount == 5)
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 8:
                menuCon.WriteMessage("これでアイテムゲットだ。\nアイテムは今後の攻略に役立つので、\n積極的にゲットしていこう。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 9:
                menuCon.WriteMessage("次は、「スコア」について。\nスコアはフロアのクリア時に\n算出されるポイントだ。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 10:
                menuCon.WriteMessage("高いスコアを獲得するほど\nロボットの性能がレベルアップする。\nこれも無視できない要素だ。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 11:
                menuCon.WriteMessage("スコアを稼ぐ基本は、\nロボが壊れる前に\nより沢山のエネミーを倒すこと。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 12:
                menuCon.WriteMessage("「コンボ」が蓄積され、高いスコアを獲得できる。\nまた、その際には使用するパネルを\n最小限にすることも重要だ。",
                    false, Color.yellow, true, -400, 400, 1000, 220);
                break;
            case 13:
                menuCon.WriteMessage("つまり、より少ない手数で\nスマートにフロアをクリアすればいい。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 14:
                menuCon.WriteMessage("それでは、高いスコアを狙って\nこのフロアを攻略してみよう。",
                    false, Color.white, true, -400, 400, 1000, 220);
                break;
            case 15:
                menuCon.CloseMessage();
                SetFilterActive(false);
                phase++;
                break;
        }
    }

    void SetFilterActive(bool state)
    {
        filter.SetActive(state);
        filterOn = state;
    }

    public void ClickFilter()
    {
        phase++;
        SelectTutorial();
    }
}
