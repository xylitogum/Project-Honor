using UnityEngine;
using System.Collections;

public class AIAgent : Character {

    // Use this for initialization
    void Start () {
        base.Character_Start();

    }

    // Update is called once per frame
    void Update () {
        base.Character_Update();

		if (GameManager.instance.gameState != GameManager.GameState.Running) return;

        move(getMoveDir());
        turn(getTurnDir());

		if (shouldFire()) {
			fire();
		}

		if (shouldReload()) {
			reload();
		}
    }

	//TODO : Tell AI character when to fire
	public bool shouldFire() {
		return true;
	}

	//TODO : Tell AI character when to reload
	public bool shouldReload() {
		return false;
	}

	//TODO : Tell AI character where to move;
    public override Vector2 getMoveDir()
    {
        
		return Vector2.zero;
    }

	//TODO : Tell AI character where to turn;
    public override Vector2 getTurnDir()
    {
        
		return getFacingDir();
    }

    public override void hit(float damage)
    {
        base.hit(damage);
    }



}
