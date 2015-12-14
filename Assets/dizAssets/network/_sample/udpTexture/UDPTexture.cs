using UnityEngine;
using System.Collections;

public class UDPTexture : MonoBehaviour {
	
	public UDP udp;
	
	public RenderTexture sendRenderTexture;
	public Texture2D sendTexture;
	public Texture2D recieveTexture;
	
	private byte[] recieveBytes;
	
	// Use this for initialization
	void Start () {
		udp.Setup("127.0.0.1", 11999, 11999);
		udp.SetReceivePacketHandler(OnReceivePacket);
		udp.Create();
	}
	
	// Update is called once per frame
	void Update () {
		SendTexture();
		
		if(recieveBytes != null)
		{
			if(recieveTexture == null){
				recieveTexture = new Texture2D(0, 0,TextureFormat.RGB24, false);
			}
			recieveTexture.LoadImage(recieveBytes);
		}
	}
	
	void OnReceivePacket(byte[] buffer)
	{
		recieveBytes = buffer;
	}
	
	public void SendTexture()
	{
		if(sendTexture == null){
			sendTexture = new Texture2D(sendRenderTexture.width, sendRenderTexture.height, TextureFormat.RGB24, false);
		}
		RenderTexture.active = sendRenderTexture;
		sendTexture.ReadPixels(new Rect(0, 0, sendRenderTexture.width, sendRenderTexture.height), 0, 0, false);
		//sendTexture.Apply(false);
		
		byte[] packet = sendTexture.EncodeToPNG();
		udp.SendPacket(packet);
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10,10,800,800));
		GUILayout.Label("Sender");
		if(GUILayout.Button("Send")){
			SendTexture();
		}
		
		if(recieveTexture != null){
			GUILayout.Label(recieveTexture);
		}
		GUILayout.EndArea();
	}
}
