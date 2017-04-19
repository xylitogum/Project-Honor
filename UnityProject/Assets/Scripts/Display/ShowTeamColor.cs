using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Display {

	[RequireComponent (typeof (MeshRenderer))]
	public class ShowTeamColor : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		public bool ChangeTeamColor(Color color) {
			MeshRenderer rd = gameObject.GetComponent<MeshRenderer>();
			if (rd) {
				rd.material.color = color;
				return true;
			}
			return false;
		}
	}

}
