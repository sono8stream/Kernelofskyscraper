using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour {

	public string copyrightText;
	private int cnt;
	public float startTime;

	// Use this for initialization
	void Start () {
		this.gameObject.GetComponent<Text>().text = "";
	}

	void FixedUpdate () {

		float opacity = EaseFunctions.Linear(1.0, 0.0, Music.MusicalTime - 11.0f, 1.0f, false, false);
		Text textComponent = this.gameObject.GetComponent<Text>();

		if (Music.MusicalTime > startTime) {
			cnt++;
		}
		cnt = (cnt > copyrightText.Length ? copyrightText.Length : cnt);

		textComponent.text = copyrightText.Substring(0, cnt);

		Color tmp = textComponent.color;
		tmp.a = opacity;
		textComponent.color = tmp;

		if (Music.MusicalTime > 12.0f) {
				Destroy(this.gameObject);
		}

	}
}
