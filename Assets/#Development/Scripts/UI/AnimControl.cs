using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimControl : MonoBehaviour
{
	#region PUBLIC VARIABLES

	public Animator animMenu;
	public Animator animInfo;
	public Animator animMap;
	public Animator animSettings;

	private string status = "status";

	public InfoUI infoUI;

	public Button btnCloseShortDesc;
	public Button btnCloseFullDesc;
	public Button btnClosePhotos;
	public Button btnCloseSettings;

	public bool hasChangePerson;

	public delegate void ActionInfo();
	//full map close
	public event ActionInfo OnInfoClose;
	//full map show
	public event ActionInfo OnInfoShow;
	
	#endregion

	#region PRIVATE VARIABLES

	Xartis xartis;
	MenuUI menuUI;
	float xronos=-10f;

	#endregion

	#region START - UPDATE

	void Start () {
		animMenu=GetComponent<Animator>();
		menuUI = Diadrasis.Instance.menuUI;
		xartis = menuUI.xartis;

		btnCloseShortDesc.onClick.AddListener(()=>MenuButton());
		btnCloseFullDesc.onClick.AddListener(()=>MenuButton());
		btnClosePhotos.onClick.AddListener(()=>MenuButton());
		btnCloseSettings.onClick.AddListener(()=>MenuButton());
	}

	void Update () {
		if(xronos>0 && Diadrasis.Instance.escapeUser!=Diadrasis.EscapeUser.inHelp)
		{
			if(animMenu.GetInteger(status)==1)
			{
				xronos-=Time.deltaTime;
				if(xronos<=0)
				{
					menuInit();
					xronos=-10f;
				}
			}
			else//if another button pressed stop counter
			{
				xronos=-10f;
			}
		}
	}

	#endregion

	#region DELEGATES

	void OnEnable()
	{
		Diadrasis.Instance.OnSceneLoadStart += ResetMenu;
		Diadrasis.Instance.OnSceneLoadStart += StopAnimators;
		Diadrasis.Instance.OnSceneLoadEnd += PlayAnimators;
		Diadrasis.Instance.OnMapFullClose += ShowSmallMap;
	}
	
	
	void OnDisable()
	{
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.OnSceneLoadStart -= ResetMenu;
			Diadrasis.Instance.OnSceneLoadStart -= StopAnimators;
			Diadrasis.Instance.OnSceneLoadEnd -= PlayAnimators;
			Diadrasis.Instance.OnMapFullClose -= ShowSmallMap;
		}
	}

	#endregion

	#region BUTTON MENU CONDITIONS

	void ResetMenu(){
		animMenu.SetInteger("status",0);
	}

	public void MenuButton()
	{

		#if UNITY_EDITOR
		Debug.Log("MenuButton");
		#endif

		if(Diadrasis.Instance.user!=Diadrasis.User.inGredits && Diadrasis.Instance.user!=Diadrasis.User.inLoading)
		{
			if(animMenu.GetInteger(status)>0)
			{
				if (Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.activeSelf) {
					Diadrasis.Instance.menuUI.warningsUI.introScript.omixli.SetActive (false);
				}

				menuInit();

				if(!animInfo.enabled){animInfo.enabled=true;}

				if(animSettings.isActiveAndEnabled && animSettings.GetInteger(status)==1)
				{
					settingsHide();
					Diadrasis.Instance.CheckUserPrin();

					if (Diadrasis.Instance.user != Diadrasis.User.onAir  || Diadrasis.Instance.moveOnAir) {
						StopCoroutine ("showSmallMapIfPreviusWasEnabled");
						StartCoroutine ("showSmallMapIfPreviusWasEnabled");
					}
				}

				if(animInfo.isActiveAndEnabled && animInfo.GetInteger(status)==1)
				{
					infoUI.blurBack.GetComponent<CanvasGroup>().alpha=0f;
					infoUI.blurBack.SetActive(false);
					infoClose();

					infoUI.ResetPhotoPanel();
					Diadrasis.Instance.CheckUserPrin();

					if (Diadrasis.Instance.user != Diadrasis.User.onAir || Diadrasis.Instance.moveOnAir) {
						StopCoroutine ("showSmallMapIfPreviusWasEnabled");
						StartCoroutine ("showSmallMapIfPreviusWasEnabled");
					}
				}
				else
				if(animInfo.isActiveAndEnabled && (animInfo.GetInteger(status)==2 || animInfo.GetInteger(status)==3))
				{
					buttonMenuMoveToShort();
					shortShow();
				}

				//hide sxolia
				menuUI.settingsUI.panelContainerSxoliou.SetActive(false);

			}
			else
			{
				if(Diadrasis.Instance.user!=Diadrasis.User.inQuit)
				{
					menuOpen();
					xronos=5.5f;
				}
			}
		}
	}

	#endregion

	#region SETTINGS
	public void SettingsButton()
	{
		if(animSettings.GetInteger(status)==0)
		{
			buttonMenuMoveToShort();
			settingsShow();
			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inSettings);
		}
	}
	#endregion

	#region SHORT & FULL DESC

	public void ShortDescShow()
	{
		if(animInfo.GetInteger(status)==0)
		{
			buttonMenuMoveToShort();

			infoUI.blurBack.GetComponent<CanvasGroup>().alpha=0f;
			infoUI.blurBack.SetActive(true);

			StopCoroutine("alphaLerp");
			StartCoroutine("alphaLerp");

			if(Diadrasis.Instance.user!=Diadrasis.User.onAir){
				xartis.SetMapPreviusStatus ();
			}
	
			shortShow();

			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inPoi);
		}
	}

	IEnumerator alphaLerp()
	{
		while(infoUI.blurBack.GetComponent<CanvasGroup>().alpha<0.75f)
		{
			infoUI.blurBack.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(infoUI.blurBack.GetComponent<CanvasGroup>().alpha,1f,Time.deltaTime);
			yield return null;
		}
		
		infoUI.blurBack.GetComponent<CanvasGroup>().alpha=0.75f;
		yield return null;
	}


	public void FullDescShow()
	{
		if(animInfo.GetInteger(status)==1)
		{
			buttonMenuMoveToTop();
			infoUI.ResetFullDescTextPos();
			fullShow();
			StopCoroutine("delayCloseAnimator");
			StartCoroutine("delayCloseAnimator");
		}
	}


	IEnumerator delayCloseAnimator()
	{
		yield return new WaitForSeconds(1f);
		animInfo.enabled=false;
	}

	#endregion

	#region PHOTOS

	public void PhotosShow()
	{
		buttonMenuMoveToTop();
	}

	#endregion

	#region MENU ANIMATOR 

	//########################################   MENU BUTTON  ##############################################

	public enum ButtonMenuStatus {CLOSE, OPEN}
	public ButtonMenuStatus btnMenuStatus = ButtonMenuStatus.CLOSE;

	public void CloseRadialMenu(){
		#if UNITY_EDITOR
		Debug.Log("CloseRadialMenu");
		#endif
		if (btnMenuStatus == ButtonMenuStatus.OPEN) {
			menuInit ();
		}
	}

	private void menuInit()
	{
		#if UNITY_EDITOR
		Debug.Log("menuInit");
		#endif

		animMenu.SetInteger(status,0);
		btnMenuStatus = ButtonMenuStatus.CLOSE;
	}

	private void menuOpen()
	{
		animMenu.SetInteger(status,1);
		btnMenuStatus = ButtonMenuStatus.OPEN;
	}

	private void buttonMenuMoveToMap()
	{
		animMenu.SetInteger(status,2);
	}

	private void buttonMenuMoveToShort()
	{
		animMenu.SetInteger(status,3);
	}

	private void buttonMenuMoveToTop()
	{
		animMenu.SetInteger(status,4);
	}

	#endregion

	#region MAP ANIMATOR
	//########################################   MAP  ##############################################
	

	public void CloseMap(){
		mapMikrosClose ();
	}

	public void ShowFullMap()
	{
		mapFullShow();
	}
	
	public void ShowSmallMap()
	{		
		mapMikrosShow();
	}

	public void MapMoveUp()
	{							
		if(animMap.GetInteger(status)==1)
		{
			menuInit();
			mapMikrosClose();
		}
		else
			if(animMap.GetInteger(status)==0)
		{
			mapMikrosShow();
		}
	}

	IEnumerator showSmallMapIfPreviusWasEnabled(){
		yield return new WaitForSeconds(0.7f);
//		if(hasChangePerson){
//			ShowFullMap();
//			hasChangePerson=false;
//		}else{
			xartis.CheckMapPreviusStatus ();

//		yield return new WaitForSeconds(0.7f);
//		animMap.enabled=true;
		
//		}
		yield break;
	}

	private void mapMikrosShow()
	{
		#if UNITY_EDITOR
		Debug.Log("mapMikrosShow");
		#endif


		if(Diadrasis.Instance.user==Diadrasis.User.inMenu || Diadrasis.Instance.user==Diadrasis.User.inLoading){
			return;
		}

		if(!animMap.enabled){animMap.enabled=true;}

		if (animMap.GetInteger (status) != 1) {

			animMap.SetInteger (status, 1);

		}

		xartis.ChangeStatus(Xartis.MapStatus.Mikros);

	}

	private void mapMikrosClose()
	{
		#if UNITY_EDITOR
		Debug.Log("mapMikrosClose");
		#endif

		if(!animMap.enabled){animMap.enabled=true;}

		if (animMap.GetInteger (status) != 0) {
			animMap.SetInteger (status, 0);//0
		}

		xartis.ChangeStatus (Xartis.MapStatus.Close);
	}

	private void mapFullShow()
	{
		#if UNITY_EDITOR
		Debug.Log("mapFullShow");
		#endif

//		if(!animMap.enabled){animMap.enabled=true;}
//
//		if (animMap.GetInteger (status) != 2) {
//			animMap.SetInteger (status, 2);
//		}

		xartis.ChangeStatus (Xartis.MapStatus.Full);
	}

	private void mapMoveUp()
	{
		#if UNITY_EDITOR
		Debug.Log("mapMoveUp");
		#endif

		if(!animMap.enabled){animMap.enabled=true;}

		if (animMap.GetInteger (status) != 3) {
			animMap.SetInteger (status, 3);
		}
	}

	#endregion

	#region SETTINGS ANIMATOR
	//########################################   SETTINGS  ##############################################
	

	private void settingsShow()
	{
		#if UNITY_EDITOR
		Debug.Log("settingsShow");
		#endif

		menuUI.warningsUI.HideGpsWarnings();

		if(Diadrasis.Instance.user!=Diadrasis.User.onAir){
			xartis.SetMapPreviusStatus ();
		}

		mapMikrosClose();

		animSettings.SetInteger(status,1);
	}

	private void settingsHide()
	{
		#if UNITY_EDITOR
		Debug.Log("settingsHide");
		#endif
		Diadrasis.Instance.menuUI.settingsUI.countToShow=0;
		
		animSettings.SetInteger(status,0);
	}

	#endregion

	#region INFO ANIMATOR
	//########################################   INFO  ##############################################


	private void infoClose()
	{
		#if UNITY_EDITOR
		Debug.Log("infoClose");
		#endif

		animInfo.SetInteger(status,0);

		StopCoroutine("ttt");
		StartCoroutine(ttt());
	}

	IEnumerator ttt(){
		yield return new WaitForSeconds(0.7f);
		if(OnInfoClose != null){
			OnInfoClose();
		}
		yield break;
	}

	private void shortShow()
	{
		#if UNITY_EDITOR
		Debug.Log("shortShow");
		#endif

		mapMikrosClose();

		animInfo.SetInteger(status,1);

		if(OnInfoShow != null){
			OnInfoShow();
		}
	}

	private void fullShow()
	{
		#if UNITY_EDITOR
		Debug.Log("fullShow");
		#endif

		animInfo.SetInteger(status,2);
	}

	private void photoShow()
	{
		#if UNITY_EDITOR
		Debug.Log("photoShow");
		#endif

		animInfo.SetInteger(status,3);
	}

	#endregion

	#region ANIMATORS CONTROLS

	private void StopAllAnimatorsWithDelay()
	{
		#if UNITY_EDITOR
		Debug.Log("StopAllAnimatorsWithDelay");
		#endif

		StopCoroutine("delayStopAnimators");
		StartCoroutine("delayStopAnimators");
	}
	
	private IEnumerator delayStopAnimators()
	{
		yield return new WaitForSeconds(1f);
		StopAnimators();
	}

	public void StopAnimators()
	{
		#if UNITY_EDITOR
		Debug.Log("StopAnimators");
		#endif

		animInfo.enabled=false;
		animMenu.enabled=false;
		animMap.enabled=false;
		animSettings.enabled=false;
	}

	public void PlayAnimators()
	{
		#if UNITY_EDITOR
		Debug.Log("PlayAnimators");
		#endif

		animInfo.enabled=true;
		animMenu.enabled=true;
		animMap.enabled=true;
		animSettings.enabled=true;
	}

	#endregion
}
