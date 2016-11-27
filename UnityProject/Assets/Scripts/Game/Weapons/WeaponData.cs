using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponData
{
    public static Dictionary<string, Weapon> weapons;

    public static Weapon getWeaponData(string type)
    {
        if (weapons.ContainsKey(type))
        {
            return weapons[type];
        }
        else
        {
            Debug.LogError("Weapon Data Not Recognized:" + type);
            return null;
        }
    }
}
