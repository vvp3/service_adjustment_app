using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
	public RawImage photoTaken;
	public InputField photoNotes;
	public Text caseNumberTitle;
	public GameObject overviewPanel;

	private string imgPath; 

	public void OnEnable()
	{
		caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
	}
	public void ProcessInfo()
	{
		//UIManager.Instance.activeCase.photoTaken = photoTaken.texture;
		//Texture2D convertedPhoto = photoTaken.texture as Texture2D; // convert texture to texture2d
		//byte[] imgData = convertedPhoto.EncodeToPNG(); // convert to bytes

		byte[] imgData = null;

		if (string.IsNullOrEmpty(imgPath) == false)
		{
			Texture2D img = NativeCamera.LoadImageAtPath(imgPath, 512, false); // create 2D Texture + apply texture from image path
			imgData = img.EncodeToPNG(); // convert to bytes the encoded image
		}

		UIManager.Instance.activeCase.photoTaken = imgData;

		UIManager.Instance.activeCase.photoNotes = photoNotes.text;
		overviewPanel.SetActive(true);
	}

	public void TakePictureButton()
	{
		TakePicture(512);
	}

	private void TakePicture(int maxSize)
	{
		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create a Texture2D from the captured image
				Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				photoTaken.texture = texture;
				photoTaken.gameObject.SetActive(true);
				imgPath = path;
				
			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
	}

}
