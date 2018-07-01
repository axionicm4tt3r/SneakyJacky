using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
	PlayerAttackStateMachine playerController;
	PlayerAttackManager playerAttackManager;

	void Start()
	{
		playerController = GetComponentInParent<PlayerAttackStateMachine>();
		playerAttackManager = GetComponentInParent<PlayerAttackManager>();
	}

	public void OnTriggerStay(Collider collider)
	{
		var attackableComponent = collider.gameObject.GetAttackableComponent();
		if (attackableComponent != null)
		{
			switch((PlayerAttackState)playerController.CurrentState)
			{
				case PlayerAttackState.ChargeAttacking:
					playerAttackManager.ChargeAttack(attackableComponent);
					break;
				case PlayerAttackState.JumpKicking:
					playerAttackManager.JumpKick(attackableComponent);
					break;
				case PlayerAttackState.SlideKicking:
					playerAttackManager.SlideKick(attackableComponent);
					break;
				default:
					break;
			}
		}
	}
}
