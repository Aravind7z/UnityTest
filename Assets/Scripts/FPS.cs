using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FPS : MonoBehaviour {
	float deltaTime = 0.0f;
	public Text textz;
	public Slider sld;
	public Button buttonz;
	void Awake () {
		Application.targetFrameRate = 60;}
	void Update(){
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		textz.text = text;
	}
	public void setfps(){
		int frame = (int)sld.value;
		//Debug.Log (frame);
	//	Application.targetFrameRate = frame;
	}	
}