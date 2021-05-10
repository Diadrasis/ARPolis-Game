using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

	public float turnSpeed = 1f;
	public Vector3 accel;
	public float limitationRotX=0.3f;
	Transform myTransform;

	void Start () {
		Input.compass.enabled=true;
		Input.location.Start ();
		myTransform=GetComponent<Transform>();
	}

	public void OnEnable(){
		if(!Input.compass.enabled){
			Input.compass.enabled=true;
		}
		Input.location.Start();
		if(!myTransform)
		{
			myTransform=GetComponent<Transform>();
		}
	}
	
	public void OnDisable(){
		Input.compass.enabled=false;
//		Input.location.Stop();
	}

	
	void FixedUpdate() {
		accel = Input.acceleration;
		if(Diadrasis.Instance.user==Diadrasis.User.isNavigating || Diadrasis.Instance.user==Diadrasis.User.onAir || Diadrasis.Instance.user==Diadrasis.User.inAsanser)
		{
//			if(accel.x>-limitationRotX && accel.x<limitationRotX)//για να μην μετακινειται με την περιστροφη του accelerometer στον αξονα x
//			{
//				if(accel.z>0.5f)
//				{
//					Debug.Log("PanoLook");
//					myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation, Quaternion.Euler(0,-Mathf.Floor(Input.compass.trueHeading), 0),Time.deltaTime * moveSettings.compassTurnSpeed);
//				}
//				else
//				if(accel.z<=-0.5f)
				if(accel.z<0.0f)//bigger value drives to 180 rotation
				{
//					Debug.Log("KatoLook");
					myTransform.localRotation = Quaternion.Lerp(myTransform.localRotation, Quaternion.Euler(0,Mathf.Floor(Input.compass.trueHeading), 0),Time.deltaTime * moveSettings.compassTurnSpeed);
				}
//			}
		}

	}


}
