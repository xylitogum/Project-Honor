using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Text txt = GetComponent<Text>();
        txt.text = GameManager.getHitPlayer() + " : " + GameManager.getHitEnemy();
	}



}
