using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using eChrono;
using Stathis;

//CLASSES SHOWN IN INSPECTOR
#region Serializable Classes

[Serializable]
public class Warnings  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Text gpsInfo;
}

[Serializable]
public class ButtonsMenu  : System.Object//Not a MonoBehaviour!
{
	public Button btnMenu;
	public Button btnMap;
	public Button btnInfo;
	public Button btnSettings;
	public Button btnKameraSettings;
	public Button btnPersonMoveUpDown;
	public Image imgPersonMoveUpDown;
	public Sprite[] btnUpDownSprites;
	public Image imgMenuButton;
	public Sprite[] btnMenuSprites;
	public Button btnHelpOnMAp;
	public Button btnSettingsOnMap;
	public Button btnReturn, btnShowIntroQuestion;

	#region SHOW - HIDE SPECIFIC BUTTON

	public void ShowBtn_Return(bool val){
		if(btnReturn.gameObject.activeSelf!=val)
		btnReturn.gameObject.SetActive (val);
	}

	public void ShowBtn_Menu(bool val){
		//stop or enable animators to hide button
		Diadrasis.Instance.animControl.animMenu.enabled=val;

		//show-hide menu button
		if (btnMenu.transform.parent.gameObject.activeSelf != val) {
			btnMenu.transform.parent.gameObject.SetActive (val);
		}
	}

	public void ShowBtn_Map(bool val){
		if (val && (Diadrasis.Instance.user == Diadrasis.User.inAsanser || (Diadrasis.Instance.user == Diadrasis.User.onAir && !Diadrasis.Instance.moveOnAir))) {
			return;
		}
		if(btnMap.gameObject.activeSelf!=val){
			btnMap.gameObject.SetActive (val);
		}
	}

	public void ShowBtn_Asanser(bool val){
		if(imgPersonMoveUpDown.gameObject.activeSelf!=val)
		imgPersonMoveUpDown.gameObject.SetActive (val);
	}

	public void ShowBtn_KameraSettings(bool val){
		if(btnKameraSettings.gameObject.activeSelf!=val)
		btnKameraSettings.gameObject.SetActive (val);
	}

	/// Shows all btns except menu.
	/// Return - Map - Asanser
	public void ShowAllExceptMenu(bool val){
		ShowBtn_Map (val);
		ShowBtn_Return(val);
		ShowBtn_Asanser (val);
		ShowPoiQuestionButton(val);
	}

	public void ShowPoiQuestionButton(bool val)
    {
		btnShowIntroQuestion.gameObject.SetActive(val);
	}

	#endregion
}

[Serializable]
/// <summary>
/// All Joysticks.
/// </summary>
public class Joysticks  : System.Object//Not a MonoBehaviour!
{
	public GameObject singleJoyLeft;
	public GameObject singleJoyRight;
	public GameObject dualJoys;
}

#endregion

public class MenuUI : MonoBehaviour
{

	#region PUBLIC VARIABLES
	public GameObject gamePanel, alertTime;
	public Text timeText, scoreText;
	[Space(2f)]
	public GameObject titloiTelousPanel;
	//ray layer
	public LayerMask rayLayer=1 << 18;//ray

	public GameObject narrationIsPlaying;

	public GameObject labelsPoiFather;
//	public GameObject labelsMapFather;

	public GameObject menuXartisPanel;
	public GameObject menuPanel;
	public GameObject infoPanel;
	public GameObject xartisPanel;
	public GameObject greditsPanel;
	public GameObject helpPanel;
	public HelpManager helpScript;
	public GameObject loadPanel;
	public Scrollbar loadBar;
	public Image[] loadLines;
	public Text loadText;
	public RawImage loadImage;
	public Text loadValuePercent;

	public GameObject introKeimenoPanel;
	public Text introKeimeno;
	public Text introTitle;
	public Button btnCloseIntroKeimeno;

	public ButtonsMenu btnsMenu;
	public Joysticks joy;
	public Warnings warnings;

	public Xartis xartis ;
	public AnimControl animControl;

	public SettingsUI settingsUI;
	public WarningEventsUI warningsUI;

	public InfoUI infoUI;

	public bool isNarrPlaying;
	public AudioSource narrationSource;
	public AudioClip narrationClip;


	public static List<Transform> poiMnimeia = new List<Transform>();
	public List<RectTransform> labelsMnimeia = new List<RectTransform>();
//	public List<RectTransform> labelsMnimeiaMap = new List<RectTransform>();

	public float maxLabelDist = Mathf.Infinity;
	public float addToYPerson = 1.5f;

	#endregion

	#region PRIVATE VARIABLES
	RectTransform currLabelHit;
	Camera currKamera;
	ButtonScene poiScript;
	cPoi currentPoi;
	Ray ray;
	RaycastHit hit = new RaycastHit ();
	public string draftCurrentKeyPoiName;
	#endregion
	
	#region MANUAL SETTINGS QUALITY
	/*
	void Awake(){
		#if UNITY_ANDROID
		Application.targetFrameRate = 30;

		QualitySettings.vSyncCount = 0; 
		
		QualitySettings.antiAliasing = 0;

		if (QualitySettings.GetQualityLevel()<3)
		{
			QualitySettings.shadowCascades = 0;
			QualitySettings.shadowDistance = 15;
		}
		
		else if (QualitySettings.GetQualityLevel()>=3)
		{
			QualitySettings.shadowCascades = 2;
			QualitySettings.shadowDistance = 70;
		}
		#endif
	}
	*/
	#endregion
	
	#region START & DELEGATES
	
	void Start () {
		xartis=GetComponent<Xartis>();
		animControl = GetComponent<AnimControl>();
		infoUI = infoPanel.GetComponent<InfoUI>();
		helpScript = helpPanel.GetComponent<HelpManager> ();
		
		//set action for menu buttons
		btnsMenu.btnMenu.onClick.AddListener(()=>animControl.MenuButton());
		btnsMenu.btnMap.onClick.AddListener(()=>animControl.ShowSmallMap());
		xartis.btnsMap.closeMap.onClick.AddListener(()=>animControl.CloseMap());
		btnsMenu.btnInfo.onClick.AddListener(()=>HelpShow());
		btnsMenu.btnHelpOnMAp.onClick.AddListener(()=>HelpShow());
		
		btnsMenu.btnSettings.onClick.AddListener(()=>animControl.SettingsButton());
		//		btnsMenu.btnSettings.onClick.AddListener(()=>settingsUI.ShowSettings(true));
		
		//		btnsMenu.btnSettingsOnMap.onClick.AddListener(()=>animControl.SettingsButton());
		btnsMenu.btnPersonMoveUpDown.onClick.AddListener(()=>AsanserPerson());
		btnsMenu.btnReturn.onClick.AddListener (() => Diadrasis.Instance.EscapeCheck ());
		
		btnsMenu.btnKameraSettings.onClick.AddListener(()=>settingsUI.OpenKameraSettings());
		
		narrationIsPlaying.GetComponent<Button>().onClick.AddListener(()=>NarrationShow());
		
		maxLabelDist = Mathf.Infinity;

		btnCloseIntroKeimeno.onClick.AddListener(()=>CloseIntroKeimeno());

		btnsMenu.btnShowIntroQuestion.onClick.AddListener(ShowQuestion);
	}

	void ShowQuestion()
    {
		introKeimeno.text = appSettings.language == "en" ? GameManager.Instance.poiSearchingNow.questionEn : GameManager.Instance.poiSearchingNow.questionGR;
		//show intro keimeno
		introKeimenoPanel.SetActive(true);
	}

	void OnEnable()
	{
		Lean.LeanTouch.OnFingerTap += OnFingerTap;

		Diadrasis.Instance.OnSceneLoadEnd += MenuButtonsShow;
		Diadrasis.Instance.OnSceneLoadStart += PrepareLoadingElements;
		Diadrasis.Instance.OnSceneLoadStart += MenuButtonsHide;
		Diadrasis.Instance.OnMapFullClose += MenuButtonActive;
		Diadrasis.Instance.OnMapFullShow += MenuButtonInactive;
		animControl.OnInfoShow += InfoPanel_isOpen;
		animControl.OnInfoClose += InfoPanel_isClosed;
		//in offsite mode if user close full map 
		//on tap to move dont gain info
		xartis.OnFullMapTap+=InfoDontShow;

		settingsUI.OnLanguangeChange += ChangeLanguange;
	}
	
	
	void OnDisable()
	{
		Lean.LeanTouch.OnFingerTap -= OnFingerTap;

		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.OnSceneLoadEnd -= MenuButtonsShow;
			Diadrasis.Instance.OnSceneLoadStart -= PrepareLoadingElements;
			Diadrasis.Instance.OnSceneLoadStart -= MenuButtonsHide;
			Diadrasis.Instance.OnMapFullClose -= MenuButtonActive;
			Diadrasis.Instance.OnMapFullShow -= MenuButtonInactive;
			animControl.OnInfoShow -= InfoPanel_isOpen;
			animControl.OnInfoClose -= InfoPanel_isClosed;
			xartis.OnFullMapTap-=InfoDontShow;

			settingsUI.OnLanguangeChange -= ChangeLanguange;
		}
	}

	#endregion

	void ChangeLanguange(){
		if(narrationSource && narrationSource.isPlaying){
			PlayNarrOnLangChange();
		}
	}

	#region INTRO & LOADING

	//set user status after intro keimeno readed
	//to stop user interaction with map or joystick or gps move
	void CloseIntroKeimeno(){
        
		//show full map if off site
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			GameManager.Instance.OnIntroMessageClosed();			
		}
		else
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
		{
			//set user current status
			//Diadrasis.Instance.ChangeStatus(Diadrasis.User.isNavigating);
			//hide load panel
//			loadPanel.GetComponent<CanvasGroup>().alpha=0f;
//			loadPanel.SetActive(false);
			CanvasGroup cg = Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.GetComponent<CanvasGroup> ();
			Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (false);
			Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.transform.SetSiblingIndex (2);
			RawImage dd = Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.GetComponent<RawImage> ();
			dd.texture = null ;
			cg.alpha = 0.75f;
		}
		
		Diadrasis.Instance.introIsClosed=true;
	}

	public void ResetIntroKeimenoScroll(){
		//reset full desc text scroll position
		Vector3 pos = introKeimeno.GetComponent<RectTransform>().position;
		pos.y=0f;
		introKeimeno.GetComponent<RectTransform>().position = pos;
	}

	void PrepareLoadingElements(){
		//hide intro text until scene is full loaded
		introKeimenoPanel.SetActive(false);

		ResetIntroKeimenoScroll();

		//reset loading properties
		loadLines [0].fillAmount = 0f;
		loadLines [1].fillAmount = 0f;
		loadBar.size = 0f;
		string zero = "0%";
		loadValuePercent.text = zero;
		loadPanel.GetComponent<CanvasGroup> ().alpha = 0f;

		SetLoadTextImage();

		//show loading panel
		loadPanel.SetActive(true);
	}

	void SetLoadTextImage()
	{
		string txt = Diadrasis.Instance.loadingText;
		string img = Diadrasis.Instance.loadingImage;

		//check if current period has intro texts
		if(!string.IsNullOrEmpty(txt)){
			loadText.text = txt;
		}else{
			loadText.text = string.Empty;
		}
		//set current period loading image
		loadImage.texture = Tools_Load.LoadTexture(appSettings.loadImagePath, img);// LoadTexture(loadingImage);
	}

	#endregion
	
	#region UPDATE
	
	void Update ()
	{
		//if is in menu dont raycast
		if(Application.loadedLevel==0){return;}

		//if we have person camera
		//and is inside a scene find mnimeia
		if(Diadrasis.Instance.kamera)
		{
			//if camera exists and in ground or allow on air movement do raycast to read info
			if(((Diadrasis.Instance.user==Diadrasis.User.isNavigating) ||(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)))
			{
				if(!labelsPoiFather.activeSelf){labelsPoiFather.SetActive(true);}

				HideAllLabels(true);
				
				ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width/2, Screen.height / 2, -1f));
				if (Physics.Raycast(ray, out hit, moveSettings.regDistance)){//,rayLayer.value)){
					if (hit.collider.transform)
					{
						Transform cTransform = hit.collider.transform;

						//check if the point has hotspotattached
						if (CheckIfPointExists(cTransform.name)) {
							MovePoiLabels(cTransform);
							return;
						}
					}
				}

				if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
					draftCurrentKeyPoiName=string.Empty;
				}

				return;
			}
			else//if on air and not allowed to move show all poi labels but do not enable on tap anywhere to read info
			if(Diadrasis.Instance.user==Diadrasis.User.onAir && !Diadrasis.Instance.moveOnAir)
			{
				if(!labelsPoiFather.activeSelf){labelsPoiFather.SetActive(true);}

				if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
					draftCurrentKeyPoiName=string.Empty;
				}

				MovePoiLabels(null);

				return;
				
			}
			else
			{
				if(labelsPoiFather.activeSelf){labelsPoiFather.SetActive(false);}

				if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
					draftCurrentKeyPoiName=string.Empty;
				}
			}
		}
	}
	
	#endregion

	#region CreateMnimeiaLabels
	
	/// <summary>
	/// Creates the poi labels.
	/// </summary>
	public void CreateMnimeiaLabels()
	{
		//		Debug.Log("labelsMnimeia = "+labelsMnimeia.Count);
		
		//TODO
		//may use a pool method and not a destroy one
		//destroy previus scene's labels
		//if they are exist
		if (labelsMnimeia.Count > 0) {
			foreach(RectTransform t in labelsMnimeia)
			{
				if(t)
				{
					DestroyImmediate(t.gameObject);
				}
			}
		}
		labelsMnimeia.Clear();
		
		Diadrasis.Instance.olaTaKeimena_PoiLabels.Clear();
		
		if(poiMnimeia.Count>0)
		{
			for(int i=0; i<poiMnimeia.Count; i++)
			{//titlosPoi
				GameObject lbl = Instantiate(Resources.Load(appSettings.poiPrefabsPath + "titlosPoi")) as GameObject;
				
				if(!lbl){return;}
				
				lbl.name = poiMnimeia[i].name;
				lbl.transform.SetParent(labelsPoiFather.transform);
				lbl.transform.localScale=new Vector3(1f,1f,1f);
				lbl.transform.localPosition = Vector3.zero;
				
				ButtonScene lblScript = lbl.GetComponent<ButtonScene>();
				
				if (appData.myPoints.ContainsKey(lbl.name))
				{
					cPoi myPoi;
					appData.myPoints.TryGetValue(lbl.name, out myPoi);
					
					lblScript.label.text =  myPoi.title;
					
					Diadrasis.Instance.olaTaKeimena_PoiLabels.Add(lblScript.label);
					lblScript.label.fontSize = Mathf.FloorToInt(Diadrasis.Instance.menuUI.settingsUI.langPanel.fontSize.value);
					
					lblScript.keyPoi = poiMnimeia[i].name;
					
					if(myPoi.ShowtInfo)
					{
						if(Diadrasis.Instance.useOnSiteMode || Diadrasis.Instance.showPoiInfo || (Diadrasis.Instance.allowSceneGps==Diadrasis.Instance.sceneName))
						{
							lblScript.myBtn.onClick.AddListener(()=>lblScript.SetPoi());
							lblScript.myBtn.onClick.AddListener(ShowPoiInfo);
							//lblScript.myBtn.onClick.AddListener(()=>infoUI.SetShortDesc());
							//lblScript.myBtn.onClick.AddListener(()=>animControl.ShortDescShow());
						}
					}else{
						lblScript.myBtn.enabled=false;
					}
				}
				
				lblScript.langNow=appSettings.language;
				
				labelsMnimeia.Add(lblScript.rt);
			}

			#region MAP POI LABELS
			/*
			
			//if they are exist
			if (labelsMnimeiaMap.Count > 0) {
				foreach(RectTransform t in labelsMnimeiaMap)
				{
					if(t)
					{
						DestroyImmediate(t.gameObject);
					}
				}
			}
			labelsMnimeiaMap.Clear();

			
			if(poiMnimeia.Count>0)
			{
				//Debug.Log("mnimeia = "+poiMnimeia.Count);
				
				for(int i=0; i<poiMnimeia.Count; i++)
				{//titlosPoi
					GameObject lbl = Instantiate(Resources.Load(appSettings.poiPrefabsPath + "titlosPoi")) as GameObject;
					
					if(!lbl){return;}

					Color xroma = lbl.GetComponent<Image>().color;
					xroma.a = 0f;
					lbl.GetComponent<Image>().color = xroma;

					lbl.name = poiMnimeia[i].name;
					lbl.transform.SetParent(labelsMapFather.transform);
					lbl.transform.localScale=new Vector3(1f,1f,1f);
					lbl.transform.localPosition = Vector3.zero;

					float rand = UnityEngine.Random.Range(0.25f, 0.75f);

					lbl.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.75f);
					
					ButtonScene lblScript = lbl.GetComponent<ButtonScene>();
					
					if (appData.myPoints.ContainsKey(lbl.name))
					{
						cPoi myPoi;
						appData.myPoints.TryGetValue(lbl.name, out myPoi);
						
						lblScript.label.text =  myPoi.title;
						lblScript.label.color = Color.black;

						if(Diadrasis.Instance.isTablet){
							lblScript.label.fontSize=16;
						}else{
							lblScript.label.fontSize=21;
						}
						
						Diadrasis.Instance.olaTaKeimena_PoiLabels.Add(lblScript.label);
						lblScript.label.fontSize = Mathf.FloorToInt(Diadrasis.Instance.menuUI.settingsUI.langPanel.fontSize.value);
						
						lblScript.keyPoi = poiMnimeia[i].name;
						
//						if(!string.IsNullOrEmpty(myPoi.desc))
//						{
//							lblScript.myBtn.onClick.AddListener(()=>lblScript.SetPoi());
//							lblScript.myBtn.onClick.AddListener(()=>infoUI.SetShortDesc());
//							lblScript.myBtn.onClick.AddListener(()=>animControl.ShortDescShow());
//						}else{
//							lblScript.myBtn.enabled=false;
//						}
					}
					
					lblScript.langNow=appSettings.language;
					
					labelsMnimeiaMap.Add(lblScript.rt);
				}


				if(poiMnimeia.Count == labelsMnimeiaMap.Count)//bug fix argument
				{
					for(int i=0; i<poiMnimeia.Count; i++)
					{
						if(poiMnimeia[i])
						{
							//move map to target posision
							Vector3 poiPos = poiMnimeia[i].position;
							
							#if UNITY_EDITOR
							Debug.LogWarning(poiMnimeia[i].name+" poiPos = "+poiPos);
							#endif
							
							poiPos.x -= moveSettings.posCenterOfMap.x;
							poiPos.z -= moveSettings.posCenterOfMap.y;
							
							#if UNITY_EDITOR
							Debug.LogWarning(poiMnimeia[i].name+" poiPos 2 = "+poiPos);
							#endif
							
							Vector3 mapPos = new Vector3(poiPos.x * xartis.zoomLogos ().x , poiPos.z * xartis.zoomLogos ().y , 0f);
							
							#if UNITY_EDITOR
							Debug.LogWarning(poiMnimeia[i].name+" mapPos = "+mapPos);
							#endif
							
							labelsMnimeiaMap[i].localPosition = mapPos;// mapRect.localPosition;
						}
					}
				}


			}
*/
				#endregion


		}
	}

	void ShowPoiInfo()
    {
		if(Application.isEditor) Debug.LogWarning("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        if (Diadrasis.Instance.isGameMode)
        {
            if (GameManager.Instance.isGameStarted)
            {
				if (GameManager.Instance.isGameCompleted)
                {
					if (Application.isEditor) Debug.Log("###############@@@@@@@@@@@ @@@@@@@@@@@@@@ @@@@@@@@@@@@@@");
					infoUI.SetShortDesc();
					animControl.ShortDescShow();
                }
                else
                {
					GameManager.Instance.CheckAnswer();
				}
			}
        }
        else
        {
			infoUI.SetShortDesc();
			animControl.ShortDescShow();
        }
    }
	
	#endregion

	#region TAP ANYWHERE TO GET INFO

	bool isTapCorrect=false;
	bool isInfoOpen = false;

	void InfoPanel_isClosed(){
		isInfoOpen=false;
		if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
			draftCurrentKeyPoiName=string.Empty;
		}
	}

	void InfoPanel_isOpen(){
		isInfoOpen=true;
	}

	void InfoDontShow(){
		isInfoOpen=true;

		StopCoroutine("ttt");
		StartCoroutine(ttt());
	}
	
	IEnumerator ttt(){
		yield return new WaitForSeconds(0.7f);
		InfoPanel_isClosed();
		yield break;
	}

	public void OnFingerTap(Lean.LeanFinger finger)
	{
		if (!Diadrasis.Instance.useOnSiteMode && !Diadrasis.Instance.showPoiInfo && Diadrasis.Instance.allowSceneGps!=Diadrasis.Instance.sceneName) {
			isTapCorrect=false;
			return;
		}

		//bug fix on site on tap intro keimeno close.. is shows info if ray is shooting at poi
		if (Diadrasis.Instance.navMode == Diadrasis.NavMode.onSite) {
			if (Diadrasis.Instance.introIsClosed) {
				isTapCorrect=false; 
				Diadrasis.Instance.introIsClosed = false; 
				return;
			}
		}

		//bug fix to let tap anywhere for poi info
		if(isInfoOpen || string.IsNullOrEmpty(draftCurrentKeyPoiName) || Tools_UI.IsPointerOverUIObjectTag("JOY") || introKeimenoPanel.activeSelf)
		{
			isTapCorrect=false; 
			return;
		}
		//Debug.LogWarning("AGE = "+finger.Age);
		//Debug.LogWarning("TAP = "+finger.Tap);
		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
		{
			if(finger.Tap && finger.Age<0.175f){
				//single tap
				isTapCorrect=true;
			}
		}else{
			isTapCorrect=false;
		}

		if(finger.Up){

			if(isInfoOpen){return;}

			if (Diadrasis.Instance.escapeUser != Diadrasis.EscapeUser.inHelp) {//if help on wait to close first and tap again
				if (Diadrasis.Instance.user == Diadrasis.User.isNavigating || (Diadrasis.Instance.user == Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)) {
					if (isTapCorrect && !string.IsNullOrEmpty (draftCurrentKeyPoiName)) {
						SetPoi ();
						ShowPoiInfo();
						//infoUI.SetShortDesc ();
						//animControl.ShortDescShow ();
						//Diadrasis.Instance.CheckAnswer();
						//Debug.LogWarning("############################################");
					}
				} else {
					isTapCorrect = false;
				}
			}

			isTapCorrect=false;
			
		}
	}

	public void SetPoi()
	{
		Diadrasis.Instance.currentPoi = draftCurrentKeyPoiName;
	}

	#endregion

	#region LABELS Functions

	void HideAllLabels(bool toBeHide){//, bool forMap){
//		if(!forMap){
			foreach(RectTransform lbl in labelsMnimeia){
				if(lbl.gameObject.activeSelf==toBeHide){
					lbl.gameObject.SetActive(!toBeHide);
				}
			}
//		}else{
//			foreach(RectTransform lbl in labelsMnimeiaMap){
//				if(lbl.gameObject.activeSelf==toBeHide){
//					lbl.gameObject.SetActive(!toBeHide);
//				}
//			}
//		}
	}

	public void LabelsSetLanguange(){
		
		foreach(Transform t in labelsPoiFather.transform)
		{
			if(t)
			{
				poiScript = t.GetComponent<ButtonScene>();
				if(poiScript){
					poiScript.SetLanguange();
				}
			}
		}
	}
	
	public void ClearPoiLabels(){
		if (labelsMnimeia.Count > 0) {
			foreach(RectTransform t in labelsMnimeia)
			{
				if(t)
				{
					DestroyImmediate(t.gameObject);
				}
			}
		}
		labelsMnimeia.Clear();
	}

	#endregion

	#region Move POI Labels 

	///poi labels movement relative to camera
	void MovePoiLabels(Transform hittedPoi)
	{

		if(!currKamera){currKamera = Diadrasis.Instance.kamera;}
		if(currKamera==null){return;}


		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
		{
//			if(maxLabelDist != 30f){maxLabelDist = 30f;}
			if(addToYPerson != 1.5f){addToYPerson = 1.5f;}
		}



		if(poiMnimeia.Count>0 && labelsMnimeia.Count>0)
		{

			if(hittedPoi != null){//show current label

				foreach(RectTransform rt in labelsMnimeia){
					if(rt.GetComponent<ButtonScene>().keyPoi == hittedPoi.name){
						draftCurrentKeyPoiName=hittedPoi.name;
						currLabelHit=rt;

						if(currLabelHit.localPosition!=Vector3.zero){
							currLabelHit.localPosition = Vector3.zero;// new Vector3(Screen.width/2f, Screen.width/2f, 0f); //screenPos;
						}
						
						currLabelHit.gameObject.SetActive(true);

						break;
					}
				}

				#region UnUsed Code

				//get mnimeiou position
//				Vector3 nPos = hittedPoi.position;
				
				//anebase to label (title) higher
				//an blepei me kamera na meinei sto ypsos tou mnimeiou + 1.5 metro
//				if(Diadrasis.Instance.isCamNavigation){
////					nPos.y = nPos.y + addToYPerson;
//				}else//na paei sto ypsos tou person + 1.5 metro
//				{
////					nPos.y = Diadrasis.Instance.person.transform.position.y + addToYPerson;
//				}
				
//				Vector3 screenPos = currKamera.WorldToScreenPoint(nPos);
//				//		Debug.Log("target is " + screenPos.x + " pixels from the left");
//				Vector3 fwd=currKamera.transform.TransformDirection(Vector3.forward);
//				Vector3 bkw=hittedPoi.position-currKamera.transform.position;

				#endregion

//				if(currLabelHit.localPosition!=Vector3.zero){
//					currLabelHit.localPosition = Vector3.zero;// new Vector3(Screen.width/2f, Screen.width/2f, 0f); //screenPos;
//				}
//
//				currLabelHit.gameObject.SetActive(true);

				if(!currLabelHit.gameObject.activeSelf){
					if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
						draftCurrentKeyPoiName=string.Empty;
					}
				}
				

			}else//show all labels at mnimeia position
			if(hittedPoi == null)
			{
				//clear poi key for next raycast
				if(!string.IsNullOrEmpty(draftCurrentKeyPoiName)){
					draftCurrentKeyPoiName=string.Empty;
				}

				if(poiMnimeia.Count == labelsMnimeia.Count)//bug fix argument
				{

					HideAllLabels(false);

					for(int i=0; i<poiMnimeia.Count; i++)
					{
						if(poiMnimeia[i] && currKamera)
						{
							//get mnimeiou position
							Vector3 poiPos = poiMnimeia[i].position;

							//anebase to label (title) higher
							poiPos.y = poiPos.y + addToYPerson;

							//get world to scren position
							Vector3 screenPos = currKamera.WorldToScreenPoint(poiPos);

							//get camera's looking direction
							Vector3 fwd=currKamera.transform.TransformDirection(Vector3.forward);
							//get camera's opposite looking direction
							Vector3 bkw=poiMnimeia[i].position-currKamera.transform.position;

							//check if person is viewing label
							//and hide or show
							if(Vector3.Angle(bkw,fwd)<45f){
								labelsMnimeia[i].position = screenPos;
								labelsMnimeia[i].gameObject.SetActive(true);
							}else{
								labelsMnimeia[i].gameObject.SetActive(false);
							}
						}
					}
				}
			}

			#region Poi Names On Full Map 
//			else
//			if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)
//			{
//				if(poiMnimeia.Count == labelsMnimeiaMap.Count)//bug fix argument
//				{
//					HideAllLabels(false, true);
//				}
//			}

			#endregion

		}//end of searching all labels
	}

	bool checkIfOtherPoiIsNearest()
	{
		return false;
	}

	//if help is selected show label tap help
	public RectTransform labelToFollowOnHelp()
	{
		if(labelsMnimeia.Count>0)
		{
			foreach(RectTransform rt in labelsMnimeia)
			{
				if(rt.gameObject.activeSelf)
				{
					return rt;
				}
			}
		}
		return null;
	}

	#endregion

	#region BUTTONS

	public void HideAllJoys(){
		joy.dualJoys.SetActive(false);
		joy.singleJoyLeft.SetActive(false);
		joy.singleJoyRight.SetActive(false);
	}

	#region QUIT BUTTON

	public void QuitTimeWalk(){
		titloiTelousPanel.SetActive (true);
	}

	#endregion

	#region ASANSER BUTTON

	void AsanserPerson()
	{
		if(Diadrasis.Instance.person!=null){
			Diadrasis.Instance.person.SendMessage("MovePersonUpDown",SendMessageOptions.DontRequireReceiver);
		}
		#if UNITY_EDITOR
		else{
			Debug.Log("person is null - asanser want work!!!");
		}
		#endif
	}

	#endregion

	#region BUTTONS PANEL
	
	/// hide all buttons in 4 corners of screen
	public void MenuButtonsShow()
	{
		menuPanel.SetActive(true);
		#if UNITY_EDITOR
		Debug.Log("MenuButtonsShow");
		#endif
	}
	public void MenuButtonsHide()
	{
		menuPanel.SetActive(false);
		#if UNITY_EDITOR
		Debug.Log("MenuButtonsHide");
		#endif
	}

	#endregion

	#region HELP

	public void HelpShow()
	{
		#if UNITY_EDITOR
		Debug.Log("Help");
		#endif

		warningsUI.HideGpsWarnings();
		Diadrasis.Instance.prevEscapeUser = Diadrasis.Instance.escapeUser;
		Diadrasis.Instance.escapeUser = Diadrasis.EscapeUser.inHelp;
		helpScript.Init ();
	}

	public void HelpShow(bool dontCloseIfTap)
	{
		#if UNITY_EDITOR
		Debug.Log("Help");
		#endif

		warningsUI.HideGpsWarnings();
		Diadrasis.Instance.prevEscapeUser = Diadrasis.Instance.escapeUser;
		Diadrasis.Instance.escapeUser = Diadrasis.EscapeUser.inHelp;
		helpScript.dontCloseIfTap = dontCloseIfTap;
		helpScript.Init ();
	}

	#endregion

	#region MENU BUTTON
	
	public void SetMenuBtnImage(int indx)
	{
		btnsMenu.imgMenuButton.overrideSprite = btnsMenu.btnMenuSprites[indx];
	}
	
	public void MenuButtonActive()
	{
		#if UNITY_EDITOR
		Debug.Log("Do we Need it ? ->> ActiveMenuButton");
		#endif
		
		btnsMenu.btnMenu.transform.parent.GetComponent<Image>().color=Color.white;
		btnsMenu.btnMenu.interactable=true;
		btnsMenu.btnMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts=true;
	}

	public void MenuButtonInactive()
	{
		#if UNITY_EDITOR
		Debug.Log("Do we Need it ? ->> InactiveMenuButton");
		#endif
		
		btnsMenu.btnMenu.transform.parent.GetComponent<Image>().color=Color.clear;
		//stop executing a function
		btnsMenu.btnMenu.interactable=false;
		//stop from tap a ray for person trasportation
		btnsMenu.btnMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts=false;
	}

	#endregion

	#region NARRATION PLAYER

	public void NarrationShow(){							
		#if UNITY_EDITOR
		Debug.Log("NarrationShow");
		#endif
		
		narrationIsPlaying.SetActive(!isNarrPlaying);
		
		NarrationPlay();
		
	}

	public void PlayNarrOnLangChange()
	{
		if(CheckIfPointExists(Diadrasis.Instance.currentPoi))
		{
			if (currentPoi.narrations.Count > 0)
			{
				if(Diadrasis.Instance.ixos)
				{
					if(!narrationSource){
						narrationSource = Diadrasis.Instance.ixos;
					}
				}
				if(!string.IsNullOrEmpty(currentPoi.narrations [0].file)){
					PlayNarration (currentPoi.narrations [0].file);
				}
			}
		}
	}

	public void NarrationPlay()
	{
		//narration
		if(CheckIfPointExists(Diadrasis.Instance.currentPoi))
		{
			if (currentPoi.narrations.Count > 0) {
				if (!isNarrPlaying) {
					if(Diadrasis.Instance.ixos)
					{
						if(!narrationSource){
							narrationSource = Diadrasis.Instance.ixos;
						}
					}
					if(!string.IsNullOrEmpty(currentPoi.narrations [0].file)){
						PlayNarration (currentPoi.narrations [0].file);
					}
				}
				else
				{
					StopNarration ();
				}
			}
		}
	}

	public void PlayNarration(string soundfile){
		narrationClip = (AudioClip)Resources.Load(appSettings.audioPath + soundfile + appSettings.language);

		if(!narrationSource)
		{
			if(Diadrasis.Instance.ixos){
				narrationSource = Diadrasis.Instance.ixos;
			}
		}

//		if(!isNarrPlaying){
			if(narrationSource && narrationClip)
			{												
			#if UNITY_EDITOR
			Debug.Log("PlayNarration "+narrationClip.name);
			#endif
//			Diadrasis.Instance.akoi.enabled=true;
			Diadrasis.Instance.ixos.volume=1f;
			Diadrasis.Instance.ixos.clip = narrationClip;
			Diadrasis.Instance.ixos.Play();
			}
			isNarrPlaying=true;
//		}else{
//			if(narrationSource && narrationClip)
//			{												Debug.Log("PlayNarration from Pause"+narrationClip.name);
//				narrationSource.volume=1f;
//				narrationSource.Play();
//			}
//		}
	}

	public void NarrationPause(){
		if(isNarrPlaying){
			if(narrationSource && narrationClip)
			{												
				#if UNITY_EDITOR
				Debug.Log("Pause Narration "+narrationClip.name);
				#endif
//				Diadrasis.Instance.akoi.enabled=false;
				Diadrasis.Instance.ixos.volume=0f;
				Diadrasis.Instance.ixos.Pause();
			}
		}
	}
	
	public void StopNarration(){
//		Diadrasis.Instance.akoi.enabled=false;
		narrationSource.volume=0f;
		narrationSource.Stop();
		isNarrPlaying = false;
	}

	#endregion

	#region VIDEO PLAYER

	public void VideoShow(){							

		#if UNITY_EDITOR
		Debug.Log("VideoShow");
		#endif

		if(isNarrPlaying)
		{
			StopNarration();
			narrationIsPlaying.SetActive(false);
		}

		if(CheckIfPointExists(Diadrasis.Instance.currentPoi))
		{
			if(currentPoi.videos.Count>0)
			{
				ShowVideo(currentPoi.videos[0].file);
			}

		}
	}


	void  ShowVideo (string path)	{
		#if UNITY_EDITOR
		Debug.Log("now playing " + "videos/" + path + ".mp4");
		#endif

		#if UNITY_ANDROID || UNITY_IPHONE
		Handheld.PlayFullScreenMovie ("videos/" + path + ".mp4", Color.black, FullScreenMovieControlMode.Full);
		#endif
		
	}

	#endregion

	#endregion

	#region HELP CLASSES

	bool CheckIfPointExists (String onoma){

		if (appData.myPoints.ContainsKey(onoma))
		{
			cPoi myPoi;
			appData.myPoints.TryGetValue(onoma, out myPoi);
			currentPoi = myPoi;
			return true;
		}

		return false;
	}

	#endregion

}
