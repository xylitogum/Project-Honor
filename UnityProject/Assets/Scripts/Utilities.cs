using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

 
 public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float LineClosestDistance(this Vector2 start, Vector2 direction, Vector2 point)
    {
        // closest distance = Sin(theta) * distance
        float theta = Vector2.Angle(direction, point - start);
        float distance = Vector2.Distance(start, point);
        float closest = Mathf.Abs(Mathf.Sin(theta) * distance);
        return closest;
    }

    
}