using UnityEngine;
using System.Collections;

public class UI_Anchors : MonoBehaviour {

	public Canvas kanvas;
	public RectTransform rekt;

	void Start () {

	}
	
	void Update () {

		if(Input.GetKeyDown(KeyCode.Keypad0))
		{
			UItool.SetAnchor(rekt,UItool.Thesi.centerDown);		Debug.Log("SetAnchor");
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad1))
		{
			UItool.SetAnchor(rekt,UItool.Thesi.center);	
			Vector2 scale = rekt.sizeDelta;
			UItool.SetPivot(rekt,UItool.Thesi.center);		Debug.Log("SetPivot");
			rekt.localPosition=Vector3.zero;
			rekt.sizeDelta=scale;
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad2))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad3))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad4))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad5))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad6))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad7))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad8))
		{
			
		}
		else
			if(Input.GetKeyDown(KeyCode.Keypad9))
		{
			
		}
	
	}





	void Turn(float angle){
//		GetComponent<RectTransform> ().offsetMax = Vector2.Lerp (oldPositionMax, newPositionMax, percentage);
//		GetComponent<RectTransform> ().offsetMin = Vector2.Lerp (oldPositionMin, newPositionMin, percentage);
//		GetComponent<RectTransform> ().rotation.eulerAngles.z =  Mathf.Lerp (0f, 90f, percentage);

	}


}
