using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAPI {

    public static MessageData DeserializeMessage(byte[] bytes) {
        byte[] recvBytesBody = Tcp_Util.Decompress(bytes);
        MessageData msg = (MessageData)Tcp_Util.Deserialize(recvBytesBody);
        return msg;
    }

    public static byte[] SerializeMessage(MessageData msg) {
        byte[] buffer = Tcp_Util.Compress(Tcp_Util.Serialize(msg));//将类转换为二进制  
        byte[] head = Tcp_Util.intToBytes(buffer.Length);
        List<byte> sendBuffer = new List<byte>();
        sendBuffer.AddRange(head);
        sendBuffer.AddRange(buffer);
        return sendBuffer.ToArray();
    }
}
