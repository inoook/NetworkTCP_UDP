using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class DataConnection : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Vector3 currentPos;
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Vector3 pos = Vector3.zero;
		if (stream.isWriting) {
			currentPos = this.transform.position;
		    pos = currentPos;
			
		    stream.Serialize(ref pos);
		} else {
		    stream.Serialize(ref pos);
			
		    currentPos = pos;
			this.transform.position = currentPos;
		}
	}
	
	void OnGUI()
	{
		GUILayout.Label(currentPos.ToString());
	}
	
	/*
	public float currentHealth;
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		float health = 0;
		if (stream.isWriting) {
			currentHealth = this.transform.position.x;
		    health = currentHealth;
			
		    stream.Serialize(ref health);
		} else {
		    stream.Serialize(ref health);
			
		    currentHealth = health;
			this.transform.position = new Vector3(currentHealth, 0,0);
		}
	}
	
	void OnGUI()
	{
		GUILayout.Label(currentHealth.ToString());
	}
	*/
}
