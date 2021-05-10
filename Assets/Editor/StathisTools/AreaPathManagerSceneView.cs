using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( PointTool_AreaPathManager ) )]
public class AreaPathManagerSceneView : Editor
{
	public float arrowSize = 1;
	DrawAreas drawScript;
	static GameObject dot;

	GUIStyle currentStyle = null;
	
	void InitStyles()
	{
		if( currentStyle == null )
		{
			currentStyle = new GUIStyle( GUI.skin.box );
			currentStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 0.5f ) );
		}
	}

	Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	
	void OnSceneGUI( )
	{
		InitStyles();

		PointTool_AreaPathManager t = target as PointTool_AreaPathManager;

		if(!t){return;}

		if(t.transform.parent){
			drawScript = t.transform.parent.GetComponent<DrawAreas>();
		}
		if(drawScript){
			int indx = drawScript.myDotObjects.IndexOf(t.gameObject);
			t.ID = indx;
		}
		
		Handles.color = Color.blue;
		Handles.Label( t.transform.position + Vector3.back * 2, t.gameObject.name+"_"+t.ID, EditorStyles.boldLabel);
		
		Handles.BeginGUI( );
		GUILayout.BeginArea( new Rect( Screen.width - 150, Screen.height - 250, 140, 250 ) );
		

		if( GUILayout.Button( "Duplicate Point" ) ){
			if(t.transform.parent){
				drawScript = t.transform.parent.GetComponent<DrawAreas>();
			}

			Vector3 pos = t.transform.position;
//			pos.y+=200f;

			// Shoot a ray from the mouse position into the world
//			Ray worldRay = new Ray(pos, Vector3.down);
//			RaycastHit hit;
			
//			if (Physics.Raycast(worldRay, out hit, Mathf.Infinity))
//			{
//				if(hit.transform)// && hit.transform.name=="map")
//				{
//
//					dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//					dot.transform.localScale = new Vector3(3f,0.01f,3f);
//
////					pos.x+=1f;
////					pos.z+=1f;
//
//					Vector3 hitPos = hit.point;
//					
//					hitPos.y +=0.2f;
//					
//					// Place the prefab at correct position (position of the hit).
//					dot.transform.position = new Vector3(pos.x, hitPos.y, pos.z);
//
////					dot.transform.position = pos;
//					PointTool_AreaPathManager db = dot.AddComponent<PointTool_AreaPathManager>();
//
//
//					if(drawScript){
//						int indx = drawScript.myDotObjects.IndexOf(t.gameObject);
//
////						indx++;
//						drawScript.myDotObjects.Insert(indx, dot);
//						if(!drawScript.hasPathFreeMove){
//							dot.name = "pointNoLimit";
//						}else{
//							dot.name = "point";
//						}
//						db.ID = indx;
//
//						dot.transform.SetParent(t.transform.parent);
//					}
//				}else{
					dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					dot.transform.localScale = new Vector3(3f,0.01f,3f);
					
//					pos.x+=1f;
//					pos.z+=1f;
					pos = t.transform.position;

					dot.transform.position = pos;
					PointTool_AreaPathManager db = dot.AddComponent<PointTool_AreaPathManager>();
					
					if(drawScript){
						int indx = drawScript.myDotObjects.IndexOf(t.gameObject);
						
				if(!drawScript.isPath){
					indx++;
				}
//						indx++;
						drawScript.myDotObjects.Insert(indx, dot);
						if(!drawScript.hasPathFreeMove && drawScript.isPath){
							dot.name = "pointNoLimit";
						}else{
							dot.name = "point";
						}
						db.ID = indx;
						
						dot.transform.SetParent(t.transform.parent);
					}
//				}
//			}
		}


		if(t.transform.parent && t.transform.parent.name.StartsWith("Path")){
			GUILayout.Label ("", EditorStyles.boldLabel);

			if(drawScript.hasPathFreeMove){
				if( GUILayout.Button( "Disable path free move" ) ){
					if(t.transform.parent){
						drawScript = t.transform.parent.GetComponent<DrawAreas>();
					}
					if(drawScript){
						for(int p=0; p<drawScript.myDotObjects.Count; p++){
							if(!drawScript.myDotObjects[p].name.Contains("NoLimit")){
								drawScript.myDotObjects[p].name += "NoLimit";
							}
						}

						drawScript.hasPathFreeMove=false;
					}
					return;
				}
			}else{
				if( GUILayout.Button( "Enable path free move" ) ){
					if(t.transform.parent){
						drawScript = t.transform.parent.GetComponent<DrawAreas>();
					}
					if(drawScript){
						for(int p=0; p<drawScript.myDotObjects.Count; p++){
							if(drawScript.myDotObjects[p].name.EndsWith("NoLimit")){
								string pp = drawScript.myDotObjects[p].name;
								int a= drawScript.myDotObjects[p].name.Length;
								int b = a - 7;
								string sname = drawScript.myDotObjects[p].name.Remove(b,7);
								drawScript.myDotObjects[p].name = sname;
							}
						}
						
						drawScript.hasPathFreeMove=true;
					}
					return;
				}
			}
		}

		GUILayout.Label ("", EditorStyles.boldLabel);

		if( GUILayout.Button( "Delete Point" ) ){
			if(t.transform.parent){
				drawScript = t.transform.parent.GetComponent<DrawAreas>();
			}
			if(drawScript){
				drawScript.myDotObjects.Remove(t.gameObject);
			}
			DestroyImmediate(t.gameObject);
			return;
		}

		GUILayout.Label ("", EditorStyles.boldLabel);

		string parentObjectID = string.Empty;

		if(t.transform.parent && t.transform.parent.name.StartsWith("Area")){
			parentObjectID = "Area";
		}else
		if(t.transform.parent && t.transform.parent.name.StartsWith("Dead")){
			parentObjectID = "Dead Area";
		}else
		if(t.transform.parent && t.transform.parent.name.StartsWith("Path")){
			parentObjectID = "Path";
		}

		if( GUILayout.Button( "Delete "+parentObjectID ) ){
			if(EditorUtility.DisplayDialog(parentObjectID + " will be deleted", "ARE YOU SURE;", "YES" , "NO")){
				if(t.transform.parent){
					DestroyImmediate(t.transform.parent.gameObject);
				}
			}
			return;
		}

		
		GUILayout.EndArea( );
		Handles.EndGUI( );


		Handles.BeginGUI( );

		GUILayout.BeginArea( new Rect( 20, Screen.height - 250, 250, 190 ) );

		GUI.Box(new Rect( 20, Screen.height - 250, 250, 190) ,"",currentStyle);

		if( GUILayout.Button( "Create New OffSite Area" ) ){

			CreateArea("Area_OffSite", t.transform.position, false, false);

			return;
		}

		if( GUILayout.Button( "Create New OnSite Area" ) ){

			CreateArea("Area_OnSite", t.transform.position, true, false);

			return;
		}
		
		GUILayout.Label ("", EditorStyles.boldLabel);

		if( GUILayout.Button( "Create New OffSite Path" ) ){
			
			CreatePath("PathOffsite", t.transform.position, false);
			
			return;
		}

		if( GUILayout.Button( "Create New OnSite Path" ) ){

			CreatePath("PathOnsite", t.transform.position, true);

			return;
		}

		GUILayout.Label ("", EditorStyles.boldLabel);

		if( GUILayout.Button( "Create New OffSite Dead Area") ){

			CreateArea("Dead_OffSite", t.transform.position,false, true);

			return;
		}

		if( GUILayout.Button( "Create New OnSite Dead Area" ) ){

			CreateArea("Dead_OnSite", t.transform.position, true, true);

			return;
		}
		
		GUILayout.EndArea( );
		Handles.EndGUI( );

		Handles.DrawWireArc( t.transform.position, t.transform.up, -t.transform.right,
		                    360, t.shieldArea );
	}


	void CreateArea(string name, Vector3 pos, bool isOnsite, bool isDead){
		GameObject fatherPath = new GameObject();
		fatherPath.transform.position=Vector3.zero;
		
		DrawAreas drawSc = fatherPath.AddComponent<DrawAreas>();
		drawSc.editableAreas=true;
		drawSc.isOnsite = isOnsite;
		drawSc.isDead = isDead;
		
		fatherPath.name = name;
		
		for(int i=0; i<3; i++)
		{
			
			if(i==0){
				pos.x+=5f;
			}else
			if(i==1){
				pos.x+=10f;
			}else
			if(i>=2){
				pos.x+=7.5f;
				pos.z+=5f;
			}
			
			// Shoot a ray from the mouse position into the world
			Ray worldRay = new Ray(pos, Vector3.down);
			RaycastHit hit;
			
			if (Physics.Raycast(worldRay, out hit, Mathf.Infinity))
			{
				if(hit.transform)// && hit.transform.name=="map")
				{
					dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					dot.transform.localScale = new Vector3(3f,0.01f,3f);
					
					Vector3 hitPos = hit.point;
					
					hitPos.y +=2f;
					
					// Place the prefab at correct position (position of the hit).
					dot.transform.position = new Vector3(pos.x, hitPos.y, pos.z);
					dot.transform.parent = fatherPath.transform;// hit.transform;
					// Mark the instance as dirty because we like dirty
					EditorUtility.SetDirty(dot);
					
					drawSc.myDotObjects.Add(dot);
					
					if(i<1){
						dot.name="start";
					}else{
						dot.name="point";
					}
					
					dot.AddComponent<PointTool_AreaPathManager>();
				}
			}
		}
	}

	void CreatePath(string name, Vector3 pos, bool isOnsite){
		GameObject fatherPath = new GameObject();
		fatherPath.transform.position=Vector3.zero;
		
		DrawAreas drawSc = fatherPath.AddComponent<DrawAreas>();
		drawSc.editableAreas=true;
		drawSc.isOnsite = isOnsite;
		drawSc.isPath = true;

		fatherPath.name = name;
		
		for(int i=0; i<2; i++)
		{
			
			if(i==0){
				pos.x+=5f;
			}else
			if(i>=1){
				pos.x+=15f;
			}
			
			// Shoot a ray from the mouse position into the world
			Ray worldRay = new Ray(pos, Vector3.down);
			RaycastHit hit;
			
			if (Physics.Raycast(worldRay, out hit, Mathf.Infinity))
			{
				if(hit.transform)// && hit.transform.name=="map")
				{
					dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					dot.transform.localScale = new Vector3(3f,0.01f,3f);
					
					Vector3 hitPos = hit.point;
					
					hitPos.y +=2f;
					
					// Place the prefab at correct position (position of the hit).
					dot.transform.position = new Vector3(pos.x, hitPos.y, pos.z);
					dot.transform.parent = fatherPath.transform;// hit.transform;
					// Mark the instance as dirty because we like dirty
					EditorUtility.SetDirty(dot);
					
					drawSc.myDotObjects.Add(dot);
					
					if(i<1){
						dot.name="start";
					}else{
						dot.name="point";
					}
					
					dot.AddComponent<PointTool_AreaPathManager>();
				}
			}
		}
	}

}
