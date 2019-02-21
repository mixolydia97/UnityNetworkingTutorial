using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//a PlayerUnit is a unit controlled by a player
public class PlayerUnit : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// This function runs on all PlayerUnits, not just the ones I own
		// and it isn't networked
		if( hasAuthority == false )
		{
			return;
		}

		if( Input.GetKeyDown(KeyCode.Space) )
		{
			this.transform.Translate( 0, 1, 0 );
		}

		if( Input.GetKeyDown(KeyCode.D) )
		{
			Destroy(gameObject);
		}
	}
}
