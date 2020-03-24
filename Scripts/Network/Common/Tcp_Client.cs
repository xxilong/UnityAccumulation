using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

public class Tcp_Client : MonoBehaviour {

	private Tcp_Connect tc;
	private string _ip;
	private int _port;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit()
	{
		Close ();
	}

	//套接字;
	private Socket _client;

	public void StartUp(string ip,int port)
	{
		this._ip = ip;
		this._port = port;
		try {
			_client = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_client.Connect (ip, port);
			tc = new Tcp_Connect (_client, ClientDisabled);
			tc.StartUp();
		} catch (Exception e) {
			Debug.Log (e.Message);
			Close ();
		}
	}

	public void Send(byte[] msg)
	{
        if (tc == null)
            return;
		tc.Send (msg);	
	}

	private void ClientDisabled(string token)
	{
		Close ();
		StartUp (_ip, _port);
	}

	/// <summary>
	///   客户端 套接字关闭;
	/// </summary>
	public void Close()
	{
		if (tc != null) {
			tc.Close ();
		}
	}
}
