﻿using UnityEngine;
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
        SelectTutorial();
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
                Tutorial2();
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
                menuCon.WriteMessage("ようこそ、摩天楼の心臓へ！",
                    true, Color.white, 0, 0);
                break;
            case 1:
                menuCon.WriteMessage(
                    "このゲームは、\n迷宮となった摩天楼を1フロアずつ攻略していくゲームだ。",
                    false, Color.white,0,0);
                break;
            case 2:
                menuCon.WriteMessage(
                    "では、これからこのゲームの\n「チュートリアル」を始めてもよいかな？"
                    ,false, Color.white,0,0);
                break;
            case 3:
                menuCon.WriteMessage(
                    "どうもありがとう。\nそれでは続けるぞ。"
                    , false, Color.white, 0, 0);
                break;
            case 4:
                menuCon.WriteMessage(
                    "まず、フロアを攻略する条件。\nそれはただ一つ、\nエネミーを全滅させることだ。",
                    true, Color.yellow,-530,400,800,220);
                break;
            case 5:
                menuCon.WriteMessage(
                    "このフロアには４体の\nロボットが君を待ち構えている。\n奴らがエネミーだ。",
                    false, Color.white, -530, 400, 800, 220);
                menuCon.SetCautionCursor(-1, 3);
                menuCon.SetCautionCursor(-1, -3);
                menuCon.SetCautionCursor(2, 0);
                menuCon.SetCautionCursor(-4, 0);
                break;
            case 6:
                menuCon.WriteMessage(
                    "奴らを全滅させることで、\n君は次のフロアへと\n進むことができるのだ。",
                    false, Color.yellow, -530, 400, 800, 220);
                break;
            case 7:
                for (int i = 0; i < 4; i++)
                {
                    menuCon.RemoveCautionCursor();
                }
                menuCon.WriteMessage(
                    "エネミーを倒すためには、\nこちらもロボットを扱う。\n画面下のロボットをタップしよう。",
                    false, Color.green, -530, 400, 800, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 8:
                if(menuCon.transform.FindChild("Selecting").gameObject.activeSelf)
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 9:
                menuCon.WriteMessage(
                    "いいぞ。あとはロボを動かしたい\n方向のカーソルをタップしよう。",
                    false, Color.green, -530, 400, 850, 220);
                SetFilterActive(false);
                phase++;
                break;
            case 10:
                if (menuCon.terCon.rCount == 5)
                {
                    SetFilterActive(true);
                    ClickFilter();
                }
                break;
            case 11:
                menuCon.WriteMessage(
                    "その調子だ。\nロボが出現し、正面のエネミーを倒す。\nこれで君はもう、エネミーを倒せる。",
                    false, Color.white, -530, 400, 850, 220);
                break;
            case 12:
                menuCon.WriteMessage(
                    "残りは3体だ。\nさあ、蹴散らせ！",
                    false, Color.white, -530, 400, 850, 220);
                break;
            case 13:
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
                    true, Color.white, -530, 400, 800, 220);
                menuCon.transform.FindChild("ScreenToucher").gameObject.SetActive(false);
                break;
            case 1:
                menuCon.WriteMessage(
                    "まずは「パネル」について\n説明をする前に、\nこのマップを見てほしい。",
                    false, Color.yellow, -530, 400, 800, 220);
                break;
            case 2:
                menuCon.WriteMessage(
                    "このフロアーでは、ただロボを\n生成するだけではエネミーを倒せない。\nつまり、攻略することができないのだ。",
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 3:
                menuCon.WriteMessage(
                    "そこで、「パネル」が必要になる。\n画面右のメニューに\n表示されているのが「パネル」だ。",
                    false, Color.yellow, -530, 400, 800, 220);
                break;
            case 4:
                menuCon.WriteMessage(
                    "「パネル」はマップに設置し、\nそれを踏んだロボットに\n様々な効果を付与することができる。",
                    false, Color.yellow, -530, 400, 800, 220);
                break;
            case 5:
                menuCon.WriteMessage(
                    "まずは実際に使用してみて、\nイメージをつかんでみよう。",
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 6:
                menuCon.WriteMessage(
                    "今回は2段目の「右折パネル」\nを使ってみよう。\nロボの時と同様に、タップして選択だ。",
                    false, Color.green, -530, 400, 800, 220);
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
                            false, Color.green, -530, 400, 800, 220);
                    }
                }
                break;
            case 8:
                menuCon.WriteMessage(
                    "そのまま、ここをタップしてみよう。",
                    false, Color.green, -530, 400, 800, 220);
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
                    false, Color.green, -530, 400, 800, 220);
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
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 13:
                menuCon.WriteMessage(
                    "このように、ロボに対して\n特定の効果を及ぼすのが\nパネルである。",
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 14:
                menuCon.WriteMessage(
                    "また、パネルには「右折パネル」\nのような「移動型」以外にも\n様々なタイプがある。",
                    false, Color.yellow, -530, 400, 800, 220);
                break;
            case 15:
                menuCon.WriteMessage(
                    "これらをいかに組み合わせるかが、\n摩天楼の攻略において\n大きなカギとなるはずだ。",
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 16:
                menuCon.WriteMessage(
                    "これで君は、このフロアーを\n攻略できるようになった。",
                    false, Color.white, -530, 400, 800, 220);
                break;
            case 17:
                menuCon.WriteMessage(
                    "さあ、パネルを駆使して蹂躙せよ！\n*パネルはダブルタップで\n除去可能。",
                    false, Color.green, -530, 400, 800, 220);
                menuCon.transform.FindChild("ScreenToucher").gameObject.SetActive(true);
                break;
            case 18:
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