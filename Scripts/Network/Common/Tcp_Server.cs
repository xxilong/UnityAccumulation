using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System;

public class Tcp_Server : MonoBehaviour {

	private static int tokenIndex = 0;

	//配置相关;
	private string _ip;
	//[SerializeField]
	private int _port = 50001;
	private Socket _server;
	private Thread _acceptClientConnectThread;
	private Dictionary<string, Tcp_Connect> _clientDictionary =new Dictionary<string, Tcp_Connect>();
	private bool _isActive = true;

	// Use this for initialization
	void Awake () {
		_ip = Tcp_Util.GetAddressIP ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnApplicationQuit()
	{
		Close ();
	}

	/// <summary>
	/// 启动服务器=建立流式套接字+配置本地地址;
	/// </summary>
	public void StartUp()
	{
		try
		{
			_isActive = true;
			_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Debug.Log("ip   " + _ip + ":" + _port);

			EndPoint endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
			_server.Bind(endPoint);
			_server.Listen(30);
			Debug.Log("Socket服务器监听启动......");
			_acceptClientConnectThread = new Thread(AcceptClientConnect);
			_acceptClientConnectThread.Start();
		}
		catch (Exception e)
		{

			Debug.Log (e.Message);
			Close ();
		}
	}

	/// <summary>
	/// Accept Client Connect接受客户端连接;
	/// </summary>
	/// 
	public static bool isClient;
	public void AcceptClientConnect()
	{
		Tcp_Connect tc;
		string token;
		while (_isActive) {
			try {
				//接受客户端连接，接一次执行一次, 返回值为客户端的套接字;
				Socket clientSocket = _server.Accept ();
				Debug.Log("客户端已连接！local:" + clientSocket.LocalEndPoint + "<---Client:" + clientSocket.RemoteEndPoint);  
				isClient=true;
				//维护一个客户端列表;
				tokenIndex++;
				token = "client_" + tokenIndex;
				tc = new Tcp_Connect (clientSocket, RemoveClient, token);
				tc.StartUp ();
				_clientDictionary.Add (token, tc);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
		}
		_acceptClientConnectThread.Abort();
	}
		
	/// <summary>
	/// 给某个客户端发消息
	/// </summary>
	/// <param name="msg">Message.</param>
	/// <param name="client">Client.</param>
	public void Send(byte[] msg , string key)
	{
		if (_clientDictionary.ContainsKey (key)) {
			_clientDictionary [key].Send (msg);
		}
	}

	/// <summary>
	/// 给所有人发消息
	/// </summary>
	/// <param name="str">String.</param>
	public void SendAll(byte[] msg)
	{
		var enumerator = _clientDictionary.GetEnumerator();
		while (enumerator.MoveNext ()) {
			Send (msg, enumerator.Current.Key);
		}
	}

	public void RemoveClient(string token)
	{
		isClient = false;
		//Debug.Log ("duankai:==>"+token);
		if (_clientDictionary.ContainsKey(token)) {
			_clientDictionary ["token"].Close ();
			_clientDictionary.Remove (token);
		}
	}

	//最后关闭套接字;
	public void Close()
	{
		_isActive = false;
		//关闭客户端套接字相关;
		var enumerator = _clientDictionary.GetEnumerator();
		while (enumerator.MoveNext ()) {
			enumerator.Current.Value.Close ();
		}
		//列表清除;
		_clientDictionary.Clear();
		//关闭服务器套接字相关;
		if (_server != null) {
			_server.Close();
		}
	}
}
