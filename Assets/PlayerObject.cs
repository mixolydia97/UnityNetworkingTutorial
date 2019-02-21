using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObject : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		//is this actually my own local PlayerObject?
		if( isLocalPlayer == false )
		{
			//no, it's someone else's
			return;
		}

		//since the Player object is invisible and not part of the world
		//need to instantiate something visibile
		Debug.Log("PlayerObject::Start -- Spawning my unit");

		//Instantiate only creates an object on the LOCAL COMPUTER
		//Even if that object has a NetworkIdentity it still will not
		//exist on the network (and not on any other client) unless
		//NetowkrServer.Spawn() is called on this object.
		
		//Instantiate(PlayerUnitPrefab);

		//politely command server to spawn our unit
		CmdSpawnMyUnit();
	}
	
	public GameObject PlayerUnitPrefab;

	[SyncVar]
	public string PlayerName = "Anonymous";

	// Update is called once per frame
	void Update () {
		//remember, update runs on everyone's computer whether they own this
		//particular player object
		if( isLocalPlayer == false )
		{
			return;
		} 

		if( Input.GetKeyDown(KeyCode.S) )
		{
			CmdSpawnMyUnit();
		}

		if( Input.GetKeyDown(KeyCode.N) )
		{
			string n = "ButtFace" + Random.Range(1, 100);
			Debug.Log("Sending name change request: " + n);
			CmdChangePlayerName(n);
		}
	}


	////// COMMANDS
	//Commands are special function that only get executed on the server

	GameObject myPlayerUnit;

	[Command]
	void CmdSpawnMyUnit()
	{
		//inside here, we're guaranteed to be on the server right now
		GameObject go = Instantiate(PlayerUnitPrefab);
		myPlayerUnit = go;

		//one way to get authority over PlayerUnit. The other way is to do
		//SpawnWithClientAuthority
		//go.GetComponent<NetworkIdentity>().AssignClientAuthority( connectionToClient );

		//Now we made the object on the server, propagate it to all clients
		NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
	}

	[Command]
	void CmdMoveUnitUp()
	{
		if( myPlayerUnit == null )
		{
			return;
		}

		myPlayerUnit.transform.Translate( 0, 1, 0 );
	}

	[Command]
	void CmdChangePlayerName(string n)
	{
		Debug.Log("CmdChangePlayerName to " + n);
		PlayerName = n;

		//Maybe check if the name doesn't have bad words in it??? OMG!?!?!
		//if the name has a bad word, do we want to ignore the cmd or do we
		//want to run the RPC with the argument being the player's old name.
		//if we had the change happen immediately in the client, the latter
		//option would correct the name for the player who changed it.

		//Now propagate the name change
		//could use sync var? Right now we'll do a RPC
		// RpcChangePlayerName(PlayerName);
	}

	////// RPCs
	//special functions that only get executed on the clients

	[ClientRpc]
	void RpcChangePlayerName(string n)
	{
		Debug.Log("RpcChangePlayerName for some Player to " + n);
		PlayerName = n;
	}
}

