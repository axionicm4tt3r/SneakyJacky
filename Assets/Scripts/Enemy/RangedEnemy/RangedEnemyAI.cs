using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyAI : EnemyAI
{
	public Transform ProjectilePrefab;

	private Transform shotSource;

	public override void Awake()
	{
		base.Awake();
		shotSource = this.gameObject.FindObjectInChildren("ShotSource").transform;
	}

	public override void Update()
	{
		base.Update();

		if (status.IsDead()) //Remove this code
		{
			Die();
		}
	}

	internal override void Die() //Remove this code
	{
		base.Die();
	}

	public override void Attack(float damage)
	{
		base.Attack(damage);

		//var playerCentrePosition = (player.position + new Vector3(0, player.GetComponent<CharacterController>().height / 2, 0));
		//var projectile = Instantiate(ProjectilePrefab, shotSource.position, Quaternion.LookRotation(playerCentrePosition - shotSource.position));
		var projectile = Instantiate(ProjectilePrefab, shotSource.position, transform.rotation);
		var projectileScript = projectile.GetComponent<Projectile>();

		projectileScript.damage = damage;
		projectileScript.parentId = gameObject.GetInstanceID();
	}
}
