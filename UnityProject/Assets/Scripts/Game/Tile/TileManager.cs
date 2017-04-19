using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tile manager containing information about all obstacles and cover
/// points in the game. Also provides access to methods that check line of sight
/// by performing custom 2D raycasts.
/// </summary>
public class TileManager : MonoBehaviour {
	//public GameObject testerParticle;
	/// <summary>
	/// Contains all the obstacles in the game.
	/// </summary>
	[HideInInspector]
	public List<Tile> obstacles;
	/// <summary>
	/// Contains all the coverSpots in the game.
	/// </summary>
	[HideInInspector]
	public List<Tile> coverSpots;
	public int BORDER = 10; // hardcode the borderline
    public List<Tile> allSpots;


	void Awake () {
		obstacles = new List<Tile>();
		coverSpots = new List<Tile>();
        allSpots = new List<Tile>();
	}

	void Start () {
		GameManager.instance.tileManager = this;
		AddTilesFromChildren();
		// Also keep a store of all non-cover tiles
	    for (var i = -13; i <= 13; i++)
	    {
	        for (var j = -13; j <= 13; j++)
	        {
	            allSpots.Add(new Tile(i, j));
	        }
	    }
	    foreach (var coverSpot in coverSpots)
	    {
	        allSpots.Remove(coverSpot);
	    }
	}
	
	// Update is called once per frame
	void Update () {
		// Test Cases
		/*
		Vector3 a1 = new Vector3(3f,1f,3f);
		Vector3 b1 = new Vector3(3f,1f,4f);
		Vector3 a2 = new Vector3(-1f,1f,6f);
		Vector3 b2 = new Vector3(-1f,1f,8f);
		Vector3 a3 = new Vector3(1f,1f,-4f);
		Vector3 b3 = new Vector3(3f,1f,-2f);
		testRayCast(a1, b1, true);
		testRayCast(a2, b2, false);
		testRayCast(a3, b3, false);
		*/
	}

	void testRayCast(Vector3 v1, Vector3 v2, bool flag = true) {
		if (PositionCanSeePosition(v1, v2) == flag) {
			Debug.Log("Good!");
		}
		else {
			Debug.Log("ERROR");
		}

		Debug.DrawLine(v1, v2, Color.red);
		
	}

	void AddTilesFromChildren() {
		foreach (Transform child in transform) { // traverse all childs
			//Debug.Log(child.name);
			if (child.name.StartsWith("Box")) { // found a Box
				int x = Mathf.RoundToInt(child.position.x);
				int y = Mathf.RoundToInt(child.position.z);

				Tile t = new Tile(child.position);
				obstacles.Add(t); // add to tile list
				Tile topSpot = new Tile(x, y+1);
				Tile bottomSpot = new Tile(x, y-1);
				Tile leftSpot = new Tile(x-1, y);
				Tile rightSpot = new Tile(x+1, y);
				AddToCoverSpots(topSpot);
				AddToCoverSpots(bottomSpot);
				AddToCoverSpots(leftSpot);
				AddToCoverSpots(rightSpot);
			}
		}
	}

	/// <summary>
	/// Adds a tile to cover spots list. Only succeed when tile is a valid cover point.
	/// </summary>
	/// <returns><c>true</c>, if to cover spots was added, <c>false</c> otherwise.</returns>
	/// <param name="newCoverSpot">New cover spot.</param>
	bool AddToCoverSpots(Tile newCoverSpot) {
		
		if (Mathf.Abs(newCoverSpot.x) >= BORDER || Mathf.Abs(newCoverSpot.y) >= BORDER) { // check border
			return false;
		}

		foreach (Tile existingTile in coverSpots) { // check repeating
			if (existingTile.Equals(newCoverSpot)) {
				return false;
			}
		}

		foreach (Tile existingTile in obstacles) { // check validity
			if (existingTile.Equals(newCoverSpot)) {
				return false;
			}
		}

		coverSpots.Add(newCoverSpot);

		return true;
	}

	/// <summary>
	/// Check if the tile the can see specified position.
	/// Traverse the list to see if any obstacle blocks the sight
	/// </summary>
	/// <returns><c>true</c>, if can see position, <c>false</c> otherwise.</returns>
	/// <param name="whichTile">Which tile.</param>
	/// <param name="Position">Position.</param>
	public bool TileCanSeePosition (Tile whichTile, Vector3 position) {
		foreach (Tile obs in obstacles) {
			if (obs.BlocksSight(whichTile, position)) {
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Check if the Position the can see  another position.
	/// </summary>
	/// <returns><c>true</c>, if can see position was positioned, <c>false</c> otherwise.</returns>
	/// <param name="start">Start.</param>
	/// <param name="target">Target.</param>
	public bool PositionCanSeePosition (Vector3 start, Vector3 target) {
        foreach (Tile obs in obstacles)
        {
            if (obs.BlocksSight2(start, target))
            {
                return false;
            }
        }

        return true;
    }

	/// <summary>
	/// Check if a position can see another position based on the obstacles
	/// in the world and a given list of Characters that also block sight.
	/// </summary>
	/// <param name="start">The viewer</param>
	/// <param name="target">The viewee</param>
	/// <param name="blockingCharacters">The list of characters which may
	/// block line of sight</param>
	/// <returns>True if the view is unblocked by blocking characters
	/// or obstacles, else false.</returns>
	public bool PositionCanSeePosition (Vector3 start, Vector3 target, List<Character> blockingCharacters)
	{
		// Check obstacles first
		if(!PositionCanSeePosition(start, target))
		{
			return false;
		}
		// Check each character which is considered as a blocking element
		foreach (var blockingCharacter in blockingCharacters)
		{
			if(new Tile(blockingCharacter.transform.position).BlocksSight2(start, target))
			{
				return false;
			}
		}
		return true;
	}
}
