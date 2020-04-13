using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("UI M is null");

            return _instance;
        }
    }

    public Case activeCase;
    public ClientInfoPanel clientInfoPanel;
    public GameObject borderPanel;

    private void Awake()
    {
        _instance = this;
    }

    public void CreateNewCase()
    {
        activeCase = new Case();

        int xR = UnityEngine.Random.Range(0, 1000);
        activeCase.caseID = "" + xR;

        clientInfoPanel.gameObject.SetActive(true);
        borderPanel.SetActive(true);

    }

    public void Submit()
    {
        Case awsCase = new Case();
        awsCase.caseID = activeCase.caseID;
        awsCase.name = activeCase.name;
        awsCase.date = activeCase.date;
        awsCase.locationNotes = activeCase.locationNotes;
        awsCase.photoTaken = activeCase.photoTaken;
        awsCase.photoTaken = activeCase.photoTaken;

        BinaryFormatter bf = new BinaryFormatter();
        string filePath = Application.persistentDataPath + "/case#" + awsCase.caseID + ".dat";
        //string filePath = Application.persistentDataPath + "/case#" + awsCase.caseID;
        FileStream file = File.Create(filePath);
        bf.Serialize(file, awsCase);
        file.Close();

        //Debug.Log("Application data path: " + Application.persistentDataPath);
        Debug.Log("filepath: " + filePath);

        AWSManager.Instance.UploadToS3(filePath, awsCase.caseID);

        Debug.Log("go to function upload to s3 with case#" + awsCase.caseID);

    }


}
