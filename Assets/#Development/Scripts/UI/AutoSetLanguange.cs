using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoSetLanguange : MonoBehaviour {

	public bool setFontSizeManually=false;
	public bool autoReplaceText = true;

	private Text myText;
	private string myLang;

	RawImage myRawImage;
	public Texture grSprite;
	public Texture enSprite;

	public bool useNearSceneName;
	public bool useCloseSceneName;
	public bool autoResizeTextFont = true;
//	int tabletFontSize = 17;
//	int mobileFontSize = 21;
//	int smallMobile = 24;

	public string stringToAdd=string.Empty;

	LayoutElement myLayOut;
	
	void OnEnable()
	{
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			if (Diadrasis.Instance.menuUI != null) {
				Diadrasis.Instance.menuUI.settingsUI.OnLanguangeChange += ChangeLanguange;
			}
		}

		if(!myLayOut){
			myLayOut = GetComponent<LayoutElement>();
		}

		if(myLayOut){
			ContentSizeFitter cs = GetComponent<ContentSizeFitter>();
			if(cs){
				myLayOut.preferredWidth = Diadrasis.Instance.canvasMainRT.sizeDelta.x / 1.5f;
			}
		}

		if(myText==null){myText=GetComponent<Text>();}

//		#if UNITY_EDITOR
//		if(myText){
//			Debug.Log("term "+myText.transform.name+" = "+appData.FindTerm_text(myText.transform.name));
//		}
//		#endif

		if (setFontSizeManually) {
			if(autoResizeTextFont)
			{
				myText.fontSize = appSettings.fontSize_keimeno;
			}
		}

		if(myRawImage==null && grSprite && enSprite){
			myRawImage = GetComponent<RawImage>();
		}

		myLang = appSettings.language;
			
		if(autoReplaceText){
			ReplaceText();
		}
	}


	void OnDisable()
	{
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.menuUI.settingsUI.OnLanguangeChange -= ChangeLanguange;
		}
	}

	void ChangeLanguange(){
		if (myLang == appSettings.language) {
			return;
		}

		myLang=appSettings.language;

		if (myText && autoReplaceText) {
				
			ReplaceText ();

			if (useNearSceneName) {
				if (myText.text != Diadrasis.Instance.nearSceneAreaName) {
					ReplaceText ();
				}
			}

			if (useCloseSceneName) {
				if (myText.text != Diadrasis.Instance.nearSceneAreaName) {
					ReplaceText ();
				}
			}
		}
	}

//	void LateUpdate(){
//		if (myText && autoReplaceText) {
//			if (myLang != appSettings.language) {
//				ReplaceText ();
//			}
//
//			if (useNearSceneName) {
//				if (myText.text != Diadrasis.Instance.nearSceneAreaName) {
//					ReplaceText ();
//				}
//			}
//
//			if (useCloseSceneName) {
//				if (myText.text != Diadrasis.Instance.nearSceneAreaName) {
//					ReplaceText ();
//				}
//			}
//		}
//	}

	public void Init(){
		if (myText) {
			myText.text = string.Empty;
			ReplaceText ();
		}
	}
	
	void ReplaceText(){
		//myLang=appSettings.language;

		//find text from terms xml with the name of transform if exists
		if(myText)
		{
			if(autoResizeTextFont)
			{
				myText.fontSize = appSettings.fontSize_keimeno;
			}

			if(appData.FindTerm_text(myText.transform.name)!="unknown"){
				if(!useNearSceneName){
					if(useCloseSceneName){
						string skini = string.Empty;

						if(myLang=="gr"){
							skini = "\n\n(i) κοντινότερη περιοχή >> "+Diadrasis.Instance.nearSceneAreaName;
						}else{
							skini = "\n\n(i) closest area >> "+Diadrasis.Instance.nearSceneAreaName;
						}

						myText.text = appData.FindTerm_text(myText.transform.name) + skini;
					}else{
						myText.text = appData.FindTerm_text(myText.transform.name);
					}
				}else{
					myText.text = Diadrasis.Instance.nearSceneAreaName + " \n\n " + appData.FindTerm_text(myText.transform.name);
				}
			}

			if(!string.IsNullOrEmpty(stringToAdd)){
				myText.text += stringToAdd;
			}
		}

		if(myRawImage && grSprite && enSprite){
			if(myLang == "gr"){
				myRawImage.texture = grSprite;
			}else{
				myRawImage.texture = enSprite;
			}
		}
	}
}
