using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System;


public class Tcp_Connect {
	public string token{ private set; get;}

	const int packageMaxLength = 102400;
	private int bodyLength = 0;

	private Socket _client;
	private Action<string> _removeAcction;
	private int _sendType;

	private Thread _receiveTcpMsg;
	private Thread _sendTcpMsg;

	private bool _isActive = true;

	private List<byte[]> _sendMessageList;

	public Tcp_Connect (Socket client, Action<string> removeAcction, string token = "")
	{
		this.token = token;
		this._client = client;
		this._removeAcction = removeAcction;
		_sendMessageList = new List<byte[]> ();
	}

	/// <summary>
	/// 启动客户端连接操作
	/// </summary>
	public void StartUp()
	{
		_isActive = true;
		_receiveTcpMsg = new Thread (ReceiveTcpMsg);
		_receiveTcpMsg.Start ();

		_sendTcpMsg = new Thread (SendTcpMsg);
		_sendTcpMsg.Start ();
	}

	/// <summary>
	/// 添加消息列表;
	/// </summary>
	/// <param name="str"></param>
	public void Send(byte[] msg)
	{
		if (msg != null && _isActive) {
			
			_sendMessageList.Add (msg);
		}
	}

	/// <summary>
	/// 关闭客户端连接
	/// </summary>
	public void Close()
	{
		_isActive = false;
		if (_client != null) {
			_client.Close ();
			_client = null;
		}
	}

	private void SendTcpMsg()
	{
		while (_isActive) {
			try {
				if(_sendMessageList.Count > 0)
				{
					byte[] sendBuffer = _sendMessageList[0];
					if(sendBuffer == null)
					{
						continue;
					}
					Debug.Log ("发出消息,长度为" + sendBuffer);
					_client.Send(sendBuffer);
					_sendMessageList.RemoveAt(0);
				}
			} catch (Exception e) {
				Debug.Log (e.Message);
				_removeAcction (this.token);
				break;
			}
		}
		_sendTcpMsg.Abort ();
		_sendTcpMsg = null;
	}
		
	private void ReceiveTcpMsg()
	{
		while (_isActive) {
			try {
				byte[] receiveHead = GetBytesReceive (4);
				bodyLength = Tcp_Util.bytesToInt (receiveHead);
				Debug.Log (" 消息长度为：" + bodyLength);
				byte[] buffer = GetBytesReceive (bodyLength);
				bodyLength = 0;
				TcpReceiveManager.addToReceiveMsg(buffer);
			} catch (Exception e) {
				Debug.Log (e.Message);
				_removeAcction (this.token);
				break;
			}
		}
		_receiveTcpMsg.Abort ();
		_receiveTcpMsg = null;
	}

	byte[] GetBytesReceive(int length)
	{
		byte[] recvBytes = new byte[length];
		while (length > 0) {
			byte[] receiveBytes = new byte[length < packageMaxLength ? length : packageMaxLength];
			int iBytesBody = 0;
			if (length >= receiveBytes.Length)
				iBytesBody = _client.Receive (receiveBytes, receiveBytes.Length, 0);
			else
				iBytesBody = _client.Receive (receiveBytes, length, 0);
			receiveBytes.CopyTo (recvBytes, recvBytes.Length - length);
			length -= iBytesBody;
		}
		return recvBytes;
	}
}
