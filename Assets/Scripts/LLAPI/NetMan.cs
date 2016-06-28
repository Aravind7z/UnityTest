using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
public class NetMan : MonoBehaviour {
	public int myReliableChannelId;
	public int maxConnections = 10;
	public int socketId,hostId;
	public int socketPort = 7733;
	public string ipAddress = "117.213.115.152";

	public int connectionId;
	// Use this for initialization
	void Start(){
		NetworkTransport.Init ();
	}
	public void StartServer () {
		
		ConnectionConfig config = new ConnectionConfig();
		myReliableChannelId = config.AddChannel(QosType.Reliable);
		HostTopology topology = new HostTopology(config, maxConnections);

		socketId = NetworkTransport.AddHost(topology, socketPort);
		Debug.Log("Socket Open. SocketId is: " + socketId);
	}

	public void Connect() {
		
		ConnectionConfig config = new ConnectionConfig();
		myReliableChannelId = config.AddChannel(QosType.Reliable);
		HostTopology topology = new HostTopology(config, maxConnections);

		socketId = NetworkTransport.AddHost(topology);
		byte error;
		connectionId = NetworkTransport.Connect(socketId, ipAddress, socketPort, 0, out error);
		Debug.Log("Connected to server. ConnectionId: " + connectionId);
		//if (error != kOk ) {
		//}
	}
	public void SendSocketMessage() {
		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream(buffer);
		BinaryFormatter formatter = new BinaryFormatter();
		formatter.Serialize(stream, "HelloServer");

		int bufferSize = 1024;

		NetworkTransport.Send(socketId, connectionId, myReliableChannelId, buffer, bufferSize, out error);
		LogNetworkError ( error );
	}
	void LogNetworkError(byte error){
		if( error != (byte)NetworkError.Ok){
			NetworkError nerror = (NetworkError)error;
			Debug.Log ("Error: " + nerror.ToString ());
		}
	}
	void Update () {
		int recsocketId;
		int recConnectionId;
		int recChannelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;

		NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recsocketId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recNetworkEvent) {
		case NetworkEventType.Nothing:
			Debug.Log("nothing");
			break;
		case NetworkEventType.ConnectEvent:
			Debug.Log("incoming connection event received");
			break;
		case NetworkEventType.DataEvent:
			Stream stream = new MemoryStream(recBuffer);
			BinaryFormatter formatter = new BinaryFormatter();
			string message = formatter.Deserialize(stream) as string;
			Debug.Log("incoming message event received: " + message);
			break;
		case NetworkEventType.DisconnectEvent:
			Debug.Log("remote client event disconnected");
			break;
		}
	}
}
