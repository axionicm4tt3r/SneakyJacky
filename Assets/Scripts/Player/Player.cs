﻿using UnityEngine;

public class Player : MonoBehaviour
{
	public float MouseSensitivity = 0.01f;
	private PlayerController playerController;
	private PlayerCamera playerCamera;

	private void Awake()
	{
		playerController = gameObject.GetComponent<PlayerController>();
		playerCamera = GameObject.FindGameObjectWithTag(Helpers.Tags.PlayerCamera).GetComponent<PlayerCamera>();
	}

	private void Start()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Tell camera to follow transform
		playerCamera.SetFollowCharacter(playerController);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		HandleCameraInput();
		HandleMovementInput();
		HandleInteractionInput();
	}

	private void HandleCameraInput()
	{
		// Create the look input vector for the camera
		float mouseLookAxisUp = Input.GetAxisRaw(InputCodes.MouseYInput);
		float mouseLookAxisRight = Input.GetAxisRaw(InputCodes.MouseXInput);
		Vector3 lookInputVector = new Vector3(mouseLookAxisRight * MouseSensitivity, mouseLookAxisUp * MouseSensitivity, 0f);

		// Prevent moving the camera while the cursor isn't locked
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			lookInputVector = Vector3.zero;
		}

		// Apply inputs to the camera
		playerCamera.UpdateWithInput(Time.deltaTime, playerController.ZoomLevelFromMovementState, lookInputVector);
	}

	private void HandleMovementInput()
	{
		PlayerMovementInputs movementInputs = new PlayerMovementInputs();

		movementInputs.MoveAxisForward = Input.GetAxisRaw(InputCodes.VerticalInput);
		movementInputs.MoveAxisRight = Input.GetAxisRaw(InputCodes.HorizontalInput);
		movementInputs.CameraRotation = playerCamera.Transform.rotation;
		movementInputs.Walk = Input.GetButton(InputCodes.Walk);
		movementInputs.Jump = Input.GetButtonDown(InputCodes.Jump);
		movementInputs.CrouchDown = Input.GetButtonDown(InputCodes.Crouch);
		movementInputs.CrouchUp = Input.GetButtonUp(InputCodes.Crouch);

		playerController.SetInputs(ref movementInputs);
	}

	private void HandleInteractionInput()
	{
		PlayerInteractionInputs interactionInputs = new PlayerInteractionInputs();

		interactionInputs.PrimaryFire = Input.GetButtonDown(InputCodes.PrimaryFire);
		interactionInputs.SecondaryFire = Input.GetButtonDown(InputCodes.SecondaryFire);
		interactionInputs.Interact = Input.GetButtonDown(InputCodes.Interact);
	}

	public struct PlayerMovementInputs
	{
		public float MoveAxisRight;
		public float MoveAxisForward;
		public Quaternion CameraRotation;
		public bool Walk;
		public bool Jump;
		public bool CrouchDown;
		public bool CrouchUp;
	}

	public struct PlayerInteractionInputs
	{
		public bool PrimaryFire;
		public bool SecondaryFire;
		public bool Interact;
	}

	public static class InputCodes
	{
		public const string HorizontalInput = "Horizontal";
		public const string VerticalInput = "Vertical";
		public const string MouseXInput = "Mouse X";
		public const string MouseYInput = "Mouse Y";
		public const string Walk = "Walk";
		public const string Jump = "Jump";
		public const string Crouch = "Crouch";
		public const string PrimaryFire = "PrimaryFire";
		public const string SecondaryFire = "SecondaryFire";
		public const string Interact = "Interact";
	}
}
