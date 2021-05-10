using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class KolonesCreation : EditorWindow {
	int mySize = 2;
	
	GameObject fatherContainer;
	GameObject prefabGb;
	GameObject prefabGb2;
	int itemsToSpawn=1;
	int itemsToSpawn2=1;
	int distBetweenItems=10;
	int areaSide=20;
	int rotAngle=0;
	
	
	GameObject startPoint;
	[SerializeField]
	GameObject[] middlePoint;
	GameObject endPoint;
	
	// Add menu named "My Window" to the Window menu
	[MenuItem ("Diadrasis/Κολωνες")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		KolonesCreation window = (KolonesCreation)EditorWindow.GetWindow (typeof (KolonesCreation));
		window.Show();
	}
	
	void OnInspectorUpdate() {
		Repaint();
	}
	
	List<Vector3> posForSpawn=new List<Vector3>();
	
	bool waitForCheck=false;
	bool isGama;
	bool isPi;
	bool isTetragono;
	bool isGrid;
	
	void OnGUI () {

		GUILayout.Label ("", EditorStyles.boldLabel);
		prefabGb = (GameObject) EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Prefab κολωνας", prefabGb, typeof(GameObject),true);
		GUILayout.Label ("", EditorStyles.boldLabel);

		GUILayout.Label ("", EditorStyles.boldLabel);
		prefabGb2 = (GameObject) EditorGUI.ObjectField(new Rect(3, 30, position.width - 6, 20), "Prefab 2ης κολωνας", prefabGb2, typeof(GameObject),true);
		GUILayout.Label ("", EditorStyles.boldLabel);

		GUILayout.Label ("", EditorStyles.boldLabel);
		startPoint = (GameObject) EditorGUI.ObjectField(new Rect(3, 60, position.width - 6, 20), "Σημειο εναρξης", startPoint, typeof(GameObject),true);
		GUILayout.Label ("", EditorStyles.boldLabel);

		GUILayout.Label ("Γωνια περιστροφης ως προς το Σημειο εναρξης", EditorStyles.boldLabel);
		rotAngle = EditorGUILayout.IntSlider("Γωνια",rotAngle,0,180);
		
		//		GUILayout.Label ("", EditorStyles.boldLabel);
		//		GUILayout.Label ("Βαλε ενα αδειο αντικειμενο ως ενδιαμεσο σημειο", EditorStyles.boldLabel);
		//		middlePoint = (GameObject) EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Μεσαια σημεια", middlePoint, typeof(GameObject));
		//		GUILayout.Label ("", EditorStyles.boldLabel);
		
		endPoint = (GameObject) EditorGUI.ObjectField(new Rect(3, 90, position.width - 6, 20), "Τελικο σημειο", endPoint, typeof(GameObject),true);
		GUILayout.Label ("--------------------------------------------------", EditorStyles.boldLabel);

		GUILayout.Label ("ΚΟΛΩΝΕΣ", EditorStyles.boldLabel);
		
		GUILayout.Label ("Ποσες κολωνες να δημιουργηθουν;", EditorStyles.boldLabel);
		itemsToSpawn = EditorGUILayout.IntSlider("Ποσοτητα",itemsToSpawn,2,50);

		GUILayout.Label ("Ποσες κολωνες στη 2η πλευρα;", EditorStyles.boldLabel);
		itemsToSpawn2 = EditorGUILayout.IntSlider("Ποσοτητα",itemsToSpawn2,2,50);

		GUILayout.Label ("", EditorStyles.boldLabel);
		isGrid = EditorGUILayout.Toggle("Grid", isGrid);

		if(!isGrid)
		{
			GUILayout.Label ("", EditorStyles.boldLabel);
			isGama = EditorGUILayout.Toggle("σχημα Γ", isGama);

			isPi = EditorGUILayout.Toggle("σχημα Π", isPi);
			isTetragono = EditorGUILayout.Toggle("ΤΕΤΡΑΓΩΝΟ", isTetragono);		

			if(isTetragono){
				isPi=true;
				isGama=true;
			}
			else
			if(isPi){
				isGama=true;
				isTetragono=false;
			}
			else
			if(isGama){
				isPi=false;
				isTetragono=false;
			}
		}
		
		if(GUILayout.Button("Εκτελεση τοποθετησης κολωνων")){
			if(prefabGb)
			{

				if(startPoint && endPoint)
				{
					if(!isGrid)
					{
						CreateKolones();	
					}
					else
					{
						CreateGrid();
					}
				}
				else
				{
					if(EditorUtility.DisplayDialog("Missing points", "Set Start - End points", "OK")){
						
					}
				}
			}
			else
			{
				if(EditorUtility.DisplayDialog("Missing prefab", "Set a prefab first", "OK")){
					
				}
			}
		}

	}

	Vector3 point;
	float percentage;
	GameObject newGb;
	GameObject node;

	void CreateGrid()
	{
		float dist = Vector3.Distance(startPoint.transform.position,endPoint.transform.position);
		Debug.Log("συνολικη αποσταση = "+dist);
		float kolDist = dist/itemsToSpawn;
		Debug.Log("αποσταση μεταξυ κολωνων = "+kolDist);

		for(int x=0; x<itemsToSpawn; x++)
		{
			for(int y=0; y<itemsToSpawn2; y++)
			{

			}
		}

	}

	void CreateKolones()
	{

		float dist = Vector3.Distance(startPoint.transform.position,endPoint.transform.position);
		Debug.Log("συνολικη αποσταση = "+dist);
		float kolDist = dist/itemsToSpawn;
		Debug.Log("αποσταση μεταξυ κολωνων = "+kolDist);

		GameObject fath = new GameObject();
		fath.transform.position = startPoint.transform.position;
		fath.transform.LookAt(endPoint.transform);
		Vector3 rot = fath.transform.eulerAngles;
		rot.y = rotAngle;
		fath.name="Pleura_1";

		Vector3 direction = endPoint.transform.position - startPoint.transform.position;

		for(float i=0; i<itemsToSpawn; i++)
		{
			percentage = i/(itemsToSpawn-1);//-1 gia na mpei kolona kai sto endpoint

			point = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, percentage);

			newGb = null;
			

			if(prefabGb2){
				int tyx = Random.Range(0,100);
				if(tyx%2==0){//even
					newGb = Instantiate (prefabGb)as GameObject;
				}
				else
				if(tyx%2==1){//odd
					newGb = Instantiate (prefabGb2)as GameObject;
				}

				newGb.transform.position = point;
				
				newGb.transform.eulerAngles=rot;
				
				newGb.transform.SetParent(fath.transform);
			}else{

				newGb = Instantiate (prefabGb)as GameObject;
				
				newGb.transform.position = point;
				
				newGb.transform.eulerAngles=rot;
				
				newGb.transform.SetParent(fath.transform);
			}

			newGb.name="obj_"+i.ToString();
		}

		Debug.Log("Pleura_1 has "+itemsToSpawn.ToString()+" objects");

		if(isGama)
		{
			dist = Vector3.Distance(startPoint.transform.position,endPoint.transform.position);
			Debug.Log("συνολικη αποσταση = "+dist);
			kolDist = dist/itemsToSpawn2;
			Debug.Log("αποσταση μεταξυ κολωνων = "+kolDist);
			
			fath = new GameObject();
			fath.transform.position = startPoint.transform.position;
			fath.transform.LookAt(endPoint.transform);
			rot = fath.transform.eulerAngles;
			rot.y = rotAngle;
			fath.name="Pleura_2";
			
			direction = endPoint.transform.position - startPoint.transform.position;
			
			for(float i=0; i<itemsToSpawn2; i++)
			{
				if(i==0){i=1;}

				percentage = i/(itemsToSpawn2-1);//-1 gia na mpei kolona kai sto endpoint
				
				point = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, percentage);
				
				newGb = null;
				
				
				if(prefabGb2){
					int tyx = Random.Range(0,100);
					if(tyx%2==0){//even
						newGb = Instantiate (prefabGb)as GameObject;
					}
					else
					if(tyx%2==1){//odd
						newGb = Instantiate (prefabGb2)as GameObject;
					}
					
					newGb.transform.position = point;
					
					newGb.transform.eulerAngles=rot;
					
					newGb.transform.SetParent(fath.transform);
				}else{
					
					newGb = Instantiate (prefabGb)as GameObject;
					
					newGb.transform.position = point;
					
					newGb.transform.eulerAngles=rot;
					
					newGb.transform.SetParent(fath.transform);
				}
				newGb.name="obj_"+i.ToString();
			}

			fath.transform.position = endPoint.transform.position;
			rot = fath.transform.eulerAngles;
			rot.y = rot.y + 90f;
			fath.transform.localEulerAngles = rot;

		}

		Debug.Log("Pleura_2 has "+itemsToSpawn2.ToString()+" objects");

		if(isPi)
		{
			dist = Vector3.Distance(startPoint.transform.position,endPoint.transform.position);
			Debug.Log("συνολικη αποσταση = "+dist);
			kolDist = dist/itemsToSpawn2;
			Debug.Log("αποσταση μεταξυ κολωνων = "+kolDist);
			
			fath = new GameObject();
			fath.transform.position = startPoint.transform.position;
			fath.transform.LookAt(endPoint.transform);
//			rot = fath.transform.eulerAngles;
			fath.transform.eulerAngles = rot;
			rot.y = rot.y + rotAngle;
			fath.name="Pleura_3";
			
			direction = endPoint.transform.position - startPoint.transform.position;
			
			for(float i=0; i<itemsToSpawn; i++)
			{
				if(i==0){i=1;}

				percentage = i/(itemsToSpawn-1);//-1 gia na mpei kolona kai sto endpoint
				
				point = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, percentage);
				
				newGb = null;
				
				if(prefabGb2){
					int tyx = Random.Range(0,100);
					if(tyx%2==0){//even
						newGb = Instantiate (prefabGb)as GameObject;
					}
					else
					if(tyx%2==1){//odd
						newGb = Instantiate (prefabGb2)as GameObject;
					}
					
					newGb.transform.position = point;
					
					newGb.transform.eulerAngles=rot;
					
					newGb.transform.SetParent(fath.transform);
				}else{
					
					newGb = Instantiate (prefabGb)as GameObject;
					
					newGb.transform.position = point;
					
					newGb.transform.eulerAngles=rot;
					
					newGb.transform.SetParent(fath.transform);
				}

				newGb.name="obj_"+i.ToString();

			}

			fath.transform.position = startPoint.transform.position;
			
//			fath.transform.position = endPoint.transform.position;
//			rot = fath.transform.eulerAngles;
			rot.y = rot.y + 90f;
			fath.transform.localEulerAngles = rot;
			
		}

		Debug.Log("Pleura_3 has "+itemsToSpawn.ToString()+" objects");

		if(isTetragono)
		{
			dist = Vector3.Distance(startPoint.transform.position,endPoint.transform.position);
			Debug.Log("συνολικη αποσταση = "+dist);
			kolDist = dist/itemsToSpawn2;
			Debug.Log("αποσταση μεταξυ κολωνων = "+kolDist);

			node = newGb;

			fath = new GameObject();
			fath.transform.position = startPoint.transform.position;
			fath.transform.LookAt(endPoint.transform);
			//			rot = fath.transform.eulerAngles;
			fath.transform.eulerAngles = rot;
			rot.y = rot.y + rotAngle;
			
			direction = endPoint.transform.position - startPoint.transform.position;
			
			for(float i=0; i<itemsToSpawn2; i++)
			{
				//dont create the first
				if(i==0){i=1;}

				//dont create the last
				if(i!=itemsToSpawn2-1)
				{
					percentage = i/(itemsToSpawn2-1);//-1 gia na mpei kolona kai sto endpoint
					
					point = Vector3.Lerp(startPoint.transform.position, endPoint.transform.position, percentage);
					
					newGb = null;
					
					if(prefabGb2){
						int tyx = Random.Range(0,100);
						if(tyx%2==0){//even
							newGb = Instantiate (prefabGb)as GameObject;
						}
						else
						if(tyx%2==1){//odd
							newGb = Instantiate (prefabGb2)as GameObject;
						}
						
						newGb.transform.position = point;
						
						newGb.transform.eulerAngles=rot;
						
						newGb.transform.SetParent(fath.transform);
					}else{
						
						newGb = Instantiate (prefabGb)as GameObject;
						
						newGb.transform.position = point;
						
						newGb.transform.eulerAngles=rot;
						
						newGb.transform.SetParent(fath.transform);
					}
					
					newGb.name="obj_"+i.ToString();
				}
				
			}
			
			fath.transform.SetParent(node.transform);
			fath.transform.localPosition = Vector3.zero;
			
			//			fath.transform.position = endPoint.transform.position;
			//			rot = fath.transform.eulerAngles;
			rot.y = rot.y + 90f;
			fath.transform.localEulerAngles = rot;

			fath.transform.SetParent(null);
			fath.name="Pleura_4";
			
		}


		Debug.Log("Pleura_4 has "+itemsToSpawn2.ToString()+" objects");

		DestroyImmediate(startPoint,true);
		DestroyImmediate(endPoint,true);

	}


	
}

