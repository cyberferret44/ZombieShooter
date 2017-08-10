using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : StateMachineBehaviour {
	private static Player player;
 	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		player.AttackPlayer ();
	}
}
