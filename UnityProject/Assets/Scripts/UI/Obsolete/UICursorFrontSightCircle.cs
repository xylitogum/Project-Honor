﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Game.UI {


	[System.Obsolete("Not used anymore", false)]
	public class UICursorFrontSightCircle : MonoBehaviour {
		private Image cursor;
		public float cursorRadius = 128f;
		public float minSize = 1f;
		public float maxSize = 1000f;
		//private float orthoSize;
		// Use this for initialization
		void Start () {
			//cursor = GetComponent<Image>();

		}

		// Update is called once per frame
		void Update () {
			/*
		Character p = GameManager.player;
		float offset = p.getWeapon().getDirOffset();

		float distance = p.getTurnDir().magnitude;

		float radius = distance * Mathf.Tan(offset * Mathf.Deg2Rad);

		float radiusOnScreen = Vector2.Distance(
			Camera.main.WorldToScreenPoint(new Vector3(0f, 0f, -Camera.main.transform.position.z)),
			Camera.main.WorldToScreenPoint(new Vector3(radius, 0f, -Camera.main.transform.position.z))
		);

		radiusOnScreen = Mathf.Clamp(radiusOnScreen, minSize, maxSize);
		//Debug.Log(radiusOnScreen);

		float scaleSize = Mathf.Max(0.01f, Camera.main.GetComponent<CameraLock>().getScaleSize());

		transform.localScale = Vector2.one *  (radiusOnScreen / cursorRadius) * scaleSize;

        */

		}
	}


}