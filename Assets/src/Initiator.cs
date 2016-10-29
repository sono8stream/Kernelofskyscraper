using UnityEngine;
using System.Collections;

public class Initiator : MonoBehaviour {

	public int FrameRate;
	public GameObject Sound;
	public GameObject Particle;

	void Awake() {
		Application.targetFrameRate = FrameRate;
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (!Application.isShowingSplashScreen) {
			AudioSource AS = Sound.GetComponent<AudioSource>();
			ParticleSystem PS = Particle.GetComponent<ParticleSystem>();
			if (!AS.isPlaying) {
				AS.Play();
			}
			if (!PS.isPlaying) {
				PS.Play();
			}
		}
	}
}
