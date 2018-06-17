using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Attacks : MonoBehaviour {

	[HideInInspector] public Controller2D controller;

	public const int NONE = 0, WHIP = 1, EPEE = 2, SHIELD = 3;

	public float m_rightHandDamages, m_leftHandDamages;

	void Awake () {
		controller = GetComponent<Controller2D>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
}
