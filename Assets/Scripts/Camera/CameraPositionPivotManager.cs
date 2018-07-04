using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionPivotManager : MonoBehaviour
{

	[HideInInspector]
	public float pitchInput = 0;

	[SerializeField]
	private float pitchMaxAngle = 45;
	[SerializeField]
	private float pitchMinAngle = -10;

	private float timeEffect = 1;
	private CameraPositionTargetManager cameraPositionTarget;

	void Start()
	{
		cameraPositionTarget = gameObject.GetComponentInChildren<CameraPositionTargetManager>();
	}

	void Update()
	{

		if (pitchInput != 0)
		{
			Vector3 objRotation = transform.rotation.eulerAngles;
			float oldPitch = objRotation.x > 180 ? objRotation.x - 360 : objRotation.x;
			float newPitch = oldPitch + (pitchInput * Time.deltaTime / timeEffect);
			float clampedPitch = Mathf.Clamp(newPitch, pitchMinAngle, pitchMaxAngle);
			transform.localEulerAngles = new Vector3(clampedPitch, 0, 0);
		}
	}

	public void ResetPosition()
	{
		transform.localEulerAngles = new Vector3(0, 0, 0);
		cameraPositionTarget.ResetPosition();
	}

	public Vector3 GetTagetPosition()
	{
		return cameraPositionTarget.GetTargetPosition();
	}

	public void SetTimeEffect(float time)
	{
		timeEffect = time;
	}
}
