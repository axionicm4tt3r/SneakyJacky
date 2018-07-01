using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAI : SuperStateMachine, IAttackable
{
	public bool isAlerted;
	public bool wasAttacked;

	private float knockbackRecoveryFraction = 3f;
	private float currentKnockbackRecoveryTime = 0f;

	private float staggerKnockbackVelocity = 2f;
	private float currentStaggerRecoveryTime = 0.1f;

	private Vector3 staggerDirection;

	internal Animator animator;
	internal EnemyStatus status;
	internal NavMeshAgent navAgent;

	public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	private void ChangeState()
	{
		lastState = CurrentState;
		timeEnteredState = Time.time;
	}

	public virtual void Awake()
	{
		animator = gameObject.GetComponentInChildren<Animator>();
		status = gameObject.GetComponent<EnemyStatus>();
		navAgent = gameObject.GetComponent<NavMeshAgent>();
		navAgent.isStopped = true;
	}

	public virtual void Update()
	{
		if (currentKnockbackRecoveryTime > 0)
		{
			currentKnockbackRecoveryTime -= Time.deltaTime;
		}
		else if (currentKnockbackRecoveryTime <= 0 && status.IsKnockedBack())
		{
			navAgent.velocity = navAgent.velocity / knockbackRecoveryFraction;
			navAgent.updateRotation = true;
			animator.SetBool("KnockedBack", false);
			status.BecomeFreeMoving();
		}

		if (currentStaggerRecoveryTime > 0)
		{
			currentStaggerRecoveryTime -= Time.deltaTime;
		}
		else if (currentStaggerRecoveryTime <= 0 && status.IsStaggered())
		{
			animator.SetBool("Staggered", false);
			navAgent.updateRotation = true;
			status.BecomeFreeMoving();
		}
	}

	public virtual void Attack(float damage)
	{

	}

	public void ReceiveAttack(float damage)
	{
		status.TakeDamage(damage);
	}

	public void ReceiveStaggerAttack(float damage, Vector3 staggerDirection, float staggerRecoveryTime)
	{
		status.BecomeStaggered();

		this.staggerDirection = staggerDirection;
		currentStaggerRecoveryTime = staggerRecoveryTime;
		animator.SetBool("Staggered", true);
		navAgent.enabled = true;
		navAgent.velocity = staggerDirection * staggerKnockbackVelocity;
		navAgent.updateRotation = false;

		status.TakeDamage(damage);
	}

	public void ReceiveKnockbackAttack(float damage, Vector3 knockbackDirection, float knockbackVelocity, float knockbackTime)
	{
		status.BecomeKnockedBack();

		currentKnockbackRecoveryTime = knockbackTime;
		animator.SetBool("KnockedBack", true);
		navAgent.enabled = true;
		navAgent.velocity = knockbackDirection * knockbackVelocity;
		navAgent.updateRotation = false;

		status.TakeDamage(damage);
	}

	public List<IAttackable> CheckInstantFrameHitboxForPlayer(BoxCollider hitbox, out bool hasEnemy)
	{
		Vector3 size = hitbox.size / 2;
		size.x = Mathf.Abs(size.x);
		size.y = Mathf.Abs(size.y);
		size.z = Mathf.Abs(size.z);
		ExtDebug.DrawBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation, Color.blue);
		int layerMask = LayerMask.GetMask(Helpers.Layers.Player);
		Collider[] colliders = Physics.OverlapBox(hitbox.transform.position + hitbox.transform.forward * 0.5f, size, hitbox.transform.rotation, layerMask);
		var results = new List<IAttackable>();

		foreach (Collider collider in colliders)
		{
			if (collider.tag == Helpers.Tags.Player)
			{
				var attackableComponent = collider.gameObject.GetAttackableComponent();
				if (attackableComponent != null)
					results.Add(attackableComponent);
			}
		}

		hasEnemy = results.Count > 0;
		return results;
	}

	internal virtual void Die()
	{
		navAgent.isStopped = true;
	}
}
