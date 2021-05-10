using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DeadAreaTools : EditorWindow {
	string myScene = "Venetian";
	string myXml = "data";
	int myPoints = 1;
	float myAreaHeight=45f;
	float myRadius=5f;
	
	bool groupEnabled;
	
	Color colorStart = Color.white;
	Color colorPoint = Color.black;
	
	List<GameObject> pointsOrdered = new List<GameObject>();
	
	Vector3[] myPositions;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Diadrasis/DeadArea Tools")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		DeadAreaTools window = (DeadAreaTools)EditorWindow.GetWindow (typeof (DeadAreaTools));
		window.Show();
	}
	
	void OnSceneGUI () {
		
	}
	
	void OnGUI () {
		GUILayout.Label ("DeadAreas Settings", EditorStyles.boldLabel);
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Clear All Dead Areas In Scene")){
			if(EditorUtility.DisplayDialog("DELETE ALL DeadAREAS!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportDeadAreas.DestroyAreas();
			}
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("Type the name of xml to import Dead areas into the scene", EditorStyles.boldLabel);
		myXml = EditorGUILayout.TextField ("name of xml", myXml);
		
		GUILayout.Label ("Type the name of scene in xml", EditorStyles.boldLabel);
		myScene = EditorGUILayout.TextField ("Scene name in xml", myScene);
		
		if(GUILayout.Button("Import DeadAreas From Xml")){
			if(EditorUtility.DisplayDialog("MUST DELETE ALL DeadAREAS IN SCENE!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportDeadAreas.CreateSceneAreas(myScene,myXml,myAreaHeight);
			}
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------New Area Settings--------------------------", EditorStyles.boldLabel);
		
		myPoints = EditorGUILayout.IntSlider ("DeadArea Points", myPoints, 2, 20);
		
		myAreaHeight = EditorGUILayout.Slider ("DeadArea Height", myAreaHeight, 0f, 100f);
		
		myRadius = EditorGUILayout.Slider ("DeadArea Radius", myRadius, 1f, 30f);
		
		colorStart = EditorGUILayout.ColorField("start point color",colorStart);
		colorPoint = EditorGUILayout.ColorField("points color",colorPoint);
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Make New DeadArea")){
			CreateNewDeadArea.MakeDeadAreaPoints(myPoints,myAreaHeight,myRadius,colorStart,colorPoint,SceneView.lastActiveSceneView.pivot);
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------Export New DeadAreas--------------------------", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Export DeadAreas to txt")){
			ExportManyDeadAreas.ExportDeadSpots();
		}

	}
	
	

	
}

