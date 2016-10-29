using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReplayScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void onReplayClick() {
		SceneManager.LoadScene(0);
	}
}
