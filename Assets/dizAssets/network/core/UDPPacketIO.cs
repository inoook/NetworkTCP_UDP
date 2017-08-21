using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
//using System.Threading;
using UnityEngine;
using System.Text;


public delegate void MessageUdpReceivedHandler(string message);
public delegate void MessageBufferReceivedHandler(byte[] buffer);

//http://www.sundh.com/blog/wp-content/uploads/2012/06/UDPPacketIO.cs
// http://social.msdn.microsoft.com/Forums/en/netfxnetcom/thread/baa3a5bb-2154-445f-965d-8a139dbe932a

  /// <summary>
  /// UdpPacket provides packetIO over UDP
  /// </summary>
  public class UDPPacketIO : MonoBehaviour 
  {
    private UdpClient sender;
    private UdpClient receiver;
    private bool socketsOpen;
    private string remoteHostName;
    private int remotePort;
    private int localPort;
	
	public bool isSender = false;
	public bool isReceiver = false;

	public bool enableQueue = false;
	private Queue messageQueue;
	private Queue messageQueueBuffer;

    void Start() 
    {
        //do nothing. init must be called  	
    }

	public void Init(string hostIP, int port, bool isSender){

        RemoteHostName = hostIP;
        RemotePort = isSender ? port : 0;
        LocalPort = isSender ? 0 : port;
        socketsOpen = false;

		this.isSender = isSender;
		this.isReceiver = !isSender;
		
		if(enableQueue){
			messageQueue = Queue.Synchronized(new Queue());
		}
  	}
	
	public void Init(string hostIP, int remotePort, int localPort){

		RemoteHostName = hostIP;
		RemotePort = remotePort;
		LocalPort = localPort;
		socketsOpen = false;

		if(enableQueue){
			messageQueue = Queue.Synchronized(new Queue());
		}
  	}

	public void CreateReceiveQueue()
	{
		enableQueue = true;
		messageQueueBuffer = Queue.Synchronized(new Queue());
	}
  	

    ~UDPPacketIO()
    {
        // latest time for this socket to be closed
        if (IsOpen())
            Close();
    }

    /// <summary>
    /// Open a UDP socket and create a UDP sender.
    /// 
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool Open()
    {
//		Debug.Log("Open: "+this.gameObject.name);
        try
        {
			/*
			if(SenderMode){
				// Server
            	Sender = new UdpClient();
			}
			
			if(!SenderMode){
				// Receiver
            	IPEndPoint listenerIp = new IPEndPoint(IPAddress.Any, localPort);
            	Receiver = new UdpClient(listenerIp);
			}
			*/

			Debug.Log("isSender: " +isSender + " / isReceiver: "+isReceiver);
			
			if(isSender){
				Debug.Log("SENDER: "+ RemotePort);
//				IPEndPoint listenerIp = new IPEndPoint(IPAddress.Any, RemotePort);
//				sender = new UdpClient(RemotePort);

				//sender = new UdpClient();

				if(localPort == 0){
					sender = new UdpClient();
				}else{
					IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, localPort);
					sender = new UdpClient(remoteIp);
				}

				((Socket)(sender.Client)).SendBufferSize = 65507;//max
				//((Socket)(Sender.Client)).SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 65507);
				//Sender.ExclusiveAddressUse = true;
				//Sender.EnableBroadcast = true;
				//Sender.MulticastLoopback = false;
				//Sender.Ttl = 1;
				
				//Debug.Log( sender.DontFragment );
			}
			
			if(isReceiver){
				Debug.Log("RECEIVER: "+localPort);
				Debug.Log("opening udpclient listener on port " + localPort);
				IPEndPoint listenerIp = new IPEndPoint(IPAddress.Any, localPort);
				receiver = new UdpClient(listenerIp);
				//((Socket)Receiver).Blocking = false;
			}
			
			socketsOpen = true;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("cannot open udp client interface at port "+localPort);
            Debug.LogWarning(e);
        }

        return false;
    }

    /// <summary>
    /// Close the socket currently listening, and destroy the UDP sender device.
    /// </summary>
    public void Close()
    {    
        if(sender != null)
            sender.Close();
        
        if (receiver != null)
        {
            receiver.Close();
			if(messageQueue != null){
				messageQueue = null;
			}
			if(messageQueueBuffer != null){
				messageQueueBuffer = null;
			}
			Debug.Log("UDP receiver closed");
        }
        receiver = null;
        socketsOpen = false;

    }

    public void OnDisable()
    {
        Close();
    }

    /// <summary>
    /// Query the open state of the UDP socket.
    /// </summary>
    /// <returns>True if open, false if closed.</returns>
    public bool IsOpen()
    {
      return socketsOpen;
    }

    /// <summary>
    /// Send a packet of bytes out via UDP.
    /// </summary>
    /// <param name="packet">The packet of bytes to be sent.</param>
    /// <param name="length">The length of the packet of bytes to be sent.</param>
    public void SendPacket(byte[] packet, int length)
    {
		//if(Sender == null){ return; }
		
        if (!IsOpen())
            Open();
        if (!IsOpen())
            return;
      	
        //Sender.Send(packet, length, remoteHostName, remotePort);

		IPEndPoint ipEP = new IPEndPoint(IPAddress.Parse(remoteHostName), remotePort);
		sender.Send(packet, length, ipEP);
		
		// broadcast
//		IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
//		sender.Send(packet, length, groupEP); 
        
		//Debug.Log("osc message sent to "+remoteHostName+" port "+remotePort+" len="+length);
    }
	
	public void SendData(string str)
	{
		Encoding sjisEnc = Encoding.GetEncoding("utf-8");
		byte[] packet = sjisEnc.GetBytes(str);
		SendPacket(packet, packet.Length);
	}

	// add
	public void SendPacket(byte[] packet, int length, string host, int port)
	{
		if (!IsOpen())
			Open();
		if (!IsOpen())
			return;

		IPEndPoint ipEP = new IPEndPoint(IPAddress.Parse(host), port);
		sender.Send(packet, length, ipEP);
	}
	public void SendData(string str, string host, int port)
	{
		Encoding sjisEnc = Encoding.GetEncoding("utf-8");
		byte[] packet = sjisEnc.GetBytes(str);
		SendPacket(packet, packet.Length, host, port);
	}


	public delegate void ReceivePacketHandler(byte[] buffer);
	public ReceivePacketHandler receivePacket;
	
	public delegate void ReceiveDataHandler(string str);
	public ReceiveDataHandler receiveData;
	
    /// <summary>
    /// Receive a packet of bytes over UDP.
    /// </summary>
    /// <param name="buffer">The buffer to be read into.</param>
    /// <returns>The number of bytes read, or 0 on failure.</returns>
    public int ReceivePacket(byte[] buffer)
    {
        if (!IsOpen())
            Open();
        if (!IsOpen())
            return 0;
	
		
      IPEndPoint iep = new IPEndPoint(IPAddress.Any, localPort);
      byte[] incoming = receiver.Receive( ref iep );
      int count = Math.Min(buffer.Length, incoming.Length);

		System.Array.Copy(incoming, buffer, count);
		//
		if (count > 0) {
			if(receivePacket != null){
				//Debug.Log("ReceivePacketEvent");
				receivePacket(buffer);
			}
			
			Encoding sjisEnc = Encoding.GetEncoding("utf-8");
			string str = sjisEnc.GetString(incoming);

			if(receiveData != null){
				receiveData(str);
			}

			if(enableQueue){
				// queueに追加
				if(messageQueue != null){
					messageQueue.Enqueue( str );
				}
			}
			if (enableQueue) {
				// queueに追加
				if (messageQueueBuffer != null) {
					messageQueueBuffer.Enqueue (buffer);
				}
			}
		}


      return count;
    }

	//
	// Queue
	public MessageUdpReceivedHandler MessageReceivedQueue;
	public MessageBufferReceivedHandler MessageReceivedQueueBuffer;
	void Update()
	{
		if(!enableQueue){ return; }

		if(messageQueue != null){
			lock(messageQueue.SyncRoot){
				if(messageQueue.Count > 0){
					
					if(this.MessageReceivedQueue != null){
						string msg = messageQueue.Dequeue().ToString();
						this.MessageReceivedQueue( msg );
					}
				}
			}
		}

		if(messageQueueBuffer != null){
			lock(messageQueueBuffer.SyncRoot){
				if(messageQueueBuffer.Count > 0){

					if(this.MessageReceivedQueueBuffer != null){
						byte[] buffer = (byte[])(messageQueueBuffer.Dequeue());
						this.MessageReceivedQueueBuffer( buffer );
					}
				}
			}
		}
	}

    /// <summary>
    /// The address of the board that you're sending to.
    /// </summary>
    public string RemoteHostName
    {
      get
      { 
        return remoteHostName; 
      }
      set
      { 
        remoteHostName = value; 
      }
    }
  
    /// <summary>
    /// The remote port that you're sending to.
    /// </summary>
    public int RemotePort
    {
      get
      { 
        return remotePort; 
      }
      set
      { 
        remotePort = value; 
      }
    }

    /// <summary>
    /// The local port you're listening on.
    /// </summary>
    public int LocalPort
    {
      get
      {
        return localPort; 
      }
      set
      { 
        localPort = value; 
      }
    }
}
