using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	private float rotX = 0.0f;
	private float rotY = 0.0f;

	private float distanceFromPlayer = 1.5f;
	private float pivotHeight = 0.8f;

	private GameObject player;
	private Camera playerCamera;
	private PlayerMovementStateMachine playerMovementStateMachine;
	private PlayerInputManager playerInputManager;

	public Vector3 AimDirectionVector
	{
		get { return playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)).direction; }
	}

	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag(Helpers.Tags.Player);
		playerCamera = gameObject.GetComponent<Camera>();
		playerMovementStateMachine = gameObject.GetComponentInParent<PlayerMovementStateMachine>();
		playerInputManager = gameObject.GetComponentInParent<PlayerInputManager>();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void LateUpdate()
	{
		MouseLook();
	}

	private void MouseLook()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			if (Input.GetButtonDown(InputCodes.PrimaryFire))
				Cursor.lockState = CursorLockMode.Locked;
		}

		rotX -= playerInputManager.Current.MouseInput.y * playerInputManager.X_MouseSensitivity * 0.02f;
		rotY += playerInputManager.Current.MouseInput.x * playerInputManager.Y_MouseSensitivity * 0.02f;

		if (rotX < -60)
			rotX = -60;
		else if (rotX > 80)
			rotX = 80;

		transform.root.rotation = Quaternion.Euler(0, rotY, 0);
		transform.localPosition = new Vector3(0, -Mathf.Cos((90 + rotX) * Mathf.Deg2Rad) + pivotHeight, -Mathf.Sin((90 + rotX) * Mathf.Deg2Rad)) * distanceFromPlayer;
		this.transform.rotation = Quaternion.Euler(rotX, rotY, 0);
	}
}
