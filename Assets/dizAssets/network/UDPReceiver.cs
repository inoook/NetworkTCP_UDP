using UnityEngine;
using System.Collections;

public class UDPReceiver : MonoBehaviour {

	public delegate void ReceiveDataQueueHandler(string message);

	private UDP udp;
	public int receivePort = 11999;

	public event ReceiveDataQueueHandler eventMessageReceivedQueue;

	// Use this for initialization
	void Start () {
	}
	public void setup () {
		udp = this.gameObject.GetComponent<UDP>();
		if(udp == null){
			udp = this.gameObject.AddComponent<UDP>();
		}
		if(!udp.IsOpen()){
			udp.Setup();
			udp.SetupReceiver(receivePort);
			//udp.SetReceiveDataHandler(OnReceiveData);
			//udp.SetReceivePacketHandler(OnReceivePacket);
			udp.SetReceiveDataQueueHandler(OnReceiveDataQueue);
			
			udp.Create();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public string receiveMessage ="";
	// よばれているのがメインスレッドでないので、transform.position=newPos などはこの中ではつかえない
	void OnReceiveData(string message)
	{
		Debug.Log("OnReceiveData>> "+message);
		
		if(message != ""){
			receiveMessage = message;
			/*
			float x,y;
			char[] separator = ("[/p]").ToCharArray();
			string[] strPoints = message.Split(separator);
			for(int i=0; i < strPoints.Length; i++){
				string[] point = ((string)(strPoints[i])).Split("|"[0]);
				if( point.Length == 2 ){
					x = float.Parse(point[0]);
					y = float.Parse(point[1]);
					//stroke.push_back(ofPoint(x,y));
					Debug.Log(x + " / "+ y);
				}
			}
			*/
		}
	}
	
	void OnReceivePacket(byte[] buffer)
	{
		Debug.Log("OnReceivePacketa>> "+buffer);
	}

	// Queue 
	void OnReceiveDataQueue(string message)
	{
		if(eventMessageReceivedQueue != null){
			eventMessageReceivedQueue(message);
		}
		
		if(message != ""){
			receiveMessage = message;
		}
	}
	
	void OnDisable()
    {
        Debug.Log("closing UDP socket in OnDisable");
		close();
    }
	public void close()
	{
		if(udp != null){
			udp.Cancel();
			udp = null;
		}
	}

	public bool IsOpen()
	{
		if(udp == null){
			return false;
		}
		return udp.IsOpen();
	}
}
