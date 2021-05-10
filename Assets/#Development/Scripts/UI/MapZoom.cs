using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean;
using Stathis;

public class MapZoom : MonoBehaviour {

	RectTransform myRect;
	public float defaultSize=1000f;
	public float dragSpeed = 0.3f;
	public float minMaxPos=500f;
//	SimpleOrthographicZoom kameraZoomScript;

	void OnEnable () {
		if(!myRect){
			myRect = GetComponent<RectTransform>();
		}

//		if(!kameraZoomScript){
//			kameraZoomScript=Camera.main.GetComponent<SimpleOrthographicZoom>();
//		}
	}
	
	void LateUpdate ()
	{
		if(Diadrasis.Instance.user==Diadrasis.User.inMenu )
		{
			if(LeanTouch.Fingers.Count>1)
			{
				if(!Tools_UI.IsPointerOverUIObject("btnReturn")){
					Tools_UI.ScaleSizeDelta(myRect, LeanTouch.PinchScale, new Vector2(defaultSize, 3000f));
				}

			}else
			if(LeanTouch.Fingers.Count==1){

				if(!Tools_UI.IsPointerOverUIObject("btnReturn")){
					// This will move the current transform based on a finger drag gesture
					LeanTouch.MoveObject(myRect, LeanTouch.DragDelta * dragSpeed);
				}
			}


			//move map mesa sta oria
//			minMaxPos = ((60.01f - Camera.main.orthographicSize) * 20f);// + 500f;

			minMaxPos = ( myRect.sizeDelta.x - defaultSize ) * 0.5f;

			dragSpeed = (minMaxPos/1000f ) + 0.3f;

			MoveMap();

		}


	}

	void MoveMap(){
		Vector3 pos = myRect.localPosition;
		
		pos.x = Mathf.Clamp(pos.x, -minMaxPos, minMaxPos);
		pos.y = Mathf.Clamp(pos.y, -minMaxPos, minMaxPos);
		pos.z = 0f;

		if(Diadrasis.Instance.menuStatus!=Diadrasis.MenuStatus.periodView)
		{
			myRect.localPosition = Vector3.Lerp(myRect.localPosition, pos, Time.deltaTime * 2f);
//			myRect.localPosition = pos;
		}
//		else{
//			myRect.localPosition = Vector3.Lerp(myRect.localPosition, pos, Time.deltaTime);
//		}
	}
}
