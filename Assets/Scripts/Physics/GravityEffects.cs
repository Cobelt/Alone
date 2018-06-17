using UnityEngine;
using System.Collections;

public class GravityEffects : MonoBehaviour {

	public bool gravityActivated = true;

	private float gravity;
	private float maxJumpForce, minJumpForce; // Min & Max Jump Forces

	[Header ("Jump Parameters")]
	[SerializeField] private float maxJumpHeight = 11; // Min & Max Jump Heigh (needed for calculate gravity and the jump force)
	[SerializeField] private float minJumpHeight = 9;
	[SerializeField] private float timeToJumpApex = .4f; // The time that is needed to reach the top point

	// Use this for initialization
	void Start () {
		CalculateGravity ();
	
	}

	public void CalculateGravity () {
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2); // Setting gravity with the max jump heigh that is modifiable in the inspector
		maxJumpForce = Mathf.Abs (gravity) * timeToJumpApex; // Setting the max jung force with the gravity and the timeToJumpApex
		minJumpForce = Mathf.Sqrt(2*Mathf.Abs(gravity) * minJumpHeight); // Setting the min jump force with gravity & minJumpHeigh
	}

	public float GetGravityVelocity () { // Call the setGravity method before returning the gravity velocity
		return gravityActivated ? gravity * Time.deltaTime : 0;
	}

	public void ApplyGravity(ref float velocityYAxis) {
		velocityYAxis += gravityActivated ? gravity * Time.deltaTime : 0;
	}



	public float MaxJumpForce {
		get {
			return maxJumpForce;
		}
		set {
			maxJumpForce = value;
		}
	}

	public float MinJumpForce {
		get {
			return minJumpForce;
		}
		set {
			minJumpForce = value;
		}
	}
}
