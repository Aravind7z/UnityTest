using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class Ping : NetworkBehaviour{
	public NetworkClient nClient;
	public int latency;
	public Text latencyText;
	public override void OnStartLocalPlayer (){
		nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
		latencyText = GameObject.Find("Latency Text").GetComponent<Text>();}
	void Update (){
		ShowLatency();}
	void ShowLatency (){
		if(isLocalPlayer)
		{
			latency = nClient.GetRTT();
			latencyText.text = latency.ToString();
		}
	}
}