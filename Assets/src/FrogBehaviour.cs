using UnityEngine;
using System.Collections;

public class FrogBehaviour : MonoBehaviour {

	// Update is called once per frame
	void Update () {

		float rate1 = EaseFunctions.EaseIn(0.95, 1.0, 1, Music.MusicalTime, 3.0f, false, false);
		float rate2 = EaseFunctions.EaseIn(0.0, 29, 2, Music.MusicalTime-2.75f, 1.0f, false, false);
		float opacity1 = EaseFunctions.Linear(0.0, 1.0, Music.MusicalTime, 2.5f, false, false);
		float opacity2 = EaseFunctions.EaseIn(0.0, -1.0, 2, Music.MusicalTime - 3.0f, 1.0f, false, false);
		float rate = rate1 + rate2;
		float opacity = opacity1 + opacity2;

		this.transform.localScale = new Vector3(rate,rate,1);
		Color tmp = this.GetComponent<SpriteRenderer>().color;
		tmp.a = opacity;
		this.GetComponent<SpriteRenderer>().color = tmp;

		if (Music.MusicalTime > 5.0f) {
			Destroy(this.gameObject);
		}
	}

}
