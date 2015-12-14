using UnityEngine;
using System.Collections;

public class UDPSampleReciever : MonoBehaviour {
	
	public UDPReceiver udpReciever;
	
//	OscMessageHandler oscFunc;
//	OscMessageHandler oscAllFunc;

	public bool autoConnect = true;
	
	// Use this for initialization
	void Start () {
		if(autoConnect){
			Connect();
		}
	}

	void Connect()
	{
		if(udpReciever == null){
			udpReciever = this.gameObject.GetComponent<UDPReceiver>();
		}

		udpReciever.eventMessageReceivedQueue += HandleEventMessageReceivedQueue;

		udpReciever.setup();
	}

	void HandleEventMessageReceivedQueue (string message)
	{
		Debug.Log("OnReceiveDataQueue>> "+message);
		if(message.IndexOf("\0") > -1){
			Debug.Log("OK: "+message);
		}
	}

	void Disconnect()
	{
		udpReciever.eventMessageReceivedQueue -= HandleEventMessageReceivedQueue;
		udpReciever.close();
	}

	bool IsOpen()
	{
		if(udpReciever == null){
			return false;
		}else{
			return udpReciever.IsOpen();
		}
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10,300,300));
		GUILayout.Label("UPD Reciever");
		if(GUILayout.Button("Connect: "+ IsOpen() )){
			Connect();
		}
		if(GUILayout.Button("DisConnect")){
			Disconnect();
		}
		GUILayout.Label((udpReciever.receivePort).ToString());

		GUIStyle style = GUI.skin.GetStyle("TextField");
		style.wordWrap = true;
		
		GUILayout.TextField(udpReciever.receiveMessage, style, GUILayout.Height(100));
		
		GUILayout.EndArea();
		/*
		if(GUILayout.Button(new Rect(10,10,100,100), "Send")){
			udp.SendData("sendStr");
		}
		*/
	}
}
