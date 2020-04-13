using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour, IPanel
{
	public Text informationText;
//	public GameObject overviewContainer;
	private void OnEnable()
	{
		informationText.text = "YOUR NAME IS " + UIManager.Instance.activeCase.name;
	}
	public void ProcessInfo()
	{
//		overviewContainer.SetActive(true);
	}
}
