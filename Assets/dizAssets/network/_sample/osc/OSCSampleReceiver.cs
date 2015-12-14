using UnityEngine;
using System.Collections;

public class OSCSampleReceiver : MonoBehaviour {
	
	public OscReceiver oscReceiver;
	
	OscMessageHandler oscFunc;
	OscMessageHandler oscAllFunc;
	
	// Use this for initialization
	void Start () {
		oscReceiver.setup();
		
		oscFunc = OscHandler;
		oscReceiver.SetAddressHandler("/sp/1/rot/xyz", oscFunc);
		oscReceiver.SetAddressHandler("/sp/1/trans/xyz", oscFunc);
		
		oscAllFunc = OscAllHandler;
		oscReceiver.SetAllMessageHandler(oscAllFunc);
	}
	
	public Vector3 rValue;
	
	void OscHandler(OscMessage oscM)
	{
		string str = Osc.OscMessageValueToString(oscM);
		string[] strs = str.Split(","[0]);
		/*
		for(int i = 0; i < strs.Length; i++){
			Debug.Log(strs[i]);
		}
		*/
		rValue.x = float.Parse(strs[0]);
		rValue.y = float.Parse(strs[1]);
		rValue.z = float.Parse(strs[2]);
		
		Debug.Log("OscHandler >>> " + oscM.getAddress() + " // " + oscM.getArgAsFloat(0) +", " + oscM.getArgAsFloat(1)  +", " + oscM.getArgAsFloat(2));
		
		//Debug.Log(rValue);
	}
	
	void OscAllHandler(OscMessage oscM)
	{
		if(oscM.getAddress() == "/sp/1/rot/xyz"){
			float z = oscM.getArgAsFloat(2);
			Debug.Log("OscAllHandler >>> "+z);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
