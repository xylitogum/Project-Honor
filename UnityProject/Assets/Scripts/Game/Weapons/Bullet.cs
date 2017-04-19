using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private GameObject FX_hitwall;
    private float damage = 0f;
    private float damageFloat = 0f;
	private bool destroying = false;

    //public GameObject FX_hitblood;
    Character owner;



    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //transform.rotation = Quaternion.Euler();
		if (destroying) {
			Destroy(gameObject);
		}
	}

    void OnCollisionEnter(Collision coll)
    {
        // Debug.Log("Hit");
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector3 rv = coll.relativeVelocity;
        float reflectangle = Mathf.Rad2Deg * Mathf.Atan2(rv.z, rv.x);
        Vector3 rn = coll.contacts[0].normal;
        
        GameObject fx = FX_hitwall;
        if (coll.gameObject.tag != "Character")
        {
            //Debug.Log(Vector2.Angle(rn, rv));
            float ar = Vector3.Angle(rn, rv) - 90f;

			foreach (ContactPoint missileHit in coll.contacts)
			{
				Vector3 hitPoint = missileHit.point;
				GameObject ps = (GameObject)Instantiate(fx, hitPoint, Quaternion.Euler( 0f, 90f-reflectangle, 0f));
				ps.SetActive(true);
			}

            if (Random.Range(0f, 45f) <= ar || Random.Range(0f, 1f) <= 0.75f || ar < 0f)
            {
                Destroy(gameObject);
            }

        }
		else if (coll.gameObject.GetComponent<Character>() != owner) {
			DestroyOnNextFrame();
		}
		else {
			return;
		}

        


        

       
    }

	public void DestroyOnNextFrame() {
		destroying = true;
	}

    public Character getOwner()
    {
        return this.owner;
    }

    public void setOwner(Character newOwner)
    {
        this.owner = newOwner;
    }

    public float getDamage()
    {
        return this.damage;
    }
    public void setDamage(float damage)
    {
        this.damage = damage;
    }

    public float getDamageFloat()
    {
        return this.damageFloat;
    }

    public void setDamageFloat(float damageFloat)
    {
        this.damageFloat = damageFloat;
    }
}
