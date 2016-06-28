using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
public class Pinput : NetworkBehaviour {
	public PObserved observer;
	public PlayerState currentState;
	public Vector3 currentPosition,lastSentPos;
	public int speed = 1;
	public KeyCode[] arrowKeys;
	public float nextTime,interval,interpTm=0.250f;
	void Start () {
		if (isLocalPlayer) {
			arrowKeys = new KeyCode[]{ KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow };
		}
	}
	void FixedUpdate () {
		currentPosition = currentState.position;
		if (isLocalPlayer) {
			foreach (KeyCode arrowKey in arrowKeys) {
				if (!Input.GetKey (arrowKey))
					continue;
				Move (arrowKey, currentState);
			}
			if (Time.fixedTime > this.nextTime) {
				currentState.duration = interpTm;
				if(lastSentPos != currentState.position){
					observer.CmdSendPos (currentState);
				//Debug.Log ("beat");
					lastSentPos = currentState.position;
				}
				this.nextTime = Time.fixedTime + interval;
			}
		}
	}
//	void Update () {		
//	}
	public PlayerState Move(KeyCode input,PlayerState previous){
		int dx = 0;
		int dy = 0;
		if(input == KeyCode.UpArrow){
			dy = speed;
		}
		else if(input == KeyCode.DownArrow){
			dy = -speed;
		}
		if(input == KeyCode.RightArrow){
			dx = speed;
		}
		else if(input == KeyCode.LeftArrow){
			dx = -speed;
		}
//		currentState.age = 1 + previous.age;
		currentState.position = new Vector3 ((dx + previous.position.x), (dy + previous.position.y), 0);
		return currentState;
	}
}
