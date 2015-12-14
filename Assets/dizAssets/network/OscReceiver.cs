using UnityEngine;
using System.Collections;

[AddComponentMenu("network/OscReceiver")]

public class OscReceiver : MonoBehaviour {
	
	private Osc oscHandler;
	public int port = 1234;
	
	private bool _isSetUp = false;
	
	//~OscReceiver()
	void OnDestroy()
    {
        if (oscHandler != null)
        {            
            oscHandler.Cancel();
        }

        // speed up finalization
        oscHandler = null;
        System.GC.Collect();
    }
	
	void Start()
	{
		//setup();
	}
	
	public void setup()
	{
		setup(port);
	}
	
	public void setup(int port)
	{
		if(_isSetUp){ return; }
		
		Debug.Log("OscReceiver SETUP: " + port);
		_isSetUp = true;
		
		UDPPacketIO udp;
		udp = this.gameObject.GetComponent<UDPPacketIO>();
		if(udp == null){
			udp = this.gameObject.AddComponent<UDPPacketIO>();
		}
        udp.Init("", port, false);
        
		oscHandler = this.gameObject.GetComponent<Osc>();
		if(oscHandler == null){
			oscHandler = this.gameObject.AddComponent<Osc>();
		}
//        oscHandler.init(udp);
        oscHandler.initReader(udp);
	}
	
	public void SetAddressHandler(string key, OscMessageHandler ah)
	{
		oscHandler.SetAddressHandler(key, ah);
	}
	public void SetAllMessageHandler(OscMessageHandler amh)
	{
		oscHandler.SetAllMessageHandler(amh);
	}
	
	void OnDisable()
    {
        Debug.Log("closing OSC UDP socket in OnDisable");
        oscHandler.Cancel();
        oscHandler = null;
    }
}
