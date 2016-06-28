using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
[System.Serializable]
public class refPstates{
//	public int age;
	public Vector3 position;
	public float duration;
	public int timeStamp;
	public bool reached;
	public refPstates(Vector3 pos,float dur,int tm,bool reach){
		position = pos;
		duration = dur;
		timeStamp = tm;
		reached = reach;
	}
}
public class PInterpolate : NetworkBehaviour{
	public Pinput localPlayer;
	public PObserved server;
	public refPstates refStat;
	public List<refPstates> refServerStat;
	public List<Vector3> posRecived;
	Vector3 start,end,refz;
	public float tm,duration;
	public bool reached;
	void FixedUpdate () {
		if (isServer) {
			interpolate (server.serverPos.position);
		//	refStat.duration = 0.0f;
		} else if (isLocalPlayer){
		//	refStat.duration = 0.0f;
			interpolate (localPlayer.currentPosition);
		} else {
			interpolate (server.serverPos.position);
		}
	}
/*	void LocalSelf(){}
	void Server(){}
	void Local(){}*/
	void interpolate(Vector3 position){
		if (refStat.position != position){
			refStat.position = position;
	//		if(reached){
				SetDestination (refStat.position, duration);//}
		}
	//	if (transform.position != end) {
	//		reached = false;
	//	} else {
	//		reached = true;
	//	}

		tm += Time.deltaTime / duration;
		transform.position = Vector3.Lerp (start,end, tm);

	}
	public void SetDestination(Vector3 destination, float time){
	//	Debug.Log ("ting");
		tm = 0;
		start = transform.position;
		duration = time;
		end = destination;
	}
}