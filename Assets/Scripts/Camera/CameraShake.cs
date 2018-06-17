using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraShake : MonoBehaviour {

	private float shakeTimer = 0;
	public float shakeAmount, shakeDuration;

	public void UpdateShake()
	{
		if (shakeTimer >= 0) 
		{
			Vector2 shakePosition = Random.insideUnitCircle * shakeAmount;
			transform.position = new Vector3 (transform.position.x + shakePosition.x, transform.position.y + shakePosition.y, transform.position.z + shakePosition.x);
			shakeTimer -= Time.deltaTime;
		}
	}

	public float shakerTimer {
		get {
			return shakeTimer;
		}
		set {
			shakeTimer = value;
		}
	}

	public void Shake() {
		shakeTimer = shakeDuration;
	}
} 