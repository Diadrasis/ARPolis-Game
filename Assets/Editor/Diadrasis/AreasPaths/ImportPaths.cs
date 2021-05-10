using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System;
using eChrono;

public class ImportPaths: MonoBehaviour {

	public static string modelPath="editorPaths/";
	public static string XmlPointsTagName;
	public static XmlDocument dataXml;
	public static List<cPath> myPaths;

	#region HIDE

	public class cPoint{
		private string _name;
		private string _prefab;
		private Vector3 _position;

		public string name   {
			get{return _name;}
			set{_name = value;}
		}

		public string prefab {
			get { return _prefab;}
			set { _prefab = value; }
		}

		public Vector3 position {
			get { return _position;}
			set { _position = value;}
		}

		public cPoint(){}
	}

	public class cPath {
		private string _name;

		public string name   {
			get{return _name;}
			set{_name = value;}
		}
	}
	
	//[MenuItem ("Diadrasis/Path/Import Paths From xml")]
	public static void CreateScenePaths(string tagScene,string xmlName,float height) {

		XmlPointsTagName=tagScene;

		dataXml = new XmlDocument();
		TextAsset textAsset = (TextAsset) Resources.Load(xmlName);
		string excludedComments;
		excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		dataXml.LoadXml(excludedComments);
		DestroyPaths();
		CreatePathsXML(height);
	}
	
	public static void DestroyPaths(){
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int p = 0;
		if(gs.Length>0){
			foreach(GameObject g in gs){
				if(g.name.StartsWith("Path") && !g.name.Contains("start") && !g.name.Contains("point")){
					p++;
					DestroyImmediate(g);
				}
			}
			if(p==0){
				Debug.Log("zero paths deleted");
			}else{
				Debug.Log(p+" paths are succesfully deleted from the scene!");
			}
		}
	}
	
	static void CreatePathsXML(float ypsos){

//		cPath myPath;
//		myPaths = new List<cPath>();
		
		XmlNodeList pointList = dataXml.SelectNodes("/chronomichani/"+ XmlPointsTagName +"/paths/path");

		//print("paths are "+pointList.Count);

		if(pointList.Count>0){
			for(int p=0; p<pointList.Count; p++){//(XmlNode poi in pointList){

				GameObject fatherPath = new GameObject();
				fatherPath.transform.position=Vector3.zero;
				fatherPath.name="Path";

				XmlNodeList segmentList=pointList[p].ChildNodes;

				//print("path_"+p+" have "+segmentList.Count+" points");

				for(int i=0; i<segmentList.Count; i++){// (XmlNode segmentNode in segmentList){
					cLineSegment seg= new cLineSegment();
					float segsx=float.Parse(segmentList[i]["start"].Attributes["x"].Value);
					float segsy=float.Parse(segmentList[i]["start"].Attributes["y"].Value);
					seg.StartOfLine=new Vector2(segsx,segsy);

					float segfx=float.Parse(segmentList[i]["finish"].Attributes["x"].Value);
					float segfy=float.Parse (segmentList[i]["finish"].Attributes["y"].Value);
					seg.EndOfLine=new Vector2(segfx,segfy);

					if(i==0){
						GameObject start = (GameObject)Instantiate(Resources.Load(modelPath + "start"));
						start.name="start";
						start.transform.parent=fatherPath.transform;
						start.transform.localPosition = new Vector3(seg.StartOfLine.x,ypsos,seg.StartOfLine.y);

						if (segmentList[i]["start"].Attributes ["limits"] != null) { //check if exists
							if (segmentList[i]["start"].Attributes ["limits"].Value == "off") {
								fatherPath.name="Path_NoLimit";
							}
						}

						GameObject point = (GameObject)Instantiate(Resources.Load(modelPath + "point"));
						point.name="point";
						point.transform.parent=fatherPath.transform;
						point.transform.localPosition = new Vector3(seg.EndOfLine.x,ypsos,seg.EndOfLine.y);
					}else{
						GameObject point = (GameObject)Instantiate(Resources.Load(modelPath + "point"));
						point.name="point";
						point.transform.parent=fatherPath.transform;
						point.transform.localPosition = new Vector3(seg.EndOfLine.x,ypsos,seg.EndOfLine.y);
					}
				}


//				if(poi.ChildNodes.Count>0){
//					XmlNodeList points = poi.ChildNodes;
//					foreach(XmlNode p in points){
//
//
//
//						cPoint myPoint=new cPoint();
//						myPoint.position = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
//						myPoint.name = p.Attributes["name"].InnerText;
//
//
//
//						if(myPoint.name.Contains("start")){
//							GameObject start = (GameObject)Instantiate(Resources.Load(modelPath + "start"));
//							start.name="start";
//							start.transform.parent=fatherPath.transform;
//							start.transform.localPosition=myPoint.position;
//						}else
//						if(myPoint.name.Contains("point")){
//							GameObject point = (GameObject)Instantiate(Resources.Load(modelPath + "point"));
//							point.name="point";
//							point.transform.parent=fatherPath.transform;
//							point.transform.localPosition=myPoint.position;
//						}
//
//					}
//				}

				//print(poi["point"].ChildNodes.Count);
			}

			if(pointList.Count==1){
				Debug.Log (pointList.Count+" μονοπάτι εισήχθη με επιτυχία !");
			}else{
				Debug.Log (pointList.Count+" μονοπάτια εισήχθησαν με επιτυχία !");
			}

		}else{
			Debug.LogWarning("Δεν υπάρχουν μονοπάτια στο xml !!!");
			Debug.LogWarning("check xml name !!!");
		}
	}

	#endregion

	public static void CreateScenePaths(string tagScene,string xmlName,int TypeArea) {
		
		XmlPointsTagName=tagScene;
		
		dataXml = new XmlDocument();
		TextAsset textAsset = (TextAsset) Resources.Load("XML/"+xmlName);
		string excludedComments;
		excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		dataXml.LoadXml(excludedComments);
		DestroyPaths();

		string s = string.Empty;
		
		if (TypeArea==0) {
			Debug.LogWarning("No Paths for Scene Map");
		} else
		if (TypeArea==1) {
			s = "/pathOnSite";
		}else 
		if (TypeArea==2) {
			s = "/path";
		}
		CreatePathsXML(s, TypeArea);
	}

	static GameObject dot;

	static void CreatePathsXML(string nameOfArea, int type){
		
		//		cPath myPath;
		//		myPaths = new List<cPath>();
		
		XmlNodeList pointList = dataXml.SelectNodes("/movement/" + XmlPointsTagName + nameOfArea);// dataXml.SelectNodes("/chronomichani/"+ XmlPointsTagName +"/paths/path");
		
		//print("paths are "+pointList.Count);

		//if paths
		if(pointList.Count>0){
			//for every paths
			for(int p=0; p<pointList.Count; p++){//(XmlNode poi in pointList){

				//get lines
				XmlNodeList segmentList=pointList[p].ChildNodes;

				//if no lines.. skip
				if(segmentList.Count<=0){
					return;
				}

				Debug.Log("path lines are "+segmentList.Count);
				
				//list to store points
				List<Vector3> positions = new List<Vector3>();

				//a vector to check if previus position is the same with current position
				//if is the same then is the same path else create new path parent (is not near)
				Vector3 prevStart = Vector3.zero;

				//a list to store every line
				List<cLineSegment> myLines = new List<cLineSegment>();

				//for every line in xml
				for(int i=0; i<segmentList.Count; i++){

					//create new line
					cLineSegment seg= new cLineSegment();

					//get start point
					float segsx=float.Parse(segmentList[i]["start"].Attributes["x"].Value);
					float segsy=float.Parse(segmentList[i]["start"].Attributes["y"].Value);
					Vector3 startPos = new Vector3(segsx, 200f, segsy);

					//get end point
					float segfx=float.Parse(segmentList[i]["finish"].Attributes["x"].Value);
					float segfy=float.Parse (segmentList[i]["finish"].Attributes["y"].Value);
					Vector3 endPos = new Vector3(segfx, 200f, segfy);

					Vector3 nextStartPos = Vector3.zero;
					int nextIndx=0;
					if(i<segmentList.Count-1){
						nextIndx=i+1;

					}else{
						nextIndx = segmentList.Count-1;
					}

					//get next start point
					float segNx=float.Parse(segmentList[nextIndx]["start"].Attributes["x"].Value);
					float segNy=float.Parse(segmentList[nextIndx]["start"].Attributes["y"].Value);
					nextStartPos = new Vector3(segNx, 200f, segNy);

//					Debug.LogWarning(A);
//					Debug.LogWarning(B);

					if(nextStartPos != endPos || i==0){

						//create parent object to hold all points
						GameObject fatherPath = new GameObject();
						//set its position to center
						fatherPath.transform.position=Vector3.zero;
						
						//add script to visualize path on instatiation of points
						DrawAreas drawScript = fatherPath.AddComponent<DrawAreas>();
						drawScript.editableAreas=true;
						drawScript.isPath=true;

						if (segmentList[i].Attributes ["limits"] != null) { //check if exists
							if (segmentList[i].Attributes ["limits"].Value == "on") {
								drawScript.hasPathFreeMove=false;
							}else{
								drawScript.hasPathFreeMove=true;
							}
						}
						
						//determine path type
						if(type==0){
							Debug.LogWarning("No Paths for Scene Map");
							fatherPath.name="Offsite? or Onsite?";
						}else
						if(type==1){
							fatherPath.name="PathOnsite";
							drawScript.isOnsite=true;
						}else
						if(type==2){
							fatherPath.name="PathOffsite";
						}

						for(int a=0; a<2; a++)
						{
							Vector3 pos = Vector3.zero;

							if(a==0){
								pos = startPos;
							}else{
								pos = endPos;
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
									
									drawScript.myDotObjects.Add(dot);
									
									if(a<1){
										if(drawScript.hasPathFreeMove){
											dot.name="start";
										}//NoLimit
										else{
											dot.name="startNoLimit";
										}
									}else{
										if(drawScript.hasPathFreeMove){
											dot.name="point";
										}//NoLimit
										else{
											dot.name="pointNoLimit";
										}
									}
									
									dot.AddComponent<PointTool_AreaPathManager>();
								}
							}
						}

						if(positions.Count>0){
							for(int b=0; b<positions.Count; b++)
							{
								Vector3 pos = positions[b];
								
								
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
										
										drawScript.myDotObjects.Add(dot);

										dot.name="point";
										
										dot.AddComponent<PointTool_AreaPathManager>();
									}
								}
							}
						}

						positions.Clear();

					}else{
						positions.Add(endPos);
					}
				}

//				if(positions.Count>0){
//
//					for(int i=0; i<positions.Count; i++){
//						
//						Vector3 pos = positions[i];
//
//						// Shoot a ray from the mouse position into the world
//						Ray worldRay = new Ray(pos, Vector3.down);
//						RaycastHit hit;
//						
//						if (Physics.Raycast(worldRay, out hit, Mathf.Infinity))
//						{
//							if(hit.transform)// && hit.transform.name=="map")
//							{
//								dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//								dot.transform.localScale = new Vector3(3f,0.01f,3f);
//								
//								Vector3 hitPos = hit.point;
//								
//								hitPos.y +=2f;
//								
//								// Place the prefab at correct position (position of the hit).
//								dot.transform.position = new Vector3(pos.x, hitPos.y, pos.z);
//								dot.transform.parent = fatherPath.transform;// hit.transform;
//								// Mark the instance as dirty because we like dirty
//								EditorUtility.SetDirty(dot);
//								
//								drawScript.myDotObjects.Add(dot);
//								
//								if(i<1){
//									dot.name="start";
//								}else{
//									dot.name="point";
//								}
//								
//								dot.AddComponent<DummyButton>();
//							}
//						}
//						
//						
//					}
//
//				}

			}
			
			if(pointList.Count==1){
				Debug.Log (pointList.Count+" μονοπάτι εισήχθη με επιτυχία !");
			}else{
				Debug.Log (pointList.Count+" μονοπάτια εισήχθησαν με επιτυχία !");
			}
			
		}else{
			Debug.LogWarning("Δεν υπάρχουν μονοπάτια στο xml !!!");
			Debug.LogWarning("check xml name !!!");
		}
	}
	
}

