using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static Character player;
    public Character initPlayer;
    public Character prefab_enemy;

    private static int hit_player;
    private static int hit_enemy;

	// Use this for initialization
	void Start () {
        player = initPlayer;

    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.G))
        {
            spawnEnemy();
        }
	}

    public void spawnEnemy()
    {
        Instantiate(prefab_enemy, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 0f), 0f), Quaternion.identity);
    }

    public static void setHitPlayer(int n)
    {
        hit_player = n;
    }

    public static void setHitEnemy(int n)
    {
        hit_enemy = n;
    }

    public static void addHitPlayer()
    {
        hit_player++;
    }

    public static void addHitEnemy()
    {
        hit_enemy++;
    }

    public static int getHitPlayer()
    {
        return hit_player;
    }

    public static int getHitEnemy()
    {
        return hit_enemy;
    }

    public static bool fail()
    {
        return player.isDead();
    }
}
