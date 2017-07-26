using UnityEngine;
using System.Collections;

public class CharacterAnimationController {

	private Animator animator;

	public CharacterAnimationController (Animator an){
		animator = an;
	}

	public void WalkInDir (Direction dir) {
		animator.SetInteger ("walkingState", (int) dir);
	}

	public void StopWalking () {
		animator.SetInteger ("walkingState", 0);
	}

	public void TurnToDir (Direction dir) {
		WalkInDir (dir);
		animator.Update (0f);
		StopWalking ();
	}
}
