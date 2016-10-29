using UnityEngine;
using System.Collections;

public class LogoBehaviour : MonoBehaviour {

	void Update () {

		float rate1 = EaseFunctions.EaseIn(5.0, 1.0, 4, Music.MusicalTime - 3.0f, 1.0f, false, false);
		float rate2 = EaseFunctions.EaseOut(0.0, -0.05, 5, Music.MusicalTime - 4.0f, 8.0f, false, false);
		float opacity1 = EaseFunctions.EaseIn(0.0, 1.0, 2, Music.MusicalTime - 3.0f, 1.0f, false, false);
		float opacity2 = EaseFunctions.Linear(0.0, -1.0, Music.MusicalTime - 11.0f, 1.0f, false, false);
		float rate = rate1 + rate2;
		float opacity = opacity1 + opacity2;

		this.transform.localScale = new Vector3(rate,rate,1);
		Color tmp = this.GetComponent<SpriteRenderer>().color;
		tmp.a = opacity;
		this.GetComponent<SpriteRenderer>().color = tmp;

		if (Music.MusicalTime > 16.0f) {
			Destroy(this.gameObject);
		}
	}
}
