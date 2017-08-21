using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawningController : MonoBehaviour {
	private float DifficultyMultiplier() { return 1f + (timer.ElapsedMilliseconds / 1000f) / 60f; }
	private const int BASE_HEALTH = 3;
	private const int BASE_WAVE_TIME = 30000; // miliseconds
	private const float BASE_ZOMBIE_SPEED = .6f;
	private const int BASE_ZOMBIES_TO_SPAWN = 6;
	private AudioSource JumpScare;

	private GameObject originalZombie;
	private int nextSpawnTime;
	System.Diagnostics.Stopwatch timer;

	bool hasSpawnedSurpriseZombie = false;

	// Use this for initialization
	void Start () {
		timer = new System.Diagnostics.Stopwatch ();
		timer.Start ();
		nextSpawnTime = 500;
		originalZombie = GameObject.FindGameObjectWithTag ("OriginalZombie");
		JumpScare = GameObject.FindGameObjectWithTag ("JumpScare").GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (nextSpawnTime < timer.ElapsedMilliseconds) {
			SpawnZombies ();
			nextSpawnTime += BASE_WAVE_TIME;
		}
		if (timer.ElapsedMilliseconds > 82000 && !hasSpawnedSurpriseZombie) {
			SpawnSurpriseZombie ();
			hasSpawnedSurpriseZombie = true;
		}
	}

	private void SpawnZombies() {
		for (int i = 0; i < (int)(BASE_ZOMBIES_TO_SPAWN * DifficultyMultiplier() * 2); i++) {
			SpawnZombie ();
		}
	}
	private void SpawnZombie() {
		Vector3 v3 = GetStartPosition ();
		var newZombie = Instantiate (originalZombie, v3, new Quaternion (0f, 0f, 0f, 0f));
		var behavior = newZombie.GetComponent<ZombieBehavior> ();
		GenerateZombieStats (behavior); 
	}

	private void SpawnSurpriseZombie() {
		Vector3 v3 = new Vector3 (0f, .01f, -1.5f);
		var newZombie = Instantiate (originalZombie, v3, new Quaternion (0f, 0f, 0f, 0f));
		var behavior = newZombie.GetComponent<ZombieBehavior> ();
		GenerateZombieStats (behavior);
		behavior.SetHealth (20);
		newZombie.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
		newZombie.transform.Rotate (new Vector3 (270f, 0f, 0f));

		JumpScare.PlayDelayed(.1f);
	}

	private Vector3 GetStartPosition() {
		var x = UnityEngine.Random.Range (-1f, 1f); // generates witin view of the camera
		var z = -1f; // generates initial distance from player
		var v3 = new Vector3 (x, .01f, z);
		v3.Normalize (); // normalize the vector to a magnitude of 1
		v3 *= 20; // then multiply it by this to determine the actual starting distance from the player
		return v3;
	}

	/// <summary>
	/// Generates the zombie stats using a Gaussian to mix up the speed
	/// </summary>
	private void GenerateZombieStats(ZombieBehavior z) {
		z.SetHealth ((int)((float)BASE_HEALTH * DifficultyMultiplier() * UnityEngine.Random.Range (.8f, 1.2f)));
		z.SetSpeed (Mathf.Pow(BASE_ZOMBIE_SPEED * DifficultyMultiplier() * UnityEngine.Random.Range (.8f, 1.2f), .8f));
	}
}
