using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;

public class ZombieBehavior : MonoBehaviour {
	public static readonly BodyPart HEAD = new BodyPart (6, 10);
	public static readonly BodyPart BODY = new BodyPart (2, 5);

	public int OriginalHealth;
	public int HitPointsRemaining = 1;
	public float Speed = 0f;
	private GameObject player;
	Animator animator;

	public void SetHealth(int health) {
		HitPointsRemaining = health;
		OriginalHealth = health;
	}
	public void SetSpeed(float speed) {
		var animator = this.GetComponent<Animator> ();
		animator.speed = speed;
		Speed = speed;
	}

	public bool IsMaxHealth() {
		return HitPointsRemaining == OriginalHealth;
	}

	public bool IsAlive() {
		return HitPointsRemaining > 0;
	}

	// returns points scored
	public int Shoot(BodyPart bodyPart) {
		double effectiveDamage = Math.Min (bodyPart.Damage, HitPointsRemaining);
		HitPointsRemaining -= bodyPart.Damage;
		if (HitPointsRemaining <= 0) {
			SelectDeathAnimation ();

			// stop moving
			var rb = this.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			animator.speed = 1.0f;

			// We need to disable
			var colliders = transform.GetComponentsInChildren<Collider> ().ToList ();
			colliders.ForEach (z => z.enabled = false);
			Destroy (this.gameObject, 15.0f); // set the sprite to disappear after 15 seconds
		}

		return (int)(effectiveDamage * bodyPart.PointsPerDamage);
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		animator = GetComponent<Animator>();
	}

	private const float MinDistance = 1.8f;
	// Update is called once per frame
	void Update () {
		if (IsAlive()) {
			var deltaV = player.transform.position - transform.position;
			if (deltaV.magnitude >= MinDistance) {
				var bestVector = GetBestVector ();
				transform.LookAt (transform.position + bestVector);
				var move = bestVector * Speed;
				var rb = this.GetComponent<Rigidbody> ();
				rb.velocity = move / 3;
			} else {
				this.transform.LookAt (player.transform);
				animator.speed = 1.0f;
				var rb = this.GetComponent<Rigidbody> ();
				rb.velocity = Vector3.zero;
				//rb.angularVelocity = Vector3.zero;
				animator.SetTrigger ("Attack");
			}
		}
	}

	Vector3 LastDirection;
	private const float ZOMBIE_SIGHT = 10f;
	private Vector3 GetBestVector() {
		RaycastHit hit;
		var originalDirection = (player.transform.position - transform.position).normalized;
		var currentDirection = originalDirection;
		int TERRAIN_LAYER = LayerMask.GetMask("Terrain");
		var delta = 0;

		Physics.Raycast (transform.position, currentDirection, out hit, ZOMBIE_SIGHT, TERRAIN_LAYER);
		if (hit.transform == null) {
			LastDirection = currentDirection;
			return currentDirection;
		}
		do {
			currentDirection = Quaternion.Euler (0, delta, 0) * (LastDirection == Vector3.zero ? originalDirection : LastDirection);
			Physics.Raycast (transform.position, currentDirection, out hit, ZOMBIE_SIGHT, TERRAIN_LAYER);
			if (hit.transform == null) {
				LastDirection = currentDirection;
				break;
			}

			currentDirection = Quaternion.Euler (0, -delta, 0) * (LastDirection == Vector3.zero ? originalDirection : LastDirection);
			Physics.Raycast (transform.position, currentDirection, out hit, ZOMBIE_SIGHT, TERRAIN_LAYER);

			delta += 10;
		} while(hit.transform != null);

		LastDirection = currentDirection;
		return currentDirection;
	}

	private void SelectDeathAnimation() {
		animator.speed = 1.0f;
		var whichDeath = UnityEngine.Random.Range (0, 3);
		if (whichDeath == 0)
			animator.SetTrigger ("DieLeft");
		else if (whichDeath == 1)
			animator.SetTrigger ("DieRight");
		else
			animator.SetTrigger ("DieBack");
	}
}

public struct BodyPart {
	public BodyPart(int dmg, int pts) {
		Damage = dmg;
		PointsPerDamage = pts;
	}
	public int Damage;
	public int PointsPerDamage;
}
