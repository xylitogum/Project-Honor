using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataReader : MonoBehaviour {
    public const string DATA_PATH = "Data";
    public const string WEAPONDATA_PATH = "WeaponData";

    

    //public static List<TextAsset> listOfFiles;
    


	// Use this for initialization
	void Awake () {
        loadFiles();
        //Debug.Log(desertEagleFile.text);
        
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void loadFiles()
    {

        /**
         * Load WeaponData files
         */
        WeaponData.weapons = new Dictionary<string, Weapon>();
        string WeaponNames = (Resources.Load(DATA_PATH + "/" + WEAPONDATA_PATH) as TextAsset).text;
        WeaponNames = WeaponNames.Replace("\r", "");
        string[] listOfWeaponNames = WeaponNames.Split(new Char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log(listOfWeaponNames[0]);
        for (int i = 0; i < listOfWeaponNames.Length; i++)
        {

            
            string path = DATA_PATH + "/" + WEAPONDATA_PATH + "/" + listOfWeaponNames[i];
            TextAsset file = Resources.Load(path) as TextAsset; 
            
            Weapon weaponData = JsonUtility.FromJson<Weapon>(file.text);
            WeaponData.weapons.Add(weaponData.type, weaponData);
        }
        

    }
    
}
