using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEffectDestroyer : MonoBehaviour {
    
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, transform.GetChild(0).GetComponent<ParticleSystem>().main.duration);
    }
}
