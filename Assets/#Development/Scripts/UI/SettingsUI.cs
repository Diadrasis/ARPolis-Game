using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine.UI.Extensions;
using System.Xml;

[Serializable]
public class LangPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Slider selectLang;
	public Slider fontSize;
	public Slider fontStyle;
	public Slider letterSpace;
	public Slider fontType;
	public Font[] fonts;

	public Slider slider_MaxDistPoiLabel;
	public Text txt_maxDistPoiValue;
}

[Serializable]
public class GeometryPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Slider speedWalk;
	public Text textSpeed;
	public Toggle snapPath;
	public Slider snapDistance;
	public Text textSnapDist;
	public Slider gpsOffset_X;
	public Text textOffsetX;
	public Slider gpsOffset_Y;
	public Text textOffsetY;
	public Toggle toggleJoys;
	public Toggle mapFilterToggle;
	//on-off settings
	public GameObject speedShow;
	public GameObject snapPathShow;
	public GameObject distPathShow;
	public GameObject gpsOffsetXshow;
	public GameObject gpsOffsetYshow;
}

[Serializable]
public class KameraPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Slider fovSlider;
	public Text textFOV;
	//gyro -> moveSettings.cameraHorAngleOffset
	public Slider gyroHorzAngle;
	public Text textHorzAngle;
	//gyro -> moveSettings.cameraVertAngleOffset
	public Slider gyroVertAngle;
	public Text textVertAngle;

	public Button gyroBtnReset;
	
	//accel -> 
	public Slider accelHorzAngle;
	public Text acceltextHorzAngle;
	//accel -> 
	public Slider accelVertAngle;
	public Text acceltextVertAngle;

	public Slider accelTurnSpeed_Y;
	public Text acceltextTurnSpeed_Y;

	public Slider accelSensitivity;
	public Text acceltextSensitivity;

	public Button accelBtnReset;

	//joys
	public Slider joyRightSensitivity;
	public Text joystextSens;

	//on-off panels checking sensorUsing
	public GameObject gyroPanel;
	public GameObject accelPanel;
	public GameObject joyPanel;
}

[Serializable]
public class ExpertPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Toggle navToggle;
	public GameObject joysToggle;
}

//[Serializable]
//public class ExpertPanelDiadrasis  : System.Object//Not a MonoBehaviour!
//{
//	public GameObject mainPanel;
//	public Slider initTimeSlider;
//	public Text textTimeValue;
//	public Slider accuracySlider;
//	public Text textAccuracyValue;
//	public Toggle enableOnSiteWalk;
//}



[Serializable]
public class ButtonsPlagia  : System.Object//Not a MonoBehaviour!
{
	//plaina buttons
	public Button btnGeometry;
	public Button btnKamera;
	public Button btnLanguage;
	public Button btnExpert;
	
	public GameObject[] btnsPlaisia;
}


public class SettingsUI : MonoBehaviour {

	public LangPanel langPanel;
	public GeometryPanel geometryPanel;
	public KameraPanel kameraPanel;
	public ExpertPanel expertPanel;
//	public ExpertPanelDiadrasis expertPanelDiadrasis;
	public ButtonsPlagia btnsPlagia;

	//display build version into settings panel
	public Text txtVersion;

	public void SetVersion(string val){
		txtVersion.text = val;
	}



	#region (Send Sxolio)

//	string serverPHP = "http://www.e-chronomichani.gr/TimeWalk/";
//	string phpSxolia = "sxolia/recSxolia.php?";

	public InputField sxolioTextField;
	public Button btnSend, btnOpenSxolioPanel;
	public GameObject btnShowSxolioText, btnSendSxolio, textSendOk, textInputField, panelContainerSxoliou, textSendError;

	public void InitSxolioPanel(){
		sxolioTextField.textComponent.fontSize = appSettings.fontSize_keimeno;
		btnSendSxolio.SetActive(true);
		textInputField.SetActive(true);
		textSendOk.SetActive(false);
		textInputField.SetActive(true);
		panelContainerSxoliou.SetActive(true);
	}
	
	public void SendSxolio(){
		
		if(string.IsNullOrEmpty(sxolioTextField.text)){return;}

		btnSendSxolio.SetActive(false);
		textInputField.SetActive(false);
		
		StopCoroutine("sendSxolio");
		StartCoroutine("sendSxolio");
	}
	
	
	IEnumerator sendSxolio(){
		string sxolio = sxolioTextField.text;
		
		string url = "http://www.e-chronomichani.gr/timewalk/sxolia/recSxolia.php?txtcontent="; //"http://www.e-chronomichani.gr/TimeWalk/sxolia/recSxolia.php?txtcontent="; //serverPHP+phpSxolia+"txtcontent="; //"http://www.stagegames.eu/GAMES/gnothi/recSxolia.php?txtcontent=";

		string finalSxolio = "\nΈκδοση εφαρμογής : "+Diadrasis.Instance.appVersion+"\n"+System.DateTime.Now.ToString()+"\n*deviceModel="+SystemInfo.deviceModel+"\n*OS="+SystemInfo.operatingSystem+"\n*processorCount="+SystemInfo.processorCount+"\nΣχόλιο\n{"+sxolio+"}";
//		finalSxolio.Trim();
		finalSxolio = WWW.EscapeURL(finalSxolio);
		
		WWW www = new WWW(url+finalSxolio);
		
		yield return www;

		if(www.error == null){
//			Debug.Log("sxolio send OK !!");
			textInputField.SetActive(false);
			textSendOk.SetActive(true);
		}else{
//			Debug.Log(www.error);
			textInputField.SetActive(false);
			textSendError.SetActive(true);
		}

		yield return new WaitForSeconds(2.5f);

		panelContainerSxoliou.SetActive(false);
		sxolioTextField.text=string.Empty;
		
		yield break;
	}
	
	
	#endregion

//	public Button btnSecret;

//	public GameObject container;

//	public void ShowSettings(bool show){
//		StopAllCoroutines();
//		
//		if(show){
//			Diadrasis.Instance.ChangeStatus(Diadrasis.User.inSettings);
//			container.GetComponent<CanvasGroup>().alpha=0f;
//			container.SetActive(true);
//			CheckStatus();
//			StartCoroutine(fadeIn());
//		}else{
//			container.GetComponent<CanvasGroup>().alpha=1f;
//			StartCoroutine(fadeOut());
//		}
//
//
//
//	}

//	IEnumerator fadeIn(){
//		//fade in
//		while(container.GetComponent<CanvasGroup>().alpha<0.99f)
//		{
//			container.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(container.GetComponent<CanvasGroup>().alpha,1f,Time.deltaTime * 0.7f);	
//			yield return null;
//		}
//		
//		//fade in full
//		container.GetComponent<CanvasGroup>().alpha=1f;
//
//		yield break;
//	}
//
//	IEnumerator fadeOut(){
//		//fade in
//		while(container.GetComponent<CanvasGroup>().alpha>0.01f)
//		{
//			container.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(container.GetComponent<CanvasGroup>().alpha,0f,Time.deltaTime * 0.7f);	
//			yield return null;
//		}
//		
//		//fade in full
//		container.GetComponent<CanvasGroup>().alpha=0f;
//
//		CheckStatus();
//		
//		container.SetActive(false);
//		
//		yield break;
//	}


	public Text[] allTexts;

	List<GameObject> panelsInSettings = new List<GameObject>();

	public void OpenKameraSettings(){
		if(Diadrasis.Instance.user==Diadrasis.User.inMenu){
			TitloiTelous.mustQuit=false;
			Diadrasis.Instance.menuUI.QuitTimeWalk();
			return;
		}
		//Debug.Log("OpenKameraSettings");
		Diadrasis.Instance.animControl.SettingsButton();
//		ShowSettings(true);
		//wait animation to finished
		Invoke("arga", 0.5f);
	}

	void arga(){
		CheckStatus();
		
		langPanel.mainPanel.SetActive(false);
		kameraPanel.mainPanel.SetActive(true);
		//show settings panel and hide rest
		ShowPanelAtSettings(kameraPanel.mainPanel, btnsPlagia.btnKamera);

		CancelInvoke();
	}

	IEnumerator Start () {

//		sxolioTextField.onValueChange.AddListener((b)=>myComment=sxolioTextField.text);
		btnOpenSxolioPanel.onClick.AddListener(()=>InitSxolioPanel());
		btnSend.onClick.AddListener(()=>SendSxolio());

		if(!Diadrasis.Instance.useOnSiteMode){
			expertPanel.navToggle.transform.parent.gameObject.SetActive(false);
		}

//		panelsInSettings.Add(expertPanelDiadrasis.mainPanel);


//		expertPanelDiadrasis.enableOnSiteWalk.onValueChanged.AddListener((b)=>EnableEpitopioMode(expertPanelDiadrasis.enableOnSiteWalk.isOn));
//		expertPanelDiadrasis.enableOnSiteWalk.isOn = Diadrasis.Instance.useOnSiteMode;
//
////		expertPanelDiadrasis.enableOnSiteWalk.gameObject.SetActive(false);
//
//
//
//		expertPanelDiadrasis.initTimeSlider.minValue=10f;
//		expertPanelDiadrasis.initTimeSlider.maxValue=60f;
//		expertPanelDiadrasis.initTimeSlider.wholeNumbers=true;
//		expertPanelDiadrasis.initTimeSlider.onValueChanged.AddListener((b)=>SetGpsInitTime(expertPanelDiadrasis.initTimeSlider.value));
//
//		//speed
//		expertPanelDiadrasis.accuracySlider.minValue=3f;
//		expertPanelDiadrasis.accuracySlider.maxValue=10f;
//		expertPanelDiadrasis.accuracySlider.wholeNumbers=true;
//		expertPanelDiadrasis.accuracySlider.onValueChanged.AddListener((b)=>SetGpsAccuracy(expertPanelDiadrasis.accuracySlider.value));
//
//		expertPanelDiadrasis.initTimeSlider.value = Gps.Instance.maxGpsAccuracy;
//		expertPanelDiadrasis.textAccuracyValue.text = Mathf.RoundToInt(Gps.Instance.maxGpsAccuracy).ToString();
//
//		expertPanelDiadrasis.accuracySlider.value = Gps.Instance.gpsInitTime;
//		expertPanelDiadrasis.textTimeValue.text = Gps.Instance.gpsInitTime.ToString();
//
//		btnSecret.onClick.AddListener(()=>SetExpert());

		//plaina buttons
		btnsPlagia.btnGeometry.onClick.AddListener(()=>ResetCounter());
		btnsPlagia.btnGeometry.onClick.AddListener(()=>ShowPanelAtSettings(geometryPanel.mainPanel, btnsPlagia.btnGeometry));
		btnsPlagia.btnKamera.onClick.AddListener(()=>ResetCounter());
		btnsPlagia.btnKamera.onClick.AddListener(()=>ShowPanelAtSettings(kameraPanel.mainPanel, btnsPlagia.btnKamera));
		btnsPlagia.btnLanguage.onClick.AddListener(()=>ShowPanelAtSettings(langPanel.mainPanel, btnsPlagia.btnLanguage));
		btnsPlagia.btnExpert.onClick.AddListener(()=>ResetCounter());
		btnsPlagia.btnExpert.onClick.AddListener(()=>ShowPanelAtSettings(expertPanel.mainPanel, btnsPlagia.btnExpert));
		btnsPlagia.btnExpert.onClick.AddListener(()=>CheckGpsStatus());
//		btnsPlagia.btnExpertDiadrasis.onClick.AddListener(()=>ShowPanelAtSettings(expertPanelDiadrasis.mainPanel, btnsPlagia.btnExpertDiadrasis));
	
//_______________________________________________________________________________________________________________
		//languange panel
		panelsInSettings.Add(langPanel.mainPanel);
		langPanel.selectLang.onValueChanged.AddListener((b)=>SetLanguange(langPanel.selectLang.value));
		langPanel.fontSize.minValue=14f;
		langPanel.fontSize.maxValue=23f;
		langPanel.fontStyle.wholeNumbers=true;
		langPanel.fontSize.onValueChanged.AddListener((b)=>SetFontSize(langPanel.fontSize.value));

		//fonts
		if(langPanel.fonts.Length>0){
			langPanel.fontType.minValue=0f;
			langPanel.fontType.maxValue=langPanel.fonts.Length-1;
			langPanel.fontStyle.wholeNumbers=true;
			langPanel.fontType.onValueChanged.AddListener((b)=>SetFontType(langPanel.fontType.value));
		}

		//letter space
		langPanel.letterSpace.minValue=1f;
		langPanel.letterSpace.maxValue=50f;
		langPanel.fontStyle.wholeNumbers=true;
		langPanel.letterSpace.onValueChanged.AddListener((b)=>SetLetterSpace(langPanel.letterSpace.value));

		//bold-normal
		langPanel.fontStyle.minValue=0f;
		langPanel.fontStyle.maxValue=1f;
		langPanel.fontStyle.wholeNumbers=true;
		langPanel.fontStyle.onValueChanged.AddListener((b)=>SetFontStyle(langPanel.fontStyle.value));

		//max dist labels poi
		langPanel.slider_MaxDistPoiLabel.minValue=5f;
		langPanel.slider_MaxDistPoiLabel.maxValue=300f;
		langPanel.slider_MaxDistPoiLabel.wholeNumbers=true;
		langPanel.slider_MaxDistPoiLabel.onValueChanged.AddListener((b)=>SetMaxPoiDistLabel(langPanel.slider_MaxDistPoiLabel.value));

//_______________________________________________________________________________________________________________
		//geometry panel
		panelsInSettings.Add(geometryPanel.mainPanel);
		//speed
		geometryPanel.speedWalk.minValue=3f;
		geometryPanel.speedWalk.maxValue=20f;
		geometryPanel.speedWalk.wholeNumbers=true;
		geometryPanel.speedWalk.onValueChanged.AddListener((b)=>SetSpeed(geometryPanel.speedWalk.value));
		//snap
		//geometryPanel.snapPath.onValueChanged.AddListener((b)=>SetMoveInPath(geometryPanel.snapPath.isOn));
		//map filter
		//geometryPanel.mapFilterToggle.onValueChanged.AddListener((b)=>SetMoveByMapFilter(geometryPanel.mapFilterToggle.isOn));
		//joysticks on-off
		if(Diadrasis.Instance.sensorAvalaible!=Diadrasis.SensorAvalaible.empty){
			geometryPanel.toggleJoys.isOn=false;
		}else{
			geometryPanel.toggleJoys.isOn=true;
		}
		geometryPanel.toggleJoys.onValueChanged.AddListener((b)=>SetJoysticks(geometryPanel.toggleJoys.isOn));
		//snap distance
//		geometryPanel.snapDistance.minValue=10f;
//		geometryPanel.snapDistance.maxValue=30f;
//		geometryPanel.snapDistance.wholeNumbers=true;
//		geometryPanel.snapDistance.onValueChanged.AddListener((b)=>SetMaxSnapToPath(geometryPanel.snapDistance.value));
		//offset x
		geometryPanel.gpsOffset_X.minValue=-20f;
		geometryPanel.gpsOffset_X.maxValue=20f;
		geometryPanel.gpsOffset_X.wholeNumbers=true;
		geometryPanel.gpsOffset_X.onValueChanged.AddListener((b)=>SetOffset_X(geometryPanel.gpsOffset_X.value));
		//offset y
		geometryPanel.gpsOffset_Y.minValue=-20f;
		geometryPanel.gpsOffset_Y.maxValue=20f;
		geometryPanel.gpsOffset_Y.wholeNumbers=true;
		geometryPanel.gpsOffset_Y.onValueChanged.AddListener((b)=>SetOffset_Y(geometryPanel.gpsOffset_Y.value));

//_______________________________________________________________________________________________________________
		//kamera panel
		panelsInSettings.Add(kameraPanel.mainPanel);
		kameraPanel.fovSlider.minValue=30f;
		kameraPanel.fovSlider.maxValue=60f;
		kameraPanel.fovSlider.wholeNumbers=true;
		kameraPanel.fovSlider.onValueChanged.AddListener((b)=>SetFov(kameraPanel.fovSlider.value));
		kameraPanel.gyroHorzAngle.minValue=-60f;
		kameraPanel.gyroHorzAngle.maxValue=60f;
		kameraPanel.gyroHorzAngle.wholeNumbers=true;
		kameraPanel.gyroHorzAngle.onValueChanged.AddListener((b)=>SetGyroHorizontalAngle(kameraPanel.gyroHorzAngle.value));
		kameraPanel.gyroVertAngle.minValue=-70f;
		kameraPanel.gyroVertAngle.maxValue=0f;
		kameraPanel.gyroVertAngle.wholeNumbers=true;
		kameraPanel.gyroVertAngle.value=moveSettings.cameraVertAngleOffset;
		kameraPanel.gyroVertAngle.onValueChanged.AddListener((b)=>SetGyroVerticalAngle(kameraPanel.gyroVertAngle.value));
		//reset gyro
		kameraPanel.gyroBtnReset.onClick.AddListener(()=>ResetGyroskopio());
		//reset accelerometer
		kameraPanel.accelBtnReset.onClick.AddListener(()=>ResetAccel());
		//accel hor angle
		kameraPanel.accelHorzAngle.minValue=-60f;
		kameraPanel.accelHorzAngle.maxValue=60f;
		kameraPanel.accelHorzAngle.wholeNumbers=true;
		kameraPanel.accelHorzAngle.value=moveSettings.camAccelRotX;
		kameraPanel.accelHorzAngle.onValueChanged.AddListener((b)=>SetAccel_HorizontalAngle(kameraPanel.accelHorzAngle.value));
		//accel vert angle
		kameraPanel.accelVertAngle.minValue=-30f;
		kameraPanel.accelVertAngle.maxValue=30f;
		kameraPanel.accelVertAngle.wholeNumbers=true;
		kameraPanel.accelVertAngle.value=moveSettings.camAccelRotY;
		kameraPanel.accelVertAngle.onValueChanged.AddListener((b)=>SetAccel_VerticalAngle(kameraPanel.accelVertAngle.value));
		//accel sensitivity
		kameraPanel.accelSensitivity.minValue=5f;
		kameraPanel.accelSensitivity.maxValue=15f;
		kameraPanel.accelSensitivity.wholeNumbers=true;
		kameraPanel.accelSensitivity.value=moveSettings.accelSensitivity*100f;
		kameraPanel.accelSensitivity.onValueChanged.AddListener((b)=>SetAccel_Sensitivity(kameraPanel.accelSensitivity.value));
		//accel compass turn speed
		kameraPanel.accelTurnSpeed_Y.minValue=1f;
		kameraPanel.accelTurnSpeed_Y.maxValue=4f;
		kameraPanel.accelTurnSpeed_Y.wholeNumbers=true;
		kameraPanel.accelTurnSpeed_Y.value=moveSettings.compassTurnSpeed;
		kameraPanel.accelTurnSpeed_Y.onValueChanged.AddListener((b)=>SetCompass_TurnSpeed(kameraPanel.accelTurnSpeed_Y.value));
		//joy right sensitivity
		kameraPanel.joyRightSensitivity.minValue=1f;
		kameraPanel.joyRightSensitivity.maxValue=15f;
		kameraPanel.joyRightSensitivity.wholeNumbers=true;
		kameraPanel.joyRightSensitivity.value=moveSettings.joyRightSensitivity;
		kameraPanel.joyRightSensitivity.onValueChanged.AddListener((b)=>SetJoyRight_Sensitivity(kameraPanel.joyRightSensitivity.value));

//_______________________________________________________________________________________________________________
		//expert panel
		panelsInSettings.Add(expertPanel.mainPanel);
		//nav mode on-off
		if(Diadrasis.Instance.navMode!=Diadrasis.NavMode.onSite){
			expertPanel.navToggle.isOn=false;
		}else{
			expertPanel.navToggle.isOn=true;
		}
		expertPanel.navToggle.onValueChanged.AddListener((b)=>SetNavMode(expertPanel.navToggle.isOn));

//________________________________________________________________________________________________________________
		//expert diadrasis

		yield return new WaitForSeconds(3f);

		CheckStatus();

	}

	public void SetNavMode(bool val){

		#if UNITY_EDITOR
		Debug.LogWarning("MANUAL SETTING GPS NAVIGATION MODE");
		Debug.LogWarning("do we need to Destroy person ??");
		#endif

		if(val){
//			Gps.Instance.CheckGPS();
			
			if(Gps.Instance.isWorking())
			{
				Diadrasis.Instance.navMode=Diadrasis.NavMode.onSite;

				//replace person
				if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
				{
					Diadrasis.Instance.AddPerson();
				}

				if(Diadrasis.Instance.sensorUsing!=Diadrasis.SensorUsing.joysticks)
				{
					//disable joystick for move
					Diadrasis.Instance.menuUI.joy.singleJoyLeft.SetActive(false);
				}
				//if gps is enabled and we get gps position stop here
				//else set off site
				return;
			}
		}

		#if UNITY_EDITOR
		Debug.LogWarning("OFF SITE MODE - GPS is disabled");
		#endif

		expertPanel.navToggle.isOn = false;

		Diadrasis.Instance.navMode=Diadrasis.NavMode.offSite;

		//replace person
		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating)
		{
			Diadrasis.Instance.AddPerson();
		}

		if(Diadrasis.Instance.sensorUsing!=Diadrasis.SensorUsing.joysticks)
		{
			//add one joystick for move
			Diadrasis.Instance.menuUI.joy.singleJoyLeft.SetActive(true);
		}
	}
	
	void CheckGpsStatus () {

		if(!Gps.Instance.isWorking()){
			expertPanel.navToggle.interactable=false;
			return;
		}

		Vector3 personPos = Diadrasis.Instance.person.transform.position;	
		Vector2 posA = new Vector2(personPos.x, personPos.z); 
		Vector2 posB = gpsPosition.FindPosition(Gps.Instance.GetMyLocation());

//		posB+=moveSettings.posCenterOfMap;

		float dist = Vector2.Distance(posA, posB);

		#if UNITY_EDITOR
		Debug.LogWarning("CheckGpsStatus to enable button manual gps nav mode selection");
		
		Debug.LogWarning("person pos = "+posA);
		Debug.LogWarning("gps pos = "+posB);
		Debug.LogWarning("dist = "+dist);
		#endif
		
		if(dist>300f){
			expertPanel.navToggle.interactable=false;//.gameObject.SetActive(false);
		}else{
			expertPanel.navToggle.interactable=true;//.gameObject.SetActive(true);
		}
	}

	AccelControl accelControl;
	Compass compass;
	MovePerson_Gyro movePersonGyro;
	MovePerson_Joys movePersonJoys;
	MovePerson_Accel movePersonAccel;

	//it is called when animation showSettings is starts or ends reverse
	void CheckStatus()
	{
		//Debug.Log("check settings status");

		Init();

		//init panels and values
		
		foreach(GameObject g in panelsInSettings){
			g.SetActive(false);
		}

		langPanel.mainPanel.SetActive(true);
		
		//show languange panel and hide rest
		SettBtnsPlaisio(btnsPlagia.btnLanguage);

		if(Diadrasis.Instance.userPrin==Diadrasis.UserPrin.inMenu)
		{
			btnsPlagia.btnExpert.gameObject.SetActive(false);
			btnsPlagia.btnGeometry.gameObject.SetActive(false);
			btnsPlagia.btnKamera.gameObject.SetActive(false);
//			btnsPlagia.btnExpertDiadrasis.gameObject.SetActive(false);
		}
		else
		{
			if(Diadrasis.Instance.userPrin==Diadrasis.UserPrin.onAir)
			{
				expertPanel.joysToggle.SetActive(false);
			}else
			if(Diadrasis.Instance.userPrin==Diadrasis.UserPrin.isNavigating){
				expertPanel.joysToggle.SetActive(true);
			}

			btnsPlagia.btnExpert.gameObject.SetActive(true);
			btnsPlagia.btnGeometry.gameObject.SetActive(true);
			btnsPlagia.btnKamera.gameObject.SetActive(true);

			//check nav mode and sensors
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
			{
				showGeometrySettings(false);
			}
			else
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
			{
				showGeometrySettings(true);
			}

			if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.accelCompass)
			{
				kameraPanel.gyroPanel.SetActive(false);
				kameraPanel.accelPanel.SetActive(true);
				kameraPanel.joyPanel.SetActive(false);

				if(!accelControl)
				{
					GameObject person = Diadrasis.Instance.person;
					if(person)
					{
						if(!accelControl)
						{
							accelControl = person.transform.GetChild(0).GetComponent<AccelControl>();
						}
						if(!compass)
						{
							compass = person.GetComponent<Compass>();
						}
						if(!movePersonAccel)
						{
							movePersonAccel = person.GetComponent<MovePerson_Accel>();
						}
					}
				}
			}
			else
			if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.gyroscopio)
			{
				kameraPanel.gyroPanel.SetActive(true);
				kameraPanel.accelPanel.SetActive(false);
				kameraPanel.joyPanel.SetActive(false);

				if(!movePersonGyro)
				{
					GameObject person = Diadrasis.Instance.person;
					if(person)
					{
						if(!movePersonGyro)
						{
							movePersonGyro = person.GetComponent<MovePerson_Gyro>();
						}
					}
				}
			}
			else
			if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
			{
				kameraPanel.gyroPanel.SetActive(false);
				kameraPanel.accelPanel.SetActive(false);
				kameraPanel.joyPanel.SetActive(true);

				if(!movePersonJoys)
				{
					GameObject person = Diadrasis.Instance.person;
					if(person)
					{
						if(!movePersonJoys)
						{
							movePersonJoys = person.GetComponent<MovePerson_Joys>();
						}
					}
				}
			}
		}

		//Debug.Log("check settings status telos");
	}

	void showGeometrySettings(bool isOnSite)
	{
		if(isOnSite)
		{
			geometryPanel.gpsOffsetXshow.SetActive(true);
			geometryPanel.gpsOffsetYshow.SetActive(true);
			geometryPanel.speedShow.SetActive(false);
		}
		else
		{
			geometryPanel.gpsOffsetXshow.SetActive(false);
			geometryPanel.gpsOffsetYshow.SetActive(false);
			geometryPanel.speedShow.SetActive(true);
		}

		geometryPanel.snapPathShow.SetActive(false);
		
	}

	//ola ta keimena
//	public List<Text> olaTaTexts = new List<Text>();

	void Init(){							//Debug.Log("INIT Settings");
		//translate ui texts 
		appData.Init();

		if(Diadrasis.Instance.olaTaKeimena_Settings.Count==0){
			Diadrasis.Instance.olaTaKeimena_Settings.AddRange(allTexts);

			foreach (Text t in Diadrasis.Instance.olaTaKeimena_Settings){
				ReplaceText(t);
				SetFontSize(17f);
//				SetFontType(0f);
			}
		}

		geometryPanel.snapPathShow.SetActive(false);

//		olaTaTexts.Clear();
//
//		foreach (var t in Diadrasis.Instance.olaTaLabelKeimena) {
//			olaTaTexts.Add(t);
//		}

//		expertPanelDiadrasis.enableOnSiteWalk.isOn = Diadrasis.Instance.useOnSiteMode;
//
//		expertPanelDiadrasis.initTimeSlider.value = Gps.Instance.maxGpsAccuracy;
//		expertPanelDiadrasis.textAccuracyValue.text = Mathf.RoundToInt(Gps.Instance.maxGpsAccuracy).ToString();
//
//		expertPanelDiadrasis.accuracySlider.value = Gps.Instance.gpsInitTime;
//		expertPanelDiadrasis.textTimeValue.text = Mathf.RoundToInt(Gps.Instance.gpsInitTime).ToString();


		//init max label show distance
		langPanel.slider_MaxDistPoiLabel.value = Diadrasis.Instance.menuUI.maxLabelDist;
		langPanel.txt_maxDistPoiValue.text=Mathf.RoundToInt(Diadrasis.Instance.menuUI.maxLabelDist).ToString();

		//init speed (data xml)
		if(PlayerPrefs.GetFloat("groundMoveSpeed")>0){
			moveSettings.groundMoveSpeed = PlayerPrefs.GetFloat("groundMoveSpeed");
		}
		geometryPanel.speedWalk.value=moveSettings.groundMoveSpeed;
		geometryPanel.textSpeed.text=Mathf.RoundToInt(moveSettings.groundMoveSpeed).ToString();
		//init snap to path bool
//		geometryPanel.snapPath.isOn=moveSettings.snapToPath;
//		geometryPanel.snapDistance.transform.parent.gameObject.SetActive(moveSettings.snapToPath);

		//init snap dist
//		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
//			if(PlayerPrefs.GetFloat("maxSnapPathDistOnsite")>0){
//				moveSettings.maxSnapPathDistOnsite = PlayerPrefs.GetFloat("maxSnapPathDistOnsite");
//			}
//			geometryPanel.snapDistance.value=moveSettings.maxSnapPathDistOnsite;
//			geometryPanel.textSnapDist.text=Mathf.RoundToInt(moveSettings.maxSnapPathDistOnsite).ToString();
//		}else //moveSettings.maxSnapPathDistOffsite
//		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
//			if(PlayerPrefs.GetFloat("maxSnapPathDistOffsite")>0){
//				moveSettings.maxSnapPathDistOffsite = PlayerPrefs.GetFloat("maxSnapPathDistOffsite");
//			}
//			geometryPanel.snapDistance.value=moveSettings.maxSnapPathDistOffsite;
//			geometryPanel.textSnapDist.text=Mathf.RoundToInt(moveSettings.maxSnapPathDistOffsite).ToString();
//		}

		//offset X
		if(PlayerPrefs.GetFloat("gpsOffsetX")!=0){
			moveSettings.gpsOffsetX = PlayerPrefs.GetFloat("gpsOffsetX");
		}
		geometryPanel.gpsOffset_X.value = moveSettings.gpsOffsetX; //(min=-20f / max=20f)
		geometryPanel.textOffsetX.text=Mathf.RoundToInt(moveSettings.gpsOffsetX).ToString();
		//offset Y
		if(PlayerPrefs.GetFloat("gpsOffsetY")!=0){
			moveSettings.gpsOffsetY = PlayerPrefs.GetFloat("gpsOffsetY");
		}
		geometryPanel.gpsOffset_Y.value = moveSettings.gpsOffsetY; //(min=-20f / max=20f)
		geometryPanel.textOffsetY.text=Mathf.RoundToInt(moveSettings.gpsOffsetY).ToString();
		//init map filter toggle
		geometryPanel.mapFilterToggle.isOn = Diadrasis.Instance.useMapFilterForMovement;

		//fiels of view
		if(Diadrasis.Instance.kamera)
		{		
			//Debug.Log(Mathf.RoundToInt(Diadrasis.Instance.kamera.fieldOfView));

			//camera settings
			if(PlayerPrefs.GetFloat("cameraFieldOfView")>0){
				moveSettings.cameraFieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
				Diadrasis.Instance.kamera.fieldOfView = moveSettings.cameraFieldOfView ;
			}

			kameraPanel.fovSlider.value=Mathf.RoundToInt(moveSettings.cameraFieldOfView );
			kameraPanel.textFOV.text = Mathf.RoundToInt(moveSettings.cameraFieldOfView).ToString();
		}

		//horizontal angle
		if(PlayerPrefs.GetFloat("cameraHorAngleOffset")<=0){
			PlayerPrefs.SetFloat("cameraHorAngleOffset", moveSettings.cameraHorAngleOffset);
		}else{
			moveSettings.cameraHorAngleOffset = PlayerPrefs.GetFloat("cameraHorAngleOffset");
		}

		kameraPanel.gyroHorzAngle.value=moveSettings.cameraHorAngleOffset;
		kameraPanel.textHorzAngle.text = Mathf.RoundToInt(moveSettings.cameraHorAngleOffset).ToString();

		//vertical angle
//		if(PlayerPrefs.GetFloat("cameraVertAngleOffset")<=0){
//			PlayerPrefs.SetFloat("cameraVertAngleOffset", moveSettings.cameraVertAngleOffset);
//		}else{
			moveSettings.cameraVertAngleOffset = PlayerPrefs.GetFloat("cameraVertAngleOffset");
//		}

		kameraPanel.gyroVertAngle.value=moveSettings.cameraVertAngleOffset;
		kameraPanel.textVertAngle.text = Mathf.RoundToInt(moveSettings.cameraVertAngleOffset).ToString();

		if(PlayerPrefs.GetFloat("camAccelRotX")>0){
			moveSettings.camAccelRotX = PlayerPrefs.GetFloat("camAccelRotX");
		}
		kameraPanel.accelHorzAngle.value = moveSettings.camAccelRotX;
		kameraPanel.acceltextHorzAngle.text = Mathf.RoundToInt(moveSettings.camAccelRotX).ToString();

		if(PlayerPrefs.GetFloat("camAccelRotY")>0){
			moveSettings.camAccelRotY = PlayerPrefs.GetFloat("camAccelRotY");
		}
		kameraPanel.accelVertAngle.value = moveSettings.camAccelRotY;
		kameraPanel.acceltextVertAngle.text = Mathf.RoundToInt(moveSettings.camAccelRotY).ToString();

		if(PlayerPrefs.GetFloat("compassTurnSpeed")>0){
			moveSettings.compassTurnSpeed = PlayerPrefs.GetFloat("compassTurnSpeed");
		}
		kameraPanel.accelTurnSpeed_Y.value = moveSettings.compassTurnSpeed;
		kameraPanel.acceltextTurnSpeed_Y.text = Mathf.RoundToInt(moveSettings.compassTurnSpeed).ToString();

		if(PlayerPrefs.GetFloat("accelSensitivity")>0){
			moveSettings.accelSensitivity = PlayerPrefs.GetFloat("accelSensitivity");
		}
		kameraPanel.accelSensitivity.value = moveSettings.accelSensitivity*100f;
		kameraPanel.acceltextSensitivity.text = Mathf.RoundToInt(moveSettings.accelSensitivity*100f).ToString();

		if(PlayerPrefs.GetFloat("joyRightSensitivity")>0){
			moveSettings.joyRightSensitivity = PlayerPrefs.GetFloat("joyRightSensitivity");
		}
		kameraPanel.joyRightSensitivity.value = moveSettings.joyRightSensitivity;
		kameraPanel.joystextSens.text = Mathf.RoundToInt(moveSettings.joyRightSensitivity).ToString();

		//init panels and values

//		CheckStatus();
	}


//###############################################		PLAGIA BUTTONS		######################################################################################
	//Buttons Settings
	void ShowPanelAtSettings(GameObject panel, Button btn){

#if UNITY_EDITOR
		Debug.LogWarning("ShowPanelAtSettings");
#endif

		//check all panels in settings
		foreach(GameObject g in panelsInSettings){
			//if its not the one that user select close it
			if(g!=panel){
				g.SetActive(false);
			}
			//otherwise open it
			else{
				g.SetActive(true);
			}
		}
		//enable plaisio for the button user has tapped
		SettBtnsPlaisio(btn);
	}
	
	
	void SettBtnsPlaisio(Button father){
		//search all plaisia
		foreach(GameObject g in btnsPlagia.btnsPlaisia){
			//if plaisio isnt the plaisio of the button user has pressed
			//hide it
			if(g.transform.parent!=father.transform){
				g.SetActive(false);
			}
			//show it
			else{
				g.SetActive(true);
			}
		}
	}

//###############################################       EXPERT PANEL 		######################################################################################

//###############################################       KAMERA PANEL 		######################################################################################

	void SetFov(float val){
		Diadrasis.Instance.kamera.fieldOfView = val; //30-60
		//save
		PlayerPrefs.SetFloat("cameraFieldOfView",val);		//Debug.LogWarning(val);
		PlayerPrefs.Save();
		moveSettings.cameraFieldOfView = val;
		//display
		kameraPanel.textFOV.text = Mathf.RoundToInt(val).ToString();

		//Debug.LogWarning("cameraFieldOfView = "+moveSettings.cameraFieldOfView);
	}

	void SetGyroHorizontalAngle(float val){
		//set new value
		moveSettings.cameraHorAngleOffset = val;
		//save
		PlayerPrefs.SetFloat("cameraHorAngleOffset",moveSettings.cameraHorAngleOffset);
		PlayerPrefs.Save();
		//display
		kameraPanel.textHorzAngle.text = Mathf.RoundToInt(val).ToString();
	}

	void SetGyroVerticalAngle(float val){
		//set new value
		moveSettings.cameraVertAngleOffset = val;
		//save
		PlayerPrefs.SetFloat("cameraVertAngleOffset",moveSettings.cameraVertAngleOffset);
		PlayerPrefs.Save();
		//display
		kameraPanel.textVertAngle.text = Mathf.RoundToInt(val).ToString();
	}

//	public Button gyroBtnReset;
	void ResetGyroskopio(){		//Debug.Log ("ResetGyroskopio");

		if(movePersonGyro){
			movePersonGyro.ResetKamera();
		}

		XmlNodeList generalSettings =Globals.Instance.settingsXml.SelectNodes ("/settings/general");
		
		if (generalSettings.Count > 0) {
			foreach (XmlNode setting in generalSettings) {
				//camera settings
				if(PlayerPrefs.GetFloat("cameraFieldOfView")<=0){
					moveSettings.cameraFieldOfView = float.Parse (setting ["cameraFieldOfView"].InnerText);
					//save
					PlayerPrefs.SetFloat("cameraFieldOfView",moveSettings.cameraFieldOfView);
					PlayerPrefs.Save();
				}else{
					moveSettings.cameraFieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
				}

				if(PlayerPrefs.GetFloat("cameraHorAngleOffset")<=0){
					moveSettings.cameraHorAngleOffset = float.Parse (setting ["cameraHorAngleOffset"].InnerText);
				}else{
					moveSettings.cameraHorAngleOffset = PlayerPrefs.GetFloat("cameraHorAngleOffset");
				}

				if(PlayerPrefs.GetFloat("cameraVertAngleOffset")<=0){
					moveSettings.cameraVertAngleOffset = float.Parse (setting ["cameraVertAngleOffset"].InnerText);
				}else{
					moveSettings.cameraVertAngleOffset = PlayerPrefs.GetFloat("cameraVertAngleOffset");
				}
			}
			
		}

		//restore settings
		kameraPanel.fovSlider.value=moveSettings.cameraFieldOfView;
		SetFov(kameraPanel.fovSlider.value);

		kameraPanel.gyroHorzAngle.value=moveSettings.cameraHorAngleOffset;
		SetGyroHorizontalAngle(kameraPanel.gyroHorzAngle.value);

		kameraPanel.gyroVertAngle.value=moveSettings.cameraVertAngleOffset;
		SetGyroVerticalAngle(kameraPanel.gyroVertAngle.value);

	}

	//accel -> 
//	public Slider accelHorzAngle;
//	public Text acceltextHorzAngle;
	void SetAccel_HorizontalAngle(float val){
		moveSettings.camAccelRotX = val;
		PlayerPrefs.SetFloat("camAccelRotX",moveSettings.camAccelRotX);
		PlayerPrefs.Save();
		kameraPanel.acceltextHorzAngle.text = Mathf.RoundToInt(val).ToString();
		if(movePersonAccel){movePersonAccel.SetKameraAngle_X();}
	}
	//accel -> 
//	public Slider accelVertAngle;
//	public Text acceltextVertAngle;
	void SetAccel_VerticalAngle(float val){
		moveSettings.camAccelRotY = val;
		PlayerPrefs.SetFloat("camAccelRotY",moveSettings.camAccelRotY);
		PlayerPrefs.Save();
		kameraPanel.acceltextVertAngle.text = Mathf.RoundToInt(val).ToString();
		if(movePersonAccel){movePersonAccel.SetKameraAngle_Y();}
	}
	
//	public Slider accelTurnSpeed_Y;
//	public Text acceltextTurnSpeed_Y;
	void SetCompass_TurnSpeed(float val){
		moveSettings.compassTurnSpeed = val;
		PlayerPrefs.SetFloat("compassTurnSpeed",moveSettings.compassTurnSpeed);
		PlayerPrefs.Save();
		kameraPanel.acceltextTurnSpeed_Y.text = Mathf.RoundToInt(val).ToString();
	}
	
//	public Slider accelSensitivity;
//	public Text acceltextSensitivity;
	void SetAccel_Sensitivity(float val){
		moveSettings.accelSensitivity = val/100f;
		PlayerPrefs.SetFloat("accelSensitivity",moveSettings.accelSensitivity);
		PlayerPrefs.Save();
		kameraPanel.acceltextSensitivity.text = Mathf.RoundToInt(val).ToString();
	}
	
//	public Button accelBtnReset;
	void ResetAccel(){
		if(accelControl){accelControl.ResetAccel();}

		XmlNodeList generalSettings =Globals.Instance.settingsXml.SelectNodes ("/settings/general");
		
		if (generalSettings.Count > 0) 
		{
			foreach(XmlNode setting in generalSettings)
			{
				//camera settings
				if(PlayerPrefs.GetFloat("cameraFieldOfView")<=0){
					moveSettings.cameraFieldOfView = float.Parse (setting ["cameraFieldOfView"].InnerText);
				}else{
					moveSettings.cameraFieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
				}

				if(PlayerPrefs.GetFloat("compassTurnSpeed")<=0){
					moveSettings.compassTurnSpeed = float.Parse (setting ["compassTurnSpeed"].InnerText);
				}else{
					moveSettings.compassTurnSpeed = PlayerPrefs.GetFloat("compassTurnSpeed");
				}

				if(PlayerPrefs.GetFloat("camAccelRotX")<=0){
					moveSettings.camAccelRotX = float.Parse (setting ["camAccelRotX"].InnerText);
				}else{
					moveSettings.camAccelRotX = PlayerPrefs.GetFloat("camAccelRotX");
				}

				if(PlayerPrefs.GetFloat("camAccelRotY")<=0){
					moveSettings.camAccelRotY = float.Parse (setting ["camAccelRotY"].InnerText);
				}else{
					moveSettings.camAccelRotY = PlayerPrefs.GetFloat("camAccelRotY");
				}

				if(PlayerPrefs.GetFloat("accelSensitivity")<=0){
					moveSettings.accelSensitivity = float.Parse (setting ["accelSensitivity"].InnerText);
				}else{
					moveSettings.accelSensitivity = PlayerPrefs.GetFloat("accelSensitivity");
				}
			}
			
		}
		
		//restore settings
		kameraPanel.fovSlider.value=moveSettings.cameraFieldOfView;
		SetFov(kameraPanel.fovSlider.value);

		//accel hor angle
		kameraPanel.accelHorzAngle.value=moveSettings.camAccelRotX;
		SetAccel_HorizontalAngle(kameraPanel.accelHorzAngle.value);

		//accel vert angle
		kameraPanel.accelVertAngle.value=moveSettings.camAccelRotY;
		SetAccel_VerticalAngle(kameraPanel.accelVertAngle.value);

		//accel sensitivity
		kameraPanel.accelSensitivity.value=moveSettings.accelSensitivity*100f;
		SetAccel_Sensitivity(kameraPanel.accelSensitivity.value);

		//accel compass turn speed
		kameraPanel.accelTurnSpeed_Y.value=moveSettings.compassTurnSpeed;
		SetCompass_TurnSpeed(kameraPanel.accelTurnSpeed_Y.value);
	}
	
	//joys
//	public Slider joyRightSensitivity;
//	public Text joystextSens;
	void SetJoyRight_Sensitivity(float val){
		moveSettings.joyRightSensitivity = val;
		PlayerPrefs.SetFloat("joyRightSensitivity",moveSettings.joyRightSensitivity );
		PlayerPrefs.Save();
		kameraPanel.joystextSens.text = Mathf.RoundToInt(val).ToString();
	}

//###############################################       GEOMETRY PANEL 		######################################################################################


	void SetSpeed(float val){
		moveSettings.groundMoveSpeed = val;
		//save
		PlayerPrefs.SetFloat("groundMoveSpeed",moveSettings.groundMoveSpeed);
		PlayerPrefs.Save();

		if(Diadrasis.Instance.cMotor){
			Diadrasis.Instance.cMotor.movement.maxForwardSpeed = val;
			Diadrasis.Instance.cMotor.movement.maxBackwardsSpeed = val;
			Diadrasis.Instance.cMotor.movement.maxSidewaysSpeed = val;
		}
		geometryPanel.textSpeed.text=Mathf.RoundToInt(moveSettings.groundMoveSpeed).ToString();
	}

	void SetMoveInPath(bool val){
		//moveSettings.snapToPath=val;
		//geometryPanel.snapDistance.transform.parent.gameObject.SetActive(val);
	}

	void SetMoveByMapFilter(bool val){
//		Diadrasis.Instance.useMapFilterForMovement = val;
//		if(!moveSettings.snapToPath){
//			moveSettings.snapToPath = true;
//			geometryPanel.snapPath.isOn= true;
//		}
	}

	void SetJoysticks(bool val){

		Diadrasis.Instance.menuUI.joy.dualJoys.SetActive(false);
		Diadrasis.Instance.menuUI.joy.singleJoyLeft.SetActive(false);
		Diadrasis.Instance.menuUI.joy.singleJoyRight.SetActive(false);

		if(!val)
		{
			if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
			{
				//add person with sensor if avalaible
				Diadrasis.Instance.CheckSensors();

			}
		}
		else
		{
			if(Diadrasis.Instance.sensorUsing!=Diadrasis.SensorUsing.joysticks)
			{
				//add person with joysticks only
				Diadrasis.Instance.sensorUsing=Diadrasis.SensorUsing.joysticks;
			}
		}

		#if UNITY_EDITOR
		Debug.LogWarning("do we need to Destroy person ??");
		#endif
		Diadrasis.Instance.AddPerson();
			
	}
	
	void SetMaxSnapToPath(float val){

		//save
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
			moveSettings.maxSnapPathDistOnsite = val; 
			//save
			PlayerPrefs.SetFloat("maxSnapPathDistOnsite",moveSettings.maxSnapPathDistOnsite);
			//display
			geometryPanel.textSnapDist.text=Mathf.RoundToInt(moveSettings.maxSnapPathDistOnsite).ToString();
		}else //moveSettings.maxSnapPathDistOffsite
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
			moveSettings.maxSnapPathDistOffsite = val;
			//save
			PlayerPrefs.SetFloat("maxSnapPathDistOffsite",moveSettings.maxSnapPathDistOffsite);
			//display
			geometryPanel.textSnapDist.text=Mathf.RoundToInt(moveSettings.maxSnapPathDistOffsite).ToString();
		}

		PlayerPrefs.Save();

	}

	void SetOffset_X(float val){
		moveSettings.gpsOffsetX = val; 
		PlayerPrefs.SetFloat("gpsOffsetX",moveSettings.gpsOffsetX);
		geometryPanel.textOffsetX.text=Mathf.RoundToInt(moveSettings.gpsOffsetX).ToString();
	}

	void SetOffset_Y(float val){
		moveSettings.gpsOffsetY = val; 
		PlayerPrefs.SetFloat("gpsOffsetY",moveSettings.gpsOffsetY);
		geometryPanel.textOffsetY.text=Mathf.RoundToInt(moveSettings.gpsOffsetY).ToString();
	}

//###############################################       LANGUANGE PANEL 		###################################################################################

//	public static List<Text> textToChangeFont = new List<Text>();

	void SetFontStyle(float val){
		if(Diadrasis.Instance.olaTaKeimena_Settings.Count>0){
			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Settings){
				if(t)
				{
					if(val==0){
						t.fontStyle=FontStyle.Normal;
					}else
					if(val==1){
						t.fontStyle=FontStyle.Bold;
					}
				}
			}
		}

//		if(Diadrasis.Instance.olaTaKeimena_PoiLabels.Count>0){
//			foreach(Text t in Diadrasis.Instance.olaTaKeimena_PoiLabels){
//				if(t)
//				{
//					if(val==0){
//						t.fontStyle=FontStyle.Normal;
//					}else
//					if(val==1){
//						t.fontStyle=FontStyle.Bold;
//					}
//				}
//			}
//		}
//
//		if(Diadrasis.Instance.olaTaKeimena_Info.Count>0){
//			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Info){
//				if(t)
//				{
//					if(val==0){
//						t.fontStyle=FontStyle.Normal;
//					}else
//					if(val==1){
//						t.fontStyle=FontStyle.Bold;
//					}
//				}
//			}
//		}
	}


	void SetLetterSpace(float val){
		foreach(Text t in Diadrasis.Instance.olaTaKeimena_Settings){
			if(t)
			{
				LetterSpacing lt = t.GetComponent<LetterSpacing>();
				if(!lt){lt = t.gameObject.AddComponent<LetterSpacing>();}
				ChangeSpace(lt,val);
			}
		}
	}

	void SetFontSize(float val){
		int newSize = Mathf.FloorToInt(val);

		if(Diadrasis.Instance.olaTaKeimena_Settings.Count>0){
			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Settings){
				if(t)
				{
					ResizeFont(t,newSize);
				}
			}
		}
		
		if(Diadrasis.Instance.olaTaKeimena_PoiLabels.Count>0){
			foreach(Text t in Diadrasis.Instance.olaTaKeimena_PoiLabels){
				if(t)
				{
					ResizeFont(t,newSize);
				}
			}
		}
		
		if(Diadrasis.Instance.olaTaKeimena_Info.Count>0){
			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Info){
				if(t)
				{
					if(t==Diadrasis.Instance.menuUI.infoUI.fullDescPanel.titleText){
						int x = newSize+2;
						ResizeFont(t,x);
					}else
					{
						ResizeFont(t,newSize);
					}
				}
			}
		}
	}

	void SetFontType(float val){
		int newFont= Mathf.FloorToInt(val);		//Debug.Log(newFont);

		if(Diadrasis.Instance.olaTaKeimena_Settings.Count>0){
			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Settings){
				if(t)
				{
					ChangeFontType(t,langPanel.fonts[newFont]);
				}
			}
		}
		
//		if(Diadrasis.Instance.olaTaKeimena_PoiLabels.Count>0){
//			foreach(Text t in Diadrasis.Instance.olaTaKeimena_PoiLabels){
//				if(t)
//				{
//					ChangeFontType(t,langPanel.fonts[newFont]);
//				}
//			}
//		}
//		
//		if(Diadrasis.Instance.olaTaKeimena_Info.Count>0){
//			foreach(Text t in Diadrasis.Instance.olaTaKeimena_Info){
//				if(t)
//				{
//					ChangeFontType(t,langPanel.fonts[newFont]);
//				}
//			}
//		}
	}

	public delegate void ActionLanguange();
	//lang change
	public event ActionLanguange OnLanguangeChange;


	public void SetLanguange(float val){		//Debug.Log("SetLanguange");
		if(val==0){
			Diadrasis.Instance.languangeNow=Diadrasis.LanguangeNow.gr;
			appSettings.language="gr";
		}else
		if(val==1){
			Diadrasis.Instance.languangeNow=Diadrasis.LanguangeNow.en;
			appSettings.language="en";
		}

		Invoke("ApplyLanguageChanges", 0.2f);
	}

	void ApplyLanguageChanges()
    {
		appData.Init();

		if (OnLanguangeChange != null)
		{
			OnLanguangeChange();
		}

#if UNITY_EDITOR
		Debug.LogWarning("this may cause an error - bug!!");
#endif

		if (Diadrasis.Instance.userPrin != Diadrasis.UserPrin.inMenu)
		{
			CreatePoints.InitPoints();
			Diadrasis.Instance.menuUI.LabelsSetLanguange();
		}
		else
		if (Diadrasis.Instance.userPrin == Diadrasis.UserPrin.inMenu)
		{
			Diadrasis.Instance.xartisMenu.GetXmlScenes();
		}

		if (Diadrasis.Instance.olaTaKeimena_Settings.Count > 0)
		{

			foreach (Text t in Diadrasis.Instance.olaTaKeimena_Settings)
			{
				if (t)
				{
					ReplaceText(t);
				}
			}
		}

		CancelInvoke();
	}

	void ReplaceText(Text t){
		//find text from terms xml with the name of transform if exists
		if(t)
		{
			if(appData.FindTerm_text(t.transform.name)!="unknown"){
				t.text = appData.FindTerm_text(t.transform.name);
			}
		}
	}

	void ResizeFont(Text t,int val){
		if(t)
		{
			t.fontSize = val;
		}
	}

	void ChangeFontType(Text t,Font f){
		if(t)
		{
			t.font=f;
		}
	}

	void ChangeSpace(LetterSpacing lt, float f){
		lt.spacing=f;
	}

	public void OnTapHide(CanvasGroup cg)
	{
		Diadrasis.Instance.animControl.animSettings.enabled=false;
		cg.alpha=0.3f;
		if(movePersonGyro){
			movePersonGyro.tempGyro=true;
		}
	}

	public int countToShow = 0;

	public void ResetCounter(){
		countToShow=0;
	}

	public void OnTapSecret(GameObject secretPanel)
	{
//		if(Diadrasis.Instance.userPrin==Diadrasis.UserPrin.inMenu){
			countToShow++;

			if(countToShow>=5){
				secretPanel.SetActive(true);
				countToShow=0;
			}
//		}

//		Debug.Log("VVVVV "+countToShow);
	}


	//EVENT TRIGGERS ON SLIDERS (MANUAL ON PREFAB)
	public void OnUnTapShow(CanvasGroup cg)
	{
		cg.alpha=1f;
		Diadrasis.Instance.animControl.animSettings.enabled=true;
		if(movePersonGyro){
			movePersonGyro.tempGyro=false;
		}
	}

	void SetMaxPoiDistLabel(float val){
		Diadrasis.Instance.menuUI.maxLabelDist = val;
		langPanel.txt_maxDistPoiValue.text=Mathf.RoundToInt(val).ToString();
	}

//	void SetGpsInitTime(float val){
////		Gps.Instance.gpsInitTime = val;
//		expertPanelDiadrasis.textTimeValue.text = Mathf.RoundToInt(val).ToString();
//
//		PlayerPrefs.SetFloat("gpsInitTime",Gps.Instance.gpsInitTime);
//
////		Debug.Log( PlayerPrefs.GetInt("gpsInitTime",moveSettings.gpsInitTime));
//	}
//
//	void SetGpsAccuracy(float val){
////		Gps.Instance.maxGpsAccuracy = val;
//		expertPanelDiadrasis.textAccuracyValue.text = Mathf.RoundToInt(val).ToString();
//
//		PlayerPrefs.SetFloat("maxGpsAccuracy", Gps.Instance.maxGpsAccuracy);
//	}
//
//	void EnableEpitopioMode(bool val){
//		Diadrasis.Instance.useOnSiteMode = val;
//	}
//
//	void SetExpert(){
////		expertPanelDiadrasis.initTimeSlider.value = Gps.Instance.maxGpsAccuracy;
//		expertPanelDiadrasis.textAccuracyValue.text = Mathf.RoundToInt(Gps.Instance.maxGpsAccuracy).ToString();
//		
////		expertPanelDiadrasis.accuracySlider.value = Gps.Instance.gpsInitTime;
//		expertPanelDiadrasis.textTimeValue.text = Gps.Instance.gpsInitTime.ToString();
//	}

}
