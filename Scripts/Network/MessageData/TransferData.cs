using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TransferData : MessageData {
	public string text;
	public string posX;
	public string posY;
	public string posZ;
	public string rtX;
	public string rtY;
	public string rtZ;
	public string scX;
	public string scY;
	public string scZ;
    public string color; 
	public string parent;
    public string enable;

	public TransferData()
	{
		id = "TextData";
		posX = "0";
		posY = "0";
		posZ = "0";
		rtX = "0";
		rtY = "0";
		rtZ = "0";
		scX = "0";
		scY = "0";
		scZ = "0";
        color = "FFFFEE";
		parent = "";
        enable = "true";
	}
}
