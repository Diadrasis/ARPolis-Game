using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InfoScreen : MonoBehaviour {

	public List<Transform> targets = new List<Transform>();
	public List<RectTransform> imgs = new List<RectTransform>();

	Camera kamera;
	float screenX;
	
	void Start() {
		kamera = GetComponent<Camera>();
		screenX=Screen.width;	Debug.Log(screenX);
	}
	
	void Update() {

		Oriothetisi();

	}


	void Oriothetisi(){
		for(int i=0; i<targets.Count; i++)
		{
			if(Vector3.Distance(targets[i].position,kamera.transform.position)<10f)
			{
				Vector3 screenPos = kamera.WorldToScreenPoint(targets[i].position);
				//		Debug.Log("target is " + screenPos.x + " pixels from the left");
				Vector3 fwd=kamera.transform.TransformDirection(Vector3.forward);
				Vector3 bkw=targets[i].position-kamera.transform.position;
				
		//		Debug.Log("A = "+Vector3.Angle(bkw,fwd));
				
				if(Vector3.Angle(bkw,fwd)<45f){
					imgs[i].position = screenPos;
					imgs[i].gameObject.SetActive(true);
					targets[i].gameObject.SetActive(true);
				}else{
					imgs[i].gameObject.SetActive(false);
					targets[i].gameObject.SetActive(kamera.gameObject);
				}
			}else{
				imgs[i].gameObject.SetActive(false);
			}
		}
		
//		if(screenPos.x>screenX || screenPos.x<0f){
//			t.gameObject.SetActive(false);
//		}else{
//			t.gameObject.SetActive(true);
//		}
	}

}
