using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class AWSManager : MonoBehaviour
{
    private string S3BucketName = "appcasefiles";

    private static AWSManager _instance;
    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("AWS MANAGER is null");
            }
            return _instance;
        }
    }


    public string S3Region = RegionEndpoint.EUCentral1.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private AmazonS3Client _s3Client;
    public AmazonS3Client S3Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(new CognitoAWSCredentials(
                "eu-central-1:41fdc6a2-cac8-4e0f-8ff1-3d745a81ccf5", // Identity Pool ID
                RegionEndpoint.EUCentral1// Region
                ), _S3Region);
            }

            return _s3Client;
        }
    }

    private void Awake()
	{
        _instance = this;
        
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        
        // CognitoAWSCredentials credentials = new CognitoAWSCredentials(
        //"eu-central-1:41fdc6a2-cac8-4e0f-8ff1-3d745a81ccf5", // Identity Pool ID
        //        RegionEndpoint.EUCentral1// Region
        //        );
        //AmazonS3Client S3Client = new AmazonS3Client(credentials, _S3Region);

        /*S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {

                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log("Bucket name:" + s3b.BucketName);
                });
            }
            else
            {
                Debug.Log("AWS ERROR :" + responseObject.Exception);
            }
        });*/
 
    }

    public void UploadToS3(string path, string caseID)
    {
        //FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Write);
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = S3BucketName,
            Key = "case#" + caseID,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        // THE CODE BELLOW IS NOT CALLED !!!! WHY ?!?!

        Debug.Log("entering S3CLIENT.POSTOBJECT");

        S3Client.PostObjectAsync(request, (responseObj) =>
        {
            Debug.Log("I AM IN S3CLIENT.POSTOBJECT");

            if (responseObj.Exception == null)
            {
                Debug.Log("object posted to bucket !!");
                SceneManager.LoadScene(0);
            }
            else
            {
                Debug.Log("Exception while posting the result object");
            }

            //stream.Close(); // by VALI !!
            Debug.Log("STREAM CLOSED !!");

        });

        Debug.Log("finished UPLOAD !!");


    }

    public void GetList(string caseNumber, Action onComplete = null)
    {
        string target = "case#" + caseNumber;

        Debug.Log("we are in GELIST void");
        
        var request = new ListObjectsRequest()
        {
            BucketName = S3BucketName
        };

        S3Client.ListObjectsAsync(request, (responseObject) => //search the object in a list of objects
        {
            if (responseObject.Exception == null)
            {
                bool caseFound = responseObject.Response.S3Objects.Any(obj => obj.Key == target);

                if (caseFound == true)
                {
                    Debug.Log("found " + target);
                    //getting the the object

                    S3Client.GetObjectAsync(S3BucketName, target, (responseObj) =>
                    {
                        // check if response stream is null
                        if (responseObj.Response.ResponseStream != null)
                        {
                            // byte array to store data from file
                            byte[] data = null;

                            // use streamreader to read response data
                            using (StreamReader reader = new StreamReader(responseObj.Response.ResponseStream))
                            {
                                // access a memory stream -- deeper level than just read text
                                using (MemoryStream memory = new MemoryStream())
                                {
                                    // populate data byte array with memstream data
                                    var buffer = new byte[512];
                                    var bytesRead = default(int);

                                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memory.Write(buffer, 0, bytesRead);
                                    }
                                    data = memory.ToArray();
                                }
                            }
                            // read data and apply it to a case(object) to be used
                            using (MemoryStream memory = new MemoryStream(data))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                Case downloadedCase = (Case)bf.Deserialize(memory);
                                Debug.Log("Downloaded case name: " + downloadedCase.name);
                                UIManager.Instance.activeCase = downloadedCase;
    
                                if (onComplete != null)
                                    onComplete();
                            }
                        }
                    });

                }
                else
                {
                    Debug.Log("Case not found !!");
                }
            }
            else
            {
                Debug.Log("Got Exception \n" + responseObject.Exception);
            }
        });
    }
}
