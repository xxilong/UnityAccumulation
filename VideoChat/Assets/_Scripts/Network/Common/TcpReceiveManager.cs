using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System;

public class TcpReceiveManager{
	
	private static List<byte[]> _receiveMsg = new List<byte[]> ();

    public static event Action<byte[]> receiveMsgAction;

	public static void addToReceiveMsg(byte[] msg)
	{
		_receiveMsg.Add (msg);
        if (receiveMsgAction != null) {
            receiveMsgAction(msg);
        }
	}

	public static byte[] getMsg()
	{
		if (_receiveMsg.Count >0) {
			byte[] msg = _receiveMsg [0];
			_receiveMsg.RemoveAt (0);
			return msg;
		}
		return null;
	}

	public static byte[] getLastMsg()
	{
		if (_receiveMsg.Count >= 0) {

			byte[] msg = _receiveMsg [_receiveMsg.Count - 1];
			_receiveMsg.Clear ();
			return msg;
		}
		return null;
	}
}
