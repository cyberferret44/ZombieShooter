using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorBehavior : MonoBehaviour {
	Image img;
	Slider HealthBar;
	private bool IsDead = false;

	public bool IsPlayerDead() {
		return IsDead;
	}

	// Use this for initialization
	void Start () {
		img = this.GetComponent<Image> ();
		HealthBar = GameObject.FindGameObjectWithTag ("HealthBar").GetComponent<Slider> ();
	}
	
	// Update is called once per frame
	void Update () {
		ModifyDamageIndicator (-.002f);
	}

	public void ModifyDamageIndicator(float alphaAdjustment) {
		float alpha = img.color.a + alphaAdjustment;
		alpha = Mathf.Min (alpha, 1.0f);
		alpha = Mathf.Max (alpha, 0);
		img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

		if(!IsDead)
			HealthBar.value = 1.0f - alpha;

		IsDead |= alpha >= 1.0f;
	}

	// returns true if
	public void ApplyDamage(float alphaAdjustment) {
		if(!IsDead)
			ModifyDamageIndicator(alphaAdjustment);
	}

	public bool IsMaxDamage() {
		return img.color.a >= 1.0f;
	}
}
