using UnityEngine;
using eChrono;
using System.Collections;
using System.Collections.Generic;

public class testInsideArea : MonoBehaviour {

	// Use this for initialization
	List<cArea> areas=new List<cArea>();
	cArea ar1=new cArea();
	cArea ar2=new cArea();

	void Start () {
		List<Vector3> points1 = new List<Vector3>();
		for (int k=0; k<5 ; k++){
			string name="Cube"+k.ToString();
			GameObject go1= GameObject.Find(name);
			points1.Add(go1.transform.position);
		}
		ar1.Simeia=points1;
		areas.Add(ar1);

	
		//ar2=new cArea();
		List<Vector3> points2 = new List<Vector3>();
		for (int k=5; k<10; k++){
			string name="Cube"+k.ToString();
			GameObject go2= GameObject.Find(name);
			points2.Add(go2.transform.position);
		}
		ar2.Simeia=points2;
		areas.Add(ar2);

	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (InsideArea(new Vector2(Camera.main.transform.position.x,Camera.main.transform.position.z), areas)); 
	}

	private  bool InsideArea(Vector2 point, List<cArea> areas) {		
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

		}else{
			return true; //if no areas
		}
	}
}
