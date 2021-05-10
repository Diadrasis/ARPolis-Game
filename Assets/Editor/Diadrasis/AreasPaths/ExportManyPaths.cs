using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//@Stathis edition 28/06/2015

public class ExportManyPaths : MonoBehaviour {

	private string XmlModelsTagName;
	private static List<GameObject> points=new List<GameObject>();
	private static List<GameObject> pointsOrdered=new List<GameObject>();
	public static XmlDocument pointsXml;
	
	
	//[MenuItem ("Diadrasis/Path/Export All Paths to txt")]
	
	public static void ExportScene_toManyPaths(){
		List<GameObject> monopatia = new List<GameObject>();
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];

		foreach(GameObject g in gs){
			if(g.name.StartsWith("Path")){
				monopatia.Add(g);
			}
		}
		//print(monopatia.Count);

		//Start righting the xml
		string path = "paths for editorXml.txt";
		TextWriter sw = new StreamWriter(path);

		sw.Write(xmlHeader());

		for(int i=0; i<monopatia.Count; i++){	//print ("i="+i);
			//onoma monopatiou me noumero
			monopatia[i].name = monopatia[i].name + "_" + i.ToString();
			string pathName = monopatia[i].name;

			sw.Write(lineStartPath(pathName));
			
			//bres tin arxi tou monopatiou
			GameObject point0=monopatia[i].transform.FindChild("start").gameObject;

			if(monopatia[i].name.Contains("NoLimit"))
			{
				point0.name="startNoLimit";
			}

			//clear list from previous path
			points.Clear();

			point0.name = pathName +" - " + point0.name;
			//balto proto sto monopati
			points.Add(point0);

			Transform[] _points = monopatia[i].GetComponentsInChildren<Transform>();

			foreach(Transform point in _points){
				if(point.name.Contains("point")){
					points.Add(point.gameObject);
				}
			}

			//clear list from previous path
			pointsOrdered.Clear();

			//tranfer to ordered list according to distance
			pointsOrdered.Add (point0);
			GameObject curPoint=point0;
			curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x), curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z));

			int num=points.Count;
			for (int a=0; a<num; a++){
				points.Remove(curPoint);
				pointsOrdered.Add (curPoint);
				if (a>0){
					curPoint.name=pathName + '-' + "point_" + a.ToString();
				}
				
				if (a<num-1){
					GameObject nextPoint=FindNearestPoint(curPoint,points);
					curPoint=nextPoint;
					nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x), nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z));
				}
			}


			//Debug.Log (points.Count);

			sw.Flush();
			
//			for (int z=1; z<=num; z++){
//				sw.WriteLine(xmlTransform(pointsOrdered[z]));
//			}

			for (int a=1;a<num;a++){
				sw.Write(xmlTransformStartPath(pointsOrdered[a]));
				sw.Flush();
				sw.Write(xmlTransformEndPath(pointsOrdered[a+1]));
				sw.Flush();
			}

			sw.Write(lineEndPath(pathName));

		}

		sw.Write(xmlFooter());
		sw.Flush();
		if(monopatia.Count==1){
			Debug.Log (monopatia.Count+" μονοπάτι εξήχθη με επιτυχία !");
		}else{
			Debug.Log (monopatia.Count+" μονοπάτια εξήχθησαν με επιτυχία !");
		}

		sw.Close ();

		//anoikse to arxeio
		Application.OpenURL(path);

//		myPaths=monopatia;

	}



	public static void ExportScene_toOnePath(){
		List<GameObject> monopatia = new List<GameObject>();
		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		foreach(GameObject g in gs){
			if(g.name.StartsWith("Path")){
				monopatia.Add(g);
			}
		}
		//print(monopatia.Count);
		
		//Start righting the xml
		string path = "paths for dataXml.txt";
		TextWriter sw = new StreamWriter(path);
		
//		sw.Write(xmlHeader());


		sw.Write(lineStartPath("of path"));
		
		for(int i=0; i<monopatia.Count; i++){	//print ("i="+i);
			//bres tin arxi tou monopatiou
			GameObject point0=monopatia[i].transform.FindChild("start").gameObject;

			if(monopatia[i].name.Contains("NoLimit"))
			{
				point0.name="startNoLimit";
			}

			//clear list from previous path
			points.Clear();
			
//			point0.name = pathName +" - " + point0.name;
			//balto proto sto monopati
			points.Add(point0);
			
			Transform[] _points = monopatia[i].GetComponentsInChildren<Transform>();
			
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
				sw.Write(xmlTransformStartPath(pointsOrdered[a]));
				sw.Flush();
				sw.Write(xmlTransformEndPath(pointsOrdered[a+1]));
				sw.Flush();
			}
			
			
		}

		sw.Write(lineEndPath("of path"));
		

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


//	static List<GameObject> myPaths = new List<GameObject>();
//
//	void OnDrawGizmos()
//	{
//		//Visual. Not used in movement
//		Debug.DrawLine(myPaths[0].transform.localPosition,myPaths[myPaths.Count-1].transform.localPosition);
//	}

	private static string lineStartPath(string onomaPath){
		StringBuilder s = new StringBuilder();
		s.AppendLine(" ");
		s.AppendLine("<!--____________________ start of "+onomaPath+" ____________________-->");
		s.AppendLine("<path>");
		return s.ToString();	
	}

	private static string lineEndPath(string onomaPath){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</path>");
		s.AppendLine("<!--____________________ end of "+onomaPath+" ____________________-->");
		s.AppendLine(" ");
		return s.ToString();	
	}

	private static string xmlTransformPath(GameObject go){
		StringBuilder s = new StringBuilder();	
		s.AppendLine("<point name=\"" + go.name + "\"" + " X=\"" + go.transform.position.x + "\"" + " Y=\"0\"" + " Z=\"" + go.transform.position.z  + "\" />");			
		return s.ToString();
	}
	
	
	private static string xmlHeader(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<paths>");
		return s.ToString();	
	}
	
	private static string xmlFooter(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</paths>");
		return s.ToString();	
	}
	
	
	private static string xmlTransformStartPath(GameObject go){
		Transform t = go.transform;
		StringBuilder s = new StringBuilder();
		if(go.name.Contains("NoLimit")){
			s.AppendLine("<segment limits=\"off\">");
		}else{
			s.AppendLine("<segment limits=\"on\">");
		}
		go.name="start";
		s.AppendLine("<start name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");			
		return s.ToString();
	}
	
	private static string xmlTransformEndPath(GameObject go){
		Transform t = go.transform;
		StringBuilder s = new StringBuilder();
		s.AppendLine("<finish name=\"" + go.name + "\"" + " x=\"" + t.position.x + "\" y=\"" + t.position.z  + "\" />");	
		s.AppendLine("</segment>");	
		return s.ToString();
	}
	
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
}	
