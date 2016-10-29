using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	public int NextSceneBuildIndex;
	public float SceneChangeTiming;

	// Update is called once per frame
	void Update () {
		if (Music.MusicalTime > SceneChangeTiming) {
			SceneManager.LoadScene(NextSceneBuildIndex);
		}
	}

}
