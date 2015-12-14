using UnityEngine;
using System.Collections;

public class NetworkTexture : MonoBehaviour {

	//public Texture2D myTexture2D;
	
	public Texture2D targetTexture;
	
	public RenderTexture renderTexture;
	private Texture2D myTexture2D;
	void Start()
	{
		myTexture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
		//InvokeRepeating("_renderTexture", 0.0f, 0.075f);
		
	}
	
    void _renderTexture() {
		
		//Debug.LogWarning(myTexture2D.mipmapCount + " / " + targetTexture.mipmapCount);
		//Color[] colors = myTexture2D.GetPixels(0);
		
		//Debug.LogWarning( "SEND >> "+colors.Length + " / "+ bytes.Length);
		if(Network.isServer){
			RenderTexture.active = renderTexture;
			myTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
			myTexture2D.Apply(false);
			byte[] bytes = myTexture2D.EncodeToPNG();
			
        	GetComponent<NetworkView>().RPC("SendTexture", RPCMode.AllBuffered, bytes);
        	//networkView.RPC("SendColor", RPCMode.OthersBuffered, colors);
		}
		//drawTexture(bytes);
		//drawTextureByColor(colors);
    }
	
	void Update()
	{
		if(textureBytes != null){
			targetTexture.LoadImage(textureBytes);
		}
	}
	
	void OnGUI()
	{
		if( GUI.Button(new Rect(300,100, 100, 100), "SEND_DATA") ){
			_renderTexture();
		}
		if( GUI.Button(new Rect(300,250, 100, 100), "START") ){
			InvokeRepeating("_renderTexture", 0.0f, 0.5f);
		}
	}
	
	
    [RPC]
    void SendTexture(byte[] bytes) {
		drawTexture(bytes);
    }
	
	private byte[] textureBytes;
	public void drawTexture(byte[] bytes){
		//Debug.LogWarning("GET>> " + bytes.Length);
		//targetTexture.LoadImage(bytes);
		textureBytes = bytes;
	}
	
	
	[RPC]
    void SendColor(Color[] colors) {
		drawTextureByColor(colors);
    }
	
	public void drawTextureByColor(Color[] colors){
		Debug.LogWarning( "GET >> "+colors.Length );
		
		targetTexture.SetPixels(colors, 0);
		targetTexture.Apply(false);
	}
}
