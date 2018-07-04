using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionTargetManager : MonoBehaviour
{

	[SerializeField]
	private Vector3 DistanceFromCharacter;

	void Start()
	{
		ResetPosition();
	}

	void Update()
	{
	}

	public void ResetPosition()
	{
		transform.localPosition = DistanceFromCharacter;
	}

	public Vector3 GetTargetPosition()
	{
		return transform.position;
	}
}
