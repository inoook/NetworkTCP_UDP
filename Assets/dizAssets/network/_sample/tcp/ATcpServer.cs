using UnityEngine;
using System.Collections;

using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

// 北九州のサーバープリントで使用。　
public class ATcpServer : MonoBehaviour {
	
	static ATcpServer _instance = null;
	public static ATcpServer instance
	{
		get
		{
			if( !_instance )
			{
				_instance = FindObjectOfType( typeof( ATcpServer ) ) as ATcpServer;
				
				if( !_instance ){
					Debug.Log("Error-----");
					//
					var obj = new GameObject( "ATcpServer" );
					_instance = obj.AddComponent<ATcpServer>();
					DontDestroyOnLoad( obj );
				}
			}
			return _instance;
		}
	}
	
	public static void SendPrintCmdToFlash(Texture2D texture, Texture2D bookTexture, float score, string copyright) {
		instance.sendPrintCmdToFlash( texture, bookTexture, score, copyright );
	}
	
	
	private AsyncTcpServer asyncServer;
	
	// Use this for initialization
	void Start () {
		asyncServer = new AsyncTcpServer(IPAddress.Any, 11999);
		asyncServer.MessageReceived += HandleMessageReceived;
		asyncServer.DisConnectClient += HandlerDisConnect;
		
		asyncServer.Start();
	}

	void HandleMessageReceived (string message)
	{
		if(message.IndexOf(TCP.END_Code) > 0){
			receiveStr = message.Replace(TCP.END_Code, "");
		}
		
		receiveStr = message;
		Debug.Log("receiveStr: "+receiveStr);
	}
	private void HandlerDisConnect()
	{
		Debug.Log("DisConnectClient _ HandlerDisConnect< ");
	}
	void OnApplicationQuit()
    {
		asyncServer.Stop();
	}
	
	public void sendPrintCmdToFlash(Texture2D texture, Texture2D bookTexture, float score, string copyright)
	{
		Debug.Log("Printint ....>>"+ texture.width + " / "+texture.height);
		if(GetClientCount() < 1){
			Debug.LogError("No Client");
			return;
		} 
		
		byte[] bytes = texture.EncodeToPNG();
		string path = Application.dataPath+"/savedTexture.png";
		File.WriteAllBytes(path, bytes);
		
		byte[] bookbytes = bookTexture.EncodeToPNG();
		string bookPath = Application.dataPath+"/savedBookTexture.png";
		File.WriteAllBytes(bookPath, bookbytes);
		
		asyncServer.Write("sendTexture,"+path+","+bookPath+","+score.ToString("0")+","+copyright+"[/TCP]");
	}
	
	public int GetClientCount()
	{
		return asyncServer.GetClientCount();
	}

	string receiveStr = "";
	string sendStr = "";
	void OnGUI () {
		GUILayout.Label("GetClientCount: "+GetClientCount());
		GUILayout.Label( "RECEIVE: " + receiveStr );

		sendStr = GUILayout.TextField( sendStr );
		if(GUILayout.Button("Send")){
			asyncServer.Write(sendStr);
		}
	}
}
