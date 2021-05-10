using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Stathis;

public class AlphaRaycastFilter : MonoBehaviour {

	public RectTransform rectTransform;
	public Image image;

	//small map stigma person
	public RectTransform pinezaPerson;
	//full map stigma person
//	public RectTransform pinezaPersonFullMap;
//	Vector2 result;

	public Canvas kanvas;

	PointerEventData eventData;
	
	Vector3 prin, meta, worldPos, screenPos, personPos;
//	Vector2 analogiaTerrainTexture;
	bool hasInit;
	Xartis xartis;
//	Camera kamera;
//	CanvasGroup kanvasGroup;
	
	void Awake()
	{
		rectTransform = transform as RectTransform;
		image = GetComponent<Image>();
//		kanvasGroup = kanvas.GetComponent<CanvasGroup> ();

//		Stathis.Tools_UI.RescaleImage (image,false);
//		image.rectTransform.pivot = new Vector2 (0.5f, 0f);
//		image.rectTransform.anchorMin = new Vector2 (0.5f, 0f); 
//		image.rectTransform.anchorMax = new Vector2 (0.5f, 0f); 

//		Init ();
	}

	public void Init(){
		if (!xartis) {
			xartis = Diadrasis.Instance.menuUI.xartis;
		}

		//make canvas mode with camera to catch touches
//		kanvas.renderMode = RenderMode.ScreenSpaceCamera;
//		kanvas.worldCamera = Camera.main;
//		kanvas.planeDistance = Camera.main.nearClipPlane;

		//set new filtro image 
		//image.sprite = Tools_Load.LoadSpriteFromResources(appSettings.mapImagePath, Diadrasis.Instance.mapFilter);

		//get diastaseis texture
		Vector2 diastaseisFiltrou = new Vector2 (image.sprite.texture.width, image.sprite.texture.height);

		//assign dimensions to rect transform
		rectTransform.sizeDelta = diastaseisFiltrou;

		//set analogia for calculations on target movement
//		analogiaTerrainTexture = zoomLogos ();

		#if UNITY_EDITOR
		Debug.Log("terrain size = "+moveSettings.terrainSize);
		Debug.Log("zoomLogose = " + zoomLogos ());
		#endif

//		kamera = Camera.main;

		hasInit = true;
	}

	void OnDisable(){
//		kanvas.renderMode = RenderMode.ScreenSpaceOverlay;
		hasInit = false;
	}


//	void KanvasInteractive(bool val){
//		kanvasGroup.interactable = val;
//		kanvasGroup.blocksRaycasts = val;
//	}

//	void Update(){
//		//if small map or full map
//		//check tap only if offsite
//		//else if on site ->> return;
//
//		//store position
//		prin = pinezaPersonSmallMap.transform.localPosition;
//	}

	public bool inArea;

	public bool autoMove;

	public void _LateUpdate(){

//		if (Diadrasis.Instance.user == Diadrasis.User.inFullMap) {
//			KanvasInteractive (true);
//		} else {
//			KanvasInteractive(false);
//		}

		if (image.sprite == null && hasInit) {
			#if UNITY_EDITOR
			Debug.Log("empty sprite !!!");
			#endif
			return;
		}

		if (autoMove) {
			//get world person positon
			personPos = xartis.targets.person.position;

			//calculate to map position
			pinezaPerson.localPosition = new Vector3 (personPos.x * zoomLogos ().x, personPos.z * zoomLogos ().y, 0f);
		}

		//store position
		prin = pinezaPerson.localPosition;

		if(prin==meta){
			return;
		}

//		worldPos = pinezaPerson.transform.TransformPoint(pinezaPerson.position);

//		screenPos = kamera.WorldToScreenPoint (worldPos);

//		if(IsMovementLocationValid(screenPos, kamera)){
		if(IsTargetLocationValid(pinezaPerson.localPosition)){
			#if UNITY_EDITOR
			Debug.LogWarning("IN AREA");
			#endif
		}

		//set new position
		meta = pinezaPerson.localPosition;
	}

	#region terrain / texture stathera

	Vector2 zoomLogos(){
		//get map container width/height
//		float mapX = mapImage.sizeDelta.x;
//		float mapY = mapImage.sizeDelta.y;

		Vector2 stathera = rectTransform.sizeDelta;
		
		//divide map size with terrain size
		stathera = new Vector2(stathera.x / moveSettings.terrainSize.x , stathera.y / moveSettings.terrainSize.y);
		
		return stathera;
	}

	#endregion

	#region IPointerClickHandler implementation

//	public void OnPointerClick(PointerEventData eventData)
//	{
//		#if UNITY_EDITOR
//		Debug.LogWarning("OnPointerClick");
//		#endif
//
////		Debug.Log(eventData.position);
//		if(IsRaycastLocationValid(eventData.position, kamera)){
//
//			#if UNITY_EDITOR
//			Debug.LogWarning("IN AREA");
//			#endif
//		}
//	}
	
	#endregion
	
	#region ICanvasRaycastFilter implementation
	
	public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{

		// Get normalized hit point within rectangle (aka UV coordinates originating from bottom-left)
		Vector2 rectPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out rectPoint);
		Vector2 normPoint = (rectPoint - rectTransform.rect.min);
		normPoint.x /= rectTransform.rect.width;
		normPoint.y /= rectTransform.rect.height;

//		bb.localPosition = rectPoint;

		// Read pixel color at normalized hit point
		Texture2D texture = image.sprite.texture;
		Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));
		
		// Filter away hits on transparent pixels
		if(color.a==0){
			inArea=false;
			#if UNITY_EDITOR
			Debug.LogWarning("OUT OF AREA!!!");
			#endif
//			return false;
		}else{
			inArea=true;
		}
//		Debug.Log(color);
		return inArea;
	}

	public bool IsTargetLocationValid(Vector2 rectPosition)
	{
		// Get normalized hit point within rectangle (aka UV coordinates originating from bottom-left)
//		Vector2 rectPoint = pinezaPerson.localPosition;
//		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out rectPoint);
		Vector2 normPoint = (rectPosition - rectTransform.rect.min);
		normPoint.x /= rectTransform.rect.width;
		normPoint.y /= rectTransform.rect.height;
		
		//		bb.localPosition = rectPoint;
		
		// Read pixel color at normalized hit point
		Texture2D texture = image.sprite.texture;
		Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));
		
		// Filter away hits on transparent pixels
		if(color.a==0){
			inArea=false;
			#if UNITY_EDITOR
			Debug.LogWarning("OUT OF AREA!!!");
			#endif
			//			return false;
		}else{
			inArea=true;
		}
		//		Debug.Log(color);
		return inArea;
	}


	public bool IsMovementLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		
		// Get normalized hit point within rectangle (aka UV coordinates originating from bottom-left)
		Vector2 rectPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out rectPoint);
		Vector2 normPoint = (rectPoint - rectTransform.rect.min);
		normPoint.x /= rectTransform.rect.width;
		normPoint.y /= rectTransform.rect.height;
		
		//		bb.localPosition = rectPoint;
		
		// Read pixel color at normalized hit point
		Texture2D texture = image.sprite.texture;
		Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));
		
		// Filter away hits on transparent pixels
		if(color.a==0){
			inArea=false;
			#if UNITY_EDITOR
			Debug.LogWarning("out of area!!!");
			#endif
			//			return false;
		}else{
			inArea=true;
		}
		//		Debug.Log(color);
		return inArea;
	}
	
	#endregion
}
