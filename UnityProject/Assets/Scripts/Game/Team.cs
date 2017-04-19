using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Display;
using Tree;

/// <summary>
/// Represents a team of cooperating agents. Agents on the same
/// team share a spawn location and do not receive damage from
/// friendly fire.
/// </summary>
public class Team {
	#region FIELDS
	public string name;
	public Color color;
	public int score;

	public List<Character> members;
	public Character captain;
	public TreeNode<Character> teamHierarchy;

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
	/// <summary>
	/// Add a character to the team
	/// </summary>
	/// <param name="character">The character to add</param>
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

	/// <summary>
	/// Remove a character from the team
	/// </summary>
	/// <param name="character">The character to remove</param>
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

	/// <summary>
	/// Builds the team structure into a binary tree. Requires that
	/// there be at least one member on the team.
	/// </summary>
	public void ConstructHierarchy()
	{
		Queue<TreeNode<Character>> queue = new Queue<TreeNode<Character>>();
		var index = 0;
		captain = members[index];
		teamHierarchy = new TreeNode<Character>(members[index]);
		queue.Enqueue(teamHierarchy);
		index++;
		while(index < members.Count)
		{
			var parent = queue.Dequeue();
			var child1 = new TreeNode<Character>(members[index]);
			parent.AddChild(child1);			
			queue.Enqueue(child1);
			index++;
			if(index < members.Count)
			{
				var child2 = new TreeNode<Character>(members[index]);
				parent.AddChild(child2);
				queue.Enqueue(child2);
			    index++;
			}
		}
	}

	#endregion
}
