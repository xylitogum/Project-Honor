using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A component which provides GUI for a floating health
/// bar above a character.
/// </summary>
public class AgentHealth : MonoBehaviour {

    // The max width of the HP bar
    private int maxHpWidth;
    public int yOffset;
    public int barHeight;

    // Use this for initialization
    void Start () {
        this.maxHpWidth = 30;
	}
	
	void OnGUI () {
        float hprate = 0f;
        Character ch = GetComponent<Character>();
        if (ch != null && ch.getMaxHealth() >= 0f)
        {
            hprate = ch.getHealth() / ch.getMaxHealth();
            hprate = Mathf.Clamp(hprate, 0f, 1f);
        }
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var width = (int)(maxHpWidth * hprate);
        var height = barHeight;
        if(width != 0) {
            GUI.Box(new Rect(pos.x - (int)(0.5 * maxHpWidth), Screen.height - pos.y - yOffset, width, height), "",
               CreateStyle(width, height, new Color(1 - hprate, hprate, 0f, 1f)));
        }
    }

    /// <summary>
    /// Creates a GUIStyle for a rectangle with the given width, height,
    /// and color.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private GUIStyle CreateStyle(int width, int height, Color color)
    {
        var style = new GUIStyle(GUI.skin.box);
        style.normal.background = MakeTexture(width, height, color);
        return style;
    }

    /// <summary>
    /// Creates a texture for a rectangle with the given dimensions
    /// and color
    /// </summary>
    /// <param name="width">Width of the texture</param>
    /// <param name="height">Height of the texture</param>
    /// <param name="color">Color of the texture</param>
    /// <returns>The texture</returns>
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for(int i = 0; i< pixels.Length; i++)
        {
            pixels[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pixels);
        result.Apply();
        return result;
    }
}
