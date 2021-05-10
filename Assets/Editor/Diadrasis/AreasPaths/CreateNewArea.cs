using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateNewArea : MonoBehaviour {

	public static void MakeAreaPoints(int points,float height,float radius,Color xromaStart,Color xromaPoint,Vector3 myPos){
		
		GameObject fatherPath = new GameObject();
		fatherPath.transform.position=Vector3.zero;
		fatherPath.name="Area";
		
		GameObject start = (GameObject)Instantiate(Resources.Load("editorPaths/start"));
		start.name="start";
		start.transform.parent=fatherPath.transform;
		Vector3 pos = start.transform.localPosition;
		pos = myPos; //Random.insideUnitSphere * 15 + myPos;
		pos.y=height;
		start.transform.localPosition=pos;
		
		start.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		start.renderer.sharedMaterial.color = xromaStart;
		
		for(int i=0; i<points; i++){

//			float d  = i*5f/points;
//
//			 angle = d * Mathf.PI * 2f;
//
//			float x = Mathf.Sin(angle) * 5f;
//			float y = Mathf.Cos(angle) * 5f;

			float angle = i * Mathf.PI * 2 / points;
//			var pos = Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

			Vector3 newPos = new Vector3 (Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + myPos;


//			myPos.x+=2f;
//			myPos.z+=2f;
			GameObject point = (GameObject)Instantiate(Resources.Load("editorPaths/point"));
			point.name="point";
			point.transform.parent=fatherPath.transform;
			Vector3 pos2 = point.transform.localPosition;
			pos2 = newPos; //Random.insideUnitSphere * 15 + myPos;
			pos2.y=height;
			point.transform.localPosition=pos2;
			point.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
			point.renderer.sharedMaterial.color = xromaPoint;
		}

	}

	public static void CreateCubeArea(float sideSize,float height,Color xroma,Vector3 myPos){    Debug.Log("height = "+height);

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		cube.GetComponent<BoxCollider>().enabled=false;

		cube.name="SQUARE_AREA";
		Vector3 pos2 = cube.transform.position;
		pos2 = myPos;
		pos2.y = height;
		cube.transform.position = pos2;
		
		cube.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		cube.renderer.sharedMaterial.color = xroma;

		cube.transform.localScale = new Vector3(sideSize, 0.01f, sideSize);
	}
	
	
	public static void CreateCircleArea(float radius,float height,Color xroma,Vector3 myPos){ Debug.Log("height = "+height);

		GameObject sfaira = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sfaira.GetComponent<SphereCollider>().enabled=false;

		sfaira.name="CIRCLE_AREA";

		Vector3 pos2 = sfaira.transform.position;
		pos2 = myPos;
		pos2.y = height;
		sfaira.transform.position = pos2;

		sfaira.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
		sfaira.renderer.sharedMaterial.color = xroma;

		sfaira.transform.localScale = new Vector3(radius, 0.01f, radius);
	}

	public static void SetSfairesPoints(bool isCircle){		
		string checkName = string.Empty;
		float checkHeight = 0f;

		if(isCircle){
			checkHeight=0f;
			checkName = "CIRCLE_AREA";
		}else{
			checkHeight=-0.5f;
			checkName = "SQUARE_AREA";
		}

		List<Transform> sfaires = new List<Transform>();

		GameObject[] gs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		if(gs.Length>0){
			foreach(GameObject g in gs){
				if(g.name == checkName){
					sfaires.Add(g.transform);
				}
			}
			Debug.Log(sfaires.Count+" "+checkName+"s detected");
		}else{
			return ;
		}

		if(sfaires.Count>0)
		{
			foreach(Transform t in sfaires)
			{
				if(!t){continue;}
				
				if(t.gameObject.isStatic){t.gameObject.isStatic=false;}
				
				MeshFilter meshFilter = t.GetComponent<MeshFilter>();
				
				if(!meshFilter){continue;}

				t.GetComponent<MeshRenderer>().enabled=false;
				
				Mesh mesh = meshFilter.sharedMesh;
				
				Vector3[] vertices = mesh.vertices;															
				
				List<Vector3> simeia = new List<Vector3>();
				
				if(vertices.Length>0){
					foreach(Vector3 p in vertices){
						if(p.y==checkHeight){
							if(!simeia.Contains(p)){
								simeia.Add(p);
							}
						}
					}
				}
				
				Debug.Log("vertsices are "+vertices.Length);
				Debug.Log("final vertsices are "+simeia.Count);
				
				if(simeia.Count>0){
					List<GameObject> simeiaTr = new List<GameObject>();
					
					foreach(Vector3 p in simeia){
						if(p.y==checkHeight){
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

					if(simeiaTr.Count>0)
					{
						for(int i=0; i<simeiaTr.Count; i++){
							if(i==0){
								simeiaTr[i].name = "start";
							}else
							if(i>0){
								simeiaTr[i].name = "point";
							}
						}
					}

					t.parent=null;
				}
			}
		}
	}

	public static void CreateAreasFromBoxColliders(GameObject target, int addSize)
	{

		BoxCollider[] boxs = target.GetComponentsInChildren<BoxCollider>();

		Debug.LogWarning("All boxcolliders are "+boxs.Length);

		List<Transform> ts = new List<Transform>();

		if(boxs.Length>0){
			foreach(BoxCollider box in boxs)
			{
				Transform t = box.transform;
				if(!t){continue;}
				
				if(t.gameObject.isStatic){t.gameObject.isStatic=false;}
				
				MeshFilter meshFilter = t.GetComponent<MeshFilter>();
				
				if(!meshFilter){continue;}

				Mesh mesh = meshFilter.sharedMesh;

				if(!mesh){continue;}

				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

				cube.transform.parent = t.transform;
				cube.transform.localEulerAngles = Vector3.zero;
				
				cube.GetComponent<BoxCollider>().enabled=false;
				
				cube.name="SQUARE_AREA";
				Vector3 pos2 = cube.transform.localPosition;
				pos2 = box.center;
				cube.transform.localPosition = pos2;
				
				cube.renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));
				cube.renderer.sharedMaterial.color = Color.blue;

				Vector3 size = box.size;

				float x = Mathf.Abs(size.x) + addSize;
				float y = Mathf.Abs(size.y);
				float z = Mathf.Abs(size.z) + addSize;
				
				cube.transform.localScale = new Vector3(x,y,z);

				cube.transform.parent=null;

				ts.Add(cube.transform);

			}


			GameObject gb = new GameObject();
			gb.name = "Group_SquareAreas";
			gb.transform.position = Vector3.zero;

//			Debug.Log("areas are "+ts.Count);
		

			//Exclude duplicates.
			ts.Sort(new Vector2Comparer());


//			Debug.Log("areas are "+ts.Count);

			for(int i=0; i<ts.Count; i++){//(Transform t in ts){
				ts[i].parent = gb.transform;
			}

		}

	}



}

class Vector2Comparer : IComparer<Transform>
{
	public int Compare(Transform a, Transform b)
	{
		if (a.position.z < b.position.z)
			return -1;
		if (a.position.z == b.position.z)
		{
			if (a.position.x == b.position.x)
				return 0;
			if (a.position.x < b.position.x)
				return -1;
		}
		return 1;
	}
}
