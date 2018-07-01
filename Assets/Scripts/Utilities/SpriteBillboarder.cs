using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarder : MonoBehaviour {

	public Sprite sprite_front;
	public Sprite sprite_front_left;
	public Sprite sprite_left;
	public Sprite sprite_rear_left;
	public Sprite sprite_rear;
	public Sprite sprite_rear_right;
	public Sprite sprite_right;
	public Sprite sprite_right_front;

	SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
		Vector3 playerPosition = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).transform.position;
		var playerDirection = playerPosition - transform.root.position;

		Vector2 playerDirectionXZ = new Vector2(playerDirection.x, playerDirection.z).normalized;
		Vector2 spriteRootObjectForwardXZ = new Vector2(transform.root.forward.x, transform.root.forward.z).normalized;
		var angle = Vector2.SignedAngle(playerDirectionXZ, spriteRootObjectForwardXZ);

		CalculateSprite(angle);

		Vector3 playerPositionXZ = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
		Quaternion facePlayerAngles = Quaternion.LookRotation(transform.position - playerPositionXZ, Vector3.up);

		transform.rotation = facePlayerAngles;
	}

	private void CalculateSprite(float angle)
	{
		if (angle < 0)
			angle += 360.0f;

		//Sprites must be in the array in clockwise direction for the Sprite[]
		//i.e. front, front-left, left, rear-left, rear, rear-right, right, right-front

		if (angle < 22.5 || angle > 337.5)
			spriteRenderer.sprite = sprite_front;
		else if (angle >= 22.5 && angle < 67.5)
			spriteRenderer.sprite = sprite_front_left;
		else if (angle >= 67.5 && angle < 112.5)
			spriteRenderer.sprite = sprite_left;
		else if (angle >= 112.5 && angle < 157.5)
			spriteRenderer.sprite = sprite_rear_left;
		else if (angle >= 157.5 && angle < 202.5)
			spriteRenderer.sprite = sprite_rear;
		else if (angle >= 202.5 && angle < 247.5)
			spriteRenderer.sprite = sprite_rear_right;
		else if (angle >= 247.5 && angle < 292.5)
			spriteRenderer.sprite = sprite_right;
		else if (angle >= 292.5 && angle < 337.5)
			spriteRenderer.sprite = sprite_right_front;
	}
}
