using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// THIS SCRIPT HAS TO BE PUT ON A PLAYER
/// IT IS A LINK TO GET THE CAMERA WHO IS FOCUSING ON HIM
/// 
/// THIS SCRIPT IS ALREADY REQUESTED BY PLAYERS SCRIPTS
/// NO NEED TO PUT THIS SCRIPT ANYWHERE
/// </summary>
public class CameraLink : MonoBehaviour {

	public CameraFollow followScript;
	// Use this for initialization
	void Awake() {
		followScript = followScript ? followScript : GameObject.Find("Main Camera").transform.GetComponent<CameraFollow>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
