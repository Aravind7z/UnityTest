using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

struct CubeState {
	public int moveNum;
	public int x;
	public int y;
}

[NetworkSettings(channel=2)] public class CubePlayer : NetworkBehaviour {
	[SyncVar] Color color;
	[SyncVar(hook="OnServerStateChanged")] CubeState serverState;
	Queue<KeyCode> pendingMoves;
	CubeState predictedState;

	void Awake () {
		InitState();
	}
	
	void Start () {
		if (isLocalPlayer) {
			pendingMoves = new Queue<KeyCode>();
			UpdatePredictedState();
		}
		SyncState(true);
		SyncColor();
	}
	
	void Update () {
		if (isLocalPlayer) {
			KeyCode[] arrowKeys = {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow};
			foreach (KeyCode arrowKey in arrowKeys) {
				if (!Input.GetKeyDown(arrowKey)) continue;
				pendingMoves.Enqueue(arrowKey);
				UpdatePredictedState();
				CmdMove(arrowKey);
			}
		}
		SyncState(false);
	}

	[Command(channel=0)] void CmdMove(KeyCode arrowKey) {
		serverState = Move(serverState, arrowKey);
	}
	
	[Server] void InitState () {
		Color[] colors = {Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow};
		color = colors[Random.Range(0, colors.Length)];
		serverState = new CubeState {
			moveNum = 0,
			x = Random.Range(0, maxX),
			y = Random.Range(0, maxY)
		};
	}
	
	CubeState Move(CubeState previous, KeyCode arrowKey) {
		int dx = 0;
		int dy = 0;
		switch (arrowKey) {
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
		int newX = dx + previous.x;
		int newY = dy + previous.y;
		return new CubeState {
			moveNum = 1 + previous.moveNum,
			x = newX < 0 ? 0 : newX >= maxX ? (maxX - 1) : newX,
			y = newY < 0 ? 0 : newY >= maxY ? (maxY - 1) : newY
		};
	}
	
	void OnServerStateChanged (CubeState newState) {
		serverState = newState;
		if (pendingMoves != null) {
			while (pendingMoves.Count > (predictedState.moveNum - serverState.moveNum)) {
				pendingMoves.Dequeue();
			}
			UpdatePredictedState();
		}
	}
	
	void SyncColor () {
		GetComponent<Renderer>().material.color = (isLocalPlayer ? Color.white : Color.grey) * color;
	}
	
	void SyncState (bool init) {
		CubeState stateToRender = isLocalPlayer ? predictedState : serverState;
		Vector3 target = spacing * (stateToRender.x * Vector3.right + stateToRender.y * Vector3.up);
		transform.position = init ? target : Vector3.Lerp(transform.position, target, easing);
	}
	
	void UpdatePredictedState () {
		predictedState = serverState;
		foreach (KeyCode arrowKey in pendingMoves) {
			predictedState = Move(predictedState, arrowKey);
		}
	}
	
	int maxX = 5;
	int maxY = 5;
	float easing = 0.1f;
	float spacing = 1.0f;
}
