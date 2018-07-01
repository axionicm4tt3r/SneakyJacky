using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
	#region AttackProperties
	public static float BasicAttackDamage = 5f;
	public static float BasicAttackStaggerTime = 0.2f;

	public static float ChargeAttackDamage = 5f;
	public static float ChargeAttackKnockbackVelocity = 12f;
	public static float ChargeAttackKnockbackTime = 0.2f;

	public static float JumpKickDamage = 18f;
	public static float JumpKickKnockbackVelocity = 25f;
	public static float JumpKickKnockbackTime = 0.2f;

	public static float SlideKickDamage = 12f;
	public static float SlideKickKnockbackVelocity = 18f;
	public static float SlideKickKnockbackTime = 0.2f;
	#endregion

	PlayerAttackStateMachine playerAttackStateMachine;
	PlayerSoundManager playerSoundManager;

	BoxCollider playerHitbox;

	List<IAttackable> enemiesHit = new List<IAttackable>();

	void Awake()
	{
		playerAttackStateMachine = GetComponentInParent<PlayerAttackStateMachine>();
		playerSoundManager = GetComponentInParent<PlayerSoundManager>();
		playerHitbox = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerHitbox).GetComponent<BoxCollider>();
	}

	public void BasicAttack()
	{
		bool hitEnemy = false;
		List<IAttackable> results = CheckInstantFrameHitboxForEnemies(out hitEnemy);

		foreach (IAttackable attackableComponent in results)
		{
			var damage = BasicAttackDamage + BasicAttackDamage * playerAttackStateMachine.DamageMultiplier;
			attackableComponent.ReceiveStaggerAttack(damage, transform.forward, BasicAttackStaggerTime);
		}

		if (hitEnemy)
			playerSoundManager.PlayBasicAttackHitSound();
		else
			playerSoundManager.PlayBasicAttackMissSound();
	}

	public void ChargeAttack(IAttackable attackableComponent)
	{
		var damage = ChargeAttackDamage + ChargeAttackDamage * playerAttackStateMachine.DamageMultiplier;

		if (!enemiesHit.Contains(attackableComponent))
		{
			enemiesHit.Add(attackableComponent);
			attackableComponent.ReceiveKnockbackAttack(damage, transform.forward, ChargeAttackKnockbackVelocity, ChargeAttackKnockbackTime);
			playerSoundManager.PlayChargeAttackHitSound();
		}
		else
			attackableComponent.ReceiveKnockbackAttack(0f, transform.forward, ChargeAttackKnockbackVelocity, ChargeAttackKnockbackTime);
	}

	public void JumpKick(IAttackable attackableComponent)
	{
		if (!enemiesHit.Contains(attackableComponent))
		{
			enemiesHit.Add(attackableComponent);
			attackableComponent.ReceiveKnockbackAttack(JumpKickDamage, transform.forward, JumpKickKnockbackVelocity, JumpKickKnockbackTime);
			playerSoundManager.PlayJumpAttackHitSound();
		}
		else
			attackableComponent.ReceiveKnockbackAttack(0f, transform.forward, JumpKickKnockbackVelocity, JumpKickKnockbackTime);
	}

	public void SlideKick(IAttackable attackableComponent)
	{
		if (!enemiesHit.Contains(attackableComponent))
		{
			enemiesHit.Add(attackableComponent);
			attackableComponent.ReceiveKnockbackAttack(SlideKickDamage, transform.forward, SlideKickKnockbackVelocity, SlideKickKnockbackTime);
			playerSoundManager.PlayJumpAttackHitSound();
		}
		else
			attackableComponent.ReceiveKnockbackAttack(0f, transform.forward, SlideKickKnockbackVelocity, SlideKickKnockbackTime);
	}

	public List<IAttackable> CheckInstantFrameHitboxForEnemies(out bool hasEnemy)
	{
		Vector3 size = playerHitbox.size / 2;
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		ExtDebug.DrawBox(playerHitbox.transform.position + playerHitbox.transform.forward * 0.5f, size, playerHitbox.transform.rotation, Color.blue);
		int layerMask = LayerMask.GetMask(Helpers.Layers.Enemy, Helpers.Layers.Interactable);
		Collider[] colliders = Physics.OverlapBox(playerHitbox.transform.position + playerHitbox.transform.forward * 0.5f, size, playerHitbox.transform.rotation, layerMask);
		var results = new List<IAttackable>();

		foreach (Collider collider in colliders)
		{
			if (collider.tag != Helpers.Tags.Player)
			{
				var attackableComponent = collider.gameObject.GetAttackableComponent();
				if (attackableComponent != null)
					results.Add(attackableComponent);
			}
		}

		hasEnemy = results.Count > 0;
		return results;
	}

	public void ClearEnemiesHit()
	{
		enemiesHit.Clear();
	}
}

public static class PlayerAttackManagerExtensions
{
	public static IAttackable GetAttackableComponent(this GameObject gameObject)
	{
		return gameObject.GetComponentInChildren<IAttackable>();
	}
}
