using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using eChrono;
using Stathis;

[Serializable]
public class Syntetagmenes  : System.Object//Not a MonoBehaviour!
{
	public Vector2 centerOfmap = new Vector2(37.974923f, 23.727023f);	//(37.9747f, 23.7281f);

	//peiraos & kolokynthous
	public Vector2 reffA_gps = new Vector2(37.981325f, 23.721805f);		//(37.9853f, 23.7107f);
	public Vector2 reffA_scene = new Vector2(-112f, 177f);	//(-336f, 254f);

	//arditoy & markou mousourou
	public Vector2 reffB_gps = new Vector2(37.968157f, 23.737310f);
	public Vector2 reffB_scene = new Vector2(226f, -184f);//(542f, -381f);

	//for testing
	public Vector2 testPos = new Vector2(37.974180f, 23.727013f);//aerides
	//Office = 37,9883 - 23,7292
	//Omonoia = 37,9841 - 23,7282
	//aerides = 37.974180 - 23.727013

	public Vector2 gpsPos;
}

[Serializable]
public class Skines  : System.Object//Not a MonoBehaviour!
{
	public List<Transform> sceneTargets = new List<Transform>();
	public List<Transform> scenesTextInfos = new List<Transform>();
	public GameObject labelAbovePerson;
	public Transform canvasMain;
	public Transform scenesFather;
	public CanvasGroup kanvasGroupScenes;
}

//for auto resize and move map childs
struct ButtonsAndLabelsForScenes
{
	private RectTransform rekt;
	private Vector2 size;
	private Vector2 pos;
	private bool needResizing;

	public RectTransform Rekt{ get; set;}
	public Vector2 Size{ get; set; }
	public Vector2 Pos{ get; set; }
	public bool NeedResizing{ get; set; }
}


public class XartisMenu : MonoBehaviour 
{

	//move scale buttons and labels relative to map zoom
	List<ButtonsAndLabelsForScenes> buttonsAndLabelsForScenes = new List<ButtonsAndLabelsForScenes> ();

	public Syntetagmenes synt;
	public Skines skines;

	public Transform personPineza;
	RectTransform pinezaRT;

	public Transform mapTransform;

	//for auto resize and move map childs
	public RectTransform mapRekt;
	public Vector2 sizeMapDefault = new Vector2 (1000f, 1000f);

	Vector2 prevGps =  new Vector2(37.976413f, 23.664408f);//faliro
	Vector2 curGPS = Vector2.one;

	string title, message, buttonLabelYes, buttonLabelNo;

	XmlNodeList menuList;
	List<string> sceneTitles = new List<string> ();
//	List<string> scenePoiNames = new List<string> ();
//	List<string> sceneToLoadNames = new List<string> ();
	List<Vector2> sceneMapPositions = new List<Vector2>();

	//	Vector2 screenMiddle;

	public List<PeriodValues> periodValuesScripts = new List<PeriodValues>();

	public void SetBestScores()
    {
		Debug.LogWarning("SetBestScores "+periodValuesScripts.Count);
		foreach(PeriodValues p in periodValuesScripts)
        {
			p.gamePoints.text = PlayerPrefs.GetInt("gameSceneBestScore" + p.onomaSkinis).ToString();
		}
    }

	public void Init(){
		#if UNITY_EDITOR
		Debug.Log("Xartis Menu INIT");
		#endif

		//get gps data
		curGPS = new Vector2 (38f, 23f);

		SetBestScores();
	}

	IEnumerator Start () {

		#if UNITY_EDITOR
		Debug.Log("Xartis Menu Start");
		#endif

//		screenMiddle = skines.canvasMain.GetComponent<RectTransform> ().sizeDelta; //new Vector2 (Screen.width / 2f, Screen.height / 2f);

		mapRekt = mapTransform.GetComponent<RectTransform> ();
		pinezaRT = personPineza.GetComponent<RectTransform> ();

		personPineza.gameObject.SetActive (false);

		skines.sceneTargets.Clear();

		//get scenes from data xml
		// no need it - we already get it from auto check languange on start >> settingsUI -> SetLanguange
//		GetXmlScenes();

		yield return new WaitForSeconds (0.2f);

		if(skines.kanvasGroupScenes==null)
		{
			skines.kanvasGroupScenes=skines.scenesFather.gameObject.AddComponent<CanvasGroup>();
		}

		yield return new WaitForSeconds (0.2f);

		//show scenes on map
		CreateScenesOnMap();
		//show user on map
//		ShowUserOnMap();

		//get gps data
//		curGPS = new Vector2 (38f, 23f);	

		appIsStarted=true;

	}


	void OnEnable()
	{
//		Diadrasis.Instance.OnSceneLoadEnd += MenuButtonsShow;
		Diadrasis.Instance.OnSceneLoadStart += ResetMapAmesos;
	}
	
	
	void OnDisable()
	{
//		Diadrasis.Instance.OnSceneLoadEnd -= MenuButtonsShow;
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.OnSceneLoadStart -= ResetMapAmesos;
		}
	}


	void HideXartiMenu(){

	}

	public void ResetMapAmesos(){
		#if UNITY_EDITOR
		Debug.Log("Close Periods from loading.");
		#endif
		ClosePeriodView ();
		mapPicturePanel.localPosition = Vector3.zero;
		mapPicturePanel.sizeDelta = sizeMapDefault;
		isShowingPeriod=false;
		//reset menu xartis script variables
		StopAllCoroutines();
		Diadrasis.Instance.menuUI.menuXartisPanel.SetActive (false);
		this.enabled = false;
	}

	void GetGps(){
		if(Gps.Instance.isWorking())
		{
			
			if(!Input.compass.enabled){
				Input.compass.enabled=true;
			}
			
//			#if UNITY_EDITOR
//			Debug.LogWarning("checking gps");
//			#endif
			
			MoveStigma();
			
		}else{
			if(personPineza.gameObject.activeSelf){
				personPineza.gameObject.SetActive(false);
			}
			
//			#if UNITY_EDITOR
//			Debug.LogWarning("checking gps is OFF");
//			#endif

			if(!string.IsNullOrEmpty(Diadrasis.Instance.nearSceneAreaName)){
				Diadrasis.Instance.nearSceneAreaName = string.Empty;
			}
			
			if(Gps.Instance.isEnabled()==false && Gps.Instance.status == Gps.Status.STOPPED){
				
				if(myNearSceneOnoma != Diadrasis.Instance.nearSceneAreaName)
				{
					myNearSceneOnoma = Diadrasis.Instance.nearSceneAreaName;
					Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.OFF, true);
				}else{
					Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.OFF, false);
				}
			}
		}
	}

	void Update () 
	{
//		#if UNITY_EDITOR
//		if(Input.GetKeyDown(KeyCode.A)){
//			Debug.Log("calculate gps distance");
//			Debug.LogWarning("dist = "+distanceBetweenTwoGpsPoints(a,b));
//		}
//		if(Input.GetKeyDown(KeyCode.B)){
//			Debug.Log("calculate gps distance from aerides to omonoia");
//			Debug.LogWarning("dist = "+distanceBetweenTwoGpsPoints(synt.testPos, omonoia));
//		}
//		if(Input.GetKeyDown(KeyCode.C)){
//			Debug.Log("calculate gps distance from aerides to diadrasis");
//			Debug.LogWarning("dist = "+distanceBetweenTwoGpsPoints(synt.testPos, diadrasis));
//		}
//		#endif


		if (Diadrasis.Instance.user == Diadrasis.User.inMenu)
		{

			if(appIsStarted){
				GetGps ();
			}

			if (Diadrasis.Instance.menuStatus == Diadrasis.MenuStatus.periodView) {
				if (Input.GetMouseButtonDown (0)) {
					#if UNITY_EDITOR
					Debug.LogWarning("IsPointerOverUIObject XARTIS MENU 244");
					#endif
					if (!Tools_UI.IsPointerOverUIObject()) {
							#if UNITY_EDITOR
							Debug.Log("Close Periods from tap enywhere.");
							#endif

							ClosePeriodView ();
					}
				}
			}

			//animation for showing period buttons
			if (isShowingPeriod) {
				ShowPeriodousWithAnimation ();
			}

			//rezise btns labels relative to map zoom move
			if (buttonsAndLabelsForScenes.Count > 0 && mapRekt != null) {
				foreach (ButtonsAndLabelsForScenes f in buttonsAndLabelsForScenes) {
					Stathis.Tools_UI.ResizeMoveRect_RelativeToParent (sizeMapDefault, mapRekt, f.Size, f.Rekt, f.Pos, f.NeedResizing);
				}
			}


			if (personPineza.gameObject.activeSelf) 
			{
				//calculate pos on resize
				Vector2 mapPos = Stathis.Tools_UI.calculateOnParentResize(sizeMapDefault, mapRekt,pinezaRT.localPosition, pinezaPosition);
				if(mapPos!=Vector2.zero && pinezaPosition!=Vector2.zero)
				{
					pinezaRT.localPosition = Vector3.Lerp(pinezaRT.localPosition, mapPos, Time.smoothDeltaTime);
				}else{
					pinezaRT.gameObject.SetActive(false);
				}

				if(Input.compass.enabled){
					pinezaRT.localRotation = Quaternion.Lerp(pinezaRT.localRotation, Quaternion.Euler(0f,0f,Mathf.Floor(Input.compass.trueHeading)),Time.deltaTime * moveSettings.compassTurnSpeed);
				}
				
			}

		}
	}

	public bool needsToResetMapAndZoom()
	{
		float dist = Vector3.Distance(mapPicturePanel.localPosition, Vector3.zero);
		float diaf = Vector3.Distance(new Vector3(mapPicturePanel.sizeDelta.x, mapPicturePanel.sizeDelta.y, 0f), new Vector3(1000f, 1000f, 0f));

		if(dist>1f || diaf>10f) //Camera.main.fieldOfView!=23f)
		{
			StopCoroutine("lerpMapReset");
			StartCoroutine(lerpMapReset());
			//deactivate return btn
			Diadrasis.Instance.menuUI.btnsMenu.btnReturn.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.5f;
			return true;
		}
		return false;
	}

	IEnumerator lerpMapReset(){		//Debug.Log ("lerpMapReset 1");

		while (Vector3.Distance(mapPicturePanel.localPosition, Vector2.zero)> 3f || Vector2.Distance(mapPicturePanel.sizeDelta, sizeMapDefault) > 3f)
		{
			if (Diadrasis.Instance.menuStatus == Diadrasis.MenuStatus.idle) {
								
				//reset move
				mapPicturePanel.localPosition = Vector3.Lerp (mapPicturePanel.localPosition, Vector3.zero, Time.deltaTime * 2.5f);
				//reset scale
				mapPicturePanel.sizeDelta = Vector2.Lerp (mapPicturePanel.sizeDelta, sizeMapDefault, Time.deltaTime * 2.5f);

				yield return null;
				
			}else{
				//in case user stops lerping activate return btn again
				Diadrasis.Instance.menuUI.btnsMenu.btnReturn.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1f;
				yield break;
			}

		}

		//if lerping reset ends activate return btn again
		Diadrasis.Instance.menuUI.btnsMenu.btnReturn.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1f;
		mapPicturePanel.localPosition = Vector3.zero;
		mapPicturePanel.sizeDelta = new Vector2 (1000f, 1000f);

		yield break;
	}

	public RectTransform mapPicturePanel;
	public int currSceneSelected=-1;
	public bool animatePeriods = true;

	void ShowPeriodousWithAnimation()
	{				
		//Debug.Log("HIDE ALL OTHERS !!!");

		Vector3 pos = periodCurrentFather.localPosition;

		Vector3 centerOfScene = skines.sceneTargets[currSceneSelected].localPosition;	

//		#if UNITY_EDITOR
//		Debug.Log(skines.sceneTargets[currSceneSelected].name+" = "+centerOfScene);
//		#endif

		//auto move and zoom of map at center of scene
		mapPicturePanel.localPosition = Vector3.Lerp (mapPicturePanel.localPosition, new Vector3 (-centerOfScene.x, -centerOfScene.y, 0f), Time.deltaTime * 2.5f);

		#region animation of periods

		if(animatePeriods)
		{
			//move periods
	 		periodCurrentFather.localPosition = Vector3.Lerp(pos,new Vector3(pos.x,100f,pos.z),Time.deltaTime*2.5f);

			//scale periods
			Vector3 scal = periodCurrentFather.localScale;
			periodCurrentFather.localScale = Vector3.Lerp(scal,new Vector3(1f,1f,1f),Time.deltaTime*2.5f);
			
			if(periodCurrentFather.localPosition.y>=99f)
			{
				pos.y=100f;
				periodCurrentFather.localPosition = pos;

				isShowingPeriod=false;

			}
		}
		#endregion

	}

	public Color colorHide;

	void Hide_RestScenes_AndMakeMapBlack(Transform tr)
	{
		#if UNITY_EDITOR
		Debug.Log("Hide_RestScenes_AndMakeMapBlack");
		#endif

		foreach(Transform t in skines.sceneTargets)
		{
			if(t!=skines.sceneTargets[currSceneSelected])
			{
				if(!t.name.StartsWith("btn_"))
				//hide with lerp or set active false
				t.gameObject.SetActive(false);
			}
		}

		foreach(Transform t in skines.scenesTextInfos)
		{
			if(t.name!=tr.name){
				if(t.gameObject.activeSelf)
				{
					//hide with lerp or set active false
					t.gameObject.SetActive(false);
				}
			}
		}

	}

	void Show_RestScenes_AndMakeMapWhite()
	{
		#if UNITY_EDITOR
		Debug.Log("Show_RestScenes_AndMakeMapWhite");
		#endif
		
		foreach(Transform t in skines.sceneTargets)
		{
			if(!t.gameObject.activeSelf)
			{
				//hide with lerp or set active false
				t.gameObject.SetActive(true);
			}
		}

		foreach(Transform t in skines.scenesTextInfos)
		{
			if(!t.gameObject.activeSelf)
			{
				//hide with lerp or set active false
				t.gameObject.SetActive(true);
			}
		}
	}

	cSceneArea currScene;

	void CreateScenesOnMap()
	{
		Debug.LogWarning("CreateScenesOnMap");

		buttonsAndLabelsForScenes.Clear ();
		periodValuesScripts.Clear();

		for (int i=0; i<appData.mySceneAreas.Count; i++)
		{
			currScene = new cSceneArea();
			currScene = appData.mySceneAreas.ElementAt(i).Value;

			//add to list
			sceneTitles.Add(currScene.LabelTitle);

			#region create button for scene on map

			//create new btn for scene on map
			GameObject sceneBtn = Instantiate(Resources.Load("prefabs/FINAL/menu/btnSceneMap")) as GameObject;

			//set parent
			sceneBtn.transform.SetParent(skines.scenesFather);
			//set name
			sceneBtn.name = "btn_"+sceneTitles[i];


			//reset scale (bug in runtime parenting)
			sceneBtn.transform.localScale = Vector3.one;

			Vector2 btnSize = new Vector2(currScene.MapBtnSizePos.x, currScene.MapBtnSizePos.y);

			RectTransform btnRT = sceneBtn.GetComponent<RectTransform>();

			btnRT.sizeDelta = btnSize;

			Vector2 skiniPos = new Vector2(currScene.MapBtnSizePos.z, currScene.MapBtnSizePos.w);	//Debug.Log(skiniPos);
			
//			sceneMapPositions.Add(skiniPos);

			//set scene position on map
			btnRT.localPosition = skiniPos;//sceneMapPositions[i];// * 0.01f;//FindPosition(sceneMapPositions[i]);//new Vector3(pos2D.x, pos2D.y, 0f);

			//add to list for movement via screen position
			skines.sceneTargets.Add(sceneBtn.transform);

			ButtonsAndLabelsForScenes bl = new ButtonsAndLabelsForScenes();
			bl.NeedResizing = true;
			bl.Pos = skiniPos;
			bl.Size = btnSize;
			bl.Rekt = btnRT;

			buttonsAndLabelsForScenes.Add(bl);

			#endregion

			#region create label and periods

//			string prefab = string.Empty;

//			if(Screen.currentResolution.width<=800f){
//				prefab = "labelSceneBig";
//			}else{
//				prefab = "labelScene";
//			}

			GameObject sceneLabel = Instantiate(Resources.Load("prefabs/FINAL/menu/labelScene")) as GameObject;///ButtonInfo")) as GameObject;

			sceneLabel.name = "label_"+sceneTitles[i];

			ButtonScene btnScript = sceneLabel.GetComponent<ButtonScene>();

			if(sceneLabel.name.Contains("Diadrasis")){
				btnScript.isHiding=true;
			}

			btnScript.myIndexInScenes=i;

			btnScript.langNow = Diadrasis.Instance.languangeNow.ToString();

			btnScript.keyPoi = 	currScene.Name;
			
			sceneLabel.transform.SetParent(skines.scenesFather);

			sceneLabel.transform.localScale = Vector3.one;
			sceneLabel.transform.position = Vector3.zero;

			btnScript.label.text = currScene.LabelTitle;// sceneTitles[i];

//			Diadrasis.Instance.menuUI.settingsUI.olaTaTexts.Add(btnScript.label);


			skines.scenesTextInfos.Add(sceneLabel.transform);

			//add period buttons
			

			Transform periodContainer = btnScript.periodsContainer; //skines.textInfo.transform.FindChild("radialMenu");	

			if(periodContainer==null){Debug.Log("!!!! no container");}

			for(int a=0; a<currScene.Periods.Count; a++)
			{
				GameObject g = Instantiate(Resources.Load("prefabs/FINAL/menu/btnPeriod")) as GameObject;

				g.transform.SetParent(periodContainer);
				g.transform.localScale = Vector3.one;
				g.transform.localEulerAngles = Vector3.zero;
				Vector3 pos = g.transform.localPosition;
				pos.z=0f;
				g.transform.localPosition = pos;

				PeriodValues periodScript = g.GetComponent<PeriodValues>();
				periodScript.periodImage.overrideSprite = Tools_Load.LoadSpriteFromResources("images/periods", currScene.Periods[a].Period_Image);

//				Debug.Log(a+ " -- "+currScene.periods[a].sceneName);
				periodScript.map = currScene.Periods[a].MapImage;
				//periodScript.mapFilter = currScene.Periods[a].MapFilterPaths;
				periodScript.mapPivot = currScene.Periods[a].MapFullPivot;
				periodScript.mapFullPosition = currScene.Periods[a].MapFullPosition;
				periodScript.mapFullZoom = currScene.Periods[a].MapFullZoom;

				periodScript.onomaSkinis = currScene.Periods[a].SceneName;
//				periodScript.kameraSkinis = currScene.periods[a].sceneKamera;
				periodScript.onomaPoi = currScene.Periods[a].PoiName;
				periodScript.loadingImage = currScene.Periods[a].LoadinImage;
//				periodScript.loadingText = currScene.periods[a].loadingText;

				//intro scene
//				periodScript.introNarrations = currScene.periods[a].intro.introNarrations;
				periodScript.loadingText = currScene.Periods[a].Intro.LoadingText;
				periodScript.introText = currScene.Periods[a].Intro.IntroText;
				periodScript.introTitle = currScene.Periods[a].Intro.IntroTitle;

				//save poi name and period number for changing languange texts
				periodScript.keyPoiname = currScene.Name;
				periodScript.myIndexInPeriods = a;
				periodScript.xartisMenu=this;
				periodScript.langNow = appSettings.language;
//				periodScript.spotPositions.Clear();
//				periodScript.spotPositions = currScene.periods[a].spotPositions;

				periodScript.btn.onClick.AddListener(()=>periodScript.OpenScene());
				periodScript.periodLabel.text = currScene.Periods[a].LabelTitle;
				periodScript.gamePoints.text = PlayerPrefs.GetInt("gameSceneBestScore" + periodScript.onomaSkinis).ToString();
				//				Diadrasis.Instance.menuUI.settingsUI.olaTaTexts.Add(periodScript.label);
				//				periodScript.gameObject.AddComponent<AddMeToListaTexts>();

				periodValuesScripts.Add(periodScript);
			}

			//TODO
			//get button
//			Button btn = skines.textInfo.GetComponent<Button>();

			//add function to open time periods new buttons for current Area
			btnScript.myBtn.onClick.AddListener(()=>ShowPeriods(periodContainer,btnScript.rt,btnScript.myIndexInScenes));

			if(!sceneBtn.name.Contains("Diadrasis")){
				Button myButton = sceneBtn.GetComponent<Button>();
				myButton.onClick.AddListener(()=>ShowPeriods(periodContainer,btnScript.rt,btnScript.myIndexInScenes));
			}

			btnScript.btnOnMapRT = btnRT;

			periodContainer.gameObject.SetActive(false);

			periodContainers.Add(periodContainer);

			//reuse previus rectransform
			RectTransform labelRT = sceneLabel.GetComponent<RectTransform>();
			
			labelRT.localPosition = new Vector3(currScene.MapLabelPos.x, currScene.MapLabelPos.y, 0f);

			btnScript.InitLabelMenu();

			ButtonsAndLabelsForScenes ba = new ButtonsAndLabelsForScenes();
			ba.NeedResizing = false;
			ba.Pos = currScene.MapLabelPos;
			ba.Size = labelRT.sizeDelta;
			ba.Rekt = labelRT;
			
			buttonsAndLabelsForScenes.Add(ba);

			
			//Debug.Log(currScene.Name+ " = " + sceneLabel.transform.localPosition);
//			Debug.Log(mySceneArea.Name + " = " +mySceneArea.MapLabelPos);

			#endregion
		}

		//add person too
//		ButtonsAndLabelsForScenes bp = new ButtonsAndLabelsForScenes();
//		bp.NeedResizing = false;
//		bp.Pos = currScene.MapLabelPos;
//		bp.Size = labelRT.sizeDelta;
//		bp.Rekt = labelRT;
//		
//		buttonsAnLabelsForScenes.Add(bp);

	}

	List<Transform> periodContainers = new List<Transform>();
	public bool isShowingPeriod=false;
	public Transform periodCurrentFather;

	void ShowPeriods(Transform gb, RectTransform rt, int myIndex)
	{
		#if UNITY_EDITOR
		Debug.Log("bug on gps warning message? look here!!");
		#endif
		//bug on gps warning message
//		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);

		currSceneSelected=myIndex;

		if(!isShowingPeriod)
		{
			Diadrasis.Instance.menuStatus=Diadrasis.MenuStatus.periodView;

			//if container is null
			if(!periodCurrentFather)
			{
				//reset all (scale and pos.y)
				ResetPeriodButtons();

				ShowScenePeriods(gb,rt);

			}
			else
			if(periodCurrentFather)
			{
				Show_RestScenes_AndMakeMapWhite();

				if(periodCurrentFather==gb)
				{
					//reset all (scale and pos.y)
					ResetPeriodButtons();

					periodCurrentFather=null;

					Diadrasis.Instance.menuStatus=Diadrasis.MenuStatus.idle;

				}
				else
				if(periodCurrentFather!=gb)
				{
					//reset all (scale and pos.y)
					ResetPeriodButtons();

					ShowScenePeriods(gb,rt);
				}

			}
		}else{
			//reset all (scale and pos.y)
//			ResetPeriodButtons();
//			periodCurrentFather=null;
//			isShowingPeriod=false;

			Show_RestScenes_AndMakeMapWhite();
			
			ShowScenePeriods(gb,rt);
		}
		//deprecated
		//close period if user tap on other scene
		/*
		else
		{
			#if UNITY_EDITOR
			Debug.Log("Close Periods from tap on other scene btn or label.");
			#endif
			ClosePeriodView();
		}
		*/
	}

	/*

	void ShowUserOnMap()
	{
		skines.sceneTargets.Add(personPineza);
		
		skines.labelAbovePerson = Instantiate(Resources.Load("prefabs/newUI/youAreHere")) as GameObject;
		skines.labelAbovePerson.transform.name = "userInfo";
		skines.labelAbovePerson.transform.eulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x,0f,0f);
		skines.labelAbovePerson.transform.SetParent(skines.scenesFather);//(Diadrasis.Instance.menuUI.labelsFather);
		skines.labelAbovePerson.transform.localScale = new Vector3(1f,1f,1f);
		Text txt = skines.labelAbovePerson.GetComponentInChildren<Text>();
		txt.name="userLabel";
		txt.text = "You are here";
		txt.gameObject.AddComponent<AutoSetLanguange>();
//		Diadrasis.Instance.olaTaLabelKeimena.Add(txt);
//		txt.gameObject.AddComponent<AddMeToListaTexts>();
		skines.scenesTextInfos.Add(skines.labelAbovePerson.transform);
	}

	*/

//	List<string> sceneTags = new List<string>();
//	int allSceneAreas =0;
	cSceneArea mySceneArea;

	//TODO
	//must reset if languange changed

	public void GetXmlScenes(){		//Debug.Log("GetXmlScenes");

		//Read message settings
		XmlNodeList messageSettings =Globals.Instance.settingsXml.SelectNodes ("/settings/messages");
		
		if (messageSettings.Count > 0) 
		{
			
			foreach(XmlNode message in messageSettings)
			{
				if (message ["xronosEmfanisisGpsInfo"] != null) {
					appSettings.xronosEmfanisisGpsInfo = int.Parse (message ["xronosEmfanisisGpsInfo"].InnerText);

					#if UNITY_EDITOR
					Debug.Log(appSettings.xronosEmfanisisGpsInfo);
					#endif

				}
			}
			
		}


		appData.mySceneAreas = new Dictionary<string, cSceneArea>();

		//get all scenes from xml
//		XmlNodeList sceneList = Globals.Instance.getXml().GetElementsByTagName ("sceneAreas/sceneArea");

//		XmlDocument doc = new XmlDocument();
//		doc.Load("data.xml");

		if (Globals.Instance.menuXML.SelectNodes ("/menu/sceneAreas/sceneArea") != null) 
		{
			XmlNodeList sceneList = Globals.Instance.menuXML.SelectNodes ("/menu/sceneAreas/sceneArea");

//			Debug.Log(sceneList.Count);

			foreach (XmlNode scene in sceneList) 
			{
				mySceneArea=new cSceneArea();

				mySceneArea.Name = scene ["name"].InnerText;

				mySceneArea.LabelTitle = scene ["labelTitle"] [appSettings.language].InnerText;

//				#if UNITY_EDITOR
//				Debug.LogWarning(mySceneArea.Name);
//				#endif

				//not need it yet
//				mySceneArea.ImageOnMap = scene ["imageOnMap"].InnerText;

				//btn pos & size on map
				Vector2 btnSize = new Vector2(float.Parse(scene ["mapBtnSizePos"] ["x"].InnerText), float.Parse(scene ["mapBtnSizePos"] ["y"].InnerText));
				Vector2 btnPos = new Vector2(float.Parse(scene ["mapBtnSizePos"] ["z"].InnerText), float.Parse(scene ["mapBtnSizePos"] ["w"].InnerText));

				mySceneArea.MapBtnSizePos = new Vector4(btnSize.x, btnSize.y, btnPos.x, btnPos.y);

				//pos of label on map
				mySceneArea.MapLabelPos = new Vector2(float.Parse(scene ["mapLabelPos"] ["x"].InnerText), float.Parse(scene ["mapLabelPos"] ["y"].InnerText));

//				Debug.Log(mySceneArea.Name + " = " +mySceneArea.MapLabelPos);

				//periodoi
				List<cPeriod> myPeriods = new List<cPeriod> ();

				XmlNode allPeriods = scene.SelectSingleNode("periods");

				if(allPeriods != null)
				{
					if (allPeriods.ChildNodes.Count > 0)
					{
						XmlNodeList periods = allPeriods.ChildNodes;	

						foreach (XmlNode period in periods)
						{
							cPeriod perd = new cPeriod ();

							//get node childs
							XmlNode poiName = period.SelectSingleNode("poiName");
							XmlNode intro = period.SelectSingleNode("intro");
							XmlNode icon = period.SelectSingleNode("icon");
							XmlNode sceneName = period.SelectSingleNode("sceneName");
							XmlNode loadingImage = period.SelectSingleNode("loadingImage");
							XmlNode labelTitleScene = period.SelectSingleNode("labelTitle");

							if(poiName != null)
							{
								if (!string.IsNullOrEmpty(poiName.InnerText)) {
									perd.PoiName = poiName.InnerText;
								} else {
									perd.PoiName = string.Empty;
								}
							}

							if(intro != null)
							{
								if(intro.ChildNodes.Count>0)
								{

									perd.Intro = new cIntro();

									//narrations parent node
									XmlNode introNarrations = intro.SelectSingleNode("introNarrations");
									List<cIntroNarration> myNarrations = new List<cIntroNarration> ();

									if (introNarrations != null) { //add to the rest
										if (introNarrations.ChildNodes.Count > 0) {
											//get all narrations
											XmlNodeList narrations = introNarrations.ChildNodes;	

											foreach (XmlNode narration in narrations) {

												cIntroNarration nar = new cIntroNarration ();

												XmlNode myFile = narration.SelectSingleNode("file");
												XmlNode pauseTime = narration.SelectSingleNode("pauseTime");

												if(!string.IsNullOrEmpty(myFile.InnerText)){
													nar.File = myFile.InnerText;
												} else {
													nar.File = string.Empty;
												}

												if(!string.IsNullOrEmpty(pauseTime.InnerText)){
													nar.PauseTime = float.Parse(pauseTime.InnerText);
												} else {
													nar.PauseTime = 0f;
												}

												myNarrations.Add (nar);
											}						
										}
									}

									perd.Intro.Narrations = myNarrations;

									XmlNode loadingText = intro.SelectSingleNode("loadingText");

									//get loading text
									if (loadingText != null)
									{
										
										string myLoadingText = loadingText [appSettings.language].InnerText;
										
										if(!string.IsNullOrEmpty(myLoadingText)){
											perd.Intro.LoadingText = myLoadingText;
										} else {
											perd.Intro.LoadingText = string.Empty;
										}
									}else{
										Ektypose("loadingText = Null");
									}
						
									//get intro text
									XmlNode introText = intro.SelectSingleNode("introText");

									if (introText != null)
									{ 
										string myIntro = introText [appSettings.language].InnerText;

										if(!string.IsNullOrEmpty(myIntro)){
											perd.Intro.IntroText = myIntro;
										} else {
											perd.Intro.IntroText = string.Empty;
										}
									}else{
										Ektypose("introText = Null");
									}

									XmlNode labelTitle = intro.SelectSingleNode("labelTitle");

									if(labelTitle != null)
									{
										string myTitleIntro = labelTitle [appSettings.language].InnerText;
										
										if(!string.IsNullOrEmpty(myTitleIntro)){
											perd.Intro.IntroTitle = myTitleIntro;
										} else {
											perd.Intro.IntroTitle = string.Empty;
										}
									}

								}else{
									Ektypose("intro childs NULL");
								}
							}else{
								Ektypose("intro is NULL");
							}


							if (icon != null)
							{
								if (!string.IsNullOrEmpty(icon.InnerText)) {
									perd.Period_Image = icon.InnerText;
								} else {
									perd.Period_Image = "none";
								}
							}

							if (sceneName != null)
							{
								if (!string.IsNullOrEmpty(sceneName.InnerText)) {
									perd.SceneName = sceneName.InnerText;
								} else {
									perd.SceneName = "none";
								}
							}


							if (labelTitleScene != null)
							{
								if (!string.IsNullOrEmpty(labelTitleScene [appSettings.language].InnerText))
								{
									perd.LabelTitle = labelTitleScene [appSettings.language].InnerText;

									//ADD YEAR TO INTRO KEIMENO TITLE
									if(!string.IsNullOrEmpty(perd.Intro.IntroTitle)){
										perd.Intro.IntroTitle +=" "+perd.LabelTitle;
									}

								} else {
									perd.LabelTitle = string.Empty;	
								}
							}

							if (loadingImage != null)
							{
								if (!string.IsNullOrEmpty(loadingImage.InnerText)) {
									perd.LoadinImage = loadingImage.InnerText;
								} else {
									perd.LoadinImage = "none";
								}
							}


							#region READ MOVEMENT XML

//							#if UNITY_EDITOR
//							Debug.LogWarning("Reading for "+perd.PoiName);
//							#endif

							//get map from movement xml using poi name
							XmlNode myMap = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/map");

							if (myMap != null)
							{
								if (myMap["file"].InnerText != "") {
									perd.MapImage = myMap ["file"].InnerText;
								} else {
									perd.MapImage = "none";
								}
							}else{
								#if UNITY_EDITOR
								Debug.LogWarning("Map for "+perd.PoiName+" is Null");
								#endif
							}

							//get scene gps center from movement xml using poi name
							XmlNode myGpsCenter = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/GpsCenter");

							if(myGpsCenter!=null)
							{
								float gpsCenterX = float.Parse(myGpsCenter["lat"].InnerText);
								float gpsCenterY = float.Parse(myGpsCenter["lon"].InnerText);

								perd.GpsCenter = new Vector2(gpsCenterX, gpsCenterY);
							}else{
								#if UNITY_EDITOR
								Debug.LogWarning("GpsCenter for "+perd.PoiName+" is Null");
								#endif
							}

							sceneMapPositions.Add(perd.GpsCenter);
								
							//get scene myMapFilter from movement xml using poi name
//							XmlNode myMapFilter = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/mapFilter");
//
//							if (myMapFilter != null)
//							{
//								if (myMapFilter ["file"].InnerText != "") {
//									perd.MapFilterPaths = myMapFilter ["file"].InnerText;
//								} else {
//									perd.MapFilterPaths = "none";
//								}
//							}else{
//								#if UNITY_EDITOR
//								Debug.LogWarning("MapFilter for "+perd.PoiName+" is Null");
//								#endif
//							}

							//get myMapFullPivot from movement xml using poi name
							XmlNode myMapFullPivot = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/mapFullPivot");

							if(myMapFullPivot!=null){
								perd.MapFullPivot =  new Vector2(float.Parse(myMapFullPivot["x"].InnerText), float.Parse(myMapFullPivot["y"].InnerText));
							}else{
								#if UNITY_EDITOR
								Debug.LogWarning("MapFullPivot for "+perd.PoiName+" is Null");
								#endif
							}


							//get MapFullPosition from movement xml using poi name
							XmlNode myMapFullPosition = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/mapFullPosition");
							
							if(myMapFullPosition!=null){
								perd.MapFullPosition =  new Vector2(float.Parse(myMapFullPosition["x"].InnerText), float.Parse(myMapFullPosition["y"].InnerText));
							}else{
								#if UNITY_EDITOR
								Debug.LogWarning("MapFullPosition for "+perd.PoiName+" is Null");
								#endif
							}


							//get scene gps center from movement xml using poi name
							XmlNode myMapFullZoom = Globals.Instance.movementXml.SelectSingleNode("movement/"+perd.PoiName+"/menu/mapFullZoom");
							
							if(myMapFullPosition!=null){
								perd.MapFullZoom =  new Vector2(float.Parse(myMapFullZoom["x"].InnerText), float.Parse(myMapFullZoom["y"].InnerText));
							}else{
								#if UNITY_EDITOR
								Debug.LogWarning("MapFullZoom for "+perd.PoiName+" is Null");
								#endif
							}

							#endregion
							

							myPeriods.Add (perd);
						}											
					}
				}
				mySceneArea.Periods = myPeriods;

				appData.mySceneAreas.Add(mySceneArea.Name,mySceneArea);
			}
			//Debug.Log(appData.mySceneAreas.Count);
		}
	}

//	float screenRatio;
//	float imageRatio;
//
//	void CalculateRatios()
//	{
//		screenRatio=(float)Screen.width / (float)Screen.height;
//		imageRatio = (float)minimapImg.width/(float)minimapImg.height;
//		
//		if (imageRatio <screenRatio) {
//			mapZoom = (float)Screen.width /(float)minimapImg.width;
//		} else {
//			mapZoom = (float)Screen.height /(float)minimapImg.height;
//		}
//	}

	bool appIsStarted;

	public float fovLimitView = 20f;

	void MoveInfo(){
		if(isAtMenu())
		{
			if(skines.sceneTargets.Count>0f && appIsStarted)
			{
				for(int i=0; i<skines.sceneTargets.Count; i++)
				{
					if(skines.kanvasGroupScenes.alpha!=1f)
					{
						skines.kanvasGroupScenes.alpha=1f;
					}

					Vector3 screenPos = skines.sceneTargets[i].localPosition;

					//position labels x metra up from scene center on map
					Vector2 imgSize = skines.sceneTargets[i].GetComponent<RectTransform>().sizeDelta;
					
					float ff = (imgSize.y/3f);
					screenPos.y+=ff;
					screenPos.z=0f;

					skines.scenesTextInfos[i].localPosition = screenPos;

				}
			}
		}
	}

	public Vector3 followPosSceneLabelHelp()
	{
		int max = skines.scenesTextInfos.Count;//-1;
		if(max>0)
		{
			int rand = UnityEngine.Random.Range(0,max);
			return skines.scenesTextInfos[rand].position;
		}
		return Vector3.zero;
	}


	Vector2 posUser = Vector2.zero;
	Vector2 pinezaPosition ;
	Vector3 accel;

	private string myNearSceneOnoma;

	void MoveStigma(){	

		prevGps = Gps.Instance.GetMyLocation();

//		if(!Diadrasis.Instance.menuUI.helpPanel.activeSelf){
			SetGpsPosMessages();
//		}

		//move user on map
		if(curGPS!=prevGps)
		{
			posUser = FindPositionOnMap(prevGps);
			pinezaPosition = posUser;
						
			#if UNITY_EDITOR
			Debug.Log(posUser);
			Debug.LogWarning("CHECKING POSITION AGAIN");
			#endif

//			personPineza.localPosition = new Vector3(posUser.x, posUser.y, 0f);

			curGPS = prevGps;


		}

//		if(personPineza.gameObject.activeSelf){
//			personPineza.localPosition = new Vector3(posUser.x, posUser.y, 0f);
//			if(skines.labelAbovePerson){
//				skines.labelAbovePerson.transform.position = personPineza.position;
//			}
//		}

//		if(skines.labelAbovePerson){
//			skines.labelAbovePerson.gameObject.SetActive(personPineza.gameObject.activeSelf);
//		}

	}

	void SetGpsPosMessages(){
		//check if user is close in some of our scenes on map
		if(userIsNearScene()==1)
		{
			//TODO hide person on map if near a scene?
			if(!personPineza.gameObject.activeSelf && pinezaPosition!=Vector2.zero){
				personPineza.gameObject.SetActive(true);
			}
			
			#if UNITY_EDITOR
			Debug.Log("You are NEAR to "+Diadrasis.Instance.nearSceneAreaName);
			#endif

			if(myNearSceneOnoma!=Diadrasis.Instance.nearSceneAreaName)
			{
				myNearSceneOnoma = Diadrasis.Instance.nearSceneAreaName;
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_NEAR, true);
			}else{
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_NEAR, false);
			}
		}
		else
			if(userIsNearScene()==2)
		{
			//TODO hide person on map if near a scene?
			if(!personPineza.gameObject.activeSelf && pinezaPosition!=Vector2.zero){
				personPineza.gameObject.SetActive(true);
			}
			
			#if UNITY_EDITOR
			Debug.Log("You are INSIDE to "+Diadrasis.Instance.nearSceneAreaName);
			#endif
			
			if(myNearSceneOnoma!=Diadrasis.Instance.nearSceneAreaName)
			{
				Diadrasis.Instance.AlertSound("bip",3);
				
				myNearSceneOnoma = Diadrasis.Instance.nearSceneAreaName;
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_INSIDE, true);
			}else{
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_INSIDE, false);
			}
		}
		else
		if(userIsNearScene()==0)
		{
//			#if UNITY_EDITOR
//			Debug.Log("USER OUT OF RANGE!!!");
//			Debug.Log(prevGps);
//			#endif
			
			if(personPineza.gameObject.activeSelf){
				personPineza.gameObject.SetActive(false);
			}
			
			if(myNearSceneOnoma!=Diadrasis.Instance.nearSceneAreaName)
			{
				myNearSceneOnoma = Diadrasis.Instance.nearSceneAreaName;
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_FAR, true);
			}else{
				Diadrasis.Instance.menuUI.warningsUI.SetGpsStatus(WarningEventsUI.GpsStatus.ON_FAR, false);
			}

//			Gps.Instance.isAccuracyBig=true;

		}
	}
	
	
	float distFromScene=0f;
	Vector2 nearestScenePosition;
	

	int userIsNearScene()
	{
//		posUser = prevGps; //FindPosition(prevGps);	//Debug.Log(posUser);

		float prevDistance = Mathf.Infinity;

		foreach(Vector2 p in sceneMapPositions)
		{
			if(!Diadrasis.Instance.enableDiadrasisScene && nearSceneName(p).Contains("iadrasis")){
				continue;
			}

			distFromScene = distanceBetweenTwoGpsPoints(prevGps, p); //Vector2.Distance(prevGps,p);//* 3.38f;

			if(distFromScene<prevDistance){
				prevDistance = distFromScene;
				nearestScenePosition = p;
			}

		}


		if(prevDistance<2000f)// && isLookingGPS)
		{
			Diadrasis.Instance.nearSceneAreaName = nearSceneName(nearestScenePosition);
			
			#if UNITY_EDITOR
			Debug.Log("You are "+prevDistance+"meters from "+nearSceneName(nearestScenePosition)); 
			#endif
			
			if(prevDistance<250f){
				//TODO
				//check if user is inside graphic filter area
				return 2;
			}
			
			return 1;
		}


		return 0;
	}

	
	string nearSceneName(Vector2 pos)
	{
		cSceneArea findScene = new cSceneArea();
		
		for(int i=0; i<appData.mySceneAreas.Count; i++)
		{
			findScene = appData.mySceneAreas.ElementAt(i).Value;
			foreach(cPeriod cp in findScene.Periods){
				if(cp.GpsCenter == pos){
					return findScene.LabelTitle + " " +cp.LabelTitle ;
				}
			}
//			if(pos == new Vector2(findScene.MapBtnSizePos.z, findScene.MapBtnSizePos.w)){
//				return findScene.LabelTitle;
//			}
		}

		return string.Empty;
		
	}

	public bool isUserNearToSpecificScene()//(Vector2 centerGpsRegLoc)
	{
		posUser = gpsPosition.FindPosition(prevGps);	
		posUser+= moveSettings.posCenterOfMap;


		#if UNITY_EDITOR
		Debug.Log("AREA check on loading scene");
		Debug.Log(posUser);
		#endif

		if(gpsPosition.PlayerInsideArea(posUser,moveSettings.activeAreaForScene)){

			#if UNITY_EDITOR
			Debug.Log("IN AREA on loading scene");
			#endif

			return true;
		}
			
		return false;
	}

//############################################### help functions for better code reading  ###############################################

	bool isAtMenu(){
		if(Diadrasis.Instance.user==Diadrasis.User.inMenu)
		{
			return true;
		}
		return false;
	}


	public void ClosePeriodView()
	{
		ResetPeriodButtons();
		isShowingPeriod=false;
		ShowPeriods(periodCurrentFather,null,currSceneSelected);
//		Show_RestScenes_AndMakeMapWhite();
	}

	void ResetPeriodButtons()
	{
		if(periodContainers.Count>0)
		{
			foreach(Transform t in periodContainers)
			{
				if(t)
				{
					//reset
					Vector3 pos = t.localPosition;
					pos.y=0f;
					t.localPosition = pos;
					t.localScale=Vector3.zero;
					
					t.gameObject.SetActive(false);
				}
				
			}
		}
	}
	
	void ShowScenePeriods(Transform tr, RectTransform rt)
	{
		//assign container
		periodCurrentFather=periodContainers.Find(t=>t==tr);
		//show container
		if(periodCurrentFather)
		{
			periodCurrentFather.gameObject.SetActive(true);
		}
		//set transform last in hierarchy
		//to be above all others and not hiding from others
		if(rt){
			rt.SetAsLastSibling();
		}
		//start animation for scale and pos y
		isShowingPeriod=true;

		if(periodCurrentFather){
			periodCurrentFather.localPosition = Vector3.zero;
			periodCurrentFather.localScale = Vector3.zero;
		}

		if (rt) {
			Hide_RestScenes_AndMakeMapBlack (rt.transform);
		}
	}


	/*Calculate position with functions and dimensions*/
	Vector2 FindPositionOnMap(Vector2 gpsPos){
		float posX=0;
		float posY=0;	

		//Debug.Log(((Mathf.Abs(synt.reffA_scene.x)+ Mathf.Abs(synt.reffB_scene.x)))/(Mathf.Abs(synt.reffB_gps.y-synt.reffA_gps.y)));

		posX =(gpsPos.y - synt.centerOfmap.y)*((Mathf.Abs(synt.reffA_scene.x)+ Mathf.Abs(synt.reffB_scene.x)))/(Mathf.Abs(synt.reffB_gps.y-synt.reffA_gps.y));
		posX+=moveSettings.gpsOffsetX;
		posY=(gpsPos.x - synt.centerOfmap.x)*((Mathf.Abs(synt.reffA_scene.y)+ Mathf.Abs(synt.reffB_scene.y)))/(Mathf.Abs(synt.reffB_gps.x-synt.reffA_gps.x));
		posY+=moveSettings.gpsOffsetY;
		
		Vector2 pos=new Vector3(posX, posY);
		return pos;		
	}

	void Ektypose(string val){
		#if UNITY_EDITOR
		Debug.LogWarning(val);
		#endif
	}


//	#if UNITY_EDITOR
//	public Vector2 a = new Vector2(37.969344f, 23.733362f);
//	public Vector2 b = new Vector2(37.971774f, 23.726641f);
//	Vector2 omonoia = new Vector2(37.984078f, 23.7278039f);
//	Vector2 diadrasis = new Vector2(37.9882f, 23.7292f);
//	#endif


	///	Haversine formula: 
	/// a = sin²(Δφ/2) + cos φ1 ⋅ cos φ2 ⋅ sin²(Δλ/2)
	///	c = 2 ⋅ atan2( √a, √(1−a) )
	///	d = R ⋅ c
	///	where 	φ is latitude, λ is longitude, R is earth’s radius (mean radius = 6,371km);
	///	note that angles need to be in radians to pass to trig functions!
	/// Προσοχη ! δεν υπολογιζει λοφους - μονο μια flat επιφανεια με την αναλογη καμπυλη
	/// μεταξυ των 2 σημειων σε σχεση με το κεντρο της Γης
	float distanceBetweenTwoGpsPoints(Vector2 a, Vector2 b){
		//διαμετρος της Γης σε μετρα
		float R = 6371f;
		float dLat = (a.x-b.x) * Mathf.Deg2Rad;
		float dLon = (a.y - b.y) * Mathf.Deg2Rad;
		float lat1 = a.x * Mathf.Deg2Rad;
		float lat2 = b.x * Mathf.Deg2Rad;
		
		float s = Mathf.Sin(dLat/2f) * Mathf.Sin(dLat/2f) + Mathf.Sin(dLon/2f) * Mathf.Sin(dLon/2f) * Mathf.Cos(lat1) * Mathf.Cos(lat2); 
		float c = 2f * Mathf.Atan2(Mathf.Sqrt(s), Mathf.Sqrt(1-s)); 
		float d = R * c;

		d = d * 1000f;// mapRekt.sizeDelta.x; //*1000f as map sizedelta or dimensions

		return d;
	}


}
