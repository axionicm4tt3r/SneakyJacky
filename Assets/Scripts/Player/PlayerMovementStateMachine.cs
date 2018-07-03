using System;
using UnityEngine;

public class PlayerMovementStateMachine : SuperStateMachine
{
	public const float RunSpeed = 7.0f;
	public const float RunAcceleration = RunSpeed * 9.0f;
	public const float RunDeceleration = RunSpeed * 8.0f;
	public const float CrouchSpeed = 4.5f;
	public const float CrouchAcceleration = CrouchSpeed * 9.0f;
	public const float CrouchDeceleration = CrouchSpeed * 8.0f;
	public const float WalkSpeedFactor = 0.5f;

	public const float JumpHeight = 1.3f;
	public const float Gravity = 25.0f;

	public const float AirControl = 0.6f;
	public const float ChangeStanceSpeed = 0.3f;

	public const float JumpingSafeFallHeight = 2.3f * JumpHeight;
	public const float FallingSafeFallHeight = 2.6f * JumpHeight;
	public const float SafeHeightMultiplier = 1.5f;

	public const float StandingRecoveryTime = 0.15f;
	public const float CrouchRecoveryTime = 0.11f;
	public const float FallingIntoRecoveryTime = 0.35f;

	public const float SlideVelocity = 8f;
	public const float SlideDuration = 0.5f;

	public Vector3 moveDirection;
	public Vector3 lookDirection { get; private set; }

	private SuperCharacterController controller;
	private CapsuleCollider playerCollider;
	private PlayerCamera playerCamera;
	private PlayerInputManager playerInputManager;
	private PlayerStatus playerStatus;
	private PlayerAttackStateMachine playerAttackStateMachine;

	//Change these if you mess with the player size
	private float collisionSphereSize = 0.4f;
	private Vector3 standingTorsoPosition = new Vector3(0, 1.2f, 0);
	private Vector3 standingHeadPosition = new Vector3(0, 1.6f, 0);

	private bool jumpInputHandled;
	private float fallStartingHeight = 0f;

	public Enum CurrentState { get { return currentState; } private set { ChangeState(); currentState = value; } }

	public float TimeSinceEnteringCurrentState { get { return Time.time - timeEnteredState; } }

	private void ChangeState()
	{
		lastState = CurrentState;
		timeEnteredState = Time.time;
	}

	public AngleDirection LocalMovementCardinalDirection
	{
		get
		{
			return Helpers.Direction(Vector2.SignedAngle(new Vector2(moveDirection.x, moveDirection.z), new Vector2(transform.forward.x, transform.forward.z)));
		}
	}

	#region PropertyGetters
	public bool LocalMovementIsForwardFacing
	{
		get
		{
			var movement = LocalMovementCardinalDirection;
			return (movement == AngleDirection.Forward) ||
				(movement == AngleDirection.ForwardLeft) ||
				(movement == AngleDirection.ForwardRight);
		}
	}

	public bool InCrouchingState
	{
		get
		{
			return (PlayerMovementState)CurrentState == PlayerMovementState.Crouching ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRunning ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.Sliding;
		}
	}

	public bool IsRecovering
	{
		get
		{
			return (PlayerMovementState)CurrentState == PlayerMovementState.Recovering ||
		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering;
		}
	}
	#endregion

	void Awake()
	{
		controller = gameObject.GetComponent<SuperCharacterController>();
		playerCollider = gameObject.GetComponent<CapsuleCollider>();
		playerCamera = gameObject.GetComponentInChildren<PlayerCamera>();
		playerInputManager = gameObject.GetComponent<PlayerInputManager>();
		playerStatus = gameObject.GetComponent<PlayerStatus>();
		playerAttackStateMachine = gameObject.GetComponent<PlayerAttackStateMachine>();
		lookDirection = transform.forward;
		CurrentState = PlayerMovementState.Standing;
	}

	protected override void EarlyGlobalSuperUpdate()
	{
		lookDirection = transform.forward;
	}

	protected override void LateGlobalSuperUpdate()
	{
		if (Input.GetButtonUp(InputCodes.Jump))
			jumpInputHandled = false;

		//Check if you're in a crouching state
		//Check above your head
		if (InCrouchingState)
			GoToCrouching();
		else
			GoToStanding();

		transform.position += moveDirection * controller.deltaTime;
	}

	public bool AcquiringGround()
	{
		return controller.currentGround.IsGrounded(false, 0.01f);
	}

	public bool MaintainingGround()
	{
		return controller.currentGround.IsGrounded(true, 0.4f);
	}

	private void OnBeforeClamp()
	{
		if ((PlayerMovementState)currentState == PlayerMovementState.Standing
			|| (PlayerMovementState)currentState == PlayerMovementState.Running
			|| (PlayerMovementState)currentState == PlayerMovementState.Crouching
			 || (PlayerMovementState)currentState == PlayerMovementState.CrouchRunning
			 || (PlayerMovementState)currentState == PlayerMovementState.Sliding)
		{
			if (!MaintainingGround())
			{
				currentState = PlayerMovementState.Falling;
				return;
			}
		}
	}

	public void RotateGravity(Vector3 up)
	{
		lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
	}

	private Vector3 LocalMovement()
	{
		Vector3 right = Vector3.Cross(controller.up, lookDirection);

		Vector3 local = Vector3.zero;

		if (playerInputManager.Current.MoveInput.x != 0)
		{
			local += right * playerInputManager.Current.MoveInput.x;
		}

		if (playerInputManager.Current.MoveInput.z != 0)
		{
			local += lookDirection * playerInputManager.Current.MoveInput.z;
		}

		return local.normalized;
	}

	private float CalculateJumpSpeed(float jumpHeight, float gravity)
	{
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	#region Movement

	#region Standing
	void Standing_EnterState()
	{
		controller.EnableSlopeLimit();
		controller.EnableClamping();
	}

	void Standing_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Running;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.CrouchRunning;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * controller.deltaTime);
	}
	#endregion

	#region Crouching
	void Crouching_EnterState()
	{
		controller.EnableSlopeLimit();
		controller.EnableClamping();
	}

	void Crouching_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.JumpInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (!playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Running;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && (playerInputManager.Current.CrouchInput || !PlayerCanExitCrouch()))
		{
			CurrentState = PlayerMovementState.CrouchRunning;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, CrouchDeceleration * controller.deltaTime);
	}
	#endregion

	#region Running
	void Running_SuperUpdate()
	{
		if (playerInputManager.Current.JumpInput)
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		//Fix up going into slide. Add a way for them to go into crouch fully without sliding
		if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput && LocalMovementIsForwardFacing)
		{
			CurrentState = PlayerMovementState.Sliding;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput && !LocalMovementIsForwardFacing)
		{
			CurrentState = PlayerMovementState.CrouchRunning;
			return;
		}

		if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}

		if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		var targetSpeed = playerInputManager.Current.WalkInput ? RunSpeed * WalkSpeedFactor : RunSpeed;
		moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * targetSpeed, RunAcceleration * controller.deltaTime);
	}
	#endregion

	#region CrouchRunning
	void CrouchRunning_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (playerInputManager.Current.JumpInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Jumping;
			return;
		}

		if (playerInputManager.Current.MoveInput != Vector3.zero && !playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Running;
			return;
		}

		if (playerInputManager.Current.MoveInput == Vector3.zero && !playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		if (playerInputManager.Current.MoveInput == Vector3.zero && playerInputManager.Current.CrouchInput)
		{
			CurrentState = PlayerMovementState.Crouching;
			return;
		}

		var targetSpeed = playerInputManager.Current.WalkInput ? CrouchSpeed * WalkSpeedFactor : CrouchSpeed;
		moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * targetSpeed, CrouchAcceleration * controller.deltaTime);
	}
	#endregion

	#region Sliding
	void Sliding_EnterState()
	{
		//Go into slide animation
	}

	void Sliding_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		if (TimeSinceEnteringCurrentState >= SlideDuration)
		{

			CurrentState = PlayerMovementState.CrouchRecovering;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * SlideVelocity, SlideVelocity * controller.deltaTime);
	}
	#endregion

	#region Recovering
	void Recovering_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		//Are we recovering from a fall, or something like a slide?
		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) ||
			(!recoverFromFalling && TimeSinceEnteringCurrentState >= StandingRecoveryTime))
		{
			CurrentState = PlayerMovementState.Standing;
			return;
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * controller.deltaTime);
	}
	#endregion

	#region CrouchRecovering
	void CrouchRecovering_SuperUpdate()
	{
		if (!MaintainingGround())
		{
			CurrentState = PlayerMovementState.Falling;
			return;
		}

		//Are we recovering from a fall, or something like a slide?
		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) ||
			(!recoverFromFalling && TimeSinceEnteringCurrentState >= CrouchRecoveryTime))
		{
			if (!playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
			{
				CurrentState = PlayerMovementState.Recovering;
				return;
			}
			else
			{
				if ((PlayerMovementState)lastState != PlayerMovementState.CrouchRecovering)
				{
					CurrentState = PlayerMovementState.CrouchRecovering;
					return;
				}
				else
				{
					CurrentState = PlayerMovementState.Crouching;
					return;
				}
			}
		}

		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, CrouchDeceleration * controller.deltaTime);
	}
	#endregion

	#region Jumping
	void Jumping_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		fallStartingHeight = transform.position.y;
		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
	}

	void Jumping_SuperUpdate()
	{
		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
		{
			moveDirection = planarMoveDirection;
			var fallenHeight = fallStartingHeight - transform.position.y;

			if (fallenHeight >= SafeHeightMultiplier * JumpingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.CrouchRecovering;
				return;
			}
			else if (fallenHeight >= JumpingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.Recovering;
				return;
			}
			else
			{
				if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput && LocalMovementIsForwardFacing)
				{
					CurrentState = PlayerMovementState.Sliding;
					return;
				}
				else
				{
					CurrentState = PlayerMovementState.Standing;
					return;
				}
			}
		}

		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, AirControl * RunAcceleration * controller.deltaTime);
		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
		moveDirection = planarMoveDirection + verticalMoveDirection;
	}
	#endregion

	#region Falling
	void Falling_EnterState()
	{
		controller.DisableClamping();
		controller.DisableSlopeLimit();

		fallStartingHeight = transform.position.y;
	}

	void Falling_SuperUpdate()
	{
		if (AcquiringGround())
		{
			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
			var fallenHeight = fallStartingHeight - transform.position.y;

			if (fallenHeight >= SafeHeightMultiplier * FallingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.CrouchRecovering;
				return;
			}
			else if (fallenHeight >= FallingSafeFallHeight)
			{
				CurrentState = PlayerMovementState.Recovering;
				return;
			}
			else
			{
				CurrentState = PlayerMovementState.Standing;
				return;
			}
		}

		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * RunSpeed, AirControl * RunAcceleration * controller.deltaTime);
		moveDirection -= controller.up * Gravity * controller.deltaTime;
	}
	#endregion

	#endregion

	private bool PlayerMovementStateShouldAllowClamping()
	{
		return (PlayerMovementState)CurrentState == PlayerMovementState.Standing
			|| (PlayerMovementState)CurrentState == PlayerMovementState.Running
			|| (PlayerMovementState)CurrentState == PlayerMovementState.Crouching
			|| (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRunning
			|| (PlayerMovementState)CurrentState == PlayerMovementState.Sliding;
	}

	private void GoToCrouching()
	{
		controller.SetCrouchingSpheres();
		controller.heightScale = 0.5f;
	}

	private void GoToStanding()
	{
		controller.SetStandingSpheres();
		controller.heightScale = 1f;
	}

	private bool PlayerCanExitCrouch()
	{
		var torsoCheck = Physics.OverlapSphere(transform.position + standingTorsoPosition, collisionSphereSize, LayerMask.GetMask(Helpers.Layers.Default));
		var headCheck = Physics.OverlapSphere(transform.position + standingHeadPosition, collisionSphereSize, LayerMask.GetMask(Helpers.Layers.Default));

		return torsoCheck.Length == 0 && headCheck.Length == 0;
	}

	//private void OnDrawGizmos()
	//{
	//	Gizmos.color = Color.red;
	//	Gizmos.DrawWireSphere(transform.position + standingTorsoPosition, collisionSphereSize);
	//	Gizmos.color = Color.blue;
	//	Gizmos.DrawWireSphere(transform.position + standingHeadPosition, collisionSphereSize);
	//}
}

public enum PlayerMovementState
{
	Standing,
	Crouching,
	Running,
	CrouchRunning,
	Sliding,
	Recovering,
	CrouchRecovering,
	Jumping,
	Falling
}