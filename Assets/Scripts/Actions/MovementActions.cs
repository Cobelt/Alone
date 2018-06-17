using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class MovementActions : MonoBehaviour {

	private Controller2D controller; 							// Reference to the Controller2D
	private Animator animator;							// Reference to the animator attached to the sprite

	[Header ("WallClimbing Parameters")]
	[SerializeField] private WallClimbing wallClimbing = new WallClimbing ();

	[Header ("Dash")]
	public Vector2 dashVelocity; // Dash controller.Velocity on x and y axis


	private bool setDoubleJumpAnimFalse = false;

	void Awake () {
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
	}

	void Update () {
		if (setDoubleJumpAnimFalse) {
			//animator.SetBool ("Double Jump", false);
			setDoubleJumpAnimFalse = false;
		}
	}


	public bool WallSliding(Vector2 directionalInput) { // check if the player slide on the wall

		if(wallClimbing.SlideOnWall(controller, directionalInput)) { 					// Call SlideOnWall method in the controller.wallclimbing class
			if(controller.Actions.canGrabWall && controller.Actions.grab) {
				Vector2 velocityTmp = controller.Velocity;
				velocityTmp.y = 0;
				controller.Velocity = velocityTmp;
			}
			controller.Actions.jumpedOnce = false;

			//animator.SetBool ("SlideOnWall", true);
			return true;
		}

		//animator.SetBool ("SlideOnWall", false);
		return false;
	}


	// Jump functions
	public void JumpInputDown () {

		Vector2 velocityTmp = controller.Velocity;
		//bool jumpedOnce = controller.Actions.getAction("jump").isDoing;

		if(controller.Actions.canWallJump && (wallClimbing.WallSliding || controller.Actions.grab && !controller.collisions.below)) {


			if (controller.Input.x == 0){ 																		// if no x-axis-inputs
				velocityTmp.x = -wallClimbing.WallDirX * wallClimbing.wallJumpOff.x; 		// Jump off the wall
				velocityTmp.y = wallClimbing.wallJumpOff.y;
			}

			else if( Mathf.Sign(controller.Input.x) == wallClimbing.WallDirX ) { 					// Input X between 1 and 0 if wall is on right and between -1 and 0 if wall is on left
				velocityTmp.x = -wallClimbing.WallDirX * wallClimbing.wallJumpClimb.x; 		// Climb on the same wall
				velocityTmp.y = wallClimbing.wallJumpClimb.y;
			}

			else {																										// if wall is on right && input in direction of left || wall on left && input in direction of right
				velocityTmp.x = -wallClimbing.WallDirX * wallClimbing.wallLeap.x;  			// Long jump to reach an other wall
				velocityTmp.y = wallClimbing.wallLeap.y;
			}

			controller.Velocity = velocityTmp;

			//animator.SetBool ("Jump", true);
			controller.Actions.jumpedOnce = true;

		}

		else if(controller.collisions.below || controller.Actions.canDoublejump && controller.Actions.jumpedOnce) {				// If grounded or already jumping but can double jump
			//animator.SetBool ( (controller.Actions.jumpedOnce) ? "Double Jump" : "Jump", true);
			if (controller.Actions.jumpedOnce) {
				setDoubleJumpAnimFalse = true;
			}

			velocityTmp.y = controller.gravityEffects.MaxJumpForce;											// Do a max jump (which can be reduced if the input is up too fast)
			controller.Velocity = velocityTmp;
			controller.Actions.jumpedOnce = !controller.Actions.jumpedOnce;																	// If jump first time : true / If jump the second time : false
		}
	}

	public void JumpInputUp () {
		if(controller.Velocity.y > controller.gravityEffects.MinJumpForce) { 	// If the input is up too fast, the controller.Velocity is reduced
			Vector2 velocityTmp = controller.Velocity;
			velocityTmp.y = controller.gravityEffects.MinJumpForce; 	// Allow jump with variable heigh
			controller.Velocity = velocityTmp;
		}
		//animator.SetBool ("Jump", false);
		//animator.SetBool ("Double Jump", false);
	}



	public void Dash() {
		Vector2 tmpVeloc = dashVelocity;
		tmpVeloc.x *= controller.faceDir;
		Debug.Log ("dash!");
		controller.Move(tmpVeloc, Vector2.zero);
	}




	[System.Serializable]
	public class WallClimbing {

		public Vector2 wallJumpClimb, wallJumpOff, wallLeap;

		public float timeToWallUnstick;

		private float wallSlideSpeedMax = 3;
		private float wallStickTime = .25f;


		private bool wallSliding;
		private int wallDirX;


		public bool WallSliding {
			get {
				return this.wallSliding;
			}
			set {
				wallSliding = value;
			}
		}

		public int WallDirX {
			get {
				return this.wallDirX;
			}
			set {
				wallDirX = value;
			}
		}



		public bool SlideOnWall (Controller2D ctrler, Vector2 input) {

			wallDirX = (ctrler.collisions.left) ? -1 : (ctrler.collisions.right) ? 1 : 0; // collision is the CollisionInfo of the Objet

			if((ctrler.collisions.left || ctrler.collisions.right) && !ctrler.collisions.below && ctrler.Velocity.y < 0) { // If collide with a wall && in air && falling

				Vector2 velocityTmp = ctrler.Velocity;
				if (ctrler.Velocity.y < -wallSlideSpeedMax) {			 // Make the sliding speed constant
					velocityTmp.y = -wallSlideSpeedMax;
				}

				if (timeToWallUnstick > 0) {						// Make you stick the wall during a little time to allow you to do a Jump Leap
					velocityTmp.x = 0;

					if (input.x != wallDirX && input.x != 0) {
						timeToWallUnstick -= Time.deltaTime;
					}
					else {
						timeToWallUnstick = wallStickTime;
					}
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
					
				ctrler.Velocity = velocityTmp;

				wallSliding = true;
				return true;
			}

			wallSliding = false;
			return false;
		}
	}

}
