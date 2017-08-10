using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawningController : MonoBehaviour {
	private float DifficultyMultiplier() { return 1f + (float)(timer.Elapsed.Seconds) / 50f; }
	private const int BASE_HEALTH = 4;
	private const int BASE_WAVE_TIME = 30000; // miliseconds
	private const float BASE_ZOMBIE_SPEED = .8f;
	private const int BASE_ZOMBIES_TO_SPAWN = 8;
	private int Health() { return (int)(BASE_HEALTH * DifficultyMultiplier()); }
	private float Speed() { return BASE_ZOMBIE_SPEED * Mathf.Pow(DifficultyMultiplier(), .8f); }

	private GameObject originalZombie;
	private int nextSpawnTime;
	System.Diagnostics.Stopwatch timer;


	// Use this for initialization
	void Start () {
		timer = new System.Diagnostics.Stopwatch ();
		timer.Start ();
		nextSpawnTime = 500;
		originalZombie = GameObject.FindGameObjectWithTag ("OriginalZombie");
	}
	
	// Update is called once per frame
	void Update () {
		if (nextSpawnTime < timer.ElapsedMilliseconds) {
			SpawnZombies ();
			nextSpawnTime += BASE_WAVE_TIME;
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
		z.SetHealth ((int)((float)Health () * UnityEngine.Random.Range (.8f, 1.2f)));
		z.SetSpeed (Speed () * UnityEngine.Random.Range (.8f, 1.2f));
	}
}
