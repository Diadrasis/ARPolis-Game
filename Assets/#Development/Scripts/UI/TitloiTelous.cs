using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Stathis;
//for xml
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

public class TitloiTelous : MonoBehaviour {

	public RectTransform maskaLetters;
	public RectTransform platforma;

	public RawImage imageUp,imageDown;

	CanvasGroup platformaMaska;

	public RawImage blackPanelImage; 
	RectTransform blackRect;
	RectTransform parentCanvas;
	Vector2 parentSize;

	Rect rekt;
	Vector2 blackSize;
//	float allPersons;
	CanvasGroup cgPanelFather;

	public Button btnClose;

	void ClosePanel(){
		btnClose.gameObject.SetActive(false);
		StartCoroutine (closeGreditsWithFade ());
	}

	void OnDisable(){
		btnClose.onClick.RemoveAllListeners();
	}

	// Use this for initialization
	void OnEnable () {
		btnClose.onClick.AddListener(()=>ClosePanel());

		btnClose.gameObject.SetActive(false);

		cgPanelFather = GetComponent<CanvasGroup> ();
		cgPanelFather.alpha = 0f;

		//add childs lezantes
		InitGredits ();

		Tools_UI.RescaleRawImage (imageUp);
		Tools_UI.Move (imageUp.rectTransform, Tools_UI.Mode.up);

		Tools_UI.RescaleRawImage (imageDown);
		Tools_UI.Move (imageDown.rectTransform, Tools_UI.Mode.down);

		StartCoroutine (moveBlackUp ());
	}

	IEnumerator moveBlackUp(){

//		platformaMaska.alpha = 1f;
		
		//fade in
		while(cgPanelFather.alpha<0.99f)
		{
			cgPanelFather.alpha = Mathf.Lerp(cgPanelFather.alpha,1f,Time.deltaTime * 4f);	
			yield return null;
		}

		yield return new WaitForSeconds(3f);

		if(mustQuit){
			#if UNITY_EDITOR
			Debug.LogWarning("QUIT APP");
			#endif

			Gps.Instance.Stop();

			PlayerPrefs.Save();

			Application.Quit();
		}else{
			btnClose.gameObject.SetActive(true);
		}


		yield break;
	}

	IEnumerator closeGreditsWithFade(){

		//		platformaMaska.alpha = 1f;

		//fade out
		while(cgPanelFather.alpha>0.01f)
		{
			cgPanelFather.alpha = Mathf.Lerp(cgPanelFather.alpha,0f,Time.deltaTime * 4f);	
			yield return null;
		}
			
		Diadrasis.Instance.CheckUserPrin();
		gameObject.SetActive(false);

		yield break;
	}

	public static bool mustQuit;

	#region Gredits XML
	
	/// <summary>
	/// Inits the sounds and create them into the scene.
	/// </summary>
	void InitGredits(){

		//load data xml files
		XmlDocument greditsXML = new XmlDocument();
		
		TextAsset textAsset = (TextAsset) Resources.Load("XML/credits");
		string excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		greditsXML.LoadXml(excludedComments);

		//Read general move settings
		XmlNodeList personsList =greditsXML.SelectNodes ("/credits/persons/person");

//		allPersons = personsList.Count * 2f;
		
		#if UNITY_EDITOR
		Debug.LogWarning("creditsXML have "+personsList.Count+" persons");
		#endif

		Transform[] childs = platforma.transform.GetComponentsInChildren<Transform>();

		if(childs.Length>0){
			foreach(Transform ch in childs){
				if(ch != platforma.transform){
					Destroy(ch.gameObject);
				}
			}
		}
		
		if (personsList.Count > 0) {
			//instatiate FINAL/ui/titlosGredits

			foreach(XmlNode person in personsList){
				if(!string.IsNullOrEmpty(person ["name"][appSettings.language].InnerText))
				{
					//Debug.Log(person ["name"][appSettings.language].InnerText);
					GameObject ps = Instantiate(Resources.Load("prefabs/FINAL/ui/titlosGredits")) as GameObject;
					GreditsPerson gp = ps.GetComponent<GreditsPerson>();
					string a = person ["name"][appSettings.language].InnerText;
					string b = "<i><color=grey>"+person ["idiotita"][appSettings.language].InnerText+"</color></i>";
					gp.infoText.text = b + "\n" + a;
					ps.transform.SetParent(platforma.transform);
					ps.transform.localPosition = Vector3.zero;
					ps.transform.localScale = Vector3.one;
				}
			}
		}
		
		
	}
	
	#endregion
	
}
