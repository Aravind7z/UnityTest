using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class PObserved : NetworkBehaviour {
	[SyncVar] 
	public PlayerState serverPos;

	public PlayerState syncedPosz;
	[SyncVar]
	public Vector3 syncedPos;

	public float nextTime,interval;

	//void Update () {}
	[Command(channel=0)]public void CmdSendPos(PlayerState previous){
		serverPos = previous;
		syncedPosz = previous;
		syncedPos = previous.position;
	}
}