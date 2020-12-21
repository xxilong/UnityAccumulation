using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DataManager{
	
	public static List<MessageData> receiveMsg = new List<MessageData> ();
	public static List<MessageData> sendMsg = new List<MessageData> ();

	public delegate void ReceiveMsg(MessageData msg);
	public static ReceiveMsg ReceiveMsgEvent;

	public static void addToReceiveMsg(MessageData msg)
	{
		receiveMsg.Add (msg);
	}

	public static void addToSendMsg(MessageData msg)
	{
		sendMsg.Add (msg);
	}

	public static MessageData getSendMsg()
	{
		int l = sendMsg.Count;
		Debug.Log (l);
		if (l > 0) {
			MessageData res = sendMsg[0];
			sendMsg.Remove (sendMsg[0]);
			return res;
		}
		return null;
	}

	public static void disposeReceiveMsg()
	{
		for(int i = 0,l = receiveMsg.Count;i<l;i++)
		{
			if (ReceiveMsgEvent != null) {
				ReceiveMsgEvent (receiveMsg [i]);
			}
		}
		receiveMsg = new List<MessageData> ();
	}

	public static MessageData getReceiveMsg()
	{
		if (receiveMsg.Count > 0) {
			MessageData res = receiveMsg[0];
			receiveMsg.RemoveAt (0);
			return res;
		}
		return null;
	}

	public static byte[] MessageToBuffer(MessageData msg)
	{
		byte[] buffer = Util.Compress( Util.Serialize (msg));//将类转换为二进制  
		byte[] head = Util.intToBytes(buffer.Length);
		List<byte> sendBuffer = new List<byte>();
		sendBuffer.AddRange(head);
		sendBuffer.AddRange(buffer);
		return sendBuffer.ToArray ();
	}

	public static MessageData BufferToMessage(byte[] bytes)
	{
		byte[] recvBytesBody = Util.Decompress (bytes);
		MessageData msg = (MessageData) Util.Deserialize(recvBytesBody);
		return msg;
	}
}
