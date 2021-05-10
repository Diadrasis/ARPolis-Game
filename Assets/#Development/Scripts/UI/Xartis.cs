using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using eChrono;
using Stathis;

#region SERIALIZABLE CLASSES

[Serializable]
public class ButtonsMap  : System.Object//Not a MonoBehaviour!
{
	public Button zoomIn;
	public Button zoomOut;
	public Button closeMap;
	public Button maxMap;

	public void ShowBtnMax(bool val){
		if (maxMap.gameObject.activeSelf != val) {
			maxMap.gameObject.SetActive(val);
		}
		maxMap.GetComponent<CanvasGroup> ().interactable = val;
		maxMap.interactable = val;
	}
}

[Serializable]
public class TargetsMap  : System.Object//Not a MonoBehaviour!
{
	[Tooltip("the target the map will follow")]
	public Transform person;
	
	[Tooltip("the target the dot will rotate with")]
	public Transform kamera;
}

#endregion

public class Xartis : MonoBehaviour {

	#region VARIABLES

	/// <summary>
	/// The map status.
	/// </summary>
	public enum MapStatus{Close, Mikros, Full}
	public MapStatus mapStatus = MapStatus.Close;

	/// <summary>
	/// The map previous status.
	/// </summary>
	public enum MapPreviusStatus{Close, Mikros, Full}
	public MapPreviusStatus mapPreviousStatus = MapPreviusStatus.Close;

	/// <summary>
	/// The ray layer.
	/// </summary>
	public LayerMask rayLayer;

	/// <summary>
	/// The touch target.
	/// </summary>
	public RectTransform touchTarget;

	//all buttons
	public ButtonsMap btnsMap;
	//all targets
	public TargetsMap targets;

	Texture mapTexture;
	RawImage mapRawImage;

	
	[Tooltip("the RawImage in canvas holding the map texture. Important -> Set Pivot Point at center of the scene (0,0)")]
	public RectTransform mapRect;//Set Pivot Point at center of the scene (0,0)
	
	[Tooltip("εικονα σημαδουρας - για να περιστρεφεται με το Υ της καμερας")]
	public RectTransform personVelaki;//always at center

	[Tooltip("map father holding everything so can be scaled")]
	public RectTransform mapContainer;//always at center

	//public Image mapFilter;
	//RectTransform mapFilterRect;
	
	public RectTransform personFullMap;
	public RectTransform personFullVelaki;

	public GameObject fullMapHelp;
	public GameObject fullMapHelpOnSite;

	Vector2 localCursor;
	int tapActions=0;
	float newPosY;

	//metatropi world position to map position
	Vector2 stathera;

	//stathera's calculations
	float mapX=0f;
	float mapY=0f;

	Canvas _canvas;
	CanvasGroup groupCanvas;

	MenuUI menuUI;
	AnimControl animControl;

	List<Button> mapBtns = new List<Button>();

	public Vector3 personAllowPosition;

	GameObject selectedObject;

	public bool isPersonInGraphicsArea;

	public delegate void ActionMap();
	//full map tap to go
	public event ActionMap OnFullMapTap;

	#endregion

	#region DELEGATES
	void OnEnable()
	{
		Diadrasis.Instance.OnMapFullClose += HideHelps;
		Diadrasis.Instance.OnSceneLoadEnd += Init;
		Diadrasis.Instance.OnMapFullShow += HideHelps;
		Diadrasis.Instance.OnMapFullShow += FullMapCenter;
		Diadrasis.Instance.OnMapFullShow += ShowHelp;
	}
	
	
	void OnDisable()
	{
		if(Diadrasis.Instance !=null)//avoid error on application quit
		{
			Diadrasis.Instance.OnMapFullClose -= HideHelps;
			Diadrasis.Instance.OnMapFullShow -= HideHelps;
			Diadrasis.Instance.OnMapFullShow -= FullMapCenter;
			Diadrasis.Instance.OnMapFullShow -= ShowHelp;
			Diadrasis.Instance.OnSceneLoadEnd -= Init;
		}
	}

	public void HideHelps(){
		fullMapHelpOnSite.SetActive(false);
		fullMapHelp.SetActive(false);
	}

	public void ShowHelp(){
		//if offsite show help that he can tap on map to transfer person
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
			fullMapHelp.SetActive(true);
		}else//show help that is far away from the scene
		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
			fullMapHelpOnSite.SetActive(true);
		}
	}

	#endregion

	#region MAP STATUS
		
	public void ChangeStatus(MapStatus status)
	{
		mapStatus=status;
		
		switch (mapStatus)
		{
		case(MapStatus.Close):
			//fix bug when closing photo or full desc
			//map btn was turning on
			//also dont need it in menu
			if(Diadrasis.Instance.user==Diadrasis.User.inPoi || Diadrasis.Instance.user==Diadrasis.User.inMenu){
				return;
			}
			menuUI.btnsMenu.ShowBtn_Map(true);
			//hide max map button
			btnsMap.ShowBtnMax (false);
			break;
		case(MapStatus.Mikros):
			//show close map button
			btnsMap.closeMap.gameObject.SetActive(true);
			//hide map buton
			menuUI.btnsMenu.ShowBtn_Map (false);
			//show max map button
			if(Diadrasis.Instance.navMode == Diadrasis.NavMode.offSite){
				btnsMap.ShowBtnMax (true);
			}else{
				btnsMap.ShowBtnMax (false);
			}
			//hide pineza full icon
			personFullMap.gameObject.SetActive(false);
			personFullVelaki.gameObject.SetActive(false);
			break;
		case(MapStatus.Full):
			//show pineza full icon
//			personFullMap.gameObject.SetActive (true);
			personFullVelaki.gameObject.SetActive(true);
			//hide max map button
			btnsMap.ShowBtnMax (false);
			//show close map button
			btnsMap.closeMap.gameObject.SetActive(false);
			break;
		default:
			menuUI.btnsMenu.ShowBtn_Map(true);
			break;
		}
	}
	
	public void ChangePreviusStatus(MapPreviusStatus previousStatus)
	{
		mapPreviousStatus=previousStatus;
	}
	
	public void CheckMapPreviusStatus(){
		#if UNITY_EDITOR
		Debug.Log("CheckMapPreviusStatus");
		#endif

		switch (mapPreviousStatus)
		{
			case(MapPreviusStatus.Close):
				//also dont need it in menu
				if(Diadrasis.Instance.user==Diadrasis.User.inMenu){
					return;
				}
				#if UNITY_EDITOR
				Debug.Log("close map !!!");
				#endif
				animControl.CloseMap();
				menuUI.btnsMenu.ShowBtn_Map(true);
				//hide max map button
				btnsMap.ShowBtnMax (false);
				break;
			case(MapPreviusStatus.Mikros):
				animControl.ShowSmallMap();
				//show close map button
				btnsMap.closeMap.gameObject.SetActive(true);
				//hide map buton
				menuUI.btnsMenu.ShowBtn_Map (false);
				//hide pineza full icon
				personFullMap.gameObject.SetActive(false);
				personFullVelaki.gameObject.SetActive(false);
				break;
			case(MapPreviusStatus.Full):
				break;
			default:
				#if UNITY_EDITOR
				Debug.Log("close map !!!");
				#endif
				animControl.CloseMap();
				menuUI.btnsMenu.ShowBtn_Map(true);
				//hide pineza full icon
				personFullMap.gameObject.SetActive(false);
				personFullVelaki.gameObject.SetActive(false);
				//hide max map button
				btnsMap.ShowBtnMax (false);
				break;
		}
	}
	
	public void SetMapPreviusStatus(){

		switch (mapStatus)
		{
		case(MapStatus.Close):
			#if UNITY_EDITOR
			Debug.Log("SetMapPreviusStatus close");
			#endif
			mapPreviousStatus = MapPreviusStatus.Close;
			break;
		case(MapStatus.Mikros):
			#if UNITY_EDITOR
			Debug.Log("SetMapPreviusStatus mikros");
			#endif
			mapPreviousStatus = MapPreviusStatus.Mikros;
			break;
		case(MapStatus.Full):
			#if UNITY_EDITOR
			Debug.Log("SetMapPreviusStatus full");
			#endif
			mapPreviousStatus = MapPreviusStatus.Full;
			break;
		default:
			#if UNITY_EDITOR
			Debug.Log("SetMapPreviusStatus default close");
			#endif
			mapPreviousStatus = MapPreviusStatus.Close;
			break;
		}
	}

	#endregion
	
	#region MAP BUTTONS

	public void ShowBtnsOffMap(bool isOn)
	{
		if(isOn)
		{
			//enable
			foreach(Button bt in mapBtns)
			{
				bt.GetComponent<Image>().color=Color.black;
				bt.interactable=true;
				bt.GetComponent<CanvasGroup>().blocksRaycasts=true;
			}
		}
		else
		{
			//disable
			foreach(Button bt in mapBtns)
			{
				bt.GetComponent<Image>().color=Color.clear;
				bt.interactable=false;
				//stop from tap a ray for person trasportation
				bt.GetComponent<CanvasGroup>().blocksRaycasts=false;
			}
		}
	}

	#endregion

	#region INITIALIZE

	public void Init ()
	{
		#if UNITY_EDITOR
		Debug.LogWarning("for correct terrain raycast set rayLayer >> terrain!!");
		#endif

		//get scripts
		if(!menuUI){menuUI=GetComponent<MenuUI>();}
		if(!animControl){animControl=GetComponent<AnimControl>();}
		//get canvas
		if(!_canvas){_canvas=GetComponent<Canvas>();}
		//get canvas group
		if(!groupCanvas){groupCanvas=_canvas.GetComponent<CanvasGroup>();}

		if (!mapRawImage) {
			mapRawImage = mapRect.GetComponent<RawImage>();
		}
//		if (!mapFilterRect) {
//			mapFilterRect = mapFilter.GetComponent<RectTransform>();
//		}

		//assign a function to each button
		btnsMap.zoomIn.onClick.AddListener(()=>Zoom(true));
		btnsMap.zoomOut.onClick.AddListener(()=>Zoom(false));

		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
			btnsMap.ShowBtnMax (false);
		}else{
			btnsMap.ShowBtnMax (true);
			btnsMap.maxMap.onClick.AddListener(()=>Diadrasis.Instance.FullScreenMap());//FullMapShow());
		}

		//add buttons to a list so we can hide / show etc.
		if(mapBtns.Count==0)
		{
			mapBtns.Add(btnsMap.zoomIn);
			mapBtns.Add(btnsMap.zoomOut);
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.offSite){
				mapBtns.Add(btnsMap.maxMap);
			}
		}

		//get the texture of map
		mapRawImage.texture = Tools_Load.LoadTexture(appSettings.mapImagePath, Diadrasis.Instance.mapScene);// + appSettings.language);

		mapRect.pivot = Diadrasis.Instance.mapPivot;

		mapTexture = mapRawImage.texture;

		if(mapTexture){
			//get texture size (width/height)
			Vector2 diastaseisMap = new Vector2(mapTexture.width, mapTexture.height); //mapTransf.sizeDelta;

//			Debug.Log("diastaseisMap = "+diastaseisMap);
			
			//scale RawImage to the size of map texture
			mapRect.sizeDelta = diastaseisMap;
		}

		zoomLimitsX = new Vector2(mapRect.sizeDelta.x,mapRect.sizeDelta.x*2f);
		zoomLimitsY = new Vector2(mapRect.sizeDelta.y,mapRect.sizeDelta.y*2f);

		#if UNITY_EDITOR
		Debug.LogWarning("loading filter map");
		#endif
		
		//mapFilter.gameObject.SetActive(true);

//		if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
//			mapFilter.sprite = Tools_Load.LoadSpriteFromResources(appSettings.mapImagePath, Diadrasis.Instance.mapFilter+"Onsite");
//		}else{
			//mapFilter.sprite = Tools_Load.LoadSpriteFromResources(appSettings.mapImagePath, Diadrasis.Instance.mapFilter);//"mapOttomanArea");//
//		}
		//mapFilterRect.sizeDelta = Diadrasis.Instance.mapFullZoom;
		
		#if UNITY_EDITOR
		Debug.Log(Diadrasis.Instance.mapScene);
		//Debug.Log(Diadrasis.Instance.mapFilter);
		#endif

//		analogiaTerrainEikoneas = zoomLogos ();

		#if UNITY_EDITOR
		Debug.Log("terrain size = "+moveSettings.terrainSize);
		Debug.Log("zoomAnalogia = " + zoomLogos ());
		#endif

		//initialize map to target position
		Move_Map();

//		GetArea();
		
	}

	#endregion

	#region LATEUPDATE

	void LateUpdate () {

		#region TOUCH EVENTS

		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)// && Diadrasis.Instance.user!=Diadrasis.User.onAir)
		{
			//dont accept tap if onsite
			if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
				return;
			}

			#if UNITY_EDITOR || UNITY_STANDALONE
			if(Input.GetMouseButtonDown(0))
			{
				Debug.Log("mouse down");
				onTap(true);
			}
			#elif UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount > 0 )
			{
				//if (Diadrasis.Instance.useMapFilterForMovement){
				//	return;
				//}
				if(Input.GetTouch (0).phase == TouchPhase.Began)
				{
					onTap(false);
				}
			}
			#endif
		}

		#endregion

		#region ARROW ON MAP MOVEMENT

		if((Diadrasis.Instance.user==Diadrasis.User.isNavigating) || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir))
		{
			//rotate dotImage with camera
			if(targets.kamera)
			{
				personVelaki.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -targets.kamera.eulerAngles.y));
			}
		}else
		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)
		{
			//rotate dotImage with camera
			if(targets.kamera)
			{
				personFullVelaki.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -targets.kamera.eulerAngles.y));
			}
		}

		#endregion

		//move relative to person transform
		Move_Map();

		//save all the time epitrepto position se periptosi pou o xristis energopoieisei apo ta settings to use graphics move map filter
//		if(Diadrasis.Instance.person && mapFilter.sprite!=null){
//			isPersonInGraphicsArea = IsTargetLocationValid();
//		}
		
	}

	#endregion

	#region MOVE MAP

	/// <summary>
	/// Moves the map.
	/// </summary>
	void Move_Map()
	{
		if((Diadrasis.Instance.user==Diadrasis.User.isNavigating) || (Diadrasis.Instance.user==Diadrasis.User.onAir && Diadrasis.Instance.moveOnAir) )
		{
			if(!personVelaki.gameObject.activeSelf)
			{
				personVelaki.gameObject.SetActive(true);
			}

			//move map to target posision
			if(targets.person)
			{
				Vector3 personPos = targets.person.position;

				personPos.x -= moveSettings.posCenterOfMap.x;
				personPos.z -= moveSettings.posCenterOfMap.y;

				mapRect.localPosition = new Vector3(personPos.x * -zoomLogos ().x , personPos.z * -zoomLogos ().y , 0f);

//				if(isPersonInGraphicsArea)
				personFullMap.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);// mapRect.localPosition;
				personFullVelaki.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);

			}
		}
		else
		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)
		{//if in full map
			if(personVelaki.gameObject.activeSelf)
			{
				personVelaki.gameObject.SetActive(false);

//				#if UNITY_EDITOR
//				Debug.Log("TODO -> set another velaki for full map showing direction of camera so user to know to where is going in gps ON (only??)");
//				#endif
			}

			//move dot to target posision
			if(targets.person && moveSettings.terrainSize!=Vector2.zero)
			{
				//get person position
				Vector3 personPos = targets.person.position;

				personPos.x -= moveSettings.posCenterOfMap.x;
				personPos.z -= moveSettings.posCenterOfMap.y;

//				if(isPersonInGraphicsArea)
				personFullMap.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);
				personFullVelaki.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);
			}
		}

	}

	#endregion

	#region TEST CODE

	/*
	 * 

	float ColorSqrDistance(Color c1, Color c2) {
    	return ((c2.r - c1.r) * (c2.r - c1.r) + (c2.b - c1.b) * (c2.b - c1.b) + (c2.g - c1.g) * (c2.g - c1.g));
 	}

	void Update()
	{
		
		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap && Diadrasis.Instance.user!=Diadrasis.User.onAir)
		{
			#if UNITY_EDITOR || UNITY_STANDALONE
			if(Input.GetMouseButtonDown(0))
			{
//				if (Diadrasis.Instance.useMapFilterForMovement) {
//					Debug.Log("Not Allow mouse down");
////					return;
//				}
				Debug.Log("mouse down");

				onTap(true);
			}
			#elif UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount > 0 )
			{
//				if (Diadrasis.Instance.useMapFilterForMovement) {
//					return;
//				}
				if(Input.GetTouch (0).phase == TouchPhase.Began)
				{
					onTap(false);
				}
			}
			#endif
		}
	}

*/

//	public Color[] pix;
//	public List<Color> notTranparentPixels = new List<Color>();

//	void GetAllValidPoints(){
//		int x = Mathf.FloorToInt(mapFilterRect.rect.x);
//		int y = Mathf.FloorToInt(mapFilterRect.rect.y);
//		int width = Mathf.FloorToInt(personFullMap.rect.width);
//		int height = Mathf.FloorToInt(personFullMap.rect.height);
//
//		// Read pixel color at normalized hit point
//		Texture2D texture = mapFilter.sprite.texture;
//		
//		if(texture==null){
//			return;
//		}
//
//		pix = texture.GetPixels(x, y, width, height);
//
//		if(pix.Length>0){
//			foreach(Color c in pix){
//				if(c.a!=0){
//					notTranparentPixels.Add(c);
//				}
//			}
//		}
//	}

	#endregion

	#region CHECK PERSON POSITION GRAPHICALLY
	/*
	public bool IsTargetLocationValid()
	{
		if(personFullMap && mapFilterRect)
		{
			Vector2 rectPoint = personFullMap.anchoredPosition;

			//se apotomi gps metetopisi check if pineza is out of picture and  stop from reading alpha>0 and getting bug results true
			if(rectPoint.x < mapFilterRect.rect.xMin || rectPoint.x > mapFilterRect.rect.xMax){
				return false;
			}
			if(rectPoint.y < mapFilterRect.rect.yMin || rectPoint.y > mapFilterRect.rect.yMax){
				return false;
			}

			Vector2 normPoint = (rectPoint - mapFilterRect.rect.min);

			normPoint.x /= mapFilterRect.rect.width;
			normPoint.y /= mapFilterRect.rect.height;
			
			// Read pixel color at normalized hit point
			Texture2D texture = mapFilter.sprite.texture;

			if(texture==null){
				return false;
			}

	//		Debug.Log((int)(normPoint.x * texture.width));

			Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));
			
			// Filter away hits on transparent pixels
			if(color.a==0){
				#if UNITY_EDITOR
				Debug.LogWarning("OUT OF Graphic AREA!!!");
				#endif
				return false;
			}

			personAllowPosition = targets.person.position;

			return true;
		}
		return false;
	}
	*/

	#endregion

	#region CHECK TOUCH POSITION GRAPHICALLY
	/*
	public bool IsTouchLocationValid()
	{
		Vector2 rectPoint = touchTarget.anchoredPosition;
		Vector2 normPoint = (rectPoint - mapFilterRect.rect.min);
		
		normPoint.x /= mapFilterRect.rect.width;
		normPoint.y /= mapFilterRect.rect.height;
		
		// Read pixel color at normalized hit point
		Texture2D texture = mapFilter.sprite.texture;
		
		if(texture==null){
			return false;
		}
		
		Color color = texture.GetPixel((int)(normPoint.x * texture.width), (int)(normPoint.y * texture.height));
		
		// Filter away hits on transparent pixels
		if(color.a==0){
			#if UNITY_EDITOR
			Debug.LogWarning("OUT OF AREA!!!");
			#endif
			return false;
		}
		
		personAllowPosition = targets.person.position;
		
		return true;
	}
	*/

	#endregion
	
	#region ON TAP TOUCH FUNCTION

	void onTap(bool isMouse)
	{
		if(isMouse)
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				selectedObject = EventSystem.current.currentSelectedGameObject;
				
				if(selectedObject)//an kapoio button exei patithei
				{
					tapActions=0;

					#if UNITY_EDITOR
					Debug.Log("onTap fullMap "+selectedObject.name);
					#endif

					if (selectedObject.name == "btnReturn") {
						if(Diadrasis.Instance.isStart<3){
							//show message "return to menu?"
							if(!menuUI.warningsUI.quitScenePanel.activeSelf){
								menuUI.warningsUI.quitScenePanel.SetActive(true);
							}
							else
							{
								menuUI.warningsUI.quitScenePanel.SetActive(false);
							}
						}else{
							Diadrasis.Instance.ReturnToMainMenu();
						}
					}
					
				}
				else//if tap on map
				{
					tapActions++;
					
					selectedObject=null;

					if(tapActions>=1)
					{
						#if UNITY_EDITOR
						Debug.LogWarning("IsPointerOverUIObject XARTIS 738");
						#endif
						if(Tools_UI.IsPointerOverUIObject())
						{
							if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)
							{
								//move person
								worldPointFromMap(Tools_UI.eventDataCurrentPosition);
							}
						}
						
					}
//					#if UNITY_EDITOR
//					Debug.Log(tapActions);
//					#endif			
				}
			}
		}
		else
		{
			#if UNITY_EDITOR
			Debug.LogWarning("IsPointerOverUIObject XARTIS 759");
			#endif
			if(Tools_UI.IsPointerOverUIObject())
			{
				selectedObject = EventSystem.current.currentSelectedGameObject;
				
				if(selectedObject)//an kapoio button exei patithei
				{
					tapActions=0;

					if (selectedObject.name == "btnReturn") {
						if(Diadrasis.Instance.isStart<3){
							//show message "return to menu?"
							if(!menuUI.warningsUI.quitScenePanel.activeSelf){
								menuUI.warningsUI.quitScenePanel.SetActive(true);
							}
							else
							{
								menuUI.warningsUI.quitScenePanel.SetActive(false);
							}
						}else{
							Diadrasis.Instance.ReturnToMainMenu();
						}
					}
				}
				else//if tap on map
				{
					tapActions++;
					
					selectedObject=null;
					
					if(tapActions>=1)
					{
						#if UNITY_EDITOR
						Debug.LogWarning("IsPointerOverUIObject XARTIS 778");
						#endif
						if(Tools_UI.IsPointerOverUIObject())
						{
							if(Diadrasis.Instance.user==Diadrasis.User.inFullMap)
							{
								//move person
								worldPointFromMap(Tools_UI.eventDataCurrentPosition);
							}
						}
						
					}
				}
			}
		}

	}

	#endregion

	#region CONVERT TOUCH TO WORLD POSITION
	cSnapPosition snapPathPoint, snapAreaPoint, snapPoint;

	//metafora person apo aggigma sto xarti
	void worldPointFromMap(PointerEventData tap)
	{

		//get tap position on map image only
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, tap.position, tap.pressEventCamera, out localCursor))//(mapTapFilter, tap.position, tap.pressEventCamera, out localCursor))
			return;

		#if UNITY_EDITOR
//		if (Diadrasis.Instance.useMapFilterForMovement && filterScript.autoMove) {
//			if(!filterScript.IsTargetLocationValid(localCursor)){
//				#if UNITY_EDITOR
//				Debug.LogWarning("Out of AREA.. Dont allow transition!");
//				#endif
//				return;
//			}else{
//				#if UNITY_EDITOR
//				Debug.LogWarning("Into an AREA.. Allow transition!");
//				#endif
//			}
//		}
		#endif

		//get person position
		Vector3 personPos = targets.person.position;

		#if UNITY_EDITOR
		Debug.Log("Person Pos Prin = "+personPos);
		#endif

		//calculate 2d pos to world position in scene
		personPos.x = localCursor.x / zoomLogos ().x ;
		personPos.y = moveSettings.personOnAirAltitude;
		personPos.z = localCursor.y / zoomLogos ().y ;

		personPos.x += moveSettings.posCenterOfMap.x;
		personPos.z += moveSettings.posCenterOfMap.y;

		#if UNITY_EDITOR
		Debug.Log("Person Pos On Touch = "+personPos);
		#endif


		//if snap to path in settings is true dont move person outside of an area or a path
		if(moveSettings.snapToPath)
		{
			/*
			if(Diadrasis.Instance.useMapFilterForMovement)
			{
				#if UNITY_EDITOR
				Debug.LogWarning("TAP ON Filter Map !");
				#endif
				
				touchTarget.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);

				if(!IsTouchLocationValid()){
					#if UNITY_EDITOR
					Debug.LogWarning("TAP IS OUT OF GREEN AREA !");
					#endif
					return;
				}
				
			}
			*/


			#if UNITY_EDITOR
			Debug.LogWarning("check areas and paths nearest point");
			#endif

			if(Diadrasis.Instance.userPrin==Diadrasis.UserPrin.isNavigating)
			{
                //an einai ektos energis perioxis na brei to kontinotero monopati
                if (moveSettings.activeAreas.Count > 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("HAS AREAS");
#endif

                    Vector2 checkPos = new Vector2(personPos.x, personPos.z);


                    if (!gpsPosition.PlayerInsideArea(checkPos, moveSettings.activeAreas))
                    {

                        //an nekri perioxi
                        if (moveSettings.deadSpots.Count > 0)
                        {
                            if (gpsPosition.PlayerInsideDeadArea(checkPos, moveSettings.deadSpots))
                            {

#if UNITY_EDITOR
                                Debug.LogWarning("you are tapping on dead area !");
#endif
                                return;
                            }
                        }

#if UNITY_EDITOR
                        Debug.LogWarning("TAP OUT OF AN AREA");
#endif

                        float dX = 1000f;
                        float dY = 1000f;

                        //αν υπάρχει perimetros
                        if (moveSettings.activeAreasPerimetroi.Count > 0)
                        {
                            //και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
                            snapAreaPoint = gpsPosition.FindSnapPosition(checkPos, moveSettings.activeAreasPerimetroi);
                            dX = Vector2.Distance(checkPos, snapAreaPoint.position);
                        }
                        //αν υπάρχει ενεργό μονοπάτι επιτόπιας πλοήγησης απο το xml
                        if (moveSettings.playerPath.Count > 0)
                        {
                            //και βρίσκουμε το κοντινότρο σημείο πάνω στο μονοπάτι
                            snapPathPoint = gpsPosition.FindSnapPosition(checkPos, moveSettings.playerPath);
                            dY = Vector2.Distance(checkPos, snapPathPoint.position);
                        }

                        if (dX < dY)
                        {
                            snapPoint = snapAreaPoint;
                            personPos = new Vector3(snapPoint.position.x, gpsPosition.FindHeight(snapPoint.position), snapPoint.position.y);
                        }
                        else
                        if (dX > dY)
                        {
                            snapPoint = snapPathPoint;
                            personPos = new Vector3(snapPoint.position.x, gpsPosition.FindHeight(snapPoint.position), snapPoint.position.y);
                        }
                        else
                        if (dX == dY)
                        {
                            //move person to that position
                            personPos.x += moveSettings.posCenterOfMap.x;
                            personPos.z += moveSettings.posCenterOfMap.y;
                        }

#if UNITY_EDITOR
                        Debug.LogWarning("SNAP POINT = " + snapPoint.position);
#endif

						if (Diadrasis.Instance.isGameMode)
						{
							if (GameManager.Instance.isGameInstructionVisible)
							{
								GameManager.Instance.TapMapAction();
							}
						}

					}
                    else
                    {
                        //an nekri perioxi
                        if (moveSettings.deadSpots.Count > 0)
                        {
                            if (gpsPosition.PlayerInsideDeadArea(checkPos, moveSettings.deadSpots))
                            {

#if UNITY_EDITOR
                                Debug.LogWarning("you are tapping on dead area !");

#endif

                                //και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
                                snapPoint = gpsPosition.FindSnapPosition(checkPos, moveSettings.deadSpotsPerimetroi);
                                //							dX = Vector2.Distance(checkPos, snapAreaPoint.position);
                                personPos = new Vector3(snapPoint.position.x, gpsPosition.FindHeight(snapPoint.position), snapPoint.position.y);

								if (Diadrasis.Instance.isGameMode)
								{
									if (GameManager.Instance.isGameInstructionVisible)
									{
										GameManager.Instance.TapMapAction();
									}
								}

							}
                        }

#if UNITY_EDITOR
                        Debug.LogWarning("TAP INTO AN AREA");
#endif
                        if (Diadrasis.Instance.isGameMode)
                        {
                            if (GameManager.Instance.isGameInstructionVisible)
                            {
								GameManager.Instance.TapMapAction();
							}
                        }
                    }
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("NO AREAS!!!");
#endif
                }

                RaycastHit hit;
				
				//raycasts higher
				personPos.y = 500f;
				
				//hit down from last y position of player
				Ray downRay = new Ray(personPos, -Vector3.up);
				
				if (Physics.Raycast(downRay, out hit,Mathf.Infinity,rayLayer.value))
				{
					//get hit distance and add person height
					newPosY = (personPos.y - hit.distance) + moveSettings.playerHeight;		//Debug.Log("newPosY = "+newPosY);
				}
				personPos.y = newPosY;

			}else
				if(Diadrasis.Instance.moveOnAir && Diadrasis.Instance.userPrin==Diadrasis.UserPrin.onAir)
			{
				//an einai ektos energis perioxis na brei to kontinotero monopati
				if (moveSettings.onAirAreas.Count>0)
				{
					#if UNITY_EDITOR
					Debug.LogWarning("HAS AIR AREAS");
					#endif
					
					Vector2 checkPos = new Vector2(personPos.x,personPos.z);
					
					//an patisei ektos perioxis 
					//move him to perimetro
					if(!gpsPosition.PlayerInsideArea(checkPos,moveSettings.onAirAreas))
					{
						
						#if UNITY_EDITOR
						Debug.LogWarning("TAP OUT OF AN AREA");
						#endif
						
						//float dX=1000f;
						//float dY=1000f;
						
						//αν υπάρχει perimetros
						if(moveSettings.activeAreasOnAirPerimetroi.Count>0){
							//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
							snapAreaPoint=gpsPosition.FindSnapPosition(checkPos,moveSettings.activeAreasOnAirPerimetroi);
							//dX = Vector2.Distance(checkPos, snapAreaPoint.position);
							snapPoint = snapAreaPoint;
							personPos = new Vector3(snapPoint.position.x, moveSettings.personOnAirAltitude, snapPoint.position.y);
						}
						
						#if UNITY_EDITOR
						Debug.LogWarning("SNAP POINT = "+snapPoint.position);
						#endif
						
					}else{
												
						#if UNITY_EDITOR
						Debug.LogWarning("TAP INTO AN AREA");
						#endif
					}
				}else{
					#if UNITY_EDITOR
					Debug.LogWarning("NO AREAS!!!");
					#endif
				}
			}
		}

		targets.person.position = personPos;

//		#if UNITY_EDITOR
//		Debug.Log("Person Pos = "+personPos);
//		#endif

		//fast down
		AmesiMetatopisi(personPos);
		targets.person.SendMessage("Ypsos",true,SendMessageOptions.DontRequireReceiver);

		//if tablet
		if(Diadrasis.Instance.screenSize<2){
			Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[1];
		}else{
			Diadrasis.Instance.menuUI.btnsMenu.imgPersonMoveUpDown.overrideSprite = Diadrasis.Instance.menuUI.btnsMenu.btnUpDownSprites[3];
		}
		//slow down
//		StopCoroutine("ptosi");
//		StartCoroutine("ptosi");
			tapActions=0;
//		}
		//enable menu button
		menuUI.MenuButtonActive();

		menuUI.animControl.animMap.enabled=true;
		
		//close map
		animControl.ShowSmallMap();

		if(OnFullMapTap!=null){
			OnFullMapTap();
		}

		fullMapHelp.SetActive(false);
	}

	bool isTheFirstTime = true;

	void showBuyMessage_A(){

		#if UNITY_EDITOR
		Debug.LogWarning("SHOW BUY Message!!!");
		#endif

		menuUI.helpScript.CloseHelps ();
		menuUI.animControl.CloseRadialMenu();
	}

	void showBuyMessage_B(){

		#if UNITY_EDITOR
		Debug.LogWarning("SHOW BUY Message!!!");
		#endif

		menuUI.warningsUI.ShowBuyFirstMessage();

		CancelInvoke ();
	}

	void showHelpFirstTime_A(){

		#if UNITY_EDITOR
		Debug.LogWarning("SHOW HELP 1st time A Message!!!");
		#endif

		Diadrasis.Instance.animControl.MenuButton ();

	}
	void showHelpFirstTime_B(){

		#if UNITY_EDITOR
		Debug.LogWarning("SHOW HELP 1st time B Message!!!");
		#endif

		Diadrasis.Instance.menuUI.HelpShow(true);

	}

	void AmesiMetatopisi(Vector3 pos)
	{
		targets.person.position = pos;
		
		//Debug.Log("PERSON IS DOWN !!");
		if(!Diadrasis.Instance.moveOnAir){
			Diadrasis.Instance.ChangeStatus(Diadrasis.User.isNavigating);
		}else{
			Diadrasis.Instance.CheckUserPrin();
		}

		if(Diadrasis.Instance.appEntrances==1 && Diadrasis.Instance.isStart==1 && isTheFirstTime){
			isTheFirstTime = false;
			//menuUI.HelpShow();
			Invoke ("showHelpFirstTime_A", 1f);
			Invoke ("showHelpFirstTime_B", 1.7f);
//			Invoke ("showBuyMessage_A", 12f);
//			Invoke ("showBuyMessage_B", 15f);
		}
		
		personFullMap.gameObject.SetActive(false);
		personFullVelaki.gameObject.SetActive(false);
	}

	#endregion

	#region MAP PIVOT - RESIZE FUNCTIONS

	public void FullMapCenter()
	{	
		animControl.ShowFullMap();

		//close animator to make manual scale
		animControl.animMap.enabled=false;

		UItool.SetAnchor(mapContainer,UItool.Thesi.center);

		mapRect.localPosition= Diadrasis.Instance.mapFullPosition;//new Vector2(155f,-94f);

		mapRect.sizeDelta = Diadrasis.Instance.mapFullZoom;//new Vector2(mapTexture.width, mapTexture.height);

		//maximaize map to screen resolution
		mapContainer.sizeDelta = Diadrasis.Instance.canvasMainRT.sizeDelta;

		mapContainer.GetComponent<Image> ().fillAmount = 1f;

		mapContainer.localPosition = Vector2.zero;

		mapRect.gameObject.SetActive (true);
	}


	/// <summary>
	/// stretch map to device screen size
	/// </summary>
	IEnumerator mapFullScreenSizing()
	{
//		Debug.Log("mapFullScreenSizing");
		yield return new WaitForSeconds(0.7f);
		//disable animator
		if(Diadrasis.Instance.user==Diadrasis.User.inFullMap){
			//close animator to make manual scale
			animControl.animMap.enabled=false;
			//maximaize map to screen resolution
			mapContainer.sizeDelta = Diadrasis.Instance.canvasMainRT.sizeDelta;
		}

	}

	#endregion

	#region MAP ZOOM

	/// <summary>
	/// Metafora world position to map position
	/// </summary>
	public Vector2 zoomLogos(){
		//get map container width/height
		mapX = mapRect.sizeDelta.x;
		mapY = mapRect.sizeDelta.y;

		//divide map size with terrain size
		stathera = new Vector2(mapX / moveSettings.terrainSize.x , mapY / moveSettings.terrainSize.y);
		
		return stathera;
	}


	Vector2 zoomLevel = new Vector2(200f,200f);
	Vector2 zoomLimitsY = new Vector2(1100f,3000f);
	Vector2 zoomLimitsX = new Vector2(1100f,3000f);
	float clampX,clampY;

	void Zoom(bool isClose){	Debug.Log("ZOOM");

		if(isClose && mapRect.sizeDelta.y!=zoomLimitsY.y){

			clampY = mapRect.sizeDelta.y + zoomLevel.y;
			clampX = mapRect.sizeDelta.x + zoomLevel.x;// * (mapImage.sizeDelta.y/zoomLimitsY.x); 

			//zoom - IN
			mapRect.sizeDelta = new Vector2(clampX, clampY);// mapImage.sizeDelta + zoomLevel;

//			Debug.Log("zoom in = "+mapImage.sizeDelta);
		}else
		if(!isClose && mapRect.sizeDelta.x!=zoomLimitsY.x)
		{
			clampY = mapRect.sizeDelta.y - zoomLevel.y;
			clampX = mapRect.sizeDelta.x - zoomLevel.x;//* (zoomLimitsY.x/mapImage.sizeDelta.y);
			//zoom - Out
			mapRect.sizeDelta = new Vector2(clampX, clampY);//mapImage.sizeDelta - zoomLevel;

//			Debug.Log("zoom out = "+mapImage.sizeDelta);
		}

		//scale or resize map with min/max limits
		clampY = Mathf.Clamp(mapRect.sizeDelta.y,zoomLimitsY.x,zoomLimitsY.y);
		clampX = Mathf.Clamp(mapRect.sizeDelta.x,zoomLimitsX.x,zoomLimitsX.y);

		mapRect.sizeDelta = new Vector2(clampX, clampY);

//		analogiaTerrainEikoneas = zoomLogos ();

		//ON/OFF Buttons and colors if a limit is reached
		if(mapRect.sizeDelta.y==zoomLimitsY.x){btnsMap.zoomOut.GetComponent<Image>().color=Color.black; btnsMap.zoomOut.interactable=false;}else{btnsMap.zoomOut.GetComponent<Image>().color=Color.white; btnsMap.zoomOut.interactable=true;}
		if(mapRect.sizeDelta.y==zoomLimitsY.y){btnsMap.zoomIn.GetComponent<Image>().color=Color.black;	btnsMap.zoomIn.interactable=false;}else{btnsMap.zoomIn.GetComponent<Image>().color=Color.white;	btnsMap.zoomIn.interactable=true;}
	}

	#endregion

	#region BETA TEST
	/*

	List<Vector2> testPos = new List<Vector2>();
	PointerEventData _eventDataCurrentPosition ;

	float ColorSqrDistance(Color c1, Color c2) {
		return ((c2.r - c1.r) * (c2.r - c1.r) + (c2.b - c1.b) * (c2.b - c1.b) + (c2.g - c1.g) * (c2.g - c1.g));
	}
	
	void GetArea(){
		List<Color> xromata = new List<Color> ();

		Texture2D tex = mapFilter.sprite.texture;// Tools_Load.LoadTexture(appSettings.mapImagePath, "mapOttomanArea");

		Debug.LogWarning("filter texture = "+tex.width+" X "+tex.height);
		Debug.LogWarning("map texture = "+mapTexture.width+" X "+mapTexture.height);
		Debug.LogWarning("mapFilterRect.rect.width = "+mapFilterRect.rect.width);
		Debug.LogWarning("mapFilterRect.rect.height = "+mapFilterRect.rect.height);
		Debug.LogWarning("mapFilterRect.rect.min = "+mapFilterRect.rect.min);

		return;
		if(!tex){return;}

		testPos.Clear();

		for (int x = 0; x < tex.width; x++) 
		{
			for (int y = 0; y < tex.height; y++) 
			{
				Color c = tex.GetPixel (x, y);
				if (c.a > 0.1f)
				{
					testPos.Add(new Vector2(x,y));
				}
			}
		}

		Debug.Log(testPos.Count);

		GameObject tt = new GameObject("SQUARE_AREA");
		tt.transform.position = Vector3.zero;
		
		for(int g=0; g<testPos.Count; g++)
		{
			Debug.Log("testPos "+g+" = "+testPos[g]);

			float posX = testPos[g].x;
			float posY = testPos[g].y;

			float normalPointX = posX / tex.width;// - (tex.width/2f);
			float normalPointY = posY /tex.height;// - (tex.height/2f);

			normalPointX *= mapFilterRect.rect.width;
			normalPointY *= mapFilterRect.rect.height;

			Vector2 normPoint = new Vector2(normalPointX, normalPointY);

			Debug.Log("normPoint = "+normPoint);

//			normPoint.x = 

			Vector2 rectPoint = normPoint + mapFilterRect.rect.min;

			Debug.Log("rectPoint = "+rectPoint);

			GameObject gb2 = new GameObject("UI BLACK DOT "+g);
			gb2.transform.parent = mapFilterRect.transform;
			gb2.transform.localPosition = rectPoint;

//			touchTarget.localPosition = rectPoint;
//			touchTarget.localPosition = new Vector3(personPos.x * zoomLogos ().x , personPos.z * zoomLogos ().y , 0f);
//			Vector3 kk = touchTarget.TransformPoint(rectPoint);

//			RectTransformUtility.WorldToScreenPoint(null, corner)

//			_eventDataCurrentPosition = new PointerEventData(EventSystem.current);
//			_eventDataCurrentPosition.pressPosition = normPoint;

//			if(Tools_UI.IsPointerOverUIObject())
//			{

				//get tap position on map image only
//				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRect, _eventDataCurrentPosition.pressPosition, _eventDataCurrentPosition.pressEventCamera, out localCursor))//(mapTapFilter, tap.position, tap.pressEventCamera, out localCursor))
//					return;

//			Vector2 p = RectTransformUtility.WorldToScreenPoint(null, 

//			Vector3[] corners = new Vector3[4];
//			mapRect.GetWorldCorners(corners);
//			Debug.Log("Screen point1: " + new Vector3(mapRect.rect.xMax, mapRect.rect.yMin, 0) + mapRect.position);
//			foreach (Vector3 corner in corners) {
//				Debug.Log("World point: " + corner);
//				Debug.Log("Screen point: " + RectTransformUtility.WorldToScreenPoint(null, corner));
//				Debug.Log("Viewport: " + Camera.main.ScreenToViewportPoint(corner));
//			}
			
			//get person position
				Vector3 blackPos = Vector3.zero;

//			rectPoint.x += moveSettings.posCenterOfMap.x;
//			rectPoint.y += moveSettings.posCenterOfMap.y;


				//calculate 2d pos to world position in scene
			blackPos.x = rectPoint.x / (zoomLogos ().x /2f);
				blackPos.y = 30f;
			blackPos.z = rectPoint.y / (zoomLogos ().y/2f) ;

			blackPos.x += moveSettings.posCenterOfMap.x;
			blackPos.z += moveSettings.posCenterOfMap.y;

				Debug.Log(blackPos);

			if(g==0){
				GameObject gb = new GameObject("start");

				gb.transform.parent = tt.transform;
				gb.transform.localPosition = Vector3.zero;

				gb.transform.position = blackPos;
			}else{
				GameObject gb = new GameObject("point");

				gb.transform.parent = tt.transform;
				gb.transform.localPosition = Vector3.zero;

				gb.transform.position = blackPos;
			}
		}
	}
	*/
	
	#endregion
	
}
