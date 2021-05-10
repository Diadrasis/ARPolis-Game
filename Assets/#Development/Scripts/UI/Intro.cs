using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

	public GameObject omixli;
	public GameObject background;

	public Image[] eikones;
	public Text textDown;
	public RectTransform greditsPanelRectTransform;
	public Animator anim;

	int isFirstTime;

	void OnEnable(){

		isFirstTime = PlayerPrefs.GetInt("intro_isFirstTime");
		isFirstTime++;
		PlayerPrefs.SetInt("intro_isFirstTime", isFirstTime);
		PlayerPrefs.Save();

		#if UNITY_EDITOR
		Debug.LogWarning("intro first time = " + isFirstTime);
		#endif

		GetComponent<CanvasGroup>().alpha=1f;

		anim.Play("IntroBackground");

//		Stathis.Tools_UI.ResizeTextureToContainerSize(panelRT,false,false);

		if (Diadrasis.Instance.canvasMainRT == null) {
			Diadrasis.Instance.canvasMainRT = transform.parent.GetComponent<RectTransform> ();
		}

		//greditsPanelRectTransform.sizeDelta = Diadrasis.Instance.canvasMainRT.sizeDelta;

		//Vector2 sss = greditsPanelRectTransform.sizeDelta;
		
		//foreach(Image eikona in eikones){
		//	CanvasGroup gb = eikona.GetComponent<CanvasGroup>();
		//	gb.alpha=0f;
		//	Stathis.Tools_UI.RescaleImage(eikona);
		//}

		//sss.x+=10f;
		//sss.y+=10f;

		//greditsPanelRectTransform.sizeDelta = sss;

		StartCoroutine(fadeCredits());
	}

	[Range(1f,10f)]
	public float waitGredits = 2f;
	
	[Range(0.1f,15f)]
	public float speedFade=5f;

	IEnumerator fadeCredits(){
		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inGredits);
		
		if(eikones.Length==0){
			#if UNITY_EDITOR
			Debug.LogWarning("NO INTRO PHOTOS !!!");
			#endif
			//disable panel
			gameObject.SetActive(false);
			yield break;
		}

		yield return new WaitForSeconds(2f);
		
		//show all intro photos diadoxika
		for(int i=0; i<eikones.Length; i++)
		{
			//set 1st photo
			if(i==1){
				Stathis.Tools_UI.Move(eikones[i].GetComponent<RectTransform>(),Stathis.Tools_UI.Mode.down);
			}
			
			//fade in
			while(eikones[i].GetComponent<CanvasGroup>().alpha<0.99f)
			{
				eikones[i].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(eikones[i].GetComponent<CanvasGroup>().alpha,1f,Time.deltaTime * speedFade);	
				yield return null;
			}
			
			//fade in full
			eikones[i].GetComponent<CanvasGroup>().alpha=1f;
			
			//wait seconds
			if(i==0) yield return new WaitForSeconds(2f);
			if(i==1) yield return new WaitForSeconds(1f);



		}

		foreach(Image eikona in eikones){
			CanvasGroup cb = eikona.GetComponent<CanvasGroup>();
			if(cb){
				Destroy(cb); //.alpha = Mathf.Lerp(eikona.GetComponent<CanvasGroup>().alpha,0f,Time.deltaTime * speedFade);
			}
		}

		yield return new WaitForSeconds(waitGredits);

		//fade out
		while(GetComponent<CanvasGroup>().alpha>0.01f)
		{
			GetComponent<CanvasGroup>().alpha = Mathf.Lerp(GetComponent<CanvasGroup>().alpha,0f,Time.deltaTime * speedFade);
			yield return null;
		}

		//fade out full
		GetComponent<CanvasGroup>().alpha=0f;

		if(isFirstTime==1){

			Diadrasis.Instance.PluginsInit();

			//wait seconds
			yield return new WaitForSeconds(0.5f);

			/*
			#if UNITY_ANDROID
			if(PlayerPrefs.GetInt("useOnSiteMode")!=111){

				Gps.Instance.warningsInfo.popUpTextMessage.fontSize = appSettings.fontSize_keimeno;

				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inBetaWarning);

				Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement>().preferredWidth=650f;

				Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text("betaWarning");
				GameObject gb = Gps.Instance.warningsInfo.popUpMessage;
				
				AutoCloseMessage atc = gb.GetComponent<AutoCloseMessage>();
				atc.objToSendMessage = null;
				atc.onTapHide=false;
				
				if(atc){
					atc.xronos=30;
					omixli.SetActive(true);
					gb.SetActive(true);
					yield return new WaitForSeconds(10f);
					if(atc.closeBtn){
						atc.closeBtn.SetActive(true);
					}
					//on tap skip enumerator
					atc.objToSendMessage = this.gameObject;
					atc.onTapHide=true;
					yield return new WaitForSeconds(15f);
				}else{
					yield return new WaitForSeconds(30);
					gb.SetActive(false);
					//yield return new WaitForSeconds(1f);
				}

			}else
			if(PlayerPrefs.GetInt("useOnSiteMode")==111){

				Gps.Instance.warningsInfo.popUpTextMessage.fontSize = appSettings.fontSize_keimeno;

				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inBetaWarning);

				Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement>().preferredWidth=300f;

				Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text("unlockedProEdition");
				GameObject gb = Gps.Instance.warningsInfo.popUpMessage;
				
				
				AutoCloseMessage popUp = gb.GetComponent<AutoCloseMessage>();
				popUp.objToSendMessage = null;
				popUp.onTapHide=false;

				if(popUp){
					popUp.xronos=7;
					omixli.SetActive(true);
					gb.SetActive(true);
					yield return new WaitForSeconds(3.5f);
					popUp.closeBtn.SetActive(true);
					//on tap skip enumerator
					popUp.objToSendMessage = this.gameObject;
					popUp.onTapHide=true;
					yield return new WaitForSeconds(3.5f);
				}else{
					yield return new WaitForSeconds(7);
					gb.SetActive(false);
					//yield return new WaitForSeconds(1f);
				}
			}
			#endif
			*/
			
			if(SystemInfo.processorCount<2 || SystemInfo.systemMemorySize<1500){
				
				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inDeviceCheckWarning);

				//wait seconds
				yield return new WaitForSeconds(1f);
				
				Gps.Instance.warningsInfo.popUpTextMessage.fontSize = appSettings.fontSize_keimeno;

				Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement>().preferredWidth=650f;

				Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text("lowMemory");

				GameObject gbA = Gps.Instance.warningsInfo.popUpMessage;
				omixli.SetActive(true);
				gbA.SetActive(true);

				AutoCloseMessage atcA = gbA.GetComponent<AutoCloseMessage>();
				atcA.objToSendMessage = null;
				atcA.onTapHide = false;

				if(atcA){
					atcA.xronos=7;
					yield return new WaitForSeconds(7f);
				}else{
					yield return new WaitForSeconds(7f);
					gbA.SetActive (false);
					yield return new WaitForSeconds(1f);
				}

				gbA.SetActive (false);
			}

			omixli.SetActive(false);


		}else{
			Diadrasis.Instance.PluginsInit();

			//wait seconds
			yield return new WaitForSeconds(0.5f);
			/*
			#if UNITY_ANDROID

			if(PlayerPrefs.GetInt("useOnSiteMode")!=111){

				Gps.Instance.warningsInfo.popUpTextMessage.fontSize = appSettings.fontSize_keimeno;

				Diadrasis.Instance.ChangeStatus(Diadrasis.User.inBetaWarning);

				Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement>().preferredWidth=650f;

				Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text("betaWarning");
				GameObject gb = Gps.Instance.warningsInfo.popUpMessage;

				AutoCloseMessage atc = gb.GetComponent<AutoCloseMessage>();


				if(atc){
					atc.objToSendMessage = this.gameObject;
					atc.onTapHide=true;
					atc.xronos=30;
					omixli.SetActive(true);
					gb.SetActive(true);
					yield return new WaitForSeconds(1f);
					atc.closeBtn.SetActive(true);
				}


				yield return new WaitForSeconds(30f);
				omixli.SetActive(false);

			}

			#endif
			*/
		}

		if(Diadrasis.Instance.appEntrances==1){
			Diadrasis.Instance.isFirstHelpMenu = true;
		}


		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);
		Diadrasis.Instance.menuUI.MenuButtonsShow ();


		if(Diadrasis.Instance.appEntrances==1){

			yield return new WaitForSeconds(0.25f);
			Diadrasis.Instance.animControl.MenuButton ();
			yield return new WaitForSeconds(0.7f);
			Diadrasis.Instance.menuUI.HelpShow(true);
			yield return new WaitForSeconds (5f);
			Diadrasis.Instance.menuUI.helpScript.CloseHelps();
			Diadrasis.Instance.menuUI.animControl.CloseRadialMenu();
			yield return new WaitForSeconds(0.7f);
			Diadrasis.Instance.isFirstHelpMenu = false;
		}


		#if UNITY_EDITOR
		Debug.Log("GPS INIT");
		#endif

		Gps.Instance.Init();
		
		#if UNITY_EDITOR
		//are you in test mode?
		Gps.Instance.isLocationFound=true;
		#endif


//		if(Diadrasis.Instance.appEntrances==1){
//			yield return new WaitForSeconds(7f);
//			Diadrasis.Instance.menuUI.HelpShow();
//		}



		//disable panel
		gameObject.SetActive(false);

		yield break;
	}

	public void popupClose(){
		StopAllCoroutines ();
		StartCoroutine (doThat ());
	}

	IEnumerator doThat(){

		if (Diadrasis.Instance.appEntrances % 3 == 0) {//show message every 5 times
			
			if (SystemInfo.processorCount < 2 || SystemInfo.systemMemorySize < 1500) {

				Diadrasis.Instance.ChangeStatus (Diadrasis.User.inDeviceCheckWarning);

				//wait seconds
				yield return new WaitForSeconds (0.5f);

				Gps.Instance.warningsInfo.popUpTextMessage.fontSize = appSettings.fontSize_keimeno;

				Gps.Instance.warningsInfo.popUpTextMessage.GetComponent<LayoutElement> ().preferredWidth = 650f;

				Gps.Instance.warningsInfo.popUpTextMessage.text = appData.FindTerm_text ("lowMemory");

				GameObject gbA = Gps.Instance.warningsInfo.popUpMessage;
				omixli.SetActive (true);
				gbA.SetActive (true);

				AutoCloseMessage atcA = gbA.GetComponent<AutoCloseMessage> ();


				if (atcA) {
					atcA.objToSendMessage = null;
					//if (Diadrasis.Instance.appEntrances <= 1) {
						atcA.onTapHide = false;
					//} else {
					//	atcA.onTapHide = true;
					//}
					atcA.xronos = 5;
					yield return new WaitForSeconds (5f);
				} else {
					yield return new WaitForSeconds (5f);
				}

				gbA.SetActive (false);
			}
		}

		omixli.SetActive(false);

		if(Diadrasis.Instance.appEntrances==1){
			Diadrasis.Instance.isFirstHelpMenu = true;
		}

		yield return new WaitForSeconds(0.5f);
//		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inHelp);

		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);
		Diadrasis.Instance.menuUI.MenuButtonsShow ();

		if(Diadrasis.Instance.appEntrances==1){
			yield return new WaitForSeconds(0.5f);
			Diadrasis.Instance.animControl.MenuButton ();
			yield return new WaitForSeconds(0.7f);
			Diadrasis.Instance.menuUI.HelpShow();
			yield return new WaitForSeconds (5f);
			Diadrasis.Instance.menuUI.helpScript.CloseHelps();
			Diadrasis.Instance.isFirstHelpMenu = false;
		}

//		yield return new WaitForSeconds(0.5f);
//
//		Diadrasis.Instance.ChangeStatus(Diadrasis.User.inMenu);

		#if UNITY_EDITOR
		Debug.Log("GPS INIT");
		#endif

		Gps.Instance.Init();

		#if UNITY_EDITOR
		//are you in test mode?
		Gps.Instance.isLocationFound=true;
		#endif
		//disable panel
		gameObject.SetActive(false);

		yield break;
	}
}
