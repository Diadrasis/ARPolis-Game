using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//@Stathis edition 10/07/2015

public class ExportManyDeadAreas : MonoBehaviour {
	private string XmlModelsTagName;
	private static List<GameObject> points=new List<GameObject>();
	private static List<GameObject> pointsOrdered=new List<GameObject>();
	public static XmlDocument pointsXml;
	
	
	//[MenuItem ("Diadrasis/Path/Export All Paths to txt")]
	
	public static void ExportDeadSpots(){
		List<GameObject> perioxes = new List<GameObject>();
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		foreach(GameObject g in gs){
			if(g.name=="Dead"){
				perioxes.Add(g);
			}
		}
		//print(monopatia.Count);
		
		//Start righting the xml
		string path = "dead_Areas.txt";
		TextWriter sw = new StreamWriter(path);
		
		sw.Write(xmlHeader());
		
		for(int i=0; i<perioxes.Count; i++){
			//onoma monopatiou me noumero
			perioxes[i].name = perioxes[i].name + "_" + i.ToString();
			string areaName =(i+1).ToString() ;
			sw.Write(lineStartPath(areaName));
			
			//bres tin arxi tou monopatiou
			GameObject point0=perioxes[i].transform.FindChild("start").gameObject;
			
			//clear list from previous path
			points.Clear();
			
			point0.name = areaName +" - " + point0.name;
			//balto proto sto monopati
			points.Add(point0);
			
			Transform[] _points = perioxes[i].GetComponentsInChildren<Transform>();
			
			foreach(Transform point in _points){
				if(point.name=="point"){
					points.Add(point.gameObject);
				}
			}
			
			//clear list from previous path
			pointsOrdered.Clear();
			
			//tranfer to ordered list according to distance
			pointsOrdered.Add (point0);
			GameObject curPoint=point0;
			curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x *100f)/100f, curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z *100f)/100f);
			
			int num=points.Count;
			for (int a=0; a<num; a++){
				points.Remove(curPoint);
				pointsOrdered.Add (curPoint);
				if (a>0){
					curPoint.name=areaName + '-' + "point_" + a.ToString();
				}
				
				if (a<num-1){
					GameObject nextPoint=FindNearestPoint(curPoint,points);
					curPoint=nextPoint;
					nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x *100f)/100f, nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z *100f)/100f);
				}
			}
			//Debug.Log (points.Count);
			
			sw.Flush();
			
			for (int z=1; z<=num; z++){
				sw.WriteLine(xmlTransform(pointsOrdered[z]));
			}
			
			sw.Write(lineEndPath(areaName));
			
		}
		
		sw.Write(xmlFooter());
		sw.Flush();
		if(perioxes.Count==1){
			Debug.Log (perioxes.Count+" νεκρη περιοχη εξήχθη με επιτυχία !");
		}else{
			Debug.Log (perioxes.Count+" νεκρες περιοχες εξήχθησαν με επιτυχία !");
		}
		
		sw.Close ();
		
		//anoikse to arxeio
		Application.OpenURL(path);
		
	}
	
	private static string lineStartPath(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine(" ");
		s.AppendLine("<!--____________________ start of Dead Area "+onomaArea+" ____________________-->");
		s.AppendLine("<deadSpot status="+"\"on\""+">");
		s.AppendLine("<name>"+"Dead Area "+onomaArea+"</name>");
		s.AppendLine("<center>");
		s.AppendLine("<x>" + 10 + "</x>");
		s.AppendLine("<z>" + 20 + "</z>"); 
		s.AppendLine("</center>");
		s.AppendLine("<radius>" + 100 + "</radius>");
		s.AppendLine("<points>");
		return s.ToString();	
	}
	
	private static string lineEndPath(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</points>");
		s.AppendLine("</deadSpot>");
		s.AppendLine("<!--____________________ end of Dead Area "+onomaArea+" ____________________-->");
		s.AppendLine(" ");
		return s.ToString();	
	}
	
	private static string xmlTransform(GameObject go){
		StringBuilder s = new StringBuilder();	
		s.AppendLine("<point " + " X=\"" + go.transform.position.x + "\"" + " Y=\"0\"" + " Z=\"" + go.transform.position.z  + "\" />");			
		return s.ToString();
	}
	
	
	private static string xmlHeader(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<DeadSpots>");
		return s.ToString();	
	}
	
	private static string xmlFooter(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</DeadSpots>");
		return s.ToString();	
	}
	
	//	
	//	private static string xmlTransformStart(GameObject go){
	//		Transform t = go.transform;
	//		StringBuilder s = new StringBuilder();
	//		s.AppendLine("<segment>");
	//		s.AppendLine("<start name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");			
	//		return s.ToString();
	//	}
	//	
	//	private static string xmlTransformEnd(GameObject go){
	//		Transform t = go.transform;
	//		StringBuilder s = new StringBuilder();
	//		s.AppendLine("<finish name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");	
	//		s.AppendLine("</segment>");		
	//		return s.ToString();
	//	}
	
	private static GameObject FindNearestPoint(GameObject point, List<GameObject> pointsList){
		int r = (int)Mathf.Floor(Random.Range(0f, (float)(pointsList.Count-1)));
		GameObject closestPoint=pointsList[r];
		float distance=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(closestPoint.transform.position.x,closestPoint.transform.position.z));
		foreach (GameObject pt in pointsList) {
			float dist=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(pt.transform.position.x,pt.transform.position.z));
			if ((dist!=0)&&(dist<distance)){
				distance=dist;
				closestPoint=pt;
			}
		}
		return closestPoint;
	}
	
	//	public static List<GameObject> DrawLinesAeras(){
	//		List<GameObject> perioxes = new List<GameObject>();
	//		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
	//		
	//		foreach(GameObject g in gs){
	//			if(g.name=="Area"){
	//				perioxes.Add(g);
	//			}
	//		}
	//		//print(monopatia.Count);
	//		
	//		
	//		
	//		for(int i=0; i<perioxes.Count; i++){
	//			
	//			//bres tin arxi tou monopatiou
	//			GameObject point0=perioxes[i].transform.FindChild("start").gameObject;
	//			
	//			//clear list from previous path
	//			points.Clear();
	//			
	//			//balto proto sto monopati
	//			points.Add(point0);
	//			
	//			Transform[] _points = perioxes[i].GetComponentsInChildren<Transform>();
	//			
	//			foreach(Transform point in _points){
	//				if(point.name=="point"){
	//					points.Add(point.gameObject);
	//				}
	//			}
	//			
	//			//clear list from previous path
	//			pointsOrdered.Clear();
	//			
	//			//tranfer to ordered list according to distance
	//			pointsOrdered.Add (point0);
	//			GameObject curPoint=point0;
	//			curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x), curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z));
	//			
	//			int num=points.Count;
	//			for (int a=0; a<num; a++){
	//				points.Remove(curPoint);
	//				pointsOrdered.Add (curPoint);
	//				
	//				if (a<num-1){
	//					GameObject nextPoint=FindNearestPoint(curPoint,points);
	//					curPoint=nextPoint;
	//					nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x), nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z));
	//				}
	//			}
	//			
	//		}
	//
	//		return pointsOrdered;
	//
	//		
	//	}
	
	
	
}	
