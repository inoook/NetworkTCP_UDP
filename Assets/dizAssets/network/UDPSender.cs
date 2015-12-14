using UnityEngine;
using System.Collections;

public class UDPSender : MonoBehaviour {
	
	public UDP udp;
	public string ip = "127.0.0.1";
	public int sendPort = 11999;

	void Start()
	{

	}

	// Use this for initialization
	public void setup () {
		//udp = this.gameObject.GetComponent<UDP>();
		if(udp == null){
			udp = this.gameObject.AddComponent<UDP>();
		}
		if(!udp.IsOpen()){
			udp.Setup();
			udp.SetupSender(ip, sendPort);
			udp.Create();
		}
	}
	public void send(string str)
	{
		if(udp != null){
			//udp.SendData(str);
			udp.SendData(str, ip, sendPort);
		}
	}
	void OnDisable()
	{
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
