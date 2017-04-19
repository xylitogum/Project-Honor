using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.FSM_Strategic;
using UnityEngine.AI;

[RequireComponent (typeof (CapsuleCollider))]
[RequireComponent (typeof (Rigidbody))]
public abstract class Character : MonoBehaviour {
	public Team team;
    public float speed;
    float health;
    public float maxHealth;
    public float healthRecovery;
    public CharacterWeapon weapon;
    public GameObject FX_hit_blood;
    public GameObject FX_Crit_hit_blood;

    public Vector3 destination;     // point on the map that the character is currently pathing to
    public bool orders = false;     // whether or not the character is executing Strategic AI commands
    public StrategicCommand strategicOrders;
    int destTimer = 0;

    float HitBoxCenter = 0.15f; // inner circle
    float HitBoxMiddle = 0.7f; // middle circle

    private NavMeshAgent navMeshAgent;

    // Use this for initialization
    public void Character_Start () {
        this.health = maxHealth;
        this.weapon.setOwner(this);

        destination = this.transform.position;
        //orders = false;

        // Don't allow the NavMeshAgent to modify the agent's rotation
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.avoidancePriority = (int)(Random.Range(0f, 1f) * 100);
    }

    // Update is called once per frame
    public void Character_Update () {
        if (this.health < this.maxHealth) {
            this.health += healthRecovery * Time.deltaTime;
        }
    }

    public void OnCollisionEnter(Collision col) {
		
		if (col.gameObject.tag == "Projectile") {
			Rigidbody rb = GetComponent<Rigidbody>();
            GameObject fx = FX_hit_blood;

            // calculating point of FX

            //RaycastHit hitray;
			Vector3 hitpoint = col.contacts[0].point;

            // calculating direction of FX

			Vector3 rv = col.relativeVelocity;
            float reflectangle = Mathf.Rad2Deg * Mathf.Atan2(rv.z, rv.x);

			Bullet blt = col.gameObject.GetComponent<Bullet>();

            // hit enemy
			if (this.team != blt.getOwner().team) {
				
                // calculating distance from center
				Vector2 projectilePoint = new Vector2(col.transform.position.x, col.transform.position.z);
				Vector2 projectileDirection = new Vector2(col.relativeVelocity.x, col.relativeVelocity.z);
				Vector2 origin = new Vector2(transform.position.x, transform.position.z);
                float closest = projectilePoint.LineClosestDistance(projectileDirection, origin);
				float hitBoxRadius = GetComponent<CapsuleCollider>().radius;
                //Debug.Log(closest + "/" + hitBoxRadius);

                // calculate Damage

				float closestRatio = closest / hitBoxRadius;
				float damage = calcDamage(blt.getDamage(), blt.getDamageFloat(), closestRatio);
                // calc done

                
                //Debug.Log(closestRatio);
				if (damage >= 0.01f) { // deals damage
                    // Crit FX
					if (damage > blt.getDamage() + blt.getDamageFloat() * 0.999f) { //
                        fx = FX_Crit_hit_blood;
                        // full blood
                        GameObject fx1 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(reflectangle, -90f, 0f));
                        ParticleSystem ps1 = fx1.GetComponent<ParticleSystem>();
                        var vel1 = ps1.velocityOverLifetime;
                        vel1.enabled = true;
                        vel1.space = ParticleSystemSimulationSpace.World;
                        vel1.x = rb.velocity.x;
                        vel1.y = rb.velocity.y;
						vel1.z = rb.velocity.z;
                        ps1.Emit(Random.Range(55, 85));
                    }
                    else
                    {
                        fx = FX_hit_blood;
                        // income blood
                        GameObject fx1 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(0f, -reflectangle-90f, 0f));
                        ParticleSystem ps1 = fx1.GetComponent<ParticleSystem>();
						var ma1 = ps1.main;
                        ma1.startLifetime = 0.50f;
                        var vel1 = ps1.velocityOverLifetime;
                        vel1.enabled = true;
                        vel1.space = ParticleSystemSimulationSpace.World;
                        vel1.x = rb.velocity.x;
                        vel1.y = rb.velocity.y;
						vel1.z = rb.velocity.z;
						ps1.Emit(Random.Range(25, 35));

                        // outleft blood
                        GameObject fx2 = (GameObject)Instantiate(fx, hitpoint, Quaternion.Euler(0f, -reflectangle+90f, 0f));
                        ParticleSystem ps2 = fx2.GetComponent<ParticleSystem>();
						var ma2 = ps1.main;
                        ma2.startLifetime = 0.60f;
                        ma2.startSize = 0.2f;
                        var sh = ps2.shape;
                        sh.angle = 25f;
                        var vel2 = ps2.velocityOverLifetime;
                        vel2.enabled = true;
                        vel2.space = ParticleSystemSimulationSpace.World;
                        vel2.x = rb.velocity.x;
                        vel2.y = rb.velocity.y;
						vel2.z = rb.velocity.z;
                        ps2.Emit((int)Random.Range(55, 85));
                    }



                    // HIT

					if (blt.getOwner().team != this.team) {
                        hit(damage);
                    }

					Physics.IgnoreCollision(col.collider, GetComponent<Collider>(), true);
                    //blt.setOwner(this);

					// Destroy the bullet
					Destroy(col.gameObject);

                }
                
                


            }
            

            
        }
    }




	/// <summary>
	/// Calculates the damage.
	/// </summary>
	/// <returns>The damage.</returns>
	/// <param name="baseDamage">Base damage.</param>
	/// <param name="floatDamage">Float damage.</param>
	/// <param name="closestRatio">ClosestRatio.</param>
	private float calcDamage(float baseDamage, float floatDamage, float closestRatio) {
		float damage = baseDamage;
		float damageFloatCof = 0f;
		if (closestRatio < HitBoxCenter) { // inner circle
			damageFloatCof = 1f; // Full Damage (base + float)
		}
		else if (closestRatio < HitBoxMiddle) { // middle circle
			damageFloatCof = 0.3f; // do 30% float damage
			if (Random.Range(0f, 1f) < 0.25f) { // 25% chance do 60% float damage
				damageFloatCof = 0.6f;
			}
		}
		else { // outer circle
			// do 0% float damage
			if (Random.Range(0f, 1f) < 0.25f) { // 25% chance do no damage
				damage = 0f;
			}
		}
		damage += floatDamage * damageFloatCof;
		return damage;
	}

	#region COMMAND
	/// <summary>
	/// Immediately attempts to fire towards current direction.
	/// It may not always succeed depending on weapon cooldown.
	/// </summary>
    public bool fire() {
       
        return this.weapon.fire(getFacingDir());
    }

	/// <summary>
	/// Turn to the specified direction immediately.
	/// </summary>
	/// <param name="direction">Direction Vector(x, z).</param>
    public bool turn(Vector2 direction) {
        //Debug.Log(direction);
        float rot = - Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90f;
		transform.rotation = Quaternion.Euler(0f, rot, 0f);
		return true;
    }


	/// <summary>
	/// Start reload weapon.
	/// </summary>
	public bool reload() {
		return this.weapon.reload();
	}

	#endregion

	/// <summary>
	/// Move towards specified direction. Only used for Human Controllers
	/// </summary>
	/// <param name="direction">Direction.</param>
	public void move(Vector2 direction)
	{
		Vector2 dir = speed * direction.normalized;
		Vector3 disp = new Vector3(dir.x, 0f, dir.y);

		transform.GetComponent<Rigidbody>().velocity = disp;
	}

    public virtual Vector2 getMoveDir()
    {
        return Vector2.zero;
    }

    public virtual Vector2 getTurnDir()
    {
        return Vector2.zero;
    }

	/// <summary>
	/// Gets the current Facing direction vector(x, z).
	/// </summary>
	/// <returns>The facing dir.</returns>
    public Vector2 getFacingDir()
    {
		float rot = 90 - transform.rotation.eulerAngles.y;
        //Debug.Log(rot);
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * rot), Mathf.Sin(Mathf.Deg2Rad *  rot));
        return direction;
    }

    /// <summary>
    /// Gets a list of characters who are opponents of this character, i.e.
    /// not on the same team.
    /// </summary>
    /// <returns>A list of opponent characters</returns>
    public List<Character> GetOpponents()
    {
        var teams = GameManager.instance.teams;
        List<Character> opponents = new List<Character>();
        foreach (var team in teams) {
            if(team.name != this.team.name) {
                opponents.AddRange(team.members);
            }
        }
        return opponents;
    }


    /// <summary>
    /// Gets a list of characters who are on the same team as this character.
    /// </summary>
    /// <returns>A list of teammate characters</returns>
    public List<Character> GetTeam()
    {
        var teams = GameManager.instance.teams;
        List<Character> teammates = new List<Character>();
        foreach (var team in teams)
        {
            if (team.name == this.team.name)
            {
                teammates.AddRange(team.members);
            }
        }
        return teammates;
    }



    /// <summary>
    /// Gets a list of teammates of this character
    /// </summary>
    /// <returns>The list of characters on the team, "this" character excluded</returns>
    public List<Character> GetTeammates()
    {
        var teams = GameManager.instance.teams;
        List<Character> teammates = new List<Character>();
        foreach (var team in teams) {
            if(team.name == this.team.name) {
                teammates.AddRange(team.members);
            }
        }
        teammates.Remove(this);
        return teammates;
    }

    /// <summary>
    /// Finds the closest character to this one from a list.
    /// </summary>
    /// <param name="characters">The list of characters</param>
    /// <returns>The closest character in the list</returns>
    public Character GetClosestCharacter(List<Character> characters)
    {
        Character closest = null;
        float closestDistance = float.PositiveInfinity;
        foreach (var chara in characters)
        {
            var distance = Vector3.Distance(chara.transform.position, this.transform.position);
            if (distance < closestDistance)
            {
                distance = closestDistance;
                closest = chara;
            }
        }
        return closest;
    }

    /// <summary>
    /// Gets the closest character who is not on the same team.
    /// </summary>
    /// <returns>The closest character or null if there are no opponents</returns>
    public Character GetClosestOpponent()
    {
        return GetClosestCharacter(GetOpponents());
    }

    /// <summary>
    /// Gets the closest character who is on the same team.
    /// </summary>
    /// <returns>The closest character or null if there are no allys</returns>
    public Character GetClosestTeammate()
    {
        return GetClosestCharacter(GetTeam());
    }

    /// <summary>
    /// Returns true iff the character is not visible to any enemies
    /// </summary>
    /// <returns>Whether or not the character is in a safe location</returns>
    public bool InSafeSpot()
    {
        var opponents = GetOpponents();
        foreach (var enemy in opponents)
        {
            if (GameManager.instance.tileManager.PositionCanSeePosition(enemy.transform.position, transform.position)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns true iff the character can see an enemy
    /// </summary>
    /// <returns>Whether or not the character can see an enemy</returns>
    public bool CanSeeEnemy()
    {
        var opponents = GetOpponents();
        foreach (var enemy in opponents)
        {
            if (GameManager.instance.tileManager.PositionCanSeePosition(enemy.transform.position, transform.position, GetTeammates()))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets whether this character has line of sight to the given character.
    /// </summary>
    /// <param name="ch">The character to check line of sight on</param>
    /// <returns>True if this character has line of sight</returns>
    public bool HasLineOfSight(Character ch)
    {
        return GameManager.instance.tileManager.PositionCanSeePosition(transform.position, ch.transform.position, GetTeammates());
    }

    /// <summary>
    /// Gets the direction vector to move towards the next location
    /// in the NavMeshAgent's path
    /// </summary>
    /// <returns>The next move direction</returns>
    public Vector2 GetNavMeshNextMoveDir()
    {
        var nextLocation = navMeshAgent.nextPosition;
        var nextMoveDir = new Vector2(nextLocation.x, nextLocation.z) - new Vector2(transform.position.x, transform.position.z);
        return nextMoveDir;
    }

    /// <summary>
    /// Returns true iff this character has low health
    /// </summary>
    /// <returns>True if health is low, else false</returns>
    public bool HasLowHealth()
    {
        return getHealth() < maxHealth * 0.6f;                  //Changed this to initiate Retreat sooner
    }

    /// <summary>
    /// Returns true iff this character has high health
    /// </summary>
    /// <returns>True if health is high, else false</returns>
    public bool HasHighHealth()
    {
        return getHealth() > maxHealth * 0.8f;
    }


    /// <summary>
    /// Returns true iff this character has health greater than 50%
    /// </summary>
    /// <returns>True if health is high, else false</returns>
    public bool HasMediumHealth()
    {
        bool healthy = this.getHealth() > maxHealth * 0.5f;

        return healthy;                 //Added this for Strategic AI assesment
    }



    /// <summary>
    /// Returns true iff this character has ammo in their gun clip
    /// </summary>
    /// <returns>True if the clip has ammo, else false</returns>
    public bool HasAmmo()
    {
        return weapon.ammoClipLoad > 0;
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
		team.Remove(this);
        Destroy(gameObject);
    }

	public void Remove() {
		Destroy(gameObject);
	}

   
    public bool GetOrders()
    {
        return orders;
    }


    public Vector3 GetDestination()
    {
        return destination;
    }



    public void SetOrders(bool order)
    {
        orders = order;
        //Debug.Log("SetOrders Called by Start() Commander");
    }


    public void SetDestination(Vector3 dest)
    {
        destination = dest;
    }


    public int GetDestTimer()
    {
        return destTimer;
    }


    public void SetDestTimer(int time)
    {
        destTimer = time;
    }


    public void IncrementTimer()
    {
        destTimer += 1;
    }




}//END: Class
