using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class TCPServerTemplate : MonoBehaviour {

	public TCPServer server;
	
	public string receiveStr;
	public string sendStr;

	// Use this for initialization
	void Start () {
		server.MessageReceived += messageReceive;
		server.eventClientConnectStatus += delegate(TCPServer.ClientStatus status, TcpClient client) {
			Debug.Log("eventClientConnectStatus: "+ status + " >> " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);
			//Debug.Log("eventClientConnectStatus: "+ status);
		};
		server.Setup();
	}

	void messageReceive(string str)
	{
		receiveStr = str;
		if(receiveStr.IndexOf(TCP.END_Code) > 0){
			receiveStr = str.Replace(TCP.END_Code, "");
		}

		Debug.Log("messageReceive: "+receiveStr);
	}
	
	void OnGUI () {
		GUILayout.BeginArea(new Rect(10,10,200,200));
		GUILayout.Label("TCP/IP SERVER: : "+server.GetClientCount());
		if( GUILayout.Button("ConnectToServer") ){
			server.Setup();
		}
		if( GUILayout.Button("Close") ){
			server.Close();
		}
		
		GUILayout.Space(20);
		GUILayout.Label("RECEIVE: " + receiveStr );

		sendStr = GUILayout.TextField(sendStr);
		if(GUILayout.Button("Send")){
			server.Send(sendStr);
		}
		GUILayout.EndArea();
	}
}
