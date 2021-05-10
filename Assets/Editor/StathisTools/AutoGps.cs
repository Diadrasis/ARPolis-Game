using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class AutoGps : EditorWindow {

	Vector2 selectedObjectGpsCoordiness;
	float earthRadius = 6371.0f;
	GameObject targetObject, centerOfScene, kameraMap, myTerrain;
	float targetLon, targetLat;
	bool showScreenShot, showTerrain, isKamSets;
	Camera myKam;
	int mySize = 2;

	string terrainName = "#####__MAP_TERRAIN_#####";

	Vector2 dotUpScenePos, dotUpGpsPos, dotDownScenePos, dotDownGpsPos, dotCenter, mapCenterPos;
	bool showResults;

	// Add menu named "My Window" to the Window menu
	[MenuItem ("Stathis/Scene Map and GPS data")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		AutoGps window = (AutoGps)EditorWindow.GetWindow (typeof (AutoGps));
		window.Show();
	}

	void OnInspectorUpdate() {
		Repaint();
	}

	bool page1, page2, page3, page4;

	private static Texture2D tex;

	void OnEnable(){
		SetTexture ();
	}

	void SetTexture(){
		sceneCurrentName = GetSceneName ();
		tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		tex.SetPixel(0, 0, Color.black);
		tex.Apply();
	}

	string sceneCurrentName;

	void Reset(){
		page1 = false;
		page2 = false;
		page3 = false;
		page4 = false;
		targetObject = null;
		targetLat = 0f;
		targetLon = 0f;
	}

	void OnGUI () 
	{

		//background color
		if (tex == null) {
			SetTexture ();
		}
		GUI.DrawTexture (new Rect (0, 0, maxSize.x, maxSize.y), tex, ScaleMode.StretchToFill);

		GUILayout.Label (copyright, EditorStyles.toolbarButton);
		GUILayout.Label ("Όνομα σκηνής ( "+sceneCurrentName+" )", EditorStyles.toolbarTextField);

		//button reset
		if (targetObject) {
			if (GUILayout.Button ("Επαναφορά")) {
				if (EditorUtility.DisplayDialog ("Θα χαθούν όλα τα δεδομένα που έχετε καταχωρίσει!", "Είστε σίγουρος;", "Εντάξει", "Άκυρο")) {
					Reset ();
				}
			}
			GUILayout.Label ("", EditorStyles.boldLabel);
		}
		//button reset
		if (targetObject) {
			if (GUILayout.Button ("Επιστροφή")) {
//				if (EditorUtility.DisplayDialog ("Θα χαθούν όλα τα δεδομένα που έχετε καταχωρίσει!", "Είστε σίγουρος;", "Εντάξει", "Άκυρο")) {
					Back ();
//				}
			}
			GUILayout.Label ("", EditorStyles.boldLabel);
		}

		if(!targetObject)
		{
			GUI.contentColor = Color.white;
			GUILayout.Label ("Βήμα 1ο:", EditorStyles.boldLabel);
			GUI.contentColor = Color.yellow;
			GUILayout.Label ("Διαλέξτε ένα αντικείμενο από την σκηνή.", EditorStyles.boldLabel);
			GUILayout.Label ("Προσοχή !! Η θέση του να μην είναι κοντά\nστο κέντρο των αξόνων (0,0)", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
			GUILayout.Label ("", EditorStyles.boldLabel);
	//		GUILayout.Label ("Το αντικειμενο με τα GPS δεδομενα", EditorStyles.boldLabel);
			targetObject = (GameObject) EditorGUILayout.ObjectField ("Target Object", targetObject, typeof(GameObject), true);// (new Rect(3, 60, position.width - 6, 17), , targetObject, typeof(GameObject),true);

//			return;
		}

		if (targetObject && !page1) {

			Vector3 posT = targetObject.transform.position;

			if ((posT.x < 0.1f && posT.x > -0.1f) || (posT.z < 0.1f && posT.z > -0.1f)) {
				targetObject = null;

				if(EditorUtility.DisplayDialog("Η θέση του είναι πολύ κοντά στο 0,0 !!", "Παρακαλώ επιλέξτε άλλο αντικείμενο", "Εντάξει")){}
			}else{
				page1 = true;
			}
		}

		#region PAGE 1

		if (page1 && !page2) {
			GUI.contentColor = Color.white;
			GUILayout.Label ("Βήμα 2ο:", EditorStyles.boldLabel);
			GUI.contentColor = Color.yellow;
			GUILayout.Label ("Επιλεγμένο αντικείμενο( "+targetObject.name+" )", EditorStyles.boldLabel);
			GUILayout.Label ("Συμπληρώστε της πραγματικές συντεταγμένες του.", EditorStyles.boldLabel);
			GUILayout.Label ("", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
			targetLat = EditorGUILayout.FloatField("Latitude", targetLat);
			targetLon = EditorGUILayout.FloatField("Longitute", targetLon);
			GUILayout.Label ("", EditorStyles.boldLabel);

			if (!page2) {
				if (targetObject) {
					if (targetLat != 0f && targetLat>34.5f && targetLat<41.5f && targetLon != 0f && targetLon>19.0f && targetLon<27f) {
						if (GUILayout.Button ("Next >>>")) {
							page2 = true;
						}
					}else{
						GUI.contentColor = Color.red;
						GUILayout.Label ("Οι συντεταγμένες δεν είναι σωστές ή είναι ελλιπείς...", EditorStyles.boldLabel);
					}
				} else {
					if (EditorUtility.DisplayDialog ("Missing target Object!!", "Set a target first", "OK")) {
						Reset ();
					}
				}
			}
		}

		#endregion

		if(showResults)
		{
			GUILayout.Label ("", EditorStyles.boldLabel);
			dotCenter = EditorGUILayout.Vector2Field("Gps Center reff", dotCenter);
			GUILayout.Label ("", EditorStyles.boldLabel);
			dotUpScenePos = EditorGUILayout.Vector2Field("Dot Up Left", dotUpScenePos);
			dotUpGpsPos = EditorGUILayout.Vector2Field("GPS Reff A", dotUpGpsPos);
			GUILayout.Label ("", EditorStyles.boldLabel);
			dotDownScenePos = EditorGUILayout.Vector2Field("Dot Down Right", dotDownScenePos);
			dotDownGpsPos = EditorGUILayout.Vector2Field("GPS Reff B", dotDownGpsPos);
		}



//		GUILayout.Label ("Type the name of xml to import areas into the scene", EditorStyles.boldLabel);
//		myXml = EditorGUILayout.TextField ("name of xml", myXml);

		#region PAGE 2

		if (page2 && !page3) {
			GUI.contentColor = Color.white;
			GUILayout.Label ("Βήμα 3ο:", EditorStyles.boldLabel);
			GUI.contentColor = Color.yellow;
			GUILayout.Label ("Υπολογισμός συντεταγμένων του κέντρου της σκηνής.", EditorStyles.boldLabel);
			GUILayout.Label ("", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
			if (GUILayout.Button ("Υπολογισμός χωρίς δημιουργία χάρτη")) {
				if (targetObject) {
					if (targetLat != 37f && targetLat != 38f && targetLon != 23f && targetLon != 24f) {
						DoCalculations ();
					} else {
						if (EditorUtility.DisplayDialog ("Λάθος συντεταγμένες..", "Συμπληρώστε τις σωστές συντεταγμένες", "Εντάξει")) {

						}
					}

				}
			}
			if (GUILayout.Button ("Υπολογισμός με δημιουργία χάρτη")) {
				if (targetObject) {
					if (targetLat != 37f && targetLat != 38f && targetLon != 23f && targetLon != 24f) {
						page3=true;
					} else {
						if (EditorUtility.DisplayDialog ("Λάθος συντεταγμένες..", "Συμπληρώστε τις σωστές συντεταγμένες", "Εντάξει")) {
							
						}
					}
					
				}
			}


		}

		#endregion


//		showScreenShot =  page3;
		
		if(page3)
		{
			GUI.contentColor = Color.white;
			GUILayout.Label ("Βήμα 4ο:", EditorStyles.boldLabel);
			GUI.contentColor = Color.yellow;
			GUILayout.Label ("Λήψη στιγμιότυπου χάρτη της σκηνής ( "+sceneCurrentName+" )", EditorStyles.boldLabel);
			GUILayout.Label ("", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
			if(!kameraMap){
				kameraMap=GameObject.Find("Camera_ScreenShot");
				
				if(kameraMap && !myKam){
					myKam=kameraMap.GetComponent<Camera>();
				}
			}
			
			if(!myKam){
				if(GUILayout.Button("Δημιούργια κάμερας")){
					SetCamera();
					if(EditorUtility.DisplayDialog("Βοήθεια", "Μετακινήστε την κάμερα (Camera_ScreenShot) στο σημείο που επιθυμείτε.", "Εντάξει")){
						
					}
				}
			}else{
				mySize = EditorGUILayout.IntSlider ("Size of png", mySize, 1, 8);
//				GUILayout.Label ("", EditorStyles.boldLabel);
//				if(GUILayout.Button("Orthographic")){
//					SetOrtho();
//				}
//				//	GUILayout.Label ("", EditorStyles.boldLabel);
//				if(GUILayout.Button("Perspective")){
//					SetPersp();
//				}
				GUILayout.Label ("", EditorStyles.boldLabel);
				
				showTerrain = GUILayout.Toggle(showTerrain,"Δημιούργησε και το Terrain");
				
				if(GUILayout.Button("Capture")){
					if(mySize>4){
						if(EditorUtility.DisplayDialog("Warning", "The size of png is big . That means that it needs more time to capture", "OK" , "Cancel")){
							Screenshot();
						}
					}else{
						Screenshot();
					}
				}

//				if(myCurrentMap) {
////					EditorGUI.PrefixLabel(Rect(25,45,100,15),0,GUIContent("Preview:"));
//					EditorGUI.DrawPreviewTexture(Rect(25,60,100,100),myCurrentMap);
//					if(GUI.Button(Rect(3,position.height - 25, position.width-6,20),"Clear texture")) {
//						myCurrentMap = EditorGUIUtility.whiteTexture;
//					}
//				}
			}
		}
	}

	void Back(){
		if(page1 && page2 && page3){
			page3=false;
		}else
		if(page1 && page2){
			page2=false;
		}else
		if(page1){
			page1=false;
			targetObject=null;
		}

	}

	Texture2D myCurrentMap;

	void DoCalculations(){
		showResults = false;
		//gps dat of target
		Vector2 gpsTarget = new Vector2(targetLat, targetLon);

		mapCenterPos = Vector2.zero;// centerOfScene.transform.position;

		if(!kameraMap){
			kameraMap=GameObject.Find("Camera_ScreenShot");
			
			if(kameraMap && !myKam){
				myKam=kameraMap.GetComponent<Camera>();
			}
		}

		if(kameraMap){
			mapCenterPos = new Vector2(kameraMap.transform.position.x, kameraMap.transform.position.z);
		}

		//avoid calculations with zero
		if(mapCenterPos.x==0){mapCenterPos.x=0.01f;}//1cm
		if(mapCenterPos.y==0){mapCenterPos.y=0.01f;}//1cm

		Debug.Log("center pos = "+mapCenterPos);
		
		//get distance from selected object to center of map
		Vector3 posTarget = targetObject.transform.position;
		Vector2 posTarget2d = new Vector2(posTarget.x, posTarget.z); 


//		Debug.Log("target pos = "+posTarget2d);

		float pivotX = 0.5f - (posTarget2d.x / 1000f);
		float pivotY = 0.5f + (posTarget2d.y / 1000f);

//		Debug.Log ("map pivot = "+pivotX+" , "+pivotY);



		float distInScene = Vector2.Distance(mapCenterPos, posTarget2d);

		distInScene = distInScene / 1000f;

		if (distInScene >= 1f) {
			//meters to km
			Debug.Log ("Απόσταση απο το κέντρο της σκηνής = " + distInScene + " km");
		} else {
			Debug.Log ("Απόσταση απο το κέντρο της σκηνής = " + (distInScene * 1000f).ToString()+ " m");
		}
			
		if(distInScene>0){
			//calculate angle from selected object to cenetr of map

			//angle side direction 
			Vector2 dirA = mapCenterPos - posTarget2d ;//-centerPos;

			float angle = Vector2Angle(dirA);

			Debug.Log("dir angle = "+ angle);

			Vector2 syntetagmenesOfCenter = coordinateFromCoord(gpsTarget, distInScene, angle);

			if(kameraMap){
				GameObject terrain = GameObject.Find(terrainName);
				
				if(terrain){
					GoogleMap goo = terrain.AddComponent<GoogleMap>();
					
					goo.loadOnStart=true;
					goo.zoom = 17;

					goo.doubleResolution = true;

					Vector3 sizeOfTerrain = terrain.transform.localScale;
//					goo.size = new Vector2(sizeOfTerrain.x, sizeOfTerrain.z);
					goo.size.x = Mathf.RoundToInt (sizeOfTerrain.x);
					goo.size.y = Mathf.RoundToInt (sizeOfTerrain.z);

					GoogleMapLocation mapLoc = new GoogleMapLocation();
					mapLoc.latitude = syntetagmenesOfCenter.x;
					mapLoc.longitude = syntetagmenesOfCenter.y;
					goo.centerLocation = mapLoc;

					goo.mapType=GoogleMap.MapType.Satellite;

					goo.autoLocateCenter=false;

					goo.markers=new List<GoogleMapMarker>();

					GoogleMapMarker markerA = new GoogleMapMarker();
					markerA.color=GoogleMapColor.yellow;
					markerA.label = targetObject.name;
					goo.markers.Add(markerA);

					markerA.locations = new List<GoogleMapLocation>();

					GoogleMapLocation location = new GoogleMapLocation();
					location.latitude = targetLat;
					location.longitude = targetLon;
					markerA.locations.Add(location);

					GameObject[] etiketes = GameObject.FindGameObjectsWithTag("etiketa");

					Debug.Log("etiketes = "+etiketes.Length);

					if(etiketes.Length>0){
						foreach(GameObject gb in etiketes){

							Vector3 pos = gb.transform.position;
							Vector2 gpsPos = getGpsData(new Vector2(pos.x, pos.z));

							GoogleMapMarker mark = new GoogleMapMarker();
							mark.color=GoogleMapColor.red;
							mark.label = gb.name;
							goo.markers.Add(mark);
							
							mark.locations = new List<GoogleMapLocation>();
							
							GoogleMapLocation loc = new GoogleMapLocation();
							loc.latitude = gpsPos.x;
							loc.longitude = gpsPos.y;
							mark.locations.Add(loc);
						}
					}


					Vector3 euler = terrain.transform.eulerAngles;
					euler.y = 180f;

					terrain.transform.eulerAngles = euler;
				}
			}

			Debug.Log("Συντεταγμένες κέντρου χαρτη ( Lat = "+syntetagmenesOfCenter.x+" / Lon = "+syntetagmenesOfCenter.y+" )");

		}else{
			Vector2 syntetagmenesOfCenter = gpsTarget;
			
			Debug.Log("Συντεταγμένες κέντρου χαρτη ( Lat = "+syntetagmenesOfCenter.x+" / Lon = "+syntetagmenesOfCenter.y+" )");
		}

	}


	Vector2 getGpsData(Vector2 scenePos){
		Vector2 syntetagmenesOfNewTarget=Vector2.zero;
		//gps dat of target
		Vector2 gpsTarget = new Vector2(targetLat, targetLon);
		
		Vector2 centerPos = scenePos;// centerOfScene.transform.position;
		
		//avoid calculations with zero
		if(centerPos.x==0){centerPos.x=0.1f;}//10cm
		if(centerPos.y==0){centerPos.y=0.1f;}//10cm
		
		Debug.Log("new target pos = "+centerPos);
		
		//get distance from selected object to center of map
		Vector3 posTarget = targetObject.transform.position;
		Vector2 posTarget2d = new Vector2(posTarget.x, posTarget.z); 
		
		
		
		Debug.Log("target pos = "+posTarget2d);
		
		float distInScene = Vector2.Distance(centerPos, posTarget2d);
		//meters to km
		distInScene = distInScene/1000f;
		
		Debug.Log("dist = "+distInScene+" km");

		if(distInScene>0){
			//calculate angle from selected object to cenetr of map
			
			//angle side direction 
			Vector2 dirA = centerPos - posTarget2d ;//-centerPos;
			
			float angle = Vector2Angle(dirA);
			
			Debug.Log("dir angle = "+ angle);
			
			syntetagmenesOfNewTarget = coordinateFromCoord(gpsTarget, distInScene, angle);

			Debug.Log("new target coordinates ( Lat = "+syntetagmenesOfNewTarget.x+" / Lon = "+syntetagmenesOfNewTarget.y+" )");
		}

		return syntetagmenesOfNewTarget;
	}



	#region math simple convertions

	//Convert Vector2 to angle in degrees (0-360)
	float Vector2Angle(Vector2 v){
		if(v.x<0) return 360-(Mathf.Atan2(v.x, v.y)*Mathf.Rad2Deg*-1);
		else return Mathf.Atan2(v.x,v.y)*Mathf.Rad2Deg;
	}
	
	//Convert degrees (0-360) to a Vector2
	Vector2 Angle2Vector(float angle,float power){
		return (new Vector2(Mathf.Sin(angle*Mathf.Deg2Rad),Mathf.Cos(angle*Mathf.Deg2Rad)))*power;
	}

	float Angle(Vector2 p_vector2)
	{
		if (p_vector2.x < 0)
		{
			return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
		}
		else
		{
			return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
		}
	}

	#endregion


	#region create terrain and google map

	void SetPersp(){
		if(myKam){
			myKam.orthographic=false;
			myKam.transform.eulerAngles=new Vector3(90f,0,0f);
		}
	}
	
	void SetOrtho(){
		if(myKam){
			myKam.orthographic=true;
			myKam.orthographicSize=-1046f;
			myKam.farClipPlane=1000f;
			myKam.transform.eulerAngles=new Vector3(90f,-180f,0f);
		}
	}

	DirectoryInfo folder,dateFolder;
	Vector2 sizeTerrain;

	void Screenshot(){
		if(myKam){
			
			float ypsos = FindHeight(myKam.transform.position);
			sizeTerrain = GetTerrainSize(ypsos, myKam);
			
			string a = "Map_ScreenShots";
			
			try
			{
				if (!Directory.Exists(a))
				{
					Directory.CreateDirectory(a);
				}
				
			}
			catch (IOException ex)
			{
				Debug.LogWarning(ex.Message);
			}
			
			string b = "/"+System.DateTime.Now.ToString("yyMMdd");
			
			try
			{
				if (!Directory.Exists(a+b))
				{
					Directory.CreateDirectory(a+b);
				}
				
			}
			catch (IOException ex)
			{
				Debug.LogWarning(ex.Message);
			}
			
			string sceneName = GetSceneName();
			
			string mapName = a+ b+ "/Map_"+sceneName+"_"+sizeTerrain.x+"_x_"+sizeTerrain.y+"_"+System.DateTime.Now.ToString("HH.mm.ss")+".jpg";


			if (myTerrain) {
				myTerrain.SetActive (false);
			}

			Application.CaptureScreenshot(mapName,mySize);


			
			if(EditorUtility.DisplayDialog("Ο υπολογισμός ολοκληρώθηκε.", "Μέγεθος terrain που καλύπτει ο χάρτης = "+sizeTerrain.x.ToString()+" x "+sizeTerrain.y.ToString(), "Άνοιγμα φακέλου αποθήκευσης", "Μην ανοίξεις το φάκελο")){
				string t = Application.dataPath;

				t=t.Substring(0, t.Length-6);

				myCurrentMap = Stathis.Tools_Load.LoadTexture(t+a,b);
				

				Application.OpenURL(t+a+b);
			}


			
		}else{
			Debug.Log("No Camera Found!");
			Debug.Log("Set new Camera");
		}
	}
	
	string GetSceneName(){
		#if UNITY_5
		return SceneManager.GetActiveScene().name;
		#endif
		
		string[] strScenePathSplit = EditorApplication.currentScene.Split('/');
		string[] strFileWithExtension =
			strScenePathSplit[strScenePathSplit.Length - 1].Split('.');
		string sceneName = strFileWithExtension[0];
		if( sceneName == null ){
			sceneName = "Scene name not Available";
			Debug.LogWarning("Scene name not Available");
		}
		return sceneName;
	}
	
	void SetCamera(){
		GameObject g = new GameObject();
		g.name="Camera_ScreenShot";
		g.tag="MainCamera";
		g.transform.position=new Vector3(0f,777f,0f);
		g.transform.eulerAngles=new Vector3(90f,0f,0f);
		g.AddComponent<Camera>();
		myKam=g.GetComponent<Camera>();
		myKam.useOcclusionCulling=false;
		//ignore terrain layer from photo
//		myKam.cullingMask &= ~(1 << LayerMask.NameToLayer("terrain"));
		isKamSets=true;
		Debug.Log("Move camera to take screenshot");
	}

//	// Render everything *except* layer 14
//	camera.cullingMask = ~(1 << 14);
//
//	// Switch off layer 14, leave others as-is
//	camera.cullingMask &= ~(1 << 14);
//
//	// Switch on layer 14, leave others as-is
//	camera.cullingMask |= (1 << 14);

	float FindHeight(Vector3 pos){
		
		//declare 2 values
		float rayHeight=0f;
		float terrainHeight=0f;
		
		RaycastHit hit;
		
		//hit down from last y position of player
		Ray downRay = new Ray(new Vector3(pos.x,pos.y,pos.z), -Vector3.up);
		
		if (Physics.Raycast(downRay, out hit,Mathf.Infinity)){
			//get hit distance and add person height
			rayHeight =  hit.distance;
		}
		
		//		#if UNITY_EDITOR
		//		Debug.Log("rayHeight = "+rayHeight);
		//		#endif
		
		return rayHeight;
	}	

	
	Vector2 GetTerrainSize(float height, Camera kamera){
		//		camera.farClipPlane = person.transform.position.y+10f;
		Vector2 mySizeOfTerrain=Vector2.zero;
		float diffHeight = height;


		if(kamera.isOrthoGraphic){
			mySizeOfTerrain = new Vector2(kamera.orthographicSize*2f, kamera.orthographicSize*2f);

			if(showTerrain){
				myTerrain = GameObject.CreatePrimitive(PrimitiveType.Cube);
				
				myTerrain.name = terrainName;
				
				myTerrain.transform.localScale = new Vector3(mySizeOfTerrain.x, 1f, mySizeOfTerrain.y);
				Vector3 posOria = kamera.transform.position;
				posOria.y -= diffHeight; //person.transform.position.y;
				myTerrain.transform.position = posOria;
				
				GameObject fylakasUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
				fylakasUp.name = "fylakasUp";
				fylakasUp.transform.parent = myTerrain.transform;
				fylakasUp.transform.localPosition = new Vector3(-0.5f, 0f, 0.5f);
				
				GameObject fylakasDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
				fylakasDown.name = "fylakasDown";
				fylakasDown.transform.parent = myTerrain.transform;
				fylakasDown.transform.localPosition = new Vector3(0.5f, 0f, -0.5f);

				Vector3 upV = fylakasUp.transform.position;
				Vector3 downV = fylakasDown.transform.position;
				
				//the center of map image
				dotCenter = getGpsData(new Vector2(posOria.x, posOria.z));	//getGpsData(new Vector2(0.1f,0.1f));
				
				//up left corner of map
				dotUpScenePos = new Vector2(upV.x, upV.z);
				dotUpGpsPos = getGpsData(dotUpScenePos);
				
				//down right corner of map
				dotDownScenePos = new Vector2(downV.x, downV.z);
				dotDownGpsPos = getGpsData(dotDownScenePos);
				
				//map terrain size
				sizeTerrain = mySizeOfTerrain;
				
				ExportDotVectors();
				
			}
		}else{
			
			
			kamera.farClipPlane=diffHeight + 150f;
			
			//		float frustumHeight = 2f * person.transform.position.y * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			float frustumHeight = 2f * diffHeight * Mathf.Tan(kamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			
			float frustumWidth = frustumHeight * kamera.aspect;
			
			Debug.LogWarning("Terrain Size = "+frustumWidth+" x "+frustumHeight);
			
			
			if(showTerrain){
				myTerrain = GameObject.CreatePrimitive(PrimitiveType.Cube);

				myTerrain.name = terrainName;
				
				myTerrain.transform.localScale = new Vector3(frustumWidth, 1f, frustumHeight);
				Vector3 posOria = kamera.transform.position;
				posOria.y -= diffHeight; //person.transform.position.y;
				myTerrain.transform.position = posOria;

				var frustumHeight2 = frustumWidth / kamera.aspect;
		
				Vector3 posFylaksDown = new Vector3(posOria.x + frustumWidth/2f ,posOria.y , posOria.z - frustumHeight2/2f );
				Vector3 posFylaksUp = new Vector3(posOria.x - frustumWidth/2f ,posOria.y , posOria.z + frustumHeight2/2f );

				GameObject fylakasUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
				fylakasUp.name = "fylakasUp";
				fylakasUp.transform.position = posFylaksUp;

				GameObject fylakasDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
				fylakasDown.name = "fylakasDown";
				fylakasDown.transform.position = posFylaksDown;

				//the center of map image
				dotCenter = getGpsData(new Vector2(posOria.x, posOria.z));	//getGpsData(new Vector2(0.1f,0.1f));
				
				//up left corner of map
				dotUpScenePos = new Vector2(posFylaksUp.x, posFylaksUp.z);
				dotUpGpsPos = getGpsData(new Vector2(posFylaksUp.x, posFylaksUp.z));

				//down right corner of map
				dotDownScenePos = new Vector2(posFylaksDown.x, posFylaksDown.z);
				dotDownGpsPos = getGpsData(new Vector2(posFylaksDown.x, posFylaksDown.z));

				//map terrain size
				sizeTerrain = new Vector2(frustumWidth, frustumHeight);
				mySizeOfTerrain = sizeTerrain;

				ExportDotVectors();

	//			Debug.Log("minX = "+ posFylaksUp.x + " - maxX = "+posFylaksDown.x );
			}
		}
		
		return mySizeOfTerrain;
		
	}

	void ExportDotVectors(){
		
//		texturesNames.Sort();
		//Start righting the xml
		string path = GetSceneName()+"_GPS_Data.txt";
		TextWriter sw = new StreamWriter(path);
		
		StringBuilder s = new StringBuilder();
		s.AppendLine("Scene -> "+GetSceneName());//+" ("+texturesNames.Count+")"
		s.AppendLine("Terrain size = "+sizeTerrain.x+" x "+sizeTerrain.y);
		s.AppendLine(" ");
		s.AppendLine("//##################### COPY / PAST to menu XML #############################");
		s.AppendLine("<terrainSize>");
		s.AppendLine("<x>"+sizeTerrain.x+"</x>");
		s.AppendLine("<y>"+sizeTerrain.y+"</y>");
		s.AppendLine("</terrainSize>");
		s.AppendLine("//##################### COPY PAST to scenes XML #############################");
		s.AppendLine("<sceneArea>");
		s.AppendLine("<lat>"+dotCenter.x+"</lat>");
		s.AppendLine("<lon>"+dotCenter.y+"</lon>");
		s.AppendLine("<posX>"+kameraMap.transform.position.x+"</posX>");
		s.AppendLine("<posZ>"+kameraMap.transform.position.z+"</posZ>");
		s.AppendLine("<refPoints>");
		s.AppendLine("<pointA>");
		s.AppendLine("<lat>"+dotUpGpsPos.x+"</lat>");
		s.AppendLine("<lon>"+dotUpGpsPos.y+"</lon>");
		s.AppendLine("<x>"+dotUpScenePos.x+"</x>");
		s.AppendLine("<y>"+dotUpScenePos.y+"</y>");
		s.AppendLine("</pointA>");
		s.AppendLine("<pointB>");
		s.AppendLine("<lat>"+dotDownGpsPos.x+"</lat>");
		s.AppendLine("<lon>"+dotDownGpsPos.y+"</lon>");
		s.AppendLine("<x>"+dotDownScenePos.x+"</x>");
		s.AppendLine("<y>"+dotDownScenePos.y+"</y>");
		s.AppendLine("</pointB>");
		s.AppendLine("</refPoints>");
		s.AppendLine("</sceneArea>");
		s.AppendLine("//##################### END of COPY to XML ########################");


//		s.AppendLine("Center of scene to Map = "+dotCenter.x+" x "+dotCenter.y);
//		s.AppendLine(" ");
//		s.AppendLine("Reff A scene = "+dotUpScenePos.x+" x "+dotUpScenePos.y);
//		s.AppendLine("Reff A gps = "+dotUpGpsPos.x+" x "+dotUpGpsPos.y);
//		s.AppendLine(" ");
//		s.AppendLine("Reff B scene = "+dotDownScenePos.x+" x "+dotDownScenePos.y);
//		s.AppendLine("Reff B gps = "+dotDownGpsPos.x+" x "+dotDownGpsPos.y);
		sw.Write(s);
		

		
		sw.Close ();
		
		//anoikse to arxeio
		Application.OpenURL(path);
		
		//		myPaths=monopatia;
		
	}

//	<sceneArea>
//		<lat>37.974872</lat>
//			<lon>23.721925</lon>
//			<refPoints>
//			<pointA>
//			<lat>37.9755</lat>
//			<lon>23.721583</lon>
//			<x>-28.7</x>
//			<y>69</y>
//			</pointA>
//			<pointB>
//			<lat>37.974002</lat>
//			<lon>23.72388</lon>
//			<x>150</x>
//			<y>-94</y>
//			</pointB>
//			</refPoints>
//			</sceneArea>

	#endregion


	#region calculate coordiness


//	double radiansFromDegrees(double degrees)
//	{
//		return degrees * (Mathf.PI/180.0f);    
//	}
//	
//	double degreesFromRadians(double radians)
//	{
//		return radians * (180.0f/Mathf.PI);
//	}

	//http://stackoverflow.com/questions/6633850/calculate-new-coordinate-x-meters-and-y-degree-away-from-one-coordinate
	public Vector2 coordinateFromCoord(Vector2 fromCoord, float distanceKm, float bearingDegrees)
	{
//		Vector2 fromCoord = new Vector2(distanceKm, bearingDegrees);

		float distanceRadians = distanceKm / earthRadius;
		//6,371 = Earth's radius in km
		float bearingRadians = Mathf.Deg2Rad * bearingDegrees;// radiansFromDegrees(bearingDegrees);
		float fromLatRadians = Mathf.Deg2Rad * fromCoord.x;
		float fromLonRadians = Mathf.Deg2Rad * fromCoord.y;

		float toLatRadians = Mathf.Asin( Mathf.Sin(fromLatRadians) * Mathf.Cos(distanceRadians) 
		                                + Mathf.Cos(fromLatRadians) * Mathf.Sin(distanceRadians) * Mathf.Cos(bearingRadians));

		float toLonRadians = fromLonRadians + Mathf.Atan2 (Mathf.Sin(bearingRadians)
		                                                   *Mathf.Sin(distanceRadians) * Mathf.Cos(fromLatRadians) , Mathf.Cos(distanceRadians)
		                                                   - Mathf.Sin(fromLatRadians) * Mathf.Sin(toLatRadians));

		// adjust toLonRadians to be in the range -180 to +180...
		//toLonRadians = fmod((toLonRadians + 3*M_PI), (2*M_PI)) - M_PI;
//		toLonRadians = toLatRadians-180f % 180f;

		Vector2 result = Vector2.zero;
		result.x = Mathf.Rad2Deg * toLatRadians;
		result.y = Mathf.Rad2Deg * toLonRadians;
		return result;
	}

	#endregion

/*

	#region Google Maps

	public class GoogleMap : MonoBehaviour
{
	public enum MapType
	{
		RoadMap,
		Satellite,
		Terrain,
		Hybrid
	}
	public bool loadOnStart = true;
	public bool autoLocateCenter = true;
	public GoogleMapLocation centerLocation;
	public int zoom = 13;
	public MapType mapType;
	public int size = 512;
	public bool doubleResolution = false;
	public List<GoogleMapMarker> markers = new List<GoogleMapMarker>();
	public List<GoogleMapPath> paths = new List<GoogleMapPath>();

	void Start() {
		if(loadOnStart) Refresh();
	}

	public void Refresh() {
		if(autoLocateCenter && (markers.Count == 0 && paths.Count == 0)) {
			Debug.LogError("Auto Center will only work if paths or markers are used.");
		}
		StartCoroutine(_Refresh());
	}

	IEnumerator _Refresh ()
	{
		var url = "http://maps.googleapis.com/maps/api/staticmap";
		var qs = "";
		if (!autoLocateCenter) {
			if (centerLocation.address != "")
				qs += "center=" + WWW.UnEscapeURL(centerLocation.address);
			else {
				qs += "center=" + WWW.UnEscapeURL (string.Format ("{0},{1}", centerLocation.latitude, centerLocation.longitude));
			}

			qs += "&zoom=" + zoom.ToString ();
		}
		qs += "&size=" + WWW.UnEscapeURL (string.Format ("{0}x{0}", size));
		qs += "&scale=" + (doubleResolution ? "2" : "1");
		qs += "&maptype=" + mapType.ToString ().ToLower ();
		var usingSensor = false;
		#if UNITY_IPHONE
		usingSensor = Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running;
		#endif
		qs += "&sensor=" + (usingSensor ? "true" : "false");

		foreach (var i in markers) {
			qs += "&markers=" + string.Format ("size:{0}|color:{1}|label:{2}", i.size.ToString ().ToLower (), i.color, i.label);
			foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL (loc.address);
				else
					qs += "|" + WWW.UnEscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}

		foreach (var i in paths) {
			qs += "&path=" + string.Format ("weight:{0}|color:{1}", i.weight, i.color);
			if(i.fill) qs += "|fillcolor:" + i.fillColor;
			foreach (var loc in i.locations) {
				if (loc.address != "")
					qs += "|" + WWW.UnEscapeURL (loc.address);
				else
					qs += "|" + WWW.UnEscapeURL (string.Format ("{0},{1}", loc.latitude, loc.longitude));
			}
		}


		var req = new WWW (url + "?" + qs);
		yield return req;
		GetComponent<Renderer> ().material.mainTexture = req.texture;
		//renderer.material.mainTexture = req.texture;
	}


}


public enum GoogleMapColor
{
	black,
	brown,
	green,
	purple,
	yellow,
	blue,
	gray,
	orange,
	red,
	white
}

[System.Serializable]
public class GoogleMapLocation
{
	public string address;
	public float latitude;
	public float longitude;
}

[System.Serializable]
public class GoogleMapMarker
{
	public enum GoogleMapMarkerSize
	{
		Tiny,
		Small,
		Mid
	}
	public GoogleMapMarkerSize size;
	public GoogleMapColor color;
	public string label;
	public List<GoogleMapLocation> locations = new List<GoogleMapLocation>();

}

[System.Serializable]
public class GoogleMapPath
{
	public int weight = 5;
	public GoogleMapColor color;
	public bool fill = false;
	public GoogleMapColor fillColor;
	public List<GoogleMapLocation> locations = new List<GoogleMapLocation>();
}

	#endregion

*/

	string copyright = "@Stathis 2017 - Scene Map & Gps data Tool";
}
