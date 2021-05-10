using UnityEngine;
using UnityEditor;
public class PathTools : EditorWindow {
	string myScene = "Aerides";
	string myXml = "editor";
	int myPoints = 1;
	float myPathHeight=1f;
	bool groupEnabled;

	Color colorStart = Color.red;
	Color colorPoint = Color.green;

//	Vector3 newPathPos=Vector3.zero;

//	Transform myTransform;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Diadrasis/Path Tools")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		PathTools window = (PathTools)EditorWindow.GetWindow (typeof (PathTools));
		window.Show();


	}
	
	void OnGUI () {
		GUILayout.Label ("Path Settings", EditorStyles.boldLabel);

		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Clear All Paths In Scene")){
			if(EditorUtility.DisplayDialog("DELETE ALL PATHS!!", "ARE YOU SURE;", "YES" , "NO")){
				ImportPaths.DestroyPaths();
			}
		}

		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("Type the name of xml to import paths into the scene", EditorStyles.boldLabel);
		myXml = EditorGUILayout.TextField ("name of xml", myXml);
		
		GUILayout.Label ("Type the name of scene in xml", EditorStyles.boldLabel);
		myScene = EditorGUILayout.TextField ("Scene name in xml", myScene);

		if(GUILayout.Button("Import Paths From Xml"))
		{
			if(EditorUtility.DisplayDialog("MUST DELETE ALL PATHS IN SCENE!!", "ARE YOU SURE;", "YES" , "NO")){
//				ImportPaths.DestroyPaths();
				ImportPaths.CreateScenePaths(myScene,myXml,myPathHeight);
			}
		}

		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------New Path Settings--------------------------", EditorStyles.boldLabel);

		myPoints = EditorGUILayout.IntSlider ("Path Points", myPoints, 1, 20);
		
		myPathHeight = EditorGUILayout.Slider ("Path Height", myPathHeight, 0, 100);

//		newPathPos = EditorGUILayout.Vector3Field ("Path Position", newPathPos);

//		myTransform = EditorGUILayout.ObjectField("Transform Near New Path", myTransform, typeof(Transform), true) as Transform;

		colorStart = EditorGUILayout.ColorField("start point color",colorStart);
		colorPoint = EditorGUILayout.ColorField("points color",colorPoint);

		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Make New Path")){


			CreateNewPath.MakePoints(myPoints,myPathHeight,colorStart,colorPoint,SceneView.lastActiveSceneView.pivot,true);
		}

		GUILayout.Label ("", EditorStyles.boldLabel);
		if(GUILayout.Button("Make New Path with No Limit")){
			
			
			CreateNewPath.MakePoints(myPoints,myPathHeight,colorStart,colorPoint,SceneView.lastActiveSceneView.pivot,false);
		}

		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label ("-------------------------Export New Paths--------------------------", EditorStyles.boldLabel);

		if(GUILayout.Button("Export Paths to txt")){
			ExportManyPaths.ExportScene_toOnePath();
			ExportManyPaths.ExportScene_toManyPaths();
		}

		GUILayout.Label ("", EditorStyles.boldLabel);
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Extra Settings", groupEnabled);
		
		//		GUILayout.Label ("", EditorStyles.boldLabel);
		GUILayout.Label (" Mesh Colliders in Scene", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Enable Mesh Colliders")){
			EnableColliders();
		}
		if(GUILayout.Button("Disable Mesh Colliders")){
			DisableColliders();
		}
		
		GUILayout.Label (" Shadows in Scene", EditorStyles.boldLabel);
		
		if(GUILayout.Button("Enable Shadows")){
			OnOff_CastRecieve_Shadows(true);
		}
		if(GUILayout.Button("Disable Shadows")){
			OnOff_CastRecieve_Shadows(false);
		}
		
		EditorGUILayout.EndToggleGroup ();
		
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
