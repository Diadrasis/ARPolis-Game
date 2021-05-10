using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class fotoItem : MonoBehaviour {

	public RawImage photo;
	public Text subText;
	public Mask maska;
	public LayoutElement myLayOut;
	public Vector2 mySizeFromFather;

	void OnEnable(){
		myLayOut.minWidth = mySizeFromFather.x;
		myLayOut.minHeight = mySizeFromFather.y;
	}

	public void SetText()
	{
		if(!maska)
		{
			maska=subText.GetComponent<Mask>();
		}

		if(maska)
		{
			if(!string.IsNullOrEmpty(subText.text))
			{
				maska.showMaskGraphic=true;
			}
			else
			{
				maska.showMaskGraphic=false;
			}
		}
		else
		{
			Debug.Log("No Mask!!");
		}
	}

	public void ResizeImage(Texture eikona)
	{
		RectTransform rectTrans_imgMain = photo.rectTransform;

		photo.texture = eikona;

		Stathis.Tools_UI.RescaleRawImage(photo, mySizeFromFather);

//		Rect rT = photo.rectTransform.rect;
//
//		rT.width = 680f;
//		rT.height = 458f;
//
////		Vector2 p = Diadrasis.Instance.ui2dcanvas.GetComponent<Canvas>().GetComponent<RectTransform>().sizeDelta;
////
////		rT.width = p.x;
////		rT.height = p.y;
////		
////		rectTrans_imgMain.sizeDelta = new Vector2(p.x,p.y);
//
//		rectTrans_imgMain.sizeDelta = new Vector2(680f,458f);
//
//		Vector2 oldSize = rectTrans_imgMain.rect.size;
//		Vector2 newSize = rectTrans_imgMain.rect.size;
//
//		photo.texture = eikona;
//
//		if(photo.texture){
//			float imgW = (float)photo.texture.width;				//print ("imgW = "+imgW);
//			float imgH = (float)photo.texture.height;				//print("imgH = "+imgH);
//			float rectW = rT.width;								//print ("rectW = "+rectW);
//			float rectH = rT.height;							//print ("rectH = "+rectH);
//			
//			if(imgH<imgW){
//				float f = imgH/imgW;
//				//			rT.width = rectW;
//				float newHeight = rectW * f;
//				newSize = new Vector2(rectTrans_imgMain.rect.size.x, newHeight);
//			}else
//			if(imgW<imgH){
//				float f = imgW/imgH;
//				//			rT.height = rectH;
//				float newWidth = rectH * f;
//				newSize = new Vector2(newWidth, rectTrans_imgMain.rect.size.y);
//			}
//			
//			//Debug.Log(newSize);
//			
//			Vector2 deltaSize = newSize - oldSize;
//			rectTrans_imgMain.offsetMin = rectTrans_imgMain.offsetMin - new Vector2(deltaSize.x * rectTrans_imgMain.pivot.x, deltaSize.y * rectTrans_imgMain.pivot.y);
//			rectTrans_imgMain.offsetMax = rectTrans_imgMain.offsetMax + new Vector2(deltaSize.x * (1f - rectTrans_imgMain.pivot.x), deltaSize.y * (1f - rectTrans_imgMain.pivot.y));
//			
//
//		}

		if(!Diadrasis.Instance.infoImageTextFullWdth){
			subText.GetComponent<LayoutElement>().minWidth = rectTrans_imgMain.sizeDelta.x - 13f;
		}else{
			subText.GetComponent<LayoutElement>().minWidth = mySizeFromFather.x - 13f;
		}

		subText.fontSize = appSettings.fontSize_keimeno;

//		if(!Diadrasis.Instance.screenSize){
//			subText.fontSize=23;
//		}else{
//			subText.fontSize=14;
//		}
	}
}
