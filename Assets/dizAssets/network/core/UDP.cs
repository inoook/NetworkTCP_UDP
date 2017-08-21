using UnityEngine;
using System.Collections;
using System.Threading;
using System; 

// http://www.sundh.com/blog/wp-content/uploads/2012/06/Osc.cs
// TODO: ocsとまとめる。
[RequireComponent (typeof (UDPPacketIO))]
public class UDP : MonoBehaviour {

	// ipは送り先を指定する
	[HideInInspector]
	public string host = "127.0.0.1";// broadcast 255.255.255.255 // 送り先
	[HideInInspector]
	public int sendPort = 11999;// 0のときはsendしない
	[HideInInspector]
	public int receivePort = 11998;// 0のときはreceiveしない
	
	public UDPPacketIO udpPacketIO;
	
	Thread ReadThread;
	bool ReaderRunning = false;

	// Use this for initialization
	
	public void Setup()
	{
		udpPacketIO = this.gameObject.GetComponent<UDPPacketIO>();
		if(udpPacketIO == null){
			udpPacketIO = this.gameObject.AddComponent<UDPPacketIO>();
		}
	}
	public void Setup(string ip, int sendPort_, int receivePort_)
	{
		host = ip;
		sendPort = sendPort_;
		receivePort = receivePort_;

		Setup();

		udpPacketIO.isSender = true;
		udpPacketIO.isReceiver = true;
	}

	public void Create () {

		//udpPacketIO.init(host, port, false);
		udpPacketIO.Init(host, sendPort, receivePort);
		udpPacketIO.Open();
		//udpPacketIO.receivePacket += HandleUdpReceivePacket;
		//udpPacketIO.receiveData += HandleUdpReceiveData;

		//if(receivePort != 0){
		if(udpPacketIO.isReceiver){
			ReadThread = new Thread(Read);
			ReaderRunning = true;
			ReadThread.IsBackground = true;      
			ReadThread.Start();
		}
	}

	public void SetupSender(string ip, int port, int localPort = 11998)
	{
		host = ip;
		sendPort = port;
		receivePort = localPort;
		
		udpPacketIO.isSender = true;
	}

	public void SetupReceiver(int port)
	{
		//sendPort = 0;
		receivePort = port;
		
		udpPacketIO.isReceiver = true;
	}

//	~UDP()
//	{           
//		if (ReaderRunning) Cancel();
//	}
	
	public void Cancel()
    {
        if (udpPacketIO != null && udpPacketIO.IsOpen())
        {
            udpPacketIO.Close();
            udpPacketIO = null;
        }

		if (ReaderRunning)
		{
			ReaderRunning = false;
			ReadThread.Abort();
		}
    }
	
	public bool IsOpen(){
		if (udpPacketIO != null && udpPacketIO.IsOpen()){
			return true;
		}else{
			return false;
		}
	}
	

	private void Read()
    {
        try {
			while (ReaderRunning)
            {
		        byte[] buffer = new byte[100000];
				int length = udpPacketIO.ReceivePacket(buffer);

				if (length > 0){
					
				}else{
					Thread.Sleep(20);
				}
			}
		//} catch (Exception e) {
		} catch (ThreadAbortException e) {
            Debug.Log("ThreadAbortException"+e);
        }
//		finally
//        {
//			//Debug.Log("finally");
//        }
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(udpServer == null){ return; }
		
		byte[] sendBytes = myTexture2D.EncodeToPNG();
		Debug.Log(">>"+sendBytes.Length);
		udpPacketIO.SendPacket(sendBytes, sendBytes.Length);
		*/
//		if(ReaderRunning && udpPacketIO.isReceiver){
//			byte[] buffer = new byte[100000];
//
//			bool active = true;
//			while (active) {
//				int length = udpPacketIO.ReceivePacket(buffer);
//
//			}
//		}
	}
	
	public void SendData(string str)
	{
		if(udpPacketIO != null){
			udpPacketIO.SendData(str);
		}else{
			Debug.LogWarning("udpPacketIO is null");
		}
	}

	// add
	public void SendData(string str, string host, int port)
	{
		if(udpPacketIO != null){
			udpPacketIO.SendData(str, host, port);
		}else{
			Debug.LogWarning("udpPacketIO is null");
		}
	}

	public void SendPacket(byte[] packet)
	{
		udpPacketIO.SendPacket(packet, packet.Length);
		//int leng = (packet.Length > 9216) ? 9216 : packet.Length;
		//udpPacketIO.SendPacket(packet, leng);
	}
	//
	public void SetReceiveDataHandler(UDPPacketIO.ReceiveDataHandler handler)
	{
		udpPacketIO.receiveData = handler;
	}
	public void SetReceivePacketHandler(UDPPacketIO.ReceivePacketHandler handler)
	{
		udpPacketIO.receivePacket = handler;
	}
	
	public void SetReceiveDataQueueHandler(MessageUdpReceivedHandler handler)
	{
		udpPacketIO.enableQueue = true;
		udpPacketIO.MessageReceivedQueue = handler;
	}
}
