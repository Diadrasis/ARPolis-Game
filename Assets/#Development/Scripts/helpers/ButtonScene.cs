using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using eChrono;

/// <summary>
///  in scene prefab (ButtonInfo)
/// </summary>
public class ButtonScene : MonoBehaviour {

	public CanvasGroup canvGroup;
	public bool isHiding;
	public RectTransform rt;
	public Transform periodsContainer;
	public Text label;
	public Button myBtn;
	public string keyPoi;
	public RectTransform btnOnMapRT;
	public RectTransform parentRT;
	public Vector2 defaultMapSize;
	public Vector2 btnOnMapDefaultSize;
	public Vector2 btnOnMapPosition;

	public int myIndexInScenes;
	public string langNow="";
	Image maska;

	public void InitLabelMenu(){

		maska = GetComponent<Image>();

		//get device screen size
		//set label and text dimensions
		//if not tablet
		if(Diadrasis.Instance.screenSize<2){
			label.fontSize=32;
		}
		Invoke("SetSize",0.05f);
	}

	void OnEnable(){
//		if(Application.loadedLevel==0)//onStart
//		{
			langNow=appSettings.language;
			Invoke("SetLanguange",0.7f);
//		}
	}

	void SetSize(){
		Vector2 ss = rt.sizeDelta;
		Vector2 pp = label.GetComponent<RectTransform>().sizeDelta;
		ss.x = pp.x + 10f;		//Debug.Log(ss.x);
		rt.sizeDelta = ss;
	}

	public void SetPoi()
	{
		Diadrasis.Instance.currentPoi = keyPoi;
	}

	void Update()
	{

		if(isHiding){
			if(canvGroup){
				if(Diadrasis.Instance.enableDiadrasisScene && canvGroup.alpha!=1f){
					canvGroup.alpha=1f;
					canvGroup.interactable=true;
					canvGroup.blocksRaycasts=true;
					canvGroup.ignoreParentGroups=true;
					maska.enabled=true;
					label.enabled=true;
				}else
				if(!Diadrasis.Instance.enableDiadrasisScene ){
					if(canvGroup.alpha!=0f){
						canvGroup.alpha=0f;
						canvGroup.interactable=false;
						canvGroup.blocksRaycasts=false;
						canvGroup.ignoreParentGroups=true;
						maska.enabled=false;
						label.enabled=false;
					}
				}
			}
		}

		//if user change the languange 
		//refresh period texts
		if(!string.IsNullOrEmpty(langNow))
		{
			//Debug.Log(Diadrasis.Instance.languangeNow.ToString());
			if(Diadrasis.Instance.languangeNow.ToString() != langNow)
			{
				langNow=appSettings.language;
				Invoke("SetLanguange",0.7f);
			}
		}
	}

	public void SetLanguange()
	{
//		#if UNITY_EDITOR
//		Debug.Log("Set Languange for sceneMap");
//		#endif

		if(Application.loadedLevel==0)//onStart
		{
			cSceneArea myPoi;
			appData.mySceneAreas.TryGetValue(keyPoi, out myPoi);

//			#if UNITY_EDITOR
//			Debug.Log(keyPoi);
//			#endif

			if(myPoi!=null){
				if(!label){transform.GetComponentInChildren<Text>();}
				if(label){
					label.text = myPoi.LabelTitle;
				}
			}

		}
		else
		{
			if(appData.myPoints.Count>0)
			{
				cPoi myPoi;
				appData.myPoints.TryGetValue(keyPoi, out myPoi);

				if(myPoi!=null){
					if(!label){transform.GetComponentInChildren<Text>();}
					if(label){
						label.text = myPoi.title;
					}
				}
			}
		}

		CancelInvoke();
	}
}
