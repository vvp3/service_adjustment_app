using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OverviewPanel : MonoBehaviour, IPanel
{
	public Text caseNumberTitle, nameTitle, dateTitle, locationNotes, photoNotes;
	public RawImage photoTaken;

	public void OnEnable()
	{
		caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
		nameTitle.text = UIManager.Instance.activeCase.name;
		dateTitle.text = DateTime.Today.ToString();
		locationNotes.text = "LOCATION NOTES: \n " + UIManager.Instance.activeCase.locationNotes;

		//photoTaken.texture = UIManager.Instance.activeCase.photoTaken;
		Texture2D reconstructedImage = new Texture2D(1, 1); // rebuild photo and display it
		reconstructedImage.LoadImage(UIManager.Instance.activeCase.photoTaken);
		//Texture img = (Texture)reconstructedImage;

		photoTaken.texture = (Texture)reconstructedImage;
		photoNotes.text = "PHOTO NOTES: \n " + UIManager.Instance.activeCase.photoNotes;
	}
	public void ProcessInfo()
	{

	}

}
