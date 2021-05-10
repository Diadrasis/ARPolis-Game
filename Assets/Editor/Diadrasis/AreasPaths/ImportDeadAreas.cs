using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System;

public class ImportDeadAreas : MonoBehaviour {
	public static string modelPath="editorPaths/";
	public static string XmlPointsTagName;
	public static XmlDocument dataXml;
	
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
	
	
	
	//[MenuItem ("Diadrasis/Path/Import Paths From xml")]
	public static void CreateSceneAreas(string tagScene,string xmlName,float height) {
		
		XmlPointsTagName=tagScene;
		
		dataXml = new XmlDocument();
		TextAsset textAsset = (TextAsset) Resources.Load(xmlName);
		string excludedComments;
		excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		dataXml.LoadXml(excludedComments);
		DestroyAreas();
		CreateAreasXML(height);
	}
	
	public static void DestroyAreas(){
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		int p = 0;
		
		if(gs.Length>0){
			foreach(GameObject g in gs){
				if(g.name.StartsWith("Dead") && !g.name.Contains("start") && !g.name.Contains("point")){
					p++;
					DestroyImmediate(g);
				}
			}
			Debug.Log(p+" dead areas deleted");
		}
	}
	
	static void CreateAreasXML(float ypsos){
		
		XmlNodeList pointList = dataXml.SelectNodes("/chronomichani/"+ XmlPointsTagName +"/DeadSpots/deadSpot");
		
		//print(pointList.Count);
		
		if(pointList.Count>0){
			foreach(XmlNode poi in pointList){
				//				myPath=new cPath();
				
				GameObject fatherPath = new GameObject();
				fatherPath.transform.position=Vector3.zero;
				fatherPath.name="Dead";
				
				if(poi.ChildNodes.Count>1){
					XmlNodeList points = poi["points"].ChildNodes;	//print (points.Count);
					
					for(int i=0; i<points.Count; i++){
						
						cPoint myPoint=new cPoint();
						myPoint.position = new Vector3 (float.Parse (points[i].Attributes ["X"].InnerText), float.Parse (points[i].Attributes ["Y"].InnerText), float.Parse (points[i].Attributes ["Z"].InnerText));
						
						if(i==0){
							GameObject start = (GameObject)Instantiate(Resources.Load(modelPath + "start"));
							start.name="start";
							start.transform.parent=fatherPath.transform;
							Vector3 myPos = new Vector3(myPoint.position.x,ypsos,myPoint.position.z);
							start.transform.localPosition=myPos;
						}else{
							GameObject point = (GameObject)Instantiate(Resources.Load(modelPath + "point"));
							point.name="point";
							point.transform.parent=fatherPath.transform;
							Vector3 myPos = new Vector3(myPoint.position.x,ypsos,myPoint.position.z);
							point.transform.localPosition=myPos;
						}
					}
					
				}
				
				//print(poi["point"].ChildNodes.Count);
			}
			
			if(pointList.Count==1){
				Debug.Log (pointList.Count+" Dead περιοχη εισήχθη με επιτυχία !");
			}else{
				Debug.Log (pointList.Count+" Dead περιοχες εισήχθησαν με επιτυχία !");
			}
			
		}else{
			Debug.LogWarning("Δεν υπάρχουν Dead περιοχες στο xml !!!");
		}
	}
	
	//[MenuItem ("Diadrasis/Path/Import Paths From xml")]
	public static void CreateSceneAreas(string tagScene,string xmlName, int TypeArea) {
		
		XmlPointsTagName=tagScene;
		
		dataXml = new XmlDocument();
		TextAsset textAsset = (TextAsset) Resources.Load("XML/"+xmlName);
		string excludedComments;
		excludedComments = Regex.Replace(textAsset.text, "(<!--(.*?)-->)", string.Empty);
		dataXml.LoadXml(excludedComments);
		DestroyAreas();
		
		
		string s = string.Empty;
		
		if (TypeArea==0) {
			Debug.LogWarning("No Dead Areas for Scene Map");
			//			s = "/ActiveAreaForScene/activeArea";
		} else
		if (TypeArea==1) {
			s = "/DeadSpotsOnSite/deadArea";
		}else 
		if (TypeArea==2) {
			s = "/DeadSpots/deadArea";
		}
		CreateAreasXML(s, TypeArea);
	}
	
	static GameObject dot;
	
	static void CreateAreasXML(string nameOfArea, int type){
		
		XmlNodeList pointList = dataXml.SelectNodes("/movement/" + XmlPointsTagName + nameOfArea);// dataXml.SelectNodes("/chronomichani/"+ XmlPointsTagName +"/DeadSpots/deadSpot");
		
		//print(pointList.Count);
		
		if(pointList.Count>0){
			foreach(XmlNode poi in pointList){
				//				myPath=new cPath();

				if(poi.ChildNodes.Count<=1){
					return;
				}
				
				GameObject fatherPath = new GameObject();
				fatherPath.transform.position=Vector3.zero;
				
				DrawAreas drawScript = fatherPath.AddComponent<DrawAreas>();
				drawScript.editableAreas=true;
				drawScript.isDead=true;

				if(type==1){
					fatherPath.name="Dead_OnSite";
					drawScript.isOnsite=true;
				}else
				if(type==2){
					fatherPath.name="Dead_OffSite";
				}
				
				if(poi.ChildNodes.Count>1){
					XmlNodeList points = poi["points"].ChildNodes;	//print (points.Count);
					
					for(int i=0; i<points.Count; i++){
						
						//						cPoint myPoint=new cPoint();
						Vector3 pos = new Vector3 (float.Parse (points[i].Attributes ["X"].InnerText), 200f, float.Parse (points[i].Attributes ["Z"].InnerText));
						
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
				
				//print(poi["point"].ChildNodes.Count);
			}
			
			if(pointList.Count==1){
				Debug.Log (pointList.Count+" Dead περιοχη εισήχθη με επιτυχία !");
			}else{
				Debug.Log (pointList.Count+" Dead περιοχες εισήχθησαν με επιτυχία !");
			}
			
		}else{
			Debug.LogWarning("Δεν υπάρχουν Dead περιοχες στο xml !!!");
		}
	}
}

