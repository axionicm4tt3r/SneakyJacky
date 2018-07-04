using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[HideInInspector]
	public float yawInput = 0;
	[HideInInspector]
	public float pitchInput = 0;


	private PlayerMovementStateMachine playerMovementStateMachine;
	private PlayerInputManager playerInputManager;
	private Transform lookTarget;
	private CameraPositionPivotManager pivotTarget;
	private PlayerStatus playerStatus;

	private Vector3 standingDistanceFromTarget = new Vector3(0f, 0f, -2.5f);
	private float standingFoV = 60f;
	private Vector3 crouchingDistanceFromTarget = new Vector3(0f, 0f, -1.2f);
	private float crouchingFoV = 45f;

	private Vector3 distanceFromTarget;

	private float changeCameraSpeed = 0.15f;
	private float PITCH_MAX_ANGLE = 65.0f;
	private float PITCH_MIN_ANGLE = -25.0f;

	private float oldPitch = 0f;

	void Awake()
	{
		distanceFromTarget = standingDistanceFromTarget;

		playerMovementStateMachine = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerMovementStateMachine>();
		playerInputManager = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerInputManager>();
		lookTarget = GameObject.FindGameObjectWithTag(Helpers.Tags.CameraLookTarget).transform;
		pivotTarget = GameObject.FindGameObjectWithTag(Helpers.Tags.CameraPositionPivot).GetComponent<CameraPositionPivotManager>();
		playerStatus = GameObject.FindGameObjectWithTag(Helpers.Tags.Player).GetComponent<PlayerStatus>();
		transform.position = lookTarget.position + transform.rotation * distanceFromTarget;
	}

	void LateUpdate()
	{
		yawInput = playerInputManager.Current.MouseInput.x * playerInputManager.X_MouseSensitivity;
		pitchInput = -playerInputManager.Current.MouseInput.y * playerInputManager.Y_MouseSensitivity;

		ShiftDistanceFromCamera();
		BasicFreeCameraMovement();
	}

	private void ShiftDistanceFromCamera()
	{
		if (playerMovementStateMachine.InCrouchingState)
		{
			distanceFromTarget = Vector3.Lerp(distanceFromTarget, crouchingDistanceFromTarget,
					   Mathf.Min(playerMovementStateMachine.TimeSinceEnteringCurrentState / changeCameraSpeed, 1));
		}
		else
		{
			distanceFromTarget = Vector3.Lerp(distanceFromTarget, standingDistanceFromTarget,
					   Mathf.Min(playerMovementStateMachine.TimeSinceEnteringCurrentState / changeCameraSpeed, 1));
		}
		
	}

	#region FreeCameraStyles
	private void BasicFreeCameraMovement()
	{
		if (yawInput != 0)
			transform.RotateAround(lookTarget.position, Vector3.up, Time.deltaTime * yawInput);

		Vector3 objRotation = transform.rotation.eulerAngles;

		if (pitchInput != 0)
			oldPitch = objRotation.x > 180 ? objRotation.x - 360 : objRotation.x;

		float newPitch = oldPitch + (pitchInput * Time.deltaTime);
		float clampedPitch = Mathf.Clamp(newPitch, PITCH_MIN_ANGLE, PITCH_MAX_ANGLE);
		transform.localEulerAngles = new Vector3(clampedPitch, objRotation.y, objRotation.z);

		transform.position = lookTarget.position + transform.rotation * distanceFromTarget;
		transform.LookAt(lookTarget.position);
	}
	#endregion
}