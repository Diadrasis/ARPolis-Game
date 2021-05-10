/*
 File 			ObjectGroupInspector.cs
 Location       Assets/Editor
 Category		Editor Script
 Author			Jos Balcaen
 Date			08/2013
 Copyright 		contact@josbalcaen.com
*/
using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

// Apply the AnchorPointGroup.cs (empty script) to a GameObject
// This script will replace the inspector interface of ObjectPointGroup.cs
[CustomEditor(typeof(ObjectGroup))]
public class ObjectGroupInspector : Editor
{
	private static bool __editMode = false; // When we are in edit mode (Raycast mode)
	//private static int __count = 0; // Counter of the placed items (Not really used right now)
	private GameObject __objectGroup; // GameObject containing the group of the anchorPoints group.
	private int __currentType = 0;
	private static string[] __typeStrings;
	
	void OnEnable()
	{
		// Get all available prefabs
		string prefabFolder = Application.dataPath + "/Prefabs/";
		string[] prefabs = System.IO.Directory.GetFiles(prefabFolder, "*.prefab");
		__typeStrings = new string[prefabs.Length];
		for (int i = 0; i < prefabs.Length; i++)
		{
			// Anchor point prefabs need to start with anp_anchorPoint to have a nice name displayed
			string filename = System.IO.Path.GetFileNameWithoutExtension(prefabs[i]);
			__typeStrings[i] = filename;
		}
	}
	void OnSceneGUI()
	{
		if(Event.current.type == EventType.keyUp && Event.current.keyCode == KeyCode.A){
			__editMode=!__editMode;
		}
		// If we are in edit mode and the user clicks (right click, middle click or alt+left click)
		if (__editMode)
		{
			if (Event.current.type == EventType.MouseUp)
			{
				// Shoot a ray from the mouse position into the world
				Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				RaycastHit hitInfo;
				// Shoot this ray. check in a distance of 10000. 
				if (Physics.Raycast(worldRay, out hitInfo, 10000))
				{
					// Load the current prefab
					string path = "Assets/Prefabs/" + __typeStrings[__currentType] + ".prefab";
					GameObject anchor_point = Resources.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
					// Instance this prefab
					GameObject prefab_instance = PrefabUtility.InstantiatePrefab(anchor_point) as GameObject;
					// Place the prefab at correct position (position of the hit).
					prefab_instance.transform.position = hitInfo.point;
					prefab_instance.transform.parent = __objectGroup.transform;
					// Mark the instance as dirty because we like dirty
					EditorUtility.SetDirty(prefab_instance);
				}
			}
			// Mark the event as used
			Event.current.Use();
		} // End if __editMode
	} // End OnSceneGUI
	
	public override void OnInspectorGUI()
	{
		// Toggle edit mode
		if (__editMode)
		{// If we are in editing mode, make the button green and change the label
			GUI.color = Color.green;
			if (GUILayout.Button("Disable Editing")) { __editMode = false; }
		}
		else
		{
			GUI.color = Color.white; // Normal color if w're not in editing mode
			if (GUILayout.Button("Enable Editing"))
			{
				__editMode = true;
				// Get the objectGroup (Active selection)
				__objectGroup = Selection.activeGameObject;
			}
		}
		GUI.color = Color.white;
		
		GUILayout.Label("Choose a type to place.");
		// Create a selection grid where the user can select the current type
		__currentType = GUILayout.SelectionGrid(__currentType, __typeStrings, 1);
	}
}
