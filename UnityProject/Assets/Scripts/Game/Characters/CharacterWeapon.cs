using UnityEngine;
using System.Collections;

public class CharacterWeapon : MonoBehaviour {

    [SerializeField]
    public GameObject bullet;
    [SerializeField]
    public GameObject firepoint;
    
    public string gunType;
    public Weapon gunData { get; set; }
    public float shootRecoveryTimeRemaining { get; set; }
    public int ammoClipLoad;
    public int ammoCarry;
    public float reloadClipTimeRemaining { get; set; }
    public float accumuOffset { get; set; }
    Status status = Status.Idle;
    public enum Status
    {
        Idle, // nothing to do here
        Firing, // has just fired a bullet and is loading for the next one
        Reloading, // is reloading for a new clip
        Disabled // not able to use
    }

    Character owner;


    public bool isClipLoaded()
    {
         return ammoClipLoad > 0 || ammoClipLoad == -1;
    }

    public bool canReload()
    {
       return (ammoCarry > 0 || ammoCarry == -1) && ammoClipLoad < gunData.clipLoadMax;
    }

    /** 
     * Basic
     */

    // Use this for initialization
    void Start()
    {

        Init();
    }

    // Use this for initialization
    void Awake()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        // accumulative Offset recover
        accumuOffset = Mathf.Clamp(accumuOffset - gunData.accumuOffsetRecovery * Time.deltaTime, 0f, gunData.accumuOffsetMax);

        // clip reloading
        if (status == Status.Reloading)
        {
            Update_Reloading();
        }

        // shoot recover
        if (status == Status.Firing)
        {
            Update_Firing();
        }

        if (status == Status.Idle)
        {
            Update_Idle();
        }
        //Debug.Log(accumuOffset);
    }

    void Init()
    {
        
        gunData = WeaponData.getWeaponData(gunType);
        //Debug.Log("Gun loaded!");
        ammoClipLoad = Mathf.Clamp(ammoClipLoad, -1, gunData.clipLoadMax);
        ammoCarry = Mathf.Clamp(ammoCarry, -1, gunData.carryMax);
        accumuOffset = 0f;
        shootRecoveryTimeRemaining = 0f;
        status = Status.Idle;
        //WeaponAmmo.clipLoad = WeaponAmmo.clipLoadMax;
        //WeaponAmmo.carry = WeaponAmmo.carryMax;
    }







    /**
     * Update Function for each Status
     */

    void Update_Reloading()
    {
        reloadClipTimeRemaining -= Time.deltaTime;
        // reloading over
        if (reloadClipTimeRemaining <= 0f)
        {
            status = Status.Idle;
            reloadClipTimeRemaining = 0f;
            if (ammoClipLoad == -1) // infinite ammoClip
            {
                // do nothing here;
            }
            else if (ammoCarry == -1) // infinite ammoCarry
            {
                int numberLoaded = gunData.clipLoadMax - ammoClipLoad;
                ammoClipLoad += numberLoaded;
            }
            else
            { // regular ammo
                int numberLoaded = Mathf.Min(gunData.clipLoadMax - ammoClipLoad, ammoCarry);
                ammoClipLoad += numberLoaded;
                ammoCarry -= numberLoaded;
            }
            
        }
    }
    void Update_Firing()
    {
        shootRecoveryTimeRemaining -= Time.deltaTime;
        // recover over
        if (shootRecoveryTimeRemaining <= 0f)
        {
            status = Status.Idle;
            shootRecoveryTimeRemaining = 0f;
        }
    }
    void Update_Idle()
    {
        // check if reloading is necessary
        if (!isClipLoaded())
        {
            reload();
        }
    }


    /** 
     * Game Logics
     */
    public bool fire(Vector2 direction)
    {
        if (status == Status.Idle && isClipLoaded())
        {
            Vector2 dir = new Vector2(direction.x, direction.y).normalized;
            float dirOffset = getDirOffset();
            dir = dir.Rotate(Random.Range(-dirOffset, dirOffset));

            // instantiate bullet
            Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
            Bullet blt = ((GameObject)Instantiate(bullet, firepoint.transform.position, getOwner().transform.rotation)).GetComponent<Bullet>();
            blt.setOwner(getOwner());
            blt.setDamage(gunData.damageBase);
            blt.setDamageFloat((gunData.damageFloat));
            blt.GetComponent<Rigidbody2D>().velocity = dir * gunData.bulletSpeed + rb.velocity;

            // create fire particle effects
            ParticleSystem ps = firepoint.GetComponent<ParticleSystem>();
            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;
            //vel.x = rb.velocity.x;
            //vel.y = rb.velocity.y;
            //vel.z = 0;
            ps.Emit((int)Random.Range(20, 30));

            // shoot
            setClipLoad(getClipLoad() - 1);
            accumuOffset = Mathf.Min(
                accumuOffset + gunData.accumuOffsetPerShot,
                gunData.accumuOffsetMax);
            status = Status.Firing;
            shootRecoveryTimeRemaining = gunData.shootRecoveryTime;

            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void fillUpAmmo()
    {
        ammoClipLoad = gunData.clipLoadMax;
        ammoCarry = gunData.carryMax;
    }

    public bool reload()
    {
        if (status != Status.Reloading && canReload())
        {
            status = Status.Reloading;
            reloadClipTimeRemaining = gunData.reloadClipTime;
            return true;
        }
        return false;
        
    }
    

    bool isShootRecovered()
    {
        return shootRecoveryTimeRemaining <= 0f;
    }

    
    
    float getDirOffset()
    {
        float offset = gunData.initialOffset;
        Rigidbody2D rb = owner.GetComponent<Rigidbody2D>();
        offset = offset + accumuOffset + Mathf.Min(
            gunData.speedOffsetMax,
            gunData.speedOffsetCoefficient * rb.velocity.magnitude);
        return offset;
    }



    /**
     * Public Methods for field access
     */

    public int getClipLoad()
    {
        return ammoClipLoad;
    }

    public void setClipLoad(int newClipLoad)
    {
        ammoClipLoad = Mathf.Clamp(newClipLoad, 0, gunData.clipLoadMax);
    }

    public int getClipLoadMax()
    {
        return gunData.clipLoadMax;
    }

    public void setClipLoadMax(int newClipLoadMax)
    {
        ammoClipLoad = newClipLoadMax;
    }

    public int getCarry()
    {
        return ammoCarry;
    }
    public Status getStatus()
    {
        return status;
    }
    public Character getOwner()
    {
        return this.owner;
    }
    public void setOwner(Character owner)
    {
        this.owner = owner;
    }

}
