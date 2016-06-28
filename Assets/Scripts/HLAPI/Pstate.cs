using UnityEngine;
using System.Collections;

public struct PlayerState {
//	public int age;
	public Vector3 position;
	public float duration;
	public int timeStamp;
	public PlayerState Set(int agez,Vector3 posz,int timeStampz){
		return new PlayerState {
		//	age = agez,
			position = posz,
			timeStamp = timeStampz
		};
	}
}