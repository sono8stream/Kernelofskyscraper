using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackGroundController : MonoBehaviour {
    float size;
    bool up;
    [SerializeField]
    GameObject barOrigin;
    [SerializeField]
    int maxBarCount;
    int length = 18;//移動長さ
    int barWaitCount;
    [SerializeField]
    float barSpeed;
    List<GameObject> bars;
    // Use this for initialization
    void Start()
    {
        size = 2;
        up = false;
        bars = new List<GameObject>();
        barWaitCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bars.Count < maxBarCount)
        {
            if (length / barSpeed / 2 < barWaitCount)
            {
                barWaitCount = 0;
                bars.Add(Instantiate(barOrigin));
                bars[bars.Count - 1].transform.position = new Vector3(-18, SetYPosition(bars.Count - 1), 0);
            }
            else
            {
                barWaitCount++;
            }
        }
        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].transform.position += Vector3.right * barSpeed;
            if (length < bars[i].transform.position.x)
            {
                GameObject sub = bars[i];
                bars.RemoveAt(i);
                i--;
                Destroy(sub);
            }
        }
    }

    float SetYPosition(int barNo)
    {
        float y = Random.Range(-4, 4);
        while ((barNo > 0 && y == bars[barNo - 1].transform.position.y)
            || (barNo == 0 && y == bars[bars.Count - 1].transform.position.y))
        {
            y = Random.Range(-4, 4);
        }
        return y;
    }
}
