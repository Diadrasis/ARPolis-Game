using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// special functions to avoid using animations
/// </summary>
public class UItool : MonoBehaviour {

//	_try obj=_try.blah;

	void Start () {
//			SetPivot((int)obj);
	}
	
//	void Update () {
//		if(Input.GetMouseButtonDown(0))
//		{
//			SetPivot(3);
//		}
//	}

	public enum Thesi{center, centerUp, centerDown, left, leftUp, leftDown, right, rightUp, rightDown}
	public static Thesi thesi = Thesi.center;


	public static void SetAnchor(RectTransform rekt, Thesi thesi)
	{
		if(thesi==Thesi.center)
		{
			rekt.anchorMin = new Vector2(0.5f, 0.5f);
			rekt.anchorMax = new Vector2(0.5f, 0.5f);
		}
		else
		if(thesi==Thesi.centerDown)
		{
			rekt.anchorMin = new Vector2(0.5f,0f);
			rekt.anchorMax = new Vector2(0.5f,0f);
		}
	}

	public static void SetPivot(RectTransform rekt, Thesi thesi)
	{
		if(thesi==Thesi.center)
		{
			rekt.anchorMin = new Vector2(0.5f,0.5f);
		}
		else
		if(thesi==Thesi.centerDown)
		{
			rekt.anchorMin = new Vector2(0.5f,0f);
		}
		else
		if(thesi==Thesi.centerUp)
		{
			rekt.anchorMin = new Vector2(0.5f,1f);
		}
		else
		if(thesi==Thesi.left)
		{
			rekt.anchorMin = new Vector2(0f,0.5f);
		}
		else
		if(thesi==Thesi.leftUp)
		{
			rekt.anchorMin = new Vector2(0f,1f);
		}
		else
		if(thesi==Thesi.leftDown)
		{
			rekt.anchorMin = new Vector2(0f,0f);
		}
		else
		if(thesi==Thesi.right)
		{
			rekt.anchorMin = new Vector2(1f,0.5f);
		}
		else
		if(thesi==Thesi.rightDown)
		{
			rekt.anchorMin = new Vector2(1f,0f);
		}
		else
		if(thesi==Thesi.rightUp)
		{
			rekt.anchorMin = new Vector2(1f,1f);
		}
		else
		{
			rekt.anchorMin = new Vector2(0.5f,0.5f);
		}

	}

	/*
	void CreateCanvasMap(){
		
		GameObject kanvas = new GameObject();
		_canvas = kanvas.AddComponent<Canvas>();
		_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		kanvas.name="CanvasMapStathis";
		groupCanvas = kanvas.AddComponent<CanvasGroup>();
		CanvasScaler cSkaler = kanvas.AddComponent<CanvasScaler>();
		cSkaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		cSkaler.scaleFactor=0.5f;
		kanvas.AddComponent<GraphicRaycaster>();
		
		GameObject panel = new GameObject();
		panel.name="xartisContainer";
		panel.AddComponent<Image>();
		panel.AddComponent<Mask>().showMaskGraphic=false;
		panel.transform.SetParent(kanvas.transform);
		panel.transform.localPosition=Vector2.zero;
		panel.GetComponent<RectTransform>().pivot=new Vector2(1f,0f);
		panel.GetComponent<RectTransform>().anchorMin=new Vector2(1f,0f);
		panel.GetComponent<RectTransform>().anchorMax=new Vector2(1f,0f);
		panel.GetComponent<RectTransform>().anchoredPosition=Vector2.zero;
		panel.GetComponent<RectTransform>().sizeDelta=new Vector2(210f,210f);
		
		GameObject imgMap = new GameObject();
		imgMap.name = "mapImage";
		imgMap.AddComponent<RawImage>().texture=mapTexture;
		imgMap.transform.SetParent(panel.transform);
		mapImage = imgMap.GetComponent<RectTransform>();
		mapImage.pivot=new Vector2(0.5f,0.5f);
		mapImage.anchorMin=new Vector2(0.5f,0.5f);
		mapImage.anchorMax=new Vector2(0.5f,0.5f);
		mapImage.anchoredPosition=Vector2.zero;
		mapImage.sizeDelta=new Vector2(mapTexture.width,mapTexture.height);
		
		GameObject imgDot = new GameObject();
		imgDot.name = "dotPerson";
		imgDot.AddComponent<Image>().overrideSprite=dotSprite;
		imgDot.transform.SetParent(panel.transform);
		dotPerson = imgDot.GetComponent<RectTransform>();
		dotPerson.pivot=new Vector2(0.5f,0.5f);
		dotPerson.anchorMin=new Vector2(0.5f,0.5f);
		dotPerson.anchorMax=new Vector2(0.5f,0.5f);
		dotPerson.anchoredPosition=Vector2.zero;
		dotPerson.sizeDelta=new Vector2(40f,40f);
		
		GameObject plaisio = new GameObject();
		plaisio.name = "plaisio";
		plaisio.transform.SetParent(panel.transform);
		Image gg = plaisio.AddComponent<Image>();
		RectTransform tr = plaisio.GetComponent<RectTransform>();
		StretchRectTransform(tr);
		gg.overrideSprite=plaisioSprite;
		
	}
	
	void StretchRectTransform(RectTransform rtr){
		rtr.pivot=new Vector2(0.5f,0.5f);
		rtr.anchorMin=new Vector2(0f,0f);
		rtr.anchorMax=new Vector2(1f,1f);
		rtr.anchoredPosition=Vector2.zero;
		rtr.sizeDelta=Vector2.zero;
	}
	*/
	
		
}



public enum _try
	
{
	lol,
	meh,
	blah,
	rofl,
	lmao
	
}