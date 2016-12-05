using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour {
    public float speed;
    float health;
    public float maxHealth;
    public float healthRecovery;
    public CharacterWeapon weapon;
    public GameObject FX_hit_blood;
    public GameObject FX_Crit_hit_blood;

    float intervalCenter = 0.12f; // inner circle
    float intervalMiddle = 0.5f; // middle circle

    // Use this for initialization
    public void Character_Start () {
        this.health = maxHealth;
        this.weapon.setOwner(this);
        

    }

    // Update is called once per frame
    public void Character_Update () {
        if (this.health < this.maxHealth) {
            this.health += healthRecovery * Time.deltaTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            GameObject fx = FX_hit_blood;

            // calculating point of FX

            RaycastHit hitray;
            Vector3 hitpoint = other.transform.position;
            if (Physics.Raycast(transform.position, transform.forward, out hitray))
            {
                hitpoint = hitray.point;
            }
            // calculating direction of FX

            Vector2 rv = other.GetComponent<Rigidbody2D>().velocity;// - this.GetComponent<Rigidbody2D>().velocity;
            float reflectangle = Mathf.Rad2Deg * Mathf.Atan2(rv.y, rv.x);

            

            
            // hit
            if (this != other.GetComponent<Bullet>().getOwner())
            {
                Bullet blt = other.gameObject.GetComponent<Bullet>();

                // calculating distance from center
                Vector2 projectilePoint = other.transform.position;
                Vector2 projectileDirection = other.GetComponent<Rigidbody2D>().velocity;
                Vector2 origin = transform.position;
                float closest = projectilePoint.LineClosestDistance(projectileDirection, origin);
				float hitBoxRadius = Mathf.Max(GetComponent<CapsuleCollider2D>().size.x, GetComponent<CapsuleCollider2D>().size.y)/2;
                //Debug.Log(closest + "/" + hitBoxRadius);

                // calculate Damage
                float damage = blt.getDamage();
                
                float closestRatio = closest / hitBoxRadius;
                float damageFloatCof = 0f;
                if (closestRatio < intervalCenter) // inner circle
                {
                    damageFloatCof = 1f;
                }
                else if (closestRatio < intervalMiddle) // middle circle
                {
                    damageFloatCof = 0.3f;
                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        damageFloatCof = 0.6f;
                    }
                }
                else // outer circle
                {
                    if (Random.Range(0f, 1f) < 0.3f)
                    {
                        damage = 0f;
                    }
                }
                // calc done

                damage += blt.getDamageFloat() * damageFloatCof;
                //Debug.Log(closestRatio);
                if (damage >= 0.01f)
                {
                    // Crit FX
                    if (damageFloatCof >= 0.99f)
                    {
                        fx = FX_Crit_hit_blood;
                        // full blood
                        GameObject fx1 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(reflectangle, -90f, 0f));
                        ParticleSystem ps1 = fx1.GetComponent<ParticleSystem>();
                        var vel1 = ps1.velocityOverLifetime;
                        vel1.enabled = true;
                        vel1.space = ParticleSystemSimulationSpace.World;
                        vel1.x = rb.velocity.x;
                        vel1.y = rb.velocity.y;
                        vel1.z = 0;
                        ps1.Emit((int)Random.Range(55f, 85f));
                    }
                    else
                    {
                        fx = FX_hit_blood;
                        // income blood
                        GameObject fx1 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(reflectangle, -90f, 0f));
                        ParticleSystem ps1 = fx1.GetComponent<ParticleSystem>();
                        ps1.startLifetime = 0.50f;
                        var vel1 = ps1.velocityOverLifetime;
                        vel1.enabled = true;
                        vel1.space = ParticleSystemSimulationSpace.World;
                        vel1.x = rb.velocity.x;
                        vel1.y = rb.velocity.y;
                        vel1.z = 0;
                        ps1.Emit((int)Random.Range(25f, 35f));

                        // outleft blood
                        GameObject fx2 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(180f + reflectangle, -90f, 0f));
                        ParticleSystem ps2 = fx2.GetComponent<ParticleSystem>();
                        ps1.startLifetime = 0.60f;
                        ps2.startSize = 0.2f;
                        var sh = ps2.shape;
                        sh.angle = 25f;
                        var vel2 = ps2.velocityOverLifetime;
                        vel2.enabled = true;
                        vel2.space = ParticleSystemSimulationSpace.World;
                        vel2.x = rb.velocity.x;
                        vel2.y = rb.velocity.y;
                        vel2.z = 0;
                        ps2.Emit((int)Random.Range(55, 85));
                    }



                    // HIT

                    if (blt.getOwner().isEnemy() != this.isEnemy())
                    {
                        hit(damage);
                    }
                    Physics2D.IgnoreCollision(other, GetComponent<Collider2D>(), true);
                    blt.setOwner(this);



                    if (damage <= 3f)
                    {
                        Destroy(other.gameObject);
                    }
                    else
                    {
                        other.GetComponent<Rigidbody2D>().velocity = 0.4f * other.GetComponent<Rigidbody2D>().velocity;
                        blt.setDamage(0.5f * blt.getDamage());
                        blt.setDamageFloat(0.5f * blt.getDamageFloat());
                    }
                }
                
                


            }
            

            
        }
    }

    public void move(Vector2 direction)
    {
        Vector2 normdir = direction.normalized;
        Vector2 disp = speed * normdir;
        Vector3 d = new Vector3(disp.x, disp.y, 0f);

        transform.GetComponent<Rigidbody2D>().velocity = d;
    }

    public void fire()
    {
       
        this.weapon.fire(getFacingDir());
    }

    public void turn(Vector2 direction)
    {
        //Debug.Log(direction);
        float rot = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90f;

        transform.rotation = Quaternion.Euler(0f, 0f, rot);
    }

    public virtual Vector2 getMoveDir()
    {
        return Vector2.zero;
    }

    public virtual Vector2 getTurnDir()
    {
        return Vector2.zero;
    }

    public Vector2 getFacingDir()
    {
        float rot = transform.rotation.eulerAngles.z - 270f;
        //Debug.Log(rot);
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * rot), Mathf.Sin(Mathf.Deg2Rad *  rot));
        return direction;
    }

    public CharacterWeapon getWeapon()
    {
        return this.weapon;
    }

    public void setWeapon(CharacterWeapon newWeapon)
    {
        this.weapon = newWeapon;        
    }

    public float getHealth()
    {
        return this.health;
    }

    public void setHealth(float health)
    {
        this.health = health;
    }

    public float getMaxHealth()
    {
        return this.maxHealth;
    }

    public void setMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    public virtual void hit(float damage)
    {
        //Debug.Log(damage);
        takeDamage(damage);
    }

    public virtual void takeDamage(float damage)
    {
        this.health -= damage;
        if (isDead())
        {
            onDeath();
        }
    }
    public virtual bool isDead()
    {
        return this.health <= 0;
    }

    public virtual void onDeath()
    {
        Destroy(gameObject);
    }

    public virtual bool isPlayer()
    {
        return false;
    }

    public virtual bool isEnemy()
    {
        return false;
    }

   
}
