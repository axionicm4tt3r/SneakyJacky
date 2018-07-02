using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	private Animator playerUIAnimator;
	private Animator playerMotionAnimator;

	private Animator PlayerUIAnimator
	{
		get
		{
			if (playerUIAnimator == null)
				playerUIAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHUD)?.GetComponentInChildren<Animator>();
			return playerUIAnimator;
		}
	}

	private Animator PlayerMotionAnimator
	{
		get
		{
			if (playerMotionAnimator == null)
				playerMotionAnimator = GameObject.FindGameObjectWithTag(Helpers.Tags.Player)?.GetComponent<Animator>();
			return playerMotionAnimator;
		}
	}

	internal void ResetAnimatorParameters()
	{
		SetAnimationBool(AnimationCodes.BasicAttacking, false);
		SetAnimationBool(AnimationCodes.Grappling, false);
	}

	internal void ExecuteBasicAttack()
	{
		SetAnimationBool(AnimationCodes.BasicAttacking, true);
		SetAnimationInteger(AnimationCodes.BasicAttackIndex, UnityEngine.Random.Range(0, 2));
	}

	internal void ExecuteGrapple()
	{
		SetAnimationBool(AnimationCodes.Grappling, true);
	}

	private void SetAnimationBool(string name, bool value)
	{
		PlayerUIAnimator?.SetBool(name, value);
		//PlayerMotionAnimator?.SetBool(name, value);
	}

	private void SetAnimationInteger(string name, int value)
	{
		PlayerUIAnimator?.SetInteger(name, value);
		//PlayerMotionAnimator?.SetInteger(name, value);
	}

	private void SetAnimationTrigger(string name)
	{
		PlayerUIAnimator?.SetTrigger(name);
		//PlayerMotionAnimator?.SetTrigger(name);
	}

	private static class AnimationCodes
	{
		public const string Grappling = "Grappling";
		public const string BasicAttacking = "BasicAttacking";
		public const string BasicAttackIndex = "BasicAttackIndex";
	}
}
