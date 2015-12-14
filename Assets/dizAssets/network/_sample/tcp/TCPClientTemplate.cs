using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TCPClient))]
public class TCPClientTemplate : MonoBehaviour {
	
	public TCPClient client;
	
	private string receiveStr = "";
	private string sendStr = "";

	private string connectStatus = "";
	
	// Use this for initialization
	void Start () {
		//client.MessageReceived += new MessageReceivedHandler( messageReceive );
		client.MessageReceivedQueue += messageReceive;
		client.eventConnectStatus += delegate(TCPClient.Status status) {
			Debug.Log("eventConnectStatus: "+ status);
			connectStatus = status.ToString();
		};
		client.ConnectToServer();
	}

	private void messageReceive(string str)
	{
		receiveStr = str;
		if(receiveStr.IndexOf(TCP.END_Code) > 0){
			receiveStr = receiveStr.Replace(TCP.END_Code, "");
		}

		Debug.Log("messageReceive: "+receiveStr);
	}
	
	
	void OnGUI () {

		GUI.matrix = Matrix4x4.Scale(Vector3.one * 3);
		GUILayout.BeginArea(new Rect(10,10,200,400));
		GUILayout.Label( "TCP/IP CLIENT: " + connectStatus );
		
		client.m_serverIp = GUILayout.TextField(client.m_serverIp);
		client.m_port = int.Parse( GUILayout.TextField(client.m_port.ToString()) );
		if( GUILayout.Button("ConnectToServer") ){
			client.ConnectToServer();
		}
		if( GUILayout.Button("Close") ){
			client.Close();
		}

		GUILayout.Space(20);
		GUILayout.Label( "RECEIVE: " + receiveStr );
		GUILayout.Space(20);
		sendStr = GUILayout.TextField(sendStr);
		if( GUILayout.Button("Send") ){
			client.Send(sendStr);
		}
		GUILayout.EndArea();
	}


}
