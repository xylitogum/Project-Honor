using UnityEngine;
using System.Collections;

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

/*

using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
	private LineRenderer line;
	private bool firing;
	private Ray ray;
	private RaycastHit hit;
	public float dps;
	public float energy_cost; // every second
	public GameObject hitFX;


	// Use this for initialization
	void Start () {
		line = gameObject.GetComponent<LineRenderer> ();
		line.enabled = false;
		firing = false;


	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (firing);
		Player p = GameController.player;

		//Debug.Log(gameObject.transform.root.gameObject.name);
		if (Input.GetKey (KeyCode.Space)) {
			if (p.costEnergy(energy_cost * Time.deltaTime)) {
				if (!isFiring()) {
					StartFireLaser();
				}
				else {
					FireLaser();
				}
			}
			else {
				StopFireLaser();
			}
		} else {
			StopFireLaser();
		}

	}

	void UpdateRay() {
		ray.origin = transform.position;
		ray.direction = transform.up;
	}

	void UpdateLine() {

		line.SetWidth (0.05f, 0.05f);
		line.SetColors (Color.red, Color.red);
		line.SetPosition (0, ray.origin);
		line.SetPosition (1, ray.GetPoint (20)); 
	}


	bool isFiring() {
		return firing;
	}
	void StopFireLaser() {
		//Debug.Log ("Stop Laser");
		line.enabled = false;
		firing = false;
	}

	void StartFireLaser() {

		firing = true;
		//Debug.Log ("Start Laser");
		line.enabled = true;
		FireLaser();
	}

	void FireLaser() {
		firing = true;
		UpdateRay();
		UpdateLine();
		// raycast
		if (Physics.Raycast (ray, out hit, 20)) {
			line.SetPosition (1, hit.point);
			if (hit.rigidbody) {
				//hit.rigidbody.AddForceAtPosition(transform.up * 200f, hit.point);
			}
			GameObject gmo = hit.collider.transform.root.gameObject;
			Ship p = gmo.GetComponent<Ship> ();
			if (p != null) {
				//					Debug.Log("Hit");
				p.takeDamage (dps * Time.deltaTime);
				if (hitFX != null) {
					//hitFX.transform.position = hit.transform.position;
					Instantiate (hitFX, hit.transform.position, Quaternion.identity);
				}
			}
		} else {
			//line.SetPosition (1, ray.GetPoint (20)); 
		}


	}
}

*/