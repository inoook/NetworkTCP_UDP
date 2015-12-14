using UnityEngine;
using System.Collections;

public class OSCSampleSender: MonoBehaviour {
	
	public OscSender _oscSender;
	public float sendRate = 24;
	
	public Vector3 v;
	
	// Use this for initialization
	void Start () {
		_oscSender = this.gameObject.GetComponent<OscSender>();
		_oscSender.setup();
		
		float sendRate_ = 1f/sendRate;
		InvokeRepeating("_sendOscData", 0, sendRate_);
	}
	
	void _sendOscData()
	{
		sendOSCMsg();
	}
	
	public void sendOSCMsg()
	{
		if(_oscSender != null){
			OscMessage oscMF = new OscMessage();
			oscMF.setAddress("/sp/1/rot/xyz");
//			oscMF.setAddress("/position");
			oscMF.addFloatArg(v.x);
			oscMF.addFloatArg(v.y);
			oscMF.addFloatArg(v.z);
			
			_oscSender.send(oscMF);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
