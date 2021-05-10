using UnityEngine;
using System.Collections;
using Lean;

public class MovePerson_Joys : MovePerson {

	void OnEnable () {
		myTransform=GetComponent<Transform>();
		cmotor = GetComponent<CharacterMotorC> ();
		if(cmotor){
			Diadrasis.Instance.cMotor = cmotor;
		}

		if(!myCamera){
			myCamera=transform.FindChild("kamera");
		}

		Lean.LeanTouch.OnFingerDrag     += OnFingerDrag;
		Lean.LeanTouch.OnFingerDown     += OnFingerDown;
		Lean.LeanTouch.OnFingerUp       += OnFingerUp;
		Lean.LeanTouch.OnFingerTap 		+= OnFingerTap;

		Diadrasis.Instance.kamera = myCamera.GetComponent<Camera>();
		if(PlayerPrefs.GetFloat("cameraFieldOfView")>0){
			myCamera.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
		}

	}
	
	void OnDisable(){
		Lean.LeanTouch.OnFingerDrag     -= OnFingerDrag;
		Lean.LeanTouch.OnFingerDown     -= OnFingerDown;
		Lean.LeanTouch.OnFingerUp       -= OnFingerUp;
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

	public void OnFingerTap(Lean.LeanFinger finger)
	{
//		if (!Stathis.Tools_UI.IsPointerOverUIObject ()) {
//			canJump = true;
//			Invoke ("StopJump", 0.15f);
//		}
	}

	void StopJump(){
		canJump = false;
		CancelInvoke ();
	}


	bool canRotateCam;

	public void OnFingerDown(Lean.LeanFinger finger)
	{
		#if UNITY_EDITOR
		Debug.LogWarning("IsPointerOverUIObject MOVEPERSON JOYS");
		#endif
		if(!Stathis.Tools_UI.IsPointerOverUIObject()){
			if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user == Diadrasis.User.onAir || Diadrasis.Instance.user==Diadrasis.User.inAsanser){
				canRotateCam=true;
			}
		}else{
			canRotateCam=false;
		}
	}
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		//		Debug.Log("Finger " + finger.Index + " finished touching the screen");
		canRotateCam=false;
	}


	Vector2 pixels;
	
	public void OnFingerDrag(Lean.LeanFinger finger)
	{
		//		Debug.Log("Finger " + finger.Index + " moved " + finger.DeltaScreenPosition + " pixels across the screen");
		if(canRotateCam)
		{
			pixels = finger.DeltaScreenPosition;
			
			Vector3 rot = myCamera.eulerAngles;

			rot.y += pixels.x * moveSettings.joyRightSensitivity * Time.deltaTime;

			rot.x -= pixels.y * moveSettings.joyRightSensitivity * Time.deltaTime;

			if(rot.x>=80f && rot.x<290){rot.x=80f;}
			if(rot.x<=300 && rot.x>85f){rot.x=300;}

			rot.z = 0f;
			myCamera.eulerAngles = rot;
		}
	}



}
