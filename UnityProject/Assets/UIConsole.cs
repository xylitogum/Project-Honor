using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour {
	
	private Text txt;
	private static string allTextDisplayed;
	// Use this for initialization
	void Start () {
		clearDisplay();
		txt = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
		txt.text = allTextDisplayed;
		
	}

	public static string getAll() {
		return allTextDisplayed;
	}

	public static void clearDisplay() {
		allTextDisplayed = "";
	}

	public static void setDisplay(string allTexts) {
		allTextDisplayed = allTexts;
	}

	public static void changeLine() {
		allTextDisplayed += "\n";
	}

	public static void addLine(string line) {
		allTextDisplayed += line;
		changeLine();
	}

	public static void removeLastLine() {
		
	}

}
