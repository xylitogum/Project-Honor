using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {
	[System.Obsolete("Not used anymore", false)]
	public class UICursorFrontSight : MonoBehaviour {
		//private Image cursor;

		// Use this for initialization
		void Start () {
			//cursor = GetComponent<Image>();;


		}

		// Update is called once per frame
		void Update () {
			transform.position = Input.mousePosition;

		}
	}
}


