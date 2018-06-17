using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CameraLink))]
public class SimpleActions : MonoBehaviour {
	
	/*[Header ("Spirit")]
	public Transform m_SpiritPrefab; 							// The prefab of the spirit
	public Vector3 DistanceSpirit; 							// It's where the spirit will spawn when you'll call it
*/
	[Header ("Looking Above/Below")]
	[Range(2, 6)] public float m_cameraVerticalOffSetModifier = 4;

	[HideInInspector] public Transform m_Spirit; 								// Transform keep a link to the spirit when it's Instantiate
	//private Player m_spiritPlayer;

	private float nextMoveTime;
	private float percentBetweenStartEnd;
	[HideInInspector] public bool spiritInvocated = false;

	[HideInInspector] public bool hasJustChanged = false;

	private CameraLink cameraLink;

	void Awake () {
		cameraLink = GetComponent<CameraLink>();
	}

	// Use this for initialization
	void Start () {
		
	}
	/*
	// Update is called once per frame
	void Update () {
		if(spiritInvocated) {
			Vector2 calculatedDplmt = CalculateSpiritAppearDplmt ();
			if (calculatedDplmt != Vector2.zero) {
				calculatedDplmt.x *= Mathf.Sign(transform.FindChild("Sprite").localScale.x);
				spiritPlayer.controller.Move (calculatedDplmt);
				spiritPlayer.animator.SetBool ("Appearing", true);
			} 
			else {
				spiritPlayer.animator.SetBool ("Appearing", false);
			}
		}
	}*/

	public void LookAhead () {
		cameraLink.followScript.verticalOffSet = cameraLink.followScript.OriginalVerticalOffSet;
		//m_player.animator.SetBool ("LookAbove", false);
		//m_player.animator.SetBool ("LookBelow", false);
	}

	public void LookAbove () {
		cameraLink.followScript.verticalOffSet = cameraLink.followScript.OriginalVerticalOffSet + m_cameraVerticalOffSetModifier;
		//m_player.animator.SetBool ("LookBelow", false);
		//m_player.animator.SetBool ("LookAbove", true);
	}

	public void LookBelow () {
		cameraLink.followScript.verticalOffSet = cameraLink.followScript.OriginalVerticalOffSet - m_cameraVerticalOffSetModifier;
		//m_player.animator.SetBool ("LookAbove", false);
		//m_player.animator.SetBool ("LookBelow", true);
	}
		
	/*
	public void CarryObject() { // porter

	}

	public void ThrowObject() { // lancer

	}


	public void InvokeSpirit() {

		//			On instancie	le spirit du prefab		à notre position	 avec notre rotation
		m_Spirit = (Transform) Instantiate(m_SpiritPrefab, transform.position, m_SpiritPrefab.rotation);
		m_Spirit.name = "Watson";

		spiritPlayer = m_Spirit.GetComponent <Player> (); 								// On recupère le script Player du spirit

		if (!m_player.controller.faceRight) {
			spiritPlayer.controller.Flip ();
		}

		spiritInvocated = true;

		ChangeBodyControl ();

		spiritPlayer.actions.simple.m_Spirit = transform; 								// Le "spirit" du spirit c'est le player
		spiritPlayer.actions.simple.spiritPlayer = transform.GetComponent<Player>();	// Le "spirit" du spirit c'est le player
	}


	public void DesinvokeSpirit() {
		if (m_Spirit != null) {
			m_Spirit.FindChild ("Sprite").GetComponent<GhostSprites> ().trailSize = 0;
			m_spiritPlayer.animator.SetBool ("Disappear", true);
			Destroy (m_Spirit.gameObject, 11*Time.deltaTime);
			spiritInvocated = false;
			percentBetweenStartEnd = 0f;
		}
	}


	public void ChangeBodyControl() {
		if (spiritPlayer != null) {			
			
			m_player.controllingThisBody = !m_player.controllingThisBody; 				// On ne controle plus ce corps
			m_player.animator.transform.GetComponent<SpriteRenderer>().sortingOrder--;
			spiritPlayer.controllingThisBody = !spiritPlayer.controllingThisBody; 		// On controle le spirit
			spiritPlayer.animator.transform.GetComponent<SpriteRenderer>().sortingOrder--;

			m_player.camera.playerTarget = spiritPlayer;						// On récupère la caméra qui nous suit pour lui dire de suivre l'autre joueur

		}
	}


	public Vector2 CalculateSpiritAppearDplmt() {
		
		if(Time.time < nextMoveTime || percentBetweenStartEnd >= 1) {
			return Vector3.zero;
		}
		float distanceBetweenWaypoints = Vector3.Distance(transform.position, transform.position+DistanceSpirit);
		percentBetweenStartEnd += Time.deltaTime * spiritPlayer.stats.speed.moveSpeed / distanceBetweenWaypoints;
		percentBetweenStartEnd = Mathf.Clamp01(percentBetweenStartEnd);

		Vector3 newPos = Vector3.Lerp(transform.position, transform.position+DistanceSpirit, percentBetweenStartEnd);

		nextMoveTime = Time.time;

		return (Vector2) (newPos - transform.position);

	}



	public Player spiritPlayer {
		get {
			return m_spiritPlayer;
		}
		set {
			m_spiritPlayer = value;
		}
	}

	*/
}
