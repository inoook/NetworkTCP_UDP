using UnityEngine;
using System.Collections;

// http://docs.unity3d.com/Documentation/ScriptReference/NetworkView.RPC.html

[RequireComponent (typeof (NetworkView))]
public class RPCSender : MonoBehaviour {
	
	public static RPCSender instance;
	
	void Awake ()
	{
		if(instance == null){
			instance = this;
		}else{
			Debug.LogWarning("RPCSender is Singleton");
		}
		
	    GetComponent<NetworkView>().group = 10;
	}
	/*
	public void SendFunction(string _refName, string methodName)
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			//networkView.RPC("RunFunction", RPCMode.OthersBuffered, _refName, methodName);
			networkView.RPC("RunFunction", RPCMode.Others, _refName, methodName);
		}
	}
	*/
	// _refNameのついたGameObjectのmethodNameをコールする。受取り側ではSendMessageをつかっている。
	public void SendFunction(string _refName, string methodName, RPCMode rpcMode = RPCMode.Others)
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			GetComponent<NetworkView>().RPC("RunFunction", rpcMode, _refName, methodName);
		}
	}
	
	[RPC]
	void RunFunction(string _refName, string methodName)
	{
		GameObject _refGObj = GameObject.Find(_refName);

		if(_refGObj != null){
			_refGObj.SendMessage(methodName);
		}
	
	}
	
	//----------
	/*
	public void SendFunctionWithString(string _refName, string methodName, string str)
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			networkView.RPC("RunFunctionWithString", RPCMode.Others, _refName, methodName, str);
		}
	}
	*/
	public void SendFunctionWithString(string _refName, string methodName, string str, RPCMode rpcMode = RPCMode.Others)
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			GetComponent<NetworkView>().RPC("RunFunctionWithString", rpcMode, _refName, methodName, str);
		}
	}
	
	[RPC]
	void RunFunctionWithString(string _refName, string methodName, string str)
	{
		GameObject _refGObj = GameObject.Find(_refName);
		
		if(_refGObj != null){
			_refGObj.SendMessage(methodName, str);
		}
	}
	
	
	
	//----------
	// sample
	/*
	public float v = 0.0f;
	
	public void SendValue(float v)
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			networkView.RPC("LoadValue", RPCMode.Others, v);
		}
	}
	
	void OnGUI()
	{
		if( GUILayout.Button("SEND") ){
			SendValue(v);
		}
		
		GUILayout.Label(v.ToString());
	}
	
	 */

}
