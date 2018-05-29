﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vuforia;
using UnityEngine.UI;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results.
/// </summary>
public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{
	//protected TrackableBehaviour mTrackableBehaviour;

	//private DefaultTrackableEventHandler TrackerScript;
	
	public ImageTargetBehaviour ImageTargetTemplate;
	private ImageTargetBehaviour imageTargetBehaviour;
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	public string mTargetMetadata = "";
	public string loadedData;
	private string jsonString;
	public string editorUrl;
	private GameObject newImageTarget;

	//Added
    //Variable "assetNames" to load all asset names in assetbundles
	static string[] assetNames;
	//Variable to Load all assets in assetbundles to "_assets"
	static UnityEngine.Object[] _assets;
	//Number of assets in AssetBundle
	int i=0;
    // Variable "asseBundle" to load asset bundle
	private AssetBundle asseBundle;
	// Variable "asset" to load prefab
	private AssetBundleRequest asset;
	//Variable "loadedAsset" to load prefab as gameobject
	private GameObject loadedAsset;	
    // AssetBundle Url
    private string url = "Load";
    private string url_loaded = "Unload";
    private bool loded;
    //download asset bundle using WWW
    private WWW www;

	public GameObject[] clone;
	private int num;

	//Added
	// ImageTracker reference to avoid lookups
    private ObjectTracker mImageTracker;
	private GameObject mParentOfImageTargetTemplate;


	// Use this for initialization
	void Start () {
		// register this event handler at the cloud reco behaviour
		/*mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();

		if (mCloudRecoBehaviour)
		{
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}*/

		/* Added ===========================================================================*/

		mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;
		
		// register this event handler at the cloud reco behaviour
        CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (cloudRecoBehaviour)
        {
            cloudRecoBehaviour.RegisterEventHandler(this);
        }
 
        // remember cloudRecoBehaviour for later
        mCloudRecoBehaviour = cloudRecoBehaviour;

		/*================================================================================== */
		
		

	}

	/*private void Update()
	{
		if (imageTargetBehaviour = null)
    	{
    	    // stop the target finder
    	    //mCloudRecoBehaviour.CloudRecoEnabled = false;
			Debug.Log("ImageTargetBehavoir is NULL!!!!!!!!!!!!!!!!!!!!!!");
  		}

	}*/


	public void OnInitialized() {

		// get a reference to the Image Tracker, remember it
        mImageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
		Debug.Log ("Cloud Reco initialized");
	}
	public void OnInitError(TargetFinder.InitState initError) {
		switch (initError)
        {
            case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
                Debug.Log ("Network Unavailable! Failed to initialize CloudReco because the device has no network connection.");
                break;
            case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
                Debug.Log ("Service Unavailable! Failed to initialize CloudReco because the service is not available.");
                break;
        }
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}
	public void OnUpdateError(TargetFinder.UpdateState updateError) {
		switch (updateError)
        {
            case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
                Debug.Log ("Authorization Error! The cloud recognition service access keys are incorrect or have expired.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_BAD_FRAME_QUALITY:
                Debug.Log ("Poor Camera Image! The camera does not have enough detail, please try again later");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
                Debug.Log ("Network Unavailable! Please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
                Debug.Log ("Authorization Error! The cloud recognition service has been suspended.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
                Debug.Log ("Request Timeout! The network request has timed out, please check your internet connection and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
                Debug.Log ("Service Unavailable! The service is unavailable, please try again later.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
                Debug.Log ("Clock Sync Error! Please update the date and time and try again.");
                break;
            case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
                Debug.Log ("Unsupported Version! The application is using an unsupported version of Vuforia.");
                break;
        }
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
	}

	public void OnStateChanged(bool scanning) {
		mIsScanning = scanning;
		if (scanning)
		{
			// clear all known trackables
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);

			if (mCloudRecoBehaviour.CloudRecoInitialized && !mCloudRecoBehaviour.CloudRecoEnabled)
		{
 			mCloudRecoBehaviour.CloudRecoEnabled = true;
		}
		}
	}

	// Here we handle a cloud target recognition event
	public void  OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		
		int len1 = clone.Length;

		Debug.Log("new Target Found");

		/*for(i=0; i<num; i++)
            {
				Destroy(clone[i]);
			}*/
		//imageTargetBehaviour = new ImageTargetBehaviour();
		newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;

		//ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate); 

		 // This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
        // Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
        // or to create a new ImageTargetBehaviour for each new result
 
        // Vuforia will return a new object with the right script automatically if you use
        // TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)
         
        
         
        // enable the new result with the same ImageTargetBehaviour:
       // ImageTargetBehaviour imageTargetBehaviour = mImageTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate);
		
	//	if (ImageTargetTemplate) {
			// enable the new result with the same ImageTargetBehaviour:
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			imageTargetBehaviour =	(ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget); //mParentOfImageTargetTemplate
	//	}

		//Check if the metadata isn't null
		 if(targetSearchResult.MetaData == null)
        {
            return;
        }
		// do something with the target metadata
		mTargetMetadata = targetSearchResult.MetaData;		
		
		/*if (imageTargetBehaviour != null)
    	{
    	    // stop the target finder
    	    mCloudRecoBehaviour.CloudRecoEnabled = false;
  		}*/

		/*if (imageTargetBehaviour != null)
        {
            // stop the target finder
            mCloudRecoBehaviour.CloudRecoEnabled = false;
             
            // Calls the TargetCreated Method of the SceneManager object to start loading
            // the BookData from the JSON
           // mContentManager.TargetCreated(targetSearchResult.MetaData);
        }*/
		url = ReadMeta();
		Debug.Log("Downloading from yes yes worked:"+ url);
		StartCoroutine("DownloadBundle");
	}


	IEnumerator DownloadBundle (){

		i = 0;

		if(www != null)
		{
			www.Dispose();
       		www = null;
			asseBundle.Unload(false);
		}
		// TrackerScript = ImageTargetTemplate.GetComponent<DefaultTrackableEventHandler>();
		// mTrackableBehaviour = TrackerScript.mTrackableBehaviour;

        // if (bundleHolder.transform.childCount > 0)
        //     Destroy(bundleHolder.transform.GetChild(0).gameObject);

        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;
 
        // Update url_loaded to prevent downloading assets again for the reco 
        url_loaded = url;
        //download AssetBundle
        www = new WWW(url);
        // using (www = new WWW(url)) -- Or use this
        using (www)
        {
            
            //wait for download
            yield return www;

            Debug.Log ("Loaded ");
            if (www.error != null)
            throw new Exception ("WWW download had an error: " + www.error);

            asseBundle = www.assetBundle;

             //load all assets
            _assets = www.assetBundle.LoadAllAssets();

            //load all asset names
            assetNames = www.assetBundle.GetAllAssetNames();
			num = _assets.Length;
            clone = new GameObject[num];   
            foreach (UnityEngine.Object eachAsset in _assets)
            {
                // request prefab to load
                asset = asseBundle.LoadAssetAsync<GameObject>(assetNames[i]);
                yield return asset;
                //GameObject yes = new GameObject(); 
                clone[i] = (GameObject)Instantiate(asset.asset,transform.position, transform.rotation);
				//clone[i] = yes;
				clone[i].transform.parent = newImageTarget.transform;
                Debug.Log(clone[i]);

				/*GameObject yes = asset.asset as GameObject;
				Rigidbody rig;
				Instantiate(yes);*/
				//clone[i] = (GameObject)Instantiate(yes);
				//clone[i].transform.parent = newImageTarget.transform;
				//GameObject yesins = (GameObject)Instantiate(yes);
				//rig = yesins.GetComponent<Rigidbody>();
				//yesins.transform.parent = newImageTarget.transform;

                /*Important to transform localscale before transform.parent */
                //clone[i].transform.parent = newImageTarget.transform;
                //Instantiate(asseBundle.LoadAsset(assetNames[i]));
                i += 2;
            }   
        }

		//asseBundle.Unload(false);
    }
 

	public string ReadMeta()
	{
		string returnMeta = mTargetMetadata;
		//Debug.Log ("Meta Data:" + returnMeta);
		MetaDataFields loadedData = JsonUtility.FromJson<MetaDataFields>(returnMeta);
		//Debug.Log(loadedData.Editor);
		// Debug.Log(loadedData.Android);
		// Debug.Log(loadedData.iOS);
		// PLatform Specific runtime
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return loadedData.iOS;
		}

		else if (Application.platform == RuntimePlatform.Android)
		{
			return loadedData.Android;
		}

		else //(Application.platform == RuntimePlatform.Android)
		{		
			editorUrl = loadedData.Editor;
			return loadedData.Editor;
		}

		//return returnMeta;
		
	}

	[System.Serializable]
	public class MetaDataFields
	{
		public string Editor;
		public string Android;
		public string iOS;
	}
}
