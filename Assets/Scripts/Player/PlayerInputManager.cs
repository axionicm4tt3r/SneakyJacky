using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	public PlayerInput Current;

	public float X_MouseSensitivity = 45.0f;
	public float Y_MouseSensitivity = 45.0f;

	void Update()
	{
		Vector3 moveInput = new Vector3(Input.GetAxisRaw(InputCodes.Horizontal), 0, Input.GetAxisRaw(InputCodes.Vertical));
		Vector2 mouseInput = new Vector2(Input.GetAxisRaw(InputCodes.MouseX), Input.GetAxisRaw(InputCodes.MouseY));

		bool jumpInput = Input.GetButtonDown(InputCodes.Jump);
		bool crouchInput = Input.GetButton(InputCodes.Crouch);
		bool primaryFireInput = Input.GetButton(InputCodes.PrimaryFire);
		bool secondaryFireInput = Input.GetButton(InputCodes.SecondaryFire);
		bool interactInput = Input.GetButtonDown(InputCodes.Interact);

		Current = new PlayerInput()
		{
			MoveInput = moveInput,
			MouseInput = mouseInput,
			JumpInput = jumpInput,
			CrouchInput = crouchInput,
			PrimaryFireInput = primaryFireInput,
			SecondaryFireInput = secondaryFireInput,
			InteractInput = interactInput
		};
	}

	public struct PlayerInput
	{
		public Vector3 MoveInput;
		public Vector2 MouseInput;
		public bool JumpInput;
		public bool CrouchInput;
		public bool PrimaryFireInput;
		public bool SecondaryFireInput;
		public bool InteractInput;
	}
}

public static class InputCodes
{
	public const string Horizontal = "Horizontal";
	public const string Vertical = "Vertical";
	public const string MouseX = "Mouse X";
	public const string MouseY = "Mouse Y";
	public const string Jump = "Jump";
	public const string Crouch = "Crouch";
	public const string PrimaryFire = "PrimaryFire";
	public const string SecondaryFire = "SecondaryFire";
	public const string Interact = "Interact";
}
