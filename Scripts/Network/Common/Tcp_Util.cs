using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class Tcp_Util {

    /// <summary>
    /// 获取本机ip地址
    /// </summary>
    /// <returns>The address I.</returns>
    public static string GetAddressIP() {
        string AddressIP = string.Empty;
        #if UNITY_EDITOR
        foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
            if (_IPAddress.AddressFamily.ToString() == "InterNetwork") {
                AddressIP = _IPAddress.ToString();
            }
        }
        #else
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in adapters) {
            if (adapter.Supports(NetworkInterfaceComponent.IPv4)) {
                UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                if (uniCast.Count > 0) {
                    foreach (UnicastIPAddressInformation uni in uniCast) {
                        //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4  
                        if (uni.Address.AddressFamily == AddressFamily.InterNetwork) {
                            AddressIP = uni.Address.ToString();
                        }
                    }
                }
            }
        }
        #endif
        //Debug.Log ("ip:=="+AddressIP);
        return AddressIP;
    }

    /// <summary>
    /// 压缩字节数组
    /// </summary>
    /// <param name="str"></param>
    public static byte[] Compress(byte[] inputBytes)
	{    
//		return inputBytes;
//		Debug.Log("原始数据长度是: " + inputBytes.Length);
		using (MemoryStream ms = new MemoryStream ()) {
			using (GZipOutputStream gzip = new GZipOutputStream (ms)) {
				gzip.Write(inputBytes, 0, inputBytes.Length);
				gzip.Close();
//				Debug.Log("压缩后的数据长度是: " + ms.ToArray().Length);
				return ms.ToArray();
			}
		}
	}

	/// <summary>
	/// 解压缩字节数组
	/// </summary>
	/// <param name="str"></param>
	public static byte[] Decompress(byte[] inputBytes)
	{
//		return inputBytes;
		using (GZipInputStream gzi = new GZipInputStream (new MemoryStream (inputBytes))) {
			using (MemoryStream re = new MemoryStream ()) {
				int count=0;
				byte[] data=new byte[1024000];
				while ((count = gzi.Read(data, 0, data.Length)) != 0)
				{
					re.Write(data,0,count);
				}
				byte[] depress = re.ToArray();
				return depress;
			}
		}
	}
		
	/// <summary>
	/// 将int数值转换为占四个字节的byte数组，本方法适用于(高位在前，低位在后)的顺序。 和bytesToInt（）配套使用 
	/// </summary>
	/// <returns>byte数组.</returns>
	/// <param name="value">要转换的int值.</param>
	public static byte[] intToBytes( int value )   
	{   
		byte[] src = new byte[4];  
		src[0] =  (byte) ((value>>24) & 0xFF);  
		src[1] =  (byte) ((value>>16) & 0xFF);  
		src[2] =  (byte) ((value>>8) & 0xFF);    
		src[3] =  (byte) (value & 0xFF);                  
		return src;   
	} 
		
	/// <summary>
	///  byte数组中取int数值，本方法适用于(高位在前，低位在后)的顺序，和和intToBytes（）配套使用 
	/// </summary>
	/// <returns>int数值</returns>
	/// <param name="src">byte数组 </param>
	/// <param name="offset">从数组的第offset位开始 </param>
	public static int bytesToInt(byte[] src, int offset = 0) {  
		int value;    
		value = (int) ((src[offset+3] & 0xFF)   
			| ((src[offset+2] & 0xFF)<<8)   
			| ((src[offset+1] & 0xFF)<<16)   
			| ((src[offset] & 0xFF)<<24));  
		return value;  
	}  

	public static byte[] StringToBytes(string msg)
	{
		return Encoding.UTF8.GetBytes (msg);
	}

	public static string BytesToString(byte[] bytes)
	{
		return Encoding.UTF8.GetString (bytes);
	}

	public static byte[] Serialize(object data){
		BinaryFormatter formatter = getBinaryFormatter ();
		MemoryStream rems = new   MemoryStream ();
		formatter.Serialize (rems, data);
		return  rems.GetBuffer ();
	}

	public static object   Deserialize(byte[]   data){
		BinaryFormatter formatter = getBinaryFormatter ();
		MemoryStream rems = new   MemoryStream (data);
		object res =  formatter.Deserialize (rems);
		return res;
	}

	private static BinaryFormatter bf;

	public static BinaryFormatter getBinaryFormatter()
	{
		if (bf == null) {
			bf = new BinaryFormatter();
			SurrogateSelector ss = new SurrogateSelector();
			bf.SurrogateSelector = ss;
		}

		return bf;
	}

	public static MessageData BufferToMessage(byte[] bytes)
	{
		byte[] recvBytesBody = Tcp_Util.Decompress (bytes);
		MessageData msg = (MessageData) Tcp_Util.Deserialize(recvBytesBody);
		return msg;
	}


	public static byte[] MessageToBuffer(MessageData msg)
	{
		byte[] buffer =  Tcp_Util.Compress( Tcp_Util.Serialize (msg));//将类转换为二进制  
		byte[] head = Tcp_Util.intToBytes(buffer.Length);
		List<byte> sendBuffer = new List<byte>();
		sendBuffer.AddRange(head);
		sendBuffer.AddRange(buffer);
		return sendBuffer.ToArray ();
	}
}
