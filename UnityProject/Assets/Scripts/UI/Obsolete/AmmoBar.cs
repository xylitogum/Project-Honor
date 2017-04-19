using UnityEngine;
using UnityEngine.UI;
using System.Collections;



namespace Game.UI {

	[System.Obsolete("Not used anymore", false)]
	public class AmmoBar : MonoBehaviour
	{
		public RectMask2D fillerMask; 
		public Image filler;
		public Image background;
		public Text fillerText;
		// Use this for initialization
		void Start()
		{

		}


		// Update is called once per frame
		void Update()
		{
			float ammoRate = 0f;
			CharacterWeapon weapon = null;//GameManager.player.getWeapon();

			if (weapon.getClipLoadMax() >= 0)
			{
				ammoRate = (float)(weapon.getClipLoad()) / weapon.getClipLoadMax();
				ammoRate = Mathf.Clamp(ammoRate, 0f, 1f);
				if (weapon.getStatus() == CharacterWeapon.Status.Idle)
				{
					filler.color = new Color(1f, 1f, 0.3f);
					fillerText.text = weapon.getClipLoad().ToString() + "/" + weapon.getCarry().ToString();
				}
				if (weapon.getStatus() == CharacterWeapon.Status.Firing)
				{
					filler.color = new Color(1f, 0.5f, 0.3f);
					fillerText.text = weapon.getClipLoad().ToString() + "/" + weapon.getCarry().ToString();
				}
				if (weapon.getStatus() == CharacterWeapon.Status.Reloading)
				{
					filler.color = new Color(1f, 0.5f, 0.3f);
					fillerText.text = "Reloading";
				}
			}

			fillerMask.rectTransform.localScale = new Vector3(ammoRate, 1f, 1f);
			if (ammoRate > 0f)
			{
				filler.rectTransform.localScale = new Vector3(1f / ammoRate, 1f, 1f);

			}
			else
			{
				filler.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			}

		}
	}

}