using UnityEngine;
using System.Collections;
using CnControls;
using System.Collections.Generic;
using eChrono;

public class MovePerson_Accel : MovePerson {

	void OnEnable () {
		myTransform=GetComponent<Transform>();
		cmotor = GetComponent<CharacterMotorC> ();
		if(cmotor){
			Diadrasis.Instance.cMotor = cmotor;
		}

		if(!myCamera){
			myCamera=transform.FindChild("kamera");
		}

		SetKameraAngle_X();

		Diadrasis.Instance.kamera = myCamera.GetComponent<Camera>();
		if(PlayerPrefs.GetFloat("cameraFieldOfView")>0){
			myCamera.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
		}

	}

	public void SetKameraAngle_X()
	{
		if(myCamera)
		{
			Vector3 rot = myCamera.localEulerAngles;
			rot.x = moveSettings.camAccelRotX;
			myCamera.localEulerAngles = rot;
		}
	}

	public void SetKameraAngle_Y()
	{
		if(myCamera)
		{
			Vector3 rot = myCamera.localEulerAngles;
			rot.y = moveSettings.camAccelRotY;
			myCamera.localEulerAngles = rot;
		}
	}

	
	void LateUpdate(){
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){

			if(Diadrasis.Instance.user==Diadrasis.User.isNavigating )
			{
				JoyMove();
			}
			else if(Diadrasis.Instance.user==Diadrasis.User.onAir)
			{
				if(Diadrasis.Instance.moveOnAir)
				{
					JoyMove();
				}
			}
		}
		else
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite)
		{
			if((Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user==Diadrasis.User.inFullMap) || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
			{
				GpsMove();
			}
		}
	}


	
}
