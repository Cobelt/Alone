using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof(SimpleActions))]
[RequireComponent (typeof(MovementActions))]
public class ActionsController : MonoBehaviour {


	private SimpleActions simpleActions;
	private MovementActions mvtActions;

	public bool canWalk, canRun, canJump, canDoublejump, canLookAbove, canLookBelow, canWallJump, canGrab, canGrabWall, canDash, canCollect, canCarryObject, canThrowObject, canMakeSpiritAppear, canChangeControl; // Allow you to allow or disallow some capacities

	[HideInInspector] public bool grab, jumpedOnce, run, walk; 
	[HideInInspector] public bool haveToWalk, haveSomethingToGrab; // en attendant de trouver mieux on stock 1/2 var pour chaque action

	private GameObject objectAbleToGrab;

	// Use this for initialization
	void Awake () {
		simpleActions = GetComponent<SimpleActions> ();
		mvtActions = GetComponent<MovementActions> ();
	}

	public void ActionDetection (bool collisionBelow) {
		/// JUMP ///
		if (CrossPlatformInputManager.GetButtonDown ("Jump") && canJump) {
			movement.JumpInputDown ();
		} 
		else if (CrossPlatformInputManager.GetButtonUp ("Jump") && canJump) {
			movement.JumpInputUp ();
		}

		/// WALK ///
		walk = (CrossPlatformInputManager.GetButton ("Walk") && canWalk || haveToWalk);

		/// RUN LIKE A DOG ON 4 LEGS ///
		run = (CrossPlatformInputManager.GetButton ("Creep") && canRun && !grab);
		// animator.SetBool("Creeping", true);

		/// GRAB AN OBJECT OR A WALL ///
		grab = (CrossPlatformInputManager.GetButton ("Grab") && collisionBelow && haveSomethingToGrab && canGrab);
		// animator.SetBool("Grabbing", true);


		/// LOOKING ABOVE BELOW AHEAD ///
		if (CrossPlatformInputManager.GetAxis ("Vertical") > 0 && CrossPlatformInputManager.GetAxis ("Horizontal") == 0 && canLookAbove) { 	
			simple.LookAbove ();
		} 
		else if (CrossPlatformInputManager.GetAxis ("Vertical") < 0 && CrossPlatformInputManager.GetAxis ("Horizontal") == 0 && canLookBelow) {
			simple.LookBelow ();
		} 
		else if (CrossPlatformInputManager.GetAxis ("Vertical") == 0) {
			simple.LookAhead ();
		}

		/// DASH ///
		if (CrossPlatformInputManager.GetButtonDown ("Dash") && canDash && (!grab || grab && !collisionBelow)) {
			movement.Dash ();
		}
	}


	public SimpleActions simple {
		get {
			return simpleActions;
		}
	}

	public MovementActions movement {
		get {
			return mvtActions;
		}
	}

	public GameObject ObjectAbleToGrab {
		get {
			return this.objectAbleToGrab;
		}
		set {
			objectAbleToGrab = value;
		}
	}
	public bool HaveSomethingToGrab {
		get {
			return this.haveSomethingToGrab;
		}
		set {
			haveSomethingToGrab = value;
		}
	}

}
