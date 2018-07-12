using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerAnimator;

	private Animator PlayerAnimator
	{
		get
		{
			if (playerAnimator == null)
				playerAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerMesh).GetComponent<Animator>();
			return playerAnimator;
		}
	}

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool(AnimationCodes.BasicAttacking, false);
		SetAnimationBool(AnimationCodes.Grappling, false);
	}

	internal void ExecuteGrapple()
	{
		SetAnimationBool(AnimationCodes.Grappling, true);
	}

	private void SetAnimationBool(string name, bool value)
	{
		PlayerAnimator?.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		PlayerAnimator?.SetInteger(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		PlayerAnimator?.SetTrigger(name);
	}

	private static class AnimationCodes
	{
		public const string Sliding = "Sliding";
		public const string Jumping = "Jumping";
		public const string Hanging = "Hanging";

		public const string Grappling = "Grappling";
		public const string BasicAttacking = "BasicAttacking";
	}
}
