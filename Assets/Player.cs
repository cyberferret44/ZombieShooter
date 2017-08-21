using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public int PlayerScore = 0;
	public int Hits = 0;
	public int Misses = 0;

	private Text scoreText;
	private Image DamageIndicator;
	private DamageIndicatorBehavior damageIndicator;
	private bool StillAlive = true;
	private const float ZOMBIE_DAMAGE_DEALT = .15f; // represented as a percentage... i.e. 20%
	AudioSource Gunshot;

	void Start() {
		Gunshot = GameObject.FindGameObjectWithTag ("Gunshot").GetComponent<AudioSource> ();
		scoreText = (GameObject.FindGameObjectWithTag ("ScoreText")).GetComponent<Text>();
		damageIndicator = GameObject.FindGameObjectWithTag ("DamageIndicator").GetComponent<DamageIndicatorBehavior> ();
	}

	public bool IsAlive() {
		return StillAlive;
	}

	public void AddScore(int numToAdd) {
		PlayerScore += numToAdd;
		scoreText.text = PlayerScore.ToString();
	}

	public void AttackPlayer() {
		damageIndicator.ApplyDamage (ZOMBIE_DAMAGE_DEALT);
		StillAlive = !damageIndicator.IsPlayerDead();
	}

	void Update () {
		// Don't receive input if player is dead
		if (IsAlive ()) {
			if (Input.GetMouseButtonDown (0)) {
				ShootGun ();
			}
			// This code is to rotate the camera with the arrow keys
			/*if (Input.GetKey (KeyCode.RightArrow)) {
				Camera.main.transform.Rotate (new Vector3 (0.0f, 2.5f, 0.0f));
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				Camera.main.transform.Rotate (new Vector3 (0.0f, -2.5f, 0.0f));
			}*/
		}
	}

	public void ShootGun() {		
		Gunshot.Play ();

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 50.0f)) {					
			ZombieBehavior zombie;
			BodyPart hitPart;
			if (hit.collider.gameObject.name.ToLower ().Contains ("head")) {
				zombie = hit.transform.GetComponent<ZombieBehavior> ();
				hitPart = ZombieBehavior.HEAD;
			} else { 
				zombie = hit.transform.GetComponent<ZombieBehavior> ();
				hitPart = ZombieBehavior.BODY;
			}
			if (zombie != null && zombie.IsAlive ()) {
				Hits++;
				AddScore(zombie.Shoot (hitPart));
			} else {
				Misses++;
			}
		} else {
			Misses++;
		}
	}
}
