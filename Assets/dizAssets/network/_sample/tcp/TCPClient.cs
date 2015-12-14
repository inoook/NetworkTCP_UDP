using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

// http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

public class TCPClient : MonoBehaviour {

	public string m_serverIp = "127.0.0.1";
	public int m_port = 11999;
	
	public float reconnectTime = 4;
	private Thread listenThread; //listening for client connections
	private TcpClient client;

	public event MessageReceivedHandler MessageReceived;
	public event MessageReceivedHandler MessageReceivedQueue;

	public enum Status
	{
		Connect, Disconnect, Error
	}
	public delegate void StatusHandler(Status status);
	public event StatusHandler eventConnectStatus;
	
	private Queue messageQueue;
	private Queue statusQueue;
	
	public void ConnectToServer (string ip, int port) {
		m_serverIp = ip;
		m_port = port;
		
		ConnectToServer(m_serverIp, m_port);
	}

	public void ConnectToServer () {
		messageQueue = Queue.Synchronized(new Queue());
		statusQueue = Queue.Synchronized(new Queue());
		
		StartConnection(m_serverIp, m_port);
	}
	
	public void Close()
	{
		DeInit();
	}

	void OnDestroy()
	{
		DeInit();
	}
	void OnApplicationQuit()
    {
		DeInit();
	}

	void DeInit()
	{
		if(client != null){
			client.Close();
		}

		if(listenThread != null){
			listenThread.Abort();
		}
	}
	
	void StartConnection(string serverIp, int port)
	{
		try{
			client = new TcpClient();

			client.Connect(IPAddress.Parse(serverIp), port);
			//Send("Hello Server!");
			
			if(eventConnectStatus != null){
				eventConnectStatus(Status.Connect);
			}
			
			this.listenThread = new Thread(new ThreadStart(ListenForClients));
			this.listenThread.Start();
			
		}catch(System.Exception e){
			Debug.LogWarning("no Server: "+e);
			// reconnect
			AutoReconnect();
		}
	}
	
	public void Send(string msgStr){

		if(client != null && client.Connected){
			ASCIIEncoding encoder = new ASCIIEncoding();
			string sendStr = msgStr + TCP.END_Code;
			Debug.Log("sendStr: "+ sendStr +"/");
			byte[] buffer = encoder.GetBytes(sendStr);
			NetworkStream clientStream = client.GetStream();
			try{
				clientStream.Write(buffer, 0 , buffer.Length);
				//clientStream.Flush();
			}catch(System.Exception e){
				Debug.LogWarning("The socket has been shut down: "+e);
			}
		}else{
			//Debug.LogWarning("no Connection");
		}
	}


	private void ListenForClients()
    {
        //while (true)
        //{
			// 受信スレッドを作成・実行
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread.Start(client);
        //}
    }
	// http://note.chiebukuro.yahoo.co.jp/detail/n25036
	
	private void HandleClientComm(object obj)
    {
        try
        {
            using (TcpClient client = (TcpClient)obj)
            {
                using (NetworkStream stream = client.GetStream())
                {
					NetworkStream clientStream = client.GetStream();

					byte[] message = new byte[4096];
					int bytesRead;
					
					while (true)
					{
						bytesRead = 0;
						
						try{
							//block until a client sends a message and is received
							bytesRead = clientStream.Read(message, 0, 4096);
						} catch (System.Exception ex) {
							//a socket error has occurred
							Debug.LogWarning(ex);
							break;
						}

						if (bytesRead == 0)
						{
							//the client has disconnected from the server
							statusQueue.Enqueue( Status.Disconnect );
							break;
						}

						//message has successfully been recieved
						ASCIIEncoding encoder = new ASCIIEncoding();
						string smessage = encoder.GetString(message, 0, bytesRead);

						if (this.MessageReceived != null){
							this.MessageReceived(smessage); // dispatch
						}

						// queueに追加
						messageQueue.Enqueue( smessage );
					}

					client.Close();
                }
            }
        } catch (System.Exception ex) {
			Debug.LogWarning("error: "+ex);
        }

    }

	//
	// http://answers.unity3d.com/questions/200176/multithreading-and-messaging.html
	void Update()
	{
		lock(messageQueue.SyncRoot){
			if(messageQueue.Count > 0){

				if(this.MessageReceivedQueue != null){
					string msg = messageQueue.Dequeue().ToString();
					this.MessageReceivedQueue( msg );
				}
			}
		}

		lock(statusQueue.SyncRoot){
			if(statusQueue.Count > 0){
				Status status = (Status)(statusQueue.Dequeue());
				if(status == Status.Disconnect){
					// disconnect
					if(eventConnectStatus != null){
						eventConnectStatus(Status.Disconnect);
					}
					// reconnect
					AutoReconnect();
				}
			}
		}
	}

	void AutoReconnect()
	{
		if(reconnectTime > 0){
			Invoke("ConnectToServer", reconnectTime);
		}
	}
	
}
