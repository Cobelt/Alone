using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Controller2D target;

	private float originalVerticalOffSet;
	public float verticalOffSet, lookAheadDistx;
	public float verticalSmoothTime, lookSmoothTimeX;

	public Vector2 focusAreaSize;
	FocusArea focusArea;

	private float currentLookAheadX, targetLookAheadX, lookAheadDirX;
	private float smoothVelocityX, smoothVelocityY;
	private bool lookAheadStopped;

	void Awake() {
		target = target ? target : GameObject.Find("Player").GetComponent<Controller2D>(); // target || GameObj doesn't work
		originalVerticalOffSet = verticalOffSet;
	}
	void Start() {
		focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
	}

	void LateUpdate () {

		focusArea.Update (target.collider.bounds);

		Vector2 focusPos = focusArea.centre + Vector2.up * verticalOffSet;

		if(focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if(Mathf.Sign(target.Input.x) == Mathf.Sign (focusArea.velocity.x) && target.Input.x != 0){
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistx;
			}
			else {
				if(!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistx - currentLookAheadX)/4f;
				}
			}
		}


		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothVelocityX, lookSmoothTimeX);

		focusPos.y = Mathf.SmoothDamp(transform.position.y, focusPos.y, ref smoothVelocityY, verticalSmoothTime);
		focusPos += Vector2.right * currentLookAheadX;

		Vector3 newPos = (Vector3) focusPos + Vector3.forward *-10;
		transform.position = newPos;

		transform.GetComponent<CameraShake>().UpdateShake ();
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (0, 0, 1, .5f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);
	}


	public float OriginalVerticalOffSet {
		get {
			return originalVerticalOffSet;
		}
		set {
			originalVerticalOffSet = value;
		}
	}
		
	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left, right;
		float bottom, top;


		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left + right)/2, (top + bottom)/2);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;

			if(targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			}

			else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}

			left += shiftX;
			right += shiftX;



			float shiftY = 0;

			if(targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			}

			else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}

			bottom += shiftY;
			top += shiftY;



			centre = new Vector2((left + right)/2, (top + bottom)/2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}
