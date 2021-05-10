using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;


//read the model xml and the existing game objects and create
// a data file so that the scene could be created via script


public class ExportPath : MonoBehaviour {
	private string XmlModelsTagName;
	private static List<GameObject> points=new List<GameObject>();
	private static List<GameObject> pointsOrdered=new List<GameObject>();
	public static XmlDocument pointsXml;


	[MenuItem ("MyMenu/Export Path")]

	static void ExportScenePath(){
		GameObject point0=GameObject.FindGameObjectWithTag("startPath");
		string pathName = point0.name;
		points.Add(point0);
		foreach(GameObject point in GameObject.FindGameObjectsWithTag ("gpsPoint")){
			points.Add(point);
		}
		//tranfer to ordered list according to distance
		pointsOrdered.Add (point0);
		GameObject curPoint=point0;
		curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x), curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z));

		int num=points.Count;
		for (int i=0;i<num;i++){
			points.Remove(curPoint);
			pointsOrdered.Add (curPoint);
			if (i>0){
				curPoint.name=pathName + '-' + "point" + i.ToString();
			}

			if (i<num-1){
				GameObject nextPoint=FindNearestPoint(curPoint,points);
				curPoint=nextPoint;
				nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x), nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z));
			}
		}

		//


		//Debug.Log (points.Count);
		//Start righting the xml
		string path = "path" +pathName + ".txt";
		TextWriter sw = new StreamWriter(path);

		// opening point tag
		//sw.Write (xmlHeader());
		sw.Flush();

		for (int i=1;i<num;i++){
			sw.Write(xmlTransformStart(pointsOrdered[i]));
			sw.Flush();
			sw.Write(xmlTransformEnd(pointsOrdered[i+1]));
			sw.Flush();
		}

		/*
		foreach(GameObject point in GameObject.FindGameObjectsWithTag ("gpsPoint")){
			if (points.Count>1){
				sw.Write(xmlTransformStart(point.transform));
				sw.Flush();
				points.Remove(point);
				GameObject go = FindNearestPoint(point, points);			 
				sw.Write(xmlTransformEnd(go.transform));
			}
		}
		*/

		//sw.Write(xmlFooter());
		sw.Flush();
		Debug.Log ("Models exported successfully.");
		sw.Close ();
	}


	private static string xmlTransformStart(GameObject go){
		Transform t = go.transform;
		StringBuilder s = new StringBuilder();
		s.AppendLine("<segment>");
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


	private static string xmlHeader(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<chronomichani>");
		s.AppendLine("<Venetian>");
		s.AppendLine("<Path>");
		s.AppendLine("<Segments>");
		return s.ToString();	
	}

	private static string xmlFooter(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</Segments>");
		s.AppendLine("</Path>");
		s.AppendLine("</Venetian>");
		s.AppendLine("</chronomichani>");
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
		