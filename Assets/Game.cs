using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	public static GameObject GameOverCanvas;
	public Player player;
	void Start () {
		GameOverCanvas = GameObject.FindGameObjectWithTag ("GameOverCanvas");
		GameOverCanvas.SetActive (false);
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void Update() {
		if (!player.IsAlive ()) {
			// Rotate death camera
			var camera = Camera.main.transform;
			if (Camera.main.transform.rotation.eulerAngles.x < 65f || Camera.main.transform.rotation.eulerAngles.x > 295f) {
				Camera.main.transform.Rotate (-3.0f, 0.0f, 0.0f);
			}
			if (Camera.main.transform.position.y > .5f) {
				var t = Camera.main.transform.position;
				Camera.main.transform.position = new Vector3 (t.x, t.y - 0.1f, t.z);
			}
			if (!GameOverCanvas.activeSelf) {
				float bonus = 1.0f + ((float)player.Hits / Mathf.Max (1.0f, ((float)player.Hits + (float)player.Misses)));
				GameOverCanvas.transform.Find ("Hits").GetComponent<Text> ().text += player.Hits.ToString ();
				GameOverCanvas.transform.Find ("Misses").GetComponent<Text> ().text += player.Misses.ToString ();
				GameOverCanvas.transform.Find ("Bonus").GetComponent<Text> ().text += bonus.ToString ("0.00");
				var temp = GameOverCanvas.transform.Find ("FinalScore").GetComponent<Text> ();
				temp.text += (int)((float)player.PlayerScore * bonus);
				GameOverCanvas.SetActive (true);
			}
		} else {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				GameOverCanvas.SetActive (!GameOverCanvas.activeSelf);
			}
		}
	}

	public void ExitGame() {
		Application.Quit();
	}
}
