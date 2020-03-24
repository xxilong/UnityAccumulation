using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class Util {

	/// <summary>
	/// 获取本机ip地址
	/// </summary>
	/// <returns>The address I.</returns>
	public static string GetAddressIP()  
	{ 
		string AddressIP = string.Empty;  
		#if UNITY_EDITOR  
		foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)  
		{  
			if (_IPAddress.AddressFamily.ToString() == "InterNetwork")  
			{  
				AddressIP = _IPAddress.ToString();  
			}  
		}  
		#else
		NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface adapter in adapters) {  
		if (adapter.Supports (NetworkInterfaceComponent.IPv4)) {  
		UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties ().UnicastAddresses;  
		if (uniCast.Count > 0) {  
		foreach (UnicastIPAddressInformation uni in uniCast) {  
		//得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4  
		if (uni.Address.AddressFamily == AddressFamily.InterNetwork) {  
		AddressIP = uni.Address.ToString ();  
		}  
		}  
		}  
		}  
		}
		#endif  
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
	/// 将int数值转换为占四个字节的byte数组，本方法适用于(低位在前，高位在后)的顺序。 和bytesToInt（）配套使用 
	/// </summary>
	/// <returns>byte数组.</returns>
	/// <param name="value">要转换的int值.</param>
	public static byte[] intToBytes( int value )   
	{   
		byte[] src = new byte[4];  
		src[3] =  (byte) ((value>>24) & 0xFF);  
		src[2] =  (byte) ((value>>16) & 0xFF);  
		src[1] =  (byte) ((value>>8) & 0xFF);    
		src[0] =  (byte) (value & 0xFF);                  
		return src;   
	} 
		
	/// <summary>
	///  byte数组中取int数值，本方法适用于(低位在前，高位在后)的顺序，和和intToBytes（）配套使用 
	/// </summary>
	/// <returns>int数值</returns>
	/// <param name="src">byte数组 </param>
	/// <param name="offset">从数组的第offset位开始 </param>
	public static int bytesToInt(byte[] src, int offset = 0) {  
		int value;    
		value = (int) ((src[offset] & 0xFF)   
			| ((src[offset+1] & 0xFF)<<8)   
			| ((src[offset+2] & 0xFF)<<16)   
			| ((src[offset+3] & 0xFF)<<24));  
		return value;  
	}  

	/// <summary>
	/// 合并数组
	/// </summary>
	/// <param name="First">第一个数组</param>
	/// <param name="Second">第二个数组</param>
	/// <returns>合并后的数组(第一个数组+第二个数组，长度为两个数组的长度)</returns>
	public static byte[] MergerBytes(byte[] First, byte[] Second)
	{
		byte[] result = new byte[First.Length + Second.Length];
		First.CopyTo(result, 0);
		Second.CopyTo(result, First.Length);
		return result;
	}

	/// <summary>
	/// 从数组中截取一部分成新的数组
	/// </summary>
	/// <param name="Source">原数组</param>
	/// <param name="StartIndex">原数组的起始位置</param>
	/// <param name="EndIndex">原数组的截止位置</param>
	/// <returns></returns>
	public static byte[] SplitBytes(byte[] Source, int StartIndex, int Count)
	{
		try
		{
			byte[] result = new byte[Count];
			for (int i = 0; i < Count; i++)
			{
				result[i] = Source[StartIndex + i];
			}
			return result;
		}
		catch (IndexOutOfRangeException ex)
		{
			throw new Exception(ex.Message);
		}
	}



	/// <summary>
	/// 将消息序列化为二进制的方法
	/// </summary>
	/// <param name="model">要序列化的对象</param>
//	public static byte[] Serialize(SendData model)
//	{
//		try {
//			//涉及格式转换，需要用到流，将二进制序列化到流中
//			using (MemoryStream ms = new MemoryStream()) {
//				//使用ProtoBuf工具的序列化方法
//				ProtoBuf.Serializer.Serialize<SendData> (ms, model);
//				//定义二级制数组，保存序列化后的结果
//				byte[] result = new byte[ms.Length];
//				//将流的位置设为0，起始点
//				ms.Position = 0;
//				//将流中的内容读取到二进制数组中
//				ms.Read (result, 0, result.Length);
//				return result;
//			}
//		} catch (Exception ex) {
//			Debug.Log ("序列化失败: " + ex.ToString());
//			return null;
//		}
//	}

	/// <summary>
	/// 将收到的消息反序列化成对象
	/// </summary>
	/// <returns>The serialize.</returns>
	/// <param name="msg">收到的消息.</param>
//	public static SendData DeSerialize(byte[] msg)
//	{
//		try {
//			using (MemoryStream ms = new MemoryStream()) {
//				//将消息写入流中
//				ms.Write (msg, 0, msg.Length);
//				//将流的位置归0
//				ms.Position = 0;
//				//使用工具反序列化对象
//				SendData result = ProtoBuf.Serializer.Deserialize<SendData> (ms);
//
//				return result;
//			}
//		} catch (Exception ex) {        
//			Debug.Log("反序列化失败: " + ex.ToString());
//			return null;
//		}
//	}


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
}
