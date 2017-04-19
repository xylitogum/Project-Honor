using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Game.UI {

	[System.Obsolete("Not used anymore", false)]
	public class HPbar : MonoBehaviour {
		public GameObject filler;
		public GameObject background;
		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			//float hprate = 0f;
			/*
		if (GameManager.player != null && GameManager.player.getMaxHealth() >= 0f) {
            hprate = GameManager.player.getHealth() / GameManager.player.getMaxHealth();
            //hprate = Mathf.Max(0f, Mathf.Min(1f, hprate));
            hprate = Mathf.Clamp(hprate, 0f, 1f);
        }
        Image img = filler.GetComponent<Image>();
        img.rectTransform.localScale = new Vector3(hprate, 1f, 1f);
        img.color = new Color(1-hprate, hprate, 0f ,1f);
        */
		}
	}



}
