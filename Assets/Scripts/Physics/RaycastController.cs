using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	[Header ("Collisions detections parameters")]
	public LayerMask whatIsGround;
	public float skinWidth = .015f;
	public float distanceBetweenRays = .25f;

	private int horizontalRayCount; 		// Number of ray on x axis
	private int verticalRayCount;		// Number of ray on y axis

	private float horizontalRaySpacing; 	// Ray spacing x axis
	private float verticalRaySpacing;   	// Ray spacing y axis

	public BoxCollider2D collider; // Reference to the BoxCollider2D
	protected RaycastOrigins raycastOrigins; // Reference to the 4 corners which will be the origins of ray casts


	// Use this for initialization
	protected virtual void Awake () {
		collider = GetComponent<BoxCollider2D>(); // Give to collider the reference on the BoxCollider2D attached to the gameobject
	}

	protected virtual void Start () {
		CalculateRaySpacing(); 
	}

	protected void UpdateRaycastOrigins() { // Set up the bounds origins
		
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2); 

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

	}


	public void CalculateRaySpacing(){ // Calculate the number of rays and the ray spacing
		
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.RoundToInt(bounds.size.y / distanceBetweenRays);
		verticalRayCount = Mathf.RoundToInt(bounds.size.x / distanceBetweenRays);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount -1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount -1);

	}



	public int HorizontalRayCount {
		get {
			return horizontalRayCount;
		}
		set {
			horizontalRayCount = value;
		}
	}

	public int VerticalRayCount {
		get {
			return verticalRayCount;
		}
		set {
			verticalRayCount = value;
		}
	}

	public float HorizontalRaySpacing {
		get {
			return horizontalRaySpacing;
		}
		set {
			horizontalRaySpacing = value;
		}
	}

	public float VerticalRaySpacing {
		get {
			return verticalRaySpacing;
		}
		set {
			verticalRaySpacing = value;
		}
	}


	public struct RaycastOrigins { // The 4 corners of the box collider
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

}
