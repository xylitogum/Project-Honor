using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Display;

public class Team {
	#region FIELDS
	public string name;
	public Color color;
	public int score;

	public List<Character> members;
	public Character captain;

	public List<SpawnPoint> spawnPoints;

	#endregion

	#region CONSTRUCTORS
	public Team(string name, Color color) {
		this.name = name;
		this.color = color;
		score = 0;

		members = new List<Character>();
		spawnPoints = new List<SpawnPoint>();
	}

	#endregion

	#region METHODS
	public void Add(Character character) {
		members.Add(character);
		character.team = this;

		ShowTeamColor[] scripts = character.GetComponentsInChildren<ShowTeamColor>();
		if (scripts.Length > 0) {
			for (int i = 0; i < scripts.Length; i++) {
				scripts[i].ChangeTeamColor(this.color);
			}
		}
	}

	public void Remove(Character character) {
		members.Remove(character);
		character.team = null;
		GameManager.instance.checkWin();
	}

	public int Count() {
		return members.Count;
	}

	/// <summary>
	/// Remove all characters in this team from the game.
	/// </summary>
	public void ClearAll() {
		
		foreach (Character ch in members) {
			
			ch.Remove();
		}

		members.Clear();
	}

	/// <summary>
	/// Spawns given number of characters in this team
	/// </summary>
	/// <param name="size">team size.</param>
	public void Spawn(int size) {
		if (size > spawnPoints.Count) {
			Debug.LogError("Team Size(" + size + ") > Spawn Points(" + spawnPoints.Count + ")!");
			return;
		}


		for (int i = 0; i < size; i++) {
			Spawn(spawnPoints[i]);
		}


	}

	/// <summary>
	/// Spawn one character for this team.
	/// </summary>
	/// <param name="whichPoint">Where to spawn.</param>
	public void Spawn(SpawnPoint whichPoint) {
		GameObject obj = whichPoint.Spawn(GameManager.instance.character_prefab);
		Character ch = obj.GetComponent<Character>();
		Add(ch);
	}

	#endregion
}
