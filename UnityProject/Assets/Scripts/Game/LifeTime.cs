using UnityEngine;
using System.Collections;

public class LifeTime : MonoBehaviour {
    public float lifetime;
    float starttime;
	// Use this for initialization
	void Awake () {
        starttime = Time.fixedTime;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Time.fixedTime - starttime >= lifetime)
        {
            Destroy(gameObject);
        }

    }
}
