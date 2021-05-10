using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AreaTools : EditorWindow {
	string myScene = "Aerides";
	string myXml = "data";
	int myPoints = 1;
	float myAreaHeight=1f;
	float myRadius=5f;

	bool groupEnabled;
	
	Color colorStart = Color.red;
	Color colorPoint = Color.green;

	List<GameObject> pointsOrdered = new List<GameObject>();

	Vector3[] myPositions;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Diadrasis/Area Tools")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		AreaTools window = (AreaTools)EditorWindow.GetWindow (typeof (AreaTools));
		window.Show();
	}

	void OnSceneGUI () {

	}
	
	void OnGUI () {
		GUILayout.Label ("Area Settings", EditorStyles.boldLabel);
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Clear All Areas In Scene")){
			if(EditorUtility.DisplayDialog("DELETE ALL AREAS!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportAreas.DestroyAreas();
			}
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("Type the name of xml to import areas into the scene", EditorStyles.boldLabel);
		myXml = EditorGUILayout.TextField ("name of xml", myXml);
		
		GUILayout.Label ("Type the name of scene in xml", EditorStyles.boldLabel);
		myScene = EditorGUILayout.TextField ("Scene name in xml", myScene);

		if(GUILayout.Button("Import Off Site Areas From Xml")){
			if(EditorUtility.DisplayDialog("MUST DELETE ALL AREAS IN SCENE!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportAreas.CreateSceneAreas(myScene,myXml,myAreaHeight,2);
			}
		}
		if(GUILayout.Button("Import OnSite Areas From Xml")){
			if(EditorUtility.DisplayDialog("MUST DELETE ALL AREAS IN SCENE!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportAreas.CreateSceneAreas(myScene,myXml,myAreaHeight,1);
			}
		}
		if(GUILayout.Button("Import Scene Areas gps check From Xml")){
			if(EditorUtility.DisplayDialog("MUST DELETE ALL AREAS IN SCENE!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportAreas.CreateSceneAreas(myScene,myXml,myAreaHeight,0);
			}
		}
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------New Area Settings--------------------------", EditorStyles.boldLabel);
		
		myPoints = EditorGUILayout.IntSlider ("Area Points", myPoints, 2, 20);
		
		myAreaHeight = EditorGUILayout.Slider ("Area Height", myAreaHeight, 0f, 100f);

		myRadius = EditorGUILayout.Slider ("Area Radius", myRadius, 1f, 30f);
		
		colorStart = EditorGUILayout.ColorField("start point color",colorStart);
		colorPoint = EditorGUILayout.ColorField("points color",colorPoint);
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Make New Area")){
			CreateNewArea.MakeAreaPoints(myPoints,myAreaHeight,myRadius,colorStart,colorPoint,SceneView.lastActiveSceneView.pivot);
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------Export New Areas--------------------------", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Export Areas to txt")){
			ExportManyAreas.ExportSceneAreas();
		}
		
//		GUILayout.Label ("", EditorStyles.boldLabel);
//
//		if(GUILayout.Button("DrawLines Aeras")){
//			pointsOrdered = ExportManyAreas.DrawLinesAeras();
//			Debug.Log(pointsOrdered.Count);
//		}

//		if(pointsOrdered.Count>0){
//			myPositions = new Vector3[pointsOrdered.Count];
//			for(int i = 0; i < pointsOrdered.Count-1; i++){
//				if(pointsOrdered[i]){
//					myPositions[i]=pointsOrdered[i].transform.position;
//				}
//			}
//			HandleUtility.GetHandleSize(myPositions[0]);
//			Handles.DrawPolyLine(myPositions);
//			Debug.Log("drawing");
//		}
//		groupEnabled = EditorGUILayout.BeginToggleGroup ("Extra Settings", groupEnabled);
//		
//		//		GUILayout.Label ("", EditorStyles.boldLabel);
//		GUILayout.Label (" Mesh Colliders in Scene", EditorStyles.boldLabel);
//		
//		if(GUILayout.Button("Enable Mesh Colliders")){
//			EnableColliders();
//		}
//		if(GUILayout.Button("Disable Mesh Colliders")){
//			DisableColliders();
//		}
//
//		GUILayout.Label (" Shadows in Scene", EditorStyles.boldLabel);
//
//		if(GUILayout.Button("Enable Shadows")){
//			OnOff_CastRecieve_Shadows(true);
//		}
//		if(GUILayout.Button("Disable Shadows")){
//			OnOff_CastRecieve_Shadows(false);
//		}
//		
//		EditorGUILayout.EndToggleGroup ();
		
		//	EditorGUILayout.TextField ("SceneViewCamera position", ""+SceneView.lastActiveSceneView.pivot);
	}
	
	
	void EnableColliders(){
		MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
		
		foreach(MeshCollider col in meshes){
			if(!col.name.StartsWith("Terrain")){
				if(!col.enabled){
					col.enabled=true;
				}
			}
		}
		
		Debug.Log("Enabled All Mesh Colliders !");
	}
	
	void DisableColliders(){
		MeshCollider[] meshes = GameObject.FindObjectsOfType<MeshCollider>();
		
		foreach(MeshCollider col in meshes){
			if(!col.name.StartsWith("Terrain")){
				if(col.enabled){
					col.enabled=false;
				}
			}
		}
		
		Debug.Log("Disabled All Mesh Colliders !");
	}

	void OnOff_CastRecieve_Shadows(bool kane){
		MeshRenderer[] meshes = GameObject.FindObjectsOfType<MeshRenderer>();

		foreach(MeshRenderer m in meshes){
			if(m){
				m.castShadows=kane;
				m.receiveShadows=kane;
			}
		}

		if(kane==true){
			Debug.Log("Enabled All Mesh Shadows !");
		}else{
			Debug.Log("Disabled All Mesh Shadows !");
		}
	}
	
}
