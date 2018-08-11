using UnityEngine;
using System.Collections;
using SensorToolkit;
using System.Linq;

public class SecurityCamera : MonoBehaviour
{
	public float RotationSpeed;
	public float ScanTime;
	public float TrackTime;
	public float SearchTime;
	public float ScanArcAngle;
	public Light SpotLight;
	public TriggerSensor FieldOfViewSensor;
	public RangeSensor RangeSensor;
	public Color ScanColour;
	public Color TrackColour;
	public Color AlarmColour;

	Quaternion leftExtreme;
	Quaternion rightExtreme;
	Quaternion targetRotation;

	void Awake()
	{
		leftExtreme = Quaternion.AngleAxis(ScanArcAngle / 2f, Vector3.up) * transform.rotation;
		rightExtreme = Quaternion.AngleAxis(-ScanArcAngle / 2f, Vector3.up) * transform.rotation;
	}

	void OnEnable()
	{
		targetRotation = transform.rotation;
		transform.rotation = rightExtreme;
		StartCoroutine(ScanState());
	}

	void Update()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
	}

	IEnumerator ScanState()
	{
		StartCoroutine(ScanMovement());
		while (true)
		{
			if (AlarmManager.Instance.IsAlarmState)
			{
				StopAllCoroutines();
				StartCoroutine(AlarmState());
				break;
			}
			else if (GetSpottedEnemy() != null)
			{
				StopAllCoroutines();
				StartCoroutine(TrackState());
				break;
			}
			yield return null;
		}
	}

	IEnumerator ScanMovement()
	{
		SpotLight.color = ScanColour;
		while (true)
		{
			targetRotation = leftExtreme;
			yield return new WaitForSeconds(ScanTime);
			targetRotation = rightExtreme;
			yield return new WaitForSeconds(ScanTime);
		}
	}

	IEnumerator TrackState()
	{
		SpotLight.color = TrackColour;
		var enemy = GetSpottedEnemy();
		var detectionTimer = 0f;
		var searchTimer = 0f;
		bool lostSightOfPlayer = false;

		while (searchTimer < SearchTime)
		{
			if (RangeSensor.IsDetected(enemy) && !lostSightOfPlayer)
				targetRotation = Quaternion.LookRotation(transform.position - enemy.transform.position, Vector3.up);
			else
				lostSightOfPlayer = true;

			if (FieldOfViewSensor.IsDetected(enemy))
			{
				searchTimer = 0;
				detectionTimer += Time.deltaTime;
				if (detectionTimer >= TrackTime)
				{
					AlarmManager.Instance.StartAlarm(enemy);
					StopAllCoroutines();
					StartCoroutine(AlarmState());
					break;
				}
			}

			searchTimer += Time.deltaTime;
			if (searchTimer >= SearchTime)
			{
				StopAllCoroutines();
				StartCoroutine(ScanState());
				break;
			}
			yield return null;
		}

		StopAllCoroutines();
		StartCoroutine(ScanState());
	}

	IEnumerator AlarmState()
	{
		targetRotation = transform.rotation;
		SpotLight.color = AlarmColour;
		yield return null;
	}

	GameObject GetSpottedEnemy()
	{
		var entities = FieldOfViewSensor.GetDetectedByComponent<Player>();
		return entities.FirstOrDefault()?.gameObject;
	}
}
