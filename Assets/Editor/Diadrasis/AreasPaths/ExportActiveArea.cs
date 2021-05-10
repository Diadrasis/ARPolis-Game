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


public class ExportActiveArea : MonoBehaviour {
	private string XmlModelsTagName;
	private static List<GameObject> points=new List<GameObject>();
	private static List<GameObject> pointsOrdered=new List<GameObject>();
	public static XmlDocument pointsXml;


	[MenuItem ("MyMenu/Export Active Area")]

	static void ExportSceneActiveArea(){
		//read the first area point
		GameObject point0=GameObject.FindGameObjectWithTag("startActiveArea");
		string areaName = point0.name;
		points.Add(point0);
		foreach(GameObject point in GameObject.FindGameObjectsWithTag ("actAreaPoint")){
			points.Add(point);
		}

		GameObject curPoint=point0;
		curPoint.transform.position=new Vector3(Mathf.Round(curPoint.transform.position.x), curPoint.transform.position.y, Mathf.Round(curPoint.transform.position.z));

		int num=points.Count;
		for (int i=0;i<num;i++){
			points.Remove(curPoint);
			pointsOrdered.Add (curPoint);
			if (i>0){
				curPoint.name=areaName+ "-" + "point" + i.ToString();
			}
			if (i<num-1){
				GameObject nextPoint=FindNearestPoint(curPoint,points);
				nextPoint.transform.position=new Vector3(Mathf.Round(nextPoint.transform.position.x), nextPoint.transform.position.y, Mathf.Round(nextPoint.transform.position.z));
				curPoint=nextPoint;
			}
		}
		//Start righting the xml
		string area = "area" + areaName + ".txt";
		TextWriter sw = new StreamWriter(area);

		// opening point tag
		sw.Write (xmlHeader());
		sw.Flush();

		for (int i=0;i<num;i++){
			sw.Write(xmlTransform(pointsOrdered[i]));
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
		sw.Write(xmlFooter());
		sw.Flush();
		Debug.Log ("Area points exported successfully.");
		sw.Close ();

	}


	private static string xmlTransform(GameObject go){
		StringBuilder s = new StringBuilder();	
		s.AppendLine("<point name=\"" + go.name + "\"" + " X=\"" + go.transform.position.x + "\"" + " Y=\"0\"" + " Z=\"" + go.transform.position.z  + "\" />");			
		return s.ToString();
	}


	private static string xmlHeader(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<points>");
		return s.ToString();	
	}

	private static string xmlFooter(){
		StringBuilder s = new StringBuilder();
		s.AppendLine("</points>");
		return s.ToString();	
	}

	private static GameObject FindNearestPoint(GameObject point, List<GameObject> pointsList){
		int r = (int)Mathf.Floor(Random.Range(0f, (float)(pointsList.Count-1)));
		GameObject closestPoint=pointsList[r];
		float distance=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(closestPoint.transform.position.x,closestPoint.transform.position.z));
		foreach (GameObject pt in pointsList) {
			float dist=Vector2.Distance(new Vector2(point.transform.position.x, point.transform.position.z), new Vector2(pt.transform.position.x,pt.transform.position.z));;
			if ((dist!=0)&&(dist<distance)){
				distance=dist;
				closestPoint=pt;
			}
		}
		return closestPoint;
	}
}	
		