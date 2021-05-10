using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using eChrono;

public class gpsPosition : MonoBehaviour {

//	public static Vector2 gpsRefLoc;

	public gpsPosition(){}
	
	/*Calculate position with functions and dimensions*/
	public static Vector2 FindPosition(Vector2 gpsPos){
		float posX=0;
		float posY=0;
		
		//gpsPos = new Vector2(35.337542f, 25.124870f);
//		if (moveSettings.locMode==moveSettings.LocMode.calc){
			/*
			if (gpsPos.y>gpsRefLoc.y){
				posX=CalcPosition(gpsPos.y, gpsRefLoc.x);
			}else{
				posX=-CalcPosition(gpsPos.y, gpsRefLoc.x);
			}
			if (gpsPos.x>gpsRefLoc.x){
				posZ= CalcPosition(gpsRefLoc.y, gpsPos.x);
			}else{
				posZ=-CalcPosition(gpsRefLoc.y, gpsPos.x);
			}
			*/
//		}else if (moveSettings.locMode==moveSettings.LocMode.prop){

			posX =(gpsPos.y - moveSettings.gpsRefLoc.y)*((Mathf.Abs(moveSettings.posPointA.x)+ Mathf.Abs(moveSettings.posPointB.x)))/(Mathf.Abs(moveSettings.gpsPointB.y-moveSettings.gpsPointA.y));
			posX+=moveSettings.gpsOffsetX;
			posY=(gpsPos.x - moveSettings.gpsRefLoc.x)*((Mathf.Abs(moveSettings.posPointA.y)+ Mathf.Abs(moveSettings.posPointB.y)))/(Mathf.Abs(moveSettings.gpsPointB.x-moveSettings.gpsPointA.x));
			posY+=moveSettings.gpsOffsetY;

//		} 
		
		Vector2 pos=new Vector3(posX, posY);
		return pos;		
	}
	
	public static float FindDistance(Vector2 gpsPos){
		float dist;
		Vector3 pos=FindPosition(gpsPos);
		float x=pos.x;
		float z=pos.z;	

		//TODO
		//+= center of scene !!!

		dist=Mathf.Sqrt(Mathf.Pow (x,2) + Mathf.Pow (z,2));
		return dist;
	}

	static float prevPosY=0f;
	static bool isHole;

	public static bool isheightManual;

	public static bool firstEntrance;
	static float ypsosRaycast = 0f;
			
	public static float FindHeight(Vector2 pos){

		//declare 2 values
		float rayHeight=0f;
		float terrainHeight=0f;

		RaycastHit hit;

		//if(Diadrasis.Instance.navMode==Diadrasis.NavMode.onSite){
			ypsosRaycast=200f;
//			firstEntrance=false;
		//}else{
		//	ypsosRaycast=2f;
		//}

		//Debug.Log("ypsosRaycast = "+ypsosRaycast);
		
		//get last y position of person to make a raycast
		prevPosY = Diadrasis.Instance.person.transform.position.y + ypsosRaycast;		//Debug.Log("prevPosY = "+prevPosY);

		//hit down from last y position of player
		Ray downRay = new Ray(new Vector3(pos.x,prevPosY,pos.y), -Vector3.up);

		if (Physics.Raycast(downRay, out hit,Mathf.Infinity,Diadrasis.Instance.menuUI.xartis.rayLayer)){
			//get hit distance and add person height
			rayHeight = (prevPosY - hit.distance) + moveSettings.playerHeight;
		}

		//get terrain height
//		if (Terrain.activeTerrain != null) {
//			//		if (moveSettings.bUseTerrainForHeight)
//			terrainHeight = Terrain.activeTerrain.SampleHeight (new Vector3 (pos.x, 0, pos.y)) + moveSettings.playerHeight + Terrain.activeTerrain.transform.position.y;
//			
//			//if terrain height ! ray height 
//			if(Terrain.activeTerrain != null && terrainHeight!=rayHeight){
//				//find difference
//				float dif = Mathf.Abs(terrainHeight-rayHeight);
//				if(dif>2f && terrainHeight>rayHeight){
//
//					#if UNITY_EDITOR
//					Debug.Log("terrainHeight = "+terrainHeight);
//					#endif
//
//					return terrainHeight;
//				}
//			}
//		}

//		#if UNITY_EDITOR
//		Debug.Log("rayHeight = "+rayHeight);
//		#endif

		if(Diadrasis.Instance.user==Diadrasis.User.onAir){
			return moveSettings.personOnAirAltitude;
		}

		return rayHeight;
	}	

	public static cSnapPosition FindSnapPosition(Vector2 pnt, List<cLineSegment> path){
		//calculate the distances from all line segments
		List<cSnapPosition>  sp=new List<cSnapPosition>();
		for (int i=0; i<path.Count;i++){
			sp.Add(FindSnapPosition(path[i].StartOfLine, path[i].EndOfLine, pnt, path[i].hasPathFreeMove));
//			Debug.Log(path[i].limitsOn);
		}
		//Find the segment with the minimum sqr distance

		float minDist=sp[0].sqrDistance;
		int index=0;
		bool hasLimits = false;

		for (int i=0;i<sp.Count;i++){
			if (sp[i].sqrDistance<minDist){
				minDist=sp[i].sqrDistance;
				index=i;
				hasLimits = sp[i].limitsOn ;
			}
		}
		cSnapPosition snap = new cSnapPosition ();
		snap.position = sp[index].position;
		snap.sqrDistance = minDist;
		snap.limitsOn = hasLimits;
		return snap;		
	}

	public static bool PlayerInsideArea(Vector2 pos, List<cArea> areas){
		//count the odd nodes
		//if odd is inside else is outside
		bool  oddNodes = false;
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
//							Debug.Log(ar.points[i]);
//							MovePerson.currentArea=ar;
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
			
		}else{
			return true; //if no areas set to true to find nearest path
		}

	}



	public static bool PlayerInsideDeadArea(Vector2 pos, List<cDeadSpot> deadAreas){
		//count the odd nodes
		//if odd is inside else is outside
		bool  oddNodes = false;
		if(deadAreas.Count > 0) {
			for (int k=0;k<deadAreas.Count;k++){
				cDeadSpot ar=deadAreas[k];			
				int   i, j = ar.points.Count - 1 ;
				float x = pos.x;
				float z = pos.y;		
				
				for (i = 0; i < ar.points.Count; i++) {
					if (ar.points[i].z < z && ar.points[j].z >= z ||  ar.points[j].z < z && ar.points[i].z >= z) {
						if (ar.points[i].x + (z-ar.points[i].z)/(ar.points[j].z-ar.points[i].z)*(ar.points[j].x-ar.points[i].x) < x) {
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
			
		}else{
			return false; //if no areas
		}
		
	}
	public static bool PlayerInsideDeadArea(out cDeadSpot deadArea, Vector2 pos, List<cDeadSpot> deadAreas){
		//count the odd nodes
		//if odd is inside else is outside
		bool  oddNodes = false;
		if(deadAreas.Count > 0) {
			for (int k=0;k<deadAreas.Count;k++){
				cDeadSpot ar=deadAreas[k];			
				int   i, j = ar.points.Count - 1 ;
				float x = pos.x;
				float z = pos.y;		
				
				for (i = 0; i < ar.points.Count; i++) {
					if (ar.points[i].z < z && ar.points[j].z >= z ||  ar.points[j].z < z && ar.points[i].z >= z) {
						if (ar.points[i].x + (z-ar.points[i].z)/(ar.points[j].z-ar.points[i].z)*(ar.points[j].x-ar.points[i].x) < x) {
							oddNodes=!oddNodes; 
						}
					}
					j=i; 
				}	
				if (oddNodes){
					deadArea = deadAreas[k];
					return true;
				}
			}

			
		}
		// if none returned true then it is  false!
		deadArea = null;
		return oddNodes;
		
	}

	private static cSnapPosition FindSnapPosition(Vector2 v, Vector2 w, Vector2 p, bool hasLimits){
		cSnapPosition sp = new cSnapPosition();
		Vector2 s;
		float d;

		float projection;
		projection=Vector2.Dot(w-v,p-v)/((w-v).sqrMagnitude);		
		if (projection>1){
			s=w;
		}else if (projection<0){
			s=v;
		}else{
			s=projection*(w-v)+v;
		}
		d= (s-p).sqrMagnitude;
		sp.position = s;
		sp.sqrDistance = d;
		sp.limitsOn = hasLimits;
		return sp;
	}


	
}
