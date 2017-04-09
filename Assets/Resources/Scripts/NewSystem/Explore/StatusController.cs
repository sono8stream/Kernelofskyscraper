using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    #region Property
    [SerializeField]
    MapLoader map;
    [SerializeField]
    RectTransform engGaugeRT, capaGaugeRT, flashGaugeRT;
    [SerializeField]
    int engLim, capaLim, flashLim;
    int eng, capa, flash;//ロボ生成量//パネル生成量//必殺ゲージ
    int engSeed, capaSeed, flashSeed;
    float gaugeLength;
    #endregion

    // Use this for initialization
    void Start()
    {
        gaugeLength = engGaugeRT.sizeDelta.x;
        eng = engLim;
        capa = capaLim;
        flash = flashLim;
        engSeed = eng;
        capaSeed = capa;
        flashSeed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (eng != engSeed)
        {
            eng = Mathf.Abs(eng - engSeed) <= 1 ? eng = engSeed : (eng + engSeed) / 2;
            SetEnergyBar();
        }
        if (capa != capaSeed)
        {
            capa = Mathf.Abs(capa - capaSeed) <= 1 ? capa = capaSeed : (capa + capaSeed) / 2;
            SetCapacityBar();
        }
        if (flash != flashSeed)
        {
            flash = Mathf.Abs(flash - flashSeed) <= 1 ? flash = flashSeed : (flash + flashSeed) / 2;
            SetFlashBar();
        }
    }

    public bool ChangeEnergy(int val)
    {
        engSeed += val;
        if (engSeed <= 0)
        {
            engSeed = eng;
            return false;
        }
        else if (engLim < engSeed)
        {
            engSeed = engLim;
        }
        return true;
    }

    public bool ChangeCapacity(int val)
    {
        capaSeed += val;
        if (capaSeed <= 0)
        {
            capaSeed = capa;
            return false;
        }
        else if (capaLim < capaSeed)
        {
            capaSeed = capaLim;
        }
        return true;
    }

    public bool ChangeFlash(int val)
    {
        flashSeed += val;
        if (flashSeed <= 0)
        {
            flashSeed = flash;
            return false;
        }
        else if (flashLim < flashSeed)
        {
            flashSeed = flashLim;
        }
        return true;
    }

    void SetEnergyBar()
    {
        engGaugeRT.sizeDelta = new Vector2(gaugeLength * eng / engLim, engGaugeRT.sizeDelta.y);
    }

    void SetCapacityBar()
    {
        capaGaugeRT.sizeDelta = new Vector2(gaugeLength * capa / capaLim, capaGaugeRT.sizeDelta.y);
    }

    void SetFlashBar()
    {
        flashGaugeRT.sizeDelta = new Vector2(gaugeLength * flash / flashLim, flashGaugeRT.sizeDelta.y);
    }
}
