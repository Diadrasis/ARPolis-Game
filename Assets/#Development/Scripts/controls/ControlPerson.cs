using UnityEngine;
using System.Collections;

public class ControlPerson : MonoBehaviour {

	MenuUI menuUI;

	void OnEnable(){

		//get menu script from 2d canvas
		if(!menuUI){
			menuUI =  Diadrasis.Instance.menuUI;
		}

		gpsPosition.firstEntrance=true;

		//hide if any joy has left on from prev scene
		menuUI.HideAllJoys();

		#if UNITY_EDITOR
		Debug.LogWarning("if onsite with camera maybe dont show map?");
		#endif

		//enable map
		menuUI.xartis.targets.person=GetComponent<Transform>();
		menuUI.xartis.targets.kamera=Diadrasis.Instance.kamera.transform;
		menuUI.xartis.enabled=true;

		CreatePoints.XmlPointsTagName = Diadrasis.Instance.XmlPointsTagName;
		CreatePoints.InitPoints();

//		menuUI.xartis.Init();

		if(Diadrasis.Instance.sensorUsing!=Diadrasis.SensorUsing.joysticks)
		{
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
				//add one joystick for move
				menuUI.joy.singleJoyLeft.SetActive(true);
			}else{
				menuUI.joy.singleJoyLeft.SetActive(false);
			}
		}
		else
		if(Diadrasis.Instance.sensorUsing==Diadrasis.SensorUsing.joysticks)
		{
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
				//add 2 joysticks (left) move - (right) camera rotation
				menuUI.joy.dualJoys.SetActive(true);
			}else{
				//add 1 joystick for camera rot and moves with gps
				menuUI.joy.singleJoyRight.SetActive(true);
			}
		}


		//Application.loadedLevelName;
	}

	void OnDisable(){
		if(menuUI.xartis)
		{
			menuUI.xartis.enabled=false;
			menuUI.joy.dualJoys.SetActive(false);
			menuUI.joy.singleJoyLeft.SetActive(false);
		}
	}
}
