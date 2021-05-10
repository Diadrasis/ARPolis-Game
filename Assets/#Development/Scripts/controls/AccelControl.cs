using UnityEngine;
using System.Collections;

public class AccelControl : Singleton<AccelControl> {

	private AccelControl(){}

	public float turnSpeed  = 10f;
	public float maxTurnLean = 50f;
	public float maxTilt = 50f;
	
	public float sensitivity = 0.1f;

	Vector3 euler = Vector3.zero;

	Transform myTransform;


	public void OnEnable(){
		if(!myTransform){
			myTransform = GetComponent<Transform>();
		}
		//reset accel
		CalibrateAccelerometer();
	}

	public void ResetAccel()
	{
		CalibrateAccelerometer();
	}

	private Quaternion calibrationQuaternion;

	void CalibrateAccelerometer()
	{
		Vector3 accelerationSnapshot = Input.acceleration;
		
		Quaternion rotateQuaternion = Quaternion.FromToRotation(
			new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
		
		calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
	}

	public void OnDisable(){

	}

		
	void FixedUpdate () {
		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user==Diadrasis.User.onAir || Diadrasis.Instance.user==Diadrasis.User.inAsanser)
		{
			if(myTransform != null)
			{
				Vector3 accelerator  = Input.acceleration;
				Vector3 fixedAcceleration = calibrationQuaternion * accelerator;
				
				// Rotate turn based on acceleration		
				euler.y += fixedAcceleration.x * turnSpeed;
				// Since we set absolute lean position, do some extra smoothing on it
				euler.z = Mathf.Lerp(euler.z, -fixedAcceleration.x * maxTurnLean, 0.2f);
				
				// Since we set absolute lean position, do some extra smoothing on it
				euler.x = Mathf.Lerp(euler.x, fixedAcceleration.y * maxTilt, 0.2f);
				
				// Apply rotation and apply some smoothing
				Quaternion rot  = Quaternion.Euler(euler);

				myTransform.localRotation = Quaternion.Lerp (myTransform.localRotation, rot, moveSettings.accelSensitivity);	//Debug.Log(moveSettings.accelSensitivity);
			}
		}
	}

}
