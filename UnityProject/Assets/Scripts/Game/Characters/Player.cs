using UnityEngine;
using System.Collections;

public class Player : Character {
    
	// Use this for initialization
	void Start () {
        base.Character_Start();
    }
	
	// Update is called once per frame
	void Update () {
        base.Character_Update();

        move(getMoveDir());
        
        //Debug.Log(turndir);
        turn(getTurnDir());

        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.reload();
        }

        

        if (Input.GetMouseButton(0))
        {
            if (weapon.gunData.firingType == "continuous")
            {
                fire();
            }

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

    public override Vector2 getMoveDir()
    {
        Vector2 movedir = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
        return movedir;
    }

    public override Vector2 getTurnDir()
    {
        Vector3 mousepoint = Input.mousePosition;
        mousepoint.z = -Camera.main.transform.position.z;
        mousepoint = Camera.main.ScreenToWorldPoint(mousepoint);
        //Debug.Log(mousepoint);

        Vector2 turndir = mousepoint - new Vector3(transform.position.x, transform.position.y);
        return turndir;
    }

    public override void hit(float damage)
    {
        base.hit(damage);
        GameManager.addHitPlayer();


    }


    public override bool isPlayer()
    {
        return true;
    }

    public override void onDeath()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }


}
