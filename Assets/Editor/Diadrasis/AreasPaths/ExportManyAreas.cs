using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//@Stathis edition 28/06/2015

public class ExportManyAreas : MonoBehaviour {
	
	private string XmlModelsTagName;
	private static List<GameObject> points=new List<GameObject>();
	private static List<GameObject> pointsOrdered=new List<GameObject>();
	public static XmlDocument pointsXml;
	
	
	//[MenuItem ("Diadrasis/Path/Export All Paths to txt")]
	
	public static void ExportSceneAreas(){
		List<GameObject> perioxes = new List<GameObject>();
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		foreach(GameObject g in gs){
			if(g.name=="Area"){
				perioxes.Add(g);
			}
		}
		Debug.LogWarning(perioxes.Count);
		
		//Start righting the xml
		string areaTxt = "area.txt";
		TextWriter sw = new StreamWriter(areaTxt);
		
		sw.Write(xmlHeader());
		
		for(int i=0; i<perioxes.Count; i++)
		{
			//onoma monopatiou me noumero
			perioxes[i].name = perioxes[i].name + "_" + i.ToString();
			string areaName =(i+1).ToString() ;

			for(int w=0; w<2; w++)
			{

				if(w==0){
					sw.Write(lineStartArea(areaName));
				}else if(w==1){
					sw.Write(lineStartPerimetros(areaName));
				}

				Debug.LogWarning(perioxes[i].name);
				
				//bres tin arxi tou monopatiou
				GameObject point0=perioxes[i].transform.FindChild("start").gameObject;
				
				//clear list from previous path
				points.Clear();
				
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
//						curPoint.name=areaName + '-' + "point_" + a.ToString();
					}
					
					if (a<num-1){
						GameObject nextPoint=FindNearestPoint(curPoint,points);
						curPoint=nextPoint;
						nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x *100f)/100f, nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z * 100f)/100f);
					}


				}

				//add start again to close circle
//				if(w==1){
//					pointsOrdered.Add (point0);
//				}
				//Debug.Log (points.Count);
				
				sw.Flush();

				if(w==0){
					for (int z=1; z<=num; z++){
						sw.WriteLine(xmlTransform(pointsOrdered[z]));
					}
					sw.Write(lineEndPoint(areaName));
					
				}else 
				if(w==1){
					for (int a=1;a<num;a++){
						sw.Write(xmlTransformStart(pointsOrdered[a]));
						sw.Flush();
						sw.Write(xmlTransformEnd(pointsOrdered[a+1]));
						sw.Flush();
					}
					sw.Write(xmlTransformStart(pointsOrdered[num]));
					sw.Flush();
					sw.Write(xmlTransformEnd(pointsOrdered[1]));
					sw.Flush();
					sw.Write(lineEndPerimetros(areaName));
					
				}

			}

			sw.Write(lineEndArea(areaName));
			
		}
		
		sw.Write(xmlFooter());
		sw.Flush();
		if(perioxes.Count==1){
			Debug.Log (perioxes.Count+" περιοχη εξήχθη με επιτυχία !");
		}else{
			Debug.Log (perioxes.Count+" περιοχες εξήχθησαν με επιτυχία !");
		}
		
		sw.Close ();
		
		//anoikse to arxeio
		Application.OpenURL(areaTxt);
		
	}

	public static void ExportScene_AreasPerimetros(){
		List<GameObject> perioxes = new List<GameObject>();
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		foreach(GameObject g in gs){
			if(g.name=="Area"){
				perioxes.Add(g);
			}
		}
		
		//Start righting the xml
		string path = "areas perimetroi.txt";
		TextWriter sw = new StreamWriter(path);
		
		sw.Write(lineStartArea("of path"));
		
		for(int i=0; i<perioxes.Count; i++){	//print ("i="+i);
			
			//bres tin arxi tou monopatiou
			GameObject point0=perioxes[i].transform.FindChild("start").gameObject;
			
			if(perioxes[i].name.Contains("NoLimit"))
			{
				point0.name="startNoLimit";
			}
			
			//clear list from previous path
			points.Clear();
			
			//			point0.name = pathName +" - " + point0.name;
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
			curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x *100f)/100f, curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z * 100f)/100f);
			
			int num=points.Count;
			for (int a=0; a<num; a++){
				points.Remove(curPoint);
				pointsOrdered.Add (curPoint);
				if (a>0){
					//					curPoint.name=pathName + '-' + "point_" + a.ToString();
				}
				
				if (a<num-1){
					GameObject nextPoint=FindNearestPoint(curPoint,points);
					curPoint=nextPoint;
					nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x * 100f)/100f, nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z * 100f)/100f);
				}
			}
			
			sw.Flush();
			
			for (int a=1;a<num;a++){
				sw.Write(xmlTransformStart(pointsOrdered[a]));
				sw.Flush();
				sw.Write(xmlTransformEnd(pointsOrdered[a+1]));
				sw.Flush();
			}
			
		}
		
		sw.Write(lineEndArea("of path"));
		
		//		sw.Write(xmlFooter());
		sw.Flush();
		//		if(monopatia.Count==1){
		//			Debug.Log (monopatia.Count+" μονοπάτι εξήχθη με επιτυχία !");
		//		}else{
		//			Debug.Log (monopatia.Count+" μονοπάτια εξήχθησαν με επιτυχία !");
		//		}
		
		sw.Close ();
		
		//anoikse to arxeio
		Application.OpenURL(path);
		
		//		myPaths=monopatia;
		
	}

	private static string xmlTransformStart(GameObject go){
		Transform t = go.transform;
		StringBuilder s = new StringBuilder();
		if(go.name=="startNoLimit"){
			s.AppendLine("<segment limits=\"off\">");
		}else{
			s.AppendLine("<segment limits=\"on\">");
		}
		go.name="start";
		s.AppendLine("<start name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");			
		return s.ToString();
	}
	
	private static string xmlTransformEnd(GameObject go){
		Transform t = go.transform;
		StringBuilder s = new StringBuilder();
		s.AppendLine("<finish name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");	
		s.AppendLine("</segment>");	
		return s.ToString();
	}

	private static string lineStartArea(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine(" ");
		s.AppendLine("<!--____________________ start of Area "+onomaArea+" ____________________-->");
		s.AppendLine("<activeArea status="+"\"on\""+">");
		s.AppendLine("<name>"+"Active Area "+onomaArea+"</name>");
		s.AppendLine("<points>");
		return s.ToString();	
	}
	
	private static string lineEndPoint(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</points>");
		return s.ToString();	
	}

	private static string lineStartPerimetros(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<perimetros>");
		return s.ToString();	
	}

	private static string lineEndPerimetros(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</perimetros>");
		return s.ToString();	
	}

	private static string lineEndArea(string onomaArea){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</activeArea>");
		s.AppendLine("<!--____________________ end of Area "+onomaArea+" ____________________-->");
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
		s.AppendLine("<ActiveAreas>");
		return s.ToString();	
	}
	
	private static string xmlFooter(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</ActiveAreas>");
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
