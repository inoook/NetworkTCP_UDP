using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class NetworkViewConnectorGUI : MonoBehaviour {
	
	public NetworkViewConnector networkViewConnector;
	
	public bool showGUI = true;
	public int windowId = 0;
	public int guiDepth = 0;
	
	private string myIpAddress;
	
	public Rect windowRect = new Rect(10,10,200,300);
	public GUISkin guiSkin;
	
	void Start()
	{
		myIpAddress = "";
		//#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
		#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
			
			string hostname = Dns.GetHostName();
			
			// ホスト名からIPアドレスを取得する
			//IPHostEntry ipInfo = Dns.GetHostByName(hostname);
			IPHostEntry ipInfo = Dns.GetHostEntry(hostname);
			Debug.Log(hostname);
			foreach (IPAddress address in ipInfo.AddressList) {
				if (address.AddressFamily == AddressFamily.InterNetwork){
					Debug.Log(address.ToString());
					myIpAddress += address.ToString() +" / ";
				}
			}
		#endif
	}
	
	void OnGUI ()
	{
		if(!showGUI){ return; }
		
		GUI.skin = guiSkin;
		windowRect = GUI.Window(windowId, windowRect, DrawWindow, "NetworkView");
	}
	
	void DrawWindow (int id)
	{
		
		GUI.depth = guiDepth;
		GUILayout.Label("IpAddress: "+myIpAddress);
		
		if (Network.peerType == NetworkPeerType.Disconnected){
			
			GUILayout.Label("status: Disconnected");
			
			GUILayout.Label("MAIN");
			networkViewConnector.connectToIP = GUILayout.TextField(networkViewConnector.connectToIP, GUILayout.MinWidth(100));
			networkViewConnector.connectPort = int.Parse(GUILayout.TextField(networkViewConnector.connectPort.ToString()));
			
			GUILayout.BeginVertical();
			networkViewConnector.autoConnect = GUILayout.Toggle(networkViewConnector.autoConnect, "autoConnect");
			if (GUILayout.Button ("Connect as client"))
			{
				//Connect to the "connectToIP" and "connectPort" as entered via the GUI
				//Ignore the NAT for now
				networkViewConnector.connectAsClient();
			}
			
			if (GUILayout.Button ("Start Server"))
			{
				//Start a server for 32 clients using the "connectPort" given via the GUI
				//Ignore the nat for now
				//Network.InitializeSecurity();
	
				networkViewConnector.connectAsServer();
			}
			
			GUILayout.EndVertical();
		}else{
			//We've got a connection(s)!
			
			if (Network.peerType == NetworkPeerType.Connecting){
			
				GUILayout.Label("status: Connecting..: "+ networkViewConnector.connectToIP.ToString() +" : "+ networkViewConnector.connectPort.ToString());
				
			} else if (Network.peerType == NetworkPeerType.Client){
				
				GUILayout.Label("status: Client! "+ networkViewConnector.connectToIP.ToString() +" : "+ networkViewConnector.connectPort.ToString());
				GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );		
				
			} else if (Network.peerType == NetworkPeerType.Server){
				
				GUILayout.Label("status: Server! "+ networkViewConnector.connectToIP.ToString() +" : "+ networkViewConnector.connectPort.ToString());
				GUILayout.Label("Connections: "+Network.connections.Length);
				if(Network.connections.Length >= 1){
					GUILayout.Label("Ping to first player: "+Network.GetAveragePing(  Network.connections[0] ) );
				}
				
				GUILayout.Label("SendRate: "+Network.sendRate.ToString());
				Network.sendRate = GUILayout.HorizontalSlider((int)Network.sendRate, 10, 60);
				networkViewConnector.sendRate = (int)Network.sendRate;
			}
	
		}
		
		GUILayout.Space(10);
		if (GUILayout.Button ("Disconnect"))
		{
			Network.Disconnect(200);
		}
		
		//GUILayout.EndArea();
		
		GUI.DragWindow();
	}
	
}
