using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent (typeof(ActionsController))]
public class Controller2D : Objet {


	[Header ("LayerMasks")]
	public LayerMask cantWallJumpOnIt;
	public LayerMask canGoDown;

	[Header ("Player Speed")]
	[Range (15,25)] public int normalSpeed = 20;

	private Transform oldObjectStuckedIn;

	public bool isPlayer = true;

	private ActionsController actions;							// Reference to the Enable Actions

	protected override void Awake () {
		actions = GetComponent<ActionsController> ();
		base.Awake ();
	}

	protected override void Update () {
		
		input = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis ("Vertical"));
		velocity.x = input.x * normalSpeed;
		actions.ActionDetection (collisions.below);

		base.Update ();

		if(collisions.above || collisions.below) {
			velocity.y = 0;
			actions.jumpedOnce = false;
		}
	}

	public override Vector2 Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
		Physics2D.queriesStartInColliders = false;
		if (actions.grab) {
			actions.ObjectAbleToGrab.GetComponent<Objet> ().Move (moveAmount, standingOnPlatform);
		}
		else if (moveAmount.x != 0 && Mathf.Sign(moveAmount.x) != Mathf.Sign(faceDir)) {
			Flip ();
		}

		Vector2 mvtDone = base.Move(moveAmount, input, standingOnPlatform);

		/* animator
		m_player.animator.SetBool ("Grounded", (collisions.below) ? true : false);			
		m_player.animator.SetFloat ("VelocityX", Mathf.Abs(mvtDone.x)*10);
		*/

		return mvtDone;
	}
		
	public override void VerticalCollision (ref Vector2 moveAmount, bool standingOnPlatform)
	{
		if(isPlayer) {
			base.VerticalCollision (ref moveAmount, standingOnPlatform);
		}
		else {

			float directionY = Mathf.Sign(moveAmount.y);
			float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;


			for (int i = 0; i < VerticalRayCount; i++) {


				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (VerticalRaySpacing * i + moveAmount.x);

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, whatIsGround+canGoDown);

				Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.white);

				if (hit) {

					if (hit.collider.tag == "Through") {

						if (directionY == 1 || hit.distance == 0) {
							continue;
						}
						if (collisions.fallingThroughPlatform) {
							continue;
						}
						if (Mathf.Sign (input.y) == -1) {
							collisions.fallingThroughPlatform = true;
							Invoke ("ResetFallingThroughPlatform", .5f);
							continue;
						}

					}

					if (((1 << hit.transform.gameObject.layer) & canGoDown) != 0) {
						if (directionY == 1 || hit.distance == 0) {
							continue;
						}
						if (Mathf.Sign (input.y) == -1) {
							oldObjectStuckedIn = hit.transform;
						}
						if (hit.transform == oldObjectStuckedIn) { 
							continue;
						}
					}

					if (!standingOnPlatform) {
						moveAmount.y = (hit.distance - skinWidth) * directionY;
					} else {
						float tmpDist = (hit.distance - skinWidth >= 0) ? skinWidth : 0f;
						moveAmount.y = (hit.distance - tmpDist) * directionY;
					}
					rayLength = hit.distance;

					if (collisions.climbingSlope) {

						moveAmount.x = moveAmount.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
					}

					collisions.below = directionY == -1;
					collisions.above = directionY == 1;

				} else
					oldObjectStuckedIn = null;
			}

			if(collisions.climbingSlope) {

				rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
				Vector2 rayOrigin = ((faceDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;

				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDir, rayLength, whatIsGround);

				if(hit) {

					float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
					if(slopeAngle != collisions.slopeAngle) { // if new slope
						moveAmount.x = (hit.distance - skinWidth) * faceDir;
						collisions.slopeAngle = slopeAngle;

					}

				}

			}
		}
	}

	public override void HorizontalCollision (ref Vector2 moveAmount)
	{
		base.HorizontalCollision (ref moveAmount);

		float rayLenght = (Mathf.Abs(moveAmount.x) < skinWidth) ? 2*skinWidth : Mathf.Abs (moveAmount.x) + skinWidth;

		Vector2 rayOrigin;
		RaycastHit2D hit;

		for (int i = 0; i < HorizontalRayCount; i++) {

			rayOrigin = (faceDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight ;
			rayOrigin += Vector2.up * (HorizontalRaySpacing * i);

			Debug.DrawRay(rayOrigin, Vector2.right * faceDir, Color.white);

			hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDir, rayLenght, cantWallJumpOnIt);

			if (hit) {
				collisions.left = false;
				collisions.right = false;

				if (hit.collider && i > HorizontalRayCount/2) {
					actions.HaveSomethingToGrab = true;
					actions.ObjectAbleToGrab = hit.collider.gameObject;
				} 
				else {
					if (actions.HaveSomethingToGrab) {
						//PlayerPrefsX.SetVector3 (m_isAbleToGrab.name, m_isAbleToGrab.transform.position);
						actions.ObjectAbleToGrab = null;
					}
					actions.HaveSomethingToGrab = false;
				}
			}

		}
	}



	public void Flip () {
		Transform sprite = transform.Find ("Sprite");
		faceDir *= -1; // if -1 -> 1 else 1 -> -1
		Vector3 newScale = new Vector3 (-1 * sprite.localScale.x, sprite.localScale.y, 1);
		sprite.localScale = newScale;	
	}



	public int NormalSpeed {
		get {
			return this.normalSpeed;
		}
		set {
			normalSpeed = value;
		}
	}

	public ActionsController Actions {
		get {
			return this.actions;
		}
	}

	public Transform OldObjectStuckedIn {
		get {
			return this.oldObjectStuckedIn;
		}
	}

	public bool IsPlayer {
		get {
			return this.isPlayer;
		}
	}

}