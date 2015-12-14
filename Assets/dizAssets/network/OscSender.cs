using UnityEngine;
using System.Collections;

[AddComponentMenu("network/OscSender")]

public class OscSender : MonoBehaviour {
	
	private Osc oscHandler;
	public string host = "127.0.0.1";
	public int port = 1234;
	
	private bool _isSetUp = false;
	
	//~OscSender()
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
		setup();
	}
	
	public void setup()
	{
		setup(host, port);
	}
	
	public void setup(string host_, int port_)
	{
		if(_isSetUp){ return; }
		
		Debug.Log("OscSender SETUP: "+ host_ + " / "+ port_);
		host = host_;
		port = port_;
		_isSetUp = true;
		
		UDPPacketIO udp;
		udp = this.gameObject.GetComponent<UDPPacketIO>();
		if(udp == null){
			udp = this.gameObject.AddComponent<UDPPacketIO>();
		}
		udp.enableQueue = false;
        udp.Init(host_, port_, true);
        
		oscHandler = this.gameObject.GetComponent<Osc>();
		if(oscHandler == null){
			oscHandler = this.gameObject.AddComponent<Osc>();
		}
        oscHandler.init(udp);
	}
	
	public void send(OscMessage oscM)
	{
		oscHandler.Send(oscM);
	}
	
	void OnDisable()
    {
        Debug.Log("closing OSC UDP socket in OnDisable");
        oscHandler.Cancel();
        oscHandler = null;
    }
}
