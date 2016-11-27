using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    [SerializeField]
    private GameObject FX_hitwall;
    private float damage = 0f;
    private float damageFloat = 0f;

    //public GameObject FX_hitblood;
    Character owner;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //transform.rotation = Quaternion.Euler();
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        // Debug.Log("Hit");
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 rv = coll.relativeVelocity;
        float reflectangle = Mathf.Rad2Deg * Mathf.Atan2(rv.y, rv.x);
        Vector2 rn = coll.contacts[0].normal;
        
        GameObject fx = FX_hitwall;
        if (coll.gameObject.tag != "Character")
        {
            //Debug.Log(Vector2.Angle(rn, rv));
            float ar = Vector2.Angle(rn, rv) - 90f;
            if (Random.Range(0f, 45f) <= ar || Random.Range(0f, 1f) <= 0.75f || ar < 0f)
            {
                Destroy(gameObject);
            }
        }

        


        foreach (ContactPoint2D missileHit in coll.contacts)
        {
            Vector3 hitPoint = missileHit.point;
            GameObject ps = (GameObject)Instantiate(fx, hitPoint, Quaternion.Euler(reflectangle, -90f, 0f));
        }

       
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
