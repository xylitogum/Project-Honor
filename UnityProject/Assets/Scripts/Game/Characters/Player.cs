using UnityEngine;
using System.Collections;

/// <summary>
/// A character implementation that can be controlled by a human player.
/// </summary>
public class Player : Character {
    
	public bool HideCursor = true;

	// Use this for initialization
	void Start () {
        base.Character_Start();
		if (HideCursor) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    }
	
	// Update is called once per frame
	void Update () {
        base.Character_Update();

		if (GameManager.instance.gameState != GameManager.GameState.Running) return;

        move(getMoveDir());
        
        //Debug.Log(turndir);
        turn(getTurnDir());


        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.reload();
        }

        

        if (Input.GetMouseButton(0))
        {
			// if the weapon shoots continuously, attempt to shoot on everyframe
            if (weapon.gunData.firingType == "continuous")
            {
                fire();
            }

			// if the weapon shoots separately, attempt to shoot once.
            else if (weapon.gunData.firingType == "separate")
            {
                if (Input.GetMouseButtonDown(0)) {
                    fire();
                }
            }
            else
            {
                Debug.LogError("Weapon Firing Type could not be read: " + weapon.name);
            }
        }
    }

	// get move direction from input
    public override Vector2 getMoveDir()
    {
        Vector2 movedir = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
		//Debug.Log(movedir);
        return movedir;
    }

	// get turn direction from input
    public override Vector2 getTurnDir()
    {
		Vector3 mousepoint = Input.mousePosition;
		mousepoint.z = -Camera.main.transform.position.y;
		mousepoint = Camera.main.ScreenToWorldPoint(mousepoint);
        

        Vector3 turndir = mousepoint - new Vector3(transform.position.x, 0f, transform.position.z);
		//Debug.Log(turndir);
		return new Vector2(turndir.x, turndir.z);
    }

	// take damage
    public override void hit(float damage)
    {
        base.hit(damage);
        //GameManager.addHitPlayer();


    }
		

    public override void onDeath()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }


}
