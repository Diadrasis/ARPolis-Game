// ***********************************************************
// Written by Heyworks Unity Studio http://unity.heyworks.com/
// ***********************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Gyroscope controller that works with any device orientation.
/// </summary>
public class GyroController : Singleton <GyroController> 
{

	private GyroController(){}
	
	private bool gyroEnabled = false;
	private const float lowPassFilterFactor = 0.2f;
	
	private readonly Quaternion baseIdentity =  Quaternion.Euler(90, 0, 0);
//	private readonly Quaternion landscapeRight =  Quaternion.Euler(0, 0, 90);
//	private readonly Quaternion landscapeLeft =  Quaternion.Euler(0, 0, -90);
//	private readonly Quaternion upsideDown =  Quaternion.Euler(0, 0, 180);
	
	private Quaternion cameraBase =  Quaternion.identity;
	private Quaternion calibration =  Quaternion.identity;
	private Quaternion baseOrientation =  Quaternion.Euler(90, 0, 0);
	private Quaternion baseOrientationRotationFix =  Quaternion.identity;
	
	private Quaternion referanceRotation = Quaternion.identity;
//	private bool debug = true;
	
	
	public Transform myKamera;
	
	public void Init(){
		Input.gyro.enabled=false;
		DetachGyro();
	}
	
	public void SetGyro(){
		if(gyroEnabled){
			DetachGyro();
		}else{
			AttachGyro();
		}
	}

	public Quaternion rotKameras;
	
	protected void Update() 
	{
		if (!gyroEnabled && !myKamera){
			return;
		}
		myKamera.localRotation = Quaternion.Slerp(myKamera.localRotation, cameraBase * ( ConvertRotation(referanceRotation * Input.gyro.attitude) * GetRotFix()), lowPassFilterFactor);
	}

//	public void SetRot(Transform kamera){
//		//get person rotation
//		rotKameras = kamera.rotation;
//	}
	
	
	/// <summary>
	/// Attaches gyro controller to the transform.
	/// </summary>
	public void AttachGyro()
	{
		myKamera = Camera.main.transform;
		gyroEnabled = true;
		ResetBaseOrientation();
		UpdateCalibration(true);
		UpdateCameraBaseRotation(true);
		RecalculateReferenceRotation();
	}
	
	/// <summary>
	/// Detaches gyro controller from the transform
	/// </summary>
	public void DetachGyro()
	{
		myKamera = null;
		gyroEnabled = false;
	}
	
	
	/// <summary>
	/// Update the gyro calibration.
	/// </summary>
	private void UpdateCalibration(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = (Input.gyro.attitude) * (-Vector3.forward);
			fw.z = 0;
			if (fw == Vector3.zero)
			{
				calibration = Quaternion.identity;
			}
			else
			{
				calibration = (Quaternion.FromToRotation(baseOrientationRotationFix * Vector3.up, fw));
			}
		}
		else
		{
			calibration = Input.gyro.attitude;
		}
	}
	
	/// <summary>
	/// Update the camera base rotation.
	/// </summary>
	/// <param name='onlyHorizontal'>
	/// Only y rotation.
	/// </param>
	private void UpdateCameraBaseRotation(bool onlyHorizontal)
	{
		if (onlyHorizontal)
		{
			var fw = transform.forward;
			fw.y = 0;
			if (fw == Vector3.zero)
			{
				cameraBase = Quaternion.identity;
			}
			else
			{
				cameraBase = Quaternion.FromToRotation(Vector3.forward, fw);
			}
		}
		else
		{
			cameraBase = transform.rotation;
		}
	}
	
	/// <summary>
	/// Converts the rotation from right handed to left handed.
	/// </summary>
	/// <returns>
	/// The result rotation.
	/// </returns>
	/// <param name='q'>
	/// The rotation to convert.
	/// </param>
	private static Quaternion ConvertRotation(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);	
	}
	
	/// <summary>
	/// Gets the rot fix for different orientations.
	/// </summary>
	private Quaternion GetRotFix()
	{
		return Quaternion.identity;
	}
	
	/// <summary>
	/// Recalculates reference system.
	/// </summary>
	private void ResetBaseOrientation()
	{
		baseOrientationRotationFix = GetRotFix();
		baseOrientation = baseOrientationRotationFix * baseIdentity;
	}
	
	/// <summary>
	/// Recalculates reference rotation.
	/// </summary>
	private void RecalculateReferenceRotation()
	{
		referanceRotation = Quaternion.Inverse(baseOrientation)*Quaternion.Inverse(calibration);
	}
	
}
