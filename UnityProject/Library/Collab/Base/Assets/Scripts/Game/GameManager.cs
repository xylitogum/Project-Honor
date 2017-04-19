using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	/// <summary>
	/// Use GameManager.instance to get access to the GameManager from outside.
	/// </summary>
	public static GameManager instance;
    [Header("StrategicFSM")]
    public Commander_FSM commander;

    private CommandPassingManager passingManager;

	[Header("Prefabs")]
	public GameObject character_prefab;

	[Header("Team Settings")]
	public int teamSize;
	[HideInInspector]
	public List<Team> teams;
	public List<string> teamNames;
	public List<Color> teamColors;
	[HideInInspector]
	public TileManager tileManager;


    [HideInInspector]
	public GameState gameState;
	public enum GameState {
		Paused,
		Running
	}

	[Header("Etc.")]
	[HideInInspector]
	public Team lastWinner = null;
	private List<Team> activeTeams;


	void Awake () {
		instance = this;
		activeTeams = new List<Team>();
		// initializing teams
		teams = new List<Team>();
		for (int i = 0; i < teamNames.Count; i++) {
			Team tm = new Team(teamNames[i], teamColors[i]);

			teams.Add(tm);
		}
		gameState = GameState.Paused;
	}

	// Use this for initialization
	void Start ()
	{
        //Time.timeScale = 2f;
        if (teamSize > 0) Reset();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate () {
		
		if (gameState == GameState.Running) checkWin();
	    if (activeTeams.Count < teams.Count)
	    {
            Debug.Log("Red team score: " + teams[0].score + ", Blue team score: " + teams[1].score);
	        Reset();
	    }
        commander.Update();
        passingManager.Update();
    }

    /// <summary>
    /// Reset everything and start a new game
    /// </summary>
    public void Reset() {
		
		activeTeams = new List<Team>(teams);
		foreach (Team tm in teams) {
			tm.ClearAll();
			tm.Spawn(teamSize);
			tm.ConstructHierarchy();
		}
		gameState = GameState.Running;
        commander = new Commander_FSM();
        commander.Start();
        passingManager = new CommandPassingManager();
        passingManager.Start();
    }


	/// <summary>
	/// Check if any team met the winning condition
	/// </summary>
	public void checkWin() {
		if (gameState != GameState.Running) return;
		updateActiveTeam();

		if (activeTeams.Count <= 1) {
			if (activeTeams.Count == 1) {
				winTeam(activeTeams[0]);

			}
		}
	}

	/// <summary>
	/// Claim the specified team as winner.
	/// </summary>
	/// <param name="winner">Winner.</param>
	public void winTeam(Team winner) {
		gameState = GameState.Paused;
		winner.score += 1;
		lastWinner = winner;
		Debug.Log("Team " + winner.name + " Wins!");
	}


	public bool updateActiveTeam() {
		bool flag = false;
		foreach(Team tm in teams) {
			if (tm.Count() <= 0) {
				Debug.Log("Team " + tm.name + " Lost!");
				flag = activeTeams.Remove(tm);
			}
		}

		return flag;
	}


}
