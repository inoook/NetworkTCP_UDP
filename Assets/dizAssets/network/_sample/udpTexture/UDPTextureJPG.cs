using UnityEngine;
using System.Collections;

public class UDPTextureJPG : MonoBehaviour {
	
	public UDP udp;
	
	public RenderTexture sendRenderTexture;
	public Texture2D sendTexture;
	public Texture2D recieveTexture;
	
	private byte[] recieveBytes;
	
	public float jpgQuality = 75.0f;
	
	// Use this for initialization
	void Start () {
		udp.Setup("127.0.0.1", 11999, 11999);
		udp.SetReceivePacketHandler(OnReceivePacket);
		udp.Create();
	}
	
	// Update is called once per frame
	void Update () {
		SendTexture();
		
		if(recieveBytes != null && recieveBytes.Length > 0)
		{
			//Debug.Log(recieveBytes.Length);
			if(recieveTexture == null){
				recieveTexture = new Texture2D(0, 0,TextureFormat.RGB24, false);
			}
			if(isUpdatePacket){
				if(recieveBytes[0] != 0){
					recieveTexture.LoadImage(recieveBytes);
					isUpdatePacket = false;
				}
			}
		}
	}
	
	
	void OnReceivePacket(byte[] buffer)
	{
		recieveBytes = buffer;
		isUpdatePacket = true;
	}
	
	private bool isUpdatePacket;
	
	
	public bool useJpgEncodeSend = false;
	
	public void SendTexture()
	{
		if(sendTexture == null){
			sendTexture = new Texture2D(sendRenderTexture.width, sendRenderTexture.height, TextureFormat.RGB24, false);
		}
		RenderTexture.active = sendRenderTexture;
		sendTexture.ReadPixels(new Rect(0, 0, sendRenderTexture.width, sendRenderTexture.height), 0, 0, false);
		//sendTexture.Apply(false);
		
		byte[] packet;
		if(useJpgEncodeSend){
			JPGEncoder encoder = new JPGEncoder(sendTexture, jpgQuality);
			encoder.doEncoding();
			packet = encoder.GetBytes();
		}else{
			packet = sendTexture.EncodeToPNG();
		}
		
		if(packet.Length > 0){
			udp.SendPacket(packet);
		}
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
