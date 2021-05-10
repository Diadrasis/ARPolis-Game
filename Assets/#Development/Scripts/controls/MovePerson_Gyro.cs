using UnityEngine;
using System.Collections;
using CnControls;
using System.Collections.Generic;
using eChrono;

public class MovePerson_Gyro : MovePerson {


	void OnEnable () {
		myTransform=GetComponent<Transform>();
		cmotor = GetComponent<CharacterMotorC> ();
		if(cmotor){
			Diadrasis.Instance.cMotor = cmotor;
		}

		if(!myCamera){
			myCamera=transform.GetChild(0).GetComponent<Transform>();
		}

		Diadrasis.Instance.kamera = myCamera.GetComponent<Camera>();
		if(PlayerPrefs.GetFloat("cameraFieldOfView")>0){
			myCamera.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
		}

		ResetGyro();

		if(myGyro==null){
			myGyro=Input.gyro;
		}
		if(!myGyro.enabled){
			myGyro.enabled=true;
		}
	}


	public bool tempGyro=false;
	
	void Update () {
		//check if is not reading
		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user==Diadrasis.User.inFullMap || Diadrasis.Instance.user==Diadrasis.User.onAir || tempGyro || Diadrasis.Instance.user==Diadrasis.User.inAsanser)
		{
			Gyroskopio();
		}
	}


	void LateUpdate(){

		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite)
		{
			if(Diadrasis.Instance.user==Diadrasis.User.isNavigating )
			{
				JoyMove();
			}
			else if(Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir)
			{
				JoyMove();
			}
		}
		else
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
		{
				if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user==Diadrasis.User.inFullMap || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
			{
				GpsMove();
			}
		}
	}

}
