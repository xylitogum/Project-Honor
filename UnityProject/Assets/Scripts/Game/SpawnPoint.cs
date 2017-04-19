using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnPoint : MonoBehaviour {
	public int team_ID;

	// Use this for initialization
	void Start () {
		// add itself to the global spawnpoint list
		GameManager.instance.teams[team_ID].spawnPoints.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// spawns a character on this point
	public GameObject Spawn(GameObject prefab) {
		GameObject obj = (GameObject)Object.Instantiate(
			prefab,
			transform.position, transform.rotation
		);
		return obj;
	}

}
