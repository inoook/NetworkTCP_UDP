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

	public float v0 = 1.0f;
	public float v1 = 1.0f;

	public void sendOSCMsg()
	{
		if(_oscSender != null){
//			OscMessage oscMF = new OscMessage();
//			oscMF.setAddress("/sp/1/rot/xyz");
////			oscMF.setAddress("/position");
//			oscMF.addFloatArg(v.x);
//			oscMF.addFloatArg(v.y);
//			oscMF.addFloatArg(v.z);
//			
//			_oscSender.send(oscMF);

			OscMessage oscMF = new OscMessage();
			oscMF.setAddress("/sp/1");
			oscMF.addFloatArg (v0);

			_oscSender.send(oscMF);
			//
			OscMessage oscMF1 = new OscMessage();
			oscMF1.setAddress("/sp/2");
			Debug.Log ("/sp/2: "+v1);
			oscMF1.addFloatArg (v1);

			_oscSender.send(oscMF1);
		}
		
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
