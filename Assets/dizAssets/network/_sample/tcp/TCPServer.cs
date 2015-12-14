//http://www.switchonthecode.com/tutorials/csharp-tutorial-simple-threaded-tcp-server
//
//fwsteal 
//08/04/2010 - 11:41

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

public delegate void MessageReceivedHandler(string message);

public class TCPServer : MonoBehaviour
{
	public class ConnectClient{
		public TcpClient client;
		public ClientStatus status;

		public ConnectClient(TcpClient client_, ClientStatus status_)
		{
			client = client_;
			status = status_;
		}
	}

    //a simple threaded server that accepts connections and read data from clients.
    private TcpListener tcpListener; //wrapping up the underlying socket communication
    private Thread listenThread; //listening for client connections
    public int iPort = 11999; //server port

	public event MessageReceivedHandler MessageReceived;
	
	public enum ClientStatus {
		Connect, Disconnect, Error
	}
	public delegate void ClientStatusHandler(ClientStatus status, TcpClient client);
	public event ClientStatusHandler eventClientConnectStatus;

	public List<TcpClient> clients;

	private Queue statusQueue;

    public void Setup()
    {
		statusQueue = Queue.Synchronized(new Queue());

        this.tcpListener = new TcpListener(IPAddress.Any, iPort);
        this.listenThread = new Thread(new ThreadStart(ListenForClients));
        this.listenThread.Start();
    }

	public void Close()
	{
		DeInit();
	}
	
	void DeInit()
	{
		foreach (TcpClient client in this.clients)
		{
			client.Close();
		}
		if(tcpListener != null){
			tcpListener.Stop();
		}
	}

    private void ListenForClients()
    {
        this.tcpListener.Start(); //start tcplistener
		this.clients = new List<TcpClient>();
		//this.tcpListener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);

        //sit in a loop accepting connections
        while (true)
        {
            //block until a client has connected to the server
            TcpClient client = this.tcpListener.AcceptTcpClient();
		
			// ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address; // ipAdress
			lock (clients) {
				statusQueue.Enqueue( new ConnectClient(client, ClientStatus.Connect) );
				/*
				if(eventClientConnectStatus != null){
					eventClientConnectStatus(ClientStatus.Connect, client);
				}
				*/
				clients.Add(client);
			}

            //when connected - create a thread to handle communication with a connected client
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            //pass the tcpclient object returned by the accepttcpclient call to our new thread
            clientThread.Start(client);
        }
    }


    private void HandleClientComm(object client)
    {
        TcpClient tcpClient = (TcpClient)client; //cast client as a tcpclient object
        //because the parameterizedthreadstart delegate can only accept object types.

        //get the network stream from the tcpclient - used for reading
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        //sit in a true loop reading information from the client
        while (true)
        {
            bytesRead = 0;

            try
            {
                //block until a client sends a message and is received
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch (Exception ex)
            {
                //a socket error has occurred
                throw ex;
                //MessageBox.Show(ex.Message);
                //break;
            }

            if (bytesRead == 0)
            {
                //the client has disconnected from the server
                //MessageBox.Show("The client has disconnected from the server.");
				lock (this.clients)
				{
					//statusQueue.Enqueue( ClientStatus.Disconnect );
					statusQueue.Enqueue( new ConnectClient(tcpClient, ClientStatus.Disconnect) );
					/*
					if(eventClientConnectStatus != null){
						eventClientConnectStatus(ClientStatus.Disconnect, tcpClient);
					}
					*/
					this.clients.Remove(tcpClient);
				}

                break;
            }

            //message has successfully been recieved
            ASCIIEncoding encoder = new ASCIIEncoding();
            string smessage = encoder.GetString(message, 0, bytesRead);
            if (this.MessageReceived != null)
                this.MessageReceived(smessage); // dispatch
			
			/*
			// callbackMsg
	            byte[] buffer = encoder.GetBytes(sendStr + TCPStatus.END_Code);
	            clientStream.Write(buffer, 0, buffer.Length);
	            clientStream.Flush();
	            */
        }

		//close
		tcpClient.Close();
    }
	
	public int GetClientCount()
	{
		return clients.Count;
	}

	void OnDisable()
	{
		DeInit();
	}
	void OnApplicationQuit()
    {
		DeInit();
	}

	//
	public void Send(string msgStr){
		foreach (TcpClient client in this.clients)
		{
			Write(client, msgStr);
		}
	}
	private void Write(TcpClient tcpClient, string data)
	{
		System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
		byte[] bytes = encoder.GetBytes(data);
		Write(tcpClient, bytes);
	}
	private void Write(TcpClient tcpClient, byte[] bytes)
	{
		NetworkStream networkStream = tcpClient.GetStream();
		networkStream.BeginWrite(bytes, 0, bytes.Length, WriteCallback, tcpClient);
	}
	private void WriteCallback(IAsyncResult result)
	{
		TcpClient tcpClient = result.AsyncState as TcpClient;
		NetworkStream networkStream = tcpClient.GetStream();
		networkStream.EndWrite(result);
	}

	//
	void Update()
	{
		lock(statusQueue.SyncRoot){
			if(statusQueue.Count > 0){
				ConnectClient connectClient = (ConnectClient)(statusQueue.Dequeue());
				ClientStatus status = connectClient.status;
				TcpClient tcpClient = connectClient.client;
				//ClientStatus status = (ClientStatus)(statusQueue.Dequeue());
				
				if(eventClientConnectStatus != null){
					if(status == ClientStatus.Connect){
						eventClientConnectStatus(ClientStatus.Connect, tcpClient);
					}
					if(status == ClientStatus.Disconnect){
						eventClientConnectStatus(ClientStatus.Disconnect, tcpClient);
					}
				}
			}
		}
	}
}

