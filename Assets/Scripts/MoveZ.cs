using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
[System.Serializable]
public class PosSnapShotZ{
	public Vector3 position;
	public int age;
}

struct PosSnapShot{
	public Vector3 position;
	public int age;
}
[NetworkSettings(channel=1,sendInterval=0.2f)]
public class MoveZ : NetworkBehaviour {
	public PosSnapShotZ currentPos,serverPos;
	PosSnapShot currentPosz;
	[SyncVar(hook="OnServerStateChanged")] 
	PosSnapShot serverPosz;
	public int lastAge;
	public List<PosSnapShotZ> recordedPos;
	List<PosSnapShot> recordedPosz;
	public KeyCode[] arrowKeys;
	PosSnapShot tempZ;
	public float step,stepTimer,stepCooldown=0.5f,stepRelease;
	void Start () {
		if (isLocalPlayer) {
			arrowKeys = new KeyCode[]{ KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow };
		}
	}
	void FixedUpdate () {
		SyncMove ();
	}
	void Update () {
		currentPos.position = currentPosz.position;
		serverPos.position = serverPosz.position;
		serverPos.age = serverPosz.age;
		if(stepTimer > 0){
			stepTimer -= Time.deltaTime;}
		if (stepTimer < 0) {
			stepTimer = 0;
		}
		if (isLocalPlayer) {
			foreach (KeyCode arrowKey in arrowKeys) {
				if (!Input.GetKey(arrowKey)) continue;
				if(stepTimer == 0){
				Vector3 temp = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
				tempZ.position = temp;
				tempZ.age++;
				Move (arrowKey,tempZ);
				CmdSendPos (arrowKey,tempZ);
					stepTimer = stepCooldown;
				}
			}
		}

	}
	[Command(channel=0)] void CmdSendPos(KeyCode arrow,PosSnapShot previous){
		serverPosz = Move(arrow, previous);
	}
	void OnServerStateChanged(PosSnapShot serverPos){
		if (!isServer && !isLocalPlayer) {
			PosSnapShotZ temp = new PosSnapShotZ();
			temp.position = serverPos.position;
			temp.age = serverPos.age;
			if (!((IList)recordedPos).Contains (temp)) {
				if(temp.age > lastAge){
				recordedPos.Add (temp);
				lastAge = temp.age;}
			}
		}
	}
	PosSnapShot Move(KeyCode arrow,PosSnapShot previous){
		int dx = 0;
		int dy = 0;
		switch (arrow) {
		case KeyCode.UpArrow:
			dy = 1;
			break;
		case KeyCode.DownArrow:
			dy = -1;
			break;
		case KeyCode.RightArrow:
			dx = 1;
			break;
		case KeyCode.LeftArrow:
			dx = -1;
			break;
		}
		currentPosz.age = 1 + previous.age;
		currentPosz.position = new Vector3 (dx + previous.position.x, dy + previous.position.y, 0);
		return currentPosz;
	}
	void SyncMove(){
		if (isServer) {
			transform.position = Vector3.Lerp (transform.position, currentPosz.position, 0.1f);
		//	transform.position = Vector3.MoveTowards(transform.position, currentPosz.position, 1f);
			return;
		}
		else if (isLocalPlayer) {
			transform.position = Vector3.Lerp (transform.position, currentPosz.position, 0.1f);
		//	transform.position = Vector3.MoveTowards(transform.position, currentPosz.position, 1f);
			return;
		} else {
			interpolate ();
		//	transform.position = Vector3.MoveTowards(transform.position, currentPos, 5);
			return;
		}
	}
	void interpolate(){
		if (recordedPos.Count > 0) {
			transform.position = Vector3.MoveTowards (transform.position, recordedPos [0].position, 1f * recordedPos.Count * Time.deltaTime);
	//		transform.position = Vector3.Lerp (transform.position, recordedPos [0].position, 0.1f * recordedPos.Count);
			if (Vector3.Distance (transform.position, recordedPos [0].position) < 0.05f) {
				recordedPos.RemoveAt (0);
			}
		}
	}
}
