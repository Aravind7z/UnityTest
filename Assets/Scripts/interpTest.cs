using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class points{
	public Vector3 position;
	public float duration;
}
public class interpTest : MonoBehaviour {
	public List<points> posz;
	public float tm;
	public Vector3 startPosition;
	public Vector3 target,refz;
	public float timeToReachTarget;
	public bool reached;
	void Start()
	{
	//	startPosition = target = transform.position;

	}
	void Update() 
	{
		if(posz.Count > 0){
			target = posz [0].position;
			timeToReachTarget = posz [0].duration;
		if (transform.position != posz[0].position) {
			reached = false;
		} else {reached = true;
				posz.RemoveAt (0);
			}
		}
		if(refz != target && !reached){SetDestination(target,timeToReachTarget);refz = target;}
		tm += Time.deltaTime/timeToReachTarget;
		transform.position = Vector3.Lerp(startPosition, target, tm);
	}
	public void SetDestination(Vector3 destination, float time)
	{
		tm = 0;
		startPosition = transform.position;
		timeToReachTarget = time;
		target = destination; 
	}
}
