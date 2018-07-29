using UnityEngine;
using System.Collections;
using SensorToolkit;
using System.Linq;

public class SecurityCamera : MonoBehaviour
{
	public float RotationSpeed;
	public float ScanTime;
	public float TrackTime;
	public float ScanArcAngle;
	public Light SpotLight;
	public Sensor Sensor;
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
		var timer = 0f;
		while (Sensor.IsDetected(enemy))
		{
			targetRotation = Quaternion.LookRotation(transform.position - enemy.transform.position, Vector3.up);
			timer += Time.deltaTime;
			if (timer >= TrackTime)
			{
				AlarmManager.Instance.StartAlarm(enemy);
				StopAllCoroutines();
				StartCoroutine(AlarmState());
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
		var entities = Sensor.GetDetectedByComponent<Player>();
		return entities.FirstOrDefault()?.gameObject;
	}
}
