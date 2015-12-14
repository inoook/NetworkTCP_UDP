using UnityEngine;
using System.Collections;

public class RPCSenderSample : MonoBehaviour {

	public RPCSender rpcSender;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Server){
			GUILayout.BeginArea(new Rect(300,10, 200, 200));
			if(GUILayout.Button("Send")){
				rpcSender.SendFunction("RPCSenderSample", "Resize");
			}
			GUILayout.EndArea();
		}
	}
				
	public void Resize()
	{
		this.transform.localScale = Vector3.one * 3;
	}
}
