using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine.UI;
//using System.Net;
public class NetworkManagerL : MonoBehaviour{
	//public string ipAddress = "";//127.0.0.1 = localhost 
	//port=7733
	public int reliableChannelId,unReliableChannelId,clientSocketId,serverSocketId,maxConnections = 10,clientConnectionId,serverConnectionId;
	public List<int> clientConnectionIds;
	public bool clientInitialized = false,serverInitialized = false;
	public Text display,message,address,portField;
	ConnectionConfig config;
	HostTopology topology;
	void Start(){
		Debug.Log (sizeof(char));
		NetworkTransport.Init();
		config = new ConnectionConfig();
		reliableChannelId = config.AddChannel(QosType.Reliable);
		topology = new HostTopology(config, maxConnections);
		AddToDisplay ("this machines local ip-" + Network.player.ipAddress);}
	public void InitializeServer(){
		serverSocketId = NetworkTransport.AddHost(topology,int.Parse(portField.text));
		if(serverSocketId < 0){ AddToDisplay ("Server socket creation failed!");} 
		else {AddToDisplay ("Socket Open for Server. SocketId is: " + serverSocketId);serverInitialized = true;}
	}
	public void InitializeClient(){
		clientSocketId = NetworkTransport.AddHost(topology);
		if(clientSocketId < 0){ AddToDisplay ("Client socket creation failed!");} 
		else {AddToDisplay ("Socket Open for Client. SocketId is: " + clientSocketId);clientInitialized = true;}
	}
	public void ClientConnect(){
		byte error;
		serverConnectionId = NetworkTransport.Connect( clientSocketId , address.text , int.Parse(portField.text) , 0 , out error);
		AddToDisplay("server's connectionID:" + serverConnectionId);
		LogNetworkError(error);}
	public void SendMsg(){
		SendData(message.text);
	}
	public void SendData(string msg){
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		//string test = message.text;
		//Type Tz = test.GetType();
		//Debug.Log(Size.Of(message.text));
		formatter.Serialize(stream ,msg);
		Debug.Log ((int)stream.Position);
		if (clientInitialized){
			NetworkTransport.Send(clientSocketId, serverConnectionId, reliableChannelId, buffer, (int)stream.Position, out error);
			LogNetworkError(error);}
		if (serverInitialized) {
			//for(int no = 0; no < clientConnectionIds.Count; no++){
				foreach(int no in clientConnectionIds){
				NetworkTransport.Send (serverSocketId, clientConnectionIds[no-1], reliableChannelId, buffer, (int)stream.Position, out error);
				LogNetworkError (error);
			}
		}
	}
	void FixedUpdate(){
		int recHostId; 
		int connectionId;
		int channelId;
		int dataSize;
		byte[] buffer = new byte[1024];
		byte error;
		NetworkEventType networkEvent = NetworkEventType.DataEvent;
		if(serverInitialized || clientInitialized){
			do{
				networkEvent = NetworkTransport.Receive (out recHostId, out connectionId, out channelId, buffer, 1024, out dataSize, out error);
				switch(networkEvent){
				case NetworkEventType.Nothing:
					break;
				case NetworkEventType.ConnectEvent:
					if(serverInitialized){
						AddToDisplay ("Server: Player " + connectionId.ToString () + " connected!");
					//	clientConnectionId = connectionId;
						clientConnectionIds.Add(connectionId);
						AddToDisplay ("client's connectionID - " + clientConnectionId);}
					if(clientInitialized){
						AddToDisplay ("Client: Client connected to " + connectionId.ToString () + "!");}
					break;
				case NetworkEventType.DataEvent:
					if(serverInitialized){
						Stream stream = new MemoryStream (buffer);
						BinaryFormatter f = new BinaryFormatter ();
						string msg = f.Deserialize (stream).ToString ();
						AddToDisplay ("Server: Received Data from No" + connectionId.ToString () + " Message: " + msg);
						SendData(msg);
					}
					if(clientInitialized){
						Stream stream = new MemoryStream (buffer);
						BinaryFormatter f = new BinaryFormatter ();
						string msg = f.Deserialize (stream).ToString ();
						AddToDisplay ("Client: Received Data from No" + connectionId.ToString () + " Message: " + msg);}
					break;
				case NetworkEventType.DisconnectEvent:
					if(clientInitialized){
						AddToDisplay ("Client: Disconnected from server!");}
					if(serverInitialized){
						clientConnectionIds.Remove(connectionId);
						AddToDisplay ("Server: Received disconnect from" + connectionId.ToString ());}
					break;}
			}while(networkEvent != NetworkEventType.Nothing);
		}
	}
	public void AddToDisplay(string msg){
		display.text += msg + "\n";
		Debug.Log (msg);}
	void LogNetworkError(byte error){
		if(error != (byte)NetworkError.Ok){
			NetworkError nerror = (NetworkError)error;
			AddToDisplay ("Error: " + nerror.ToString ());}
	}
	public void Clean(){
		display.text = "";}
/*	public string GetIp(){
	string strHostName = "";
	IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
	IPAddress[] addr = ipEntry.AddressList;
	return addr [addr.Length - 1].ToString ();
}*/
}
public static class Size{
	public static int Of(int x){
		return sizeof(int);}
	public static int Of(long x){
		return sizeof(long);}
	public static int Of( string x){
		return x.Length * sizeof(char);}
}