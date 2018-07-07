//using System;
//using UnityEngine;

//public class PlayerMovementStateMachine : SuperStateMachine
//{

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