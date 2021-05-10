using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Stathis;

public class WarningEventsUI : MonoBehaviour {

	public Intro introScript;

	//######## SHOP PRO EDITION ###########
	public GameObject popUpBuyFirstTime, popUpBuyConfirmation, popUpCoupon, txtWrongCoupon, txtCorrectCoupon, txtSameCoupon, btnsCouponAsk, btnsCouponApply;
	public Button btnBuyCancel1, btnBuyCancel2, btnBuyFirstTime, btnBuyMenu, btnBuyPurchase, btnBuyCouponYes, btnBuyCouponCancel, btnOwnCouponYes, btnOwnCouponNo, btnRestorePurchases;
	public AutoSetLanguange txtBtnBuy;
	public InputField inputCouponField;
	public RectTransform couponContainer;



	//#####################################

	public GameObject popUpSxolioBtn;
	public Button btnSxolio;

	public GameObject popUpMessage;
	public Text popUpTextMessage;

	//####### GPS Status ##########
	public GameObject menuGpsOff;
	public GameObject menuGpsOnFar;
	public GameObject menuGpsOnNear;
	public GameObject menuGpsOnInside;
	public GameObject menuGpsOFF_FirstMessage;

	public enum GpsStatus{IDLE, OFF, ON_FAR, ON_NEAR, ON_INSIDE}
	public GpsStatus gpsStatus = GpsStatus.IDLE;

	//####### Menu Status ##########
	public GameObject menuMainPanel;
	public GameObject menuStatus;
	//0=xartis 1=zoomIdle 2=zoomIn 3=ZoomOut 4=periodView 5=mapMove 
	public Sprite[] spritesMenuStatus;
	Image imgMenuStatus;
	//##############################

	//######## QUIT Panel###########
	public GameObject quitMainPanel;
	public Button quitBtn_Yes;
	public Button quitBtn_No;
	//##############################

	//######## nav Mode Panel###########
	public GameObject navModeMainPanel;
	public GameObject gpsDemoVersionMessage;
	public Button btnDemo_OK;
	public Button navMode_Yes;
	public Button navMode_No;
	//##############################

	//######## nav Camera Mode Panel###########
	public GameObject navCameraModeMainPanel;
	public Button navCameraMode_Yes;
	public Button navCameraMode_No;
	//##############################

	//######## Quit Scene Panel###########
	public GameObject quitScenePanel;
	public Button quitScene_Yes;
	public Button quitScene_No;
	//##############################

	public GameObject gpsInitialingMSG;
	public GameObject gpsTimeOurMSG;
	public GameObject gpsFaultAccuracy;
	public GameObject gpsWaitForCheckingAccuracy;

	public GameObject endOfArea;
	public static bool isEndOfMove;

	List<GameObject> allMainPanels = new List<GameObject>();

	void Start () {
		allMainPanels.Add(menuMainPanel);
		allMainPanels.Add(menuStatus);
		allMainPanels.Add(quitMainPanel);
		allMainPanels.Add(navModeMainPanel);
		navCameraModeMainPanel.SetActive(false);
//		allMainPanels.Add(navCameraModeMainPanel);
//		allMainPanels.Add(gpsErrorPanel);
		allMainPanels.Add(endOfArea);


		imgMenuStatus=menuStatus.GetComponent<Image>();

		quitBtn_No.onClick.AddListener(()=>CancelQuit());
		quitBtn_Yes.onClick.AddListener(()=>QuitApp());
		navMode_Yes.onClick.AddListener(()=>NavMode (true));
		navMode_No.onClick.AddListener(()=>NavMode (false));
		btnDemo_OK.onClick.AddListener(()=>NavMode (false));

//		navCameraMode_Yes.onClick.AddListener(()=>navCamMode (true));
//		navCameraMode_No.onClick.AddListener(()=>navCamMode (false));

		quitScene_No.onClick.AddListener(()=>CancelQuitScene());
		quitScene_Yes.onClick.AddListener(()=>ReturnToMenu());

		btnSxolio.onClick.AddListener(()=>OpenSxolioPanel());

		btnBuyCancel1.onClick.AddListener(()=>BuyCancel(true));
		btnBuyCancel2.onClick.AddListener(()=>BuyCancel(true));
		btnBuyCouponCancel.onClick.AddListener(()=>BuyCancel(true));

		btnBuyMenu.onClick.AddListener(()=>ShowBuyFirstMessage());
		btnBuyFirstTime.onClick.AddListener(()=>ShowBuyCoupon());
		btnBuyCouponYes.onClick.AddListener(()=>BuyPurchase()); //ShowBuyConfirmation());
		btnBuyPurchase.onClick.AddListener(()=>BuyPurchase());

		inputCouponField.onValueChange.AddListener((b)=>HideCodeInfoTexts());
		inputCouponField.onEndEdit.AddListener((b)=>CheckCoupon(inputCouponField.text));

		btnOwnCouponNo.onClick.AddListener(()=>BuyPurchase()); //ShowBuyConfirmation());
		btnOwnCouponYes.onClick.AddListener(()=>ShowIputField());

		btnRestorePurchases.onClick.AddListener(()=>Shop.Instance._shopManager.Restore());

	}

	void OnEnable()
	{
		Diadrasis.Instance.OnSceneLoadStart += HideCommentButton;
		Diadrasis.Instance.OnSceneLoadStart += HideShopButton;
		Diadrasis.Instance.OnSceneLoadStart += HideGpsWarnings;
	}
	
	
	void OnDisable()
	{
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.OnSceneLoadStart -= HideCommentButton;
			Diadrasis.Instance.OnSceneLoadStart -= HideShopButton;
			Diadrasis.Instance.OnSceneLoadStart -= HideGpsWarnings;
		}
	}

	#region BUY OPTIONS

	void ShowIputField(){

		Tools_UI.Move(couponContainer, Tools_UI.Mode.up);
		btnsCouponAsk.SetActive(false);
		btnsCouponApply.SetActive(true);
		inputCouponField.gameObject.SetActive(true);
		//open keyboard with opaque and secure string (asterics)
//		TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true, true);
	}

	void InitInputCoupon(){
		Shop.Instance.isCouponValid=false;
		inputCouponField.text=string.Empty;
		HideCodeInfoTexts();
	}

	void HideCodeInfoTexts(){
		txtWrongCoupon.SetActive(false);
		txtCorrectCoupon.SetActive(false);
		txtSameCoupon.SetActive(false);

		if (!string.IsNullOrEmpty (inputCouponField.text)) {
			inputCouponField.text = inputCouponField.text.ToUpper ();
		}
	}

	public void ShowBuyCoupon(){
		BuyCancel(false);
		InitInputCoupon();

		//Debug.LogWarning(Application.loadedLevelName);

		//if not in menu return to menu
		if(Application.loadedLevelName!="main"){
			Shop.Instance.isFromSceneAction=true;
			BuyCancel(true);
			Diadrasis.Instance.ReturnToMainMenu();
			return;
		}

		Tools_UI.Move(couponContainer, Tools_UI.Mode.center);

		popUpCoupon.SetActive(true);
	}

	void CheckCoupon(string val){

		if(string.IsNullOrEmpty(val)){
			InitInputCoupon();
			return;
		}

		//inputCouponField.text = val.ToUpper ();

		bool isCorrect = appData.isCouponCorrect(val.ToUpper());

		//coupon is correct
		if(isCorrect){

			PlayerPrefs.SetString("myCoupon", val);
			PlayerPrefs.Save();

			if(txtWrongCoupon.activeSelf){
				txtWrongCoupon.SetActive(false);
			}
			if(txtSameCoupon.activeSelf){
				txtSameCoupon.SetActive(false);
			}
			if(!txtCorrectCoupon.activeSelf){
				txtCorrectCoupon.SetActive(true);
			}
			if(!Shop.Instance.isCouponValid){
				Shop.Instance.isCouponValid=true;
			}
		}else{//coupon is wrong
			if(txtCorrectCoupon.activeSelf){
				txtCorrectCoupon.SetActive(false);
			}
			if(txtSameCoupon.activeSelf){
				txtSameCoupon.SetActive(false);
			}
			if(!txtWrongCoupon.activeSelf){
				txtWrongCoupon.SetActive(true);
			}
			if(Shop.Instance.isCouponValid){
				Shop.Instance.isCouponValid=false;
			}
		}
	}

	public void ShowBuyFirstMessage(){
		//if(Diadrasis.Instance.appEntrances==1 || Diadrasis.Instance.isStart>2){return;}

//		Shop.Instance._shopManager.Restore();
		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inShop);
		inputCouponField.gameObject.SetActive(false);
		btnsCouponAsk.SetActive(true);
		btnsCouponApply.SetActive(false);
		popUpBuyFirstTime.SetActive(true);

		CloseOmixli ();
		HideGpsWarnings ();
	}

	void ShowBuyConfirmation(){
		if(Diadrasis.Instance.appEntrances==1 && Diadrasis.Instance.isStart==1  && Diadrasis.Instance.user==Diadrasis.User.inMenu){return;}
		
		BuyCancel(false);
		Shop.Instance._shopManager.Refresh();

		popUpBuyConfirmation.SetActive(true);

		
		if(Shop.Instance.isCouponValid){
			txtBtnBuy.stringToAdd = " "+Shop.Instance._shopManager.priceOfProAffiliateEdition;
			#if UNITY_EDITOR
			Debug.LogWarning(Shop.Instance._shopManager.priceOfProAffiliateEdition);
			#endif
		}else{
			txtBtnBuy.stringToAdd = " "+Shop.Instance._shopManager.priceOfProEdition;
			#if UNITY_EDITOR
			Debug.LogWarning(Shop.Instance._shopManager.priceOfProEdition);
			#endif
		}

		txtBtnBuy.Init();
		
	}

	public void BuyCancel(bool val){
//		Shop.Instance.isCouponValid=false;
		popUpBuyFirstTime.SetActive(false);
		popUpBuyConfirmation.SetActive(false);
		popUpCoupon.SetActive(false);
		if(val){
			Diadrasis.Instance.CheckUserPrin();
		}
		CloseOmixli ();
	}

	void CloseOmixli(){
		#if UNITY_EDITOR
		Debug.Log("CLOSE OMIXLI");
		#endif
		introScript.omixli.SetActive (false);
	}

	void BuyPurchase(){
		string productName = string.Empty;

		if(Shop.Instance.isCouponValid){
			if (!string.IsNullOrEmpty (Shop.Instance.customItemBuy)) {
				productName = Shop.Instance.customItemBuy;
			} else {
				productName = Shop.Instance._shopManager.athensTimeWalkPro_Affiliate;
			}
		}else{
			productName = Shop.Instance._shopManager.athensTimeWalkPro;
		}

		Shop.Instance._shopManager.Purchase(productName);

		#if UNITY_EDITOR
		Debug.LogWarning("Purchase "+productName);
		#endif

		introScript.omixli.SetActive (false);

		BuyCancel(true);
	}

	public void CheckPurchases(){

		#if UNITY_EDITOR
		Debug.LogWarning("CheckPurchases");
		#endif

		BuyCancel(false);

		if(PlayerPrefs.GetInt("useOnSiteMode")==111){
			#if UNITY_EDITOR
			Debug.LogWarning("Pro is on");
			#endif
			btnBuyMenu.gameObject.SetActive(false);
			Diadrasis.Instance.useOnSiteMode =true;
			Diadrasis.Instance.showPoiInfo =true;
		}else{
			#if UNITY_EDITOR
			Debug.LogWarning("Pro is off");
			#endif
			Diadrasis.Instance.useOnSiteMode =false;
			btnBuyMenu.gameObject.SetActive(true);
		}

	}

	public void HideShopButton(){
		if(btnBuyMenu.gameObject.activeSelf){
			btnBuyMenu.gameObject.SetActive(false);
		}
	}

	#endregion

	public void ShowCommentButton(){
		if(!popUpSxolioBtn.activeSelf){
			popUpSxolioBtn.SetActive(true);
		}
//		#if UNITY_EDITOR
//		Debug.Log("ShowCommentButton");
//		#endif
	}

	public void HideCommentButton(){
		if(popUpSxolioBtn.activeSelf){
			popUpSxolioBtn.SetActive(false);
		}
//		#if UNITY_EDITOR
//		Debug.Log("HideCommentButton");
//		#endif
	}

	void OpenSxolioPanel(){
		Diadrasis.Instance.animControl.MenuButton();
		Diadrasis.Instance.animControl.SettingsButton();
		Diadrasis.Instance.menuUI.settingsUI.InitSxolioPanel();
		introScript.omixli.SetActive (true);
		//int p = introScript.omixli.transform.GetSiblingIndex (); Debug.Log ("omixli indx = " + p);

		HideGpsWarnings ();
	}

	public void GpsErrorOn()
	{
		HideGpsWarnings();
		if(!gpsInitialingMSG.activeSelf && !Diadrasis.Instance.menuUI.helpPanel.activeSelf)
		{
			gpsInitialingMSG.SetActive(true);
		}
	}

	public void GpsErrorOff()
	{
		if(gpsInitialingMSG.activeSelf)
		{
			gpsInitialingMSG.SetActive(false);
		}
	}

	void CancelQuitScene()
	{
		quitScenePanel.SetActive(false);
	}

	void ReturnToMenu()
	{
		quitScenePanel.SetActive(false);
		Diadrasis.Instance.ReturnToMainMenu();
	}

	void CancelQuit()
	{
		//change status to inMenu
		Diadrasis.Instance.CheckUserPrin();
	}

	void QuitApp()
	{
		#if UNITY_EDITOR
		Debug.Log("CLEAR SAVE DATA ON QUIT ????");
//		PlayerPrefs.DeleteAll();
		#endif
		PlayerPrefs.Save();
		TitloiTelous.mustQuit=true;
		Diadrasis.Instance.menuUI.QuitTimeWalk ();
	}

	void NavMode(bool onSite)
	{
		#if UNITY_EDITOR
		Debug.Log ("NavMode");
		#endif
		 
		navModeMainPanel.SetActive(false);
		gpsDemoVersionMessage.SetActive(false);

		if(onSite)
		{
			Diadrasis.Instance.navMode=Diadrasis.NavMode.onSite;
			//load scene with models
			#if UNITY_EDITOR
			Debug.Log("loadPeriodScene 1");
			#endif
			loadPeriodScene();
		}
		else
		{
			Diadrasis.Instance.navMode=Diadrasis.NavMode.offSite;

			Debug.Log("off site load Scene");

			//load scene with models
			#if UNITY_EDITOR
			Debug.Log("loadPeriodScene 2");
			#endif
			loadPeriodScene();
		}
	}


//	void navCamMode(bool withCam)
//	{
//		if(withCam){
//			Diadrasis.Instance.isCamNavigation=true;
//		}else{
//			Diadrasis.Instance.isCamNavigation=false;
//		}
//
//		#if UNITY_EDITOR
//		Debug.Log("loadPeriodScene 3");
//		#endif
//		loadPeriodScene();
//	}

	public bool showSelectModeWarning;
	
	public void loadPeriodScene()
	{		
		#if UNITY_EDITOR
		Debug.Log("loadPeriodScene");
		#endif

		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inLoading);
		
		Diadrasis.Instance.LoadScene();

		HideGpsWarnings();

		//reset menu map gps status so in next menu entrance can check again
		gpsStatus=GpsStatus.IDLE;
	}


	private void LateUpdate()
	{
		CheckStatus();
	}

	void HideAllMainPanels()
	{
		allMainPanels.ForEach(delegate(GameObject obj){
			if(obj.activeSelf){obj.SetActive(false);}
		});
		
	}

	void DisplayIcon_Map()
	{
		if(!menuStatus.activeSelf){menuStatus.SetActive(true);}

		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[0])
			imgMenuStatus.overrideSprite=spritesMenuStatus[0];
	}

	void DisplayIcon_MapMove()
	{
		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[5])
			imgMenuStatus.overrideSprite=spritesMenuStatus[5];
	}

	void DisplayIcon_Zoom()
	{
		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[1])
		{
			imgMenuStatus.overrideSprite=spritesMenuStatus[1];
		}
	}

	void DisplayIcon_ZoomIn()
	{
		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[2])
		{
			imgMenuStatus.overrideSprite=spritesMenuStatus[2];
		}
	}

	void DisplayIcon_ZoomOut()
	{
		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[3])
		{
			imgMenuStatus.overrideSprite=spritesMenuStatus[3];
		}
	}

	void DisplayIcon_PeriodView()
	{
		if(imgMenuStatus.overrideSprite!=spritesMenuStatus[4])
			imgMenuStatus.overrideSprite=spritesMenuStatus[4];
	}

	void Show_menuMainPanel()
	{
		if(!menuMainPanel.activeSelf)
		{
			menuMainPanel.SetActive(true);
		}
	}

	void Show_menuStatus()
	{
		if(!menuStatus.activeSelf)
		{
			menuStatus.SetActive(true);
		}
	}


	public void SetGpsStatus(GpsStatus status, bool checkAgain){

		if((gpsStatus==status && !checkAgain) || Diadrasis.Instance.user!=Diadrasis.User.inMenu || Diadrasis.Instance.isFirstHelpMenu == true ){
			return;
		}

		if(Diadrasis.Instance.isStart==2 && Diadrasis.Instance.appEntrances==1){
			return;
		}

		gpsStatus = status;

		HideGpsWarnings();

		if(gpsStatus==GpsStatus.OFF){
			if(Diadrasis.Instance.isStart==1){
				if(!menuGpsOFF_FirstMessage.activeSelf){
					menuGpsOFF_FirstMessage.SetActive(true);
				}
			}else{
				if(!menuGpsOff.activeSelf){
					menuGpsOff.SetActive(true);
				}
			}
		}else
		if(gpsStatus==GpsStatus.ON_FAR){
			if(!menuGpsOnFar.activeSelf){
				menuGpsOnFar.SetActive(true);
			}
		}else
		if(gpsStatus==GpsStatus.ON_NEAR){
			if(!menuGpsOnNear.activeSelf){
				menuGpsOnNear.SetActive(true);
			}
		}else
		if(gpsStatus==GpsStatus.ON_INSIDE){
			if(!menuGpsOnInside.activeSelf){
				menuGpsOnInside.SetActive(true);
			}
		}
	}

	public void HideGpsWarnings(){
//		gpsStatus=GpsStatus.IDLE;
		gpsWaitForCheckingAccuracy.SetActive (false);
		gpsFaultAccuracy.SetActive (false);
		gpsTimeOurMSG.SetActive(false);
		gpsInitialingMSG.SetActive(false);
		menuGpsOnFar.SetActive(false);
		menuGpsOnNear.SetActive(false);
		menuGpsOnInside.SetActive(false);
		menuGpsOff.SetActive(false);
		menuGpsOFF_FirstMessage.SetActive(false);
	}

	void CheckStatus()
	{
		HideAllMainPanels();

		switch(Diadrasis.Instance.user)
		{
			case(Diadrasis.User.inMenu):

				Show_menuMainPanel();
//				Show_menuStatus();

				switch(Diadrasis.Instance.menuStatus)
				{
					case(Diadrasis.MenuStatus.idle)://show warning message "close application?"
//						DisplayIcon_Map();
						//if select mode was open close it
						showSelectModeWarning=false;
						navModeMainPanel.SetActive(false);
						gpsDemoVersionMessage.SetActive(false);
						navCameraModeMainPanel.SetActive(false);
						break;
					case(Diadrasis.MenuStatus.mapMove):
						DisplayIcon_MapMove();
						break;
					case(Diadrasis.MenuStatus.mapZoom):
						switch(Diadrasis.Instance.zoomStatus)
						{
							case(Diadrasis.ZoomStatus.idle):
								DisplayIcon_Zoom();
								break;
							case(Diadrasis.ZoomStatus.zoomIN):
								DisplayIcon_ZoomIn();
								break;
							case(Diadrasis.ZoomStatus.zoomOut):
								DisplayIcon_ZoomOut();
								break;
						}
						break;
					case(Diadrasis.MenuStatus.periodView):
						DisplayIcon_PeriodView();
						if(showSelectModeWarning){
							if(Diadrasis.Instance.useOnSiteMode || Diadrasis.Instance.allowSceneGps==Diadrasis.Instance.sceneName){
								navModeMainPanel.SetActive(true);
							}else{
								gpsDemoVersionMessage.SetActive(true);
							}
				
						}
						break;
//					case(Diadrasis.MenuStatus.periodViewAnimating):
//						DisplayIcon_PeriodView();
////						if(showSelectModeWarning){
//							navModeMainPanel.SetActive(false);
//							navCameraModeMainPanel.SetActive(true);
////						}
//						break;
				}
				break;
			case(Diadrasis.User.isNavigating):
				if(isEndOfMove){
					if(!endOfArea.activeSelf){
						endOfArea.SetActive(true);
					}
				}

				break;
			case(Diadrasis.User.onAir):
				if(Diadrasis.Instance.moveOnAir){
					if(isEndOfMove){
						if(!endOfArea.activeSelf){
							endOfArea.SetActive(true);
						}
					}
				}
				break;
			case(Diadrasis.User.inHelp):
				//hide help
//				Diadrasis.Instance.EscapeCheck();
				break;
			case(Diadrasis.User.inPause):
				break;
			case(Diadrasis.User.inSettings):
				break;
			case(Diadrasis.User.inPoi):
				break;
			case(Diadrasis.User.isIdle):
				break;
			case(Diadrasis.User.inLoading):
				break;
			case(Diadrasis.User.inFullMap):
				break;
			case(Diadrasis.User.inQuit):
				if(!quitMainPanel.activeSelf){quitMainPanel.SetActive(true);}
				break;
			default:
				break;
		}
	}

}
