using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using eChrono;

//CLASSES SHOWN IN INSPECTOR
#region Serializable Classes

[Serializable]
public class TitlePanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Transform titleText;
	//Instantiate(Resources.Load("prefabs/newUI/LeanTouch")) as GameObject;
}

[Serializable]
public class ShortDescPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Text titlosText;
	public Text shortDescText;
	public RawImage imgMain;
	public Font myFont;

	public Button btnFullDesc;
	public Button btnVideo;
	public Button btnNarration;
	public Button btnPhotos;
	public Button btnPhotosResize;
}

[Serializable]
public class FullDescPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Text titleText;
	public Text fullDescText;
}

[Serializable]
public class PhotosPanel  : System.Object//Not a MonoBehaviour!
{
	public GameObject mainPanel;
	public Text subtitleText;
	public RawImage foto;
	public Button btnNext;
	public Button btnPrev;

	public Transform listItemsContainer;
	public ScrollSnap scrolSnap;
}

#endregion


public class InfoUI : MonoBehaviour
{

	#region PUBLIC VARIABLES

	public TitlePanel titlePanel;
	public ShortDescPanel shortDescPanel;
	public FullDescPanel fullDescPanel;
	public PhotosPanel photoPanel;

	public GameObject blurBack;

	public List<RectTransform> listChilds = new List<RectTransform>();

	#endregion

	#region PRIVATE VARIABLES

	MenuUI menuUI;
	AnimControl animControl;
	cPoi currentPoi;
	Vector3 descPos,shortPos;
	
	#endregion

	#region START

	void Start () {
		menuUI=transform.parent.GetComponent<MenuUI>();
		animControl=transform.parent.GetComponent<AnimControl>();
		animControl.infoUI=this;

		shortDescPanel.btnFullDesc.onClick.AddListener(()=>animControl.FullDescShow());
		shortDescPanel.btnPhotos.onClick.AddListener(()=>ShowPhoto());// animControl.PhotosShow());
		shortDescPanel.btnPhotosResize.onClick.AddListener(()=>ShowPhoto());// animControl.PhotosShow());

		shortDescPanel.btnVideo.onClick.AddListener(()=>menuUI.VideoShow());
		shortDescPanel.btnNarration.onClick.AddListener(()=>menuUI.NarrationShow());

		shortDescPanel.titlosText.font=shortDescPanel.myFont;

//		Diadrasis.Instance.olaTaKeimena_Info.Add(shortDescPanel.titlosText);
		Diadrasis.Instance.olaTaKeimena_Info.Add(fullDescPanel.fullDescText);
		Diadrasis.Instance.olaTaKeimena_Info.Add(fullDescPanel.titleText);
	}

	#endregion

	#region CHECK IF POI EXISTS
	
	bool CheckIfPointExists (String name){	
		//bool exists=false;
		if(string.IsNullOrEmpty(name)){
			return false;
		}
		if (name.Contains("(Clone)"))
			name = name.Replace("(Clone)", "");			
		
		if (appData.myPoints.ContainsKey(name))
		{
			cPoi myPoi;
			appData.myPoints.TryGetValue(name, out myPoi);
			currentPoi = myPoi;
			return true;
		}else{
			return false;
		}
	}

	#endregion

	#region ON LOAD INFO (SHORT DESC)

	public void ResetShortDescTextPos(){
		//reset full desc text scroll position
		shortPos = shortDescPanel.shortDescText.GetComponent<RectTransform>().position;
		shortPos.y=0f;
		shortDescPanel.shortDescText.GetComponent<RectTransform>().position = shortPos;
	}

	public void SetShortDesc()
	{
		if(CheckIfPointExists(Diadrasis.Instance.currentPoi))
		{
			ResetShortDescTextPos ();

			shortDescPanel.titlosText.text = currentPoi.title;
			shortDescPanel.titlosText.font=shortDescPanel.myFont;
			shortDescPanel.shortDescText.text = currentPoi.shortDesc;

			shortDescPanel.shortDescText.fontSize = appSettings.fontSize_keimeno;
			shortDescPanel.titlosText.fontSize = appSettings.fontSize_titlos;

			//shortDescPanel.imgMain.texture = currentPoi.images[0].file

			RectTransform rectTrans_imgMain = shortDescPanel.imgMain.transform.parent.GetComponent<RectTransform>();
			Vector2 sizeFather = rectTrans_imgMain.sizeDelta;

			if(currentPoi.images!=null && currentPoi.images.Count>0){
				//shortDescPanel.imgMain.gameObject.SetActive(true);

				//ResizeImage(LoadTexture(currentPoi.images[0].file));

				shortDescPanel.imgMain.texture = LoadTexture(currentPoi.images[0].file);


//				Stathis.Tools_UI.ResizeTextureToContainerSize (rectTrans_imgMain, false, false);
				Stathis.Tools_UI.RescaleRawImage_keepWidth (shortDescPanel.imgMain, sizeFather, true);

				shortDescPanel.btnPhotos.gameObject.SetActive(true);
				shortDescPanel.btnPhotosResize.gameObject.SetActive(true);
				
				SetPhotos();
			}else
			{
				//ResizeImage(LoadTexture("not_available.jpg"));

				shortDescPanel.imgMain.texture = LoadTexture("not_available.jpg");

				//Stathis.Tools_UI.ResizeTextureToContainerSize (rectTrans_imgMain, false);
				Stathis.Tools_UI.RescaleRawImage (shortDescPanel.imgMain, sizeFather);
				
				//hide buttons
				shortDescPanel.btnPhotos.gameObject.SetActive(false);
				shortDescPanel.btnPhotosResize.gameObject.SetActive(false);
				//hide image container
				//shortDescPanel.imgMain.gameObject.SetActive(false);
			}

			//check available infos
			if(currentPoi.videos!=null && currentPoi.videos.Count>0)
			{
				shortDescPanel.btnVideo.gameObject.SetActive(true);
			}
			else
			{
				shortDescPanel.btnVideo.gameObject.SetActive(false);
			}

			if(menuUI.narrationSource && menuUI.narrationSource.isPlaying){
				if(menuUI.isNarrPlaying)
				{
					menuUI.StopNarration();
					menuUI.narrationIsPlaying.SetActive(false);
				}
			}

			if(currentPoi.narrations!=null && currentPoi.narrations.Count>0)
			{
				shortDescPanel.btnNarration.gameObject.SetActive(true);
			}
			else
			{
				shortDescPanel.btnNarration.gameObject.SetActive(false);
			}

			if(!string.IsNullOrEmpty(currentPoi.desc))
			{
				shortDescPanel.btnFullDesc.gameObject.SetActive(true);
				SetFullDesc();
			}
			else
			{
				shortDescPanel.btnFullDesc.gameObject.SetActive(false);
			}
		}
	}

	#endregion

	#region FULL DESC
	
	public void ResetFullDescTextPos(){
		//reset full desc text scroll position
		descPos = fullDescPanel.fullDescText.GetComponent<RectTransform>().position;
		descPos.y=0f;
		fullDescPanel.fullDescText.GetComponent<RectTransform>().position = descPos;
	}

	public void SetFullDesc()
	{
		if(CheckIfPointExists(Diadrasis.Instance.currentPoi))
		{
			ResetFullDescTextPos();
			fullDescPanel.titleText.text = currentPoi.title;

			fullDescPanel.fullDescText.text = currentPoi.desc;

			fullDescPanel.fullDescText.fontSize = appSettings.fontSize_keimeno;
			fullDescPanel.titleText.fontSize = appSettings.fontSize_titlos;

			shortDescPanel.shortDescText.fontSize = appSettings.fontSize_keimeno;
			shortDescPanel.titlosText.fontSize = appSettings.fontSize_titlos;

		}
	}

	#endregion

	#region PHOTO FUNCTIONS
	
	//not animation because we have runtime childs created
	void ShowPhoto()
	{
		StopCoroutine("eFoto");
		StartCoroutine("eFoto");
	}
	
	IEnumerator eFoto()
	{
		animControl.PhotosShow();
		animControl.animInfo.SetInteger("status",3);
		yield return new WaitForSeconds(0.5f);
		animControl.animInfo.enabled=false;
		photoPanel.scrolSnap.enabled = true;
		photoPanel.mainPanel.GetComponent<CanvasGroup>().alpha=0f;
		photoPanel.mainPanel.SetActive(true);
		SetPhotos();
		
		while(photoPanel.mainPanel.GetComponent<CanvasGroup>().alpha<0.9f)
		{
			photoPanel.mainPanel.GetComponent<CanvasGroup>().alpha	= Mathf.Lerp(photoPanel.mainPanel.GetComponent<CanvasGroup>().alpha,1f,Time.deltaTime);	//+=0.1f;
			//			yield return new WaitForSeconds(0.2f);
			yield return null;
		}
		
		photoPanel.mainPanel.GetComponent<CanvasGroup>().alpha=1f;
		yield return null;
	}

	public void SetPhotos()
	{
//		photoPanel.foto.texture = LoadTexture(currentPoi.images[0].file);

		photoPanel.btnNext.gameObject.SetActive(false);
		photoPanel.btnPrev.gameObject.SetActive(false);

		if(currentPoi.images.Count==0){return;}

#if UNITY_EDITOR
		foreach(cImage s in currentPoi.images) Debug.Log("### image " + s.file);
#endif

		if (currentPoi.images.Count>1)
		{
			photoPanel.btnNext.gameObject.SetActive(true);
			photoPanel.btnPrev.gameObject.SetActive(true);
		}

		// checking how many children of list are active
		int activeCount = 0;
		
		foreach (Transform tr in photoPanel.listItemsContainer)
		{
			if(tr.name=="fotoItem")
			{
				activeCount++;
			}
		}

		//Debug.Log("I count "+activeCount+" items");


		if(listChilds.Count>0)
		{
			foreach(RectTransform t in listChilds)
			{
				//if is disabled from previus poi renable it
				if(t && !t.gameObject.activeSelf)
				{
					t.gameObject.SetActive(true);
				}
			}
		}

		//if container have childs
		if(activeCount>0)
		{
			//clear previous list transform
			listChilds.Clear();

			//add new childs to list tranform
			foreach(RectTransform t in photoPanel.listItemsContainer)
			{
				if(t.name=="fotoItem")
				{
					//added to the list
					listChilds.Add(t);
				}
			}

			//if childs are less than images of current poi add more
			if(listChilds.Count<currentPoi.images.Count)
			{
				int newItems = currentPoi.images.Count - listChilds.Count;

				for(int i=0; i<newItems; i++)
				{
					//instatiate new child item
					GameObject fotoItem = Instantiate(Resources.Load("prefabs/FINAL/ui/fotoItem")) as GameObject;
					fotoItem.name = "fotoItem";
					//add child to container
					fotoItem.transform.SetParent(photoPanel.listItemsContainer);
					fotoItem.transform.localScale = new Vector3(1f,1f,1f);
					//add child to list transform
					listChilds.Add(fotoItem.GetComponent<RectTransform>());
				}

			}
			else
			// if childs are more than images disable the rest
				if(listChilds.Count>currentPoi.images.Count)
			{
				for(int i=listChilds.Count-1; i>=0; i--)
				{
					//disable child in container
					listChilds[i].gameObject.SetActive(false);
					//remove child from the list transform
					listChilds.Remove(listChilds[i]);
				}
			}
		}
		else
		//else if there are no childs create new ones
		{
			//clear previous list transform
			listChilds.Clear();

			for(int i=0; i<currentPoi.images.Count; i++)
			{
				//instatiate new child item
				GameObject fotoItem = Instantiate(Resources.Load("prefabs/FINAL/ui/fotoItem")) as GameObject;
				fotoItem.name = "fotoItem";
				//add child to container
				fotoItem.transform.SetParent(photoPanel.listItemsContainer);
				fotoItem.transform.localScale = new Vector3(1f,1f,1f);
				//add child to list transform
				listChilds.Add(fotoItem.GetComponent<RectTransform>());
			}
		}

#if UNITY_EDITOR
		Debug.Log("### listChilds Count = " + listChilds.Count);
#endif

		//set new values to all childs
		if (listChilds.Count>0)
		{
			for(int i=0; i<listChilds.Count; i++)
			{
				//get script
				fotoItem script =  listChilds[i].GetComponent<fotoItem>();
				script.mySizeFromFather = Diadrasis.Instance.canvasMainRT.sizeDelta;

#if UNITY_EDITOR
				Debug.Log("### loadin image " + currentPoi.images[i].file);
#endif

				//set image
				script.ResizeImage(LoadTexture(currentPoi.images[i].file));

				//set text
				script.subText.text = currentPoi.images[i].text;
				script.SetText();
			}
		}

	}

	public void ResetPhotoPanel(){
		if(listChilds.Count>0)
		{
			#if UNITY_EDITOR
			Debug.Log("ResetPhotoPanel");
			#endif

			foreach(RectTransform t in listChilds)
			{
				//if is disabled from previus poi renable it
				if(t)
				{
					t.transform.SetParent(null);
					DestroyImmediate(t.gameObject);
				}
			}
			listChilds.Clear();
		}
	}

	Texture LoadTexture(string t){
		t=t.Substring(0, t.Length-4); //truncate the file extension
#if UNITY_EDITOR
		Debug.Log("### loadin image path = " + appSettings.dataImagePath + t);
#endif
		return (Texture2D)Resources.Load (appSettings.dataImagePath + t) as Texture2D;
	}

	public void ResizeImage(Texture eikona)
	{
		RectTransform rectTrans_imgMain = shortDescPanel.imgMain.rectTransform;
		Rect rT = shortDescPanel.imgMain.rectTransform.rect;
		
		rT.width = 250f;
		rT.height = 125f;
		
		rectTrans_imgMain.sizeDelta = new Vector2(250f,125f);
		
		Vector2 oldSize = rectTrans_imgMain.rect.size;
		Vector2 newSize = rectTrans_imgMain.rect.size;
		
		shortDescPanel.imgMain.texture = eikona;
		
		if(shortDescPanel.imgMain.texture){
			float imgW = (float)shortDescPanel.imgMain.texture.width;				//print ("imgW = "+imgW);
			float imgH = (float)shortDescPanel.imgMain.texture.height;				//print("imgH = "+imgH);
			float rectW = rT.width;								//print ("rectW = "+rectW);
			float rectH = rT.height;							//print ("rectH = "+rectH);
			
			if(imgH<imgW){
				float f = imgH/imgW;
				//			rT.width = rectW;
				float newHeight = rectW * f;
				newSize = new Vector2(rectTrans_imgMain.rect.size.x, newHeight);
			}else
			if(imgW<imgH){
				float f = imgW/imgH;
				//			rT.height = rectH;
				float newWidth = rectH * f;
				newSize = new Vector2(newWidth, rectTrans_imgMain.rect.size.y);
			}
			
			//Debug.Log(newSize);
			
			Vector2 deltaSize = newSize - oldSize;
			rectTrans_imgMain.offsetMin = rectTrans_imgMain.offsetMin - new Vector2(deltaSize.x * rectTrans_imgMain.pivot.x, deltaSize.y * rectTrans_imgMain.pivot.y);
			rectTrans_imgMain.offsetMax = rectTrans_imgMain.offsetMax + new Vector2(deltaSize.x * (1f - rectTrans_imgMain.pivot.x), deltaSize.y * (1f - rectTrans_imgMain.pivot.y));
		}
	}

	#endregion

}
