using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
	#region AttackProperties
	public static float BasicAttackDamage = 5f;
	public static float BasicAttackStaggerTime = 0.2f;
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
			var damage = BasicAttackDamage + BasicAttackDamage;
			attackableComponent.ReceiveStaggerAttack(damage, transform.forward, BasicAttackStaggerTime);
		}

		if (hitEnemy)
			playerSoundManager.PlayBasicAttackHitSound();
		else
			playerSoundManager.PlayBasicAttackMissSound();
	}

	public void Grapple()
	{
		bool hitEnemy = false;
		List<IAttackable> results = CheckInstantFrameHitboxForEnemies(out hitEnemy);

		var damage = BasicAttackDamage + BasicAttackDamage;
		results.FirstOrDefault()?.ReceiveStaggerAttack(damage, transform.forward, BasicAttackStaggerTime);
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
