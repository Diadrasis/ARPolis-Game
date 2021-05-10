using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Secret : MonoBehaviour {

	public GameObject page1,page2,page3;
	public Button btnP1,btnP2,btn3;
	public Text infoTextLeft;
	public Text infoTextRight;

	public Text latitudeText, longitudeText;

	public Slider initTimeSlider;
	public Text textTimeValue;
	public Slider accuracySlider;
	public Text textAccuracyValue;
	public Toggle enableOnSiteWalk;

	public Toggle showRealAccuracy;
	public Text realAccuracyText;

	public Slider startingAccuracySlider;
	public Text textStartingAccuracy;

	public Slider waitingAccuracySlider;
	public Text textWaitingAccuracy;

	public Slider updateDistanceSlider;
	public Text textUpdateDistance;

	public Slider checkGpsRepeatTimeSlider;
	public Text textcheckGpsRepeatTime;

	public Toggle stopLocationOnFailedToggle;

	public Toggle diadrasisSceneToggle;

	public int eisodoi;


	void Init () {

//		showRealAccuracy.isOn = false;

		btnP1.onClick.AddListener(()=>ShowPage(page1));
		btnP2.onClick.AddListener(()=>ShowPage(page2));
		btn3.onClick.AddListener(()=>ShowPage(page3));

		diadrasisSceneToggle.onValueChanged.AddListener((b)=>DiadrasisSecretScene(diadrasisSceneToggle.isOn));
		diadrasisSceneToggle.isOn = Diadrasis.Instance.enableDiadrasisScene;

		enableOnSiteWalk.onValueChanged.AddListener((b)=>EnableEpitopioMode(enableOnSiteWalk.isOn));
		enableOnSiteWalk.isOn = Diadrasis.Instance.useOnSiteMode;

		stopLocationOnFailedToggle.onValueChanged.AddListener((b)=>StopOnFailed(stopLocationOnFailedToggle.isOn));
		stopLocationOnFailedToggle.isOn = Gps.Instance.stopLocationOnFailed;
		
		initTimeSlider.minValue=10f;
		initTimeSlider.maxValue=60f;
		initTimeSlider.wholeNumbers=true;
		initTimeSlider.onValueChanged.AddListener((b)=>SetGpsInitTime(initTimeSlider.value));

		checkGpsRepeatTimeSlider.minValue=0.1f;
		checkGpsRepeatTimeSlider.maxValue=1f;
		checkGpsRepeatTimeSlider.wholeNumbers=false;
		checkGpsRepeatTimeSlider.onValueChanged.AddListener((b)=>SetCheckRepeatTime(checkGpsRepeatTimeSlider.value));

		waitingAccuracySlider.minValue=1f;
		waitingAccuracySlider.maxValue=15f;
		waitingAccuracySlider.wholeNumbers=true;
		waitingAccuracySlider.onValueChanged.AddListener((b)=>SetWaitingAccuracyTimer(waitingAccuracySlider.value));

		accuracySlider.minValue=3f;
		accuracySlider.maxValue=20f;
		accuracySlider.wholeNumbers=true;
		accuracySlider.onValueChanged.AddListener((b)=>SetGpsAccuracy(accuracySlider.value));

		startingAccuracySlider.minValue=5f;
		startingAccuracySlider.maxValue=50f;
		startingAccuracySlider.wholeNumbers=true;
		startingAccuracySlider.onValueChanged.AddListener((b)=>SetStartingAccuracy(startingAccuracySlider.value));

		updateDistanceSlider.minValue=0.1f;
		updateDistanceSlider.maxValue=5f;
		updateDistanceSlider.wholeNumbers=false;
		updateDistanceSlider.onValueChanged.AddListener((b)=>SetUpdateDistance(updateDistanceSlider.value));

		infoTextLeft.text = "<i><color=grey>deviceModel = </color></i>"+SystemInfo.deviceModel+
			"\n <i><color=grey>Device Type = </color></i>"+SystemInfo.deviceType+
				"\n <i><color=grey>OS = </color></i>"+SystemInfo.operatingSystem+
				"\n <i><color=grey>System Memory Size = </color></i>"+SystemInfo.systemMemorySize+
				"\n <i><color=grey>Processor Count = </color></i>"+SystemInfo.processorCount+
				"\n <i><color=grey>Processor Type = </color></i>"+SystemInfo.processorType+
				"\n <i><color=grey>Graphics Device Name = </color></i>"+SystemInfo.graphicsDeviceName+
				"\n <i><color=grey>Graphics Device Version = </color></i>"+SystemInfo.graphicsDeviceVersion+
				"\n <i><color=grey>Graphics Memory Size = </color></i>"+SystemInfo.graphicsMemorySize+
				"\n <i><color=grey>Graphics Pixel Fillrate = </color></i>"+SystemInfo.graphicsPixelFillrate+
				"\n <i><color=grey>Graphics Shader Level = </color></i>"+SystemInfo.graphicsShaderLevel+
				"\n <i><color=grey>Max Texture Size = </color></i>"+SystemInfo.maxTextureSize;


		infoTextRight.text = " <i><color=grey>Npot Support = </color></i>"+SystemInfo.npotSupport+
			"\n <i><color=grey>Supports Compute Shaders = </color></i>"+SystemInfo.supportsComputeShaders+
				"\n <i><color=grey>Supports 3D Textures = </color></i>"+SystemInfo.supports3DTextures+
				"\n <i><color=grey>Supported Render Target Count = </color></i>"+SystemInfo.supportedRenderTargetCount+
				"\n <i><color=grey>Supports Image Effects = </color></i>"+SystemInfo.supportsImageEffects+
				"\n <i><color=grey>Supports Render Textures = </color></i>"+SystemInfo.supportsRenderTextures+
				"\n <i><color=grey>Supports Render To Cubemap = </color></i>"+SystemInfo.supportsRenderToCubemap+
				"\n <i><color=grey>Supports Shadows = </color></i>"+SystemInfo.supportsShadows+
				"\n <i><color=grey>Supports Sparse Textures = </color></i>"+SystemInfo.supportsSparseTextures+
				"\n <i><color=grey>Supports Stencil = </color></i>"+SystemInfo.supportsStencil;

		
	}

	void ShowPage(GameObject gb){
		if(gb==page1){
			btnP1.image.color=Color.blue;
			btnP2.image.color = Color.gray;
			btn3.image.color = Color.gray;
			page1.SetActive(true);
			page2.SetActive(false);
			page3.SetActive(false);
		}else
		if(gb==page2){
			btnP1.image.color=Color.gray;
			btnP2.image.color = Color.blue;
			btn3.image.color = Color.gray;
			page1.SetActive(false);
			page2.SetActive(true);
			page3.SetActive(false);
		}else{
			btnP1.image.color=Color.gray;
			btnP2.image.color = Color.gray;
			btn3.image.color = Color.blue;
			page1.SetActive(false);
			page2.SetActive(false);
			page3.SetActive(true);
		}
	}

	void StopOnFailed(bool val){
		Gps.Instance.stopLocationOnFailed = val;
	}

	void SetUpdateDistance(float val){
		float valRounded = Mathf.Round(val * 10f)/10f;
		PlayerPrefs.SetFloat("locUpdateDistance", valRounded);
		PlayerPrefs.Save();
//		Gps.Instance.locUpdateDistance = Mathf.Round(val * 10f)/10f;
		textUpdateDistance.text = valRounded.ToString("F1");
	}

	void SetStartingAccuracy(float val){
		PlayerPrefs.SetFloat("locStartingAccuracy",val);
		PlayerPrefs.Save();
//		Gps.Instance.locStartingAccuracy = val;
		textStartingAccuracy.text = Mathf.RoundToInt(val).ToString();
		
	}

	void SetCheckRepeatTime(float val){
		float valRounded =  Mathf.Round(val * 10f)/10f;
		PlayerPrefs.SetFloat("checkRepeatTime", valRounded);
		PlayerPrefs.Save();
//		Gps.Instance.checkRepeatTime = Mathf.Round(val * 10f)/10f;
		textcheckGpsRepeatTime.text = valRounded.ToString("F1");
		
	}

	void OnEnable () {
		if (eisodoi == 0) {
			Init();
		}

		ShowPage(page1);

		eisodoi++;

		SetExpert ();
	}



	 public void Close(){
//		PlayerPrefs.SetFloat("maxAccuracy", Gps.Instance.maxGpsAccuracy);
//		PlayerPrefs.SetFloat("initTime",Gps.Instance.gpsInitTime);
//		PlayerPrefs.SetFloat("locUpdateDistance", Gps.Instance.locUpdateDistance);
//		PlayerPrefs.SetFloat("locStartingAccuracy",Gps.Instance.locStartingAccuracy);
//		PlayerPrefs.SetFloat("gpsICheckAccuracyTime",Gps.Instance.gpsTimeForAccuracyCheck);


		//stop invoke and run again gps check
		if (PlayerPrefs.GetFloat ("checkRepeatTime") != Gps.Instance.checkRepeatTime) {
//			PlayerPrefs.SetFloat("checkRepeatTime",Gps.Instance.checkRepeatTime);
			Gps.Instance.SetRepeatingAgain();
		}

		gameObject.SetActive (false);
	}

	void EnableEpitopioMode(bool val){
		Diadrasis.Instance.useOnSiteMode = val;
		Diadrasis.Instance.showPoiInfo = val;
	}

	void DiadrasisSecretScene(bool val){
		Diadrasis.Instance.enableDiadrasisScene = val;
	}

	void SetExpert(){
		//do we want on site mode enabled (for free version which is false by default)
		enableOnSiteWalk.isOn = Diadrasis.Instance.useOnSiteMode;

		//if init has failed should we stop location sevice?
		stopLocationOnFailedToggle.isOn = Gps.Instance.stopLocationOnFailed;

		//set the max accuracy 
		accuracySlider.value = PlayerPrefs.GetFloat ("maxAccuracy");
		textAccuracyValue.text = Mathf.RoundToInt(accuracySlider.value).ToString();

		//set the time that gps need to initialize
		initTimeSlider.value = PlayerPrefs.GetFloat ("initTime");
		textTimeValue.text = initTimeSlider.value.ToString();

		//set the time to wait for accuracy check after position found
		waitingAccuracySlider.value = PlayerPrefs.GetFloat ("gpsICheckAccuracyTime");
		textWaitingAccuracy.text = waitingAccuracySlider.value.ToString();

		//how many meters needs user to move to get new position
		updateDistanceSlider.value = PlayerPrefs.GetFloat ("locUpdateDistance");
		textUpdateDistance.text = updateDistanceSlider.value.ToString();

		//what is the preferred starting accuracy
		startingAccuracySlider.value = PlayerPrefs.GetFloat ("locStartingAccuracy");
		textStartingAccuracy.text = startingAccuracySlider.value.ToString();

		//how often should check for position (min = 0.1")
		checkGpsRepeatTimeSlider.value = PlayerPrefs.GetFloat ("checkRepeatTime");
		textcheckGpsRepeatTime.text = checkGpsRepeatTimeSlider.value.ToString();
	}

	void SetWaitingAccuracyTimer(float val){
		PlayerPrefs.SetFloat("gpsICheckAccuracyTime",val);
//		Gps.Instance.gpsTimeForAccuracyCheck = val;
		textWaitingAccuracy.text = val.ToString ();
	}

	void SetGpsInitTime(float val){
		PlayerPrefs.SetFloat("initTime",val);
		PlayerPrefs.Save();
//		Gps.Instance.gpsInitTime = val;
		textTimeValue.text = Mathf.RoundToInt(val).ToString();
		
	}
	
	void SetGpsAccuracy(float val){
		PlayerPrefs.SetFloat("maxAccuracy", val);
		PlayerPrefs.Save();
//		Gps.Instance.maxGpsAccuracy = val;
		textAccuracyValue.text = Mathf.RoundToInt(val).ToString();
	}

	void LateUpdate () {

		if (Diadrasis.Instance.user != Diadrasis.User.inSettings) {
			Close();
		}
		if (latitudeText.text != Gps.Instance.GetMyLocation ().x.ToString ()) {
			latitudeText.text = Gps.Instance.GetMyLocation ().x.ToString ();
		}

		if (longitudeText.text != Gps.Instance.GetMyLocation ().y.ToString ()) {
			longitudeText.text = Gps.Instance.GetMyLocation ().y.ToString ();
		}

		if (showRealAccuracy.isOn) {

			if(!realAccuracyText.gameObject.activeSelf){
				realAccuracyText.gameObject.SetActive(true);
			}

			if (SystemInfo.supportsLocationService) {
				if (Gps.Instance.isEnabled ()) {
					realAccuracyText.text = Input.location.lastData.horizontalAccuracy.ToString ("F2");
				} 
			} else {
				if (realAccuracyText.text != "No Data") {
					realAccuracyText.text = "No Data";
				}
			}
		} else {
			if(realAccuracyText.gameObject.activeSelf){
				realAccuracyText.gameObject.SetActive(false);
			}
		}
	}
}
