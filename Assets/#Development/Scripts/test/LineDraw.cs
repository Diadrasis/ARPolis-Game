using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LineDraw : MonoBehaviour {

	List<Vector3 > pointsMouse = new List<Vector3>();

	List<Vector4> pathLines = new List<Vector4>();

	public List<GameObject> allPoints = new List<GameObject>();

	Vector3 pStart,pCurr,pPrev,pEnd;

	public GameObject prefab;

	public float minDistOfNextPoint = 3f;

	public bool closePath;

	public bool clearDrawing;

	public LayerMask layer;

	// Update is called once per frame
	void Update () {

		Vector3 mPos = Input.mousePosition;

		Ray ray;
		RaycastHit hit = new RaycastHit ();
		
		ray = Camera.main.ScreenPointToRay (mPos);

		Physics.Raycast(ray, out hit, Mathf.Infinity, layer);

		if(hit.transform){
			if(hit.transform.name == "path"){
				return;
			}
		}else{
			return;
		}

//		if(hit.collider){
//			Debug.Log(hit.transform.name);
//			float dist = Vector3.Distance(transform.position, hit.transform.position);
//			Debug.Log(dist+" m");
//			mPos.z=dist;
//		}

		if(Input.GetMouseButtonUp(0)){
			pEnd = hit.point;
			pEnd.y=0.1f;
			if(!pointsMouse.Contains(pEnd)){
				pointsMouse.Add(pEnd);
			}

			GameObject ob = Instantiate(prefab,pEnd, Quaternion.identity) as GameObject;
			ob.name = "path";
			if(allPoints.Count>0 && !allPoints.Contains(ob)){
				allPoints.Add(ob);
			}

//			Debug.Log("pEnd = "+pEnd);
		}

		if(Input.GetMouseButtonDown(0)){
			pStart = hit.point;
			pStart.y=0.1f;
			pointsMouse.Add(pStart);

			GameObject ob = Instantiate(prefab,pStart, Quaternion.identity) as GameObject;
			ob.name = "path";
			if(!allPoints.Contains(ob)){
				allPoints.Add(ob);
			}
		}

		if(Input.GetMouseButton(0)){
			pCurr = hit.point;
//			pCurr.y=0.1f;

			if(!pointsMouse.Contains(pCurr)){
				if(pointsMouse.Count>0){
					foreach(Vector3 p in pointsMouse){
						if(Vector3.Distance(p, pCurr) < minDistOfNextPoint){
							return;
						}
					}
				}
				pointsMouse.Add(pCurr);
				int indx = pointsMouse.IndexOf(pCurr) - 1;
				pPrev = pointsMouse[indx];

				Vector4 p4 = new Vector4(pPrev.x, pPrev.z, pCurr.x, pCurr.z);

				if(!pathLines.Contains(p4)){
					pathLines.Add(p4);
				}

				GameObject ob = Instantiate(prefab,pCurr, Quaternion.identity) as GameObject;
				ob.name = "path";
				if(allPoints.Count>0 && !allPoints.Contains(ob)){
					allPoints.Add(ob);
				}

				DrawLine(pPrev, pCurr, Color.red);
			}
		}

		if(Input.GetKeyDown(KeyCode.S)){
			ExportScene_toOnePath();
		}

		if(Input.GetKeyDown(KeyCode.C)){
			clearDrawing = true;
		}

		if(clearDrawing){
			if(pathLines.Count>0){
				pathLines.Clear();
			}
			if(pointsMouse.Count>0){
				pointsMouse.Clear();
			}

			if(allPoints.Count>0){
				foreach(GameObject p in allPoints){
//					p.SetActive(false);
					DestroyImmediate(p);
				}
				allPoints.Clear();
			}

			clearDrawing=false;
		}
	}

	List<Vector3> pointsOrdered = new List<Vector3>();
	List<Vector3> points = new List<Vector3>();

	void ExportScene_toOnePath(){
		//Start righting the xml
		string path = "Drawing paths for dataXml.txt";
		TextWriter sw = new StreamWriter(path);
		
		//		sw.Write(xmlHeader());
		sw.Write(lineStartPath("of path"));
		
		for(int i=0; i<pathLines.Count; i++){	//print ("i="+i);
			sw.Flush();
			sw.Write(xml_LineStart(pathLines[i]));
			sw.Flush();
			sw.Write(xml_LineEnd(pathLines[i]));
			sw.Flush();
		}

		if(closePath){
			sw.Flush();
			sw.Write(xml_LineStart(pathLines[0]));
			sw.Flush();
			sw.Write(xml_LineEnd(pathLines[0]));
			sw.Flush();
		}
		
		sw.Write(lineEndPath("of path"));
		//		sw.Write(xmlFooter());
		sw.Flush();
		
		sw.Close ();
		
		//anoikse to arxeio
		Application.OpenURL(path);
		
	}

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

	private static string xmlTransform(GameObject go){
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
	
	
	private static string xml_LineStart(Vector4 vStart){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<segment limits=\"off\">");
//		s.AppendLine("<segment limits=\"on\">");

		s.AppendLine("<start name=\"" + "start" + "\"" + " x=\"" + vStart.x + "\" y=\"" + vStart.y  + "\" />");			
		return s.ToString();
	}
	
	private static string xml_LineEnd(Vector4 vEnd){
		StringBuilder s = new StringBuilder();
		s.AppendLine("<finish name=\"" + "end" + "\"" + " x=\"" + vEnd.z + "\" y=\"" + vEnd.w  + "\" />");	
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

	void OnDrawGizmos(){
		if(points.Count>0){
			for (int k=0; k<pointsMouse.Count; k++) {
				Gizmos.color=Color.red;
				if(pPrev!=pCurr){
					Gizmos.DrawLine(pPrev , pCurr);
				}
			}
		}
	}

	public float paxosGrammis = 0.5f;

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
	{
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(paxosGrammis, paxosGrammis);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}
}
