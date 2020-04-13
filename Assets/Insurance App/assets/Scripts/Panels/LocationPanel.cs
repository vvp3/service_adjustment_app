using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocationPanel : MonoBehaviour, IPanel
{
	public RawImage mapImg;
	public InputField mapNotes;
	public Text caseNumberTitle;
	
	public string apiKey;
	public float xC, yC;
	public int zoom, size;
	public string url = "https://maps.googleapis.com/maps/api/staticmap?";

	public void OnEnable()
	{
		caseNumberTitle.text = "CASE NUMBER " + UIManager.Instance.activeCase.caseID;
	}

	public IEnumerator Start()
	{
		if (Input.location.isEnabledByUser == true)
		{
			// Start service before querying location
			Input.location.Start();

			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			{
				yield return new WaitForSeconds(1);
				maxWait--;
			}

			// Service didn't initialize in 20 seconds
			if (maxWait < 1)
			{
				Debug.Log("Timed out");
				yield break;
			}

			if (Input.location.status == LocationServiceStatus.Failed)
			{
				Debug.Log("Unable to determine device location");
				yield break;
			}
			else
			{
				// Access granted and location value could be retrieved
				// Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
				xC = Input.location.lastData.latitude;
				yC = Input.location.lastData.longitude;
			}

			// Stop service if there is no need to query location updates continuously
			Input.location.Stop();
		}
		else Debug.Log("Location Services are not enabled");

		StartCoroutine(GetLocationRoutine());
	}

	IEnumerator GetLocationRoutine()
	{
		url = url + "center=" + xC + "," + yC + "&zoom=" + zoom + "&size=" + size + "x" + size + "&key=" + apiKey;
		/*
				using (WWW map = new WWW(url))
				{
					yield return map;

					if (map.error != null)
						Debug.LogError(map.error);

					mapImg.texture = map.texture;
				}		
		*/
		using (UnityWebRequest map = UnityWebRequestTexture.GetTexture(url))
		{
			yield return map.SendWebRequest();

			if (map.isNetworkError || map.isHttpError)
				Debug.LogError(map.error);
			else
				mapImg.texture = DownloadHandlerTexture.GetContent(map);
		}
	}

		public void ProcessInfo()
	{
		if (string.IsNullOrEmpty(mapNotes.text) == false)
			UIManager.Instance.activeCase.locationNotes = mapNotes.text;
	}
}

//https://maps.googleapis.com/maps/api/staticmap?center=x,y&zoom=zoom&size=sizexsize&key=apiKey