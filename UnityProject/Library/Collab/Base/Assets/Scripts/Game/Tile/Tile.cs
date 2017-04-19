using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// Tile.
/// </summary>
public class Tile {
	/// <summary>
	/// The x.
	/// </summary>
	public int x;
	/// <summary>
	/// The y.
	/// </summary>
	public int y;

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile(int x, int y) {
		this.x = x;
		this.y = y;

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Tile"/> class.
	/// </summary>
	/// <param name="position">From Position.</param>
	public Tile(Vector3 position) {
		this.x = Mathf.RoundToInt(position.x);
		this.y = Mathf.RoundToInt(position.z);
	}


	/// <summary>
	/// Determines whether the specified <see cref="Tile"/> is equal to the current <see cref="Tile"/>.
	/// </summary>
	/// <param name="other">The <see cref="Tile"/> to compare with the current <see cref="Tile"/>.</param>
	/// <returns><c>true</c> if the specified <see cref="Tile"/> is equal to the current <see cref="Tile"/>; otherwise, <c>false</c>.</returns>
	public bool Equals(Tile other) {
		return this.x == other.x && this.y == other.y;
	}

	/// <summary>
	/// Contains the specified position.
	/// </summary>
	/// <param name="position">Position.</param>
	public bool Contains(Vector3 position) {
		return (Mathf.Abs(position.x - this.x) <= 0.5f) && 
			(Mathf.Abs(position.z - this.y) <= 0.5f);
	}

	/// <summary>
	/// Convert to a vector3.
	/// </summary>
	/// <returns>The vector3.</returns>
	public Vector3 ToVector3() {
		return new Vector3((float)this.x, 0f, (float)this.y);
	}

	/// <summary>
	/// If the tile Blocks the sight from start to target.
	/// </summary>
	/// <returns><c>true</c>, if sight was blocked, <c>false</c> otherwise.</returns>
	/// <param name="start">Start.</param>
	/// <param name="target">Target.</param>
    public bool BlocksSight(Tile start, Vector3 target, float border = 0.6f) {
        // Check if the sight is blocked (Raycast in 2D)
        float x0 = (float)start.x; // start(3,3) target(3,4) this(3,5)
        float y0 = (float)start.y; // 
        float x1 = target.x - x0; // (0,1)
        float y1 = target.z - y0;
        float k; // lerp coefficient
        float t; // modified standard value
        float r; // temporary result
        
        // bottom border
        t = (float)this.y - border;
        k = (t - y0) / y1;
        r = x0 + k * x1;
        if (Mathf.Abs(r - this.x) <= border && k >= 0f && k <= 1f)
        {
            return true;
        }
        // top border
        t = (float)this.y + border;
        k = (t - y0) / y1;
        r = x0 + k * x1;
        if (Mathf.Abs(r - this.x) <= border && k >= 0f && k <= 1f)
        {
            return true;
        }
        // left border
        t = (float)this.x - border;
        k = (t - x0) / x1;
        r = y0 + k * x1;
        if (Mathf.Abs(r - this.y) <= border && k >= 0f && k <= 1f)
        {
            return true;
        }
        // right border
        t = (float)this.x + border; // t = 3 + 0.5 = 3.5
        k = (t - x0) / x1; // k = (3.5-3)/1 = 0.5
        r = y0 + k * x1; // 3 + 0.5 * 1
        if (Mathf.Abs(r - this.y) <= border && k >= 0f && k <= 1f)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets whether or not this tile blocks the sight from start to target using
    /// equations of lines and value bound checks.
    /// </summary>
    /// <param name="start">The start location</param>
    /// <param name="target">The target location</param>
    /// <param name="border">The half-width/height of the tile</param>
    /// <returns>True iff this tile intersects the line segment between start and
    /// target, else false</returns>
    public bool BlocksSight2(Vector3 start, Vector3 target, float border = 0.5f) //Set this to 0.3 from 0.5
    {
        // ax + by + c = 0
        var a = start.z - target.z;
        var b = target.x - start.x;
        var c = ((start.x - target.x) * start.z) + ((target.z - start.z) * start.x);

        var top = y + border;
        var bottom = y - border;
        var left = x - border;
        var right = x + border;
        
        // Handle horizontal and vertical lines
        if (start.x == target.x)
        {
            var segTop = Mathf.Max(start.z, target.z);
            var segBot = Mathf.Min(start.z, target.z);
            return start.x >= left && start.x <= right && 
                    ((top >= segBot && top <= segTop) || (bottom >= segBot && bottom <= segTop));
        }
        if (start.z == target.z)
        {
            var segLeft = Mathf.Min(start.x, target.x);
            var segRight = Mathf.Max(start.x, target.x);
            return start.z >= segLeft && start.z <= segRight &&
                   ((left >= segLeft && left <= segRight) || (right >= segLeft && right <= segRight));
        }

        float r;
        // Intersect with left
        r = SolveForY(a, b, c, left);
        if (r >= bottom && r <= top && left >= Mathf.Min(start.x, target.x) && left <= Mathf.Max(start.x, target.x))
            return true;

        // Intersect with right
        r = SolveForY(a, b, c, right);
        if (r >= bottom && r <= top && right >= Mathf.Min(start.x, target.x) && right <= Mathf.Max(start.x, target.x))
            return true;

        // Intersect with top
        r = SolveForX(a, b, c, top);
        if (r >= left && r <= right && top >= Mathf.Min(start.z, target.z) && top <= Mathf.Max(start.z, target.z))
            return true;

        // Intersect with bottom
        r = SolveForX(a, b, c, bottom);
        if (r >= left && r <= right && bottom >= Mathf.Min(start.z, target.z) && bottom <= Mathf.Max(start.z, target.z))
            return true;

        return false;
    }

    /// <summary>
    /// Solves a linear equation in general form for y when x is known.
    /// General form is Ax + By + C = 0
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="x"></param>
    /// <returns>The y value</returns>
    private static float SolveForY(float a, float b, float c, float x)
    {
        return -((a * x) + c) / b;
    }

    /// <summary>
    /// Solves a linear equation in general form for x when y is known.
    /// General form is Ax + By + C = 0
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="y"></param>
    /// <returns>The x value</returns>
    private static float SolveForX(float a, float b, float c, float y)
    {
        return -((b * y) + c) / a;
    }

    /// <summary>
    /// Gets the manhattan distance between two tiles
    /// </summary>
    /// <param name="t1">The first tile</param>
    /// <param name="t2">The second tile</param>
    /// <returns>The manhattan distance between the given tiles</returns>
    public static int ManhattanDistance(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y);
    }


}
