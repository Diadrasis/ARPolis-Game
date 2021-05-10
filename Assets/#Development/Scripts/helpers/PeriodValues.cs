using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using eChrono;
using System.Xml;
using System;
using System.Linq;

/// <summary>
/// in period prefab (period)
/// </summary>
public class PeriodValues : MonoBehaviour {

	public Button btn;
	public Text periodLabel, gamePoints;
	public Image periodImage;
	public string onomaSkinis;
//	public string kameraSkinis;
	public string onomaPoi;
	public string loadingImage;
//	public List<cIntroNarration> introNarrations = new List<cIntroNarration>();
	public string introText, loadingText, introTitle;
//	public float introPauseTime;
	public string keyPoiname;
	//index of label to change languange
	public int myIndexInPeriods;
	public string langNow="";
	public string map;
	//public string mapFilter;
	public Vector2 mapPivot;
	public Vector2 mapFullPosition;
	public Vector2 mapFullZoom;

//	public List<Vector3> spotPositions = new List<Vector3>();

	public XartisMenu xartisMenu;

	public void OpenScene()
	{																	//Debug.Log("open scene "+onomaSkinis);

		Diadrasis.Instance.sceneName = onomaSkinis;
//		Diadrasis.Instance.kameraSkinis = kameraSkinis;
		Diadrasis.Instance.XmlPointsTagName = onomaPoi;
		Diadrasis.Instance.introText = introText;
		Diadrasis.Instance.introTitle = introTitle;

		Diadrasis.Instance.loadingText = loadingText;
		Diadrasis.Instance.loadingImage = loadingImage;

//		Diadrasis.Instance.loadingIntroAudio = introNarrations;
//		Diadrasis.Instance.introPauseTime = introNarrations[0].pauseTime;

		Diadrasis.Instance.mapScene = map;
		//Diadrasis.Instance.mapFilter = mapFilter;
		Diadrasis.Instance.mapPivot = mapPivot;
		Diadrasis.Instance.mapFullPosition = mapFullPosition;
		Diadrasis.Instance.mapFullZoom = mapFullZoom;

//		Diadrasis.Instance.menuUI.xartis.spotPositions.Clear();
//		Diadrasis.Instance.menuUI.xartis.spotPositions = spotPositions;

		//Read 
		//read gps ref point
		XmlNode sceneArea = Globals.Instance.movementXml.SelectNodes ("/movement/" + onomaPoi + "/sceneArea")[0];
		float lat0 = float.Parse(sceneArea["lat"].InnerText);
		float lon0 = float.Parse(sceneArea["lon"].InnerText);
		moveSettings.gpsRefLoc=new Vector2(lat0,lon0);
		if(!string.IsNullOrEmpty(sceneArea["posX"].InnerText) && !string.IsNullOrEmpty(sceneArea["posZ"].InnerText)){
			float posX = float.Parse(sceneArea["posX"].InnerText);
			float posZ = float.Parse(sceneArea["posZ"].InnerText);
			moveSettings.posCenterOfMap=new Vector2(posX,posZ);
		}else{
			#if UNITY_EDITOR
			//Debug.LogWarning(onomaPoi+" center = "+moveSettings.posCenterOfMap);
			#endif
		}

		#if UNITY_EDITOR
		//Debug.Log(onomaPoi+" center = "+moveSettings.posCenterOfMap);
		#endif

		//orio skinis gia ploigisi kai emfanisi full map
		moveSettings.activeAreaForScene=new List<cArea>();
		
		XmlNodeList areaList = Globals.Instance.movementXml.SelectNodes ("/movement/" + onomaPoi + "/ActiveAreaForScene/activeArea");
		foreach (XmlNode area in areaList) {							
			if (area.Attributes ["status"] != null) {
				if (area.Attributes ["status"].Value == "on") {
					cArea myArea = new cArea();
					//myArea.name=area["name"].InnerText;
					//myArea.center=new Vector2(float.Parse(area["center"]["x"].InnerText),float.Parse(area["center"]["z"].InnerText));
					//myArea.radius=float.Parse(area["radius"].InnerText);
					
					XmlNodeList psList = area["points"].ChildNodes;
					List<Vector3> ps = new List<Vector3> ();
					foreach (XmlNode p in psList) {
						if (p.Name == "point") {
							Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
							ps.Add (v);
						} 
					}
					myArea.Simeia=ps;

					//the area that user can walk in on-site mode
					//and the area for checking if we are near to a scene
					//at menu map
					moveSettings.activeAreaForScene.Add(myArea);

					#if UNITY_EDITOR
					//Debug.Log(onomaPoi+" area for scene exists");
					#endif
				}
			}
		}
		
		//Read settings from the specific scene
		XmlNodeList refPoints=Globals.Instance.movementXml.SelectNodes ("/movement/" + onomaPoi + "/sceneArea/refPoints");
		moveSettings.gpsPointA=new Vector2(float.Parse(refPoints[0]["pointA"]["lat"].InnerText), float.Parse(refPoints[0]["pointA"]["lon"].InnerText));
		moveSettings.posPointA=new Vector2(float.Parse(refPoints[0]["pointA"]["x"].InnerText), float.Parse(refPoints[0]["pointA"]["y"].InnerText));
		moveSettings.gpsPointB=new Vector2(float.Parse(refPoints[0]["pointB"]["lat"].InnerText), float.Parse(refPoints[0]["pointB"]["lon"].InnerText));
		moveSettings.posPointB=new Vector2(float.Parse(refPoints[0]["pointB"]["x"].InnerText), float.Parse(refPoints[0]["pointB"]["y"].InnerText));

		//TODO
		//check user position and scene gpsRefLoc center;
		Diadrasis.Instance.sceneGpsPosition = moveSettings.gpsRefLoc;

		if(Gps.Instance.isWorking())//select mode
		{
			if(xartisMenu.isUserNearToSpecificScene())//show select onsite or offsite message
			{
				Diadrasis.Instance.menuUI.warningsUI.showSelectModeWarning=true;
				return;
			}
			else//nav mode off site
			{
				Diadrasis.Instance.navMode=Diadrasis.NavMode.offSite;
				Diadrasis.Instance.menuUI.warningsUI.loadPeriodScene();
			}
		}
		else//nav mode off site
		{
			Diadrasis.Instance.navMode=Diadrasis.NavMode.offSite;
			Diadrasis.Instance.menuUI.warningsUI.loadPeriodScene();
		}

	}


	void Update()
	{
		//if user change the languange 
		//refresh period texts
		if(!string.IsNullOrEmpty(langNow))
		{
			if(langNow != appSettings.language)
			{
				langNow=appSettings.language;
				SetLanguange();
			}
		}
	}

	void SetLanguange()
	{
		#if UNITY_EDITOR
		Debug.Log("Set Languange for period");
		#endif

		//dont need it we get them from changing the languange >> settingsUI -> SetLanguange
//		xartisMenu.GetXmlScenes();

		cSceneArea myPoi;
		appData.mySceneAreas.TryGetValue(keyPoiname, out myPoi);

		periodLabel.text = myPoi.Periods[myIndexInPeriods].LabelTitle;
		loadingText = myPoi.Periods[myIndexInPeriods].Intro.LoadingText;
		introText = myPoi.Periods[myIndexInPeriods].Intro.IntroText;
		introTitle = myPoi.Periods[myIndexInPeriods].Intro.IntroTitle;
	}
}
