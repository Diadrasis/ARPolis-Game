using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Stathis;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Linq;

public class OnStart : MonoBehaviour {

	public string _bundleVersionCode = "v1.0.1";

	public bool fakeAgoraPro=false;

	public bool showPoiInfo = true;
	public bool testON = true;
	public bool useMapFilters;
	public bool useGpsInFreeVersion;
	public string allow_Scene_Gps;
	public bool setGPS_On;
	public bool useCharacterController;
	public bool isGameMode=true;


	[Range(1f,10f)]
	public float waitGredits = 4f;

	[Range(0.1f,5f)]
	public float speedFade=0.5f;

	public Texture[] introEikones;

	//get gredits panel fron 2d canvas
	GameObject introPanel ;
	GameObject canvas2d;

	void Awake(){
		if(Diadrasis.Instance.isStart==0)
		{
			//show canvas for all 2d alements only once
			canvas2d = Instantiate(Resources.Load("prefabs/FINAL/ui/Kanvas_2D")) as GameObject;

			PlayerPrefs.SetInt("useOnSiteMode", 111);
			Diadrasis.Instance.useOnSiteMode = true;
			Diadrasis.Instance.isGameMode = isGameMode;
		}	
	}

	IEnumerator Start () {

		#if UNITY_EDITOR

		//PlayerPrefs.DeleteAll();

		Debug.LogWarning("SET LOAD PRIORITY HERE!!");

		if(fakeAgoraPro){
			PlayerPrefs.SetInt("useOnSiteMode",111);
		}

//		Action sayHello = () => { Debug.Log("Hello"); };
//		sayHello();
//		
//		Action<int> sendToLog = (arg) => { Debug.Log(arg); };
//		sendToLog(5);
//		sendToLog(-10);
//		sendToLog(42);

		#else
		testON=false;
		#endif


		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

		//Shop.Instance.Init();

		if(Diadrasis.Instance.isStart==0)
		{
			//ZPlayerPrefs.Initialize("!diadrasis$129", "StaTh1s75");
			
			//get-read data xml 
			Globals.Instance.Init();

			//get menu texts from terms xml
			appData.Init();

			appData.InitCoupons();

			//call manager
			Diadrasis.Instance.Init();

			#if UNITY_EDITOR
			//are you in test mode?
			Diadrasis.Instance.isTesting = testON;
			Diadrasis.Instance.gpsIsOn = setGPS_On;
			Diadrasis.Instance.useCharacterControllerToPerson = useCharacterController;
//			Gps.Instance.isLocationFound=true;
			#endif

//			if(testON){
			Diadrasis.Instance.showPoiInfo = showPoiInfo;
//			}

//			if (canvas2d == null) {
//				Debug.LogError ("NULL canvas2d");
//			}
//
//			if (canvas2d.GetComponent<RectTransform>() == null) {
//				Debug.LogError ("NULL RectTransform");
//			}

			//assign canvas2d 
			Diadrasis.Instance.canvasMainRT=canvas2d.GetComponent<RectTransform>();

//			if (Diadrasis.Instance.canvasMainRT == null) {
//				Debug.LogError ("NULL canvasMainRT");
//			}

			Diadrasis.Instance.appVersion = _bundleVersionCode;

			//allow one scene for on site mode in free version
			//the name of the scene
			Diadrasis.Instance.allowSceneGps = allow_Scene_Gps;

			//create an object holder for all scenes
			GameObject father = new GameObject();
			father.name = "-------EVERYWHERE_OBJECTS-------";

			//set to manager the object that will be in all scenes
			Diadrasis.Instance.objDontDestroy=father;

			//check sensors and instatiate person with proper scripts
			Diadrasis.Instance.CheckSensors();

			canvas2d.name="CANVAS_2D";
			//set parent for all scenes
			canvas2d.transform.SetParent(father.transform);

			//get scripts
			Diadrasis.Instance.menuUI = canvas2d.GetComponent<MenuUI>();
			Diadrasis.Instance.animControl = canvas2d.GetComponent<AnimControl>();
			//instatiate touch events manager
			GameObject touchManager = Instantiate(Resources.Load("prefabs/FINAL/tools/LeanTouch")) as GameObject;
			touchManager.name="touchManager";
			//set parent for all scenes
			touchManager.transform.SetParent(father.transform);

			//Audio Player
			GameObject audioPlayer = Instantiate(Resources.Load("prefabs/FINAL/audio/AudioPlayer")) as GameObject;
			audioPlayer.name="AudioPlayer";
//			Diadrasis.Instance.akoi = audioPlayer.GetComponent<AudioListener>();
			Diadrasis.Instance.ixos = audioPlayer.GetComponent<AudioSource>();
			Diadrasis.Instance.ixos.clip=null;
			Diadrasis.Instance.ixos.playOnAwake=false;
			Diadrasis.Instance.ixos.Stop();
			//set parent for all scenes
			audioPlayer.transform.SetParent(father.transform);

			if(Application.platform==RuntimePlatform.WindowsEditor){
				Diadrasis.Instance.useOnSiteMode = useGpsInFreeVersion;
			}else{
				
				if(Diadrasis.Instance.appEntrances==1){
					PlayerPrefs.SetInt("useOnSiteMode",0);
					PlayerPrefs.Save();
					//Diadrasis.Instance.useOnSiteMode =false;
					
					Diadrasis.Instance.menuUI.warningsUI.HideShopButton();

					Diadrasis.Instance.menuUI.warningsUI.HideCommentButton();
					
					
					#if UNITY_EDITOR
					Debug.LogWarning("RESTORE PURCHASE IF USER BOUGHT PRO EDITION FROM OTHER DEVICE!!!");
					#endif

					//Shop.Instance._shopManager.Restore();
					
				}else{
					//Diadrasis.Instance.menuUI.warningsUI.CheckPurchases();
				}
			}

			//show gredits panel
			if(!testON)
			{
				//get gredits panel fron 2d canvas
				introPanel = Diadrasis.Instance.menuUI.greditsPanel;
				//hide menu button
				Diadrasis.Instance.menuUI.MenuButtonsHide ();
				//enable panel
				introPanel.SetActive(true);

				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inGredits);
				
			}else{
				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);
				//show menu button
				Diadrasis.Instance.menuUI.MenuButtonsShow();
			}

			//show menu map
			Diadrasis.Instance.menuUI.menuXartisPanel.SetActive(true);
			//get menu map script and initialize
			Diadrasis.Instance.xartisMenu = canvas2d.GetComponent<XartisMenu>();
			Diadrasis.Instance.xartisMenu.enabled = true;
			Diadrasis.Instance.xartisMenu.Init();

			//show menu button
			//Diadrasis.Instance.menuUI.MenuButtonsShow();

			//get - set device languange
			if(Application.systemLanguage==SystemLanguage.Greek)
			{
				//to SettingsUI
				Diadrasis.Instance.menuUI.settingsUI.langPanel.selectLang.value=0;
				Diadrasis.Instance.menuUI.settingsUI.SetLanguange(0f);
			}
			else
			{
				//to SettingsUI
				Diadrasis.Instance.menuUI.settingsUI.langPanel.selectLang.value=1;
				Diadrasis.Instance.menuUI.settingsUI.SetLanguange(1f);
			}


//			Diadrasis.Instance.PluginsInit();

			yield return new WaitForSeconds (1.5f);

			Diadrasis.Instance.useMapFilterForMovement = useMapFilters;

			//display build version into settings panel
			Diadrasis.Instance.menuUI.settingsUI.SetVersion(_bundleVersionCode);
			

		}
		else
		{
			Diadrasis.Instance.isStart++;


			//if(PlayerPrefs.GetInt("useOnSiteMode")!=111){
			//	if (Diadrasis.Instance.isStart == 2 && Diadrasis.Instance.appEntrances == 1) {
			//		Shop.Instance.isFromSceneAction = true;
			//	} else {
			//		if (Diadrasis.Instance.appEntrances > 1) {
			//			if (Diadrasis.Instance.isStart % 3 == 0) {
			//				Shop.Instance.isFromSceneAction = true;
			//			}
			//		}
			//	}
			//}

			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);

			//if(Diadrasis.Instance.isStart==2){
			//Diadrasis.Instance.menuUI.warningsUI.CheckPurchases();
			//}


			//get - set languange
			Diadrasis.Instance.xartisMenu.GetXmlScenes();

			if(introPanel){
				introPanel.transform.parent.gameObject.SetActive(false);
			}

			//fade out omixli
			if (!Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.activeSelf) {
				Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (true);
			}

			//show menu map
			Diadrasis.Instance.xartisMenu.enabled = true;
			Diadrasis.Instance.xartisMenu.Init();
			Diadrasis.Instance.menuUI.menuXartisPanel.SetActive(true);

			if(Diadrasis.Instance.xartisMenu == null){
				Diadrasis.Instance.xartisMenu = canvas2d.GetComponent<XartisMenu>();
			}

			//fade out omixli
			if (Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.activeSelf) {

				CanvasGroup cg = Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.GetComponent<CanvasGroup> ();

				//fade out
				while(cg.alpha>0.01f)
				{
					cg.alpha = Mathf.Lerp(cg.alpha,0f,Time.deltaTime * 4f);

					//Debug.Log (cg.alpha);

					yield return null;
				}

				//fade out full
				cg.alpha=0f;

				Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (false);
				Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.transform.SetSiblingIndex (2);
				RawImage dd = Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.GetComponent<RawImage> ();
				dd.texture = null ;
				cg.alpha = 0.75f;
			}

			//Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);

			Diadrasis.Instance.menuStatus=Diadrasis.MenuStatus.idle;

			//reset clip for other scene period if it doesnt have an intro narration
			Diadrasis.Instance.ixos.clip=null;

			/*
			if(Shop.Instance.isFromSceneAction){

				#if UNITY_EDITOR
				Debug.Log("OPEN SHOP");
				#endif


				//check if user has bought pro edition from other device
				Shop.Instance._shopManager.Restore();

				if(PlayerPrefs.GetInt("useOnSiteMode")!=111){
					Diadrasis.Instance.ChangeStatus(Diadrasis.User.inShop);
					
					Diadrasis.Instance.menuUI.warningsUI.ShowBuyFirstMessage();
				}else
				if(PlayerPrefs.GetInt("useOnSiteMode")==111){
					Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement>().preferredWidth=300f;

					Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text("unlockedProEdition");
					GameObject gb = Gps.Instance.warningsInfo.popUpMessage;
					
					
					AutoCloseMessage atc = gb.GetComponent<AutoCloseMessage>();
					atc.objToSendMessage = null;
					atc.onTapHide=false;
					atc.xronos=7;
					gb.SetActive(true);

					// omixli
					Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (true);

					yield return new WaitForSeconds(7f);
					Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (false);
					gb.SetActive(false);
				}

				Shop.Instance.isFromSceneAction=false;
			}
			*/
			yield return new WaitForSeconds (2f);

			#if UNITY_EDITOR
			Debug.Log("GPS INIT");
			#endif

			Gps.Instance.Init();

			//show menu button
			//Diadrasis.Instance.menuUI.MenuButtonsShow();

			yield return new WaitForSeconds (0.5f);

		}

		yield return(StartCoroutine(CheckIfInternet()));

		StopCoroutine("CheckIfInternet");

		//check if user bought pro edition
		if(PlayerPrefs.GetInt("useOnSiteMode")!=111){
			Shop.Instance._shopManager.Restore();
		}

		//download xml updates
		if(Diadrasis.Instance.internetIsOn && !Diadrasis.Instance.updateXml)
		{
			if(xmlsNames.Length>0){
				for(int i=0; i<xmlsNames.Length; i++){
					//wait for current xml
					yield return(StartCoroutine(DownloadXml(xmlsNames[i])));
					//wait a frame
					yield return null;
				}
			}

			StopCoroutine("DownloadXml");

			#if UNITY_EDITOR
			Debug.Log("end of all downloads !!!");
			#endif

			if(xmlIsUpdated){
				//get-read data xml 
				Globals.Instance.Init();
				
				//get menu texts from terms xml
				appData.Init();
				
				appData.InitCoupons();

				xmlIsUpdated=false;
			}
		}

		yield break;

	}

	string serverHtml = "http://e-chronomichani.gr/timeWalk/";
	string serverXmlFolder = "xmls/";
	string[] xmlsNames = new string[7]{"credits", "coupons", "terms", "menu", "movement", "settings", "sounds"};//"scenes" > chars are bigger than 255
	string htmlCheckInternet = "internetCheck.txt";
	string fromServer, currVersion;
	bool isLoading;
	WWW wwwData;
	float localVersion=0f;
	float remoteVersion=0f;
	bool xmlIsUpdated;

	#region CHECK INTERNET

	//checks if is online every 90 secs
	public IEnumerator CheckIfInternet(){

		#if UNITY_EDITOR
		Debug.Log("CheckIfInternet");
		#endif

		fromServer=string.Empty;
		yield return new WaitForSeconds(1f);
		//get string from txt on server
		WWW www = new WWW(serverHtml+htmlCheckInternet);
		//wait download
		yield return www;
		//if there is no error
		if(www.error==null){
			//assign text to our string
			fromServer = www.text;			//Debug.Log ("fromserver = "+fromServer);
			//if string is the  same then user is online
			if(fromServer!=string.Empty && fromServer=="Athens Time-Walk"){

				Diadrasis.Instance.internetIsOn = true;

			}else{
				Diadrasis.Instance.internetIsOn = true; //false;
//				yield break;
			}
		}else{
			Diadrasis.Instance.internetIsOn = true;//false;
//			yield break;
		}
	}

	#endregion

	#region DOWNLOAD XML

	IEnumerator DownloadXml(string xml){
		

		//get and save all used coupons from server
		if (xml == "coupons") {

			string dp = serverHtml + xml +"/coupons.txt";// 

			#if UNITY_EDITOR
			dp = "http://www.e-chronomichani.gr/timewalk/coupons/testCoupons.txt";
			#endif

			WWW www = new WWW(dp);

			while(!www.isDone){
				yield return null;
			}

			if (!string.IsNullOrEmpty (wwwData.error)) {
				#if UNITY_EDITOR
				Debug.Log(wwwData.error);
				#endif
			} else {

				Shop.Instance._shopManager.couponUsed = PlayerPrefsX.GetStringArray ("couponUsed").ToList ();

				#if UNITY_EDITOR
				if (Shop.Instance._shopManager.couponUsed.Count > 0) {
					foreach (string kod in Shop.Instance._shopManager.couponUsed) {
						Debug.LogWarning ("code = " + kod);
					}
				}
				#endif

				string serverText = Regex.Replace(www.text, "(<!--(.*?)-->)", string.Empty);
				//διαχωρισε το string σε γραμμες
				string[] kaneLines = serverText.Split('\n');
				//καθε γραμμη προσθεσε την σαν νεα τιμη στη λιστα
				for (int i = 0; i < kaneLines.Length; i++) {
					kaneLines [i].Trim ();

					if (kaneLines [i].StartsWith ("{")) {

						//clear line
						string code = Regex.Replace (kaneLines [i], "{(<!--(.*?)-->)}", string.Empty);
						code = code.Replace ("{", "");
						code = code.Replace ("}", "");

						//αν υπερβαινει τους 4 χαρακτηρες
						if (code.Length > 3) {
							Shop.Instance._shopManager.couponUsed.Add (code);
						}
					}
				}

				//remove duplicates
				Shop.Instance._shopManager.couponUsed = Shop.Instance._shopManager.couponUsed.Distinct ().ToList();

				#if UNITY_EDITOR
				if (Shop.Instance._shopManager.couponUsed.Count > 0) {
					foreach (string kod in Shop.Instance._shopManager.couponUsed) {
						Debug.LogWarning ("code = " + kod);
					}
				}
				#endif

				//save coupons
				PlayerPrefsX.SetStringArray("couponUsed",Shop.Instance._shopManager.couponUsed.ToArray());
				PlayerPrefs.Save ();
			}
		}

		string[] savedCurrentXml = PlayerPrefsX.GetStringArray("fromServer_"+xml); //last update

		string downloadPath = serverHtml + serverXmlFolder + xml +".xml";

		#if UNITY_EDITOR
		Debug.Log(xml.ToUpper()+" start download >> "+downloadPath);
		#endif

		wwwData = new WWW(downloadPath);

		isLoading=true;

		while(!wwwData.isDone){
			yield return null;
		}

		isLoading=false;

		if(!string.IsNullOrEmpty(wwwData.error)){
			#if UNITY_EDITOR
			Debug.Log(wwwData.error);
			#endif
		}else{

			#if UNITY_EDITOR
			Debug.Log(xml+" .. checking local version...");  
			#endif

			if(savedCurrentXml.Length>0){
				//check local saved xml version
				XmlDocument localXml = new XmlDocument();
				string textAsset = string.Empty;

				for(int x=0; x<savedCurrentXml.Length; x++){
					textAsset+=savedCurrentXml[x];
				}

				string  localExcludedComments = Regex.Replace(textAsset, "(<!--(.*?)-->)", string.Empty);
				localXml.LoadXml(localExcludedComments);
				
				XmlNode localNodeVersion = localXml.SelectSingleNode("/"+xml);
				if(localNodeVersion.Attributes["version"].Value!=null){
					currVersion=localNodeVersion.Attributes["version"].Value;
					localVersion=float.Parse(currVersion);
					
					#if UNITY_EDITOR
					Debug.Log("local saved version is "+localVersion.ToString());
					#endif
				}
			}else{

				//check local initial xml version
				XmlDocument localXml = new XmlDocument();
				TextAsset textAsset = (TextAsset) Resources.Load("XML/"+xml);
				string  localExcludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
				localXml.LoadXml(localExcludedComments);

				XmlNode localNodeVersion = localXml.SelectSingleNode("/"+xml);
				if(localNodeVersion.Attributes["version"].Value!=null){
					currVersion=localNodeVersion.Attributes["version"].Value;
					localVersion=float.Parse(currVersion);

					#if UNITY_EDITOR
					Debug.Log("local initial version is "+localVersion.ToString());
					#endif
				}
			}

			yield return new WaitForSeconds(0.25f);

			//bale to text olo se ena string
												//Debug.Log(fromServer);
			
//			//διαχωρισε το string σε γραμμες
//			string[] kaneLines = fromServer.Split('\n');
//			//καθε γραμμη προσθεσε την σαν νεα τιμη στη λιστα
//			for (int i = 0; i < kaneLines.Length; i++) {
//				//αν δεν υπερβαινει τους 255 χαρακτηρες
//				if(kaneLines[i].Length<255){
//					fromInternet.Add(kaneLines[i]);
//				}
//			}

//			#if UNITY_EDITOR
//			Debug.LogWarning(wwwData.text);
//			#endif

			XmlDocument draft = new XmlDocument();
			string fromServer = wwwData.text;//WWW.EscapeURL(wwwData.text);
			string serverExcludedComments = Regex.Replace(fromServer, "(<!--(.*?)-->)", string.Empty);
			draft.LoadXml(serverExcludedComments);

			XmlNode serverNodeVersion = draft.SelectSingleNode("/"+xml);
			if(serverNodeVersion!=null)
			{
				if(serverNodeVersion.Attributes["version"].Value!=null){
					currVersion=serverNodeVersion.Attributes["version"].Value;
					remoteVersion=float.Parse(currVersion);

					#if UNITY_EDITOR
					Debug.Log("server new version is "+remoteVersion.ToString());
					#endif
				}
			}

			if(localVersion!=remoteVersion){
				if(localVersion>remoteVersion){

					#if UNITY_EDITOR
					Debug.Log("local data xml is newest");
					#endif

				}else{

					#if UNITY_EDITOR
					Debug.Log("local data xml is old.. updating...");
					#endif

					List<string> fromInternet = new List<string>();

					//clear saved data
					if(savedCurrentXml.Length>0){
						fromInternet.Clear();
						//reset
						PlayerPrefsX.SetStringArray("fromServer"+xml,fromInternet.ToArray());

						savedCurrentXml = fromInternet.ToArray();
					}
						
					//διαχωρισε το string σε γραμμες
					string[] kaneLines = fromServer.Split('\n');
					//καθε γραμμη προσθεσε την σαν νεα τιμη στη λιστα
					for (int i = 0; i < kaneLines.Length; i++) {
						//αν δεν υπερβαινει τους 255 χαρακτηρες
						if(kaneLines[i].Length<255){
							fromInternet.Add(kaneLines[i]+"\n");//important >>> add new line
						}else{
							#if UNITY_EDITOR
							Debug.LogError("Chars > 255");
							#endif
						}
					}

					//save updated xml
					PlayerPrefsX.SetStringArray("fromServer_"+xml,fromInternet.ToArray());
					savedCurrentXml = fromInternet.ToArray();

					xmlIsUpdated=true;
				}
			}else{
				#if UNITY_EDITOR
				Debug.Log("local xml has the same version with server!");
				#endif
			}

			Diadrasis.Instance.updateXml=true;
			

			//File.Delete(draftPath);
			//}
		}
	}

	#endregion


	string GetiPhoneDocumentsPath()
	{ 
		string path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);
		path = path.Substring(0, path.LastIndexOf('/'));  
		return path + "/Documents";
	}

	string GetAndroidDocumentsPath()
	{ 
		string path = Application.persistentDataPath;
		return path;
	}
}
