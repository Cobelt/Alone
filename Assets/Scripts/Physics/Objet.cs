using UnityEngine;
using System.Collections;


[RequireComponent (typeof (GravityEffects))]
public class Objet : RaycastController {

	protected Vector2 input = Vector2.zero;
	protected Vector2 velocity = Vector2.zero;
	[HideInInspector] public float faceDir;

	public CollisionInfo collisions = new CollisionInfo ();


	protected float maxClimbAngle = 80;
	protected float maxDescendAngle = 85;

	protected Vector3 oldAngle;

	[HideInInspector] public Vector3 lastPosGrounded;


	private bool onPlatform;
	[HideInInspector] public bool falling;

	[HideInInspector] public GravityEffects gravityEffects;

	public float lengthRays;

	protected override void Awake() {
		base.Awake();
		lastPosGrounded = transform.position;
		gravityEffects = GetComponent<GravityEffects> ();

		faceDir = 1;
	}

	protected virtual void Update () {

		gravityEffects.ApplyGravity(ref velocity.y);
		Move(velocity * Time.deltaTime);
	}

	public Vector2 Move(Vector2 moveAmount, bool standingOnPlatform = false) { return Move(moveAmount, Vector2.zero, standingOnPlatform); }

	public virtual Vector2 Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {

		onPlatform = standingOnPlatform;

		UpdateRaycastOrigins();

		collisions.Reset();

		collisions.moveAmountOld = moveAmount;


		if(moveAmount.x != 0) {
			faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		if(moveAmount.y < 0) {
			falling = true;
			if(moveAmount.x != 0) {
				DescendSlope(ref moveAmount);
			}
		}

		HorizontalCollision (ref moveAmount);

		if(moveAmount.y != 0)
			VerticalCollision (ref moveAmount, standingOnPlatform);


		collisions.moveAmountNew = moveAmount;
		transform.Translate (moveAmount);

		if(onPlatform) {
			collisions.below = true;
		}

		if (collisions.below) {
			lastPosGrounded = transform.position;
		} 
		else if (isOut ()) {
			transform.position = lastPosGrounded;
		}

		return moveAmount;
	}


	public bool isOut () {
		bool isOut = false;
		Physics2D.queriesStartInColliders = false;

		//Horizontal
		Debug.DrawRay(transform.position, Vector2.right, Color.red);
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, lengthRays /4, LayerMask.GetMask("KillingWalls"));
		if (hit)
			isOut = true;

		Debug.DrawRay(transform.position, Vector2.left, Color.red);
			hit = Physics2D.Raycast (transform.position, Vector2.left, lengthRays / 4, LayerMask.GetMask("KillingWalls"));
		if (hit)
			isOut = true;
	
		//Vertical
		Debug.DrawRay(transform.position, Vector2.up, Color.red);
			hit = Physics2D.Raycast (transform.position, Vector2.up, lengthRays, LayerMask.GetMask("KillingWalls"));
		if (hit)
			isOut = true;

		Debug.DrawRay(transform.position, Vector2.down, Color.red);
			hit = Physics2D.Raycast (transform.position, Vector2.down, lengthRays, LayerMask.GetMask("KillingWalls"));
		if (hit)
			isOut = true;
					
		return isOut;
	}
		

	public virtual void HorizontalCollision(ref Vector2 moveAmount) {

		float rayLenght = (Mathf.Abs(moveAmount.x) < skinWidth) ? 2*skinWidth : Mathf.Abs (moveAmount.x) + skinWidth;

		Vector2 rayOrigin;
		RaycastHit2D hit;

		for (int i = 0; i < HorizontalRayCount; i++) {

			rayOrigin = (faceDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight ;
			rayOrigin += Vector2.up * (HorizontalRaySpacing * i);

			Debug.DrawRay(rayOrigin, Vector2.right * faceDir, Color.white);

			hit = Physics2D.Raycast(rayOrigin, Vector2.right * faceDir, rayLenght, whatIsGround);

			if (hit) {

				if (hit.distance == 0 || hit.collider.tag == "Through") {
					continue;
				}

				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);


				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
					
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;

						oldAngle = new Vector3 (transform.parent.rotation.x, transform.parent.rotation.y, transform.parent.rotation.z);
						transform.parent.Rotate (new Vector3 (transform.parent.rotation.x, transform.parent.rotation.y, transform.parent.rotation.z + slopeAngle));

					} 
						
					float distanceToSlopeStart = 0;

					if (slopeAngle != collisions.slopeAngleOld) {
					
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * faceDir;
					}

					ClimbSlope (ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * faceDir;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					
					moveAmount.x = (hit.distance - skinWidth) * faceDir;
					rayLenght = hit.distance;

					if (collisions.climbingSlope) {
						
						moveAmount.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x);
					}

					collisions.left = faceDir == -1;
					collisions.right = faceDir == 1;

				}
			}
		}
	}


	public virtual void VerticalCollision(ref Vector2 moveAmount, bool standingOnPlatform) {
		
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;


		for (int i = 0; i < VerticalRayCount; i++) {
			
			
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (VerticalRaySpacing * i + moveAmount.x);

			Physics2D.queriesStartInColliders = false;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, whatIsGround);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.white);

			if(hit) {

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

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if(collisions.climbingSlope) {
					
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

			}
		}

		if(collisions.climbingSlope) {
			
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((faceDir == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;

			Physics2D.queriesStartInColliders = false;
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

	protected void ClimbSlope(ref Vector2 moveAmount, float slopeAngle) {
		float moveDistance = Mathf.Abs(moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if(moveAmount.y <= climbmoveAmountY){
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	protected void DescendSlope(ref Vector2 moveAmount){
		Vector2 rayOrigin = (faceDir == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

		Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, whatIsGround);

		if(hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle < maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == faceDir) {
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x)) {
						float moveDistance = Mathf.Abs(moveAmount.x);
						float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
						moveAmount.y -= descendmoveAmountY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	protected void ResetFallingThroughPlatform () {
		collisions.fallingThroughPlatform = false;
	}
		

	public Vector2 Input {
		get {
			return input;
		}
		set {
			input = value;
		}
	}
	public Vector2 Velocity {
		get {
			return velocity;
		}
		set {
			velocity = value;
		}
	}

	public CollisionInfo Collisions {
		get {
			return collisions;
		}
		set {
			collisions = value;
		}
	}

	[System.Serializable]
	public class CollisionInfo {
		public bool above, below;
		public bool left, right;

		[HideInInspector] public bool climbingSlope, descendingSlope;
		[HideInInspector] public float slopeAngle, slopeAngleOld;

		[HideInInspector] public Vector2 moveAmountOld, moveAmountNew;
		[HideInInspector] public bool fallingThroughPlatform;

		public void Reset(){
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}		

}
