using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

// http://d.hatena.ne.jp/hoge-maru/20111025/1319556606

// 9216 maxbytes
public class UDPTest : MonoBehaviour {
	
	
	IPEndPoint remoteIP;
	byte[] data;
	Socket s;
	
	public int byteSize;
	
	private string result;
	
	void OnGUI(){
		GUILayout.BeginArea(new Rect(10,10,100,300));
		if(GUILayout.Button ("Send Packet")){
			data = new byte[byteSize];
			//s.SendTo(data, 0, data.Length, SocketFlags.None, remoteIP);
			try{
				s.SendTo(data, 0, data.Length, SocketFlags.None, remoteIP);
				result = "OK";
			}catch{
				result = "NG";
			}
		}
		byteSize = int.Parse( GUILayout.TextField(byteSize.ToString()) );
		GUILayout.Label(result);
		GUILayout.EndArea();
		
	}
	
	// Use this for initialization
	void Start () {
		remoteIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11998);
		
		//s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		//s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
		s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		//s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, 255);
		
		//s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 65507);
		Debug.Log(s.SendBufferSize);// default
		s.SendBufferSize = 65507;
		Debug.Log(s.SendBufferSize);// max
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}