using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Case
{
	public string caseID, name, date, locationNotes, photoNotes;
	//public Texture photoTaken;
	public byte[] photoTaken;
}
