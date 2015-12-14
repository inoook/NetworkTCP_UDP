using UnityEngine;
using System.Collections;

public class UDPSampleSender: MonoBehaviour {
	
	public UDPSender udpSender;
	public float sendRate = 24;

	public bool autoConnect = true;

	// Use this for initialization
	void Start () {
		if(autoConnect){
			Connect();
		}
//		float sendRate_ = 1f/sendRate;
//		InvokeRepeating("sendMsg", 0, sendRate_);
	}

	void Connect()
	{
		if(udpSender == null){
			udpSender = this.gameObject.GetComponent<UDPSender>();
		}
		udpSender.setup();
	}
	void Disconnect()
	{
		udpSender.close();
	}

	public void sendMsg()
	{
		if(udpSender != null){
			udpSender.send(sendMessage);
		}
	}
	
	public string sendMessage = "send data";
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(350,10,300,300));
		GUILayout.Label("UPD Sender");
		GUILayout.Label("send to: "+udpSender.ip + " : "+udpSender.sendPort);

		if(GUILayout.Button("Connect: "+ udpSender.IsOpen())){
			Connect();
		}
		if(GUILayout.Button("DisConnect")){
			Disconnect();
		}
		sendMessage = GUILayout.TextField(sendMessage);
		if(GUILayout.Button("Send")){
			udpSender.send(sendMessage + "\0");
		}
		GUILayout.EndArea();
	}
}
