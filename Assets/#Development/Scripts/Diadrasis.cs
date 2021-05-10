using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Stathis;
using eChrono;
using System;

public class Diadrasis : Singleton<Diadrasis>
{
	private Diadrasis(){}

	#region DELEGATES
	
	public delegate void ActionSceneLoading();
	//Load Start
	public event ActionSceneLoading OnSceneLoadStart;
	//Load End parts
	public event ActionSceneLoading OnSceneLoadEnd;

	public delegate void ActionPause();
	//Pause true
	public event ActionPause OnPauseTrue;
	//pause false
	public event ActionPause OnPauseFalse;
	
	public delegate void ActionMap();
	//full map close
	public event ActionMap OnMapFullClose;
	//full map show
	public event ActionMap OnMapFullShow;

	#endregion

	#region PUBLIC VARIABLES

	public float currScreenDiagonios;

	public string appVersion;

	public bool isFirstHelpMenu;

	public bool isGameMode;
	public bool isTesting;
	public bool gyroExists;
	public bool accelExists;
	public bool gpsIsOn;
	public bool internetIsOn = true;
	public bool updateXml;
	public bool moveOnAir=true;
	//should lezanta of image have full width?
	public bool infoImageTextFullWdth=false;
	public bool useMapFilterForMovement;
	public bool useOnSiteMode;
	public bool showPoiInfo=true;
	public bool enableDiadrasisScene;
	public bool useCharacterControllerToPerson;
	public string allowSceneGps;

	public List<AudioSource> allSounds = new List<AudioSource>();
	public List<Text> olaTaKeimena_Settings = new List<Text>();
	public List<Text> olaTaKeimena_PoiLabels = new List<Text>();
	public List<Text> olaTaKeimena_Info = new List<Text>();

	//isInPause --> when app is minimized by user or an external application
	//isIdle ?? --> when user is not touching the screen for about 5 min 
	//				then we enable screen sleep (by user default) (battery saver)
	//				and also we disable all using sensors
	public enum User{inMenu, isNavigating, inPoi, inSettings, inHelp, inPause, isIdle, inLoading, inFullMap, inGredits, inWarning, onAir, inQuit, inAsanser, inShop, inBetaWarning, inDeviceCheckWarning}
	public User user = User.isIdle;

	//for checking previus status in common situations like help-info-warning-pause-settings
	public enum UserPrin{inMenu, isNavigating, inPoi, inSettings, inHelp, inPause, isIdle, inLoading, inFullMap, inGredits, inWarning, onAir, inQuit, inAsanser}
	public UserPrin userPrin = UserPrin.inMenu;

	//escape button options
	public enum EscapeUser{inMenu, isNavigating, inPoi, inSettings, inHelp, inFullMap, inWarning, onAir, inQuit, inAsanser,isIdle, inShop}
	public EscapeUser escapeUser = EscapeUser.inMenu;
	public EscapeUser prevEscapeUser = EscapeUser.inMenu;

	public enum MenuStatus{idle, mapMove, mapZoom, periodView, mapLerping}
	public MenuStatus menuStatus = MenuStatus.idle;

	public enum ZoomStatus{idle, zoomIN, zoomOut}
	public ZoomStatus zoomStatus = ZoomStatus.idle;

	//which sensor can use
	public enum SensorAvalaible{empty, gyroscopio, accelCompass};
	public SensorAvalaible sensorAvalaible = SensorAvalaible.empty;

	//which sensor is using
	public enum SensorUsing{joysticks, gyroscopio, accelCompass};
	public SensorUsing sensorUsing = SensorUsing.joysticks;

	//which sensor is using
	public enum LanguangeNow {gr, en};
	public LanguangeNow languangeNow = LanguangeNow.gr;

	//which navigation mode is selected by user
	public enum NavMode {onSite, offSite};
	public NavMode navMode = NavMode.offSite;

	public MenuUI menuUI;
	public XartisMenu xartisMenu;
	public AnimControl animControl;

	public GameObject objDontDestroy ;
	public GameObject person;
	public Camera kamera;
	public CharacterMotorC cMotor;


	public RectTransform canvasMainRT;

	public string nearSceneAreaName;
	
	public string sceneName ;
	public string XmlPointsTagName;
	public string currentPoi;
	public string introText, loadingText, introTitle;
	public string loadingImage;

	public List<cIntroNarration> loadingIntroAudio = new List<cIntroNarration>();

	public string mapScene;
	//public string mapFilter;
	public Vector2 mapPivot;
	public Vector2 mapFullPosition;
	public Vector2 mapFullZoom;

	public Vector2 sceneGpsPosition;

	public int isStart=0;
	public int appEntrances=0;

	public float idleTime = 100f;

	public AudioSource ixos;
//	public AudioListener akoi;
	public AudioListener akoiPerson;
	AudioListener speaker;

	public int screenSize;

	//[SerializeField]
	//public List<PoiQuest> poiQuestions = new List<PoiQuest>();
	//public PoiQuest poiSearchingNow;
	//public bool isGameStarted, isGameInstructionVisible, isGameCompleted;
	//public float gameTotalTime;
	//public int gameTotalScore;

	//public bool HasCurrentSceneQuestion() { return poiQuestions.Count > 0 && isGameMode; }

	//public void CheckAnswer()
 //   {
	//	if (isGameMode && isGameStarted && poiSearchingNow != null && !isGameCompleted)
	//	{
	//		if (currentPoi == poiSearchingNow.key)
	//		{
	//			if (poiQuestions.Count > 0)
	//			{
	//				Debug.LogWarning("Next Question");
	//				poiQuestions.Remove(poiSearchingNow);

	//				gameTotalScore += 150;

	//				ShowRandomQuestion();
	//				ShowIntroGamePoiFoundMessage();
	//			}
 //               else
 //               {
	//				Debug.LogWarning("YOU WIN !!!!!!!!!!!!!!!!!!!");
	//				ShowIntroGameWinMessage();
	//			}
	//		}
	//		else
	//		{
	//			gameTotalTime -= 300f;
	//			gameTotalScore -= 50; 
	//			if (gameTotalScore < 0) gameTotalScore = 0;

	//			Debug.LogWarning("NO NO NO - Try another poi");
	//		}
	//	}
 //   }

	//void ShowIntroGamePoiFoundMessage()
	//{
	//	introText = appData.FindTerm_text("poiFound");

	//	menuUI.introKeimeno.text = introText;
	//	menuUI.introTitle.text = introTitle;

	//	menuUI.introKeimeno.fontSize = appSettings.fontSize_keimeno;
	//	menuUI.introTitle.fontSize = appSettings.fontSize_titlos;

	//	//show intro keimeno
	//	menuUI.introKeimenoPanel.SetActive(true);

	//	menuUI.btnsMenu.ShowPoiQuestionButton(false);

	//	isGameCompleted = true;
	//}

	//void ShowIntroGameWinMessage()
	//{
	//	introText = appData.FindTerm_text("win_game");

	//	menuUI.introKeimeno.text = introText;
	//	menuUI.introTitle.text = introTitle;

	//	menuUI.introKeimeno.fontSize = appSettings.fontSize_keimeno;
	//	menuUI.introTitle.fontSize = appSettings.fontSize_titlos;

	//	//show intro keimeno
	//	menuUI.introKeimenoPanel.SetActive(true);

	//	menuUI.btnsMenu.ShowPoiQuestionButton(false);

	//	isGameCompleted = true;
	//}

	//void ShowIntroGameOverMessage()
	//{
	//	introText = appData.FindTerm_text("game_over");

	//	menuUI.introKeimeno.text = introText;
	//	menuUI.introTitle.text = introTitle;

	//	menuUI.introKeimeno.fontSize = appSettings.fontSize_keimeno;
	//	menuUI.introTitle.fontSize = appSettings.fontSize_titlos;

	//	//show intro keimeno
	//	menuUI.introKeimenoPanel.SetActive(true);

	//	menuUI.btnsMenu.ShowPoiQuestionButton(false);

	//	isGameCompleted = true;
	//}

	//public void ShowRandomQuestion()
	//{
	//	int rand = UnityEngine.Random.Range(0, poiQuestions.Count);
	//	poiSearchingNow = poiQuestions[rand];

	//	introText = appSettings.language == "en" ? poiSearchingNow.questionEn : poiSearchingNow.questionGR;

	//	menuUI.introKeimeno.text = introText;
	//	menuUI.introTitle.text = introTitle;

	//	menuUI.introKeimeno.fontSize = appSettings.fontSize_keimeno;
	//	menuUI.introTitle.fontSize = appSettings.fontSize_titlos;

	//	//show intro keimeno
	//	menuUI.introKeimenoPanel.SetActive(true);
	//}

	#endregion

	#region PRIVATE VARIABLES

	//async variables
	private AsyncOperation async = null;
	public bool introIsClosed;
	
	#endregion

	#region INITIALIZE

	public void Init () {

		currScreenDiagonios = Methods.DeviceDiagonalSizeInInches();

		if (currScreenDiagonios > appSettings.screenLargeInches) {
			screenSize = 2;

			appSettings.fontSize_titlos = appSettings.bigScreenFontSize_titlos;
			appSettings.fontSize_keimeno = appSettings.bigScreenFontSize_keimeno;

		} else if (currScreenDiagonios > appSettings.screenMediumInches && currScreenDiagonios < appSettings.screenLargeInches) {
			screenSize = 1;

			appSettings.fontSize_titlos = appSettings.mediumScreenFontSize_titlos;
			appSettings.fontSize_keimeno = appSettings.mediumScreenFontSize_keimeno;

		} else {
			screenSize = 0;

			appSettings.fontSize_titlos = appSettings.mediumScreenFontSize_titlos;
			appSettings.fontSize_keimeno = appSettings.smallScreenFontSize_keimeno;
		}

		//set first enter so returning to menu
		//from  other scenes do not OnStart class
		//make canvas 2d again
		isStart = 0;

		//dont dimm screen 
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		appEntrances = PlayerPrefs.GetInt("appEntrances");

		isStart++;
		appEntrances++;

		PlayerPrefs.SetInt("appEntrances",appEntrances);

		PlayerPrefs.Save();

		#if UNITY_EDITOR
		Debug.Log("isStart = "+isStart);
		Debug.Log("appEntrances = "+appEntrances);
		#endif
	}

	#endregion

	#region ONPAUSE

	//if app is paused or minimized by user or opened externel app
	private void OnApplicationPause(bool pauseStatus){
		if(pauseStatus)
		{
			//user=User.inPause;	
			#if UNITY_EDITOR
			Debug.Log("pause");
			#endif

			if(OnPauseTrue !=null){
				OnPauseTrue();
			}
		}
		else
		{
//			Gps.Instance.status=Gps.Status.IDLE;
//			Diadrasis.Instance.nearSceneAreaName= Random.Range(0,10000).ToString();
			//check gps status
//			Gps.Instance.CheckGPS();
			#if UNITY_EDITOR
			Debug.Log("unPause");
			#endif

			if(OnPauseFalse != null){
				OnPauseFalse();
			}
		}
	}

	#endregion

	#region LATE UPDATE

	void LateUpdate () 
	{
		//if user dont touch the screen for more than 1 minute
//		IdleManagement();

		if(user==User.inMenu)
		{
			CheckTouches();
		}

		#if !UNITY_IOS
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			EscapeCheck();
		}
		#endif

		if(user==User.inLoading)
		{
			if(async!=null)
			{
				if(menuUI.loadLines[0].fillAmount<1f){
					menuUI.loadLines[0].fillAmount+=Time.deltaTime /2f;
				}else{
					if(menuUI.loadLines[1].fillAmount<1f){
						menuUI.loadLines[1].fillAmount+=Time.deltaTime / 2f;
					}
				}
				menuUI.loadBar.size = async.progress + 0.1f;
				menuUI.loadValuePercent.text = Mathf.RoundToInt(async.progress * 100f).ToString()+"%";
			}
		}
	}

	#endregion

	#region ESCAPE CHECK

	public void EscapeCheck()
	{
		#if UNITY_EDITOR
		Debug.Log("starting escape check");
		#endif


		switch(escapeUser)
		{
			case(EscapeUser.inMenu):
			#if UNITY_EDITOR
			Debug.Log("in menu escape check");
			#endif
				switch(menuStatus)
				{
					case(MenuStatus.idle):
						//reset map position to center and field of view to max
						if(xartisMenu.needsToResetMapAndZoom()){
							#if UNITY_EDITOR
							Debug.Log("reseting map");
							#endif
							return;
						}
						//show warning message "close application?"
						ChangeStatus(User.inQuit);

						#if UNITY_EDITOR
						Debug.Log("menu tap on quit");
						#endif

						animControl.MenuButton();
						break;
					case(MenuStatus.mapMove):
						
						break;
					case(MenuStatus.mapZoom):
						
						break;
					case(MenuStatus.periodView):
						#if UNITY_EDITOR
						Debug.Log("Close Periods from escape.");
						#endif
						//close current period view
						xartisMenu.ClosePeriodView();
						//status is idle
						menuStatus=MenuStatus.idle;
						break;
				}
				break;
			case(EscapeUser.isNavigating):

				if(Diadrasis.Instance.isStart<3){
					//show message "return to menu?"
					if(!menuUI.warningsUI.quitScenePanel.activeSelf){
						menuUI.warningsUI.quitScenePanel.SetActive(true);
					}
					else
					{
						menuUI.warningsUI.quitScenePanel.SetActive(false);
					}
				}else{
					Diadrasis.Instance.ReturnToMainMenu();
				}
				break;
			case(EscapeUser.onAir):
				//move person on ground
				person.SendMessage("MovePersonUpDown",SendMessageOptions.DontRequireReceiver);
				break;
			case(EscapeUser.inHelp):
				//close current help
				#if UNITY_EDITOR
				Debug.Log("escape help");
				#endif

				menuUI.helpScript.CloseHelps();//.helpPanel.SendMessage("CloseHelps",SendMessageOptions.DontRequireReceiver);
//				menuUI.helpPanel.SetActive(false);
				CheckStatus();
				break;
			case(EscapeUser.inSettings):
				//close settings
				animControl.MenuButton();

				#if UNITY_EDITOR
				Debug.Log("escape settings");
				#endif

				break;
			case(EscapeUser.inWarning):
				//close current warning
				
				break;
			case(EscapeUser.inPoi):
				//close stadiaka info poi (if is in photos return to short and then close info)
				animControl.MenuButton();

				#if UNITY_EDITOR
				Debug.Log("escape info");
				#endif

				break;
			case(EscapeUser.inFullMap):
				if(navMode==NavMode.offSite){
					//close full map
					#if UNITY_EDITOR
					Debug.Log("close full map if its not the 1st time");
					#endif
					if(Diadrasis.Instance.isStart<3){
						//show message "return to menu?"
						if(!menuUI.warningsUI.quitScenePanel.activeSelf){
							menuUI.warningsUI.quitScenePanel.SetActive(true);
						}
						else
						{
							menuUI.warningsUI.quitScenePanel.SetActive(false);
						}
					}else{
						Diadrasis.Instance.ReturnToMainMenu();
					}
//					animControl.MenuButton();

				}else{
					if(Diadrasis.Instance.isStart<3){
						//show message "return to menu?"
						if(!menuUI.warningsUI.quitScenePanel.activeSelf){
							menuUI.warningsUI.quitScenePanel.SetActive(true);
						}
						else
						{
							menuUI.warningsUI.quitScenePanel.SetActive(false);
						}
					}else{
						Diadrasis.Instance.ReturnToMainMenu();
					}
				}
				break;
			case(EscapeUser.inQuit):
				//close quit message
				CheckUserPrin();
				break;
			case(EscapeUser.inShop):
				//close current warning
				menuUI.warningsUI.BuyCancel(true);
			break;
			default:
				#if UNITY_EDITOR
				Debug.Log("NO CODITION escape check");
				#endif
			break;
		}
	}

	#endregion

	#region RETURNING TO MENU FROM A SCENE

	public void ReturnToMainMenu()
	{
		#if UNITY_EDITOR
		Debug.Log("ReturnToMainMenu!!!");
		#endif

		sceneName="main";

		//stop all Sounds
		if(allSounds.Count>0){
			foreach(AudioSource ad in allSounds){
				ad.Stop ();
			}
			allSounds.Clear();
		}

		StopCoroutine("closeFullMapOnSite");
		StartCoroutine("closeFullMapOnSite");
	}


	IEnumerator closeFullMapOnSite()
	{
		#if UNITY_EDITOR
		Debug.Log("close ONSITE map AND RETURN TO MENU!!!");
		#endif

		//close menu radial if is on
		animControl.CloseRadialMenu();

		menuUI.ClearPoiLabels ();

		//TODO
		//hide info panel ?
		menuUI.infoPanel.SetActive(false);

		if (!Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.activeSelf) {
			RawImage dd = menuUI.warningsUI.introScript.omixli.GetComponent<RawImage> ();
			dd.texture = Tools_Load.LoadTexture(appSettings.loadImagePath, loadingImage) ;

			CanvasGroup cg = menuUI.warningsUI.introScript.omixli.GetComponent<CanvasGroup> ();
			cg.alpha = 0f;



			menuUI.warningsUI.introScript.omixli.transform.SetAsLastSibling ();

			menuUI.warningsUI.introScript.omixli.SetActive (true);

			//fade out
			while(cg.alpha<0.95f)
			{
				cg.alpha = Mathf.Lerp(cg.alpha,1f,Time.deltaTime * 4f);

				yield return null;
			}

			//fade out full
			cg.alpha = 1f;

			yield return new WaitForSeconds (0.25f);

		}

		//close map
		if (menuUI.xartis.mapStatus == Xartis.MapStatus.Full) {
			
			Diadrasis.Instance.menuUI.xartis.HideHelps ();

			//hide mikro xarti
			animControl.CloseMap ();

			//wait animation to finished
			yield return new WaitForSeconds (0.7f);
		} else {
			//hide mikro xarti
			animControl.CloseMap ();
			//wait animation to finished
			yield return new WaitForSeconds (0.7f);
		}

		if(person)
		{
			person.transform.SetParent(null);
			
			if(menuUI.narrationSource && menuUI.narrationSource.isPlaying){
				if(menuUI.isNarrPlaying)
				{
					menuUI.StopNarration();
					menuUI.narrationIsPlaying.SetActive(false);
				}
			}
		}

		//destroy camera??
		person=null;

		//set status in loading
		user=User.inLoading;
		
		//keep ui + scripts on every scene
		DontDestroyOnLoad(objDontDestroy);

		menuUI.joy.dualJoys.SetActive(false);
		menuUI.joy.singleJoyLeft.SetActive(false);
		menuUI.joy.singleJoyRight.SetActive(false);

		//show menu button
		menuUI.MenuButtonActive();

		//load menu instantly
		Application.LoadLevel(sceneName);

//		if (Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.activeSelf) {
//			Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (false);
//		}

		//stop
		yield break;
	}


	#endregion

	#region LOAD SCENE

	public void LoadScene(){		//Debug.Log("loadin scene");

		//delegate scene start loading
		if(OnSceneLoadStart != null){
			OnSceneLoadStart();
		}

		DontDestroyOnLoad(objDontDestroy);

		animControl.CloseRadialMenu();

		StartCoroutine(loadSkini());
	}
			
	private IEnumerator loadSkini(){

		//wait 
		yield return new WaitForSeconds(0.25f);

		#region FADE IN LOADING PANEL

		//fade in loading panel
		while(menuUI.loadPanel.GetComponent<CanvasGroup>().alpha<0.99f)
		{
			menuUI.loadPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(menuUI.loadPanel.GetComponent<CanvasGroup>().alpha,1f,Time.deltaTime * 2f);	
			yield return null;
		}

		#endregion

		#region NARRATIONS PART 1

		//check if current period has intro narrations
		if(loadingIntroAudio.Count>0){
			if(!string.IsNullOrEmpty(loadingIntroAudio[0].File))
			{
				menuUI.PlayNarration(loadingIntroAudio[0].File);

				#if UNITY_EDITOR
				//pause until scene is load's full
				Debug.Log("introPauseTime ="+loadingIntroAudio[0].PauseTime);
				#endif

				if(loadingIntroAudio[0].PauseTime>0f){
					yield return new WaitForSeconds(loadingIntroAudio[0].PauseTime);
				
					menuUI.NarrationPause();
				}
			}
		}

		#endregion

		#region START LOADING NEW SCENE AND WAIT UNTIL IS FULL LOADED

		//load the selected period scene
		async = Application.LoadLevelAsync(sceneName);

		//wait until scene is loaded
		while (!async.isDone) {
//			Debug.Log(async.progress);
			yield return async;	
		}

		#endregion

		//if scene is loaded wait 
		yield return new WaitForSeconds(0.5f);

		//reset async for next use
		async=null;

		#region ADD PERSON
		//add camera and person to scene
		AddPerson();

		//if person exists
		//person is auto assign to Diadrasis.Instance
		//when function AddPerson is executing
		if(person)
		{
			//enable person
			person.SetActive(true);

			//if we are in editor
			//show al paths ad areas from xml
			//into scene view from top down view 
			#if UNITY_EDITOR
			if(!person.GetComponent<DrawPaths>()){
				person.AddComponent<DrawPaths>();
			}
			#endif

			//if navigation is off-site 
			//set person's position from xml
			if(navMode==NavMode.offSite)
			{
				person.transform.position = new Vector3(moveSettings.startCamPosition.x,person.transform.position.y,moveSettings.startCamPosition.y);
			}
		}

		#endregion

		//wait
		yield return new WaitForSeconds(0.2f);

		#region NARRATIONS PART 2 & INTRO KEIMENA

		//set float to save waiting text-audio reading
		//in case we have only one big text
		//and we separated into 2 parts
		float narrationTimeRemaining=0;

		//if audio was paused continue playing
		if(loadingIntroAudio.Count==1){
			if(loadingIntroAudio[0].PauseTime>0f){
//				akoi.enabled=true;
				if(akoiPerson){
					akoiPerson.enabled=false;
				}
				ixos.volume=1f;
				ixos.time=loadingIntroAudio[0].PauseTime;
				ixos.Play();
				narrationTimeRemaining=ixos.clip.length-loadingIntroAudio[0].PauseTime;

				#if UNITY_EDITOR
				Debug.Log("audio time remaining = "+narrationTimeRemaining);
				#endif
			}
		}
		else // play new audio if exists
		if(loadingIntroAudio.Count>1){
			if(!string.IsNullOrEmpty(loadingIntroAudio[1].File))
			{
				menuUI.PlayNarration(loadingIntroAudio[1].File);

				#if UNITY_EDITOR
				Debug.Log("introPauseTime ="+loadingIntroAudio[1].PauseTime);
				#endif

				if(loadingIntroAudio[1].PauseTime>0f){
					yield return new WaitForSeconds(loadingIntroAudio[1].PauseTime);
//					akoi.enabled=false;
					ixos.Pause();
				}
			}
		}

		if (GameManager.Instance.HasCurrentSceneQuestion())
		{
			GameManager.Instance.CreateGameManager();
			GameManager.Instance.StartNewGame();
		}
		else
		{
			menuUI.btnsMenu.ShowPoiQuestionButton(false);
			//show intro text -- scene is full loaded
			//check if current period has intro texts
			if (!string.IsNullOrEmpty(introText))
			{
				menuUI.introKeimeno.text = introText;
				menuUI.introTitle.text = introTitle;

				menuUI.introKeimeno.fontSize = appSettings.fontSize_keimeno;
				menuUI.introTitle.fontSize = appSettings.fontSize_titlos;

				//show intro keimeno
				menuUI.introKeimenoPanel.SetActive(true);
			}
			else
			{
				menuUI.introKeimeno.text = string.Empty;
				menuUI.introTitle.text = string.Empty;
				//hide intro keimeno
				menuUI.introKeimenoPanel.SetActive(false);
			}
		}

		#endregion
		
		//delegate scene has loaded
		if(OnSceneLoadEnd != null){
			OnSceneLoadEnd();
		}

		//put this after delegate
		introIsClosed=false;

		#region MAP SETTINGS

		//show full map if off site
		if(navMode==NavMode.offSite)
		{
			FullScreenMap();
			//dont show the first time the posision of person on full map
			menuUI.xartis.personFullMap.gameObject.SetActive(false);
			menuUI.xartis.personFullVelaki.gameObject.SetActive(false);
		}
		else
		if(navMode==NavMode.onSite)
		{
			//show menu button
			menuUI.MenuButtonActive();
		}

		#endregion

		//reset menu status
		menuStatus=MenuStatus.idle;

		//wait
		yield return new WaitForSeconds(0.5f);

		#region FADE OUT & CLOSE LOADING PANEL
		//hide load panel
		if(navMode==NavMode.offSite){
			//fade out
			while(menuUI.loadPanel.GetComponent<CanvasGroup>().alpha>0.01f)
			{
				menuUI.loadPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(menuUI.loadPanel.GetComponent<CanvasGroup>().alpha,0f,Time.deltaTime * 5f);
				yield return null;
			}


			menuUI.loadPanel.SetActive(false);
		}else
		if(navMode==NavMode.onSite){

			CanvasGroup cg = menuUI.warningsUI.introScript.omixli.GetComponent<CanvasGroup> ();
			cg.alpha=1f;

			RawImage dd = menuUI.warningsUI.introScript.omixli.GetComponent<RawImage> ();
			dd.texture = Tools_Load.LoadTexture(appSettings.loadImagePath, loadingImage) ;

			menuUI.warningsUI.introScript.omixli.transform.SetSiblingIndex (13);

			menuUI.warningsUI.introScript.omixli.SetActive(true);

			menuUI.loadPanel.GetComponent<CanvasGroup>().alpha=0f;
			menuUI.loadPanel.SetActive(false);

			//set user current status
			ChangeStatus(Diadrasis.User.isNavigating);
		}

		#endregion

		#region CREATE POI LABELS

		//create poi labels
		menuUI.CreateMnimeiaLabels();
		//enable info panel
		menuUI.infoPanel.SetActive(true);

		#endregion

		#region WAIT NARRATION 2nd PART TO FINISH PLAYING

		//wait until reading of intro has finished
		//because load panel is hided
		//this text is showing with the scene in backround
		//if exists
		yield return new WaitForSeconds(narrationTimeRemaining);

		//stop narration intro
		if(menuUI.narrationSource && menuUI.narrationSource.isPlaying){
			if(menuUI.isNarrPlaying)
			{
				menuUI.StopNarration();
				menuUI.narrationIsPlaying.SetActive(false);
			}
		}

		#endregion

		#region CREATE SOUNDS OF LOADED SCENE
		//if scene has sounds 
		//create all sounds
		CreatePoints.InitSounds();

		#endregion

		//stop
		yield break;
	}

	

	#endregion

	#region FULL MAP

	public void FullMapClose(){

		#if UNITY_EDITOR
		Debug.Log("FullMapClose");
		#endif

		if(OnMapFullClose != null){
			OnMapFullClose();
		}
	}

	public void FullScreenMap()
	{
		if(user!=User.inFullMap && user!=User.inAsanser && user!=User.onAir || (user==User.onAir && moveOnAir))
		{
			#if UNITY_EDITOR
			Debug.Log("FullScreenMap");
			#endif

			if(OnMapFullShow != null){
				OnMapFullShow();
			}

			if(introIsClosed){
				//set user current status
				ChangeStatus(User.inFullMap);
				escapeUser=EscapeUser.inFullMap;
				//userPrin=UserPrin.isNavigating;
			}
		}
	}

	#endregion

	#region CALL PLUGINS

	public void PluginsInit()
	{
		//get gps
		Gps.Instance.warningsInfo = menuUI.warningsUI;
		//after intro finished
//		Gps.Instance.Init();
		
		Internet.Instance.Init();
	}

	#endregion

	#region CHECK DEVICE AVALAIBLE SENSORS

	//only at start of app
	public void CheckSensors(){
		//check if device have gyroscope
		if(SystemInfo.supportsGyroscope){
			if(!Input.gyro.enabled){
				Input.gyro.enabled=true;
			}
			sensorAvalaible=SensorAvalaible.gyroscopio;
			sensorUsing=SensorUsing.gyroscopio;
		}else
			//check if device have accelerometer
		if(SystemInfo.supportsAccelerometer){
			sensorAvalaible=SensorAvalaible.accelCompass;
			sensorUsing=SensorUsing.accelCompass;
		}else{
			sensorAvalaible=SensorAvalaible.empty;
			sensorUsing=SensorUsing.joysticks;
		}
	}

	#endregion

	#region ADD PERSON (DEPENDS ON SENSOR)
	Vector3 oldPersonPosition;
	
	public void AddPerson()
	{
		//check if device have gyroscope
		if(sensorUsing==SensorUsing.gyroscopio){

			if(person==null || person && person.name != "Person_Gyro")
			{
				DestroyPerson("Person_Gyro");
				//instantiate person
				if(!useCharacterControllerToPerson){
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonGyro")) as GameObject;
				}else{
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonGyro_Moto")) as GameObject;
				}
				person.name="Person_Gyro";

				SetNewPerson();
			}
		}else
		//check if device have accelerometer
		if(sensorUsing==SensorUsing.accelCompass){
			if(person==null || person && person.name != "Person_Accel")
			{
				DestroyPerson("Person_Accel");
				//instantiate person
				if(!useCharacterControllerToPerson){
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonAccel")) as GameObject;
				}else{
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonAccel_Moto")) as GameObject;
				}
				person.name="Person_Accel";

				SetNewPerson();
			}
		}
		else
		if(sensorUsing==SensorUsing.joysticks)
		{
			if(person==null || person && person.name != "Person_Joysticks")
			{
				DestroyPerson("Person_Joysticks");
				//instantiate person
				if(!useCharacterControllerToPerson){
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonJoys")) as GameObject;
				}else{
					person = Instantiate(Resources.Load("prefabs/FINAL/person/PersonJoys_Moto")) as GameObject;
				}
				person.name="Person_Joysticks";

				SetNewPerson();
			}
		}

		//TODO
		//if with kamera onsite maybe disable person camera???
	}

	void SetNewPerson(){
		//move new person to old person position
		person.transform.position = oldPersonPosition;
		
		person.SetActive(true);
		
		person.AddComponent<CreatePoints>();
		person.AddComponent<ControlPerson>();
		
		person.SendMessage("ResetKamera");
	}

	void DestroyPerson(string name){
		if(person && person.name!=name)
		{
			oldPersonPosition = person.transform.position;
			Destroy(person);
			person=null;
			kamera=null;
			#if UNITY_EDITOR
			Debug.LogWarning("Destroy Person");
			#endif
		}
	}

	#endregion

	#region STATUS CONTROL

	#region CHECK USER STATUS

	void CheckStatus()
	{
		switch(user)
		{
			case(User.inMenu):
				CheckTouches();
				userPrin=UserPrin.inMenu;
				escapeUser=EscapeUser.inMenu;

				//change radial menu buttons image
				menuUI.SetMenuBtnImage(0);

				if((Diadrasis.Instance.appEntrances==1 && Diadrasis.Instance.isStart>1) || (Diadrasis.Instance.appEntrances>1)){
					Diadrasis.Instance.menuUI.warningsUI.ShowCommentButton();
				}

				GameManager.Instance.ResetVariables();

				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.btnsMenu.ShowBtn_Return(true);
				menuUI.btnsMenu.ShowBtn_Menu(true);
				//if(isStart==1 && appEntrances==1){
					menuUI.warningsUI.HideShopButton();
				//}else{
				//	menuUI.warningsUI.CheckPurchases();
				//}
				
				menuUI.warningsUI.HideCommentButton();
				break;
			case(User.isNavigating):
					
				menuUI.xartis.CheckMapPreviusStatus();
				menuUI.warningsUI.HideCommentButton();
				userPrin=UserPrin.isNavigating;
				escapeUser=EscapeUser.isNavigating;

				//change radial menu buttons image
				menuUI.SetMenuBtnImage(1);

				menuUI.btnsMenu.ShowBtn_Asanser(true);
				menuUI.btnsMenu.ShowBtn_Return(true);
				menuUI.btnsMenu.ShowBtn_Menu(true);
				if(GameManager.Instance.HasCurrentSceneQuestion() && !GameManager.Instance.isGameCompleted) menuUI.btnsMenu.ShowPoiQuestionButton(true);
				menuUI.warningsUI.HideShopButton();

				break;
			case(User.onAir):
				//set map previus status
				if(moveOnAir){
					menuUI.xartis.CheckMapPreviusStatus();
					//menuUI.btnsMenu.ShowBtn_Return(true);
					//menuUI.btnsMenu.ShowBtn_Map(true);
					escapeUser=EscapeUser.isNavigating;
					menuUI.btnsMenu.ShowBtn_Return(true);
				}else{
					escapeUser=EscapeUser.onAir;
					//menuUI.btnsMenu.ShowBtn_Map(false);
				}
				menuUI.warningsUI.HideCommentButton();

				userPrin=UserPrin.onAir;
				menuUI.btnsMenu.ShowBtn_Asanser(true);
				menuUI.btnsMenu.ShowBtn_Menu(true);
				break;
			case(User.inAsanser):
				menuUI.warningsUI.HideCommentButton();

				escapeUser=EscapeUser.inAsanser;
				menuUI.btnsMenu.ShowBtn_Asanser(false);
				
				if(moveOnAir){
					menuUI.xartis.SetMapPreviusStatus();
				}
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.btnsMenu.ShowBtn_Menu(false);
				break;
			case(User.inHelp):
				escapeUser=EscapeUser.inHelp;
				//menuUI.warningsUI.HideShopButton();
				//menuUI.warningsUI.HideCommentButton();
				break;
			case(User.inShop):
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.btnsMenu.ShowBtn_Menu(false);
				menuUI.warningsUI.HideShopButton();
				menuUI.warningsUI.HideCommentButton();
				escapeUser=EscapeUser.inShop;
				break;
			case(User.inPause):
				break;
			case(User.inSettings):
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.warningsUI.HideShopButton();
				menuUI.warningsUI.HideCommentButton();
				escapeUser=EscapeUser.inSettings;
				break;
			case(User.inPoi):
				escapeUser=EscapeUser.inPoi;
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.warningsUI.HideCommentButton();
				menuUI.warningsUI.HideShopButton();
				break;
			case(User.isIdle):
				break;
			case(User.inLoading):
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.warningsUI.HideShopButton();
				menuUI.warningsUI.HideCommentButton();
				break;
			case(User.inFullMap):
				//userPrin=UserPrin.inFullMap;
				escapeUser=EscapeUser.inFullMap;
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.warningsUI.HideShopButton();
				menuUI.warningsUI.HideCommentButton();
				//if(navMode==NavMode.onSite){
					menuUI.btnsMenu.ShowBtn_Return(true);
				//}
				break;
			case(User.inQuit):
				userPrin=UserPrin.inQuit;
				escapeUser=EscapeUser.inQuit;
				menuUI.btnsMenu.ShowAllExceptMenu(false);
				menuUI.warningsUI.HideShopButton();
				menuUI.warningsUI.HideCommentButton();
				break;
			case(User.inBetaWarning):
				
				break;
			case(User.inDeviceCheckWarning):
				
				break;
		}
	}

	#endregion


	public void ChangeStatus(User status)
	{
		#if UNITY_EDITOR
		Debug.Log("ChangeStatus to "+status.ToString());
		#endif
		user=status;
		CheckStatus();
	}

	#region CHECK USER STATUS PREVIUS CONTITION
	public void CheckUserPrin()
	{															
		#if UNITY_EDITOR
		Debug.Log("CheckUserPrin !!!!!");
		#endif


		switch(userPrin)
		{
			case(UserPrin.inMenu):
				ChangeStatus(User.inMenu);
				break;
			case(UserPrin.isNavigating):
				ChangeStatus(User.isNavigating);
				break;
			case(UserPrin.onAir):
				ChangeStatus(User.onAir);
				break;
			case(UserPrin.inQuit):
				ChangeStatus(User.inMenu);
				break;
			case(UserPrin.inFullMap):
				ChangeStatus(User.inFullMap);
				break;
			case(UserPrin.inPoi):
				if(user==User.isIdle || user==User.inPause)
				ChangeStatus(User.inPoi);
				break;
			case(UserPrin.inSettings):
				if(user==User.isIdle || user==User.inPause)
				ChangeStatus(User.inSettings);
				break;
			case(UserPrin.inHelp):
				if(user==User.isIdle || user==User.inPause)
				ChangeStatus(User.inHelp);
				break;
			case(UserPrin.inWarning):
				if(user==User.isIdle || user==User.inPause)
				ChangeStatus(User.inWarning);
				break;
		}
	}

	#endregion

	#endregion

	#region CHECK TOUCHES
	int daxtyla=0;
			
	void CheckTouches()
	{
		daxtyla = Lean.LeanTouch.Fingers.Count;
		
		#region IN MENU
		//if(user==User.inMenu)
		//{
//		#if UNITY_EDITOR
//		Debug.LogWarning("IsPointerOverUIObject DIADRASIS");
//		#endif
			if(menuStatus!=MenuStatus.periodView && !Tools_UI.IsPointerOverUIObject())
			{
				if(daxtyla==0)
				{
					if(menuStatus!=MenuStatus.idle)
					menuStatus=MenuStatus.idle;
				}
				else
				if(daxtyla==1)
				{
					if(menuStatus!=MenuStatus.mapMove){
						menuStatus=MenuStatus.mapMove;
					}

					Diadrasis.Instance.menuUI.btnsMenu.btnReturn.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1f;

					menuUI.warningsUI.HideGpsWarnings();

					animControl.CloseRadialMenu();
				}
				else
				if(daxtyla==2)
				{
					if(menuStatus!=MenuStatus.mapZoom){
						menuStatus=MenuStatus.mapZoom;
					}

					Diadrasis.Instance.menuUI.btnsMenu.btnReturn.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1f;

					animControl.CloseRadialMenu();
				}
			}
		//}
		#endregion
		/*
		else
		if(user==User.isNavigating || (user==User.onAir && moveOnAir))
		{
			
			if(daxtyla==0)
			{

			}
			else
			if(daxtyla==1)
			{
				#if UNITY_EDITOR
				Debug.LogWarning("TAP FINGER !!!");
				#endif
			}
			else
			if(daxtyla==2)
			{

			}
		}
		*/
		#region IN NAVIGATION

		#endregion
	}

	#endregion

	#region ALERT USER (ONSTITE) IF IS NEAR A SCENE

	bool oncePlayVibrate;

	public void AlertSound(string clip, int vibrationTimes){

		if(oncePlayVibrate==true){return;}

		oncePlayVibrate=true;
		AudioClip clip1 = (AudioClip) Resources.Load("audio/"+clip);
		ixos.PlayOneShot(clip1);
		StopCoroutine("playVibrate");
		StartCoroutine("playVibrate",vibrationTimes);
	}

	IEnumerator playVibrate(int times){
		for(int i=0; i<times; i++)
		{
			#if !UNITY_EDITOR
			Handheld.Vibrate();
			#else
			Debug.LogWarning("VIBRATING!!!");
			#endif
			yield return new WaitForSeconds(0.3f);
		}

		yield return new WaitForSeconds(3f);
		oncePlayVibrate=false;

		yield break;
	}

	#endregion
	
	#region RETURN FROM SLEEP (BETA TEST)
	/*

	/// <summary>
	/// Change status if user return from sleep status (if not touching the screen for x seconds)
	/// </summary>
	void ReturnFromSleepStatus()
	{
		if(escapeUser==EscapeUser.inPoi)
		{
			ChangeStatus(User.inPoi);
		}
		else
		if(escapeUser==EscapeUser.inSettings)
		{
			ChangeStatus(User.inSettings);
		}
		else
		if(escapeUser==EscapeUser.inFullMap)
		{
			ChangeStatus(User.inFullMap);
		}
		else
		if(escapeUser==EscapeUser.onAir)
		{
			ChangeStatus(User.onAir);
		}
		else
		if(escapeUser==EscapeUser.isNavigating)
		{
			ChangeStatus(User.isNavigating);
		}
		else
		if(escapeUser==EscapeUser.inHelp)
		{
			ChangeStatus(User.inHelp);
		}
		else
		if(escapeUser==EscapeUser.inShop)
		{
			ChangeStatus(User.inShop);
		}
	}
	*/
	
	/*

	bool isHelpEmfanistike=false;

	void IdleManagement()
	{
		if(idleTime>0f)
		{
			idleTime-=Time.deltaTime;
		}else{
			idleTime=-100;
		}

		//show help after 60sec of idle time and auto close
		if(idleTime<=45f && idleTime>=40f && !isHelpEmfanistike)
		{
			menuUI.HelpShow();
			isHelpEmfanistike=true;
		}
		
		if(idleTime==-100f)
		{
			if(Screen.sleepTimeout==SleepTimeout.NeverSleep)
			{
				Screen.sleepTimeout=SleepTimeout.SystemSetting;
			}

			if(isHelpEmfanistike){isHelpEmfanistike=false;}

			#if UNITY_EDITOR
			Debug.LogWarning("do we need user to be idle??");
//			user=User.isIdle;
			#endif

			idleTime=-10;
		}
		
		//if any finger is touching the screen 
		if(Lean.LeanTouch.Fingers.Count>0)
		{
			menuUI.warningsUI.HideGpsWarnings();

			//stop sleeping screen
			if(Screen.sleepTimeout!=SleepTimeout.NeverSleep){
				Screen.sleepTimeout=SleepTimeout.NeverSleep;
			}
			//reset idlw time
			if(idleTime!=100f){idleTime=100f;}

			if(isHelpEmfanistike){isHelpEmfanistike=false;}
			
			//return user to previus status
			if(user==User.isIdle){
				ReturnFromSleepStatus();
			}
			else
			if(user==User.inHelp)
			{
				CheckUserPrin();
			}
		}
	}

	*/
	
	#endregion

}
