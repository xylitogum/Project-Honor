using UnityEngine;
using System.Collections;

public class Weapon {

    

    public int id;
    public string type;
    // fire
    public float damageBase;
    public float damageFloat;
    public float shootRecoveryTime;
    public int bulletCount;
    public int bulletSpeed;
    public string firingType;

    // ammo
    public int clipLoadMax;
    public int carryMax;
    public float reloadClipTime;

    // aim
    public float initialOffset;
    public float accumuOffsetRecovery;
    public float accumuOffsetPerShot;
    public float accumuOffsetMax;
    public float speedOffsetCoefficient;
    public float speedOffsetMax;
    
	
    


    /*
    public Fire fire;
    
    public class Fire
    {
        
    }
    
    public Ammo ammo;

    public class Ammo
    {
        
    }

    public Aim aim;
    public class Aim
    {
        
    }
    */

   
}
