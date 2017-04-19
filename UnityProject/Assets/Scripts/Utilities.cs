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

public static class Vector3Extension
{
	/// <summary>
	/// Convert Vector3 to Vector2 by removing y axis.
	/// </summary>
	/// <returns>The vec2.</returns>
	/// <param name="v">V.</param>
	public static Vector2 ToVec2(this Vector3 v) {
		return new Vector2(v.x, v.z);
	}
}

 public static class Vector2Extension
{

	/// <summary>
	/// Rotate the specified v and degrees.
	/// </summary>
	/// <param name="v">V.</param>
	/// <param name="degrees">Degrees.</param>
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

	/// <summary>
	/// Lines the closest distance.
	/// </summary>
	/// <returns>The closest distance.</returns>
	/// <param name="start">Start.</param>
	/// <param name="direction">Direction.</param>
	/// <param name="point">Point.</param>
    public static float LineClosestDistance(this Vector2 start, Vector2 direction, Vector2 point)
    {
        // closest distance = Sin(theta) * distance
        float theta = Vector2.Angle(direction, point - start);
        float distance = Vector2.Distance(start, point);
        float closest = Mathf.Abs(Mathf.Sin(theta) * distance);
        return closest;
    }

    
}