//using System;
//using UnityEngine;

//public class PlayerMovementStateMachine : SuperStateMachine
//{
//	public const float JumpingSafeFallHeight = 2.3f * JumpHeight;
//	public const float FallingSafeFallHeight = 2.6f * JumpHeight;
//	public const float SafeHeightMultiplier = 1.5f;

//	public const float StandingRecoveryTime = 0.15f;
//	public const float CrouchRecoveryTime = 0.11f;
//	public const float FallingIntoRecoveryTime = 0.35f;

//	public const float SlideVelocity = 8f;
//	public const float SlideDuration = 0.5f;

//	private bool jumpInputHandled;
//	private float fallStartingHeight = 0f;

//	public bool InCrouchingState
//	{
//		get
//		{
//			return (PlayerMovementState)CurrentState == PlayerMovementState.Crouching ||
//		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRunning ||
//		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering ||
//		   (PlayerMovementState)CurrentState == PlayerMovementState.Sliding;
//		}
//	}

//	public bool IsRecovering
//	{
//		get
//		{
//			return (PlayerMovementState)CurrentState == PlayerMovementState.Recovering ||
//		   (PlayerMovementState)CurrentState == PlayerMovementState.CrouchRecovering;
//		}
//	}
//	#endregion

//	protected override void LateGlobalSuperUpdate()
//	{
//		//Debug.Log($"Movement is {LocalMovementCardinalDirection}");

//		if (Input.GetButtonUp(InputCodes.Jump))
//			jumpInputHandled = false;

//		//Check if you're in a crouching state
//		//Check above your head
//		if (InCrouchingState)
//			GoToCrouching();
//		else
//			GoToStanding();

//	}


//	#region Movement

//	#region Sliding
//	void Sliding_EnterState()
//	{
//		//Go into slide animation
//	}

//	void Sliding_SuperUpdate()
//	{
//		if (!MaintainingGround())
//		{
//			CurrentState = PlayerMovementState.Falling;
//			return;
//		}

//		if (TimeSinceEnteringCurrentState >= SlideDuration)
//		{

//			CurrentState = PlayerMovementState.CrouchRecovering;
//			return;
//		}

//		moveDirection = Vector3.MoveTowards(moveDirection, CameraMovement() * SlideVelocity, SlideVelocity * controller.deltaTime);
//	}
//	#endregion

//	#region Recovering
//	void Recovering_SuperUpdate()
//	{
//		if (!MaintainingGround())
//		{
//			CurrentState = PlayerMovementState.Falling;
//			return;
//		}

//		//Are we recovering from a fall, or something like a slide?
//		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

//		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) ||
//			(!recoverFromFalling && TimeSinceEnteringCurrentState >= StandingRecoveryTime))
//		{
//			CurrentState = PlayerMovementState.Standing;
//			return;
//		}

//		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, RunDeceleration * controller.deltaTime);
//	}
//	#endregion

//	#region CrouchRecovering
//	void CrouchRecovering_SuperUpdate()
//	{
//		if (!MaintainingGround())
//		{
//			CurrentState = PlayerMovementState.Falling;
//			return;
//		}

//		//Are we recovering from a fall, or something like a slide?
//		bool recoverFromFalling = ((PlayerMovementState)lastState == PlayerMovementState.Falling || (PlayerMovementState)lastState == PlayerMovementState.Jumping);

//		if ((recoverFromFalling && TimeSinceEnteringCurrentState >= FallingIntoRecoveryTime) ||
//			(!recoverFromFalling && TimeSinceEnteringCurrentState >= CrouchRecoveryTime))
//		{
//			if (!playerInputManager.Current.CrouchInput && PlayerCanExitCrouch())
//			{
//				CurrentState = PlayerMovementState.Recovering;
//				return;
//			}
//			else
//			{
//				if ((PlayerMovementState)lastState != PlayerMovementState.CrouchRecovering)
//				{
//					CurrentState = PlayerMovementState.CrouchRecovering;
//					return;
//				}
//				else
//				{
//					CurrentState = PlayerMovementState.Crouching;
//					return;
//				}
//			}
//		}

//		moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, CrouchDeceleration * controller.deltaTime);
//	}
//	#endregion

//	#region Jumping
//	void Jumping_EnterState()
//	{
//		controller.DisableClamping();
//		controller.DisableSlopeLimit();

//		fallStartingHeight = transform.position.y;
//		moveDirection += controller.up * CalculateJumpSpeed(JumpHeight, Gravity);
//	}

//	void Jumping_SuperUpdate()
//	{
//		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
//		Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

//		if (Vector3.Angle(verticalMoveDirection, controller.up) > 90 && AcquiringGround())
//		{
//			moveDirection = planarMoveDirection;
//			var fallenHeight = fallStartingHeight - transform.position.y;

//			if (fallenHeight >= SafeHeightMultiplier * JumpingSafeFallHeight)
//			{
//				CurrentState = PlayerMovementState.CrouchRecovering;
//				return;
//			}
//			else if (fallenHeight >= JumpingSafeFallHeight)
//			{
//				CurrentState = PlayerMovementState.Recovering;
//				return;
//			}
//			else
//			{
//				if (playerInputManager.Current.MoveInput != Vector3.zero && playerInputManager.Current.CrouchInput && LocalMovementIsForwardFacing)
//				{
//					CurrentState = PlayerMovementState.Sliding;
//					return;
//				}
//				else
//				{
//					CurrentState = PlayerMovementState.Standing;
//					return;
//				}
//			}
//		}

//		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, CameraMovement() * RunSpeed, AirControl * RunAcceleration * controller.deltaTime);
//		verticalMoveDirection -= controller.up * Gravity * controller.deltaTime;
//		moveDirection = planarMoveDirection + verticalMoveDirection;
//	}
//	#endregion

//	#region Falling
//	void Falling_EnterState()
//	{
//		controller.DisableClamping();
//		controller.DisableSlopeLimit();

//		fallStartingHeight = transform.position.y;
//	}

//	void Falling_SuperUpdate()
//	{
//		if (AcquiringGround())
//		{
//			moveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
//			var fallenHeight = fallStartingHeight - transform.position.y;

//			if (fallenHeight >= SafeHeightMultiplier * FallingSafeFallHeight)
//			{
//				CurrentState = PlayerMovementState.CrouchRecovering;
//				return;
//			}
//			else if (fallenHeight >= FallingSafeFallHeight)
//			{
//				CurrentState = PlayerMovementState.Recovering;
//				return;
//			}
//			else
//			{
//				CurrentState = PlayerMovementState.Standing;
//				return;
//			}
//		}

//		Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(controller.up, moveDirection);
//		planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, CameraMovement() * RunSpeed, AirControl * RunAcceleration * controller.deltaTime);
//		moveDirection -= controller.up * Gravity * controller.deltaTime;
//	}
//	#endregion

//	#endregion

//	private void GoToCrouching()
//	{
//		currentHeight = Mathf.Lerp(currentHeight, crouchingHeight,
//			Mathf.Min(TimeSinceEnteringCurrentState / ChangeStanceSpeed, 1));

//		var offsetRatio = currentHeight / standingHeight;
//		if (currentHeight <= standingHeight / 2)
//			controller.SetCrouchingSpheres();
//		controller.heightScale = Mathf.Clamp(offsetRatio, 0.5f, 1);
//		playerCollider.height = crouchingHeight;
//		playerCollider.center = new Vector3(0, crouchingHeight / 2, 0);
//	}

//	private void GoToStanding()
//	{
//		currentHeight = Mathf.Lerp(currentHeight, standingHeight,
//			Mathf.Min(TimeSinceEnteringCurrentState / ChangeStanceSpeed, 1));

//		var offsetRatio = currentHeight / standingHeight;
//		if (currentHeight > standingHeight / 2)
//			controller.SetStandingSpheres();
//		controller.heightScale = Mathf.Clamp(offsetRatio, 0.5f, 1);
//		playerCollider.height = standingHeight;
//		playerCollider.center = new Vector3(0, crouchingHeight, 0);
//	}
//}