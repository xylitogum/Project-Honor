using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


namespace Game.UI {
	public class ButtonRestart : MonoBehaviour {
		public Button restartButton;
		// Use this for initialization
		void Start () {
			restartButton.gameObject.SetActive(false);

		}

		// Update is called once per frame
		void Update () {
			/*
	    if (GameManager.fail())
        {
            //Debug.Log("fail");
            restartButton.gameObject.SetActive(true);
        }
        */
		}

		public void ButtonRestartClick()
		{
			SceneManager.LoadScene(0);
		}


	}


}
