using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//for xml
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using eChrono;
using CnControls;


public class CreateArea_Test : MonoBehaviour {

	public GameObject points;

	public List<Transform> allPoints = new List<Transform>();
	List<Transform> allPointsTemp = new List<Transform>();
	public List<Vector2> allPos = new List<Vector2>();
	List<Vector2> allPosTemp = new List<Vector2>();

	public Transform targetStart;
	public TextAsset areaXml;

	public List<cArea> myAreas = new List<cArea>();
	List<cLineSegment> allPerimetroi=new List<cLineSegment>();
	public XmlDocument areaDoc;

	#region READ XML



	void ReadXml(){

		if(!areaXml){return;}
		  
		//load data xml files
		areaDoc = new XmlDocument();
		string excludedComments = Regex.Replace(areaXml.text, "(<!--(.*?)-->)", string.Empty);
		areaDoc.LoadXml(excludedComments);

		moveSettings.activeAreas=new List<cArea>();
		
		XmlNodeList areaList = areaDoc.SelectNodes ("/ActiveAreas/activeArea");
		
		if(areaList.Count>0)
		{
			foreach (XmlNode area in areaList) {							
				if (area.Attributes ["status"] != null) {
					if (area.Attributes ["status"].Value == "on") {
						cArea myArea = new cArea();
						myArea.OnomaPerioxis=area["name"].InnerText;
						//myArea.center=new Vector2(float.Parse(area["center"]["x"].InnerText),float.Parse(area["center"]["z"].InnerText));
						//myArea.radius=float.Parse(area["radius"].InnerText);
						
						XmlNodeList psList = area["points"].ChildNodes;
						List<Vector3> ps = new List<Vector3> ();
						foreach (XmlNode p in psList) {
							if (p.Name == "point") {
								Vector3 v = new Vector3 (float.Parse (p.Attributes ["X"].InnerText), float.Parse (p.Attributes ["Y"].InnerText), float.Parse (p.Attributes ["Z"].InnerText));
								ps.Add (v);
							} 
						}
						myArea.Simeia=ps;

						List<cLineSegment> segments=new List<cLineSegment>();

						XmlNodeList segmentList = area["perimetros"].ChildNodes;

						if(segmentList.Count>0)
						{
							foreach (XmlNode segmentNode in segmentList){
								cLineSegment seg= new cLineSegment();
								float segsx=float.Parse(segmentNode["start"].Attributes["x"].Value);
								float segsy=float.Parse(segmentNode["start"].Attributes["y"].Value);
								seg.StartOfLine=new Vector2(segsx,segsy);
								float segfx=float.Parse(segmentNode["finish"].Attributes["x"].Value);
								float segfy=float.Parse (segmentNode["finish"].Attributes["y"].Value);
								seg.EndOfLine=new Vector2(segfx,segfy);
								if (segmentNode.Attributes ["limits"] != null) { //check if exists
									if (segmentNode.Attributes ["limits"].Value == "on") {
										seg.hasPathFreeMove=true;
									}else{
										seg.hasPathFreeMove=false;
									}
								}
								//					Debug.Log(seg.limitsOn);
								segments.Add(seg);
							}
							
							myArea.PerimetrosLines=segments;


							#if UNITY_EDITOR
							Debug.LogWarning("has "+myArea.PerimetrosLines.Count+" points for path");
							#endif
						}


						moveSettings.activeAreas.Add(myArea);
					}

				}
			}
		}


		if(moveSettings.activeAreas.Count>0){
			foreach(cArea ar in moveSettings.activeAreas){
				if(ar.PerimetrosLines.Count>0){
					allPerimetroi.AddRange(ar.PerimetrosLines);
				}
			}
		}

		Debug.LogWarning("Remove from Lines Move Limits");

		if(allPerimetroi.Count>0){
			foreach(cLineSegment l in allPerimetroi){
				l.hasPathFreeMove=false;
			}
		}

		Debug.LogWarning("lines are "+allPerimetroi.Count);
	}

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

	void ImportAreasFromXML(){
		
		XmlNodeList pointList = areaDoc.SelectNodes("/ActiveAreas/activeArea");
		
		//print(pointList.Count);
		
		if(pointList.Count>0){
			foreach(XmlNode poi in pointList){
				//				myPath=new cPath();
				
				GameObject fatherPath = new GameObject();
				fatherPath.transform.position=Vector3.zero;
				fatherPath.name="Area";
				
				if(poi.ChildNodes.Count>1){
					XmlNodeList points = poi["points"].ChildNodes;	//print (points.Count);
					
					for(int i=0; i<points.Count; i++){
						
						cPoint myPoint=new cPoint();
						myPoint.position = new Vector3 (float.Parse (points[i].Attributes ["X"].InnerText), float.Parse (points[i].Attributes ["Y"].InnerText), float.Parse (points[i].Attributes ["Z"].InnerText));
						
						if(i==0){
							GameObject start = (GameObject)Instantiate(Resources.Load("editorPaths/" + "start"));
							start.name="start";
							start.transform.parent=fatherPath.transform;
							Vector3 myPos = new Vector3(myPoint.position.x,0f,myPoint.position.z);
							start.transform.localPosition=myPos;
							start.transform.localScale = Vector3.one/3f;
						}else{
							GameObject point = (GameObject)Instantiate(Resources.Load("editorPaths/" + "point"));
							point.name="point";
							point.transform.parent=fatherPath.transform;
							Vector3 myPos = new Vector3(myPoint.position.x,0f,myPoint.position.z);
							point.transform.localPosition=myPos;
							point.transform.localScale = Vector3.one/3f;
						}
					}
					
				}
				
				//print(poi["point"].ChildNodes.Count);
			}
			
			if(pointList.Count==1){
				Debug.Log (pointList.Count+" περιοχη εισήχθη με επιτυχία !");
			}else{
				Debug.Log (pointList.Count+" περιοχες εισήχθησαν με επιτυχία !");
			}
			
		}else{
			Debug.LogWarning("Δεν υπάρχουν περιοχες στο xml !!!");
		}
	}



	#endregion
	

	#region DEFAULT Functions

	public GameObject[] boxCollParents;

	void GetSetColliders(){
		if(boxCollParents.Length>0)
		{

			for(int i=0; i<boxCollParents.Length; i++)
			{
				boxCollParents[i].isStatic=false;

				BoxCollider[] boxes = boxCollParents[i].GetComponentsInChildren<BoxCollider>();

				if(boxes.Length>0)
				{
					foreach(BoxCollider b in boxes)
					{
						if(!sfaires.Contains(b.transform))
						{
							MeshFilter meshFilter = b.GetComponent<MeshFilter>();
							
							if(!meshFilter){continue;}

							if(b.gameObject.isStatic){b.gameObject.isStatic=false;}

							sfaires.Add(b.transform);
						}
					}
				}
			}
		}

//		SetSfairesPoints();
	}

	public List<Transform> sfaires = new List<Transform>();

	void SetSfairesPoints(){
		if(sfaires.Count>0)
		{
			foreach(Transform t in sfaires)
			{
				if(!t){continue;}

				if(t.gameObject.isStatic){t.gameObject.isStatic=false;}

				MeshFilter meshFilter = t.GetComponent<MeshFilter>();

				if(!meshFilter){continue;}

				Mesh mesh = meshFilter.mesh;

				Vector3[] vertices = mesh.vertices;
		//		float radius = Vector3.Distance(transform.position, vertices[0]);

				List<Vector3> simeia = new List<Vector3>();
				List<GameObject> simeiaTr = new List<GameObject>();

				if(vertices.Length>0){
					foreach(Vector3 p in vertices){
						if(p.y==0f){
							if(!simeia.Contains(p)){
								simeia.Add(p);
							}
						}
					}
				}

				Debug.Log("vertsices are "+vertices.Length);
				Debug.Log("final are "+simeia.Count);

				if(simeia.Count>0){
					foreach(Vector3 p in simeia){
						if(p.y==0f){
							GameObject gb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
							gb.transform.localScale = Vector3.one/10f;
							gb.transform.parent = t;
							gb.transform.localPosition = p;
							if(!simeiaTr.Contains(gb)){
								simeiaTr.Add(gb);
							}
						}
					}


					t.name = "Area";

					for(int i=0; i<simeiaTr.Count; i++){
						if(i==0){
							simeiaTr[i].name = "start";
						}else
						if(i>0){
							simeiaTr[i].name = "point";
						}

					}
				}
			}
		}
	}

	IEnumerator Start () {

		GetSetColliders();

		yield return new WaitForSeconds(1f);

		SetSfairesPoints();

		ReadXml();

		yield return new WaitForSeconds(1f);

		GetDeadAreas();

		yield return new WaitForSeconds(1f);

		ConnectAllPerimetrous();

		Debug.Log("areas = "+moveSettings.activeAreas.Count);
		Debug.Log("dead areas = "+moveSettings.deadSpots.Count);
		Debug.Log("perimetros lines are "+allPerimetroi.Count);
	}

	//move with left joystick
	public void JoyMove(){

		if(!Camera.main){return;}
		
		//get input from left joystick
		Vector2 inputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		//if is up or down (mpros/piso kinisi)
		if(Mathf.Abs(inputVector.y) > Mathf.Abs(inputVector.x)){
			//get direction from camera view (αξονα z)
			Vector3 fwd=Camera.main.transform.TransformDirection(Vector3.forward);
			//the next pos will be at where the camera is looking + joystick (mpros/piso) * speed + now person position
			Vector2 pos2d = new Vector2(fwd.x,fwd.z) * inputVector.y * Time.deltaTime * 15f + new Vector2(transform.position.x, transform.position.z);
			moveTo (pos2d);
		}
		//if is left or right (plagia kinisi)
		else{
			//get direction from camera view (αξονα x)
			Vector3 fwd=Camera.main.transform.TransformDirection(Vector3.right);
			//the next pos will be at where the camera is looking + joystick (left/right) * speed + now person position
			Vector2 pos2d = new Vector2(fwd.x,fwd.z) * inputVector.x * Time.deltaTime * 15f + new Vector2(transform.position.x, transform.position.z);
			//check if next pos is in path or area
			moveTo (pos2d);
		}
	}

	bool isInArea(Vector2 pos)
	{

		if (moveSettings.activeAreas.Count>0f && gpsPosition.PlayerInsideArea(pos,moveSettings.activeAreas)){
			return true;
		}

		return false;
	}

	float maxDistance;
	public float anoxiFromMonopati = 0.1f;
	float minX,maxX,minZ,maxZ;
	cSnapPosition sp;
	bool hasPaths,hasDeadPerimterous;

	void moveTo(Vector2 myPos){
		
		//αν είναι εκτός ενεργών περιοχών
		if (!isInArea(myPos))
		{
			//αν υπάρχει perimetros
			if(allPerimetroi.Count>0){
				//δήλωνουμε οτι υπάρχει μονοπατι
				hasPaths=true;
				
				maxDistance = 2f;
				
				//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
				sp=gpsPosition.FindSnapPosition(myPos,allPerimetroi);
			}else{
				//δηλώνουμε οτι δεν υπάρχει μονοπάτι
				hasPaths=false;
			}
			
			//αν έχουμε μονοπάτι και είναι εντός μέγιστης αποδεκτής απόστασης
			if (hasPaths && sp.sqrDistance < maxDistance * maxDistance)
			{
				//θέση πάνω στο μονοπάτι
				Vector2 snapPos= sp.position;
				
				//μετακίνηση προς σε αυτή τη θέση
				transform.position = new Vector3(snapPos.x, 0f, snapPos.y);
				
				//Debug.Log("limits are "+gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath).limitsOn);
				
				//αν απο το xml είναι ενεργό το περιθώριο κίνησης εκτός μονοπατιού
				//με όριο κάποια απόσταση apo το μονοπατι
				if(gpsPosition.FindSnapPosition(myPos,allPerimetroi).limitsOn)
				{
					//get pos from joy
					Vector3 nPos=new Vector3(myPos.x, 0f, myPos.y);
					//TODO
					//move person with lerp
					transform.position = nPos;
					//get limits from path
					minX = snapPos.x - anoxiFromMonopati;   //Debug.Log(minX);
					maxX = snapPos.x + anoxiFromMonopati;	//Debug.Log(maxX);
					minZ = snapPos.y - anoxiFromMonopati;	//Debug.Log(minZ);
					maxZ = snapPos.y + anoxiFromMonopati;	//Debug.Log(maxZ);
					//keep movement in limits
					transform.position=new Vector3(Mathf.Clamp(transform.position.x , minX , maxX), 0f, Mathf.Clamp(transform.position.z , minZ , maxZ));
				}
			}
			else//αν δεν έχουμε μονοπάτι ή είναι εκτός μέγιστης αποδεκτής απόστασης
			{

			}
		}
		else//αν είναι εντός ενεργής περιοχής
		if (isInArea(myPos))
	{
			//if deadspots exists in xml
			if (moveSettings.deadSpots.Count>0)
			{
				cDeadSpot myDeadArea = new cDeadSpot();

				if(gpsPosition.PlayerInsideDeadArea(out myDeadArea, myPos,moveSettings.deadSpots))
				{
					//θέτουμε οτι είναι εντος απαγορευμένης περιοχής
					Debug.LogWarning("Dead Area");
					//dont move


					//αν υπάρχει perimetros
					if(myDeadArea.DeadPerimetros.Count>0){
						//δήλωνουμε οτι υπάρχει μονοπατι
						hasDeadPerimterous=true;
						
						maxDistance = 0.2f;
						
						//και βρίσκουμε το κοντινότρο σημείο πάνω stin perimetro
						sp=gpsPosition.FindSnapPosition(myPos,myDeadArea.DeadPerimetros);
					}else{
						//δηλώνουμε οτι δεν υπάρχει μονοπάτι
						hasDeadPerimterous=false;
					}

					//αν έχουμε dead perimetro και είναι εντός μέγιστης αποδεκτής απόστασης
					if (hasDeadPerimterous && sp.sqrDistance < maxDistance * maxDistance)
					{
						//θέση πάνω στο μονοπάτι
						Vector2 snapPos= sp.position;
						
						//μετακίνηση προς σε αυτή τη θέση
						transform.position = new Vector3(snapPos.x, 0f, snapPos.y);
						
						//Debug.Log("limits are "+gpsPosition.FindSnapPosition(myPos,moveSettings.playerPath).limitsOn);
						
						//αν απο το xml είναι ενεργό το περιθώριο κίνησης εκτός μονοπατιού
						//με όριο κάποια απόσταση apo το μονοπατι
						if(gpsPosition.FindSnapPosition(myPos,myDeadArea.DeadPerimetros).limitsOn)
						{
							Debug.Log("dead limits are ON");
							
							//get pos from joy
							Vector3 nPos=new Vector3(myPos.x, 0f, myPos.y);
							//TODO
							//move person with lerp
							transform.position = nPos;
							//get limits from path
							minX = snapPos.x - anoxiFromMonopati;   //Debug.Log(minX);
							maxX = snapPos.x + anoxiFromMonopati;	//Debug.Log(maxX);
							minZ = snapPos.y - anoxiFromMonopati;	//Debug.Log(minZ);
							maxZ = snapPos.y + anoxiFromMonopati;	//Debug.Log(maxZ);
							//keep movement in limits
							transform.position=new Vector3(Mathf.Clamp(transform.position.x , minX , maxX), 0f, Mathf.Clamp(transform.position.z , minZ , maxZ));
						}else{
							Debug.Log("dead limits are OFF");
						}
					}
					else//αν δεν έχουμε μονοπάτι ή είναι εκτός μέγιστης αποδεκτής απόστασης
					{
						
					}

				}else
				//αν είναι εκτός νεκρής περιοχής
				//και άρα εντός ενεργής
				{
					//επόμενη θέση απο joystick
					Vector3 nPos=new Vector3(myPos.x,0f, myPos.y);	
					transform.position= nPos;
				}
			}
			else//if no deadspots in xml move free in area
			{
				//επόμενη θέση απο joystick
				Vector3 nPos=new Vector3(myPos.x, 0f, myPos.y);	
				transform.position= nPos;
			}
			
		}
	}
	


	
	void LateUpdate () {

		JoyMove();

	}

	#endregion

	#region ENTOLES

	public Transform trA,trAend,trB,trBend,targetResult;

	void Cross(){ 
		List<Vector3> points = new List<Vector3>();
		Vector3 A = trA.position;
		Vector3 B = trAend.position;
		Vector3 C = trB.position;
		Vector3 D =trBend.position;
		points.Add(A);
		points.Add(B);
		points.Add(C);
		points.Add(D);

		Vector3 result;

		Vector3 dirA = B-A;
		Vector3 dirB = D-C;

    

		if(Math3d.LineLineIntersection(out result,A,dirA,C,dirB)){// .AreLineSegmentsCrossing(A,B,C,D)){

			Vector3 dir1 = A-result;
			Vector3 dir2 = B-result;

			float dotA = Vector3.Dot(dir1.normalized,dir2.normalized);

			Vector3 dir3 = C-result;
			Vector3 dir4= D-result;
			
			float dotB = Vector3.Dot(dir3.normalized,dir4.normalized);
			Debug.LogWarning("dotA = "+dotA);
			Debug.LogWarning("dotB = "+dotB);

			//an ta 2 dianysmata koitane to ena to allo
			if(dotA<0 && dotB<0){
				Debug.Log("point is on the line");
			}
			else//an exoun antithetes fores
			{
				Debug.LogWarning("point is out of the 4 points area");
				//an den exoun tin idia fora mporoume na doume
				//an to simeio diastaurosis einai pano se kapoia apo tis 2 grammes
				if(dotA<0){
					Debug.Log("point is on the line A");
				}
				if(dotB<0){
					Debug.Log("point is on the line B");
				}

				if(dotA==0 && dotB==0){
					Debug.Log("lines are kathetes with one point in common");
				}
			}

//			Debug.Log("CROSSING to "+result);
//			Debug.Log("crossing 2 = "+LineIntersectionPoint(new Vector2(A.x,A.z),new Vector2(B.x,B.z),new Vector2(C.x,C.z),new Vector2(D.x,D.z)));

			targetResult.position = result;
		}
	}

	
	List<cLineSegment> crossLines = new List<cLineSegment>();
	List<cLineSegment> inAreaLines = new List<cLineSegment>();
	List<cLineSegment> inAreaDeadLines = new List<cLineSegment>();
	public List<Vector3> crossPoints = new List<Vector3>();

	List<cLineSegment> synexomenes = new List<cLineSegment>();

//	List<cLineSegment> errorLines = new List<cLineSegment>();

	public Transform inAreaTargetTransform;

	void ConnectAllPerimetrous(){

		if(allPerimetroi.Count>0){

			crossLines.Clear();
			inAreaLines.Clear();
			crossPoints.Clear();

//			//find crossing lines and connect them at croosing point
			for(int i=0; i<allPerimetroi.Count; i++)
			{
				//for every line
				for(int a=0; a<allPerimetroi.Count; a++)
				{
					if(a!=i)
					{

						Vector3 crossPoint = Vector3.zero;

						//TODO check if cross point is not in list
						if(isLineCrossing(out crossPoint, allPerimetroi[i], allPerimetroi[a], true, moveSettings.activeAreas)==1)
						{
							if(!crossPoints.Contains(crossPoint)){
								crossPoints.Add(crossPoint);
						

								if(!crossLines.Contains(allPerimetroi[i]))
								{
									crossLines.Add(allPerimetroi[i]);
								}

								if(!crossLines.Contains(allPerimetroi[a]))
								{
									crossLines.Add(allPerimetroi[a]);
								}
							}
						}
					}
				}
			}

			//Remove all lines that are entirly into an area
			if(allPerimetroi.Count>0)
			{
				for(int x=0; x<allPerimetroi.Count; x++)
				{
//					if(!crossLines.Contains(allPerimetroi[x]))
//					{
						foreach(cArea area in moveSettings.activeAreas)
						{
							if(!area.PerimetrosLines.Contains(allPerimetroi[x]))
							{
								if(!inAreaLines.Contains(allPerimetroi[x]))
								{
									if(pointInsideArea(allPerimetroi[x].StartOfLine, area) && pointInsideArea(allPerimetroi[x].EndOfLine, area))
									{
										inAreaLines.Add(allPerimetroi[x]);
									}
								}
							}
						}
//					}
				}
			}

			#if UNITY_EDITOR
			Debug.Log("all crossing lines are "+crossLines.Count);
			Debug.Log("all in area lines are "+inAreaLines.Count);
			#endif
			

			if(inAreaLines.Count>0){
				foreach(cLineSegment l in inAreaLines){
					if(allPerimetroi.Contains(l)){
						allPerimetroi.Remove(l);
					}
				}
			}

			Debug.Log("final lines are "+allPerimetroi.Count);

			synexomenes.Clear();

			for(int i=0; i<allPerimetroi.Count; i++)
			{
				for(int a=0; a<allPerimetroi.Count; a++)
				{
					if(a!=i){

						Vector3 crossPoint = Vector3.zero;
						//TODO check if cross point is not in list
						if(isLineCrossing(out crossPoint, allPerimetroi[i], allPerimetroi[a], false, moveSettings.activeAreas)==0)
						{
//							Debug.Log("SYNEXOMENES");

							if(!synexomenes.Contains(allPerimetroi[i])){
								synexomenes.Add(allPerimetroi[i]);
							}

							if(!synexomenes.Contains(allPerimetroi[a])){
								synexomenes.Add(allPerimetroi[a]);
							}

						}else{
							//Debug.Log("error -> "+isLineCrossing(out crossPoint, allPerimetroi[i], allPerimetroi[a], false, moveSettings.activeAreas));
						}
					}
				}

				GameObject A = new GameObject();
				A.transform.position = new Vector3(allPerimetroi[i].StartOfLine.x, 0f, allPerimetroi[i].StartOfLine.y);
				A.name = "line_"+i+"_start";

				GameObject B = new GameObject();
				B.transform.position = new Vector3(allPerimetroi[i].EndOfLine.x, 0f, allPerimetroi[i].EndOfLine.y);
				B.name = "line_"+i+"_end";
			}

			Debug.Log("synexomenes are "+synexomenes.Count);
		}
	}

	void GetDeadAreas(){
		if(moveSettings.activeAreas.Count>0)
		{
			inAreaDeadLines.Clear();
			moveSettings.deadSpots.Clear();
			List<cArea> smallAreas = new List<cArea>();

			foreach(cArea area in moveSettings.activeAreas)
			{
				if(moveSettings.activeAreas.Contains(area))
				{
					cArea biggerArea = new cArea();
					if(areaInsideArea(out biggerArea, area, moveSettings.activeAreas))
					{
						if(!smallAreas.Contains(area))
						{
							smallAreas.Add(area);
						}

						cDeadSpot dSpot = new cDeadSpot();

						dSpot.name = area.OnomaPerioxis;
						dSpot.points = area.Simeia;
						dSpot.center = area.CenterOfArea;
						dSpot.DeadPerimetros = biggerArea.DeadLines;

						if(dSpot.DeadPerimetros.Count>0){
							foreach(cLineSegment d in dSpot.DeadPerimetros){
								d.hasPathFreeMove = false;
							}
						}

						moveSettings.deadSpots.Add(dSpot);

						foreach(cLineSegment line in area.PerimetrosLines){
							inAreaDeadLines.Add(line);
						}

						#if UNITY_EDITOR
						Debug.LogWarning(area.OnomaPerioxis+" is now dead area of "+biggerArea.OnomaPerioxis);
						#endif
					}
				}
			}

			if(smallAreas.Count>0){
				for(int i=0; i<moveSettings.activeAreas.Count; i++){
					foreach(cArea r in smallAreas){
						if(r.OnomaPerioxis == moveSettings.activeAreas[i].OnomaPerioxis){
							moveSettings.activeAreas.Remove(moveSettings.activeAreas[i]);
						}
					}
				}
			}

			if(inAreaDeadLines.Count>0){
				foreach(cLineSegment deadLine in inAreaDeadLines){
					if(allPerimetroi.Contains(deadLine)){
						allPerimetroi.Remove(deadLine);
					}

				}
			}
		}
	}


	#endregion

	#region GIZMOS

	void OnDrawGizmos(){//Debug.LogWarning("OnDrawGizmos 1");
		//show active areas
		if (moveSettings.activeAreas.Count > 0) {	//	Debug.LogWarning("OnDrawGizmos 2");
			for (int l=0; l<moveSettings.activeAreas.Count; l++) {
				for (int m=0; m<moveSettings.activeAreas[l].Simeia.Count-1; m++) {
					Gizmos.color=Color.blue;
					Gizmos.DrawLine (new Vector3 (moveSettings.activeAreas[l].Simeia [m].x, 0f, moveSettings.activeAreas[l].Simeia [m].z), new Vector3 (moveSettings.activeAreas [l].Simeia [m + 1].x, 0f, moveSettings.activeAreas[l].Simeia [m + 1].z));
				}
				//close the loop
				Gizmos.color=Color.blue;
				Gizmos.DrawLine (new Vector3 (moveSettings.activeAreas[l].Simeia [moveSettings.activeAreas[l].Simeia.Count-1].x, 0f, moveSettings.activeAreas[l].Simeia[moveSettings.activeAreas[l].Simeia.Count-1].z), new Vector3 (moveSettings.activeAreas[l].Simeia [0].x, 0f, moveSettings.activeAreas[l].Simeia[0].z));
			}
		}

		//show path perimetros
//		if(allPerimetroi.Count>0){
//			for (int k=0; k<allPerimetroi.Count; k++) {
//				Gizmos.color=Color.green;
//				Gizmos.DrawLine(new Vector3(allPerimetroi[k].StartOfLine.x, 0f,allPerimetroi[k].StartOfLine.y) , new Vector3(allPerimetroi[k].EndOfLine.x, 0f,allPerimetroi[k].EndOfLine.y));
//			}
//		}
////
		if(inAreaDeadLines.Count>0){
			for (int k=0; k<inAreaDeadLines.Count; k++) {
				Gizmos.color=Color.red;
				Gizmos.DrawLine(new Vector3(inAreaDeadLines[k].StartOfLine.x, 0f,inAreaDeadLines[k].StartOfLine.y) , new Vector3(inAreaDeadLines[k].EndOfLine.x, 0f,inAreaDeadLines[k].EndOfLine.y));
			}
		}

//		if(crossLines.Count>0){
//			for (int k=0; k<crossLines.Count; k++) {
//				Gizmos.color=Color.black;
//				Gizmos.DrawLine(new Vector3(crossLines[k].StartOfLine.x, 0f,crossLines[k].StartOfLine.y) , new Vector3(crossLines[k].EndOfLine.x, 0f,crossLines[k].EndOfLine.y));
//			}
//		}
//
//		if(synexomenes.Count>0){
//			for (int k=0; k<synexomenes.Count; k++) {
//				Gizmos.color=Color.white;
//				Gizmos.DrawLine(new Vector3(synexomenes[k].StartOfLine.x, 0f,synexomenes[k].StartOfLine.y) , new Vector3(synexomenes[k].EndOfLine.x, 0f,synexomenes[k].EndOfLine.y));
//			}
//		}

//		if(errorLines.Count>0){
//			for (int k=0; k<errorLines.Count; k++) {
//				Gizmos.color=Color.red;
//				Gizmos.DrawLine(new Vector3(errorLines[k].StartOfLine.x, 0f,errorLines[k].StartOfLine.y) , new Vector3(errorLines[k].EndOfLine.x, 0f,errorLines[k].EndOfLine.y));
//			}
//		}

	}

	#endregion

	#region Math TOOLS

	///find if an area's all points are inside another area
	///and if true make small area dead area of the bigger area
	/// and remove small area from areas list
	bool areaInsideArea(out cArea areaBig, cArea area, List<cArea> areas){
		if(areas.Count>0)
		{
			foreach(cArea perioxi in areas)
			{
				//dont check the same area
				if(area != perioxi)
				{
					//check all points of area if are inside perioxi
					if(area.Simeia.Count>0)
					{
						int count = 0;
						foreach(Vector3 point in area.Simeia)
						{
							Vector2 p = new Vector2(point.x, point.z);
							
							if(pointInsideArea(p,perioxi))
							{
								//count points that are inside perioxi
								count++;
							}
						}
						
						//if all points are inside make area dead area of perioxi
						if(count == area.Simeia.Count){
							perioxi.DeadLines = area.PerimetrosLines;
							//TODO ????? error ?
							//							areas.Remove(area);
							areaBig = perioxi;
							return true;
						}
					}
				}
			}
		}
		areaBig=null;
		return false;
	}

	private  bool pointInsideArea(Vector2 point, List<cArea> areas) {		
		bool  oddNodes = false;
		if(areas.Count > 0) {
			for (int k=0;k<areas.Count;k++){
				cArea ar=areas[k];			
				int   i, j = ar.Simeia.Count - 1 ;
				float x = point.x;
				float z = point.y;		
				
				for (i = 0; i < ar.Simeia.Count; i++) {
					if (ar.Simeia[i].z < z && ar.Simeia[j].z >= z ||  ar.Simeia[j].z < z && ar.Simeia[i].z >= z) {
						if (ar.Simeia[i].x + (z-ar.Simeia[i].z)/(ar.Simeia[j].z-ar.Simeia[i].z)*(ar.Simeia[j].x-ar.Simeia[i].x) < x) {
							oddNodes=!oddNodes; 
						}
					}
					j=i; 
				}	
				if (oddNodes){
					return true;
				}
			}
			// if none returned true then it is  false!
			return oddNodes;
			
		}
		return false;
	}

	private  bool pointInsideArea(Vector2 point, cArea area) {		
		bool  oddNodes = false;
		cArea ar=area;			
		int   i, j = ar.Simeia.Count - 1 ;
		float x = point.x;
		float z = point.y;		
		
		for (i = 0; i < ar.Simeia.Count; i++) {
			if (ar.Simeia[i].z < z && ar.Simeia[j].z >= z ||  ar.Simeia[j].z < z && ar.Simeia[i].z >= z) {
				if (ar.Simeia[i].x + (z-ar.Simeia[i].z)/(ar.Simeia[j].z-ar.Simeia[i].z)*(ar.Simeia[j].x-ar.Simeia[i].x) < x) {
					oddNodes=!oddNodes; 
				}
			}
			j=i; 
		}	
		if (oddNodes){
			return true;
		}
		// if none returned true then it is  false!
		return oddNodes;
		
	}

	bool isLineInArea(out cArea result, cLineSegment line, List<cArea> areas){
		result = new cArea();
		
		if(lineInsideArea(out result, line.StartOfLine, areas) && lineInsideArea(out result, line.EndOfLine, areas)){
			return true;
		}
		
		return false;
	}

	bool lineInsideArea(out cArea result, Vector2 pos, List<cArea> areas){
		//count the odd nodes
		//if odd is inside else is outside
		bool  oddNodes = false;
		
		result = new cArea();
		
		if(areas.Count > 0) {
			for (int k=0;k<areas.Count;k++){
				cArea ar=areas[k];			
				int   i, j = ar.Simeia.Count - 1 ;
				float x = pos.x;
				float z = pos.y;		
				
				for (i = 0; i < ar.Simeia.Count; i++) {
					if (ar.Simeia[i].z < z && ar.Simeia[j].z >= z ||  ar.Simeia[j].z < z && ar.Simeia[i].z >= z) {
						if (ar.Simeia[i].x + (z-ar.Simeia[i].z)/(ar.Simeia[j].z-ar.Simeia[i].z)*(ar.Simeia[j].x-ar.Simeia[i].x) < x) {
							oddNodes=!oddNodes; 
						}
					}
					j=i; 
				}	
				if (oddNodes){
					result = ar;
					if(!ar.Simeia.Contains(new Vector3(pos.x, 0f, pos.y))){
						return true;
					}
				}
			}
			// if none returned true then it is  false!
			return oddNodes;
			
		}
		return oddNodes;
		
	}

	bool sameVectors(Vector2 a,Vector2 b,Vector2 c,Vector2 d){		//Debug.Log("&*************");
		if(a==b || a==c || a==d || b==c || b==d || c==d){
			return true;
		}
		
		return false;
	}

	bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
	{
		Vector2 a = p2 - p1;
		Vector2 b = p3 - p4;
		Vector2 c = p1 - p3;
		
		float alphaNumerator = b.y*c.x - b.x*c.y;
		float alphaDenominator = a.y*b.x - a.x*b.y;
		float betaNumerator  = a.x*c.y - a.y*c.x;
		float betaDenominator  = a.y*b.x - a.x*b.y;
		
		bool doIntersect = true;
		
		if (alphaDenominator == 0 || betaDenominator == 0) {
			doIntersect = false;
		} else {
			
			if (alphaDenominator > 0) {
				if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
					doIntersect = false;
					
				}
			} else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
				doIntersect = false;
			}
			
			if (doIntersect && betaDenominator > 0) {
				if (betaNumerator < 0 || betaNumerator > betaDenominator) {
					doIntersect = false;
				}
			} else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
				doIntersect = false;
			}
		}
		
		return doIntersect;
	}

	Vector3 lerpPointOnLine(Vector3 start, Vector3 end, float percent)
	{
		return (start + percent*(end - start));
	}

	/// <summary>
	/// -1 αν ειναι παραλληλες
	/// 0 αν ειναι συνεχομενες
	/// 1 αν τεμνονται εντος οριων
	/// 2 αν η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
	/// 3 αν τεμνονται νοητα (εκτος οριων) και η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
	///   και το σημειο επαφης ειναι εντος οριων σε μια απο τις 2 γραμμες
	/// 4 αν τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
	int isLineCrossing(out Vector3 resultPoint, cLineSegment lineA, cLineSegment lineB, bool cutLines, List<cArea> perioxes){
		//Line A
		Vector3 Astart = new Vector3(lineA.StartOfLine.x, 0f, lineA.StartOfLine.y);
		Vector3 Aend = new Vector3(lineA.EndOfLine.x, 0f, lineA.EndOfLine.y);
		//Line B
		Vector3 Bstart = new Vector3(lineB.StartOfLine.x, 0f, lineB.StartOfLine.y);
		Vector3 Bend = new Vector3(lineB.EndOfLine.x, 0f, lineB.EndOfLine.y);
		
		//dianysma A
		Vector3 dirA = Aend-Astart;
		//dianysna B
		Vector3 dirB = Bend-Bstart;
		
		//find if 2 lines are crossing
		if(Math3d.LineLineIntersection(out resultPoint,Astart,dirA,Bstart,dirB)){	//Debug.LogWarning(resultPoint);
			
			//dianysma crossin point to start of line A
			Vector3 dir1 = Astart-resultPoint;
			//dianysma crossin point to end of line A
			Vector3 dir2 = Aend-resultPoint;
			
			//get dot product 
			//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
			//η νοητα (με προεκταση) εκτος των οριων της γραμμης
			float dotA = Vector3.Dot(dir1.normalized,dir2.normalized);
			
			//dianysma crossin point to start of line B
			Vector3 dir3 = Bstart-resultPoint;
			//dianysma crossin point to end of line B
			Vector3 dir4= Bend-resultPoint;
			
			//get dot product 
			//για να δουμε αν το σημειο ειναι εντος των οριων της γραμμης
			//η νοητα (με προεκταση) εκτος των οριων της γραμμης
			float dotB = Vector3.Dot(dir3.normalized,dir4.normalized);
			
			//η γραμμες ειναι διαδοχικα συνεχομενες
			if(dotA==0 && dotB==0){
				return 0;
			}else
				//οι γραμμες τεμνονται και το σημειο επαφης
				//βρισκεται εντος των οριων και των 2 γραμμων
			if(dotA==-1 && dotB ==-1){		
				
				if(cutLines)
				{
					Vector2 r = new Vector2(resultPoint.x, resultPoint.z);
					
//					Debug.LogWarning("cutting lines at "+r);
					
					if(perioxes.Count>0)
					{
						foreach(cArea area in perioxes)
						{
							if(!area.PerimetrosLines.Contains(lineA))
							{
								if(pointInsideArea(lineA.StartOfLine, area) && !pointInsideArea(lineA.EndOfLine, area)){
									lineA.StartOfLine = r;
								}else
								if(pointInsideArea(lineA.EndOfLine, area) && !pointInsideArea(lineA.StartOfLine, area)){
									lineA.EndOfLine = r;
								}
							}
							
							if(!area.PerimetrosLines.Contains(lineB))
							{
								if(pointInsideArea(lineB.StartOfLine, area) && !pointInsideArea(lineB.EndOfLine, area)){
									lineB.StartOfLine = r;
								}else
								if(pointInsideArea(lineB.EndOfLine, area) && !pointInsideArea(lineB.StartOfLine, area)){
									lineB.EndOfLine = r;
								}
							}
						}
					}
				}
				
				//temnontai
				return 1;
			}
			else
				//τεμνονται και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
				//και πανω σε ενα απο τα ορια της αλλης
				//δηλαδη η μια γραμμη ακουμπα την μια ακρη της στο ενδιαμεσο της αλλης
			if((dotA==-1 && dotB==0) || (dotA==0 && dotB==-1)){
				return 2;
			}else
				//τεμνονται νοητα και το σημειο επαφης ειναι μεσα στα ορια της μιας γραμμης
				//και πανω σε ενα απο τα ορια της αλλης
				//δηλαδη η μια γραμμη ακουμπα νοητα την μια ακρη της στο ενδιαμεσο της αλλης
			if((dotA==1 && dotB==0) || (dotA==0 && dotB==1)){
				return 3;
			}else
				//τεμνονται νοητα και το σημειο επαφης ειναι εκτος οριων και απο τις 2 γραμμες
			if((dotA==1 && dotB==1) || (dotA==1 && dotB==1)){
				return 4;
			}
			
			//Debug.Log("CROSSING to "+result);
			
//			targetResult.position = resultPoint;
		}


		return -1;
		
	}
	
	
	
	Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
	{
		// Get A,B,C of first line - points : ps1 to pe1
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		float C1 = A1*ps1.x+B1*ps1.y;
		
		// Get A,B,C of second line - points : ps2 to pe2
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;
		float C2 = A2*ps2.x+B2*ps2.y;
		
		// Get delta and check if the lines are parallel
		float delta = A1*B2 - A2*B1;
		if(delta == 0)
			throw new System.Exception("Lines are parallel");
		
		// now return the Vector2 intersection point
		return new Vector2(
			(B2*C1 - B1*C2)/delta,
			(A1*C2 - A2*C1)/delta
			);
	}

	Vector2 findNearest(Vector2 p, List<Vector2> pList){
		int tyxaio = UnityEngine.Random.Range(0,pList.Count-1);
		Vector2 closestPoint = pList[tyxaio];

		Vector2 dir = p-closestPoint;

		float angle = Vector2Angle(dir);
//		float angle = Vector2.Angle(Vector2.up, dir);

		Debug.Log("angle = "+angle);

		foreach(Vector2 v in pList){
			Vector2 newDir = p-v;

			float newAngle = Vector2Angle(newDir);

			if((newAngle!=0f) && (newAngle<angle)){
				angle=newAngle;
				closestPoint = v;
				Debug.Log("final Angle = "+newAngle);
			}
		}

		pList.Remove(closestPoint);

		return closestPoint;

	}

	
	//Convert Vector2 to angle in degrees (0-360)
	float Vector2Angle(Vector2 v){
		if(v.x<0) return 360-(Mathf.Atan2(v.x, v.y)*Mathf.Rad2Deg*-1);
		else return Mathf.Atan2(v.x,v.y)*Mathf.Rad2Deg;
	}
	
	//Convert degrees (0-360) to a Vector2
	Vector2 Angle2Vector(float angle,float power){
		return (new Vector2(Mathf.Sin(angle*Mathf.Deg2Rad),Mathf.Cos(angle*Mathf.Deg2Rad)))*power;
	}

	#endregion
}
